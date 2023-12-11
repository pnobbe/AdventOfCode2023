using System.Drawing;

namespace AdventOfCode;

public partial class Day11 : BaseDay
{
    private readonly string[] _input;
    private readonly Universe _universe;

    public Day11()
    {
        _input = File.ReadAllLines(InputFilePath);
        _universe = ParseUniverse();
    }

    public override ValueTask<string> Solve_1() => new(_universe.WithAge(2).CalculateDistanceBetweenAllGalaxies().ToString());

    public override ValueTask<string> Solve_2() => new(_universe.WithAge(1000000).CalculateDistanceBetweenAllGalaxies().ToString());

    Universe ParseUniverse()
    {
        ILocatable[,] spaces = new ILocatable[_input[0].Length, _input.Length];

        for (var y = 0; y < _input.Length; y++)
        {
            var row = _input[y];
            for (var x = 0; x < row.Length; x++)
            {
                var space = ParseLocation(row[x], new Point(x, y));
                spaces[y, x] = space;
            }
        }

        return new(spaces);
    }

    static ILocatable ParseLocation(char identifier, Point location) => identifier switch
    {
        '#' => new Galaxy(location),
        '.' => new Void(location),
        _ => throw new Exception($"Unknown space type: {identifier}")
    };


    internal record Universe(ILocatable[,] Space)
    {
        internal int Age { get; set; } = 1;

        internal Universe WithAge(int age)
        {
            Age = age; return this;
        }

        internal long CalculateDistanceBetweenAllGalaxies()
        {
            var pairs = GalaxyPairs();

            return pairs.Sum(pair => CalculateDistance(pair.First, pair.Second));
        }

        internal IEnumerable<(Galaxy First, Galaxy Second)> GalaxyPairs()
        {
            var galaxies = Enumerable.Range(0, Space.GetLength(1))
                .SelectMany(index => GetRow(Space, index))
                .OfType<Galaxy>();

            return galaxies.SelectMany((galaxy, i) => galaxies.Skip(i + 1).Select(otherGalaxy => (First: galaxy, Second: otherGalaxy)));
        }

        readonly int[] EmptyRows = Enumerable.Range(0, Space.GetLength(0)).Where(index => GetRow(Space, index).All(space => space is Void)).ToArray();
        readonly int[] EmptyColumns = Enumerable.Range(0, Space.GetLength(1)).Where(index => GetColumn(Space, index).All(space => space is Void)).ToArray();

        internal long CalculateDistance(ILocatable one, ILocatable two)
        {
            var minY = Math.Min(one.Location.Y, two.Location.Y);
            var maxY = Math.Max(one.Location.Y, two.Location.Y);
            var minX = Math.Min(one.Location.X, two.Location.X);
            var maxX = Math.Max(one.Location.X, two.Location.X);

            var distance = (maxY - minY) + (maxX - minX);

            var extraRows = EmptyRows.Count(row => row > minY && row < maxY);
            var extraColumns = EmptyColumns.Count(column => column > minX && column < maxX);

            var result = distance + (extraRows + extraColumns) * (Age - 1); ;
            return result;
        }

        public static ILocatable[] GetColumn(ILocatable[,] space, int columnNumber) => Enumerable.Range(0, space.GetLength(0))
                    .Select(y => space[y, columnNumber])
                    .ToArray();

        public static ILocatable[] GetRow(ILocatable[,] space, int rowNumber) => Enumerable.Range(0, space.GetLength(1))
                    .Select(x => space[rowNumber, x])
                    .ToArray();
    }

    internal record Galaxy(Point Location) : ILocatable
    {
        public string PrintableString => "#";
    }

    internal record Void(Point Location) : ILocatable
    {
        public string PrintableString => ".";
    }

    internal interface ILocatable : IPrintable
    {
        Point Location { get; }
    }

    internal interface IPrintable
    {
        string PrintableString { get; }
    }


}
