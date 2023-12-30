using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.FamilyShowLib
{
  /// <summary>
  /// Representation for a single serializable Person.
  /// INotifyPropertyChanged allows properties of the Person class to
  /// participate as source in data bindings.
  /// </summary>
  [Serializable]
  public class Person : INotifyPropertyChanged, IEquatable<Person>, IDataErrorInfo
  {
    #region Fields and Constants

    private string id;
    private Gender gender;
    private bool isLiving;

    private Restriction restriction;

    private string firstName;
    private string lastName;
    private string suffix;

    private string occupation;
    private string occupationCitation;
    private string occupationSource;
    private string occupationLink;
    private string occupationCitationNote;
    private string occupationCitationActualText;

    private string education;
    private string educationCitation;
    private string educationSource;
    private string educationLink;
    private string educationCitationNote;
    private string educationCitationActualText;

    private string religion;
    private string religionCitation;
    private string religionSource;
    private string religionLink;
    private string religionCitationNote;
    private string religionCitationActualText;

    private DateTime? birthDate;
    private string birthDateDescriptor;
    private string birthPlace;
    private string birthCitation;
    private string birthSource;
    private string birthLink;
    private string birthCitationNote;
    private string birthCitationActualText;

    private DateTime? deathDate;
    private string deathDateDescriptor;
    private string deathPlace;
    private string deathCitation;
    private string deathSource;
    private string deathLink;
    private string deathCitationNote;
    private string deathCitationActualText;

    private string cremationPlace;
    private DateTime? cremationDate;
    private string cremationDateDescriptor;
    private string cremationCitation;
    private string cremationSource;
    private string cremationLink;
    private string cremationCitationNote;
    private string cremationCitationActualText;

    private string burialPlace;
    private DateTime? burialDate;
    private string burialDateDescriptor;
    private string burialCitation;
    private string burialSource;
    private string burialLink;
    private string burialCitationNote;
    private string burialCitationActualText;

    private string note;
    private readonly PhotoCollection photos;
    private readonly AttachmentCollection attachments;
    private Story story;
    private readonly RelationshipCollection relationships;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the unique identifier for each person.
    /// </summary>
    [XmlAttribute]
    public string Id
    {
      get { return id; }
      set
      {
        if (id != value)
        {
          id = value;
          OnPropertyChanged(nameof(Id));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's gender
    /// </summary>
    public Gender Gender
    {
      get { return gender; }
      set
      {
        if (gender != value)
        {
          gender = value;
          OnPropertyChanged(nameof(Gender));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's privacy level
    /// </summary>
    public Restriction Restriction
    {
      get { return restriction; }
      set
      {
        if (restriction != value)
        {
          restriction = value;
          OnPropertyChanged(nameof(Restriction));
          OnPropertyChanged(nameof(HasRestriction));
          OnPropertyChanged(nameof(IsLocked));
          OnPropertyChanged(nameof(IsPrivate));
          OnPropertyChanged(nameof(IsLockedIsLiving));
          OnPropertyChanged(nameof(IsDeletable));

        }
      }
    }

    #region name details

    /// <summary>
    /// Gets first names
    /// </summary>
    public string FirstName
    {
      get { return firstName; }
      set
      {
        if (firstName != value)
        {
          firstName = value;
          OnPropertyChanged(nameof(FirstName));
          OnPropertyChanged(nameof(Name));
          OnPropertyChanged(nameof(FullName));
        }
      }

    }

    /// <summary>
    /// Gets middle names and append to first name for version 4
    /// </summary>
    public string MiddleName
    {
      get { return null; }
      set
      {
        if (!string.IsNullOrEmpty(value))
        {
          firstName += " " + value;
          OnPropertyChanged(nameof(FirstName));
          OnPropertyChanged(nameof(Name));
          OnPropertyChanged(nameof(FullName));
        }
      }

    }

    /// <summary>
    ///Gets last name
    /// </summary>
    public string LastName
    {
      get { return lastName; }
      set
      {
        if (lastName != value)
        {
          lastName = value;
          OnPropertyChanged(nameof(LastName));
          OnPropertyChanged(nameof(Name));
          OnPropertyChanged(nameof(FullName));
        }
      }
    }

    /// <summary>
    /// Gets the person's name in the format FirstName LastName.
    /// </summary>
    public string Name
    {
      get
      {
        string name = "";
        if (!string.IsNullOrEmpty(FirstName))
        {
          name += firstName;
        }

        if (!string.IsNullOrEmpty(LastName))
        {
          name += " " + lastName;
        }

        return name;
      }
    }

    /// <summary>
    /// Gets the person's fully qualified name: FirstName LastName Suffix
    /// </summary>
    public string FullName
    {
      get
      {
        string fullName = "";
        if (!string.IsNullOrEmpty(FirstName))
        {
          fullName += firstName;
        }

        if (!string.IsNullOrEmpty(LastName))
        {
          fullName += " " + lastName;
        }

        if (!string.IsNullOrEmpty(suffix))
        {
          fullName += " " + suffix;
        }

        return fullName;
      }
    }

    /// <summary>
    /// Gets or sets the text that appears after the last name providing additional information about the person
    /// </summary>

    public string Suffix
    {
      get { return suffix; }
      set
      {
        if (suffix != value)
        {
          suffix = value;
          OnPropertyChanged(nameof(Suffix));
          OnPropertyChanged(nameof(FullName));
        }
      }
    }

    #endregion

    #region age

    /// <summary>
    /// The age of the person.
    /// </summary>

    public int? Age
    {
      get
      {
        if (BirthDate == null)
        {
          return null;
        }

        //Do not show  age  of dead person if no death date is entered.
        if (!isLiving && DeathDate == null)
        {
          return null;
        }

        // Determine the age of the person based on just the year.
        DateTime startDate = BirthDate.Value;
        DateTime endDate = (IsLiving || DeathDate == null) ? DateTime.Now : DeathDate.Value;
        int age = endDate.Year - startDate.Year;

        // Compensate for the month and day of month (if they have not had a birthday this year).
        if (endDate.Month < startDate.Month ||
            (endDate.Month == startDate.Month && endDate.Day < startDate.Day))
        {
          age--;
        }

        return Math.Max(0, age);
      }

    }

    /// <summary>
    /// The age of the person.
    /// </summary>
    [XmlIgnore]
    public AgeGroup AgeGroup
    {
      get
      {
        AgeGroup ageGroup = AgeGroup.Unknown;

        if (Age.HasValue)
        {
          // The AgeGroup enumeration is defined later in this file. It is up to the Person
          // class to define the ages that fall into the particular age groups  
          if (Age >= 0 && Age < 20)
          {
            ageGroup = AgeGroup.Youth;
          }
          else if (Age >= 20 && Age < 40)
          {
            ageGroup = AgeGroup.Adult;
          }
          else if (Age >= 40 && Age < 70)
          {
            ageGroup = AgeGroup.MiddleAge;
          }
          else
          {
            ageGroup = AgeGroup.Senior;
          }
        }
        return ageGroup;
      }
    }

    /// <summary>
    /// The year the person was born
    /// </summary>

    public string YearOfBirth
    {
      get
      {
        if (birthDate.HasValue)
        {
          return birthDate.Value.Year.ToString(CultureInfo.CurrentCulture);
        }
        else
        {
          return "-";
        }
      }
    }

    /// <summary>
    /// The year the person died
    /// </summary>

    public string YearOfDeath
    {
      get
      {
        if (deathDate.HasValue && !isLiving)
        {
          return deathDate.Value.Year.ToString(CultureInfo.CurrentCulture);
        }
        else
        {
          return "-";
        }
      }
    }

    /// <summary>
    /// Gets or sets whether the person is still alive or deceased.
    /// </summary>
    public bool IsLiving
    {
      get { return isLiving; }
      set
      {
        if (isLiving != value)
        {
          isLiving = value;
          OnPropertyChanged(nameof(IsLiving));
          OnPropertyChanged(nameof(Age));
          OnPropertyChanged(nameof(IsLockedIsLiving));

        }
      }
    }

    #endregion

    #region birth details

    /// <summary>
    /// Gets or sets the person's birth date.  This property can be null.
    /// </summary>

    public DateTime? BirthDate
    {
      get { return birthDate; }
      set
      {
        if (birthDate == null || birthDate != value)
        {
          birthDate = value;
          OnPropertyChanged(nameof(BirthDate));
          OnPropertyChanged(nameof(Age));
          OnPropertyChanged(nameof(AgeGroup));
          OnPropertyChanged(nameof(YearOfBirth));
          OnPropertyChanged(nameof(BirthMonthAndDay));
          OnPropertyChanged(nameof(BirthDateAndPlace));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's birth date descriptor
    /// </summary>
    public string BirthDateDescriptor
    {
      get { return birthDateDescriptor; }
      set
      {
        if (birthDateDescriptor == null || birthDateDescriptor != value)
        {
          birthDateDescriptor = value;
          OnPropertyChanged(nameof(BirthDateDescriptor));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's place of birth
    /// </summary>
    public string BirthPlace
    {
      get { return birthPlace; }
      set
      {
        if (birthPlace != value)
        {
          birthPlace = value;
          OnPropertyChanged(nameof(BirthPlace));
          OnPropertyChanged(nameof(BirthDateAndPlace));
          OnPropertyChanged(nameof(HasBirthPlace));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's birth citation
    /// </summary>
    public string BirthCitation
    {
      get { return birthCitation; }
      set
      {
        if (birthCitation != value)
        {
          birthCitation = value;
          OnPropertyChanged(nameof(BirthCitation));
          OnPropertyChanged(nameof(HasCitations));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's birth source
    /// </summary>
    public string BirthSource
    {
      get { return birthSource; }
      set
      {
        if (birthSource != value)
        {
          birthSource = value;
          OnPropertyChanged(nameof(BirthSource));
          OnPropertyChanged(nameof(HasCitations));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's birth link
    /// </summary>
    public string BirthLink
    {
      get { return birthLink; }
      set
      {
        if (birthLink != value)
        {
          birthLink = value;
          OnPropertyChanged(nameof(BirthLink));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's birth citation note
    /// </summary>
    public string BirthCitationNote
    {
      get { return birthCitationNote; }
      set
      {
        if (birthCitationNote != value)
        {
          birthCitationNote = value;
          OnPropertyChanged(nameof(BirthCitationNote));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's birth citation actual text
    /// </summary>
    public string BirthCitationActualText
    {
      get { return birthCitationActualText; }
      set
      {
        if (birthCitationActualText != value)
        {
          birthCitationActualText = value;
          OnPropertyChanged(nameof(BirthCitationActualText));
        }
      }
    }

    /// <summary>
    /// Gets the month and day the person was born in. This property can be null.
    /// </summary>
    [XmlIgnore]
    public string BirthMonthAndDay
    {
      get
      {
        if (birthDate == null)
        {
          return null;
        }
        else
        {
          return birthDate.Value.ToString(
              DateTimeFormatInfo.CurrentInfo.MonthDayPattern,
              CultureInfo.CurrentCulture);
        }
      }
    }

    /// <summary>
    /// Gets a friendly string for BirthDate and Place
    /// </summary>
    [XmlIgnore]
    public string BirthDateAndPlace
    {
      get
      {
        if (birthDate == null)
        {
          return null;
        }
        else
        {
          StringBuilder returnValue = new StringBuilder();
          returnValue.Append("Born ");
          returnValue.Append(
              birthDate.Value.ToString(
                  DateTimeFormatInfo.CurrentInfo.ShortDatePattern,
                  CultureInfo.CurrentCulture));

          if (!string.IsNullOrEmpty(birthPlace))
          {
            returnValue.Append(", ");
            returnValue.Append(birthPlace);
          }

          return returnValue.ToString();
        }
      }
    }

    #endregion

    #region death details

    /// <summary>
    /// Gets or sets the person's death of death.  This property can be null.
    /// </summary>
    public DateTime? DeathDate
    {
      get { return deathDate; }
      set
      {
        if (deathDate != value)
        {
          deathDate = value;
          OnPropertyChanged(nameof(DeathDate));
          OnPropertyChanged(nameof(Age));
          OnPropertyChanged(nameof(YearOfDeath));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's death date descriptor
    /// </summary>
    public string DeathDateDescriptor
    {
      get { return deathDateDescriptor; }
      set
      {
        if (deathDateDescriptor == null || deathDateDescriptor != value)
        {
          deathDateDescriptor = value;
          OnPropertyChanged(nameof(DeathDateDescriptor));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's place of death
    /// </summary>
    public string DeathPlace
    {
      get { return deathPlace; }
      set
      {
        if (deathPlace != value)
        {
          deathPlace = value;
          OnPropertyChanged(nameof(DeathPlace));
          OnPropertyChanged(nameof(HasDeathPlace));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's place of death citation
    /// </summary>
    public string DeathCitation
    {
      get { return deathCitation; }
      set
      {
        if (deathCitation != value)
        {
          deathCitation = value;
          OnPropertyChanged(nameof(DeathCitation));
          OnPropertyChanged(nameof(HasCitations));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's place of death source
    /// </summary>
    public string DeathSource
    {
      get { return deathSource; }
      set
      {
        if (deathSource != value)
        {
          deathSource = value;
          OnPropertyChanged(nameof(DeathSource));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's death link
    /// </summary>
    public string DeathLink
    {
      get { return deathLink; }
      set
      {
        if (deathLink != value)
        {
          deathLink = value;
          OnPropertyChanged(nameof(DeathLink));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's death citation note
    /// </summary>
    public string DeathCitationNote
    {
      get { return deathCitationNote; }
      set
      {
        if (deathCitationNote != value)
        {
          deathCitationNote = value;
          OnPropertyChanged(nameof(DeathCitationNote));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's death citation actual text
    /// </summary>
    public string DeathCitationActualText
    {
      get { return deathCitationActualText; }
      set
      {
        if (deathCitationActualText != value)
        {
          deathCitationActualText = value;
          OnPropertyChanged(nameof(DeathCitationActualText));
        }
      }
    }

    #endregion

    #region cremation details

    /// <summary>
    /// Gets or sets cremation place
    /// </summary>
    public string CremationPlace
    {
      get { return cremationPlace; }
      set
      {
        if (cremationPlace != value)
        {
          cremationPlace = value;
          OnPropertyChanged(nameof(CremationPlace));
          OnPropertyChanged(nameof(HasCremationPlace));
        }
      }
    }

    /// <summary>
    /// Gets or sets cremation date
    /// </summary>
    public DateTime? CremationDate
    {
      get { return cremationDate; }
      set
      {
        if (cremationDate == null || cremationDate != value)
        {
          cremationDate = value;
          OnPropertyChanged(nameof(CremationDate));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's cremation date descriptor
    /// </summary>
    public string CremationDateDescriptor
    {
      get { return cremationDateDescriptor; }
      set
      {
        if (cremationDateDescriptor == null || cremationDateDescriptor != value)
        {
          cremationDateDescriptor = value;
          OnPropertyChanged(nameof(CremationDateDescriptor));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's cremation citation
    /// </summary>
    public string CremationCitation
    {
      get { return cremationCitation; }
      set
      {
        if (cremationCitation != value)
        {
          cremationCitation = value;
          OnPropertyChanged(nameof(CremationCitation));
          OnPropertyChanged(nameof(HasCitations));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's cremation source
    /// </summary>
    public string CremationSource
    {
      get { return cremationSource; }
      set
      {
        if (cremationSource != value)
        {
          cremationSource = value;
          OnPropertyChanged(nameof(CremationSource));
          OnPropertyChanged(nameof(HasCitations));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's cremation link
    /// </summary>
    public string CremationLink
    {
      get { return cremationLink; }
      set
      {
        if (cremationLink != value)
        {
          cremationLink = value;
          OnPropertyChanged(nameof(CremationLink));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's cremation citation note
    /// </summary>
    public string CremationCitationNote
    {
      get { return cremationCitationNote; }
      set
      {
        if (cremationCitationNote != value)
        {
          cremationCitationNote = value;
          OnPropertyChanged(nameof(CremationCitationNote));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's cremation citation actual text
    /// </summary>
    public string CremationCitationActualText
    {
      get { return cremationCitationActualText; }
      set
      {
        if (cremationCitationActualText != value)
        {
          cremationCitationActualText = value;
          OnPropertyChanged(nameof(CremationCitationActualText));
        }
      }
    }

    #endregion

    #region burial details

    /// <summary>
    /// Gets or sets burial place
    /// </summary>
    public string BurialPlace
    {
      get { return burialPlace; }
      set
      {
        if (burialPlace != value)
        {
          burialPlace = value;
          OnPropertyChanged(nameof(BurialPlace));
          OnPropertyChanged(nameof(HasBurialPlace));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's burial citation
    /// </summary>
    public string BurialCitation
    {
      get { return burialCitation; }
      set
      {
        if (burialCitation != value)
        {
          burialCitation = value;
          OnPropertyChanged(nameof(BurialCitation));
          OnPropertyChanged(nameof(HasCitations));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's burial source
    /// </summary>
    public string BurialSource
    {
      get { return burialSource; }
      set
      {
        if (burialSource != value)
        {
          burialSource = value;
          OnPropertyChanged(nameof(BurialSource));
          OnPropertyChanged(nameof(HasCitations));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's burial link
    /// </summary>
    public string BurialLink
    {
      get { return burialLink; }
      set
      {
        if (burialLink != value)
        {
          burialLink = value;
          OnPropertyChanged(nameof(BurialLink));
        }
      }
    }

    /// <summary>
    /// Gets or sets burial date
    /// </summary>
    public DateTime? BurialDate
    {
      get { return burialDate; }
      set
      {
        if (burialDate == null || burialDate != value)
        {
          burialDate = value;
          OnPropertyChanged(nameof(BurialDate));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's burial date descriptor
    /// </summary>
    public string BurialDateDescriptor
    {
      get { return burialDateDescriptor; }
      set
      {
        if (burialDateDescriptor == null || burialDateDescriptor != value)
        {
          burialDateDescriptor = value;
          OnPropertyChanged(nameof(BurialDateDescriptor));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's burial citation note
    /// </summary>
    public string BurialCitationNote
    {
      get { return burialCitationNote; }
      set
      {
        if (burialCitationNote != value)
        {
          burialCitationNote = value;
          OnPropertyChanged(nameof(BurialCitationNote));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's burial citation actual text
    /// </summary>
    public string BurialCitationActualText
    {
      get { return burialCitationActualText; }
      set
      {
        if (burialCitationActualText != value)
        {
          burialCitationActualText = value;
          OnPropertyChanged(nameof(BurialCitationActualText));
        }
      }
    }

    #endregion

    #region occupation details

    /// <summary>
    /// Gets or sets the person's occupation
    /// </summary>
    public string Occupation
    {
      get { return occupation; }
      set
      {
        if (occupation != value)
        {
          occupation = value;
          OnPropertyChanged(nameof(Occupation));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's occupation link
    /// </summary>
    public string OccupationLink
    {
      get { return occupationLink; }
      set
      {
        if (occupationLink != value)
        {
          occupationLink = value;
          OnPropertyChanged(nameof(OccupationLink));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's occupation citation
    /// </summary>
    public string OccupationCitation
    {
      get { return occupationCitation; }
      set
      {
        if (occupationCitation != value)
        {
          occupationCitation = value;
          OnPropertyChanged(nameof(OccupationCitation));
          OnPropertyChanged(nameof(HasCitations));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's occupation source
    /// </summary>
    public string OccupationSource
    {
      get { return occupationSource; }
      set
      {
        if (occupationSource != value)
        {
          occupationSource = value;
          OnPropertyChanged(nameof(OccupationSource));
          OnPropertyChanged(nameof(HasCitations));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's occupation citation note
    /// </summary>
    public string OccupationCitationNote
    {
      get { return occupationCitationNote; }
      set
      {
        if (occupationCitationNote != value)
        {
          occupationCitationNote = value;
          OnPropertyChanged(nameof(OccupationCitationNote));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's occupation citation actual text
    /// </summary>
    public string OccupationCitationActualText
    {
      get { return occupationCitationActualText; }
      set
      {
        if (occupationCitationActualText != value)
        {
          occupationCitationActualText = value;
          OnPropertyChanged(nameof(OccupationCitationActualText));
        }
      }
    }

    #endregion

    #region education details


    /// <summary>
    /// Gets or sets the person's education
    /// </summary>
    public string Education
    {
      get { return education; }
      set
      {
        if (education != value)
        {
          education = value;
          OnPropertyChanged(nameof(Education));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's education citation
    /// </summary>
    public string EducationCitation
    {
      get { return educationCitation; }
      set
      {
        if (educationCitation != value)
        {
          educationCitation = value;
          OnPropertyChanged(nameof(EducationCitation));
          OnPropertyChanged(nameof(HasCitations));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's education source
    /// </summary>
    public string EducationSource
    {
      get { return educationSource; }
      set
      {
        if (educationSource != value)
        {
          educationSource = value;
          OnPropertyChanged(nameof(EducationSource));
          OnPropertyChanged(nameof(HasCitations));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's education link
    /// </summary>
    public string EducationLink
    {
      get { return educationLink; }
      set
      {
        if (educationLink != value)
        {
          educationLink = value;
          OnPropertyChanged(nameof(EducationLink));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's education citation note
    /// </summary>
    public string EducationCitationNote
    {
      get { return educationCitationNote; }
      set
      {
        if (educationCitationNote != value)
        {
          educationCitationNote = value;
          OnPropertyChanged(nameof(EducationCitationNote));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's education citation actual text
    /// </summary>
    public string EducationCitationActualText
    {
      get { return educationCitationActualText; }
      set
      {
        if (educationCitationActualText != value)
        {
          educationCitationActualText = value;
          OnPropertyChanged(nameof(EducationCitationActualText));
        }
      }
    }

    #endregion

    #region religion details

    /// <summary>
    /// Gets or sets the person's religion
    /// </summary>
    public string Religion
    {
      get { return religion; }
      set
      {
        if (religion != value)
        {
          religion = value;
          OnPropertyChanged(nameof(Religion));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's religion citation
    /// </summary>
    public string ReligionCitation
    {
      get { return religionCitation; }
      set
      {
        if (religionCitation != value)
        {
          religionCitation = value;
          OnPropertyChanged(nameof(ReligionCitation));
          OnPropertyChanged(nameof(HasCitations));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's religion link
    /// </summary>
    public string ReligionLink
    {
      get { return religionLink; }
      set
      {
        if (religionLink != value)
        {
          religionLink = value;
          OnPropertyChanged(nameof(ReligionLink));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's religion Source
    /// </summary>
    public string ReligionSource
    {
      get { return religionSource; }
      set
      {
        if (religionSource != value)
        {
          religionSource = value;
          OnPropertyChanged(nameof(ReligionSource));
          OnPropertyChanged(nameof(HasCitations));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's religion citation note
    /// </summary>
    public string ReligionCitationNote
    {
      get { return religionCitationNote; }
      set
      {
        if (religionCitationNote != value)
        {
          religionCitationNote = value;
          OnPropertyChanged(nameof(ReligionCitationNote));
        }
      }
    }

    /// <summary>
    /// Gets or sets the person's religion citation actual text
    /// </summary>
    public string ReligionCitationActualText
    {
      get { return religionCitationActualText; }
      set
      {
        if (religionCitationActualText != value)
        {
          religionCitationActualText = value;
          OnPropertyChanged(nameof(ReligionCitationActualText));
        }
      }
    }

    #endregion

    #region other details

    /// <summary>
    /// Gets or sets the person's note  (field)
    /// </summary>
    public string Note
    {
      get { return note; }
      set
      {
        if (note != value)
        {
          note = value;
          OnPropertyChanged(nameof(Note));
          OnPropertyChanged(nameof(HasNote));
        }
      }
    }

    /// <summary>
    /// Gets the person's story file (rich text)
    /// </summary>
    public Story Story
    {
      get { return story; }
      set
      {
        if (story != value)
        {
          story = value;
          OnPropertyChanged(nameof(Story));
          OnPropertyChanged(nameof(HasNote));
        }
      }
    }

    /// <summary>
    /// Gets or sets the photos associated with the person
    /// </summary>
    public PhotoCollection Photos
    {
      get
      {
        OnPropertyChanged("HasPhotos");
        return photos;
      }
    }

    // <summary>
    /// Gets or sets the attachments associated with the person
    /// </summary>
    public AttachmentCollection Attachments
    {
      get
      {
        OnPropertyChanged(nameof(HasAttachments));
        OnPropertyChanged("AttachmentList");
        return attachments;
      }
    }

    /// <summary>
    /// Gets or sets the person's graphical identity
    /// </summary>
    [XmlIgnore]
    public string Avatar
    {
      get
      {
        string avatar = "";

        if (photos != null && photos.Count > 0)
        {
          foreach (Photo photo in photos)
          {
            if (photo.IsAvatar)
            {
              return photo.FullyQualifiedPath;
            }
          }
        }

        return avatar;
      }
      set
      {
        // This setter is used for change notification.
        OnPropertyChanged(nameof(Avatar));
        OnPropertyChanged(nameof(HasAvatar));
      }
    }

    /// <summary>
    /// Determines whether a person is deletable.
    /// </summary>
    [XmlIgnore]
    public bool IsDeletable
    {
      get
      {
        // This person is locked so you cannot delete them
        if (restriction == Restriction.Locked)
        {
          return false;
        }

        // With a few exceptions, anyone with less than 3 relationships is deletable
        if (relationships.Count < 3)
        {
          // The person has 2 spouses. Since they connect their spouses, they are not deletable.
          if (Spouses.Count == 2)
          {
            return false;
          }

          // The person is connecting two generations
          if (Parents.Count == 1 && Children.Count == 1)
          {
            return false;
          }

          // The person is connecting inlaws
          if (Parents.Count == 1 && Spouses.Count == 1)
          {
            return false;
          }

          // Anyone else with less than 3 relationships is deletable
          return true;
        }

        // More than 3 relationships, however the relationships are from only Children. 
        if (Children.Count > 0 && Parents.Count == 0 && Siblings.Count == 0 && Spouses.Count == 0)
        {
          return true;
        }

        // More than 3 relationships. The relationships are from siblings. Deletable since siblings are connected to each other or the parent.
        if (Siblings.Count > 0 && Parents.Count >= 0 && Spouses.Count == 0 && Children.Count == 0)
        {
          return true;
        }

        // This person has complicated dependencies that does not allow deletion.
        return false;
      }
    }

    #endregion

    #region relationship details

    /// <summary>
    /// Collections of relationship connection for the person
    /// </summary>
    public RelationshipCollection Relationships
    {
      get { return relationships; }
    }

    /// <summary>
    /// Accessor for the person's spouse(s)
    /// </summary>
    [XmlIgnore]
    public Collection<Person> Spouses
    {
      get
      {
        Collection<Person> spouses = [];
        foreach (Relationship rel in relationships)
        {
          if (rel.RelationshipType == RelationshipType.Spouse)
          {
            spouses.Add(rel.RelationTo);
          }
        }
        return spouses;
      }
    }

    [XmlIgnore]
    public Collection<Person> CurrentSpouses
    {
      get
      {
        Collection<Person> spouses = [];
        foreach (Relationship rel in relationships)
        {
          if (rel.RelationshipType == RelationshipType.Spouse)
          {
            if (rel is SpouseRelationship spouseRel && spouseRel.SpouseModifier == SpouseModifier.Current)
            {
              spouses.Add(rel.RelationTo);
            }
          }
        }
        return spouses;
      }
    }

    [XmlIgnore]
    public Collection<Person> PreviousSpouses
    {
      get
      {
        Collection<Person> spouses = [];
        foreach (Relationship rel in relationships)
        {
          if (rel.RelationshipType == RelationshipType.Spouse)
          {
            if (rel is SpouseRelationship spouseRel && spouseRel.SpouseModifier == SpouseModifier.Former)
            {
              spouses.Add(rel.RelationTo);
            }
          }
        }
        return spouses;
      }
    }

    /// <summary>
    /// Accessor for the person's children
    /// </summary>
    [XmlIgnore]
    public Collection<Person> Children
    {
      get
      {
        Collection<Person> children = [];
        foreach (Relationship rel in relationships)
        {
          if (rel.RelationshipType == RelationshipType.Child)
          {
            children.Add(rel.RelationTo);
          }
        }
        return children;
      }
    }

    [XmlIgnore]
    public Collection<Person> NaturalChildren
    {
      get
      {
        Collection<Person> children = [];
        foreach (Relationship rel in relationships)
        {
          if (rel.RelationshipType == RelationshipType.Child)
          {
            if (rel is ChildRelationship childRel && childRel.ParentChildModifier == ParentChildModifier.Natural)
            {
              children.Add(rel.RelationTo);
            }
          }
        }
        return children;
      }
    }

    [XmlIgnore]
    public Collection<Person> AdoptedChildren
    {
      get
      {
        Collection<Person> children = [];
        foreach (Relationship rel in relationships)
        {
          if (rel.RelationshipType == RelationshipType.Child)
          {
            if (rel is ChildRelationship childRel && childRel.ParentChildModifier == ParentChildModifier.Adopted)
            {
              children.Add(rel.RelationTo);
            }
          }
        }
        return children;
      }
    }

    [XmlIgnore]
    public Collection<Person> FosteredChildren
    {
      get
      {
        Collection<Person> children = [];
        foreach (Relationship rel in relationships)
        {
          if (rel.RelationshipType == RelationshipType.Child)
          {
            if (rel is ChildRelationship childRel && childRel.ParentChildModifier == ParentChildModifier.Foster)
            {
              children.Add(rel.RelationTo);
            }
          }
        }
        return children;
      }
    }

    /// <summary>
    /// Accessor for the person's natural parents
    /// </summary>
    [XmlIgnore]
    public Collection<Person> NaturalParents
    {
      get
      {
        Collection<Person> parents = [];
        foreach (Relationship rel in relationships)
        {

          if (rel.RelationshipType == RelationshipType.Parent)
          {
            if (rel is ParentRelationship parentRel && parentRel.ParentChildModifier == ParentChildModifier.Natural)
            {
              parents.Add(rel.RelationTo);
            }
          }
        }
        return parents;
      }
    }


    /// <summary>
    /// Accessor for all of the person's parents
    /// </summary>
    [XmlIgnore]
    public Collection<Person> Parents
    {
      get
      {
        Collection<Person> parents = [];
        foreach (Relationship rel in relationships)
        {
          if (rel.RelationshipType == RelationshipType.Parent)
          {
            parents.Add(rel.RelationTo);
          }
        }
        return parents;
      }
    }

    /// <summary>
    /// Accessor for the person's siblings
    /// </summary>
    [XmlIgnore]
    public Collection<Person> Siblings
    {
      get
      {
        Collection<Person> siblings = [];
        foreach (Relationship rel in relationships)
        {
          if (rel.RelationshipType == RelationshipType.Sibling)
          {
            siblings.Add(rel.RelationTo);
          }
        }
        return siblings;
      }
    }

    /// <summary>
    /// Accessor for the person's half siblings. A half sibling is a person
    /// that contains one or more same parents as the person, but does not 
    /// contain all of the same parents.
    /// </summary>
    [XmlIgnore]
    public Collection<Person> HalfSiblings
    {
      get
      {
        // List that is returned.
        Collection<Person> halfSiblings = [];

        // Get list of full siblings (a full sibling cannot be a half sibling).
        Collection<Person> siblings = Siblings;

        // Iterate through each parent, and determine if the parent's children
        // are half siblings.
        foreach (Person parent in Parents)
        {
          foreach (Person child in parent.Children)
          {
            if (child != this && !siblings.Contains(child) &&
                !halfSiblings.Contains(child))
            {
              halfSiblings.Add(child);
            }
          }
        }

        return halfSiblings;
      }
    }

    /// <summary>
    /// Get the person's parents as a ParentSet object
    /// </summary>
    [XmlIgnore]
    public ParentSet ParentSet
    {
      get
      {
        // Only need to get the parent set if there are two parents.
        if (Parents.Count == 2)
        {
          ParentSet parentSet = new ParentSet(Parents[0], Parents[1]);
          return parentSet;
        }
        else
        {
          return null;
        }
      }
    }

    /// <summary>
    /// Get the possible combination of parents when editting this person or adding this person's sibling.
    /// </summary>
    [XmlIgnore]
    public ParentSetCollection PossibleParentSets
    {
      get
      {
        ParentSetCollection parentSets = [];

        foreach (Person parent in Parents)
        {
          foreach (Person spouse in parent.Spouses)
          {
            ParentSet ps = new ParentSet(parent, spouse);

            // Don't add the same parent set twice.
            if (!parentSets.Contains(ps))
            {
              parentSets.Add(ps);
            }
          }
        }

        return parentSets;
      }
    }

    #endregion

    #region "Has" variables

    /// <summary>
    /// Calculated property that returns parent information
    /// </summary>
    [XmlIgnore]
    public bool HasParents
    {
      get
      {
        return (Parents.Count >= 1);
      }
      set
      {
        // This setter is used for change notification.
        OnPropertyChanged(nameof(HasParents));
        if (Parents.Count >= 2)  //There must be at least 2 parents for a parent set
        {
          OnPropertyChanged(nameof(PossibleParentSets));
        }
        OnPropertyChanged(nameof(Parents));
      }
    }


    /// <summary>
    /// Calculated property that returns true if a person has a restriction
    /// </summary>
    [XmlIgnore]
    public bool HasRestriction
    {
      get
      {
        return (restriction == Restriction.Locked || restriction == Restriction.Private);
      }
      set
      {
        // This setter is used for change notification.
        OnPropertyChanged(nameof(HasRestriction));
      }
    }

    /// <summary>
    /// Gets or sets whether a death related field is editable
    /// Returns false if the person is not locked or is dead.
    /// </summary>
    [XmlIgnore]
    public bool IsLockedIsLiving
    {
      get { return (restriction == Restriction.Locked || isLiving == true); }
      set
      {
        OnPropertyChanged(nameof(IsLockedIsLiving));
      }
    }

    /// <summary>
    /// Gets or sets whether the a non death related field is editable
    /// Returns false if the person is not locked
    /// </summary>
    [XmlIgnore]
    public bool IsLocked
    {
      get { return (restriction == Restriction.Locked); }
      set
      {
        OnPropertyChanged(nameof(IsLocked));
      }
    }

    /// <summary>
    /// Returns true if the person is private
    /// </summary>
    [XmlIgnore]
    public bool IsPrivate
    {
      get { return (restriction == Restriction.Private); }
      set
      {
        OnPropertyChanged(nameof(IsPrivate));
      }
    }

    /// <summary>
    /// Calculated property that returns if a person has siblings
    /// </summary>
    [XmlIgnore]
    public bool HasSiblings
    {
      get
      {
        return (Siblings.Count >= 1);
      }
      set
      {
        // This setter is used for change notification.
        OnPropertyChanged(nameof(HasSiblings));
        OnPropertyChanged(nameof(Siblings));
      }
    }

    /// <summary>
    /// Calculated property that returns whether the person has 1 or more spouse(s).
    /// </summary>
    [XmlIgnore]
    public bool HasSpouse
    {
      get
      {
        return (Spouses.Count >= 1);
      }
      set
      {
        // This setter is used for change notification.
        OnPropertyChanged(nameof(HasSpouse));
        OnPropertyChanged(nameof(Spouses));
      }
    }

    /// <summary>
    /// Calculated property that returns whether the person has an avatar photo.
    /// </summary>
    [XmlIgnore]
    public bool HasAvatar
    {
      get
      {
        if (photos != null && photos.Count > 0)
        {
          foreach (Photo photo in photos)
          {
            if (photo.IsAvatar)
            {
              return true;
            }
          }
        }

        return false;
      }
    }

    /// <summary>
    /// Calculated property that returns whether the person has any photos
    /// </summary>
    [XmlIgnore]
    public bool HasPhoto
    {
      get
      {
        if (photos.Count > 0)
        {
          return true;
        }
        else
        {
          return false;
        }
      }
    }

    /// <summary>
    /// Calculated property that returns whether the person has a birth place
    /// </summary>
    [XmlIgnore]
    public bool HasBirthPlace
    {
      get
      {
        if (birthPlace == null || birthPlace.Length == 0)
        {
          return false;
        }
        else
        {
          return true;
        }
      }
    }

    /// <summary>
    /// Calculated property that returns whether the person has a death place
    /// </summary>
    [XmlIgnore]
    public bool HasDeathPlace
    {
      get
      {
        if (deathPlace == null || deathPlace.Length == 0)
        {
          return false;
        }
        else
        {
          return true;
        }
      }
    }

    /// <summary>
    /// Calculated property that returns whether the person has a cremation place
    /// </summary>
    [XmlIgnore]
    public bool HasCremationPlace
    {
      get
      {
        if (cremationPlace == null || cremationPlace.Length == 0)
        {
          return false;
        }
        else
        {
          return true;
        }
      }
    }

    /// <summary>
    /// Calculated property that returns whether the person has a burial place
    /// </summary>
    [XmlIgnore]
    public bool HasBurialPlace
    {
      get
      {
        if (burialPlace == null || burialPlace.Length == 0)
        {
          return false;
        }
        else
        {
          return true;
        }
      }
    }

    /// <summary>
    /// Calculated property that returns whether the person has any complete citations
    /// </summary>
    [XmlIgnore]
    public bool HasCitations
    {
      get
      {
        if (((religionSource == null || religionSource.Length == 0) || (religionCitation == null || religionCitation.Length == 0)) &&
             ((burialSource == null || burialSource.Length == 0) || (burialCitation == null || burialCitation.Length == 0)) &&
             ((deathSource == null || deathSource.Length == 0) || (deathCitation == null || deathCitation.Length == 0)) &&
             ((educationSource == null || educationSource.Length == 0) || (educationCitation == null || educationCitation.Length == 0)) &&
             ((birthSource == null || birthSource.Length == 0) || (birthCitation == null || birthCitation.Length == 0)) &&
             ((occupationSource == null || occupationSource.Length == 0) || (occupationCitation == null || occupationCitation.Length == 0)) &&
             ((cremationSource == null || cremationSource.Length == 0) || (cremationCitation == null || cremationCitation.Length == 0)))
        {
          return false;
        }
        else
        {
          return true;
        }
      }
    }

    /// <summary>
    /// Calculated property that returns whether the person has a note
    /// </summary>
    [XmlIgnore]
    public bool HasNote
    {
      get
      {
        if (note == null || note.ToString().Length == 0)
        {
          return false;
        }
        else
        {
          return true;
        }
      }
    }

    /// <summary>
    /// Calculated property that returns whether the person has any attachments
    /// </summary>
    [XmlIgnore]
    public bool HasAttachments
    {
      get
      {

        if (attachments.Count > 0)
        {
          return true;
        }
        else
        {
          return false;
        }
      }
    }

    #endregion

    #region Relationship text

    /// <summary>
    /// Calculated property that returns string that describes this person to their parents.
    /// </summary>
    [XmlIgnore]
    public string ParentRelationshipText
    {
      get
      {
        if (gender == Gender.Male)
        {
          return Properties.Resources.Son;
        }
        else
        {
          return Properties.Resources.Daughter;
        }
      }
    }

    /// <summary>
    /// Calculated property that returns string text for this person's parents
    /// </summary>
    [XmlIgnore]
    public string ParentsText
    {
      get
      {
        int i = 1;
        string parentsText = string.Empty;
        foreach (Relationship rel in relationships)
        {

          if (rel.RelationshipType == RelationshipType.Parent)
          {
            if (rel is ParentRelationship parents && parents.ParentChildModifier == ParentChildModifier.Natural)
            {
              if (i == 1)
              {
                parentsText += parents.PersonFullName;
              }

              if (i != 1)
              {
                parentsText += " " + Properties.Resources.And + " " + parents.PersonFullName;
              }

              i += 1;
            }
          }

        }
        if (!string.IsNullOrEmpty(parentsText))
        {
          return " " + Properties.Resources.Of + " " + parentsText;
        }
        else
        {
          return parentsText;
        }
      }
    }

    /// <summary>
    /// Calculated property that returns string that describes this person to their siblings.
    /// </summary>
    [XmlIgnore]
    public string SiblingRelationshipText
    {
      get
      {
        if (gender == Gender.Male)
        {
          return Properties.Resources.Brother;
        }
        else
        {
          return Properties.Resources.Sister;
        }
      }
    }

    /// <summary>
    /// Calculated property that returns string text for this person's siblings
    /// </summary>
    [XmlIgnore]
    public string SiblingsText
    {
      get
      {
        Collection<Person> siblings = Siblings;

        string siblingsText = string.Empty;
        if (siblings.Count > 0)
        {
          siblingsText = siblings[0].Name;

          if (siblings.Count == 2)
          {
            siblingsText += " " + Properties.Resources.And + " " + siblings[1].Name;
          }
          else
          {
            for (int i = 1; i < siblings.Count; i++)
            {
              if (i == siblings.Count - 1)
              {
                siblingsText += " " + Properties.Resources.And + " " + siblings[i].Name;
              }
              else
              {
                siblingsText += ", " + siblings[i].Name;
              }
            }
          }
        }

        if (!string.IsNullOrEmpty(siblingsText))
        {
          return " " + Properties.Resources.To + " " + siblingsText;
        }
        else
        {
          return siblingsText;
        }
      }
    }

    /// <summary>
    /// Calculated property that returns string that describes this person to their spouses.
    /// </summary>
    [XmlIgnore]
    public string SpouseRelationshipText
    {
      get
      {
        if (gender == Gender.Male)
        {
          return Properties.Resources.Husband;
        }
        else
        {
          return Properties.Resources.Wife;
        }
      }
    }

    /// <summary>
    /// Calculated property that returns string text for this person's spouses.
    /// </summary>
    [XmlIgnore]
    public string SpousesText
    {
      get
      {
        Collection<Person> spouses = Spouses;

        string spousesText = string.Empty;
        if (spouses.Count > 0)
        {
          spousesText = spouses[0].Name;

          if (spouses.Count == 2)
          {
            spousesText += " " + Properties.Resources.And + " " + spouses[1].Name;
          }
          else
          {
            for (int i = 1; i < spouses.Count; i++)
            {
              if (i == spouses.Count - 1)
              {
                spousesText += ", " + Properties.Resources.And + " " + spouses[i].Name;
              }
              else
              {
                spousesText += ", " + spouses[i].Name;
              }
            }
          }
        }
        if (!string.IsNullOrEmpty(spousesText))
        {
          return " " + Properties.Resources.To + " " + spousesText;
        }
        else
        {
          return spousesText;
        }
      }
    }

    /// <summary>
    /// Calculated property that returns string that describes this person to their children.
    /// </summary>
    [XmlIgnore]
    public string ChildRelationshipText
    {
      get
      {

        if (gender == Gender.Male)
        {
          return Properties.Resources.Father;
        }
        else
        {
          return Properties.Resources.Mother;
        }
      }
    }

    /// <summary>
    /// Calculated property that returns string text for this person's children.
    /// </summary>
    [XmlIgnore]
    public string ChildrenText
    {
      get
      {
        int i = 1;
        string childrensText = string.Empty;
        foreach (Relationship rel in relationships)
        {

          if (rel.RelationshipType == RelationshipType.Child)
          {
            if (rel is ChildRelationship children && children.ParentChildModifier == ParentChildModifier.Natural)
            {
              if (i == 1)
              {
                childrensText += children.PersonFullName;
              }

              if (i != 1)
              {
                childrensText += " " + Properties.Resources.And + " " + children.PersonFullName;
              }

              i += 1;
            }
          }

        }

        if (!string.IsNullOrEmpty(childrensText))
        {
          return " " + Properties.Resources.To + " " + childrensText;
        }
        else
        {
          return childrensText;
        }
      }

    }

    #endregion

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new instance of a person object.
    /// Each new instance will be given a unique identifier.
    /// This parameterless constructor is also required for serialization.
    /// </summary>
    public Person()
    {
      id = Guid.NewGuid().ToString();
      relationships = [];
      photos = [];
      attachments = [];
      firstName = Properties.Resources.Unknown;
      isLiving = true;
      restriction = Restriction.None;
    }

    /// <summary>
    /// Creates a new instance of the person class with the firstname and the lastname.
    /// </summary>
    public Person(string firstNames, string lastName)
        : this()
    {
      //Use the first name if specified, if not, the default first name is used.
      if (!string.IsNullOrEmpty(firstNames))
      {
        firstName = firstNames;
      }

      this.lastName = lastName;
    }

    /// <summary>
    /// Creates a new instance of the person class with the firstname, the lastname, and gender
    /// </summary>
    public Person(string firstName, string lastName, Gender gender)
        : this(firstName, lastName)
    {
      this.gender = gender;
    }

    #endregion

    #region INotifyPropertyChanged Members

    /// <summary>
    /// INotifyPropertyChanged requires a property called PropertyChanged.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Fires the event for the property when it changes.
    /// </summary>
    public virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    #endregion

    #region IEquatable Members

    /// <summary>
    /// Determine equality between two persons.
    /// </summary>
    public bool Equals(Person other)
    {
      if (other is null)
      {
        return false;
      }
      // Optimization for a common success case.
      if (ReferenceEquals(this, other))
      {
        return true;
      }
      // If run-time types are not exactly the same, return false.
      if (GetType() != other.GetType())
      {
        return false;
      }
      // Return true if the Id match.
      return Id == other.Id;
    }

    #endregion

    #region Equals Methods

    /// <summary>
    /// Determines whether the specified object is equal to the current person.
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as Person);

    /// <summary>
    /// Returns the hash code of this person.
    /// </summary>
    public override int GetHashCode()
    {
      return Id.GetHashCode();
    }

    public static bool operator ==(Person lhs, Person rhs)
    {
      return lhs is null ? rhs is null : lhs.Equals(rhs);
    }

    public static bool operator !=(Person lhs, Person rhs) => !(lhs == rhs);

    #endregion

    #region Methods

    /// <summary>
    /// Get the spouse relationship for the specified spouse.
    /// </summary>
    public SpouseRelationship GetSpouseRelationship(Person spouse)
    {
      foreach (Relationship relationship in relationships)
      {
        if (relationship is SpouseRelationship spouseRelationship)
        {
          if (spouseRelationship.RelationTo.Equals(spouse))
          {
            return spouseRelationship;
          }
        }
      }

      return null;
    }

    public ChildRelationship GetParentChildRelationship(Person child)
    {
      foreach (Relationship relationship in relationships)
      {
        if (relationship is ChildRelationship childRelationship)
        {
          if (childRelationship.RelationTo.Equals(child))
          {
            return childRelationship;
          }
        }
      }

      return null;
    }

    /// <summary>
    /// Gets the combination of parent sets for this person and his/her spouses
    /// </summary>
    /// <returns></returns>
    public ParentSetCollection MakeParentSets()
    {
      ParentSetCollection parentSets = [];

      foreach (Person spouse in Spouses)
      {
        ParentSet ps = new ParentSet(this, spouse);

        // Don't add the same parent set twice.
        if (!parentSets.Contains(ps))
        {
          parentSets.Add(ps);
        }
      }

      return parentSets;
    }

    /// <summary>
    /// Called to delete the person's story
    /// </summary>
    public void DeleteStory()
    {
      if (story != null)
      {
        story.Delete();
        story = null;
      }
    }

    public override string ToString()
    {
      return Name;
    }

    #endregion

    #region IDataErrorInfo Members

    public string Error
    {
      get { return null; }
    }

    public string this[string columnName]
    {
      get
      {
        string result = null;

        if (columnName == "BirthDate")
        {
          if (BirthDate == DateTime.MinValue)
          {
            result = Properties.Resources.InvalidDate;
          }
        }

        if (columnName == "DeathDate")
        {
          if (DeathDate == DateTime.MinValue)
          {
            result = Properties.Resources.InvalidDate;
          }
        }

        if (columnName == "CremationDate")
        {
          if (CremationDate == DateTime.MinValue)
          {
            result = Properties.Resources.InvalidDate;
          }
        }

        if (columnName == "BurialDate")
        {
          if (BurialDate == DateTime.MinValue)
          {
            result = Properties.Resources.InvalidDate;
          }
        }

        return result;
      }
    }

    #endregion
  }

  /// <summary>
  /// Enumeration of the person's gender
  /// </summary>
  public enum Gender
  {
    Male, Female
  }

  /// <summary>
  /// Enumeration of the person's restriction
  /// </summary>
  public enum Restriction
  {
    None, Locked, Private
  }

  /// <summary>
  /// Enumeration of the person's age group
  /// </summary>
  public enum AgeGroup
  {
    Unknown, Youth, Adult, MiddleAge, Senior
  }

  /// <summary>
  /// Representation for a Parent couple.  E.g. Bob and Sue
  /// </summary>
  public class ParentSet : IEquatable<ParentSet>
  {
    #region Properties

    public Person FirstParent { get; set; }

    public Person SecondParent { get; set; }

    public string Name
    {
      get
      {
        string name = "";
        name += FirstParent.Name + " + " + SecondParent.Name;
        return name;
      }
    }

    #endregion

    #region Constructors

    public ParentSet(Person firstParent, Person secondParent)
    {
      FirstParent = firstParent;
      SecondParent = secondParent;
    }

    // Parameterless contstructor required for serialization
    public ParentSet() { }

    #endregion

    #region IEquatable<ParentSet> Members

    /// <summary>
    /// Determine equality between two ParentSet classes.  Note: Bob and Sue == Sue and Bob
    /// </summary>
    public bool Equals(ParentSet other)
    {
      if (other is null)
      {
        return false;
      }
      // Optimization for a common success case.
      if (ReferenceEquals(this, other))
      {
        return true;
      }
      // If run-time types are not exactly the same, return false.
      if (GetType() != other.GetType())
      {
        return false;
      }
      // Return true if the parents match.
      return FirstParent.Equals(other.FirstParent) && SecondParent.Equals(other.SecondParent) ||
        FirstParent.Equals(other.SecondParent) && SecondParent.Equals(other.FirstParent);
    }

    #endregion

    #region Equals Methods

    /// <summary>
    /// Determines whether the specified object is equal to the current parents.
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as ParentSet);

    /// <summary>
    /// Returns the hash code of the parents.
    /// </summary>
    public override int GetHashCode()
    {
      return 17 * 31 + FirstParent.GetHashCode() + SecondParent.GetHashCode();
    }

    public static bool operator ==(ParentSet lhs, ParentSet rhs)
    {
      return lhs is null ? rhs is null : lhs.Equals(rhs);
    }

    public static bool operator !=(ParentSet lhs, ParentSet rhs) => !(lhs == rhs);

    #endregion
  }

  /// <summary>
  /// Collection of ParentSet objects.
  /// </summary>
  public class ParentSetCollection : Collection<ParentSet> { }
}
