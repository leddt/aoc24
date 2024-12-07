namespace Aoc24;

public class Template(ITestOutputHelper output)
{
    private const string Sample = """
                                  TODO
                                  """;
    
    [Fact] public void TestPart1() => Assert.Equal(0, RunPart1(Sample));
    [Fact] public void TestPart2() => Assert.Equal(0, RunPart2(Sample));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/0.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    int RunPart1(string input)
    {
        return 0;
    }

    int RunPart2(string input)
    {
        return 0;
    }
}