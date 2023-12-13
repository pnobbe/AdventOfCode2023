using Spectre.Console;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using static AdventOfCode.Day12.Spring;
using static Crayon.Output;

namespace AdventOfCode;

public partial class Day12 : BaseDay
{
    private readonly string[] _input;

    public Day12()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    int CalculateArrangementSum()
    {
        var records = _input.Select(line =>
        {
            var row = line.Split(' ').Take(2);

            return new Record(
                row.First().Select(ch => new Spring(ch)),
                row.Last().Split(',').Select(int.Parse));
        });

        return records.Sum(record => record.Calculate());
    }

    public override ValueTask<string> Solve_1() => new(CalculateArrangementSum().ToString());

    public override ValueTask<string> Solve_2() => new("soontm");

    internal record Record(IEnumerable<Spring> Springs, IEnumerable<int> DamagedSpringGroups)
    {
        internal int Calculate()
        {
            // For the group of damaged springs, take the first.
            var firstSpringGroup = DamagedSpringGroups.First();

            // Reserve room for the rest
            var restOfTheGroups = DamagedSpringGroups.Skip(1);
            var reservedSpace = ReserveSpace(Springs, restOfTheGroups);

            // Calculate the permutation possibilities for the first group
            var playground = Springs.SkipLast(reservedSpace.Count());

            // * For each permutation, reserve space for for the first group, add the sum to the total
            // * Calculate all the permutations for the rest given the reserved space for the first group, add the sum to our total

            // Sum it with the calculations for the rest of the group minus the reserved space for the first group
            return 0;
        }

        internal IEnumerable<Spring> ReserveSpace(IEnumerable<Spring> springs, IEnumerable<int> RestOfTheGroups)
        {
            var cache = new List<Spring>();
            var sum = RestOfTheGroups.Sum();
            var count = RestOfTheGroups.Count();

            foreach (var spring in Enumerable.Reverse(springs))
            {
                if (cache.Count(spring => spring.Type != SpringType.Operational) >= sum
                    && cache.Count(spring => spring.Type == SpringType.Operational) >= count)
                {
                    break;
                }

                cache.Add(spring);
                yield return spring;
            }
        }
    }

    internal record Spring(char Label)
    {
        internal SpringType Type = Label switch
        {
            '.' => SpringType.Operational,
            '#' => SpringType.Damaged,
            '?' => SpringType.Unknown,
            _ => throw new Exception("Invalid spring type")
        };

        internal enum SpringType
        {
            Operational,
            Damaged,
            Unknown
        }
    }


}