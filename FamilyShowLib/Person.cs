using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

using Genealogy.Domain.Interfaces;

#pragma warning disable IDE0161 // Convert to file-scoped namespace - class diagram does not work without
namespace FamilyShowLib
#pragma warning restore IDE0161 // Convert to file-scoped namespace
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

        private string _id;
        private Gender _gender;
        private bool _isLiving;

        private Restriction _restriction;

        private string _firstName;
        private string _lastName;
        private string _suffix;

        // New: Collection of names for supporting multiple names
        private readonly NameCollection _names;

        private string _occupation;
        private string _occupationCitation;
        private string _occupationSource;
        private string _occupationLink;
        private string _occupationCitationNote;
        private string _occupationCitationActualText;

        private string _education;
        private string _educationCitation;
        private string _educationSource;
        private string _educationLink;
        private string _educationCitationNote;
        private string _educationCitationActualText;

        private string _religion;
        private string _religionCitation;
        private string _religionSource;
        private string _religionLink;
        private string _religionCitationNote;
        private string _religionCitationActualText;

        private DateWrapper _birthDate;
        private string _birthDateDescriptor;
        private string _birthPlace;
        private string _birthCitation;
        private string _birthSource;
        private string _birthLink;
        private string _birthCitationNote;
        private string _birthCitationActualText;

        private DateWrapper _deathDate;
        private string _deathDateDescriptor;
        private string _deathPlace;
        private string _deathCitation;
        private string _deathSource;
        private string _deathLink;
        private string _deathCitationNote;
        private string _deathCitationActualText;

        private string _cremationPlace;
        private DateWrapper _cremationDate;
        private string _cremationDateDescriptor;
        private string _cremationCitation;
        private string _cremationSource;
        private string _cremationLink;
        private string _cremationCitationNote;
        private string _cremationCitationActualText;

        private string _burialPlace;
        private DateWrapper _burialDate;
        private string _burialDateDescriptor;
        private string _burialCitation;
        private string _burialSource;
        private string _burialLink;
        private string _burialCitationNote;
        private string _burialCitationActualText;

        private string _note;
        private readonly PhotoCollection _photos;
        private readonly AttachmentCollection _attachments;
        private Story _story;
        private readonly RelationshipCollection _relationships;

        // Flag to prevent recursion when syncing names
        private bool _isSyncingNames;

        #endregion

#pragma warning disable IDE1006 // Naming Styles
        internal string _UID;
#pragma warning restore IDE1006 // Naming Styles

        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for each person.
        /// </summary>
        [XmlAttribute]
        public string Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's gender
        /// </summary>
        public Gender Gender
        {
            get { return _gender; }
            set
            {
                if (_gender != value)
                {
                    _gender = value;
                    OnPropertyChanged(nameof(Gender));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's privacy level
        /// </summary>
        public Restriction Restriction
        {
            get { return _restriction; }
            set
            {
                if (_restriction != value)
                {
                    _restriction = value;
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
        /// Gets the collection of names for this person (supports multiple names from GEDCOM)
        /// </summary>
        public NameCollection Names
        {
            get { return _names; }
        }

        /// <summary>
        /// Gets first names (from primary name)
        /// </summary>
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    SyncPrimaryNameFromFields();
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
                    _firstName += " " + value;
                    OnPropertyChanged(nameof(FirstName));
                    OnPropertyChanged(nameof(Name));
                    OnPropertyChanged(nameof(FullName));
                }
            }

        }

        /// <summary>
        ///Gets last name (from primary name)
        /// </summary>
        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    SyncPrimaryNameFromFields();
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
                    name += _firstName;
                }

                if (!string.IsNullOrEmpty(LastName))
                {
                    name += " " + _lastName;
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
                    fullName += _firstName;
                }

                if (!string.IsNullOrEmpty(LastName))
                {
                    fullName += " " + _lastName;
                }

                if (!string.IsNullOrEmpty(_suffix))
                {
                    fullName += " " + _suffix;
                }

                return fullName;
            }
        }

        /// <summary>
        /// Gets or sets the text that appears after the last name providing additional information about the person
        /// additional information about the person (from primary name)
        /// </summary>

        public string Suffix
        {
            get { return _suffix; }
            set
            {
                if (_suffix != value)
                {
                    _suffix = value;
                    SyncPrimaryNameFromFields();
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
                if (!DateWrapper.IsDateExact(BirthDate, out IDateExact birthDate))
                {
                    return null;
                }

                // Do not show age of dead person if no death birthDate is entered.
                if (!DateWrapper.IsDateExact(DeathDate, out IDateExact deathDate) && !_isLiving)
                {
                    return null;
                }

                // Determine the age of the person based on just the year.
                DateTime startDate = DateExactAsDateTime(birthDate);
                DateTime endDate = IsLiving ? DateTime.Now : DateExactAsDateTime(deathDate);
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

        internal static DateTime DateExactAsDateTime(IDateExact date) =>
            new(date.Year, Math.Max(date.Month, 1), Math.Max(date.Day, 1));

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
                if (DateWrapper.IsDateExact(_birthDate, out IDateExact birthDate))
                {
                    return birthDate.Year.ToString(CultureInfo.CurrentCulture);
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
                if (DateWrapper.IsDateExact(_deathDate, out IDateExact deathDate) && !_isLiving)
                {
                    return deathDate.Year.ToString(CultureInfo.CurrentCulture);
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
            get { return _isLiving; }
            set
            {
                if (_isLiving != value)
                {
                    _isLiving = value;
                    OnPropertyChanged(nameof(IsLiving));
                    OnPropertyChanged(nameof(Age));
                    OnPropertyChanged(nameof(IsLockedIsLiving));

                }
            }
        }

        #endregion

        #region birth details

        /// <summary>
        /// Gets or sets the person's birth birthDate. This property can be null.
        /// </summary>

        public DateWrapper BirthDate
        {
            get { return _birthDate; }
            set
            {
                if (_birthDate != value)
                {
                    _birthDate = value;
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
        /// Gets or sets the person's birth birthDate descriptor
        /// </summary>
        public string BirthDateDescriptor
        {
            get { return _birthDateDescriptor; }
            set
            {
                if (_birthDateDescriptor != value)
                {
                    _birthDateDescriptor = value;
                    OnPropertyChanged(nameof(BirthDateDescriptor));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's place of birth
        /// </summary>
        public string BirthPlace
        {
            get { return _birthPlace; }
            set
            {
                if (_birthPlace != value)
                {
                    _birthPlace = value;
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
            get { return _birthCitation; }
            set
            {
                if (_birthCitation != value)
                {
                    _birthCitation = value;
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
            get { return _birthSource; }
            set
            {
                if (_birthSource != value)
                {
                    _birthSource = value;
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
            get { return _birthLink; }
            set
            {
                if (_birthLink != value)
                {
                    _birthLink = value;
                    OnPropertyChanged(nameof(BirthLink));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's birth citation note
        /// </summary>
        public string BirthCitationNote
        {
            get { return _birthCitationNote; }
            set
            {
                if (_birthCitationNote != value)
                {
                    _birthCitationNote = value;
                    OnPropertyChanged(nameof(BirthCitationNote));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's birth citation actual text
        /// </summary>
        public string BirthCitationActualText
        {
            get { return _birthCitationActualText; }
            set
            {
                if (_birthCitationActualText != value)
                {
                    _birthCitationActualText = value;
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
                return DateWrapper.IsDateExact(BirthDate, out IDateExact birthdate)
                  ? DateExactAsDateTime(birthdate).ToString(
                      DateTimeFormatInfo.CurrentInfo.MonthDayPattern,
                      CultureInfo.CurrentCulture)
                  : null;
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
                if (DateWrapper.IsDateExact(BirthDate, out IDateExact birthdate))
                {
                    StringBuilder returnValue = new();
                    returnValue.Append("Born ");
                    returnValue.Append(
                        DateExactAsDateTime(birthdate).ToString(
                            DateTimeFormatInfo.CurrentInfo.ShortDatePattern,
                            CultureInfo.CurrentCulture));

                    if (!string.IsNullOrEmpty(_birthPlace))
                    {
                        returnValue.Append(", ");
                        returnValue.Append(_birthPlace);
                    }

                    return returnValue.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region death details

        /// <summary>
        /// Gets or sets the person's death of death.  This property can be null.
        /// </summary>
        public DateWrapper DeathDate
        {
            get { return _deathDate; }
            set
            {
                if (_deathDate != value)
                {
                    _deathDate = value;
                    OnPropertyChanged(nameof(DeathDate));
                    OnPropertyChanged(nameof(Age));
                    OnPropertyChanged(nameof(YearOfDeath));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's death birthDate descriptor
        /// </summary>
        public string DeathDateDescriptor
        {
            get { return _deathDateDescriptor; }
            set
            {
                if (_deathDateDescriptor != value)
                {
                    _deathDateDescriptor = value;
                    OnPropertyChanged(nameof(DeathDateDescriptor));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's place of death
        /// </summary>
        public string DeathPlace
        {
            get { return _deathPlace; }
            set
            {
                if (_deathPlace != value)
                {
                    _deathPlace = value;
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
            get { return _deathCitation; }
            set
            {
                if (_deathCitation != value)
                {
                    _deathCitation = value;
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
            get { return _deathSource; }
            set
            {
                if (_deathSource != value)
                {
                    _deathSource = value;
                    OnPropertyChanged(nameof(DeathSource));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's death link
        /// </summary>
        public string DeathLink
        {
            get { return _deathLink; }
            set
            {
                if (_deathLink != value)
                {
                    _deathLink = value;
                    OnPropertyChanged(nameof(DeathLink));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's death citation note
        /// </summary>
        public string DeathCitationNote
        {
            get { return _deathCitationNote; }
            set
            {
                if (_deathCitationNote != value)
                {
                    _deathCitationNote = value;
                    OnPropertyChanged(nameof(DeathCitationNote));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's death citation actual text
        /// </summary>
        public string DeathCitationActualText
        {
            get { return _deathCitationActualText; }
            set
            {
                if (_deathCitationActualText != value)
                {
                    _deathCitationActualText = value;
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
            get { return _cremationPlace; }
            set
            {
                if (_cremationPlace != value)
                {
                    _cremationPlace = value;
                    OnPropertyChanged(nameof(CremationPlace));
                    OnPropertyChanged(nameof(HasCremationPlace));
                }
            }
        }

        /// <summary>
        /// Gets or sets cremation birthDate
        /// </summary>
        public DateWrapper CremationDate
        {
            get { return _cremationDate; }
            set
            {
                if (_cremationDate != value)
                {
                    _cremationDate = value;
                    OnPropertyChanged(nameof(CremationDate));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's cremation birthDate descriptor
        /// </summary>
        public string CremationDateDescriptor
        {
            get { return _cremationDateDescriptor; }
            set
            {
                if (_cremationDateDescriptor != value)
                {
                    _cremationDateDescriptor = value;
                    OnPropertyChanged(nameof(CremationDateDescriptor));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's cremation citation
        /// </summary>
        public string CremationCitation
        {
            get { return _cremationCitation; }
            set
            {
                if (_cremationCitation != value)
                {
                    _cremationCitation = value;
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
            get { return _cremationSource; }
            set
            {
                if (_cremationSource != value)
                {
                    _cremationSource = value;
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
            get { return _cremationLink; }
            set
            {
                if (_cremationLink != value)
                {
                    _cremationLink = value;
                    OnPropertyChanged(nameof(CremationLink));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's cremation citation note
        /// </summary>
        public string CremationCitationNote
        {
            get { return _cremationCitationNote; }
            set
            {
                if (_cremationCitationNote != value)
                {
                    _cremationCitationNote = value;
                    OnPropertyChanged(nameof(CremationCitationNote));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's cremation citation actual text
        /// </summary>
        public string CremationCitationActualText
        {
            get { return _cremationCitationActualText; }
            set
            {
                if (_cremationCitationActualText != value)
                {
                    _cremationCitationActualText = value;
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
            get { return _burialPlace; }
            set
            {
                if (_burialPlace != value)
                {
                    _burialPlace = value;
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
            get { return _burialCitation; }
            set
            {
                if (_burialCitation != value)
                {
                    _burialCitation = value;
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
            get { return _burialSource; }
            set
            {
                if (_burialSource != value)
                {
                    _burialSource = value;
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
            get { return _burialLink; }
            set
            {
                if (_burialLink != value)
                {
                    _burialLink = value;
                    OnPropertyChanged(nameof(BurialLink));
                }
            }
        }

        /// <summary>
        /// Gets or sets burial birthDate
        /// </summary>
        public DateWrapper BurialDate
        {
            get { return _burialDate; }
            set
            {
                if (_burialDate != value)
                {
                    _burialDate = value;
                    OnPropertyChanged(nameof(BurialDate));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's burial birthDate descriptor
        /// </summary>
        public string BurialDateDescriptor
        {
            get { return _burialDateDescriptor; }
            set
            {
                if (_burialDateDescriptor != value)
                {
                    _burialDateDescriptor = value;
                    OnPropertyChanged(nameof(BurialDateDescriptor));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's burial citation note
        /// </summary>
        public string BurialCitationNote
        {
            get { return _burialCitationNote; }
            set
            {
                if (_burialCitationNote != value)
                {
                    _burialCitationNote = value;
                    OnPropertyChanged(nameof(BurialCitationNote));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's burial citation actual text
        /// </summary>
        public string BurialCitationActualText
        {
            get { return _burialCitationActualText; }
            set
            {
                if (_burialCitationActualText != value)
                {
                    _burialCitationActualText = value;
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
            get { return _occupation; }
            set
            {
                if (_occupation != value)
                {
                    _occupation = value;
                    OnPropertyChanged(nameof(Occupation));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's occupation link
        /// </summary>
        public string OccupationLink
        {
            get { return _occupationLink; }
            set
            {
                if (_occupationLink != value)
                {
                    _occupationLink = value;
                    OnPropertyChanged(nameof(OccupationLink));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's occupation citation
        /// </summary>
        public string OccupationCitation
        {
            get { return _occupationCitation; }
            set
            {
                if (_occupationCitation != value)
                {
                    _occupationCitation = value;
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
            get { return _occupationSource; }
            set
            {
                if (_occupationSource != value)
                {
                    _occupationSource = value;
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
            get { return _occupationCitationNote; }
            set
            {
                if (_occupationCitationNote != value)
                {
                    _occupationCitationNote = value;
                    OnPropertyChanged(nameof(OccupationCitationNote));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's occupation citation actual text
        /// </summary>
        public string OccupationCitationActualText
        {
            get { return _occupationCitationActualText; }
            set
            {
                if (_occupationCitationActualText != value)
                {
                    _occupationCitationActualText = value;
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
            get { return _education; }
            set
            {
                if (_education != value)
                {
                    _education = value;
                    OnPropertyChanged(nameof(Education));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's education citation
        /// </summary>
        public string EducationCitation
        {
            get { return _educationCitation; }
            set
            {
                if (_educationCitation != value)
                {
                    _educationCitation = value;
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
            get { return _educationSource; }
            set
            {
                if (_educationSource != value)
                {
                    _educationSource = value;
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
            get { return _educationLink; }
            set
            {
                if (_educationLink != value)
                {
                    _educationLink = value;
                    OnPropertyChanged(nameof(EducationLink));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's education citation note
        /// </summary>
        public string EducationCitationNote
        {
            get { return _educationCitationNote; }
            set
            {
                if (_educationCitationNote != value)
                {
                    _educationCitationNote = value;
                    OnPropertyChanged(nameof(EducationCitationNote));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's education citation actual text
        /// </summary>
        public string EducationCitationActualText
        {
            get { return _educationCitationActualText; }
            set
            {
                if (_educationCitationActualText != value)
                {
                    _educationCitationActualText = value;
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
            get { return _religion; }
            set
            {
                if (_religion != value)
                {
                    _religion = value;
                    OnPropertyChanged(nameof(Religion));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's religion citation
        /// </summary>
        public string ReligionCitation
        {
            get { return _religionCitation; }
            set
            {
                if (_religionCitation != value)
                {
                    _religionCitation = value;
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
            get { return _religionLink; }
            set
            {
                if (_religionLink != value)
                {
                    _religionLink = value;
                    OnPropertyChanged(nameof(ReligionLink));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's religion Source
        /// </summary>
        public string ReligionSource
        {
            get { return _religionSource; }
            set
            {
                if (_religionSource != value)
                {
                    _religionSource = value;
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
            get { return _religionCitationNote; }
            set
            {
                if (_religionCitationNote != value)
                {
                    _religionCitationNote = value;
                    OnPropertyChanged(nameof(ReligionCitationNote));
                }
            }
        }

        /// <summary>
        /// Gets or sets the person's religion citation actual text
        /// </summary>
        public string ReligionCitationActualText
        {
            get { return _religionCitationActualText; }
            set
            {
                if (_religionCitationActualText != value)
                {
                    _religionCitationActualText = value;
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
            get { return _note; }
            set
            {
                if (_note != value)
                {
                    _note = value;
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
            get { return _story; }
            set
            {
                if (_story != value)
                {
                    _story = value;
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
                return _photos;
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
                return _attachments;
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

                if (_photos != null && _photos.Count > 0)
                {
                    foreach (Photo photo in _photos)
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
                if (_restriction == Restriction.Locked)
                {
                    return false;
                }

                // With a few exceptions, anyone with less than 3 relationships is deletable
                if (_relationships.Count < 3)
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

                // More than 3 relationships. The relationships are from siblings.
                // Deletable since siblings are connected to each other or the parent.
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
            get { return _relationships; }
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
                foreach (Relationship rel in _relationships)
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
                foreach (Relationship rel in _relationships)
                {
                    if (rel.RelationshipType == RelationshipType.Spouse)
                    {
                        if (rel is SpouseRelationship spouseRel &&
                            spouseRel.SpouseModifier == SpouseModifier.Current)
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
                foreach (Relationship rel in _relationships)
                {
                    if (rel.RelationshipType == RelationshipType.Spouse)
                    {
                        if (rel is SpouseRelationship spouseRel &&
                            spouseRel.SpouseModifier == SpouseModifier.Former)
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
                foreach (Relationship rel in _relationships)
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
                foreach (Relationship rel in _relationships)
                {
                    if (rel.RelationshipType == RelationshipType.Child)
                    {
                        if (rel is ChildRelationship childRel &&
                            childRel.ParentChildModifier == ParentChildModifier.Natural)
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
                foreach (Relationship rel in _relationships)
                {
                    if (rel.RelationshipType == RelationshipType.Child)
                    {
                        if (rel is ChildRelationship childRel &&
                            childRel.ParentChildModifier == ParentChildModifier.Adopted)
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
                foreach (Relationship rel in _relationships)
                {
                    if (rel.RelationshipType == RelationshipType.Child)
                    {
                        if (rel is ChildRelationship childRel &&
                            childRel.ParentChildModifier == ParentChildModifier.Foster)
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
                foreach (Relationship rel in _relationships)
                {

                    if (rel.RelationshipType == RelationshipType.Parent)
                    {
                        if (rel is ParentRelationship parentRel &&
                            parentRel.ParentChildModifier == ParentChildModifier.Natural)
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
                foreach (Relationship rel in _relationships)
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
                foreach (Relationship rel in _relationships)
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
                    ParentSet parentSet = new(Parents[0], Parents[1]);
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
                        ParentSet ps = new(parent, spouse);

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
                return (_restriction == Restriction.Locked || _restriction == Restriction.Private);
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
            get { return (_restriction == Restriction.Locked || _isLiving == true); }
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
            get { return (_restriction == Restriction.Locked); }
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
            get { return (_restriction == Restriction.Private); }
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
                if (_photos != null && _photos.Count > 0)
                {
                    foreach (Photo photo in _photos)
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
                if (_photos.Count > 0)
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
                if (_birthPlace == null || _birthPlace.Length == 0)
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
                if (_deathPlace == null || _deathPlace.Length == 0)
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
                if (_cremationPlace == null || _cremationPlace.Length == 0)
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
                if (_burialPlace == null || _burialPlace.Length == 0)
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
                if (((_religionSource == null || _religionSource.Length == 0) ||
                    (_religionCitation == null || _religionCitation.Length == 0)) &&
                     ((_burialSource == null || _burialSource.Length == 0) ||
                     (_burialCitation == null || _burialCitation.Length == 0)) &&
                     ((_deathSource == null || _deathSource.Length == 0) ||
                     (_deathCitation == null || _deathCitation.Length == 0)) &&
                     ((_educationSource == null || _educationSource.Length == 0) ||
                     (_educationCitation == null || _educationCitation.Length == 0)) &&
                     ((_birthSource == null || _birthSource.Length == 0) ||
                     (_birthCitation == null || _birthCitation.Length == 0)) &&
                     ((_occupationSource == null || _occupationSource.Length == 0) ||
                     (_occupationCitation == null || _occupationCitation.Length == 0)) &&
                     ((_cremationSource == null || _cremationSource.Length == 0) ||
                     (_cremationCitation == null || _cremationCitation.Length == 0)))
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
                if (_note == null || _note.ToString().Length == 0)
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

                if (_attachments.Count > 0)
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
                if (_gender == Gender.Male)
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
                foreach (Relationship rel in _relationships)
                {

                    if (rel.RelationshipType == RelationshipType.Parent)
                    {
                        if (rel is ParentRelationship parents &&
                            parents.ParentChildModifier == ParentChildModifier.Natural)
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
                if (_gender == Gender.Male)
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
                if (_gender == Gender.Male)
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

                if (_gender == Gender.Male)
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
                foreach (Relationship rel in _relationships)
                {

                    if (rel.RelationshipType == RelationshipType.Child)
                    {
                        if (rel is ChildRelationship children &&
                            children.ParentChildModifier == ParentChildModifier.Natural)
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

        #region Name Synchronization

        /// <summary>
        /// Syncs the primary name in the Names collection from the FirstName/LastName/Suffix fields
        /// </summary>
        private void SyncPrimaryNameFromFields()
        {
            if (_isSyncingNames || _names == null)
            {
                return;
            }

            _isSyncingNames = true;
            try
            {
                Name primaryName = _names.PrimaryName;
                if (primaryName != null)
                {
                    primaryName.FirstName = _firstName;
                    primaryName.Surname = _lastName;
                    primaryName.Suffix = _suffix;
                }
            }
            finally
            {
                _isSyncingNames = false;
            }
        }

        /// <summary>
        /// Syncs the FirstName/LastName/Suffix fields from the primary name in the Names collection
        /// </summary>
        private void SyncFieldsFromPrimaryName()
        {
            if (_isSyncingNames || _names == null)
            {
                return;
            }

            _isSyncingNames = true;
            try
            {
                Name primaryName = _names.PrimaryName;
                if (primaryName != null)
                {
                    _firstName = primaryName.FirstName ?? string.Empty;
                    _lastName = primaryName.Surname ?? string.Empty;
                    _suffix = primaryName.Suffix ?? string.Empty;

                    OnPropertyChanged(nameof(FirstName));
                    OnPropertyChanged(nameof(LastName));
                    OnPropertyChanged(nameof(Suffix));
                    OnPropertyChanged(nameof(Name));
                    OnPropertyChanged(nameof(FullName));
                }
            }
            finally
            {
                _isSyncingNames = false;
            }
        }

        /// <summary>
        /// Handles changes to the Names collection
        /// </summary>
        private void Names_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // When names are added or changed, sync the primary name to the legacy fields
            SyncFieldsFromPrimaryName();

            // Subscribe to property changes on new names
            if (e.NewItems != null)
            {
                foreach (Name name in e.NewItems)
                {
                    name.PropertyChanged += Name_PropertyChanged;
                }
            }

            // Unsubscribe from property changes on removed names
            if (e.OldItems != null)
            {
                foreach (Name name in e.OldItems)
                {
                    name.PropertyChanged -= Name_PropertyChanged;
                }
            }
        }

        /// <summary>
        /// Handles property changes on individual Name objects
        /// </summary>
        private void Name_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Name changedName && changedName.IsPrimary)
            {
                // If the primary name changed, sync to legacy fields
                SyncFieldsFromPrimaryName();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of a person object.
        /// Each new instance will be given a unique identifier.
        /// This parameterless constructor is also required for serialization.
        /// </summary>
        public Person()
        {
            _id = Guid.NewGuid().ToString();
            _relationships = [];
            _photos = [];
            _attachments = [];
            _names = [];
            _firstName = Properties.Resources.Unknown;
            _isLiving = true;
            _restriction = Restriction.None;

            // Initialize with a default primary name
            _names.Add(new Name(_firstName, string.Empty) { IsPrimary = true });

            // Subscribe to collection changes to keep fields in sync
            _names.CollectionChanged += Names_CollectionChanged;
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
                _firstName = firstNames;
            }

            _lastName = lastName;
        }

        /// <summary>
        /// Creates a new instance of the person class with the firstname, the lastname, and gender
        /// </summary>
        public Person(string firstName, string lastName, Gender gender)
            : this(firstName, lastName)
        {
            _gender = gender;
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            foreach (Relationship relationship in _relationships)
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
            foreach (Relationship relationship in _relationships)
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
                ParentSet ps = new(this, spouse);

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
            if (_story != null)
            {
                _story.Delete();
                _story = null;
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

                if (columnName == "BirthDate" && DateWrapper.IsDateEmpty(BirthDate))
                {
                    result = Properties.Resources.InvalidDate;
                }

                if (columnName == "DeathDate" && DateWrapper.IsDateEmpty(DeathDate))
                {
                    result = Properties.Resources.InvalidDate;
                }

                if (columnName == "CremationDate" && DateWrapper.IsDateEmpty(CremationDate))
                {
                    result = Properties.Resources.InvalidDate;
                }

                if (columnName == "BurialDate" && DateWrapper.IsDateEmpty(BurialDate))
                {
                    result = Properties.Resources.InvalidDate;
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
