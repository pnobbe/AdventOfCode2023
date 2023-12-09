using Spectre.Console;
using System.Data;

namespace AdventOfCode;

public partial class Day09 : BaseDay
{
    private readonly string[] _input;

    public Day09()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new(_input.Select(ParseLine).Select(Solve1).Sum().ToString());

    public override ValueTask<string> Solve_2() => new(_input.Select(ParseLine).Select(Solve2).Sum().ToString());

    IEnumerable<int> ParseLine(string line) => line.Split(' ').Select(int.Parse);

    int Solve1(IEnumerable<int> startingRow) => ExtrapolateForward(GetHistory(startingRow).Reverse()).Last().Last();

    int Solve2(IEnumerable<int> startingRow) => ExtrapolateBackward(GetHistory(startingRow).Reverse()).Last().First();

    static IEnumerable<IEnumerable<int>> GetHistory(IEnumerable<int> seed)
    {
        yield return seed;

        var currentRow = seed;
        while (!currentRow.All(x => x == 0))
        {
            currentRow = GetDifferences(currentRow.ToArray());
            yield return currentRow;
        }
    }

    static IEnumerable<IEnumerable<int>> ExtrapolateForward(IEnumerable<IEnumerable<int>> history)
    {
        var lastAppendValue = 0;

        foreach (var row in history)
        {
            lastAppendValue = row.Last() + lastAppendValue;

            yield return row.Append(lastAppendValue);
        }
    }

    static IEnumerable<IEnumerable<int>> ExtrapolateBackward(IEnumerable<IEnumerable<int>> history)
    {
        var lastPrependValue = 0;

        foreach (var row in history)
        {
            lastPrependValue = row.First() - lastPrependValue;

            yield return row.Prepend(lastPrependValue);
        }
    }

    static IEnumerable<int> GetDifferences(int[] row)
    {
        for (int i = 0; i < row.Length - 1; i++)
        {
            yield return row[i + 1] - row[i];
        }
    }
}
