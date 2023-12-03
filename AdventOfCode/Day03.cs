using System.Drawing;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day03 : BaseDay
{
    private readonly string[] _input;

    private readonly Regex NumberRegex = new(@"\d+");
    private readonly Regex SymbolRegex = new(@"[^\w.]|_+");
    private readonly Regex GearRegex = new(@"\*+");

    public Day03()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new(CalculatePartNumbers(ParseSchematic(SymbolRegex, NumberRegex)).ToString());

    public override ValueTask<string> Solve_2() => new(CalculateGearRatios(ParseSchematic(GearRegex, NumberRegex)).ToString());

    private Schematic ParseSchematic(Regex symbolRegex, Regex numberRegex)
    {
        var numbers = _input.SelectMany((row, rowIndex) => GetAllGridNumbers(numberRegex, row, rowIndex));
        var symbols = _input.SelectMany((row, rowIndex) => GetAllGridSymbols(symbolRegex, row, rowIndex));

        return new(numbers, symbols);
    }

    private static long CalculatePartNumbers(Schematic schematic) => schematic.Numbers
        .Where(number =>
            schematic.Symbols
                .Any(symbol => symbol.BoundingBox.IntersectsWith(number.BoundingBox)))
                .Select(number => number.Value)
        .Sum();


    private static long CalculateGearRatios(Schematic schematic) => schematic.Symbols
        .Select(gear =>
            schematic.Numbers
                .Where(number => number.BoundingBox.IntersectsWith(gear.BoundingBox))
                .Select(number => number.Value))
        .Where(collisions => collisions.Count() == 2)
            .Select(gearParts => gearParts.First() * gearParts.Last())
        .Sum();

    static IEnumerable<GridNumber> GetAllGridNumbers(Regex numberRegex, string row, int rowIndex) => numberRegex.Matches(row).Select(match => 
        new GridNumber(int.Parse(match.Value), FromMatch(match, rowIndex)));
    static IEnumerable<GridSymbol> GetAllGridSymbols(Regex symbolRegex, string row, int rowIndex) => symbolRegex.Matches(row).Select(match => 
        new GridSymbol(FromMatch(match, rowIndex)));

    static Rectangle FromMatch(Match match, int rowIndex) => Rectangle.FromLTRB(match.Index - 1, rowIndex - 1, match.Index + match.Length, rowIndex + 1);

    private record struct GridNumber(int Value, Rectangle BoundingBox);
    private record struct GridSymbol(Rectangle BoundingBox);
    private record struct Schematic(IEnumerable<GridNumber> Numbers, IEnumerable<GridSymbol> Symbols);
}
