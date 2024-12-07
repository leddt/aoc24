namespace Aoc24;

public class Day1(ITestOutputHelper output)
{
    const string Sample = """
                          3   4
                          4   3
                          2   5
                          1   3
                          3   9
                          3   3
                          """;
    [Fact] public void TestPart1() => Assert.Equal(11, RunPart1(Sample));
    [Fact] public void TestPart2() => Assert.Equal(31, RunPart2(Sample));

    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/1.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    int RunPart1(string input)
    {
        var (left, right) = Parse(input);

        var totalDistance = left.Zip(right).Sum(pair => Math.Abs(pair.First - pair.Second));
        return totalDistance;
    }

    int RunPart2(string input)
    {
        var (left, right) = Parse(input);

        var counts = right.GroupBy(r => r).ToDictionary(g => g.Key, g => g.Count());
        var similarity = left.Sum(l => l * counts.GetValueOrDefault(l));
        return similarity;
    }

    private static (List<int> left, List<int> right) Parse(string input)
    {
        var lines = input.Split('\n');
        var data = lines.Select(x => x.Split("   ").Select(int.Parse)).ToList();
        var left = data.Select(x => x.First()).Order().ToList();
        var right = data.Select(x => x.Last()).Order().ToList();
        return (left, right);
    }
}