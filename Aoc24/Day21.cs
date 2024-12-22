using System.Text;

namespace Aoc24;

public class Day21(ITestOutputHelper output)
{
    private const string Sample = """
                                  029A
                                  980A
                                  179A
                                  456A
                                  379A
                                  """;
    
    [Fact] public void TestPart1() => Assert.Equal(126384, RunPart1(Sample));
    [Fact] public void TestPart2() => Assert.Equal(0, RunPart2(Sample));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/21.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    long RunPart1(string input)
    {
        var codes = input.GetLines();
        return codes.Sum(x => GetCodeComplexity(x, 2));
    }

    long RunPart2(string input)
    {
        var codes = input.GetLines();
        // Current approach doesn't scale beyond chain length 2
        // return codes.Sum(x => GetCodeComplexity(x, 25));
        return 0;
    }

    private Grid numPad = Grid.Parse("""
                                     789
                                     456
                                     123
                                      0A
                                     """);

    private Grid dPad = Grid.Parse("""
                                    ^A
                                   <v>
                                   """);

    long GetCodeComplexity(string code, int chainLength)
    {
        var numericPart = int.Parse(code.Split('A').First());
        var keyPressCount = BestLength(code, chainLength);

        return numericPart * keyPressCount;
    }
    
    static char DirToChar(Dir d) => d switch
    {
        Dir.Up => '^',
        Dir.Left => '<',
        Dir.Down => 'v',
        Dir.Right => '>',
        _ => throw new ArgumentOutOfRangeException(nameof(d))
    };
    
    long BestLength(string code, int chainLength)
    {
        var allPaths = FindBestPaths(numPad, code);
        for (var i = 0; i < chainLength; i++)
            allPaths = allPaths.SelectMany(x => FindBestPaths(dPad, x));

        var bestPath = allPaths.OrderBy(x => x.Length).First();
        // output.WriteLine($"{code}: {bestPath}");

        return bestPath.Length;
    }

    IEnumerable<string> FindBestPaths(Grid map, string keys)
    {
        IEnumerable<string> result = [""];
        
        var previous = 'A';
        foreach (var target in keys)
        {
            var chunk = FindBestPaths(map, previous, target);
            result = result.Join(chunk, _ => 1, _ => 1, (a, b) => a + b);
            previous = target;
        }

        return result;
    }

    private readonly Dir[] _allDirs = [Dir.Up, Dir.Right, Dir.Down, Dir.Left]; 
    private readonly Dictionary<(char, char), string[]> _pathCache = new();
    record Path(int Cost, char[] Moves, V2 CurPos);
    
    string[] FindBestPaths(Grid map, char from, char to)
    {
        if (_pathCache.TryGetValue((from, to), out var cached)) return cached;
        
        var start = map.FindFirst(from);
        var end = map.FindFirst(to);
        
        var open = new PriorityQueue<Path, int>();
        open.Enqueue(new Path(0, [], start), 0);
        var gScore = new Dictionary<V2, int> { [start] = 0 };

        int? bestScore = null;
        var bestPaths = new List<string>();

        while (open.TryDequeue(out var current, out _))
        {
            if (current.CurPos == end)
            {
                if (bestScore == null)
                    bestScore = current.Cost;
                else if (bestScore < current.Cost)
                    break;
                
                bestPaths.Add(new string([..current.Moves, 'A']));
            }
            else
            {
                foreach (var dir in _allDirs)
                {
                    var pos = current.CurPos + dir;
                    if (!map.Contains(pos)) continue;
                    if (map[pos] == ' ') continue;
                    
                    var g = current.Cost + 1;
                    if (bestScore != null && bestScore < g) continue;
                    if (gScore.GetValueOrDefault(pos, int.MaxValue) < g) continue;
                    
                    open.Enqueue(new Path(g, [..current.Moves, DirToChar(dir)], pos), g + (end - pos).XyLength);
                }
            }
        }

        _pathCache[(from, to)] = bestPaths.ToArray();
        return _pathCache[(from, to)];
    }
}