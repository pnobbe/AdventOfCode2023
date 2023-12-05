using System;
using System.Text;

namespace AdventOfCode;

public class Day05 : BaseDay
{
    private readonly string[] _input;

    public Day05()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new(Solve1(GetSeedNumbers(_input[0]).ToArray()).ToString());

    public override ValueTask<string> Solve_2() => new(Solve2(ParseSeedRanges(_input[0])).ToString());

    long Solve1(long[] seeds)
    {
        var entries = new List<(int, Map)>();
        var sectionIndex = 0;
        foreach (var line in _input)
        {
            if (line.EndsWith(':'))
            {
                // start new section
                sectionIndex++;
                continue;
            }

            if (line.Length > 0 && char.IsDigit(line[0]))
            {
                entries.Add((sectionIndex, ParseMap(line)));
            }
        }

        var lookup = entries.ToLookup(e => e.Item1, e => e.Item2);

        var updatedSeeds = seeds;
        foreach (var section in lookup)
        {
            seeds = updatedSeeds;
            updatedSeeds = seeds;

            for (var i = 0; i < seeds.Length; i++)
            {
                var seed = seeds[i];
                foreach (var map in section)
                {
                    if (map.Source.Start <= seed && map.Source.End >= seed)
                    {
                        updatedSeeds[i] = seed + map.Offset;
                        break;
                    }
                }
            }
        }

        return seeds.Min();
    }

    internal long Solve2(IEnumerable<SeedRange> seedRanges)
    {
        var entries = new List<(string, Map)>();
        var sectionName = "";
        foreach (var line in _input)
        {
            if (line.EndsWith(':'))
            {
                // start new section
                sectionName = line;
                continue;
            }

            if (line.Length > 0 && char.IsDigit(line[0]))
            {
                entries.Add((sectionName, ParseMap(line)));
            }
        }

        var lookup = entries.ToLookup(e => e.Item1, e => e.Item2);

        var updatedRanges = seedRanges;

        foreach (var section in lookup)
        {
            updatedRanges = ProcessRanges(seedRanges, section).ToList();

            seedRanges = updatedRanges;
        }

        return seedRanges.MinBy(selector => selector.Start).Start;
    }

    private IEnumerable<SeedRange> ProcessRanges(IEnumerable<SeedRange> ranges, IEnumerable<Map> maps)
    {
        var rangesToProcess = new List<SeedRange>(ranges.ToList());
        while (rangesToProcess.Count > 0)
        {
            var range = rangesToProcess[0];
            rangesToProcess.RemoveAt(0);
            var updatedRange = range;

            foreach (var map in maps)
            {
                var result = GetUpdatedRanges(map, range, out var leftovers);

                if (result is not null)
                {
                    updatedRange = result;
                    rangesToProcess.AddRange(leftovers);
                    break;
                }
            }

            yield return updatedRange;
        }
    }

    static IEnumerable<long> GetSeedNumbers(string line) => line.Trim()
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Select(long.Parse);

    internal static IEnumerable<SeedRange> ParseSeedRanges(string line)
    {
        var numbers = GetSeedNumbers(line).ToArray();

        for (var i = 0; i < numbers.Length - 1; i += 2)
        {
            var startSeed = numbers[i];
            var range = numbers[i + 1];

            yield return new(startSeed, startSeed + Math.Max(range - 1, 0));
        }
    }

    internal Map ParseMap(string line)
    {
        var numbers = line.Trim()
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse).ToArray();

        var destinationStart = numbers[0];
        var sourceStart = numbers[1];
        var rangeLength = numbers[2];

        var source = new SeedRange(sourceStart, sourceStart + rangeLength - 1);
        var destination = new SeedRange(destinationStart, destinationStart + rangeLength - 1);

        return new(source, destination);
    }

    internal SeedRange? GetUpdatedRanges(Map map, SeedRange seed, out List<SeedRange> leftovers)
    {
        leftovers = [];

        var distanceFromSeedStartToMapStart = seed.Start - map.Source.Start;
        var distanceFromSeedStartToMapEnd = seed.Start - map.Source.End;
        var distanceFromSeedEndToMapStart = seed.End - map.Source.Start;
        var distanceFromSeedEndToMapEnd = seed.End - map.Source.End;

        if (distanceFromSeedStartToMapStart >= 0 && distanceFromSeedEndToMapEnd <= 0)
        {
            // seed is completely inside of this map
            return map.ConvertToDestination(seed);
        }
        else if (distanceFromSeedStartToMapStart < 0 && distanceFromSeedEndToMapEnd > 0)
        {
            // seed overlaps with both bounds of this map
            leftovers = [
                new(seed.Start, map.Source.Start - 1),
                new(map.Source.End + 1, seed.End)
            ];

            return map.ConvertToDestination(new(map.Source.Start, map.Source.End));
        }
        else if (distanceFromSeedStartToMapStart < 0 && distanceFromSeedEndToMapStart >= 0)
        {
            // seed overlaps with the lower bound of this map
            leftovers = [new(seed.Start, map.Source.Start - 1)];
            return map.ConvertToDestination(new(map.Source.Start, seed.End));
        }
        else if (distanceFromSeedStartToMapEnd <= 0 && distanceFromSeedEndToMapEnd > 0)
        {
            // seed overlaps with the upper bound of this map
            leftovers = [new(map.Source.End + 1, seed.End)];

            return map.ConvertToDestination(new(seed.Start, map.Source.End));
        }
        else if (distanceFromSeedEndToMapStart < 0 || distanceFromSeedStartToMapEnd > 0)
        {
            //seed is completely outside of this map
            leftovers = [seed];
            return null;
        }

        return null;
    }

    internal record Map(SeedRange Source, SeedRange Destination)
    {
        public long Offset = Destination.Start - Source.Start;

        public SeedRange ConvertToDestination(SeedRange seed) => new(seed.Start + Offset, seed.End + Offset);
    }

    internal record SeedRange(long Start, long End);
}
