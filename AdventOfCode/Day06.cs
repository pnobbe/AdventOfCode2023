namespace AdventOfCode;

public class Day06 : BaseDay
{
    private readonly string[] _input;

    public Day06()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new(ParseRaces().Select(CalculateWinningOptions).Aggregate((aggr, options) => aggr * options).ToString());

    public override ValueTask<string> Solve_2() => new(CalculateWinningOptions(ParseRace()).ToString());

    private IEnumerable<Race> ParseRaces()
    {
        var times = ParseNumbers(_input[0].Split(':').Last());
        var distances = ParseNumbers(_input[1].Split(':').Last());

        return times.Zip(distances, (time, record) => new Race(time, record));

        static IEnumerable<int> ParseNumbers(string numbersString) => numbersString.Trim()
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse);
    }

    private Race ParseRace()
    {
        var time = long.Parse(string.Join("", ParseNumbers(_input[0].Split(':').Last())));
        var distance = long.Parse(string.Join("", ParseNumbers(_input[1].Split(':').Last())));

        return new(time, distance);

        static IEnumerable<int> ParseNumbers(string numbersString) => numbersString.Trim()
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse);
    }

    private int CalculateWinningOptions(Race race)
    {
        var optionsCount = 0;
        for (int i = 0; i < race.Time; i++)
        {
            var holdButtonPressInMs = i;
            var timeLeft = race.Time - i;

            var speed = holdButtonPressInMs;

            if (speed * timeLeft > race.Record)
            {
                optionsCount++;
            }
            else
            {
                if (optionsCount > 0)
                {
                    break;
                }
            }
        }

        return optionsCount;
    }

    internal record struct Race(long Time, long Record);
}
