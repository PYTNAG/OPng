using OPng.Core;

namespace Tests;

public class DcpoTests
{
    [Fact]
    public void Constructor_WithSupremum_CanIndexSupremumInDcpo()
    {
        DCPO<int> dcpo = new(1);

        try
        {
            var _ = dcpo[1];
        }
        catch
        {
            Assert.Fail("Cannot access elements that covered by supremum");
        }
    }

    [Fact]
    public void AddBefore_TwoElements_CanIndexCoveredElements()
    {
        DCPO<int> dcpo = new(1);

        dcpo.AddBefore(1, 2);
        dcpo.AddBefore(1, 3);

        IEnumerable<int> coveredElements = dcpo[1];

        Assert.Equal(2, coveredElements.Count());
        Assert.True(coveredElements.All(e => e == 2 || e == 3));
    }

    [Fact]
    public void AddBefore_ElementThatEqualsSupremum_NothingHappens()
    {
        DCPO<int> dcpo = new(1);

        dcpo.AddBefore(1, 1);

        IEnumerable<int> coveredElements = dcpo[1];

        Assert.Empty(coveredElements);
    }
}