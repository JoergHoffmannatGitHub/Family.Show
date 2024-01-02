namespace FamilyShowLib.Tests;

public class PersonTest
{
  private static readonly Person s_sut = new("John", "Doe", Gender.Male);

  [Fact]
  public void Equality_Null()
  {
    Person nullPerson = null!;

    Assert.False(s_sut.Equals(null));
    Assert.True(nullPerson == null);
  }

  [Fact]
  public void Equality_Assigned()
  {
    Person assignedPerson = s_sut;

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
    Person samePerson = new("John", "Doe", Gender.Male)
    {
      Id = s_sut.Id
    };

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
    Person differentPerson = new("John", "Doe", Gender.Male);

    Assert.False(s_sut.Equals(differentPerson));
    Assert.False(s_sut.Equals((object)differentPerson));

    Assert.False(s_sut == differentPerson);
    Assert.True(s_sut != differentPerson);

    Assert.NotEqual(s_sut.GetHashCode(), differentPerson.GetHashCode());
  }
}

public class ParentSetTest
{
  private static readonly Person s_john = new("John", "Doe", Gender.Male);
  private static readonly Person s_jane = new("Jane", "Doe", Gender.Female);

  [Fact]
  public void Equality_Null()
  {
    ParentSet sut = new(s_john, s_jane);
    ParentSet nullParentSet = null!;

    Assert.False(sut.Equals(null));
    Assert.True(nullParentSet == null);
  }

  [Fact]
  public void Equality_Assigned()
  {
    ParentSet sut = new(s_john, s_jane);
    ParentSet assignedParentSet = sut;

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
    ParentSet sut = new(s_john, s_jane);
    ParentSet sameParentSet = new(s_john, s_jane);

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
    ParentSet sut = new(s_john, s_jane);
    ParentSet sameParentSet = new(s_jane, s_john);

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
    ParentSet sut = new(s_john, s_jane);
    ParentSet differentParentSet = new(s_jane, s_jane);

    Assert.False(sut.Equals(differentParentSet));
    Assert.False(sut.Equals((object)differentParentSet));

    Assert.False(sut == differentParentSet);
    Assert.True(sut != differentParentSet);

    Assert.NotEqual(sut.GetHashCode(), differentParentSet.GetHashCode());
  }
}
