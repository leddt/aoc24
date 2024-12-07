namespace Aoc24;

public class Day4(ITestOutputHelper output)
{
    private const string Sample = """
                                  MMMSXXMASM
                                  MSAMXMSMSA
                                  AMXSXMAAMM
                                  MSAMASMSMX
                                  XMASAMXAMM
                                  XXAMMXXAMA
                                  SMSMSASXSS
                                  SAXAMASAAA
                                  MAMMMXMMMM
                                  MXMXAXMASX
                                  """;
    
    [Fact] public void TestPart1() => Assert.Equal(18, RunPart1(Sample));
    [Fact] public void TestPart2() => Assert.Equal(9, RunPart2(Sample));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/4.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    int RunPart1(string input)
    {
        var map = Parse(input);

        var count = 0;

        for (var x = 0; x < map.Width; x++)
        for (var y = 0; y < map.Height; y++)
        {
            if (LookDir(map, x, y, 0, 1)) count++;
            if (LookDir(map, x, y, 0, -1)) count++;
            if (LookDir(map, x, y, 1, 0)) count++;
            if (LookDir(map, x, y, 1, 1)) count++;
            if (LookDir(map, x, y, 1, -1)) count++;
            if (LookDir(map, x, y, -1, 0)) count++;
            if (LookDir(map, x, y, -1, 1)) count++;
            if (LookDir(map, x, y, -1, -1)) count++;
        }

        return count;
    }

    int RunPart2(string input)
    {
        var map = Parse(input);

        var count = 0;

        for (var x = 0; x < map.Width; x++)
        for (var y = 0; y < map.Height; y++)
        {
            if (map.Grid[y][x] != 'A') continue;
            if (CheckDiag(map, x, y, 1) && CheckDiag(map, x, y, -1)) count++;
        }

        return count;
    }

    Map Parse(string input)
    {
        var grid = input.Split('\n', StringSplitOptions.TrimEntries);
        var width = grid[0].Length;
        var height = grid.Length;
        
        return new Map(grid, width, height);
    }
    
    bool LookDir(Map map, int x, int y, int dx, int dy, int c = 0)
    {
        const string lookingFor = "XMAS";
        
        if (map.Grid[y][x] != lookingFor[c]) return false;
        if (c == lookingFor.Length - 1) return true;

        var nextX = x + dx;
        var nextY = y + dy;

        if (nextX < 0 || nextX >= map.Width) return false;
        if (nextY < 0 || nextY >= map.Height) return false;

        return LookDir(map, nextX, nextY, dx, dy, c + 1);
    }

    bool CheckDiag(Map map, int x, int y, int diag)
    {
        if (x == 0 || x == map.Width - 1 || y == 0 || y == map.Height - 1) return false;

        var left = map.Grid[y + diag][x - 1];
        var right = map.Grid[y - diag][x + 1];

        char[] chars = [left, right];
        return chars.Contains('M') && chars.Contains('S');
    }

    record Map(string[] Grid, int Width, int Height);
}