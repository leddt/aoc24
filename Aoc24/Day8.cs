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
        var grid = Grid.Parse(input);

        var groups = new Dictionary<char, List<V2>>();
        
        grid.ForEach(pos =>
        {
            var type = grid[pos];
            if (type == '.') return;

            if (!groups.TryGetValue(type, out var group))
            {
                group = new List<V2>();
                groups.Add(type, group);
            }

            group.Add(pos);
        });

        return new Map(grid, groups.Select(x => x.Value.ToArray()).ToArray());
    }

    IEnumerable<(V2 a, V2 b)> GetPairs(V2[] group)
    {
        for (var i = 0; i < group.Length; i++)
        for (var j = i + 1; j < group.Length; j++)
            yield return (group[i], group[j]);
    }

    IEnumerable<V2> GetSimpleNodes(Map map, V2 a, V2 b)
    {
        var n1 = GetNext(a, b);
        var n2 = GetNext(b, a);

        if (map.Grid.Contains(n1)) yield return n1;
        if (map.Grid.Contains(n2)) yield return n2;
    }

    IEnumerable<V2> GetRepeatingNodes(Map map, V2 a, V2 b)
    {
        return Follow(a, b).Concat(Follow(b, a));

        IEnumerable<V2> Follow(V2 l1, V2 l2)
        {
            while (map.Grid.Contains(l1))
            {
                yield return l1;
                (l1, l2) = (l2, GetNext(l2, l1));
            }
        }
    }

    V2 GetNext(V2 a, V2 b) => a + (a - b);
    record Map(Grid Grid, V2[][] Groups);
}