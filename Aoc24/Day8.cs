namespace Aoc24;

public class Day8(ITestOutputHelper output)
{
    private const string Sample = """
                                  ............
                                  ........0...
                                  .....0......
                                  .......0....
                                  ....0.......
                                  ......A.....
                                  ............
                                  ............
                                  ........A...
                                  .........A..
                                  ............
                                  ............
                                  """;
    
    [Fact] public void TestPart1() => Assert.Equal(14, RunPart1(Sample));
    [Fact] public void TestPart2() => Assert.Equal(34, RunPart2(Sample));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/8.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    int RunPart1(string input)
    {
        var map = Parse(input);

        return map.Groups
            .SelectMany(GetPairs)
            .SelectMany(p => GetSimpleNodes(map, p.a, p.b))
            .Distinct()
            .Count();
    }

    int RunPart2(string input)
    {
        var map = Parse(input);

        return map.Groups
            .SelectMany(GetPairs)
            .SelectMany(p => GetRepeatingNodes(map, p.a, p.b))
            .Distinct()
            .Count();
    }

    Map Parse(string input)
    {
        var grid = input.GetLines().Select(x => x.ToArray()).ToArray();

        var height = grid.Length;
        var width = grid.First().Length;

        var groups = new Dictionary<char, List<Loc>>();
        
        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
        {
            var type = grid[y][x];
            if (type == '.') continue;

            if (!groups.TryGetValue(type, out var group))
            {
                group = new List<Loc>();
                groups.Add(type, group);
            }

            group.Add(new Loc(x, y));
        }

        return new Map(width, height, groups.Select(x => x.Value.ToArray()).ToArray());
    }

    IEnumerable<(Loc a, Loc b)> GetPairs(Loc[] group)
    {
        for (var i = 0; i < group.Length; i++)
        for (var j = i + 1; j < group.Length; j++)
            yield return (group[i], group[j]);
    }

    IEnumerable<Loc> GetSimpleNodes(Map map, Loc a, Loc b)
    {
        var n1 = GetNext(a, b);
        var n2 = GetNext(b, a);

        if (map.IsInBounds(n1)) yield return n1;
        if (map.IsInBounds(n2)) yield return n2;
    }

    IEnumerable<Loc> GetRepeatingNodes(Map map, Loc a, Loc b)
    {
        return Follow(a, b).Concat(Follow(b, a));

        IEnumerable<Loc> Follow(Loc l1, Loc l2)
        {
            while (map.IsInBounds(l1))
            {
                yield return l1;
                (l1, l2) = (l2, GetNext(l2, l1));
            }
        }
    }

    Loc GetNext(Loc a, Loc b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return new Loc(a.X + dx, a.Y + dy);
    }

    record Map(int Width, int Height, Loc[][] Groups)
    {
        public bool IsInBounds(Loc l) => l.X >= 0 && l.X < Width && l.Y >= 0 && l.Y < Height;
    }
    record Loc(int X, int Y);
}