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
        var map = Parse(input);
        
        var position = FindStart(map);
        Dir facing = default; // Up

        while (true)
        {
            Set(map, position, 'X');

            (position, facing) = Step(map, position, facing);

            if (IsOutOfBounds(map, position)) break;
        }

        return map.Grid.Sum(x => x.Count(y => y == 'X'));
    }

    int RunPart2(string input)
    {
        var map = Parse(input);
        
        var start = FindStart(map);
        var count = 0;

        for (var x = 0; x < map.Width; x++)
        for (var y = 0; y < map.Height; y++)
        {
            if (map.Grid[y][x] != '.') continue;

            var candidateMap = CreateCandidate(map, x, y);

            if (IsLoop(candidateMap, start)) count++;
        }

        return count;
    }

    Map CreateCandidate(Map source, int x, int y)
    {
        var candidateGrid = source.Grid.Select(g => g.ToArray()).ToArray();
        candidateGrid[y][x] = '#';
        return source with { Grid = candidateGrid };
    }
    
    Map Parse(string input)
    {
        var grid = input.Split('\n', StringSplitOptions.TrimEntries).Select(x => x.Select(c => c).ToArray()).ToArray();
        var width = grid.Length;
        var height = grid.First().Length;

        return new Map(grid, width, height);
    }

    Loc FindStart(Map map)
    {
        for (var x = 0; x < map.Width; x++)
        for (var y = 0; y < map.Height; y++)
        {
            if (map.Grid[y][x] == '^')
                return new Loc(x, y);
        }
    
        throw new Exception("Start not found");
    }
    
    Loc NextLoc(Loc loc, Dir dir)
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
    
    bool IsLoop(Map map, Loc position)
    {
        var facing = Dir.Up;
    
        while (true)
        {
            if (IsOutOfBounds(map, position)) return false;
            if (IsDir(map, position, facing)) return true;
            if (Peek(map, position) is '.' or '^') SetDir(map, position, facing);
    
            (position, facing) = Step(map, position, facing);
        }
    }
    
    (Loc, Dir) Step(Map map, Loc position, Dir facing)
    {
        var next = NextLoc(position, facing);
    
        if (!IsOutOfBounds(map, next) && IsObstacle(map, next))
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
    
    char Peek(Map map, Loc loc) => map.Grid[loc.Y][loc.X];
    void Set(Map map, Loc loc, char val) => map.Grid[loc.Y][loc.X] = val;
    bool IsDir(Map map, Loc loc, Dir dir) => Peek(map, loc) == dir.ToString()[0];
    void SetDir(Map map, Loc loc, Dir dir) => Set(map, loc, dir.ToString()[0]);
    bool IsOutOfBounds(Map map, Loc loc) => loc.X < 0 || loc.X >= map.Width || loc.Y < 0 || loc.Y >= map.Height;
    bool IsObstacle(Map map, Loc loc) => Peek(map, loc) == '#';

    record Map(char[][] Grid, int Width, int Height);
    record Loc(int X, int Y);
    enum Dir { Up, Right, Down, Left }
}