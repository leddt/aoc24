namespace Aoc24;

public class Day18(ITestOutputHelper output)
{
    private const string Sample = """
                                  5,4
                                  4,2
                                  4,5
                                  3,0
                                  2,1
                                  6,3
                                  2,4
                                  1,5
                                  0,6
                                  3,3
                                  2,6
                                  5,1
                                  1,2
                                  5,5
                                  2,5
                                  6,5
                                  1,4
                                  0,4
                                  6,4
                                  1,1
                                  6,1
                                  1,0
                                  0,5
                                  1,6
                                  2,0
                                  """;
    
    [Fact] public void TestPart1() => Assert.Equal(22, RunPart1(Sample, 7, 12));
    [Fact] public void TestPart2() => Assert.Equal(new V2(6, 1), RunPart2(Sample, 7));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/18.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input, 71, 1024)}");
        output.WriteLine($"Part 2: {RunPart2(input, 71)}");
    }

    int RunPart1(string input, int size, int ticks)
    {
        var positions = Parse(input);
        
        V2 start = new(0, 0), target = new(size-1, size-1);
        var grid = Grid.Filled(size, size, '.');
        
        foreach (var p in positions.Take(ticks))
            grid[p] = '#';

        return FindPath(grid, start, target).Count() - 1;
    }

    V2 RunPart2(string input, int size)
    {
        var positions = Parse(input).ToList();

        V2 start = new(0, 0), target = new(size - 1, size - 1);
        var grid = Grid.Filled(size, size, '.');
        
        foreach (var p in positions)
        {
            grid[p] = '#';
            if (!FindPath(grid, start, target).Any())
                return p;
        }

        return default;
    }

    private static IEnumerable<V2> Parse(string input)
    {
        return input.GetLines()
            .Select(x => x.Split(',').Select(int.Parse).ToArray())
            .Select(x => new V2(x[0], x[1]));
    }

    IEnumerable<V2> FindPath(Grid<char> map, V2 start, V2 end)
    {
        var open = new PriorityQueue<V2, int>();
        open.Enqueue(start, H(start));

        var from = new Dictionary<V2, V2>();
        var gScore = new Dictionary<V2, int>
        {
            [start] = 0
        };

        while (open.TryDequeue(out var pos, out _))
        {
            if (pos == end)
                return BuildPath(pos);

            foreach (var n in Neighbors(pos))
            {
                var tentativeScore = gScore[pos] + 1;
                if (tentativeScore < gScore.GetValueOrDefault(n, int.MaxValue))
                {
                    from[n] = pos;
                    gScore[n] = tentativeScore;
                    open.Enqueue(n, tentativeScore + H(n));
                }
            }
        }

        return Array.Empty<V2>();

        IEnumerable<V2> BuildPath(V2 pos)
        {
            yield return pos;
            if (pos == start) yield break;

            foreach (var p in BuildPath(from[pos]))
                yield return p;
        }

        IEnumerable<V2> Neighbors(V2 pos) => new[] { Dir.Up, Dir.Right, Dir.Down, Dir.Left }
            .Select(d => pos + d)
            .Where(p => map.Contains(p) && map[p] != '#');
        
        int H(V2 pos) => (end - pos).XyLength;
    }
}