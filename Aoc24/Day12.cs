namespace Aoc24;

public class Day12(ITestOutputHelper output)
{
    private const string Sample = """
                                  RRRRIICCFF
                                  RRRRIICCCF
                                  VVRRRCCFFF
                                  VVRCCCJFFF
                                  VVVVCJJCFE
                                  VVIVCCJJEE
                                  VVIIICJJEE
                                  MIIIIIJJEE
                                  MIIISIJEEE
                                  MMMISSJEEE
                                  """;
    
    [Fact] public void TestPart1() => Assert.Equal(1930, RunPart1(Sample));
    [Fact] public void TestPart2() => Assert.Equal(1206, RunPart2(Sample));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/12.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    int RunPart1(string input)
    {
        var regions = GetAllRegions(input);

        return regions.Sum(r => GetArea(r) * GetPerimeter(r));
    }

    int RunPart2(string input)
    {
        var regions = GetAllRegions(input);

        return regions.Sum(r => GetArea(r) * GetSideCount(r));
    }

    private List<HashSet<V2>> GetAllRegions(string input)
    {
        var grid = Grid.Parse(input);
        var visited = new HashSet<V2>();
        var regions = new List<HashSet<V2>>();

        foreach (var pos in grid.All())
        {
            if (visited.Contains(pos)) continue;

            var region = ScanRegion(grid, visited, pos);
            regions.Add(region);
        }

        return regions;
    }

    HashSet<V2> ScanRegion(Grid grid, HashSet<V2> visited, V2 start)
    {
        visited.Add(start);
        
        var neighbors = grid.Neighbors(start)
            .Where(n => !visited.Contains(n.pos))
            .Where(n => n.val == grid[start])
            .Select(n => n.pos)
            .ToList();
        
        return neighbors.SelectMany(n => ScanRegion(grid, visited, n)).Append(start).ToHashSet();
    }

    int GetArea(IEnumerable<V2> region) => region.Count();

    int GetPerimeter(HashSet<V2> region)
    {
        var perimeter = 0;
        
        foreach (var pos in region)
        {
            if (!region.Contains(pos + V2.Up)) perimeter++;
            if (!region.Contains(pos + V2.Right)) perimeter++;
            if (!region.Contains(pos + V2.Down)) perimeter++;
            if (!region.Contains(pos + V2.Left)) perimeter++;
        }
        
        return perimeter;
    }

    int GetSideCount(HashSet<V2> region)
    {
        var walls = new List<(V2 dir, List<V2> positions)>();
        
        foreach (var pos in region.OrderBy(x => x.X).ThenBy(x => x.Y))
        {
            CheckWall(pos, Dir.Up);
            CheckWall(pos, Dir.Right);
            CheckWall(pos, Dir.Down);
            CheckWall(pos, Dir.Left);
        }
        
        return walls.Count;

        void CheckWall(V2 pos, Dir dir)
        {
            var vDir = V2.FromDir(dir);
            
            if (!region.Contains(pos + vDir))
            {
                var wall = GetOrAddWall(pos, vDir, dir.TurnLeft(), dir.TurnRight());
                wall.positions.Add(pos);
            }
        }

        (V2 dir, List<V2> positions) GetOrAddWall(V2 pos, V2 dir, params Dir[] sides)
        {
            var wall = walls.FirstOrDefault(w => w.dir == dir && w.positions.Intersect(sides.Select(s => pos + V2.FromDir(s))).Any());
            
            if (wall == default)
            {
                wall = (dir, new List<V2>());
                walls.Add(wall);
            }

            return wall;
        }
    }
}