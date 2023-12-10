namespace AdventOfCode;

public partial class Day00 : BaseDay
{
    private readonly string[] _input;

    public Day00()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new();

    public override ValueTask<string> Solve_2() => new();
}
