namespace AdventOfCode;

public class Day07 : BaseDay
{
    internal string[] _input;

    public Day07()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new(CalculateWinnings(ParseHands(false)).ToString());

    public override ValueTask<string> Solve_2() => new(CalculateWinnings(ParseHands(true)).ToString());

    internal static int CalculateWinnings(IEnumerable<Hand> hands) => hands
        .OrderByDescending(hands => hands)
        .Select((hand, index) => hand.Bid * (index + 1))
        .Sum();

    internal IEnumerable<Hand> ParseHands(bool enableJokers) => _input
        .Select(line =>
        {
            var split = line.Split(' ');
            var bid = int.Parse(split[1]);
            var cards = split[0].Select(c => new Card(c, enableJokers));

            return new Hand(cards.ToArray(), bid);
        });

    internal record struct Card(char Label, bool EnableJoker) : IComparable<Card>
    {
        private readonly char[] SortOrder => (EnableJoker ? "AKQT98765432J" : "AKQJT98765432").ToCharArray();

        internal readonly bool IsJoker => Label == 'J' && EnableJoker;

        public readonly int CompareTo(Card other)
        {
            var indexOfX = Array.IndexOf(SortOrder, Label);
            var indexOfY = Array.IndexOf(SortOrder, other.Label);

            return indexOfX == indexOfY ? 0 : (indexOfX > indexOfY ? 1 : -1);
        }
    };

    internal record Hand(Card[] Cards, int Bid) : IComparable<Hand>
    {
        internal enum HandType
        {
            FiveOfAKind,
            FourOfAKind,
            FullHouse,
            ThreeOfAKind,
            TwoPair,
            OnePair,
            HighCard,
        }

        ILookup<char, Card> CardLookup => Cards
            .Where(c => !c.IsJoker)
            .DefaultIfEmpty()
            .ToLookup(c => c.Label);

        int JokerCount => Cards.Count(c => c.IsJoker);

        internal HandType Type => CardLookup.Count switch
        {
            1 => HandType.FiveOfAKind,
            2 => CardLookup.Any(g => g.Count() + JokerCount == 4) ? HandType.FourOfAKind : HandType.FullHouse,
            3 => CardLookup.Any(g => g.Count() + JokerCount == 3) ? HandType.ThreeOfAKind : HandType.TwoPair,
            4 => HandType.OnePair,
            _ => HandType.HighCard,
        };

        public int CompareTo(Hand other) => Type == other.Type
            ? Cards.Select((c, i) =>
                c.CompareTo(other.Cards[i]))
                .FirstOrDefault(r => r != 0, 0)
            : Type > other.Type ? 1 : -1;
    };
}
