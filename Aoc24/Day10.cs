namespace Aoc24;

public class Day10(ITestOutputHelper output)
{
    private const string Sample = """
                                  89010123
                                  78121874
                                  87430965
                                  96549874
                                  45678903
                                  32019012
                                  01329801
                                  10456732
                                  """;
    
    [Fact] public void TestPart1() => Assert.Equal(36, RunPart1(Sample));
    [Fact] public void TestPart2() => Assert.Equal(81, RunPart2(Sample));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/10.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    int RunPart1(string input)
    {
        var grid = Grid.ParseInts(input);
        var heads = grid.FindAll(0);

        return heads.Sum(h => GetHeadScoreP1(grid, h));
    }

    int RunPart2(string input)
    {
        var grid = Grid.ParseInts(input);
        var heads = grid.FindAll(0);

        return heads.Sum(h => GetHeadScoreP2(grid, h));
    }

    int GetHeadScoreP1(Grid<int> grid, V2 head) => FindPeaks(grid, head).Distinct().Count();
    int GetHeadScoreP2(Grid<int> grid, V2 head) => FindPeaks(grid, head).Count();

    IEnumerable<V2> FindPeaks(Grid<int> grid, V2 loc)
    {
        if (grid[loc] == 9) return [loc];
        
        return grid.Neighbors(loc).Where(x => x.c == grid[loc] + 1).SelectMany(x => FindPeaks(grid, x.pos));
    }
}