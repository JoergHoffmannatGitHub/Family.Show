using System.Globalization;

namespace FamilyShowLib.Tests;

public class PersonTest
{
  private static readonly Person s_sut = new("John", "Doe", Gender.Male);

  [Fact]
  public void Equality_Null()
  {
    // Act
    Person nullPerson = null!;

    // Assert
    Assert.False(s_sut.Equals(null));
    Assert.Null(nullPerson);
  }

  [Fact]
  public void Equality_Assigned()
  {
    // Act
    Person assignedPerson = s_sut;

    // Assert
    Assert.True(s_sut.Equals(assignedPerson));
    Assert.True(s_sut.Equals((object)assignedPerson));

    Assert.True(s_sut == assignedPerson);
    Assert.True(assignedPerson == s_sut);
    Assert.False(s_sut != assignedPerson);
    Assert.False(assignedPerson != s_sut);

    Assert.Equal(s_sut.GetHashCode(), assignedPerson.GetHashCode());
  }

  [Fact]
  public void Equality_SameId()
  {
    // Act
    Person samePerson = new("John", "Doe", Gender.Male)
    {
      Id = s_sut.Id
    };

    // Assert
    Assert.True(s_sut.Equals(samePerson));
    Assert.True(s_sut.Equals((object)samePerson));

    Assert.True(s_sut == samePerson);
    Assert.True(samePerson == s_sut);
    Assert.False(s_sut != samePerson);
    Assert.False(samePerson != s_sut);

    Assert.Equal(s_sut.GetHashCode(), samePerson.GetHashCode());
  }

  [Fact]
  public void Equality_Different()
  {
    // Act
    Person differentPerson = new("John", "Doe", Gender.Male);

    // Assert
    Assert.False(s_sut.Equals(differentPerson));
    Assert.False(s_sut.Equals((object)differentPerson));

    Assert.False(s_sut == differentPerson);
    Assert.True(s_sut != differentPerson);

    Assert.NotEqual(s_sut.GetHashCode(), differentPerson.GetHashCode());
  }

  [Fact]
  public void Age_NoBirthDate_ReturnsNull()
  {
    // Arrange
    Person person = new();

    // Act
    int? age = person.Age;

    // Assert
    Assert.Null(age);
  }

  [Fact]
  public void Age_LivingPerson_ReturnsCorrectAge()
  {
    // Arrange
    Person person = new()
    {
      BirthDate = new(2000, 1, 1),
    };

    // Act
    int? age = person.Age;

    // Assert
    Assert.Equal(DateTime.Now.Year - 2000, age);
  }

  [Fact]
  public void Age_DeceasedPersonWithDeathDate_ReturnsCorrectAge()
  {
    // Arrange
    Person person = new()
    {
      BirthDate = new DateWrapper(2000, 1, 1),
      DeathDate = new DateWrapper(2001, 1, 1),
      IsLiving = false
    };

    // Act
    int? age = person.Age;

    // Assert
    Assert.Equal(1, age);
  }

  [Fact]
  public void Age_DeceasedPersonWithoutDeathDate_ReturnsNull()
  {
    // Arrange
    Person person = new()
    {
      BirthDate = new(2000, 1, 1),
      IsLiving = false
    };

    // Act
    int? age = person.Age;

    // Assert
    Assert.Null(age);
  }

  [Fact]
  public void Age_BirthdayNotYetOccurredThisYear_ReturnsCorrectAge()
  {
    // Arrange
    Person person = new()
    {
      BirthDate = new(2000, DateTime.Now.Month + 1, 1),
    };

    // Act
    int? age = person.Age;

    // Assert
    Assert.Equal(DateTime.Now.Year - 2000 - 1, age);
  }

  [Fact]
  public void Age_BirthdayOccurredThisYear_ReturnsCorrectAge()
  {
    // Arrange
    Person person = new()
    {
      BirthDate = new(2000, DateTime.Now.Month - 1, 1),
    };

    // Act
    int? age = person.Age;

    // Assert
    Assert.Equal(DateTime.Now.Year - 2000, age);
  }

  [Fact]
  public void YearOfBirth_NoBirthDate_ReturnsHyphen()
  {
    // Arrange
    Person person = new();

    // Act
    string yearOfBirth = person.YearOfBirth;

    // Assert
    Assert.Equal("-", yearOfBirth);
  }

  [Fact]
  public void YearOfBirth_WithBirthDate_ReturnsYear()
  {
    // Arrange
    Person person = new()
    {
      BirthDate = new(2000, 1, 1)
    };

    // Act
    string yearOfBirth = person.YearOfBirth;

    // Assert
    Assert.Equal("2000", yearOfBirth);
  }

  [Fact]
  public void YearOfDeath_NoDeathDate_ReturnsHyphen()
  {
    // Arrange
    Person person = new();

    // Act
    string yearOfDeath = person.YearOfDeath;

    // Assert
    Assert.Equal("-", yearOfDeath);
  }

  [Fact]
  public void YearOfDeath_WithDeathDate_ReturnsYear()
  {
    // Arrange
    Person person = new()
    {
      DeathDate = new(2000, 1, 1),
      IsLiving = false
    };

    // Act
    string yearOfDeath = person.YearOfDeath;

    // Assert
    Assert.Equal("2000", yearOfDeath);
  }

  [Fact]
  public void BirthDate_SetBirthDate_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    DateWrapper birthDate = new(2000, 1, 1);

    // Act
    person.BirthDate = birthDate;

    // Assert
    Assert.Equal(birthDate, person.BirthDate);
  }

  [Fact]
  public void BirthDate_SetBirthDateTwice_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    DateWrapper birthDate = new(2000, 1, 1);

    // Act
    person.BirthDate = birthDate;
    person.BirthDate = birthDate;

    // Assert
    Assert.Equal(birthDate, person.BirthDate);
  }

  [Fact]
  public void BirthDate_SetNullBirthDate_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    DateWrapper? birthDate = null;

    // Act
    person.BirthDate = birthDate;

    // Assert
    Assert.Null(person.BirthDate);
  }

  [Fact]
  public void BirthDate_SetBirthDate_TriggersPropertyChanged()
  {
    // Arrange
    Person person = new();
    bool propertyBirthDateChangedTriggered = false;
    person.PropertyChanged += (sender, e) =>
    {
      if (e.PropertyName == nameof(Person.BirthDate))
      {
        propertyBirthDateChangedTriggered = true;
      }
    };
    bool propertAgeChangedTriggered = false;
    person.PropertyChanged += (sender, e) =>
    {
      if (e.PropertyName == nameof(Person.Age))
      {
        propertAgeChangedTriggered = true;
      }
    };
    bool propertAgeGroupChangedTriggered = false;
    person.PropertyChanged += (sender, e) =>
    {
      if (e.PropertyName == nameof(Person.AgeGroup))
      {
        propertAgeGroupChangedTriggered = true;
      }
    };
    bool propertYearOfBirthChangedTriggered = false;
    person.PropertyChanged += (sender, e) =>
    {
      if (e.PropertyName == nameof(Person.YearOfBirth))
      {
        propertYearOfBirthChangedTriggered = true;
      }
    };
    bool propertBirthMonthAndDayChangedTriggered = false;
    person.PropertyChanged += (sender, e) =>
    {
      if (e.PropertyName == nameof(Person.BirthMonthAndDay))
      {
        propertBirthMonthAndDayChangedTriggered = true;
      }
    };
    bool propertBirthDateAndPlaceChangedTriggered = false;
    person.PropertyChanged += (sender, e) =>
    {
      if (e.PropertyName == nameof(Person.BirthDateAndPlace))
      {
        propertBirthDateAndPlaceChangedTriggered = true;
      }
    };

    // Act
    person.BirthDate = new(2000, 1, 1);

    // Assert
    Assert.True(propertyBirthDateChangedTriggered);
    Assert.True(propertAgeChangedTriggered);
    Assert.True(propertAgeGroupChangedTriggered);
    Assert.True(propertYearOfBirthChangedTriggered);
    Assert.True(propertBirthMonthAndDayChangedTriggered);
    Assert.True(propertBirthDateAndPlaceChangedTriggered);
  }

  [Theory, CombinatorialData]
  public void BirthDate_SetBirthDate_UpdatesRelatedProperties(
    [CombinatorialValues("en-US", "en-GB", "it-IT", "es-ES", "fr-FR", "de-DE")] string culture
    )
  {
    using (new AnotherCulture(culture))
    {
      // Arrange
      Person person = new();
      DateTime birthDate = new(2000, 1, 1);
      DateWrapper birthDateWrapper = new(birthDate.Year, birthDate.Month, birthDate.Day);

      // Act
      person.BirthDate = birthDateWrapper;

      // Assert
      Assert.Equal(birthDateWrapper, person.BirthDate);
      Assert.Equal(DateTime.Now.Year - birthDate.Year, person.Age);
      Assert.Equal("2000", person.YearOfBirth);
      Assert.Equal(birthDate.ToString(DateTimeFormatInfo.CurrentInfo.MonthDayPattern, CultureInfo.CurrentCulture), person.BirthMonthAndDay);
      Assert.Equal($"Born {birthDate.ToString(DateTimeFormatInfo.CurrentInfo.ShortDatePattern, CultureInfo.CurrentCulture)}", person.BirthDateAndPlace);
    }
  }

  [Fact]
  public void BirthDateDescriptor_SetBirthDateDescriptor_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    string birthDateDescriptor = "Approx. 2000";

    // Act
    person.BirthDateDescriptor = birthDateDescriptor;

    // Assert
    Assert.Equal(birthDateDescriptor, person.BirthDateDescriptor);
  }

  [Fact]
  public void BirthDateDescriptor_SetNullBirthDateDescriptor_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    string? birthDateDescriptor = null;

    // Act
    person.BirthDateDescriptor = birthDateDescriptor;

    // Assert
    Assert.Null(person.BirthDateDescriptor);
  }

  [Fact]
  public void BirthDateDescriptor_SetBirthDateDescriptor_TriggersPropertyChanged()
  {
    // Arrange
    Person person = new();
    bool propertyChangedTriggered = false;
    person.PropertyChanged += (sender, e) =>
    {
      if (e.PropertyName == nameof(Person.BirthDateDescriptor))
      {
        propertyChangedTriggered = true;
      }
    };

    // Act
    person.BirthDateDescriptor = "Approx. 2000";

    // Assert
    Assert.True(propertyChangedTriggered);
  }

  [Fact]
  public void BirthMonthAndDay_SetBirthDate_UpdatesProperty()
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      Person person = new();
      DateTime birthDate = new(2000, 1, 1);
      DateWrapper birthDateWrapper = new(birthDate.Year, birthDate.Month, birthDate.Day);

      // Act
      person.BirthDate = birthDateWrapper;

      // Assert
      Assert.Equal(birthDate.ToString(DateTimeFormatInfo.CurrentInfo.MonthDayPattern, CultureInfo.CurrentCulture), person.BirthMonthAndDay);
    }
  }

  [Fact]
  public void BirthMonthAndDay_SetNullBirthDate_ReturnsNull()
  {
    // Arrange
    Person person = new()
    {
      // Act
      BirthDate = null
    };

    // Assert
    Assert.Null(person.BirthMonthAndDay);
  }

  [Fact]
  public void BirthDateAndPlace_SetBirthDateAndPlace_UpdatesProperty()
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      Person person = new();
      DateTime birthDate = new(2000, 1, 1);
      DateWrapper birthDateWrapper = new(birthDate.Year, birthDate.Month, birthDate.Day);
      string birthPlace = "New York";

      // Act
      person.BirthDate = birthDateWrapper;
      person.BirthPlace = birthPlace;

      // Assert
      Assert.Equal($"Born {birthDate.ToString(DateTimeFormatInfo.CurrentInfo.ShortDatePattern, CultureInfo.CurrentCulture)}, {birthPlace}", person.BirthDateAndPlace);
    }
  }

  [Fact]
  public void BirthDateAndPlace_SetBirthDateOnly_UpdatesProperty()
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      Person person = new();
      DateTime birthDate = new(2000, 1, 1);
      DateWrapper birthDateWrapper = new(birthDate.Year, birthDate.Month, birthDate.Day);

      // Act
      person.BirthDate = birthDateWrapper;

      // Assert
      Assert.Equal($"Born {birthDate.ToString(DateTimeFormatInfo.CurrentInfo.ShortDatePattern, CultureInfo.CurrentCulture)}", person.BirthDateAndPlace);
    }
  }

  [Fact]
  public void BirthDateAndPlace_SetBirthPlaceOnly_ReturnsNull()
  {
    // Arrange
    Person person = new();
    string birthPlace = "New York";

    // Act
    person.BirthPlace = birthPlace;

    // Assert
    Assert.Null(person.BirthDateAndPlace);
  }

  [Fact]
  public void BirthDateAndPlace_SetNullBirthDateAndPlace_ReturnsNull()
  {
    // Arrange
    Person person = new()
    {
      // Act
      BirthDate = null,
      BirthPlace = null
    };

    // Assert
    Assert.Null(person.BirthDateAndPlace);
  }

  [Fact]
  public void DeathDate_SetDeathDate_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    DateWrapper deathDate = new(2000, 1, 1);

    // Act
    person.DeathDate = deathDate;

    // Assert
    Assert.Equal(deathDate, person.DeathDate);
  }

  [Fact]
  public void DeathDate_SetDeathDateTwice_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    DateWrapper deathDate = new(2000, 1, 1);

    // Act
    person.DeathDate = deathDate;
    person.DeathDate = deathDate;

    // Assert
    Assert.Equal(deathDate, person.DeathDate);
  }

  [Fact]
  public void DeathDate_SetNullDeathDate_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    DateWrapper? deathDate = null;

    // Act
    person.DeathDate = deathDate;

    // Assert
    Assert.Null(person.DeathDate);
  }

  [Fact]
  public void DeathDate_SetDeathDate_TriggersPropertyChanged()
  {
    // Arrange
    Person person = new();
    bool propertyDeathDateChangedTriggered = false;
    person.PropertyChanged += (sender, e) =>
    {
      if (e.PropertyName == nameof(Person.DeathDate))
      {
        propertyDeathDateChangedTriggered = true;
      }
    };
    bool propertyAgeChangedTriggered = false;
    person.PropertyChanged += (sender, e) =>
    {
      if (e.PropertyName == nameof(Person.Age))
      {
        propertyAgeChangedTriggered = true;
      }
    };
    bool propertyYearOfDeathChangedTriggered = false;
    person.PropertyChanged += (sender, e) =>
    {
      if (e.PropertyName == nameof(Person.YearOfDeath))
      {
        propertyYearOfDeathChangedTriggered = true;
      }
    };

    // Act
    person.DeathDate = new(2000, 1, 1);

    // Assert
    Assert.True(propertyDeathDateChangedTriggered);
    Assert.True(propertyAgeChangedTriggered);
    Assert.True(propertyYearOfDeathChangedTriggered);
  }

  [Fact]
  public void DeathDate_SetDeathDate_UpdatesRelatedProperties()
  {
    // Arrange
    Person person = new()
    {
      IsLiving = false
    };
    DateWrapper deathDate = new(2000, 1, 1);

    // Act
    person.DeathDate = deathDate;

    // Assert
    Assert.Equal(deathDate, person.DeathDate);
    Assert.Equal("2000", person.YearOfDeath);
  }

  [Fact]
  public void DeathDateDescriptor_SetDeathDateDescriptor_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    string deathDateDescriptor = "Approx. 2000";

    // Act
    person.DeathDateDescriptor = deathDateDescriptor;

    // Assert
    Assert.Equal(deathDateDescriptor, person.DeathDateDescriptor);
  }

  [Fact]
  public void DeathDateDescriptor_SetNullDeathDateDescriptor_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    string? deathDateDescriptor = null;

    // Act
    person.DeathDateDescriptor = deathDateDescriptor;

    // Assert
    Assert.Null(person.DeathDateDescriptor);
  }

  [Fact]
  public void DeathDateDescriptor_SetDeathDateDescriptor_TriggersPropertyChanged()
  {
    // Arrange
    Person person = new();
    bool propertyChangedTriggered = false;
    person.PropertyChanged += (sender, e) =>
    {
      if (e.PropertyName == nameof(Person.DeathDateDescriptor))
      {
        propertyChangedTriggered = true;
      }
    };

    // Act
    person.DeathDateDescriptor = "Approx. 2000";

    // Assert
    Assert.True(propertyChangedTriggered);
  }

  [Fact]
  public void CremationDate_SetCremationDate_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    DateWrapper cremationDate = new(2000, 1, 1);

    // Act
    person.CremationDate = cremationDate;

    // Assert
    Assert.Equal(cremationDate, person.CremationDate);
  }

  [Fact]
  public void CremationDate_SetCremationDateTwice_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    DateWrapper cremationDate = new(2000, 1, 1);

    // Act
    person.CremationDate = cremationDate;
    person.CremationDate = cremationDate;

    // Assert
    Assert.Equal(cremationDate, person.CremationDate);
  }

  [Fact]
  public void CremationDate_SetNullCremationDate_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    DateWrapper? cremationDate = null;

    // Act
    person.CremationDate = cremationDate;

    // Assert
    Assert.Null(person.CremationDate);
  }

  [Fact]
  public void CremationDate_SetCremationDate_TriggersPropertyChanged()
  {
    // Arrange
    Person person = new();
    bool propertyChangedTriggered = false;
    person.PropertyChanged += (sender, e) =>
    {
      if (e.PropertyName == nameof(Person.CremationDate))
      {
        propertyChangedTriggered = true;
      }
    };

    // Act
    person.CremationDate = new(2000, 1, 1);

    // Assert
    Assert.True(propertyChangedTriggered);
  }

  [Fact]
  public void CremationDateDescriptor_SetCremationDateDescriptor_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    string cremationDateDescriptor = "Approx. 2000";

    // Act
    person.CremationDateDescriptor = cremationDateDescriptor;

    // Assert
    Assert.Equal(cremationDateDescriptor, person.CremationDateDescriptor);
  }

  [Fact]
  public void CremationDateDescriptor_SetNullCremationDateDescriptor_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    string? cremationDateDescriptor = null;

    // Act
    person.CremationDateDescriptor = cremationDateDescriptor;

    // Assert
    Assert.Null(person.CremationDateDescriptor);
  }

  [Fact]
  public void CremationDateDescriptor_SetCremationDateDescriptor_TriggersPropertyChanged()
  {
    // Arrange
    Person person = new();
    bool propertyChangedTriggered = false;
    person.PropertyChanged += (sender, e) =>
    {
      if (e.PropertyName == nameof(Person.CremationDateDescriptor))
      {
        propertyChangedTriggered = true;
      }
    };

    // Act
    person.CremationDateDescriptor = "Approx. 2000";

    // Assert
    Assert.True(propertyChangedTriggered);
  }

  [Fact]
  public void BurialDate_SetBurialDate_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    DateWrapper burialDate = new(2000, 1, 1);

    // Act
    person.BurialDate = burialDate;

    // Assert
    Assert.Equal(burialDate, person.BurialDate);
  }

  [Fact]
  public void BurialDate_SetBurialDateTwice_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    DateWrapper burialDate = new(2000, 1, 1);

    // Act
    person.BurialDate = burialDate;
    person.BurialDate = burialDate;

    // Assert
    Assert.Equal(burialDate, person.BurialDate);
  }

  [Fact]
  public void BurialDate_SetNullBurialDate_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    DateWrapper? burialDate = null;

    // Act
    person.BurialDate = burialDate;

    // Assert
    Assert.Null(person.BurialDate);
  }

  [Fact]
  public void BurialDate_SetBurialDate_TriggersPropertyChanged()
  {
    // Arrange
    Person person = new();
    bool propertyChangedTriggered = false;
    person.PropertyChanged += (sender, e) =>
    {
      if (e.PropertyName == nameof(Person.BurialDate))
      {
        propertyChangedTriggered = true;
      }
    };

    // Act
    person.BurialDate = new(2000, 1, 1);

    // Assert
    Assert.True(propertyChangedTriggered);
  }

  [Fact]
  public void BurialDateDescriptor_SetBurialDateDescriptor_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    string burialDateDescriptor = "Approx. 2000";

    // Act
    person.BurialDateDescriptor = burialDateDescriptor;

    // Assert
    Assert.Equal(burialDateDescriptor, person.BurialDateDescriptor);
  }

  [Fact]
  public void BurialDateDescriptor_SetNullBurialDateDescriptor_UpdatesProperty()
  {
    // Arrange
    Person person = new();
    string? burialDateDescriptor = null;

    // Act
    person.BurialDateDescriptor = burialDateDescriptor;

    // Assert
    Assert.Null(person.BurialDateDescriptor);
  }

  [Fact]
  public void BurialDateDescriptor_SetBurialDateDescriptor_TriggersPropertyChanged()
  {
    // Arrange
    Person person = new();
    bool propertyChangedTriggered = false;
    person.PropertyChanged += (sender, e) =>
    {
      if (e.PropertyName == nameof(Person.BurialDateDescriptor))
      {
        propertyChangedTriggered = true;
      }
    };

    // Act
    person.BurialDateDescriptor = "Approx. 2000";

    // Assert
    Assert.True(propertyChangedTriggered);
  }

  [Fact]
  public void Indexer_InvalidBirthDate_ReturnsError()
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      Person person = new() { BirthDate = new("Invalid") };

      // Act
      string error = person["BirthDate"];

      // Assert
      Assert.Equal(Properties.Resources.InvalidDate, error);
    }
  }

  [Fact]
  public void Indexer_ValidBirthDate_ReturnsNull()
  {
    // Arrange
    Person person = new() { BirthDate = new(2000, 1, 1) };

    // Act
    string error = person["BirthDate"];

    // Assert
    Assert.Null(error);
  }

  [Fact]
  public void Indexer_WithoutBirthDate_ReturnsNull()
  {
    // Arrange
    Person person = new();

    // Act
    string error = person["BirthDate"];

    // Assert
    Assert.Null(error);
  }

  [Fact]
  public void Indexer_InvalidDeathDate_ReturnsError()
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      Person person = new() { DeathDate = new("Invalid") };

      // Act
      string error = person["DeathDate"];

      // Assert
      Assert.Equal(Properties.Resources.InvalidDate, error);
    }
  }

  [Fact]
  public void Indexer_ValidDeathDate_ReturnsNull()
  {
    // Arrange
    Person person = new() { DeathDate = new(2000, 1, 1) };

    // Act
    string error = person["DeathDate"];

    // Assert
    Assert.Null(error);
  }

  [Fact]
  public void Indexer_WithoutDeathDate_ReturnsNull()
  {
    // Arrange
    Person person = new();

    // Act
    string error = person["DeathDate"];

    // Assert
    Assert.Null(error);
  }

  [Fact]
  public void Indexer_InvalidCremationDate_ReturnsError()
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      Person person = new() { CremationDate = new("Invalid") };

      // Act
      string error = person["CremationDate"];

      // Assert
      Assert.Equal(Properties.Resources.InvalidDate, error);
    }
  }

  [Fact]
  public void Indexer_ValidCremationDate_ReturnsNull()
  {
    // Arrange
    Person person = new() { CremationDate = new(2000, 1, 1) };

    // Act
    string error = person["CremationDate"];

    // Assert
    Assert.Null(error);
  }

  [Fact]
  public void Indexer_WithoutCremationDate_ReturnsNull()
  {
    // Arrange
    Person person = new();

    // Act
    string error = person["CremationDate"];

    // Assert
    Assert.Null(error);
  }

  [Fact]
  public void Indexer_InvalidBurialDate_ReturnsError()
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      Person person = new() { BurialDate = new("Invalid") };

      // Act
      string error = person["BurialDate"];

      // Assert
      Assert.Equal(Properties.Resources.InvalidDate, error);
    }
  }

  [Fact]
  public void Indexer_ValidBurialDate_ReturnsNull()
  {
    // Arrange
    Person person = new() { BurialDate = new(2000, 1, 1) };

    // Act
    string error = person["BurialDate"];

    // Assert
    Assert.Null(error);
  }

  [Fact]
  public void Indexer_WithoutBurialDate_ReturnsNull()
  {
    // Arrange
    Person person = new();

    // Act
    string error = person["BurialDate"];

    // Assert
    Assert.Null(error);
  }
}

public class ParentSetTest
{
  private static readonly Person s_john = new("John", "Doe", Gender.Male);
  private static readonly Person s_jane = new("Jane", "Doe", Gender.Female);

  [Fact]
  public void Equality_Null()
  {
    // Act
    ParentSet sut = new(s_john, s_jane);
    ParentSet nullParentSet = null!;

    // Assert
    Assert.False(sut.Equals(null));
    Assert.Null(nullParentSet);
  }

  [Fact]
  public void Equality_Assigned()
  {
    // Act
    ParentSet sut = new(s_john, s_jane);
    ParentSet assignedParentSet = sut;

    // Assert
    Assert.True(sut.Equals(assignedParentSet));
    Assert.True(sut.Equals((object)assignedParentSet));

    Assert.True(sut == assignedParentSet);
    Assert.True(assignedParentSet == sut);
    Assert.False(sut != assignedParentSet);
    Assert.False(assignedParentSet != sut);

    Assert.Equal(sut.GetHashCode(), assignedParentSet.GetHashCode());
  }

  [Fact]
  public void Equality_SameParents()
  {
    // Act
    ParentSet sut = new(s_john, s_jane);
    ParentSet sameParentSet = new(s_john, s_jane);

    // Assert
    Assert.True(sut.Equals(sameParentSet));
    Assert.True(sut.Equals((object)sameParentSet));

    Assert.True(sut == sameParentSet);
    Assert.True(sameParentSet == sut);
    Assert.False(sut != sameParentSet);
    Assert.False(sameParentSet != sut);

    Assert.Equal(sut.GetHashCode(), sameParentSet.GetHashCode());
  }

  [Fact]
  public void Equality_SameParentsDifferentOrder()
  {
    // Act
    ParentSet sut = new(s_john, s_jane);
    ParentSet sameParentSet = new(s_jane, s_john);

    // Assert
    Assert.True(sut.Equals(sameParentSet));
    Assert.True(sut.Equals((object)sameParentSet));

    Assert.True(sut == sameParentSet);
    Assert.True(sameParentSet == sut);
    Assert.False(sut != sameParentSet);
    Assert.False(sameParentSet != sut);

    Assert.Equal(sut.GetHashCode(), sameParentSet.GetHashCode());
  }

  [Fact]
  public void Equality_Different()
  {
    // Act
    ParentSet sut = new(s_john, s_jane);
    ParentSet differentParentSet = new(s_jane, s_jane);

    // Assert
    Assert.False(sut.Equals(differentParentSet));
    Assert.False(sut.Equals((object)differentParentSet));

    Assert.False(sut == differentParentSet);
    Assert.True(sut != differentParentSet);

    Assert.NotEqual(sut.GetHashCode(), differentParentSet.GetHashCode());
  }
}
