namespace FamilyShowLib.Tests;

public class RepositoryTest
{
    private static readonly Repository s_sut = new("R1", "Archive", "here");

    [Fact]
    public void Equality_Null()
    {
        Repository nullRepository = null!;

        Assert.False(s_sut.Equals(null));
        Assert.Null(nullRepository);
    }

    [Fact]
    public void Equality_Assigned()
    {
        Repository assignedRepository = s_sut;

        Assert.True(s_sut.Equals(assignedRepository));
        Assert.True(s_sut.Equals((object)assignedRepository));

        Assert.True(s_sut == assignedRepository);
        Assert.True(assignedRepository == s_sut);
        Assert.False(s_sut != assignedRepository);
        Assert.False(assignedRepository != s_sut);

        Assert.Equal(s_sut.GetHashCode(), assignedRepository.GetHashCode());
    }

    [Fact]
    public void Equality_SameId()
    {
        Repository sameRepository = new("R1", "Archive", "here");

        Assert.True(s_sut.Equals(sameRepository));
        Assert.True(s_sut.Equals((object)sameRepository));

        Assert.True(s_sut == sameRepository);
        Assert.True(sameRepository == s_sut);
        Assert.False(s_sut != sameRepository);
        Assert.False(sameRepository != s_sut);

        Assert.Equal(s_sut.GetHashCode(), sameRepository.GetHashCode());
    }

    [Fact]
    public void Equality_Different()
    {
        Repository differentRepository = new("R2", "Archive", "here");

        Assert.False(s_sut.Equals(differentRepository));
        Assert.False(s_sut.Equals((object)differentRepository));

        Assert.False(s_sut == differentRepository);
        Assert.True(s_sut != differentRepository);

        Assert.NotEqual(s_sut.GetHashCode(), differentRepository.GetHashCode());
    }
}
