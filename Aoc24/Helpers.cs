namespace Aoc24;

public static class Helpers
{
    public static string[] GetLines(this string value) => value.Split('\n').Select(x => x.Replace("\r", "")).ToArray();
}