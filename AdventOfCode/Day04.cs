namespace AdventOfCode;

public class Day04 : BaseDay
{
    private readonly string[] _input;
    private readonly char[] separators = [':', '|'];

    public Day04()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new(_input.Select(ParseLine1).Sum().ToString());

    public override ValueTask<string> Solve_2() => new(Parse(_input).ToString());

    private int ParseLine1(string line)
    {
        var entries = line.Split(separators).Skip(1);

        var winningNumbers = ParseNumbers(entries.First());
        var myNumbers = ParseNumbers(entries.Last());

        return winningNumbers.Aggregate(0, (aggr, winningNumber) =>
            myNumbers.Contains(winningNumber) ? Math.Max(aggr * 2, 1) : aggr);

        static IEnumerable<int> ParseNumbers(string numbersString) => numbersString.Trim()
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse);
    }

    private int Parse(string[] lines)
    {
        var cards = lines.Select(ParseCard).ToArray();
        var cardDict = cards.ToDictionary(c => c.Id, c => 1);

        for (int i = 0; i < cards.Length; i++)
        {
            var card = cards[i];
            var count = cardDict[i];

            for (int x = 1; x < card.MatchingNumbersCount + 1; x++)
            {
                cardDict[i + x] += count;
            }
        }

        return cardDict.Aggregate(0, (aggr, pair) => aggr += pair.Value);
    }

    private Card ParseCard(string line)
    {
        var entries = line.Split(separators).ToArray();
        var cardId = int.Parse(entries[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[1]) - 1;
        var winningNumbers = ParseNumbers(entries[1]);
        var myNumbers = ParseNumbers(entries[2]);

        var matchingNumberCount = winningNumbers.Where(myNumbers.Contains).Count();
        var points = winningNumbers.Aggregate(0, (aggr, winningNumber) =>
                       myNumbers.Contains(winningNumber) ? Math.Max(aggr * 2, 1) : aggr);

        return new Card(cardId, winningNumbers, myNumbers, matchingNumberCount, points);

        static IEnumerable<int> ParseNumbers(string numbersString) => numbersString.Trim()
            .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse);
    }

    private record struct Card(int Id, IEnumerable<int> WinningNumbers, IEnumerable<int> MyNumbers, int MatchingNumbersCount, int Points);
}
