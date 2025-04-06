/*
 * Imports data from a GEDCOM file to the People collection. The GEDCOM file
 * is first converted to an XML file so it's easier to parse, then individuals
 * are parsed from the file, and then families.  GEDCOM Ids are converted to GUIDs.
 *
 * More information on the GEDCOM format is at http://en.wikipedia.org/wiki/Gedcom
 * and http://homepages.rootsweb.ancestry.com/~pmcbride/gedcom/55gctoc.htm
 * 
 * This class has a few modifications to use _frel and _mrel which are common proprietary tags
 * used by other programs for adoption
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace FamilyShowLib;

/// <summary>
/// Import data from a GEDCOM file to the Person collection.
/// </summary>
public class GedcomImport
{
  #region fields

  // The collection to add entries.
  private PeopleCollection _people;
  private SourceCollection _sources;
  private RepositoryCollection _repositories;

  // Convert the GEDCOM file to an XML file which is easier 
  // to parse, this contains the GEDCOM info in an XML format.
  private XmlDocument _doc;

  #endregion

  /// <summary>
  /// Populate the people collection with information from the GEDCOM file.
  /// </summary>
  public bool Import(PeopleCollection peopleCollection, SourceCollection sourceCollection, RepositoryCollection repositoryCollection, string gedcomFilePath, bool disableCharacterCheck)
  {
    // Clear current content.
    peopleCollection.Clear();
    sourceCollection.Clear();
    repositoryCollection.Clear();

    // First convert the GEDCOM file to an XML file so it's easier to parse,
    // the temp XML file is deleted when importing is complete.
    string xmlFilePath = Path.GetTempFileName();
    bool loaded = false;  // if user canceled the load

    try
    {
      _people = peopleCollection;
      _sources = sourceCollection;
      _repositories = repositoryCollection;

      // Convert the GEDCOM file to a temp XML file.
      GedcomConverter.ConvertToXml(gedcomFilePath, xmlFilePath, true, disableCharacterCheck);

      _doc = new XmlDocument();
      _doc.Load(xmlFilePath);

      // Get list of people.
      XmlNodeList list = _doc.SelectNodes("/root/INDI");

      // Import data from the temp XML file to the people collection.
      ImportPeople(list);
      ImportFamilies();
      ImportSources();
      ImportRepositories();

      // The collection requires a primary-person, use the first
      // person added to the collection as the primary-person.
      if (peopleCollection.Count > 0)
      {
        peopleCollection.Current = peopleCollection[0];
      }

      // Update IDs to match Family.Show standards
      UpdatePeopleIDs();
      UpdateSourceIDs();
      UpdateRepositoryIDs();

      loaded = true;


    }
    finally
    {
      //Delete the temp XML file.
      File.Delete(xmlFilePath);
    }

    return loaded;
  }

  /// <summary>
  /// Imports the individuals (INDI tags) from the GEDCOM XML file.
  /// </summary>
  private void ImportPeople(XmlNodeList list)
  {

    foreach (XmlNode node in list)
    {

      // Create a new person that will be added to the collection.
      Person person = new()
      {
        // Import details about the person.
        FirstName = GetNames(node),
        LastName = GetSurname(node)
      };

      // If no name or surname, call them unknown rather than an empty string
      if (string.IsNullOrEmpty(person.FirstName) && string.IsNullOrEmpty(person.LastName))
      {
        person.FirstName = Properties.Resources.Unknown;
      }

      person.Suffix = GetSuffix(node);
      person.Id = GetId(node);
      person.Gender = GetGender(node);
      person.Restriction = GetRestriction(node);

      ImportBirth(person, node, _doc);
      ImportDeath(person, node, _doc);
      ImportBurial(person, node, _doc);
      ImportCremation(person, node, _doc);
      ImportOccupation(person, node, _doc);
      ImportReligion(person, node, _doc);
      ImportEducation(person, node, _doc);

      ImportPhotosAttachments(person, node);
      ImportNote(person, node);

      _people.Add(person);
    }
  }

  private void UpdatePeopleIDs()
  {
    foreach (Person p in _people)
    {
      string oldpId = p.Id;
      p.Id = Guid.NewGuid().ToString();

      foreach (Relationship r1 in p.Relationships)
      {
        foreach (Relationship r2 in r1.RelationTo.Relationships)
        {
          if (oldpId == r2.PersonId)
          {
            r2.PersonId = p.Id;
          }
        }
      }

      // Now we have a GUID, we can save a story for people who have notes
      if (!string.IsNullOrEmpty(p.Note))
      {
        p.Story = new Story();
        string FileName = new StringBuilder(App.ReplaceEncodedCharacters(p.FullName + "(" + p.Id + ")")).Append(".rtf").ToString();
        p.Story.Save(p.Note, FileName);
      }

    }
  }

  /// <summary>
  /// Imports the source (SOUR tags) from the GEDCOM XML file.
  /// </summary>
  private void ImportSources()
  {
    // Get list of people.
    XmlNodeList list = _doc.SelectNodes("/root/SOUR");

    foreach (XmlNode node in list)
    {
      Source source = new()
      {
        // Import details about the person.
        Id = GetId(node),
        SourceName = GetValue(node, "TITL"),
        SourceAuthor = GetValue(node, "AUTH"),
        SourcePublisher = GetValue(node, "PUBL"),
        SourceNote = ImportEventNote(node, "NOTE", _doc),
        SourceRepository = GetValueId(node, "REPO").Replace("@", string.Empty)
      };

      _sources.Add(source);
    }
  }

  private void UpdateSourceIDs()
  {
    string[,] sourceIDMap = new string[2, _sources.Count];

    int i = 0;

    foreach (Source s in _sources)
    {
      sourceIDMap[0, i] = s.Id.Replace("@", string.Empty);
      s.Id = "S" + (i + 1).ToString();
      sourceIDMap[1, i] = s.Id;
      i++;
    }

    foreach (Person p in _people)
    {
      //only ever replace an id once
      bool replacedMarriage = false;
      bool replacedDivorce = false;
      bool replacedBirth = false;
      bool replacedDeath = false;
      bool replacedCremation = false;
      bool replacedBurial = false;
      bool replacedOccupation = false;
      bool replacedEducation = false;
      bool replacedReligion = false;

      for (int z = 0; z < _sources.Count; z++)
      {

        string s1 = sourceIDMap[0, z];
        string s2 = sourceIDMap[1, z];

        if (p.HasSpouse)
        {
          foreach (Relationship rel in p.Relationships)
          {
            if (rel.RelationshipType == RelationshipType.Spouse)
            {
              SpouseRelationship spouseRel = (SpouseRelationship)rel;

              if (spouseRel.MarriageSource != null && !replacedMarriage)
              {
                spouseRel.MarriageSource = s2;
                replacedMarriage = true;
              }

              if (spouseRel.DivorceSource != null && !replacedDivorce)
              {
                spouseRel.DivorceSource = s2;
                replacedDivorce = true;
              }

            }
          }

        }

        if (p.BirthSource == s1)
        {
          if (s2 != null && !replacedBirth)
          {
            p.BirthSource = s2;
            replacedBirth = true;
          }
        }
        if (p.DeathSource == s1)
        {
          if (s2 != null && !replacedDeath)
          {
            p.DeathSource = s2;
            replacedDeath = true;
          }
        }
        if (p.OccupationSource == s1)
        {
          if (s2 != null && !replacedOccupation)
          {
            p.OccupationSource = s2;
            replacedOccupation = true;
          }
        }
        if (p.EducationSource == s1)
        {
          if (s2 != null && !replacedEducation)
          {
            p.EducationSource = s2;
            replacedEducation = true;
          }
        }
        if (p.ReligionSource == s1)
        {
          if (s2 != null && !replacedReligion)
          {
            p.ReligionSource = s2;
            replacedReligion = true;
          }
        }

        if (p.CremationSource == s1)
        {
          if (s2 != null && !replacedCremation)
          {
            p.CremationSource = s2;
            replacedCremation = true;
          }
        }

        if (p.BurialSource == s1)
        {
          if (s2 != null && !replacedBurial)
          {
            p.BurialSource = s2;
            replacedBurial = true;
          }
        }
      }

    }
  }

  /// <summary>
  /// Imports the source (REPO tags) from the GEDCOM XML file.
  /// </summary>
  private void ImportRepositories()
  {
    // Get list of people.
    XmlNodeList list = _doc.SelectNodes("/root/REPO");

    foreach (XmlNode node in list)
    {
      Repository repository = new()
      {
        // Import details about the person.
        Id = GetId(node),
        RepositoryName = GetValue(node, "NAME"),
        RepositoryAddress = GetValue(node, "ADDR")
      };
      _repositories.Add(repository);
    }
  }

  private void UpdateRepositoryIDs()
  {
    string[,] repositoryIDMap = new string[2, _repositories.Count];

    int i = 0;

    foreach (Repository r in _repositories)
    {
      repositoryIDMap[0, i] = r.Id.Replace("@", string.Empty);
      r.Id = "R" + (i + 1).ToString();
      repositoryIDMap[1, i] = r.Id;
      i++;
    }


    foreach (Source s in _sources)
    {
      bool replaced = false;  //only replace an id once!

      for (int z = 0; z < _repositories.Count; z++)
      {
        if (s.SourceRepository == repositoryIDMap[0, z])
        {
          if (repositoryIDMap[1, z] != null && replaced == false)
          {
            s.SourceRepository = repositoryIDMap[1, z];
            replaced = true;
          }
        }
      }
    }
  }

  /// <summary>
  /// Imports the families (FAM tags) from the GEDCOM XML file.
  /// </summary>
  private void ImportFamilies()
  {
    // Get list of families.
    XmlNodeList list = _doc.SelectNodes("/root/FAM");

    foreach (XmlNode node in list)
    {
      // Get family (husband, wife and children) IDs from the GEDCOM file.
      string husband = GetHusbandID(node);
      string wife = GetWifeID(node);
      string[] children = GetChildrenIDs(node);

      //get the child modifier if present
      string[,] children1 = GetChildrenIDsAndModifiers(node);

      // Get the Person objects for the husband and wife,
      // required for marriage info and adding children.
      Person husbandPerson = _people.Find(husband);
      Person wifePerson = _people.Find(wife);

      // Add any marriage / divorce details.
      ImportMarriage(husbandPerson, wifePerson, node, _doc);

      int i = 0;

      // Import the children.
      foreach (string child in children)
      {
        // Get the Person object for the child.
        Person childPerson = _people.Find(child);

        string husbandChildModifier = children1[1, i];
        string wifeChildModifier = children1[2, i];

        ParentChildModifier husbandModifier = ParentChildModifier.Natural;
        ParentChildModifier wifeModifier = ParentChildModifier.Natural;

        if (husbandChildModifier == "Adopted")
        {
          husbandModifier = ParentChildModifier.Adopted;
        }

        if (husbandChildModifier == "Foster")
        {
          husbandModifier = ParentChildModifier.Foster;
        }

        if (wifeChildModifier == "Adopted")
        {
          wifeModifier = ParentChildModifier.Adopted;
        }

        if (wifeChildModifier == "Foster")
        {
          wifeModifier = ParentChildModifier.Foster;
        }

        if (husbandPerson != null && wifePerson != null & childPerson != null)
        {
          _people.AddChild(husbandPerson, childPerson, husbandModifier);
          _people.AddChild(wifePerson, childPerson, wifeModifier);

          List<Person> firstParentChildren = new(husbandPerson.NaturalChildren);
          List<Person> secondParentChildren = new(wifePerson.NaturalChildren);

          // Combined children list that is returned.
          List<Person> naturalChildren = [];

          // Go through and add the children that have both parents.            
          foreach (Person child1 in firstParentChildren)
          {
            if (secondParentChildren.Contains(child1))
            {
              naturalChildren.Add(child1);
            }
          }

          // Go through and add natural siblings
          foreach (Person s in naturalChildren)
          {
            if (s != childPerson && wifeModifier == ParentChildModifier.Natural && husbandModifier == ParentChildModifier.Natural)
            {
              _people.AddSibling(childPerson, s);
            }
          }

        }

        if (husbandPerson == null && wifePerson != null & childPerson != null)
        {
          _people.AddChild(wifePerson, childPerson, wifeModifier);

          // Go through and add natural siblings
          foreach (Person s in wifePerson.NaturalChildren)
          {
            if (s != childPerson && wifeModifier == ParentChildModifier.Natural)
            {
              _people.AddSibling(childPerson, s);
            }
          }
        }

        if (husbandPerson != null && wifePerson == null & childPerson != null)
        {
          _people.AddChild(husbandPerson, childPerson, husbandModifier);

          // Go through and add natural siblings
          foreach (Person s in husbandPerson.NaturalChildren)
          {
            if (s != childPerson && husbandModifier == ParentChildModifier.Natural)
            {
              _people.AddSibling(childPerson, s);
            }
          }
        }

        i++;
      }
    }
  }

  /// <summary>
  /// Update the marriage / divorce information for the two people.
  /// </summary>
  /// <param name="husband">The husband person object.</param>
  /// <param name="wife">The wife person object.</param>
  /// <param name="node">The XML node containing marriage/divorce information.</param>
  /// <param name="doc">The XML document.</param>
  internal static void ImportMarriage(Person husband, Person wife, XmlNode node, XmlDocument doc)
  {
    // Return right away if there are not two people.
    if (husband == null || wife == null)
    {
      return;
    }

    // See if a marriage (or divorce) is specified.
    if (node.SelectSingleNode("MARR") != null || node.SelectSingleNode("DIV") != null)
    {
      string marriageDateDescriptor = GetValueDateDescriptor(node, "MARR/DATE");
      DateWrapper marriageDate = GetValueDate(node, "MARR/DATE");
      string marriagePlace = GetValue(node, "MARR/PLAC");
      string marriageSource = GetValueId(node, "MARR/SOUR");
      string marriageCitation = GetValue(node, "MARR/SOUR/PAGE");
      string marriageCitationActualText = GetValue(node, "MARR/SOUR/DATA/TEXT");

      string marriageCitationNote = ImportEventNote(node, "MARR/SOUR/NOTE", doc);
      string marriageLink = string.Empty;

      if (GetValue(node, "MARR/SOUR/OBJE/FORM") == "URL")  //get correct link if present
      {
        marriageLink = GetValue(node, "MARR/SOUR/OBJE/TITL");
      }

      if (string.IsNullOrEmpty(marriageLink))  //if no link see if there is one in the note
      {
        marriageLink = GetLink(marriageCitationNote);
      }

      string divorceDateDescriptor = GetValueDateDescriptor(node, "DIV/DATE");
      DateWrapper divorceDate = GetValueDate(node, "DIV/DATE");
      string divorceSource = GetValueId(node, "DIV/SOUR");
      string divorceCitation = GetValue(node, "DIV/SOUR/PAGE");
      string divorceCitationActualText = GetValue(node, "DIV/SOUR/DATA/TEXT");

      string divorceCitationNote = ImportEventNote(node, "DIV/SOUR/NOTE", doc);
      string divorceLink = string.Empty;

      if (GetValue(node, "DIV/SOUR/OBJE/FORM") == "URL")  //get correct link if present
      {
        divorceLink = GetValue(node, "DIV/SOUR/OBJE/TITL");
      }

      if (string.IsNullOrEmpty(divorceLink))  //if no link see if there is one in the note
      {
        divorceLink = GetLink(divorceCitationNote);
      }

      SpouseModifier modifier = GetDivorced(node) ? SpouseModifier.Former : SpouseModifier.Current;

      // Add info to husband.
      if (husband.GetSpouseRelationship(wife) == null)
      {
        SpouseRelationship husbandMarriage = new(wife, modifier)
        {
          MarriageDate = marriageDate,
          MarriageDateDescriptor = marriageDateDescriptor,
          MarriagePlace = marriagePlace,

          MarriageCitation = marriageCitation,
          MarriageSource = marriageSource,
          MarriageLink = marriageLink,
          MarriageCitationNote = marriageCitationNote,
          MarriageCitationActualText = marriageCitationActualText,

          DivorceDate = divorceDate,
          DivorceDateDescriptor = divorceDateDescriptor,

          DivorceCitation = divorceCitation,
          DivorceSource = divorceSource,
          DivorceLink = divorceLink,
          DivorceCitationNote = divorceCitationNote,
          DivorceCitationActualText = divorceCitationActualText
        };

        husband.Relationships.Add(husbandMarriage);
      }

      // Add info to wife.
      if (wife.GetSpouseRelationship(husband) == null)
      {
        SpouseRelationship wifeMarriage = new(husband, modifier)
        {
          MarriageDate = marriageDate,
          MarriageDateDescriptor = marriageDateDescriptor,
          MarriagePlace = marriagePlace,

          MarriageCitation = marriageCitation,
          MarriageSource = marriageSource,
          MarriageLink = marriageLink,
          MarriageCitationNote = marriageCitationNote,
          MarriageCitationActualText = marriageCitationActualText,

          DivorceDate = divorceDate,
          DivorceDateDescriptor = divorceDateDescriptor,

          DivorceCitation = divorceCitation,
          DivorceSource = divorceSource,
          DivorceLink = divorceLink,
          DivorceCitationNote = divorceCitationNote,
          DivorceCitationActualText = divorceCitationActualText
        };

        wife.Relationships.Add(wifeMarriage);
      }
    }
    else
    {
      SpouseRelationship wifeMarriage = new(husband, SpouseModifier.Current);
      SpouseRelationship husbandMarriage = new(wife, SpouseModifier.Current);

      wife.Relationships.Add(wifeMarriage);
      husband.Relationships.Add(husbandMarriage);
    }
  }

  /// <summary>
  /// Import photo information from the GEDCOM XML file.
  /// Adds the photo if the referenced file exists.
  /// </summary>
  private static void ImportPhotosAttachments(Person person, XmlNode node)
  {
    try
    {
      // Get list of photos and attachments for this person.
      string[] files = GetFiles(node);
      if (files == null || files.Length == 0)
      {
        return;
      }

      // Import each photo/attachment. Make the first photo specified
      // the default photo (avatar).
      for (int i = 0; i < files.Length; i++)
      {
        // Only import a photo if it actually exists and it is a supported format.
        if (File.Exists(files[i]) && App.IsPhotoFileSupported(files[i]))
        {
          Photo photo = new(files[i])
          {
            IsAvatar = (i == 0)
          };
          person.Photos.Add(photo);
        }
        else if (File.Exists(files[i]) && App.IsAttachmentFileSupported(files[i]))
        {
          Attachment attachment = new(files[i]);
          person.Attachments.Add(attachment);
        }
      }
    }
    catch
    {
      // There was an error importing a photo, ignore 
      // and continue processing the GEDCOM XML file.
      // inform the developer
      Debug.Assert(false);
    }
  }

  /// <summary>
  /// Import the note info from the GEDCOM XMl file.
  /// </summary>
  private void ImportNote(Person person, XmlNode node)
  {
    string value = GetNote(node, "NOTE", _doc);

    try
    {
      if (!string.IsNullOrEmpty(value))
      {
        person.Note = value; //stores the note value in a field
      }
    }
    catch
    {
      // There was an error importing the note, ignore
      // and continue processing the GEDCOM XML file.
      // inform the developer
      Debug.Assert(false);
    }
  }

  /// <summary>
  /// Import the note info from the GEDCOM XMl file.  Event notes have variable paths.
  /// </summary>
  private static string ImportEventNote(XmlNode node, string path, XmlDocument doc)
  {
    string value = GetNote(node, path, doc);
    return value;
  }

  /// <summary>
  /// Import the birth info from the GEDCOM XML file.
  /// </summary>
  private static void ImportBirth(Person person, XmlNode node, XmlDocument doc)
  {
    person.BirthDateDescriptor = GetValueDateDescriptor(node, "BIRT/DATE");
    person.BirthDate = GetValueDate(node, "BIRT/DATE");
    person.BirthPlace = GetValue(node, "BIRT/PLAC");

    person.BirthSource = GetValue(node, "BIRT/SOUR").Replace("@", string.Empty);
    person.BirthCitation = GetValue(node, "BIRT/SOUR/PAGE");
    person.BirthCitationActualText = GetValue(node, "BIRT/SOUR/DATA/TEXT");

    person.BirthCitationNote = ImportEventNote(node, "BIRT/SOUR/NOTE", doc);

    if (GetValue(node, "BIRT/SOUR/OBJE/FORM") == "URL")  //get correct link if present
    {
      person.BirthLink = GetValue(node, "BIRT/SOUR/OBJE/TITL");
    }

    if (string.IsNullOrEmpty(person.BirthLink))  //if no link see if there is one in the note
    {
      person.BirthLink = GetLink(person.BirthCitationNote);
    }
  }

  /// <summary>
  /// Import the death info from the GEDCOM XML file.
  /// </summary>
  private static void ImportDeath(Person person, XmlNode node, XmlDocument doc)
  {

    person.IsLiving = (node.SelectSingleNode("DEAT") == null);

    if (node.SelectSingleNode("DEAT") == null)  //if no DEAT tag check burial tag
    {
      person.IsLiving = (node.SelectSingleNode("BURI") == null);
    }

    if (node.SelectSingleNode("DEAT") == null && node.SelectSingleNode("BURI") == null)  //if no deat tag and no buial tagcheck cremation tag
    {
      person.IsLiving = (node.SelectSingleNode("CREM") == null);
    }

    if (person.Age > 90 && person.IsLiving == true)
    {
      person.IsLiving = false;  //make an assumption that anyone without a death date and over 90 is dead.  This leads to far less people imported as living when then are dead since GEDCOM does not have an IsLiving field.
    }

    person.DeathDateDescriptor = GetValueDateDescriptor(node, "DEAT/DATE");
    person.DeathDate = GetValueDate(node, "DEAT/DATE");
    person.DeathPlace = GetValue(node, "DEAT/PLAC");

    person.DeathSource = GetValue(node, "DEAT/SOUR").Replace("@", string.Empty);
    person.DeathCitation = GetValue(node, "DEAT/SOUR/PAGE");
    person.DeathCitationActualText = GetValue(node, "DEAT/SOUR/DATA/TEXT");

    person.DeathCitationNote = ImportEventNote(node, "DEAT/SOUR/NOTE", doc);

    if (GetValue(node, "DEAT/SOUR/OBJE/FORM") == "URL")  //get correct link if present
    {
      person.DeathLink = GetValue(node, "DEAT/SOUR/OBJE/TITL");
    }

    if (string.IsNullOrEmpty(person.DeathLink))  //if no link see if there is one in the note
    {
      person.DeathLink = GetLink(person.DeathCitationNote);
    }
  }

  /// <summary>
  /// Import the burial event info from the GEDCOM XML file.
  /// </summary>
  private static void ImportBurial(Person person, XmlNode node, XmlDocument doc)
  {
    person.BurialDate = GetValueDate(node, "BURI/DATE");
    person.BurialDateDescriptor = GetValueDateDescriptor(node, "BURI/DATE");
    person.BurialPlace = GetValue(node, "BURI/PLAC");

    person.BurialSource = GetValue(node, "BURI/SOUR").Replace("@", string.Empty);
    person.BurialCitation = GetValue(node, "BURI/SOUR/PAGE");
    person.BurialCitationActualText = GetValue(node, "BURI/SOUR/DATA/TEXT");

    person.BurialCitationNote = ImportEventNote(node, "BURI/SOUR/NOTE", doc);

    if (GetValue(node, "BURI/SOUR/OBJE/FORM") == "URL")  //get correct link if present
    {
      person.BurialLink = GetValue(node, "BURI/SOUR/OBJE/TITL");
    }

    if (string.IsNullOrEmpty(person.BurialLink))  //if no link see if there is one in the note
    {
      person.BurialLink = GetLink(person.BurialCitationNote);
    }
  }

  /// <summary>
  /// Import the cremation event info from the GEDCOM XML file.
  /// </summary>
  private static void ImportCremation(Person person, XmlNode node, XmlDocument doc)
  {
    person.CremationDate = GetValueDate(node, "CREM/DATE");
    person.CremationDateDescriptor = GetValueDateDescriptor(node, "CREM/DATE");
    person.CremationPlace = GetValue(node, "CREM/PLAC");

    person.CremationSource = GetValue(node, "CREM/SOUR").Replace("@", string.Empty);
    person.CremationCitation = GetValue(node, "CREM/SOUR/PAGE");
    person.CremationCitationActualText = GetValue(node, "CREM/SOUR/DATA/TEXT");

    person.CremationCitationNote = ImportEventNote(node, "CREM/SOUR/NOTE", doc);

    if (GetValue(node, "CREM/SOUR/OBJE/FORM") == "URL")  //get correct link if present
    {
      person.CremationLink = GetValue(node, "CREM/SOUR/OBJE/TITL");
    }

    if (string.IsNullOrEmpty(person.CremationLink))  //if no link see if there is one in the note
    {
      person.CremationLink = GetLink(person.CremationCitationNote);
    }
  }

  /// <summary>
  /// Import the religion event info from the GEDCOM XML file.
  /// </summary>
  private static void ImportReligion(Person person, XmlNode node, XmlDocument doc)
  {
    //person.ReligionDate = GetValueDate(node, "RELI/DATE");                            //field not supported in Family.Show
    //person.ReligionDateDescriptor = GetValueDateDescriptor(node, "RELI/DATE");        //field not supported in Family.Show
    person.Religion = GetValue(node, "RELI");
    //person.ReligionPlace = GetValue(node, "RELI/PLAC");                               //field not supported in Family.Show
    person.ReligionSource = GetValue(node, "RELI/SOUR").Replace("@", string.Empty);
    person.ReligionCitation = GetValue(node, "RELI/SOUR/PAGE");
    person.ReligionCitationActualText = GetValue(node, "RELI/SOUR/DATA/TEXT");

    person.ReligionCitationNote = ImportEventNote(node, "RELI/SOUR/NOTE", doc);

    if (GetValue(node, "RELI/SOUR/OBJE/FORM") == "URL")  //get correct link if present
    {
      person.ReligionLink = GetValue(node, "RELI/SOUR/OBJE/TITL");
    }

    if (string.IsNullOrEmpty(person.ReligionLink))  //if no link see if there is one in the note
    {
      person.ReligionLink = GetLink(person.ReligionCitationNote);
    }
  }

  /// <summary>
  /// Import the occupation event info from the GEDCOM XML file.
  /// </summary>
  private static void ImportOccupation(Person person, XmlNode node, XmlDocument doc)
  {
    //person.OccupationDate = GetValueDate(node, "OCCU/DATE");                            //field not supported in Family.Show
    //person.OccupationDateDescriptor = GetValueDateDescriptor(node, "OCCU/DATE");        //field not supported in Family.Show
    person.Occupation = GetValue(node, "OCCU");
    //person.OccupationPlace = GetValue(node, "OCCU/PLAC");                               //field not supported in Family.Show
    person.OccupationSource = GetValue(node, "OCCU/SOUR").Replace("@", string.Empty);
    person.OccupationCitation = GetValue(node, "OCCU/SOUR/PAGE");
    person.OccupationCitationActualText = GetValue(node, "OCCU/SOUR/DATA/TEXT");

    person.OccupationCitationNote = ImportEventNote(node, "OCCU/SOUR/NOTE", doc);

    if (GetValue(node, "OCCU/SOUR/OBJE/FORM") == "URL")  //get correct link if present
    {
      person.OccupationLink = GetValue(node, "OCCU/SOUR/OBJE/TITL");
    }

    if (string.IsNullOrEmpty(person.OccupationLink))  //if no link see if there is one in the note
    {
      person.OccupationLink = GetLink(person.OccupationCitationNote);
    }
  }

  /// <summary>
  /// Import the education event info from the GEDCOM XML file.
  /// </summary>
  private static void ImportEducation(Person person, XmlNode node, XmlDocument doc)
  {
    //person.EducationDate = GetValueDate(node, "EDUC/DATE");                            //field not supported in Family.Show
    //person.EducationDateDescriptor = GetValueDateDescriptor(node, "EDUC/DATE");        //field not supported in Family.Show
    person.Education = GetValue(node, "EDUC");
    //person.EducationPlace = GetValue(node, "EDUC/PLAC");                               //field not supported in Family.Show
    person.EducationSource = GetValue(node, "EDUC/SOUR").Replace("@", string.Empty);
    person.EducationCitation = GetValue(node, "EDUC/SOUR/PAGE");
    person.EducationCitationActualText = GetValue(node, "EDUC/SOUR/DATA/TEXT");

    person.EducationCitationNote = ImportEventNote(node, "EDUC/SOUR/NOTE", doc);

    if (GetValue(node, "EDUC/SOUR/OBJE/FORM") == "URL")  //get correct link if present
    {
      person.EducationLink = GetValue(node, "EDUC/SOUR/OBJE/TITL");
    }

    if (string.IsNullOrEmpty(person.EducationLink))  //if no link see if there is one in the note
    {
      person.EducationLink = GetLink(person.EducationCitationNote);
    }
  }

  /// <summary>
  /// Return a list of file paths specified in the GEDCOM XML file.
  /// </summary>
  private static string[] GetFiles(XmlNode node)
  {
    string[] files;
    XmlNodeList list = node.SelectNodes("OBJE");
    files = new string[list.Count];

    for (int i = 0; i < list.Count; i++)
    {
      files[i] = GetValue(list[i], "FILE");
    }

    return files;
  }

  /// <summary>
  /// Often programs do not store links correctly.
  /// Method to extract the first url link out of citation note.
  /// </summary>
  private static string GetLink(string Note)
  {

    if (!string.IsNullOrEmpty(Note))
    {
      Array Link = Note.Split();

      foreach (string s in Link)
      {
        if ((s.StartsWith("http://") || s.StartsWith("www.")))
        {
          return s;  //only extract one link
        }
      }

      return string.Empty;
    }
    else
    {
      return string.Empty;
    }
  }

  /// <summary>
  /// Method to get a note.  Looks for notes which are continued using linked IDs
  /// </summary>
  private static string GetNote(XmlNode node, string path, XmlDocument doc)
  {
    string value = GetValue(node, path);  //note or often id of note

    //if the note node is an id then find the note and import it.
    if (value.StartsWith('@') && value.EndsWith('@'))
    {

      //get a list of all notes stating 0 NOTE
      XmlNodeList list = doc.SelectNodes("/root/NOTE");

      foreach (XmlNode n in list)
      {
        string s = GetId(n);  //s is the id

        if (!string.IsNullOrEmpty(s) && !string.IsNullOrEmpty(value))
        {
          if (s.Contains(value.Replace("@", string.Empty)))
          {
            value = s.Replace(value.Replace("@", string.Empty), string.Empty);
            break;
          }
        }
      }
    }

    value = value.Trim();

    //remove trailing @ signs

    do
    {
      if (value.StartsWith('@'))
      {
        value = value.Remove(0, 1);
      }
    }
    while (value.StartsWith('@'));

    if (value.Contains("  "))
    {
      value = value.Replace("  ", " ");
    }

    value = value.Trim();

    return value;

  }

  private static string GetSuffix(XmlNode node)
  {
    return GetValue(node, "NAME/NPFX");
  }

  private static string GetHusbandID(XmlNode node)
  {
    return GetValueId(node, "HUSB");
  }

  private static string GetWifeID(XmlNode node)
  {
    return GetValueId(node, "WIFE");
  }

  private static Gender GetGender(XmlNode node)
  {
    string value = GetValue(node, "SEX");
    if (string.Compare(value, "f", true, CultureInfo.InvariantCulture) == 0)
    {
      return Gender.Female;
    }

    return Gender.Male;
  }

  private static Restriction GetRestriction(XmlNode node)
  {
    string value = GetValue(node, "RESN");
    if (string.Compare(value, "privacy", true, CultureInfo.InvariantCulture) == 0)
    {
      return Restriction.Private;
    }
    else if (string.Compare(value, "locked", true, CultureInfo.InvariantCulture) == 0)
    {
      return Restriction.Locked;
    }
    else
    {
      return Restriction.None;
    }
  }

  private static bool GetDivorced(XmlNode node)
  {
    string value = GetValue(node, "DIV");
    if (string.Compare(value, "n", true, CultureInfo.InvariantCulture) == 0)
    {
      return false;
    }

    // Divorced if the tag exists.
    return node.SelectSingleNode("DIV") != null;
  }

  private static string[] GetChildrenIDs(XmlNode node)
  {
    string[] children;
    XmlNodeList list = node.SelectNodes("CHIL");

    children = new string[list.Count];

    for (int i = 0; i < list.Count; i++)
    {
      children[i] = GetId(list[i]);
    }

    return children;
  }

  /// <summary>
  /// Method to determine child modifiers from Ancestry.com/Ancestry.co.uk GEDCOM files
  /// </summary>
  private static string[,] GetChildrenIDsAndModifiers(XmlNode node)
  {
    string[,] children;
    XmlNodeList childrenIDs = node.SelectNodes("CHIL");

    //For adoption tags where present

    XmlNodeList motherModifiers = node.SelectNodes("CHIL/_MREL");
    XmlNodeList fatherModifiers = node.SelectNodes("CHIL/_FREL");

    //Create and fill an array with children ids and 
    //the associated parental relationship modifiers.

    children = new string[3, childrenIDs.Count];

    for (int i = 0; i < childrenIDs.Count; i++)
    {
      children[0, i] = GetId(childrenIDs[i]);
      children[1, i] = GetId(motherModifiers[i]);
      children[2, i] = GetId(fatherModifiers[i]);
    }
    return children;
  }

  private static string GetNames(XmlNode node)
  {
    string name = GetValue(node, "NAME");
    string[] parts = name.Split('/');
    if (parts.Length > 0)
    {
      return parts[0].Trim();
    }

    return string.Empty;
  }

  private static string GetSurname(XmlNode node)
  {
    string name = GetValue(node, "NAME");
    string[] parts = name.Split('/');
    if (parts.Length > 1)
    {
      return parts[1].Trim();
    }

    return string.Empty;
  }

  /// <summary>
  /// Method to try and extract approximate dates from the wide variety of non-standard date notations in use.
  /// Could be extended to work with more scenarios.
  /// </summary>
  /// <param name="node">The XML node containing the date information.</param>
  /// <param name="xpath">The XPath query to select the date node.</param>
  /// <returns>A nullable DateTime object representing the extracted date, or null if the date is invalid.</returns>
  internal static DateWrapper GetValueDate(XmlNode node, string xpath)
  {
    DateWrapper result = null;

    try
    {
      string value = GetValue(node, xpath);

      //look for quarter abbreviations and remove

      if (value.Contains("jan-feb-mar"))  //Q1
      {
        value = value.Replace("-feb-mar", string.Empty);
      }

      if (value.Contains("apr-may-jun"))  //Q2
      {
        value = value.Replace("-may-jun", string.Empty);
      }

      if (value.Contains("jul-aug-sep"))  //Q3
      {
        value = value.Replace("-aug-sep", string.Empty);
      }

      if (value.Contains("oct-nov-dec"))  //Q4
      {
        value = value.Replace("-nov-dec", string.Empty);
      }

      if (value.Contains("jan feb mar"))  //Q1
      {
        value = value.Replace(" feb mar", string.Empty);
      }

      if (value.Contains("apr may jun"))  //Q2
      {
        value = value.Replace(" may jun", string.Empty);
      }

      if (value.Contains("jul aug sep"))  //Q3
      {
        value = value.Replace(" aug sep", string.Empty);
      }

      if (value.Contains("oct nov dec"))  //Q4
      {
        value = value.Replace(" nov dec", string.Empty);
      }

      value = value.Trim();  //remove leading and trailing spaces which will confuse when looking for only year

      if (!string.IsNullOrEmpty(value))
      {
        result = new DateWrapper(value);
        if (DateWrapper.IsNullOrEmpty(result))
        {
          result = null;
        }
      }
    }
    catch
    {
      // The date is invalid, ignore and continue processing.
      // inform the developer
      Debug.Assert(false);
    }

    return result;
  }

  /// <summary>
  /// Gets a descriptor for a date value from the GEDCOM XML file.
  /// The descriptor can be "ABT" (about), "AFT" (after), or "BEF" (before) based on the date value.
  /// </summary>
  /// <param name="node">The XML node containing the date information.</param>
  /// <param name="xpath">The XPath query to select the date node.</param>
  /// <returns>A string representing the date descriptor, or an empty string if no descriptor is found.</returns>
  internal static string GetValueDateDescriptor(XmlNode node, string xpath)
  {
    try
    {
      string value = GetValue(node, xpath);
      value = value.ToLower(new CultureInfo("en-GB", false));

      if (value.Contains("jan-feb-mar"))  //Q1
      {
        return "ABT ";
      }

      if (value.Contains("apr-may-jun"))  //Q2
      {
        return "ABT ";
      }

      if (value.Contains("jul-aug-sep"))  //Q3
      {
        return "ABT ";
      }

      if (value.Contains("oct-nov-dec"))  //Q4
      {
        return "ABT ";
      }

      if (value.Contains("jan feb mar"))  //Q1
      {
        return "ABT ";
      }

      if (value.Contains("apr may jun"))  //Q2
      {
        return "ABT ";
      }

      if (value.Contains("jul aug sep"))  //Q3
      {
        return "ABT ";
      }

      if (value.Contains("oct nov dec"))  //Q4
      {
        return "ABT ";
      }

      if (value.Contains("abt"))
      {
        return "ABT ";
      }

      if (value.Contains("aft"))
      {
        return "AFT ";
      }

      if (value.Contains("bef"))
      {
        return "BEF ";
      }
    }
    catch
    {
      // The date is invalid, ignore and continue processing.
      // inform the developer
      Debug.Assert(false);
    }

    return string.Empty;
  }

  private static string GetId(XmlNode node)
  {
    try
    {
      if (node != null)
      {
        return node.Attributes["Value"].Value.Trim();
      }
    }
    catch
    {
      // Invalid line, keep processing the file.
      // inform the developer
      Debug.Assert(false);
    }
    return string.Empty;
  }

  private static string GetValueId(XmlNode node, string xpath)
  {
    try
    {
      XmlNode valueNode = node.SelectSingleNode(xpath);
      if (valueNode != null)
      {
        return valueNode.Attributes["Value"].Value.Trim();
      }
    }
    catch
    {
      // Invalid line, keep processing the file.
      // inform the developer
      Debug.Assert(false);
    }
    return string.Empty;
  }

  private static string GetValue(XmlNode node, string xpath)
  {
    try
    {
      XmlNode valueNode = node.SelectSingleNode(xpath);

      if (valueNode != null)
      {
        return valueNode.Attributes["Value"].Value.Trim();
      }
    }
    catch
    {
      //Invalid line, keep processing the file.
      // inform the developer
      Debug.Assert(false);
    }
    return string.Empty;
  }
}
