using System.Runtime.CompilerServices;

namespace AdventOfCode;

public class Day01 : BaseDay
{
    private readonly string[] _input;

    private static readonly Dictionary<string, char> SpelledDigits = new()
    {
        { "one", '1' },
        { "two", '2' },
        { "three", '3' },
        { "four", '4' },
        { "five", '5' },
        { "six", '6' },
        { "seven", '7' },
        { "eight", '8' },
        { "nine", '9' },
    };

    public Day01()
    {
        _input = File.ReadAllText(InputFilePath).Split(Environment.NewLine);
    }

    public override ValueTask<string> Solve_1() => new(_input.Select(ParseLineToNumber).Sum().ToString());

    public override ValueTask<string> Solve_2() => Solve_1();

    private static int ParseLineToNumber(string line) => int.Parse($"{FindFirstDigit(line)}{FindLastDigit(line)}");
    
    private static char FindFirstDigit(string str)
    {
        for (var i = 1; i <= str.Length; i++)
        {
            if (char.IsDigit(str[i-1]))
            {
                return str[i-1];
            }

            var match = SpelledDigits.FirstOrDefault(pair => str[..i].EndsWith(pair.Key)).Value;
            if (match is not default(char))
            {
                return match;
            }
        }

        return '0';
    }

    private static char FindLastDigit(string str)
    {
        for (var i = str.Length-1; i >= 0; i--)
        {
            if (char.IsDigit(str[i]))
            {
                return str[i];
            }
            
            var match = SpelledDigits.FirstOrDefault(pair => str[i..].StartsWith(pair.Key)).Value;
            if (match is not default(char))
            {
                return match;
            }
        }

        return '0';
    }
}
