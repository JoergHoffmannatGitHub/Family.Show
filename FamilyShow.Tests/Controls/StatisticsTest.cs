using FamilyShowLib;

namespace FamilyShow.Tests.Controls;

[CollectionDefinition("SequentialTest", DisableParallelization = true)]

[Collection("SequentialTest")]
public class StatisticsTest
{
    private static void Marry(Person husband, Person wife)
    {
        SpouseRelationship currentRelationship = new(husband, SpouseModifier.Current)
        {
            MarriageCitation = "Citation1",
            MarriageSource = "Source1",
            MarriagePlace = "Place1",
            MarriageDate = new DateWrapper(2000, 1, 1)
        };
        wife.Relationships.Add(currentRelationship);
        currentRelationship = new(wife, SpouseModifier.Current)
        {
            MarriageCitation = "Citation1",
            MarriageSource = "Source1",
            MarriagePlace = "Place1",
            MarriageDate = new DateWrapper(2000, 1, 1)
        };
        husband.Relationships.Add(currentRelationship);
    }

    private static void Divorce(Person husband, Person wife)
    {
        SpouseRelationship formerRelationship = new(husband, SpouseModifier.Former)
        {
            DivorceCitation = "Citation2",
            DivorceSource = "Source2",
            DivorceDate = new DateWrapper(2010, 1, 1)
        };
        wife.Relationships.Add(formerRelationship);
        formerRelationship = new(wife, SpouseModifier.Former)
        {
            DivorceCitation = "Citation2",
            DivorceSource = "Source2",
            DivorceDate = new DateWrapper(2010, 1, 1)
        };
        husband.Relationships.Add(formerRelationship);
    }

    [StaFact]
    public void DisplayStats_ShouldCountCitationsCorrectly()
    {
        using (AnotherCulture.UnitedStates())
        {
            // Arrange
            var peopleCollection = new PeopleCollection
      {
        new Person("John", "Doe")
        {
            ReligionSource = "Source1",
            ReligionCitation = "Citation1",
            Religion = "Christianity",

            CremationSource = "Source2",
            CremationCitation = "Citation2",
            CremationPlace = "Place1",
            CremationDate = new DateWrapper(2000, 1, 1),

            OccupationSource = "Source3",
            OccupationCitation = "Citation3",
            Occupation = "Engineer",

            EducationSource = "Source4",
            EducationCitation = "Citation4",
            Education = "College",

            BirthSource = "Source5",
            BirthCitation = "Citation5",
            BirthPlace = "Place2",
            BirthDate = new DateWrapper(1980, 1, 1),

            DeathSource = "Source6",
            DeathCitation = "Citation6",
            DeathPlace = "Place3",
            DeathDate = new DateWrapper(2020, 1, 1),

            BurialSource = "Source7",
            BurialCitation = "Citation7",
            BurialPlace = "Place4",
            BurialDate = new DateWrapper(2020, 1, 2)
        }
      };
            var sourceCollection = new SourceCollection();
            var repositoryCollection = new RepositoryCollection();
            var statistics = new Statistics();

            // Act
            statistics.DisplayStats(peopleCollection, sourceCollection, repositoryCollection);

            // Assert
            Assert.Equal("Citations: 7", statistics.Citations.Text);

            SharedBirthdays.s_lcv = null;
        }
    }

    [StaFact]
    public void DisplayStats_ShouldCountMarriagesAndDivorcesCorrectly()
    {
        using (AnotherCulture.UnitedStates())
        {
            // Arrange
            Person husband = new("John", "Doe");
            Person currenWife = new("Jane", "Doe");
            Marry(husband, currenWife);
            Person formerWife = new("Jane", "Smith");
            Divorce(husband, formerWife);
            PeopleCollection peopleCollection = [husband, currenWife, formerWife];
            var sourceCollection = new SourceCollection();
            var repositoryCollection = new RepositoryCollection();
            var statistics = new Statistics();

            // Act
            statistics.DisplayStats(peopleCollection, sourceCollection, repositoryCollection);

            // Assert
            Assert.Equal("Marriages: 2", statistics.Marriages.Text);
            Assert.Equal("Divorces: 1", statistics.Divorces.Text);

            SharedBirthdays.s_lcv = null;
        }
    }

    [StaFact]
    public void DisplayStats_ShouldCountOtherEventsCorrectly()
    {
        using (AnotherCulture.UnitedStates())
        {
            // Arrange
            var peopleCollection = new PeopleCollection
      {
        new Person("John", "Doe")
        {
          Religion = "Christianity",
          Education = "College",
          Occupation = "Engineer",
          BurialDate = new DateWrapper(2020, 1, 2), BurialPlace = "Place4",
          CremationDate = new DateWrapper(2000, 1, 1), CremationPlace = "Place1",
          DeathDate = new DateWrapper(2020, 1, 1), DeathPlace = "Place3",
          BirthDate = new DateWrapper(1980, 1, 1), BirthPlace = "Place2"
        }
      };
            var sourceCollection = new SourceCollection();
            var repositoryCollection = new RepositoryCollection();
            var statistics = new Statistics();

            // Act
            statistics.DisplayStats(peopleCollection, sourceCollection, repositoryCollection);

            // Assert
            Assert.Equal("Facts/Events: 7", statistics.TotalFactsEvents.Text);

            SharedBirthdays.s_lcv = null;
        }
    }
}
