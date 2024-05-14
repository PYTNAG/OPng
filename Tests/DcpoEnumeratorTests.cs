using System.Text;
using OPng.Core;

namespace Tests;

public class DcpoEnumeratorTests
{
    [Fact]
    public void Iterating_DCPO_ReturnsElementsInSpecifiedOrder()
    {
        int supremum = -1;
        int infinum = -2;

        DCPO<int> dcpo = new(supremum);

        dcpo.AddBefore(supremum, 1);
        dcpo.AddBefore(1, infinum);

        dcpo.AddBefore(supremum, 7);
        dcpo.AddBefore(7, 2);
        dcpo.AddBefore(2, infinum);

        dcpo.AddBefore(7, 5);
        dcpo.AddBefore(5, 3);
        dcpo.AddBefore(3, infinum);

        dcpo.AddBefore(supremum, 8);
        dcpo.AddBefore(8, 6);
        dcpo.AddBefore(6, 4);
        dcpo.AddBefore(4, infinum);

        StringBuilder @string = new();

        foreach (int i in dcpo)
        {
            @string.Append(i);
        }

        string dcpoString = @string.ToString();

        AssertChainOrder(dcpoString, infinum, 4, 6, 8, supremum);
        AssertChainOrder(dcpoString, infinum, 3, 5, 7, supremum);
        AssertChainOrder(dcpoString, infinum, 2, 7, supremum);
        AssertChainOrder(dcpoString, infinum, 1, supremum);
    }

    private static void AssertChainOrder(string @string, params object[] chain)
    {
        Assert.Matches($".*{string.Join(".*", chain)}.*", @string);
    }
}