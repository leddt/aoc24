namespace Aoc24;

public class Day19(ITestOutputHelper output)
{
    private const string Sample = """
                                  r, wr, b, g, bwu, rb, gb, br
                                  
                                  brwrr
                                  bggr
                                  gbbr
                                  rrbgbr
                                  ubwu
                                  bwurrg
                                  brgr
                                  bbrgwb
                                  """;
    
    [Fact] public void TestPart1() => Assert.Equal(6, RunPart1(Sample));
    [Fact] public void TestPart2() => Assert.Equal(16, RunPart2(Sample));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/19.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    int RunPart1(string input)
    {
        var lines = input.GetLines();
        var patterns = lines[0].Split(',', StringSplitOptions.TrimEntries);
        var targets = lines.Skip(2).ToArray();

        return targets.Count(t => CountAllPossibilities(t, patterns, []) > 0);
    }

    long RunPart2(string input)
    {
        var lines = input.GetLines();
        var patterns = lines[0].Split(',', StringSplitOptions.TrimEntries);
        var targets = lines.Skip(2).ToArray();

        return targets.Sum(t => CountAllPossibilities(t, patterns, []));
    }

    long CountAllPossibilities(string design, string[] patterns, Dictionary<string, long> tested)
    {
        if (tested.TryGetValue(design, out var c)) return c;
        if (design.Length == 0) return 1;
        
        patterns = patterns.Where(design.Contains).ToArray();
        var matches = patterns.Where(design.StartsWith);
        
        var count = matches.Sum(m => CountAllPossibilities(design.Substring(m.Length), patterns, tested));
        tested[design] = count;
        
        return count;
    }
}