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
        var stones = input.Split(' ').Select(long.Parse);

        for (var i = 0; i < 25; i++)
            stones = Blink(stones);

        return stones.LongCount();
    }

    long RunPart2(string input)
    {
        var stones = input.Split(' ').Select(long.Parse);
        
        // Naive approach is too slow
        // for (var i = 0; i < 75; i++)
        //     stones = Blink(stones);
        //
        // return stones.LongCount();
        
        return 0;
    }

    private static IEnumerable<long> Blink(IEnumerable<long> stones) => stones.SelectMany(Blink);

    private static IEnumerable<long> Blink(long stone)
    {
        if (stone == 0)
        {
            yield return 1;
            yield break;
        }
            
        var ss = stone.ToString();
        if (ss.Length % 2 == 0)
        {
            var mid = ss.Length / 2;
            yield return long.Parse(ss[..mid]);
            yield return long.Parse(ss[mid..]);
        }
        else
        {
            yield return stone * 2024;
        }
    }
}