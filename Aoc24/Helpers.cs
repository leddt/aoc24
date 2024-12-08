namespace Aoc24;

public static class Extensions
{
    public static string[] GetLines(this string value) => value.Split('\n').Select(x => x.Replace("\r", "")).ToArray();

    public static Dir TurnRight(this Dir dir) => dir switch
    {
        Dir.Up => Dir.Right,
        Dir.Right => Dir.Down,
        Dir.Down => Dir.Left,
        Dir.Left => Dir.Up,
        _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
    };
    
    public static Dir TurnLeft(this Dir dir) => dir switch
    {
        Dir.Up => Dir.Left,
        Dir.Right => Dir.Up,
        Dir.Down => Dir.Right,
        Dir.Left => Dir.Down,
        _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
    };

    public static char ToChar(this Dir dir) => dir.ToString()[0];
}

public class Grid(IReadOnlyList<char[]> lines)
{
    public int Width { get; } = lines[0].Length;
    public int Height { get; } = lines.Count;
    public IReadOnlyList<IReadOnlyList<char>> Lines => lines;

    public bool Contains(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
    public bool Contains(V2 v) => Contains(v.X, v.Y);

    public char this[int x, int y]
    {
        get => lines[y][x];
        set => lines[y][x] = value;
    }
    
    public char this[V2 v]
    {
        get => lines[v.Y][v.X];
        set => lines[v.Y][v.X] = value;
    }

    public static Grid Parse(string input) => new(input.GetLines().Select(x => x.ToArray()).ToArray());
}

public readonly record struct V2(int X, int Y)
{
    public static V2 operator +(V2 a, V2 b) => new(a.X + b.X, a.Y + b.Y);
    public static V2 operator -(V2 a, V2 b) => new(a.X - b.X, a.Y - b.Y);
    public static V2 operator *(V2 a, int scale) => new(a.X * scale, a.Y * scale);
    
    public V2 Move(Dir dir, int dist = 1) => this + FromDir(dir) * dist;

    public static readonly V2 Up = new(0, -1);
    public static readonly V2 Right = new(1, 0);
    public static readonly V2 Down = new(0, 1);
    public static readonly V2 Left = new(-1, 0);

    public static V2 FromDir(Dir dir) => dir switch
    {
        Dir.Up => Up,
        Dir.Right => Right,
        Dir.Down => Down,
        Dir.Left => Left,
        _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
    };
}

public enum Dir { Up, Right, Down, Left }