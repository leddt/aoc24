namespace Aoc24;

// This was really frustrating to solve
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
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/21.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    long RunPart1(string input) => input.GetLines().Sum(x => GetCodeComplexity(x, 2));
    long RunPart2(string input) => input.GetLines().Sum(x => GetCodeComplexity(x, 25));

    private readonly Grid _numPad = Grid.Parse("""
                                               789
                                               456
                                               123
                                                0A
                                               """);

    private readonly Grid _dPad = Grid.Parse("""
                                              ^A
                                             <v>
                                             """);

    long GetCodeComplexity(string code, int chainLength)
    {
        var numericPart = int.Parse(code.Split('A').First());
        var keyPressCount = GetMinStepCountForCode(_numPad, code, chainLength);
    
        return numericPart * keyPressCount;
    }

    // Returns the number of steps required to input a code on a specified keyboard, given a number of intermediate dPad robots.
    long GetMinStepCountForCode(Grid keyboard, string code, int chainLength) =>
        FindPathsBetweenAllKeys(keyboard, code).Min(p => GetMinDPadStepsForPath(p, chainLength));

    private readonly Dictionary<(string, long), long> _minStepsCache = new();
    
    // Returns the number of steps required to complete the specified path, given a number of intermediate dPad robots.
    long GetMinDPadStepsForPath(string path, int chainLength)
    {
        if (chainLength == 0) return path.Length;
        if (_minStepsCache.TryGetValue((path, chainLength), out var result)) return result;

        // Split the path in "moves" ending with A
        // Since all "moves" begin and end at A, it makes them independent of each other, making the required cache space fairly small 
        result = path
            .Split('A')
            .SkipLast(1)
            .Select(move => move + 'A')
            .Sum(move => GetMinStepCountForCode(_dPad, move, chainLength - 1));

        return _minStepsCache[(path, chainLength)] = result;
    }
    
    // Returns the shortest paths that activate the specified key sequence on the specified keyboard
    IEnumerable<string> FindPathsBetweenAllKeys(Grid keyboard, string keys, char prev = 'A', string pathSoFar = "")
    {
        if (keys.Length == 0)
        {
            yield return pathSoFar;
            yield break;
        }

        foreach (var potentialPathSegment in FindPathsBetweenTwoKeys(keyboard, prev, keys[0]))
        foreach (var path in FindPathsBetweenAllKeys(keyboard, keys[1..], keys[0], pathSoFar + potentialPathSegment + 'A'))
            yield return path;
    }

    private readonly Dir[] _allDirs = [Dir.Up, Dir.Right, Dir.Down, Dir.Left]; 
    private readonly Dictionary<(char, char), string[]> _pathCache = new();
    record Path(int Cost, char[] Moves, V2 CurPos);
    
    // A* variant returning all best paths
    // There is probably a better way to do this (pre-compute all possible paths between keys), but this is good enough for me
    string[] FindPathsBetweenTwoKeys(Grid keyboard, char from, char to)
    {
        if (_pathCache.TryGetValue((from, to), out var cached)) return cached;
        
        var start = keyboard.FindFirst(from);
        var end = keyboard.FindFirst(to);
        
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
                
                bestPaths.Add(new string(current.Moves));
            }
            else
            {
                foreach (var dir in _allDirs)
                {
                    var pos = current.CurPos + dir;
                    if (!keyboard.Contains(pos)) continue;
                    if (keyboard[pos] == ' ') continue;
                    
                    var dirChar = DirToChar(dir);
                    
                    var turnCost = 0;
                    // Prioritize straight paths
                    if (current.Moves.Any() && dirChar != current.Moves.Last())
                        turnCost = 1;
                    
                    var g = current.Cost + 1 + turnCost;
                    if (bestScore != null && bestScore < g) continue;
                    if (gScore.GetValueOrDefault(pos, int.MaxValue) < g) continue;
                    
                    open.Enqueue(new Path(g, [..current.Moves, dirChar], pos), g + (end - pos).XyLength);
                }
            }
        }

        _pathCache[(from, to)] = bestPaths.ToArray();
        return _pathCache[(from, to)];
    
        char DirToChar(Dir d) => d switch
        {
            Dir.Up => '^',
            Dir.Left => '<',
            Dir.Down => 'v',
            Dir.Right => '>',
            _ => throw new ArgumentOutOfRangeException(nameof(d))
        };
    }
}