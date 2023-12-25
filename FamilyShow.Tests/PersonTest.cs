using Microsoft.FamilyShowLib;

namespace FamilyShowLib.Tests;

public class PersonTest
{
  readonly Person sut = new("John", "Doe", Gender.Male);

  [Fact]
  public void Equality_Null()
  {
    Person nullPerson = null!;

    Assert.False(sut.Equals(null));
    Assert.True(nullPerson == null);
  }

  [Fact]
  public void Equality_Assigned()
  {
    Person assignedPerson = sut;

    Assert.True(sut.Equals(assignedPerson));
    Assert.True(sut.Equals((object)assignedPerson));

    Assert.True(sut == assignedPerson);
    Assert.True(assignedPerson == sut);
    Assert.False(sut != assignedPerson);
    Assert.False(assignedPerson != sut);

    Assert.Equal(sut.GetHashCode(), assignedPerson.GetHashCode());
  }

  [Fact]
  public void Equality_SameId()
  {
    Person samePerson = new("John", "Doe", Gender.Male)
    {
      Id = sut.Id
    };

    Assert.True(sut.Equals(samePerson));
    Assert.True(sut.Equals((object)samePerson));

    Assert.True(sut == samePerson);
    Assert.True(samePerson == sut);
    Assert.False(sut != samePerson);
    Assert.False(samePerson != sut);

    Assert.Equal(sut.GetHashCode(), samePerson.GetHashCode());
  }

  [Fact]
  public void Equality_Different()
  {
    Person differentPerson = new("John", "Doe", Gender.Male);

    Assert.False(sut.Equals(differentPerson));
    Assert.False(sut.Equals((object)differentPerson));

    Assert.False(sut == differentPerson);
    Assert.True(sut != differentPerson);

    Assert.NotEqual(sut.GetHashCode(), differentPerson.GetHashCode());
  }
}

public class ParentSetTest
{
  readonly Person John = new("John", "Doe", Gender.Male);
  readonly Person Jane = new("Jane", "Doe", Gender.Female);

  [Fact]
  public void Equality_Null()
  {
    ParentSet sut = new(John, Jane);
    ParentSet nullParentSet = null!;

    Assert.False(sut.Equals(null));
    Assert.True(nullParentSet == null);
  }

  [Fact]
  public void Equality_Assigned()
  {
    ParentSet sut = new(John, Jane);
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
    ParentSet sut = new(John, Jane);
    ParentSet sameParentSet = new(John, Jane);

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
    ParentSet sut = new(John, Jane);
    ParentSet sameParentSet = new(Jane, John);

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
    ParentSet sut = new(John, Jane);
    ParentSet differentParentSet = new(Jane, Jane);

    Assert.False(sut.Equals(differentParentSet));
    Assert.False(sut.Equals((object)differentParentSet));

    Assert.False(sut == differentParentSet);
    Assert.True(sut != differentParentSet);

    Assert.NotEqual(sut.GetHashCode(), differentParentSet.GetHashCode());
  }
}
