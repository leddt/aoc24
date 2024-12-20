namespace Aoc24;

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
    
    [Fact] public void TestPart1() => Assert.Equal(5, RunPart1(Sample, 20));
    [Fact] public void TestPart2() => Assert.Equal(0, RunPart2(Sample));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/20.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input, 100)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    int RunPart1(string input, int threshold)
    {
        var map = Grid.Parse(input);

        var start = map.FindFirst('S');
        var end = map.FindFirst('E');

        var scores = GetScores(map, start, end);
        var result = 0;

        foreach (var pos in map.FindAll('#'))
        {
            var neighbors = Neighbors(map, pos).ToArray(); 
            if (neighbors.Length != 2) continue;

            var diff = Math.Abs(scores[neighbors[0]] - scores[neighbors[1]]);
            if (diff >= threshold + 2) result++;
        }

        return result;
    }

    int RunPart2(string input)
    {
        return 0;
    }

    Dictionary<V2, int> GetScores(Grid map, V2 start, V2 end)
    {
        var open = new PriorityQueue<V2, int>();
        open.Enqueue(start, H(start));

        var gScore = new Dictionary<V2, int>
        {
            [start] = 0
        };

        while (open.TryDequeue(out var pos, out _))
        {
            if (pos == end)
                return gScore;

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

    IEnumerable<V2> Neighbors(Grid map, V2 pos) => new[] { Dir.Up, Dir.Right, Dir.Down, Dir.Left }
        .Select(d => pos + d)
        .Where(p => map.Contains(p) && map[p] != '#');
}