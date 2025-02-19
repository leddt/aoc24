﻿using System.Text;

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

    public void Swap(V2 a, V2 b) => (this[a], this[b]) = (this[b], this[a]);

    public bool Contains(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
    public bool Contains(V2 v) => Contains(v.X, v.Y);

    public IEnumerable<V2> All()
    {
        for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
            yield return new V2(x, y);
    }

    public IEnumerable<T> Col(int col)
    {
        for (var row = 0; row < Height; row++)
            yield return this[col, row];
    }

    public IEnumerable<T> Row(int row)
    {
        for (var col = 0; col < Width; col++)
            yield return this[col, row];
    }
    
    public IEnumerable<V2> FindAll(T c) => All().Where(x => Equals(this[x], c));
    public V2 FindFirst(T c) => All().First(x => Equals(this[x], c));

    public IEnumerable<(V2 pos, T val)> Neighbors(V2 pos, Func<(V2 pos, T val), bool>? predicate = null)
    {
        V2[] all = [
            pos + V2.Up, 
            pos + V2.Right, 
            pos + V2.Down, 
            pos + V2.Left
        ];

        predicate ??= _ => true;
        return all.Where(Contains).Select(x => (x, this[x])).Where(x => predicate(x));
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

    public override string ToString()
    {
        var sb = new StringBuilder();
        
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                sb.Append(this[x, y]);
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}

public class Grid(IReadOnlyList<char[]> lines) : Grid<char>(lines)
{
    public static Grid Parse(IEnumerable<char> input, int width) => new(input.Chunk(width).ToArray());
    public static Grid Parse(string input) => Parse(input.GetLines());
    public static Grid Parse(IEnumerable<string> lines) => new(lines.Select(x => x.ToArray()).ToArray());
    public static Grid<int> ParseInts(string input) => Parse(input, c => c - '0');
    public static Grid<T> Parse<T>(string input, Func<char, T> map) => new(input.GetLines().Select(x => x.Select(map).ToArray()).ToArray());

    public static Grid<T> Filled<T>(int width, int height, T val) => new(
        Enumerable.Range(0, height)
            .Select(_ => Enumerable.Repeat(val, width).ToArray())
            .ToArray()
    );
}

public readonly record struct V2(int X, int Y)
{
    public int XyLength => Math.Abs(X) + Math.Abs(Y);
    
    public static V2 operator +(V2 a, V2 b) => new(a.X + b.X, a.Y + b.Y);
    public static V2 operator -(V2 a, V2 b) => new(a.X - b.X, a.Y - b.Y);
    public static V2 operator *(V2 a, int scale) => new(a.X * scale, a.Y * scale);
    
    public static readonly V2 Up = new(0, -1);
    public static readonly V2 Right = new(1, 0);
    public static readonly V2 Down = new(0, 1);
    public static readonly V2 Left = new(-1, 0);

    public static implicit operator V2(Dir dir) => FromDir(dir);

    public static V2 FromDir(Dir dir) => dir switch
    {
        Dir.Up => Up,
        Dir.Right => Right,
        Dir.Down => Down,
        Dir.Left => Left,
        _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
    };

    public override string ToString() => $"V2({X},{Y})";
}

public enum Dir { Up, Right, Down, Left }