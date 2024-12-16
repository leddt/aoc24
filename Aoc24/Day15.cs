namespace Aoc24;

public class Day15(ITestOutputHelper output)
{
    private const string Sample = """
                                  ##########
                                  #..O..O.O#
                                  #......O.#
                                  #.OO..O.O#
                                  #..O@..O.#
                                  #O#..O...#
                                  #O..O..O.#
                                  #.OO.O.OO#
                                  #....O...#
                                  ##########
                                  
                                  <vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
                                  vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
                                  ><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
                                  <<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
                                  ^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
                                  ^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
                                  >^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
                                  <><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
                                  ^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
                                  v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^
                                  """;
    
    [Fact] public void TestPart1() => Assert.Equal(10092, RunPart1(Sample));
    [Fact] public void TestPart2() => Assert.Equal(9021, RunPart2(Sample));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/15.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    int RunPart1(string input)
    {
        var (map, moves) = ParseSimple(input);

        var robot = map.FindFirst('@');
        map[robot] = '.';
        
        foreach (var move in moves)
            robot = MoveSimple(map, robot, move);

        var boxes = map.FindAll('O');
        return boxes.Sum(b => b.X + b.Y * 100);
    }

    int RunPart2(string input)
    {
        var (map, moves) = ParseWide(input);
        
        var robot = map.FindFirst('@');
        map[robot] = '.';
        
        foreach (var move in moves)
            robot = ApplyMoveWide(map, robot, move);
        
        var boxes = map.FindAll('[');
        return boxes.Sum(b => b.X + b.Y * 100);
    }

    V2 MoveSimple(Grid map, V2 robot, Dir move)
    {
        var pos = robot + move;
        while (map[pos] == 'O')
            pos += move;

        if (map[pos] == '#') return robot;

        map.Swap(pos, robot + move);
        return robot + move;
    }

    V2 ApplyMoveWide(Grid map, V2 robot, Dir move)
    {
        var pushedBoxes = new HashSet<V2>();
        if (IsMoveBlocked(map, robot, move, pushedBoxes)) return robot;

        foreach (var box in SortBoxes(pushedBoxes, move))
        {
            map[box + move] = '[';
            map[box] = '.';
            
            map[box + move + V2.Right] = ']';
            if (map[box + V2.Right] == ']') map[box + V2.Right] = '.';
        }
        
        return robot + move;
    }

    private IEnumerable<V2> SortBoxes(IEnumerable<V2> boxes, Dir direction) => direction switch
    {
        Dir.Up => boxes.OrderBy(b => b.Y),
        Dir.Right => boxes.OrderByDescending(b => b.X),
        Dir.Down => boxes.OrderByDescending(b => b.Y),
        Dir.Left => boxes.OrderBy(b => b.X),
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };

    bool IsMoveBlocked(Grid map, V2 pos, Dir move, ISet<V2> boxes)
    {
        if (map[pos + move] == '#') return true;

        if (move == Dir.Left && map[pos + move] == ']')
        {
            boxes.Add(pos + move + move);
            return IsMoveBlocked(map, pos + move + move, move, boxes);
        }

        if (move == Dir.Right && map[pos + move] == '[')
        {
            boxes.Add(pos + move);
            return IsMoveBlocked(map, pos + move + move, move, boxes);
        }

        if (move is Dir.Up or Dir.Down && map[pos + move] == '[')
        {
            boxes.Add(pos + move);
            return IsMoveBlocked(map, pos + move, move, boxes) ||
                   IsMoveBlocked(map, pos + move + Dir.Right, move, boxes);
        }

        if (move is Dir.Up or Dir.Down && map[pos + move] == ']')
        {
            boxes.Add(pos + move + Dir.Left);
            return IsMoveBlocked(map, pos + move, move, boxes) ||
                   IsMoveBlocked(map, pos + move + Dir.Left, move, boxes);
        }

        return false;
    }

    (Grid map, Dir[] moves) ParseSimple(string input)
    {
        var parts = input.Replace("\r", "").Split("\n\n");
        
        var map = Grid.Parse(parts[0]);
        var moves = ParseMoves(parts[1]);

        return (map, moves);
    }

    (Grid map, Dir[] moves) ParseWide(string input)
    {
        var parts = input.Replace("\r", "").Split("\n\n");

        var width = parts[0].IndexOf('\n') * 2;
        var map = Grid.Parse(parts[0].SelectMany<char, char>(c => c switch
        {
            '@' => ['@', '.'],
            'O' => ['[', ']'],
            '\n' => [],
            _ => [c, c]
        }), width);
        
        var moves = ParseMoves(parts[1]);

        return (map, moves);
    }

    private static Dir[] ParseMoves(string movesString)
    {
        return movesString
            .Where(c => "^>v<".Contains(c))
            .Select(c => c switch
            {
                '^' => Dir.Up,
                '>' => Dir.Right,
                'v' => Dir.Down,
                '<' => Dir.Left,
                _ => throw new ArgumentOutOfRangeException()
            })
            .ToArray();
    }
}