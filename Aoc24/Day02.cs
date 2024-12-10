namespace Aoc24;

public class Day02(ITestOutputHelper output)
{
    private const string Sample = """
                                  7 6 4 2 1
                                  1 2 7 8 9
                                  9 7 6 2 1
                                  1 3 2 4 5
                                  8 6 4 4 1
                                  1 3 6 7 9
                                  """;

    [Fact] public void TestPart1() => Assert.Equal(2, RunPart1(Sample));
    [Fact] public void TestPart2() => Assert.Equal(4, RunPart2(Sample));

    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/2.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }
    
    int RunPart1(string input)
    {
        var reports = Parse(input);
        return reports.Count(IsSafeReport);
    }

    int RunPart2(string input)
    {
        var reports = Parse(input);
        return reports.Count(HasSafePermutation);
    }

    private static int[][] Parse(string input) =>
        input.GetLines()
            .Select(x => x.Split(" ").Select(int.Parse).ToArray())
            .ToArray();


    bool IsSafeReport(int[] report) => IsAscending(report) || IsDescending(report);
    bool IsAscending(int[] report) => CheckReport(report, (v, p) => v - p is < 1 or > 3);
    bool IsDescending(int[] report) => CheckReport(report, (v, p) => p - v is < 1 or > 3);

    bool HasSafePermutation(int[] report)
    {
        return Permutations(report).Any(IsSafeReport);
    }

    IEnumerable<int[]> Permutations(int[] report)
    {
        yield return report;
    
        for (var i = 0; i < report.Length; i++)
        {
            yield return report.Where((_, ii) => i != ii).ToArray();
        }
    }

    bool CheckReport(int[] report, SequenceCheck isBadSequence)
    {
        var p = report[0];
    
        for (var i = 1; i < report.Length; i++)
        {
            var v = report[i];
        
            if (isBadSequence(v, p))
                return false;
        
            p = v;
        }

        return true;
    }

    delegate bool SequenceCheck(int value, int previous);
}