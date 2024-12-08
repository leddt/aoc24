namespace Aoc24;

public static class Helpers
{
    public static string[] GetLines(this string value) => value.Split('\n').Select(x => x.Replace("\r", "")).ToArray();
}

public class Grid(IReadOnlyList<char[]> lines)
{
    public int Width { get; } = lines.First().Length;
    public int Height { get; } = lines.Count;
    public IReadOnlyList<IReadOnlyList<char>> Lines => lines;

    public bool Contains(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
    public bool Contains(V2 v) => Contains(v.X, v.Y);
    public char Get(V2 v) => Get(v.X, v.Y);
    public char Get(int x, int y) => lines[y][x];
    public void Set(V2 v, char c) => Set(v.X, v.Y, c);
    public void Set(int x, int y, char c) => lines[y][x] = c;

    public char this[int x, int y]
    {
        get => Get(x, y);
        set => Set(x, y, value);
    }
    
    public char this[V2 v]
    {
        get => Get(v);
        set => Set(v, value);
    }

    public static Grid Parse(string input) => new(input.GetLines().Select(x => x.ToArray()).ToArray());
}

public record struct V2(int X, int Y)
{
    public static V2 operator +(V2 a, V2 b) => new(a.X + b.X, a.Y + b.Y);
    public static V2 operator -(V2 a, V2 b) => new(a.X - b.X, a.Y - b.Y);
}