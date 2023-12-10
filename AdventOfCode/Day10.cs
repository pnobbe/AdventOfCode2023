using Spectre.Console;
using System.Drawing;
using System.Text;
using static AdventOfCode.Day10;
using static Crayon.Output;

namespace AdventOfCode;

public partial class Day10 : BaseDay
{
    private readonly string[] _input;
    private readonly Field _field;

    public Day10()
    {
        _input = File.ReadAllLines(InputFilePath);
        _field = ParseField();

        _field.Draw();
    }

    public override ValueTask<string> Solve_1() => new(Solve1().ToString());

    public override ValueTask<string> Solve_2() => new(Solve2().ToString());

    int Solve1()
    {
        var start = _field.StartingLocation;
        IEnumerable<Pipe> pipesToCheck = [_field.StartingLocation];

        var distance = 0;
        var loop = new List<Pipe>();
        while (pipesToCheck.GetEnumerator().MoveNext())
        {
            loop.AddRange(pipesToCheck);
            var nextPipes = pipesToCheck.SelectMany(_field.GetConnectingPipes).ToList();
            distance++;
            Field.Redraw(pipesToCheck, ConsoleColor.Black);
            pipesToCheck = nextPipes;

            Thread.Sleep((distance % 50 == 0) ? 1 : 0);
        }

        _field.Loop = [.. loop];
        return distance;
    }

    int Solve2()
    {
        //IEnumerable<IEnumerable<ILocatable>> untraversed =
        //[
        //    _field.Locations.First(),
        //    _field.Locations.Last(),
        //    _field.Locations.Select(row => row.First()),
        //    _field.Locations.Select(row => row.Last()),
        //];
        //
        //var locationsToCheck = untraversed.SelectMany(v => v);
        //var distance = 0;
        //
        //while (locationsToCheck.GetEnumerator().MoveNext())
        //{
        //    var nextUntraversed = locationsToCheck.SelectMany(_field.GetConnectingUntraversedLocations).ToList();
        //    distance++;
        //    Field.Redraw(locationsToCheck, ConsoleColor.DarkBlue);
        //    locationsToCheck = nextUntraversed;
        //
        //    Thread.Sleep((distance % 50 == 0) ? 1 : 0);
        //}

        var count = 0;
        foreach (var row in _field.Locations
            .Select(row => row))
        {
            bool insideLoop = false;
            foreach (var field in row)
            {
                if (_field.Loop.Contains(field) && field is Pipe pipe && pipe.ConnectingDirections.Contains(Up))
                {
                    insideLoop = !insideLoop;
                }

                if (insideLoop && !_field.Loop.Contains(field))
                {
                        count++;
                  
                    Field.Redraw([field], ConsoleColor.Green);
                }
            }
        }

        return count;
    }

    Field ParseField() => new(_input.
        Select((line, y) => line
            .Select((identifier, x) => (identifier, point: new Point(x, y)))
            .Select(pair => ParseLocation(pair.identifier, pair.point)).ToArray()).ToArray());

    static ILocatable ParseLocation(char identifier, Point location) => identifier switch
    {
        '.' => new Void(location),
        _ => new Pipe(location) { Label = identifier },
    };

    internal static Direction Up = new(0, -1);
    internal static Direction Down = new(0, 1);
    internal static Direction Left = new(-1, 0);
    internal static Direction Right = new(1, 0);
    internal static Direction[] AllDirections = [Up, Down, Left, Right];

    internal record Direction(int X, int Y);

    internal record Field(ILocatable[][] Locations)
    {
        internal Rectangle BoundingBox = new(new(0, 0), new(Locations.MaxBy(row => row.Length).Length, Locations.Length));

        internal IEnumerable<Pipe> Loop { get; set; }

        internal static void Redraw(IEnumerable<ILocatable> locatable, ConsoleColor color)
        {
            var (Left, Top) = Console.GetCursorPosition();
            foreach (var loc in locatable)
            {
                Console.BackgroundColor = color;
                Console.SetCursorPosition(loc.Location.X, loc.Location.Y);
                Console.Write(loc.PrintableString);
            }
            Console.SetCursorPosition(Left, Top);
        }

        internal void Draw()
        {
            Console.Clear();
            Console.ResetColor();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.CursorVisible = true;
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

            StringBuilder builder = new();
            for (int y = 0; y < Locations.Length; y++)
            {
                for (int x = 0; x < Locations[y].Length; x++)
                {
                    builder.Append(Locations[y][x].PrintableString);
                }
                builder.AppendLine();
            }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(builder.ToString());
        }

        internal Pipe StartingLocation => Locations
            .SelectMany(row => row.OfType<Pipe>())
            .OfType<Pipe>()
            .Where(pipe => pipe.Type == Pipe.PipeType.StartingPosition)
            .First();

        internal IEnumerable<Pipe> GetConnectingPipes(Pipe pipe)
        {
            pipe.IsTraversed = true;

            var surroundingLocations = AllDirections
                .Select(direction => new Point(pipe.Location.X + direction.X, pipe.Location.Y + direction.Y))
                .Where(BoundingBox.Contains)
                .Select(point => Locations[point.Y][point.X]);

            var surroundingTraversablePipes = surroundingLocations.OfType<Pipe>().Where(pipe => !pipe.IsTraversed);

            if (pipe.Type == Pipe.PipeType.StartingPosition)
            {
                pipe.FindBestPipeFor(surroundingTraversablePipes.Where(pipe.ConnectsWith));
            }

            foreach (var traversablePipe in surroundingTraversablePipes)
            {
                if (pipe.ConnectsWith(traversablePipe))
                {
                    yield return traversablePipe;
                }
            }
        }

        //internal IEnumerable<ILocatable> GetConnectingUntraversedLocations(ILocatable locatable)
        //{
        //    locatable.IsTraversed = true;
        //
        //    var surroundingLocations = AllDirections
        //        .Select(direction => new Point(locatable.Location.X + direction.X, locatable.Location.Y + direction.Y))
        //        .Where(BoundingBox.Contains)
        //        .Select(point => Locations[point.Y][point.X]);
        //
        //    var surroundingTraversableVoids = surroundingLocations.Where(locatable => !locatable.IsTraversed);
        //
        //    foreach (var traversableVoid in surroundingTraversableVoids)
        //    {
        //        yield return traversableVoid;
        //    }
        //}
    }

    internal record Pipe(Point Location) : ILocatable
    {
        public string PrintableString => IsTraversed ? Bold((Type switch
        {
            PipeType.Vertical => '║',
            PipeType.Horizontal => '═',
            PipeType.NorthEastBend => '╚',
            PipeType.NorthWestBend => '╝',
            PipeType.SouthWestBend => '╗',
            PipeType.SouthEastBend => '╔',
            _ => '?',
        }).ToString()) : " ";

        public char Label { get; set; }

        public bool IsTraversed { get; set; } = false;

        internal enum PipeType
        {
            Vertical,
            Horizontal,
            NorthEastBend,
            NorthWestBend,
            SouthWestBend,
            SouthEastBend,
            StartingPosition,
        }

        internal PipeType Type => Label switch
        {
            '|' => PipeType.Vertical,
            '-' => PipeType.Horizontal,
            'L' => PipeType.NorthEastBend,
            'J' => PipeType.NorthWestBend,
            '7' => PipeType.SouthWestBend,
            'F' => PipeType.SouthEastBend,
            'S' => PipeType.StartingPosition,
            _ => throw new InvalidOperationException("huh")
        };

        internal Direction[] ConnectingDirections => Type switch
        {
            PipeType.Vertical => [Up, Down],
            PipeType.Horizontal => [Left, Right],
            PipeType.NorthEastBend => [Up, Right],
            PipeType.NorthWestBend => [Up, Left],
            PipeType.SouthEastBend => [Down, Right],
            PipeType.SouthWestBend => [Down, Left],
            PipeType.StartingPosition => [Up, Down, Left, Right],
            _ => throw new InvalidOperationException("huh")
        };

        internal void FindBestPipeFor(IEnumerable<Pipe> neighbors)
        {
            var locations = neighbors.Select(n => new Direction(Location.X - n.Location.X, n.Location.Y - Location.Y));
            if (locations.SequenceEqual([Up, Down])) Label = '|';
            else if (locations.SequenceEqual([Left, Right])) Label = '-';
            else if (locations.SequenceEqual([Up, Right])) Label = 'L';
            else if (locations.SequenceEqual([Up, Left])) Label = 'J';
            else if (locations.SequenceEqual([Down, Right])) Label = '7';
            else if (locations.SequenceEqual([Down, Left])) Label = 'F';
        }

        internal bool ConnectsWith(Pipe otherPipe)
        {
            var directionFromMeToOtherPipe = new Direction(otherPipe.Location.X - Location.X, otherPipe.Location.Y - Location.Y);
            var directionFromOtherPipeToMe = new Direction(0 - directionFromMeToOtherPipe.X, 0 - directionFromMeToOtherPipe.Y);

            return ConnectingDirections.Contains(directionFromMeToOtherPipe) && otherPipe.ConnectingDirections.Contains(directionFromOtherPipeToMe);
        }
    }

    internal record Void(Point Location) : ILocatable
    {
        public string PrintableString => " ";
        public bool IsTraversed { get; set; } = false;
    }

    internal interface ILocatable : IPrintable
    {
        Point Location { get; }
        bool IsTraversed { get; set; }
    }

    internal interface IPrintable
    {
        string PrintableString { get; }
    }
}
