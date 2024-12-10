namespace Aoc24;

public static class Extensions
{
    public static string[] GetLines(this string value) => value.Split('\n').Select(x => x.Replace("\r", "")).ToArray();

    private static readonly int DirCount = Enum.GetValues<Dir>().Length;
    public static Dir TurnRight(this Dir dir) => (Dir)(((int)dir + 1) % DirCount);
    public static Dir TurnLeft(this Dir dir) => (Dir)(((int)dir - 1 + DirCount) % DirCount);

    public static char ToChar(this Dir dir) => dir.ToString()[0];
}

public class Grid<T>(IReadOnlyList<T[]> lines)
{
    public int Width { get; } = lines[0].Length;
    public int Height { get; } = lines.Count;
    public IReadOnlyList<IReadOnlyList<T>> Lines => lines;

    public bool Contains(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
    public bool Contains(V2 v) => Contains(v.X, v.Y);

    public IEnumerable<V2> All()
    {
        for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
            yield return new V2(x, y);
    }
    
    public IEnumerable<V2> FindAll(T c) => All().Where(x => Equals(this[x], c));

    public IEnumerable<(V2 pos, T val)> Neighbors(V2 pos)
    {
        V2[] all = [
            pos + V2.Up, 
            pos + V2.Right, 
            pos + V2.Down, 
            pos + V2.Left
        ];

        return all.Where(Contains).Select(x => (x, this[x]));
    }

    public void ForEach(Action<V2> action)
    {
        foreach (var pos in All()) 
            action(pos);
    }

    public T this[int x, int y]
    {
        get => lines[y][x];
        set => lines[y][x] = value;
    }
    
    public T this[V2 v]
    {
        get => lines[v.Y][v.X];
        set => lines[v.Y][v.X] = value;
    }
}

public class Grid(IReadOnlyList<char[]> lines) : Grid<char>(lines)
{
    public static Grid Parse(string input) => new(input.GetLines().Select(x => x.ToArray()).ToArray());
    public static Grid<int> ParseInts(string input) => Parse(input, c => c - '0');
    public static Grid<T> Parse<T>(string input, Func<char, T> map) => new(input.GetLines().Select(x => x.Select(map).ToArray()).ToArray());
}

public readonly record struct V2(int X, int Y)
{
    public static V2 operator +(V2 a, V2 b) => new(a.X + b.X, a.Y + b.Y);
    public static V2 operator -(V2 a, V2 b) => new(a.X - b.X, a.Y - b.Y);
    public static V2 operator *(V2 a, int scale) => new(a.X * scale, a.Y * scale);
    
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