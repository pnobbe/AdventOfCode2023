using System.Text.RegularExpressions;

namespace AdventOfCode;

public partial class Day08 : BaseDay
{
    private readonly string[] _input;
    private readonly Regex _nodeRegex = NodeRegex();

    public Day08()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new(Solve1().ToString());

    public override ValueTask<string> Solve_2() => new(Solve2().ToString());

    long Solve1()
    {
        var nodes = ParseNodes();

        var instructionSet = ParseInstructionSet().ToArray();
        var instructionSetCount = instructionSet.Length;

        var currentNode = nodes["AAA"];

        var map = new Map(nodes, instructionSet);
        var distance = 0;

        for (var index = 0; index < instructionSetCount; index++)
        {
            currentNode = map.Navigate(currentNode, index);
            distance++;

            if (currentNode.Root == "ZZZ")
            {
                return distance;
            }

            if (index == instructionSetCount - 1)
            {
                index = -1;
            }
        }

        return distance;
    }

    long Solve2()
    {
        var nodes = ParseNodes();

        var instructionSet = ParseInstructionSet().ToArray();
        var instructionSetCount = instructionSet.Length;

        var startingNodes = nodes.Where(n => n.Value.Root.EndsWith('A')).Select(node => node.Value).ToArray();
        var startingNodeCount = startingNodes.Length;

        var map = new Map(nodes, instructionSet);
        var currentNodes = startingNodes;
        var zLocations = new List<long>();

        for (int nodeIndex = 0; nodeIndex < currentNodes.Length; nodeIndex++)
        {

            Node[] updatedNodes = startingNodes;

            int distance = 0;

            // loop over each node and navigate
            for (var instructionIndex = 0; instructionIndex < instructionSetCount; instructionIndex++)
            {
                var node = updatedNodes[nodeIndex];
                var updatedNode = map.Navigate(node, instructionIndex);
                distance++;

                if (updatedNode.EndsAtZ)
                {
                    zLocations.Add(distance);
                    break;
                }

                Console.WriteLine($"Distance: {distance}, Node: {node.Root} {(instructionSet[instructionIndex] == Direction.Left ? $"(>{node.Left}<, {node.Right})" : $"({node.Left}, >{node.Right}<)")}, ---- UpdatedNode: {updatedNode.Root}, Ends at Z: {updatedNode.EndsAtZ}, Instruction: {instructionSet[instructionIndex]} ({instructionIndex})");

                updatedNodes[nodeIndex] = updatedNode;

                if (instructionIndex == instructionSetCount - 1)
                {
                    instructionIndex = -1;
                }
            }

            currentNodes = updatedNodes;
        }

        Console.WriteLine(string.Join("\n", zLocations.Select(l => $"Z at: {l}")));
        var lcm = LCM([.. zLocations]);

        return lcm;
    }

    IEnumerable<Direction> ParseInstructionSet() => _input[0].Select(c => c switch
    {
        'R' => Direction.Right,
        'L' => Direction.Left,
        _ => throw new NotImplementedException("huh"),
    });

    internal record Map(IDictionary<string, Node> Nodes, Direction[] InstructionSet)
    {
        internal Node Navigate(Node node, int instructionIndex) => InstructionSet[instructionIndex] switch
        {
            Direction.Right => Nodes[node.Right],
            Direction.Left => Nodes[node.Left],
            _ => throw new NotImplementedException("huh"),
        };
    }

    IDictionary<string, Node> ParseNodes() => _input[2..]
        .Select(line =>
        {
            var matches = _nodeRegex.Matches(line);
            var root = matches[0].Value;
            var left = matches[1].Value;
            var right = matches[2].Value;

            return new Node(root, left, right);
        })
        .ToDictionary(node => node.Root, node => node);

    internal record Node(string Root, string Left, string Right)
    {
        internal bool EndsAtZ => Root.EndsWith('Z');
    }

    internal enum Direction
    {
        Right,
        Left,
    }

    static long LCM(long[] numbers)
    {
        return numbers.Aggregate(lcm);
    }
    static long lcm(long a, long b)
    {
        return Math.Abs(a * b) / GCD(a, b);
    }
    static long GCD(long a, long b)
    {
        return b == 0 ? a : GCD(b, a % b);
    }

    [GeneratedRegex(@"[A-Z]{3}")]
    private static partial Regex NodeRegex();
}
