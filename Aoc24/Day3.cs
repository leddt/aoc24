namespace Aoc24;

public partial class Day3(ITestOutputHelper output)
{
    [Fact] 
    public void TestPart1()
    {
        Assert.Equal(161, RunPart1("xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))"));
    }

    [Fact] 
    public void TestPart2()
    {
        Assert.Equal(48, RunPart2("xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))"));
    }

    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/3.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    int RunPart1(string input) => Run(Part1Regex(), input);
    int RunPart2(string input) => Run(Part2Regex(), input);

    private static int Run(Regex regex, string input)
    {
        var matches = regex.Matches(input);

        var sum = 0;
        var enabled = true;

        foreach (var match in matches.ToList())
        {
            var inst = match.Groups["inst"];
            switch (inst.Value)
            {
                case "do":
                    enabled = true;
                    break;
                case "don't":
                    enabled = false;
                    break;
                case "mul":
                    if (enabled)
                    {
                        var a = int.Parse(match.Groups[1].Value);
                        var b = int.Parse(match.Groups[2].Value);

                        sum += a * b;
                    }
                    break;
            }
        }

        return sum;
    }

    [GeneratedRegex(@"(?<inst>mul)\((\d+)\,(\d+)\)")]
    private static partial Regex Part1Regex();
    
    [GeneratedRegex(@"(?<inst>mul)\((\d+)\,(\d+)\)|(?<inst>do)\(\)|(?<inst>don't)\(\)")]
    private static partial Regex Part2Regex();
}