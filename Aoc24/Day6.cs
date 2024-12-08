namespace Aoc24;

public class Day6(ITestOutputHelper output)
{
    private const string Sample = """
                                  ....#.....
                                  .........#
                                  ..........
                                  ..#.......
                                  .......#..
                                  ..........
                                  .#..^.....
                                  ........#.
                                  #.........
                                  ......#...
                                  """;
    
    [Fact] public void TestPart1() => Assert.Equal(41, RunPart1(Sample));
    [Fact] public void TestPart2() => Assert.Equal(6, RunPart2(Sample));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/6.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    int RunPart1(string input)
    {
        var map = Grid.Parse(input);
        
        var position = FindStart(map);
        Dir facing = default; // Up

        while (true)
        {
            map.Set(position, 'X');

            (position, facing) = Step(map, position, facing);

            if (!map.Contains(position)) break;
        }

        return map.Lines.Sum(y => y.Count(x => x == 'X'));
    }

    int RunPart2(string input)
    {
        var map = Grid.Parse(input);
        
        var start = FindStart(map);
        var count = 0;

        for (var x = 0; x < map.Width; x++)
        for (var y = 0; y < map.Height; y++)
        {
            if (map[x, y] != '.') continue;

            var candidateMap = CreateCandidate(map, x, y);

            if (IsLoop(candidateMap, start)) count++;
        }

        return count;
    }

    Grid CreateCandidate(Grid source, int x, int y)
    {
        var candidateGrid = source.Lines.Select(l => l.ToArray()).ToArray();
        candidateGrid[y][x] = '#';
        return new Grid(candidateGrid);
    }

    V2 FindStart(Grid map)
    {
        for (var x = 0; x < map.Width; x++)
        for (var y = 0; y < map.Height; y++)
        {
            if (map[x, y] == '^')
                return new V2(x, y);
        }

        throw new Exception("Start not found");
    }
    
    V2 NextV2(V2 loc, Dir dir)
    {
        return dir switch
        {
            Dir.Up => loc with { Y = loc.Y - 1 },
            Dir.Right => loc with { X = loc.X + 1 },
            Dir.Down => loc with { Y = loc.Y + 1 },
            Dir.Left => loc with { X = loc.X - 1 },
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
        };
    }
    
    bool IsLoop(Grid map, V2 position)
    {
        var facing = Dir.Up;
    
        while (true)
        {
            if (!map.Contains(position)) return false;
            if (IsDir(map, position, facing)) return true;
            if (map[position] is '.' or '^') SetDir(map, position, facing);
    
            (position, facing) = Step(map, position, facing);
        }
    }
    
    (V2, Dir) Step(Grid map, V2 position, Dir facing)
    {
        var next = NextV2(position, facing);
    
        if (map.Contains(next) && IsObstacle(map, next))
        {
            facing++;
            if (!Enum.IsDefined(facing)) facing = default;
        }
        else
        {
            position = next;
        }
    
        return (position, facing);
    }

    bool IsDir(Grid map, V2 loc, Dir dir) => map[loc] == dir.ToString()[0];
    void SetDir(Grid map, V2 loc, Dir dir) => map[loc] = dir.ToString()[0];
    bool IsObstacle(Grid map, V2 loc) => map[loc] == '#';

    enum Dir { Up, Right, Down, Left }
}