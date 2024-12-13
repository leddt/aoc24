using Microsoft.VisualBasic.FileIO;

namespace Aoc24;

public partial class Day13(ITestOutputHelper output)
{
    private const string Sample = """
                                  Button A: X+94, Y+34
                                  Button B: X+22, Y+67
                                  Prize: X=8400, Y=5400
                                  
                                  Button A: X+26, Y+66
                                  Button B: X+67, Y+21
                                  Prize: X=12748, Y=12176
                                  
                                  Button A: X+17, Y+86
                                  Button B: X+84, Y+37
                                  Prize: X=7870, Y=6450
                                  
                                  Button A: X+69, Y+23
                                  Button B: X+27, Y+71
                                  Prize: X=18641, Y=10279
                                  """;
    
    [Fact] public void TestPart1() => Assert.Equal(480, RunPart1(Sample));
    [Fact] public void TestPart2() => Assert.Equal(0, RunPart2(Sample));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/13.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    long RunPart1(string input)
    {
        var machines = Parse(input);
        return machines.Select(FindLowestCost).Sum();
    }

    long RunPart2(string input)
    {
        // Need better algo...
        var machines = Parse(input/*, new LV2(10_000_000_000_000, 10_000_000_000_000)*/);
        return machines.Select(FindLowestCost).Sum();
    }

    long FindLowestCost(Machine m)
    {
        var seen = new Dictionary<LongV2, long>();
        var open = new PriorityQueue<Candidate, double>();
        open.Enqueue(new Candidate(m.Prize, 0, 0), 0);

        while (open.TryDequeue(out var current, out _))
        {
            if (current.Dist == LongV2.Zero) return current.Cost;
            
            if (current.Dist - m.A is { X: >= 0, Y: >= 0 } aPos) 
                AddCandidate(new Candidate(aPos, current.ACount + 1, current.BCount));
            if (current.Dist - m.B is { X: >= 0, Y: >= 0 } bPos) 
                AddCandidate(new Candidate(bPos, current.ACount, current.BCount + 1));
        }

        return 0; // no solution

        void AddCandidate(Candidate c)
        {
            if (seen.TryGetValue(c.Dist, out var x) && x <= c.Cost) return;
            
            seen[c.Dist] = c.Cost;
            open.Enqueue(c, c.Cost + c.Dist.XyLength);
        }
    }

    IEnumerable<Machine> Parse(string input, LongV2 prizeOffset = default)
    {
        var lines = input.GetLines();

        var x = 0;
        while (x < lines.Length)
        {
            var a = XyRegex().Match(lines[x]);
            var b = XyRegex().Match(lines[x + 1]);
            var prize = XyRegex().Match(lines[x + 2]);

            yield return new Machine(ToLongV2(a), ToLongV2(b), ToLongV2(prize) + prizeOffset);
            
            x += 4;
        }

        LongV2 ToLongV2(Match m) => new(long.Parse(m.Groups[1].ValueSpan), long.Parse(m.Groups[2].ValueSpan));
    }

    record Machine(LongV2 A, LongV2 B, LongV2 Prize);

    record Candidate(LongV2 Dist, long ACount, long BCount)
    {
        public long Cost => ACount * 3 + BCount * 1;
    }

    [GeneratedRegex(@"X.(\d+), Y.(\d+)")]
    private static partial Regex XyRegex();
}