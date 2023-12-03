namespace AdventOfCode;

public class Day02 : BaseDay
{
    private readonly string[] _input;
    private readonly char[] separators = [',', ':', ';', ' '];

    public Day02()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new(_input.Select(ParseLine).Sum().ToString());

    public override ValueTask<string> Solve_2() => new(_input.Select(ParseLine2).Sum().ToString());

    private int ParseLine(string line)
    {
        var entries = line.Split(separators, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
        var gameId = int.Parse(entries.First());

        for (int i = 1; i < entries.Length; i += 2)
        {
            var count = int.Parse(entries[i]);
            var color = entries[i + 1];

            var possible = color switch
            {
                "red" => count <= 12,
                "green" => count <= 13,
                "blue" => count <= 14,
            };

            if (!possible)
            {
                return 0;
            }

        }

        return gameId;
    }

    private int ParseLine2(string line)
    {
        var entries = line.Split(separators, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
        var gameId = int.Parse(entries.First());

        var maxRed = 0;
        var maxGreen = 0;
        var maxBlue = 0;

        for (int i = 1; i < entries.Length; i += 2)
        {
            var count = int.Parse(entries[i]);
            var color = entries[i + 1];

            _ = color switch
            {
                "red" => maxRed = int.Max(count, maxRed),
                "green" => maxGreen = int.Max(count, maxGreen),
                "blue" => maxBlue = int.Max(count, maxBlue),
            };
        }

        return maxRed * maxGreen * maxBlue;
    }
}
