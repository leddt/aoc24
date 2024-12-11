namespace Aoc24;

public class Day11(ITestOutputHelper output)
{
    private const string Sample = """
                                  125 17
                                  """;
    
    [Fact] public void TestPart1() => Assert.Equal(55312, RunPart1(Sample));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/11.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    long RunPart1(string input)
    {
        var stones = input.Split(' ').Select(x => new Stone(long.Parse(x)));

        return GetStoneCount(stones, 25);
    }

    long RunPart2(string input)
    {
        var stones = input.Split(' ').Select(x => new Stone(long.Parse(x)));

        return GetStoneCount(stones, 75);
    }

    private static long GetStoneCount(IEnumerable<Stone> stones, int iters)
    {
        for (var i = 0; i < iters; i++) 
            stones = Blink(stones);

        return stones.Sum(s => s.Count);
    }

    private static IEnumerable<Stone> Blink(IEnumerable<Stone> stones)
    {
        return stones
            .GroupBy(s => s.Num)
            .Select(x => new Stone(x.Key, x.Sum(s => s.Count)))
            .SelectMany(Blink);
    }

    private static IEnumerable<Stone> Blink(Stone stone)
    {
        if (stone.Num == 0)
        {
            yield return stone with { Num = 1 };
            yield break;
        }
            
        var ss = stone.Num.ToString();
        if (ss.Length % 2 == 0)
        {
            var mid = ss.Length / 2;
            yield return stone with { Num = long.Parse(ss[..mid]) };
            yield return stone with { Num = long.Parse(ss[mid..]) };
        }
        else
        {
            yield return stone with { Num = stone.Num * 2024 };
        }
    }

    readonly record struct Stone(long Num, long Count = 1);
}