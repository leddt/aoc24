namespace Aoc24;
using Cheat = (V2 start, V2 end);

public class Day20(ITestOutputHelper output)
{
    private const string Sample = """
                                  ###############
                                  #...#...#.....#
                                  #.#.#.#.#.###.#
                                  #S#...#.#.#...#
                                  #######.#.#.###
                                  #######.#.#...#
                                  #######.#.###.#
                                  ###..E#...#...#
                                  ###.#######.###
                                  #...###...#...#
                                  #.#####.#.###.#
                                  #.#...#.#.#...#
                                  #.#.#.#.#.#.###
                                  #...#...#...###
                                  ###############
                                  """;
    
    [Fact] public void TestPart1() => Assert.Equal(5, CountCheats(Sample, 2, 20));
    [Fact] public void TestPart2() => Assert.Equal(285, CountCheats(Sample, 20, 50));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/20.txt");
        
        output.WriteLine($"Part 1: {CountCheats(input, 2, 100)}");
        output.WriteLine($"Part 2: {CountCheats(input, 20, 100)}");
    }

    int CountCheats(string input, int maxCheatLength, int minCheatSavings)
    {
        var map = Grid.Parse(input);

        var start = map.FindFirst('S');
        var end = map.FindFirst('E');
        map[start] = map[end] = '.';
        
        var scores = GetBaselineScores(map, start, end);

        return FindAllPossibleCheats(map, maxCheatLength)
            .Count(cheat => GetDiff(cheat) >= minCheatSavings);

        int GetDiff(Cheat cheat)
        {
            if (!scores.TryGetValue(cheat.start, out var startScore)) return 0;
            if (!scores.TryGetValue(cheat.end, out var endScore)) return 0;

            return endScore - startScore - (cheat.end - cheat.start).XyLength;
        }
    }

    IEnumerable<Cheat> FindAllPossibleCheats(Grid map, int maxLength) =>
        map.FindAll('.')
            .SelectMany(p => FindCheatsFrom(map, p, maxLength));

    IEnumerable<Cheat> FindCheatsFrom(Grid map, V2 start, int maxLength)
    {
        for (var dx = 0; dx <= maxLength; dx++)
        {
            var dy = maxLength - dx;

            var ends = new[]
            {
                start + V2.Left * dx + V2.Up * dy,
                start + V2.Left * dx + V2.Down * dy,
                start + V2.Right * dx + V2.Up * dy,
                start + V2.Right * dx + V2.Down * dy,
            };

            foreach (var end in ends.Distinct().Where(map.Contains))
                yield return (start, end);
        }

        if (maxLength > 2)
        {
            foreach (var cheat in FindCheatsFrom(map, start, maxLength - 1))
                yield return cheat;
        }
    }

    Dictionary<V2, int> GetBaselineScores(Grid map, V2 start, V2 end)
    {
        var open = new PriorityQueue<V2, int>();
        open.Enqueue(start, H(start));

        var gScore = new Dictionary<V2, int> { [start] = 0 };

        while (open.TryDequeue(out var pos, out _))
        {
            if (pos == end) return gScore;

            foreach (var n in Neighbors(map, pos))
            {
                var tentativeScore = gScore[pos] + 1;
                if (tentativeScore < gScore.GetValueOrDefault(n, int.MaxValue))
                {
                    gScore[n] = tentativeScore;
                    open.Enqueue(n, tentativeScore + H(n));
                }
            }
        }

        return gScore;
        
        int H(V2 pos) => (end - pos).XyLength;
    }

    IEnumerable<V2> Neighbors(Grid map, V2 pos) => map.Neighbors(pos, x => x.val == '.').Select(x => x.pos);
}