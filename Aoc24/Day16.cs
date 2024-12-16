namespace Aoc24;
using State = (V2 pos, Dir dir);
using Path = (int cost, (V2 pos, Dir dir)[] moves);

public class Day16(ITestOutputHelper output)
{
    private const string Sample1 = """
                                  ###############
                                  #.......#....E#
                                  #.#.###.#.###.#
                                  #.....#.#...#.#
                                  #.###.#####.#.#
                                  #.#.#.......#.#
                                  #.#.#####.###.#
                                  #...........#.#
                                  ###.#.#####.#.#
                                  #...#.....#.#.#
                                  #.#.#.###.#.#.#
                                  #.....#...#.#.#
                                  #.###.#.#.#.#.#
                                  #S..#.....#...#
                                  ###############
                                  """;
    private const string Sample2 = """
                                   #################
                                   #...#...#...#..E#
                                   #.#.#.#.#.#.#.#.#
                                   #.#.#.#...#...#.#
                                   #.#.#.#.###.#.#.#
                                   #...#.#.#.....#.#
                                   #.#.#.#.#.#####.#
                                   #.#...#.#.#.....#
                                   #.#.#####.#.###.#
                                   #.#.#.......#...#
                                   #.#.###.#####.###
                                   #.#.#...#.....#.#
                                   #.#.#.#####.###.#
                                   #.#.#.........#.#
                                   #.#.#.#########.#
                                   #S#.............#
                                   #################
                                   """;
    
    [Fact] public void TestPart1_1() => Assert.Equal(7036, RunPart1(Sample1));
    [Fact] public void TestPart1_2() => Assert.Equal(11048, RunPart1(Sample2));
    [Fact] public void TestPart2_1() => Assert.Equal(45, RunPart2(Sample1));
    [Fact] public void TestPart2_2() => Assert.Equal(64, RunPart2(Sample2));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/16.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    int RunPart1(string input)
    {
        var map = Grid.Parse(input);
        var start = map.FindFirst('S');
        var end = map.FindFirst('E');

        var open = new PriorityQueue<(V2, Dir), int>();
        var gScore = new Dictionary<State, int>();

        var initial = (start, Dir.Right);
        Enqueue(initial, 0);

        while (open.TryDequeue(out State current, out _))
        {
            if (current.pos == end) return gScore[current];

            foreach (var (neighbor, moveCost) in Neighbors(map, current))
            {
                var g = gScore[current] + moveCost;
                if (gScore.ContainsKey(neighbor) && gScore[neighbor] <= g) 
                    continue;

                gScore[neighbor] = g;
                Enqueue(neighbor, g);
            }
        }

        throw new Exception("Path not found");

        void Enqueue(State state, int g)
        {
            gScore[state] = g;
            open.Enqueue(state, g + H(state));
        }
        int H(State state) => (end - state.pos).XyLength;
    }

    int RunPart2(string input)
    {
        var map = Grid.Parse(input);
        var start = map.FindFirst('S');
        var end = map.FindFirst('E');

        var open = new PriorityQueue<Path, int>();
        var gScore = new Dictionary<State, int>();

        var initial = (start, Dir.Right);
        Enqueue((0, [initial]));

        int? bestScore = null;
        var bestTiles = new HashSet<V2>();
        
        while (open.TryDequeue(out Path current, out _))
        {
            var move = current.moves[0];
            if (move.pos == end)
            {
                if (bestScore == null)
                {
                    // It's the first time we reach the end, note this score as the best score
                    bestScore = current.cost;
                }
                else if (bestScore < current.cost)
                {
                    // We reached the end using a less efficient path, so we found all the best paths
                    break;
                }

                foreach (var m in current.moves)
                    bestTiles.Add(m.pos);
            }
            else
            {

                foreach (var (neighbor, moveCost) in Neighbors(map, move))
                {
                    var g = current.cost + moveCost;
                    if (gScore.ContainsKey(neighbor) && gScore[neighbor] < g)
                        continue;

                    Enqueue((g, [neighbor, ..current.moves]));
                }
            }
        }

        return bestTiles.Count;

        void Enqueue(Path path)
        {
            gScore[path.moves[0]] = path.cost;
            open.Enqueue(path, path.cost + H(path.moves[0]));
        }
        int H(State state) => (end - state.pos).XyLength;
    }

    IEnumerable<(State neighbor, int moveCost)> Neighbors(Grid map, State s)
    {
        yield return (s with { dir = s.dir.TurnLeft() }, 1000);
        yield return (s with { dir = s.dir.TurnRight() }, 1000);
            
        var forward = s.pos + s.dir;
        if (map[forward] != '#')
            yield return (s with { pos = forward }, 1);
    }
}