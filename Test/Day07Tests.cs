using AdventOfCode;
using static AdventOfCode.Day07;

namespace Test;

public class Day07Tests
{
    [Theory]
    [InlineData("AAAAA 0", Hand.HandType.FiveOfAKind)]
    [InlineData("AA8AA 0", Hand.HandType.FourOfAKind)]
    [InlineData("23332 0", Hand.HandType.FullHouse)]
    [InlineData("TTT98 0", Hand.HandType.ThreeOfAKind)]
    [InlineData("23432 0", Hand.HandType.TwoPair)]
    [InlineData("A23A4 0", Hand.HandType.OnePair)]
    [InlineData("23456 0", Hand.HandType.HighCard)]
    internal void Day7_WhenParsingHandsWithoutJokers_TypeIsAsExpected(string handInput, Hand.HandType expectedHandType)
    {
        var day = new Day07()
        {
            _input = new[] { handInput },
        };

        var hands = day.ParseHands(false);

        Assert.Equal(expectedHandType, hands.First().Type);
    }

    [Theory]
    [InlineData("JJJJJ 0", Hand.HandType.FiveOfAKind)]
    [InlineData("AAAAJ 0", Hand.HandType.FiveOfAKind)]
    [InlineData("AAJJJ 0", Hand.HandType.FiveOfAKind)]
    [InlineData("AA8AJ 0", Hand.HandType.FourOfAKind)]
    [InlineData("AA8JJ 0", Hand.HandType.FourOfAKind)]
    [InlineData("233J2 0", Hand.HandType.FullHouse)]
    [InlineData("TTJ98 0", Hand.HandType.ThreeOfAKind)]
    [InlineData("TJJ98 0", Hand.HandType.ThreeOfAKind)]
    internal void Day7_WhenParsingHandsWithJokers_TypeIsAsExpected(string handInput, Hand.HandType expectedHandType)
    {
        var day = new Day07()
        {
            _input = new[] { handInput },
        };

        var hands = day.ParseHands(true);

        Assert.Equal(expectedHandType, hands.First().Type);
    }
}