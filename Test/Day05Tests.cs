using AdventOfCode;
using static AdventOfCode.Day05;

namespace Test
{
    public class Day05Tests
    {

        [Fact]
        public void Day5_WhenGettingUpdatedRanges_AndSeedRangeIsEntirelyInMap_NoLeftoversAreReturnedAndResultIsCorrect()
        {
            var day = new Day05();

            var map = new Map(new SeedRange(5, 10), new SeedRange(15, 20));
            var seedRange = new SeedRange(5, 10);

            var result = day.GetUpdatedRanges(map, seedRange, out var leftovers);

            Assert.Empty(leftovers);
            Assert.NotNull(result);
            Assert.Equal(15, result.Start);
        }

        [Fact]
        public void Day5_WhenGettingUpdatedRanges_AndSeedRangeOverlapsWithLowerbound_OneLeftoversIsReturnedAndResultIsCorrect()
        {
            var day = new Day05();

            var map = new Map(new SeedRange(5, 10), new SeedRange(15, 20));
            var seedRange = new SeedRange(0, 10);

            var result = day.GetUpdatedRanges(map, seedRange, out var leftovers);

            Assert.Single(leftovers);
            Assert.Equal(0, leftovers[0].Start);

            Assert.NotNull(result);
            Assert.Equal(15, result.Start);
        }

        [Fact]
        public void Day5_WhenGettingUpdatedRanges_AndSeedRangeOverlapsWithUpperbound_OneLeftoversIsReturnedAndResultIsCorrect()
        {
            var day = new Day05();

            var map = new Map(new SeedRange(5, 10), new SeedRange(15, 20));
            var seedRange = new SeedRange(5, 15);

            var result = day.GetUpdatedRanges(map, seedRange, out var leftovers);

            Assert.Single(leftovers);
            Assert.Equal(11, leftovers[0].Start);

            Assert.NotNull(result);
            Assert.Equal(15, result.Start);
        }

        [Fact]
        public void Day5_WhenGettingUpdatedRanges_AndSeedRangeOverlapsBothBounds_TwoLeftoversAreReturnedAndResultIsCorrect()
        {
            var day = new Day05();

            var map = new Map(new SeedRange(5, 10), new SeedRange(15, 20));
            var seedRange = new SeedRange(0, 15);

            var result = day.GetUpdatedRanges(map, seedRange, out var leftovers);

            Assert.Equal(2, leftovers.Count);
            Assert.Equal(0, leftovers[0].Start);
            Assert.Equal(11, leftovers[1].Start);

            Assert.NotNull(result);
            Assert.Equal(15, result.Start);
        }

        [Fact]
        public void Day5_WhenGettingUpdatedRanges_AndSeedRangeIsCompletelyOutOfBounds_ResultIsNullAndIsSeedIsLeftover()
        {
            var day = new Day05();

            var map = new Map(new SeedRange(5, 10), new SeedRange(15, 20));
            var seedRange = new SeedRange(0, 4);

            var result = day.GetUpdatedRanges(map, seedRange, out var leftovers);

            Assert.Null(result);
            Assert.Single(leftovers);
            Assert.Equal(0, leftovers[0].Start);
        }

        [Fact]
        public void Day5_WhenGettingUpdatedRanges_OffByOneLowerboundTests()
        {
            var day = new Day05();

            var map = new Map(new SeedRange(5, 10), new SeedRange(15, 20));
            var seedRange = new SeedRange(0, 5);

            var result = day.GetUpdatedRanges(map, seedRange, out var leftovers);

            Assert.NotNull(result);
            Assert.Equal(15, result.Start);
            
            Assert.Single(leftovers);
            Assert.Equal(0, leftovers[0].Start);
        }

        [Fact]
        public void Day5_WhenGettingUpdatedRanges_OffByOneUpperboundTests()
        {
            var day = new Day05();

            var map = new Map(new SeedRange(5, 10), new SeedRange(15, 20));
            var seedRange = new SeedRange(10, 15);

            var result = day.GetUpdatedRanges(map, seedRange, out var leftovers);

            Assert.NotNull(result);
            Assert.Equal(20, result.Start);
            Assert.Equal(20, result.End);

            Assert.Single(leftovers);
            Assert.Equal(11, leftovers[0].Start);
            Assert.Equal(15, leftovers[0].End);
        }

        [Theory]
        [InlineData("50 98 2", 98, 99, 50, 51)]
        [InlineData("1270068015 1235603193 242614277", 1235603193, 1478217469, 1270068015, 1512682291)]
        [InlineData("0 1838890301 226793587", 1838890301, 2065683887, 0, 226793586)]
        [InlineData("2741010192 0 358613369", 0, 358613368, 2741010192, 3099623560)]
        public void Day5_WhenParsingMaps_ResultIsParsedAsExpected(string line, long expectedSourceStart, long expectedSourceEnd, long expectedDestinationStart, long expectedDestinationEnd)
        {
            var day = new Day05();

            var map = day.ParseMap(line);

            Assert.Equal(expectedSourceStart, map.Source.Start);
            Assert.Equal(expectedSourceEnd, map.Source.End);
            Assert.Equal(expectedDestinationStart, map.Destination.Start);
            Assert.Equal(expectedDestinationEnd, map.Destination.End);
        }

        [Theory]
        [InlineData("seeds: 2041142901 113138307 302673608 467797997 1787644422 208119536 143576771 99841043 4088720102 111819874 946418697 13450451 3459931852 262303791 2913410855 533641609 2178733435 26814354 1058342395 175406592", 10, 2041142901, 2154281207)]
        [InlineData("seeds: 0 0", 1, 0, 0)]
        [InlineData("seeds: 10 0", 1, 10, 10)]
        [InlineData("seeds: 79 4", 1, 79, 82)]

        public void Day5_WhenParsingSeeds_ResultIsParsedAsExpected(string line, int expectedLength, long expectedFirstRangeStart, long expectedFirstRangeEnd)
        {
            var day = new Day05();

            var seeds = ParseSeedRanges(line);

            Assert.Equal(expectedLength, seeds.Count());
            Assert.Equal(expectedFirstRangeStart, seeds.First().Start);
            Assert.Equal(expectedFirstRangeEnd, seeds.First().End);
        }

        //[Theory]
        //[InlineData("seeds: 10 0")]
        //public void Day5_WhenSolving_ResultIsParsedAsExpected(string seedInput)
        //{
        //    var day = new Day05();
        //
        //    var seeds = day.ParseSeedRanges(seedInput);
        //
        //    var result = day.Solve2(seeds);
        //}
    }
}