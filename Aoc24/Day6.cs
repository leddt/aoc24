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
        
        var position = map.FindAll('^').First();
        var facing = Dir.Up;

        while (true)
        {
            map[position] = 'X';

            (position, facing) = Step(map, position, facing);

            if (!map.Contains(position)) break;
        }

        return map.Lines.Sum(y => y.Count(x => x == 'X'));
    }

    int RunPart2(string input)
    {
        var map = Grid.Parse(input);
        
        var start = map.FindAll('^').First();
        var count = 0;

        map.ForEach(pos =>
        {
            if (map[pos] != '.') return;
            var candidateMap = CreateCandidate(map, pos);
            if (IsLoop(candidateMap, start)) count++;
        });

        return count;
    }

    Grid CreateCandidate(Grid source, V2 pos) =>
        new(source.Lines.Select(l => l.ToArray()).ToArray())
        {
            [pos] = '#'
        };

    bool IsLoop(Grid map, V2 position)
    {
        var facing = Dir.Up;
    
        while (true)
        {
            if (!map.Contains(position)) return false;
            if (map[position] == facing.ToChar()) return true;
            if (map[position] is '.' or '^') map[position] = facing.ToChar();
    
            (position, facing) = Step(map, position, facing);
        }
    }
    
    (V2, Dir) Step(Grid map, V2 position, Dir facing)
    {
        var next = position + V2.FromDir(facing);
    
        if (map.Contains(next) && map[next] == '#')
            facing = facing.TurnRight();
        else
            position = next;
    
        return (position, facing);
    }
}