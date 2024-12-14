namespace Aoc24;

public partial class Day14(ITestOutputHelper output)
{
    private const string Sample = """
                                  p=0,4 v=3,-3
                                  p=6,3 v=-1,-3
                                  p=10,3 v=-1,2
                                  p=2,0 v=2,-1
                                  p=0,0 v=1,3
                                  p=3,0 v=-2,-2
                                  p=7,6 v=-1,-3
                                  p=3,0 v=-1,-2
                                  p=9,3 v=2,3
                                  p=7,3 v=-1,2
                                  p=2,4 v=2,-3
                                  p=9,5 v=-3,-3
                                  """;

    private const int SampleRoomWidth = 11;
    private const int SampleRoomHeight = 7;
    private const int RoomWidth = 101;
    private const int RoomHeight = 103;
    
    [Fact] public void TestPart1() => Assert.Equal(12, RunPart1(Sample, SampleRoomWidth, SampleRoomHeight));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/14.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input, RoomWidth, RoomHeight)}");
        // I solved part 2 using a visualizer built as a separate console app
        // My input drew the tree at tick 7132
    }

    int RunPart1(string input, int roomWidth, int roomHeight)
    {
        var robots = Parse(input).ToList();
        
        for (var tick = 0; tick < 100; tick++)
        {
            MoveRobots(robots, roomWidth, roomHeight);
        }

        return robots
            .GroupBy(r => GetQuadrant(r.Pos, roomWidth, roomHeight))
            .Where(x => x.Key > 0)
            .Aggregate(1, (a, g) => a * g.Count());
    }

#pragma warning disable xUnit1013
    public static void MoveRobots(IEnumerable<Robot> robots, int roomWidth, int roomHeight, int speed = 1)
#pragma warning restore xUnit1013
    {
        foreach (var robot in robots)
        {
            robot.Pos = WrapAround(robot.Pos + robot.Vel * speed, roomWidth, roomHeight) ;
        }
    }

    int GetQuadrant(V2 pos, int roomWidth, int roomHeight)
    {
        var midW = roomWidth / 2;
        var midH = roomHeight / 2;
        
        if (pos.X == midW) return 0;
        if (pos.Y == midH) return 0;

        var q = 1;
        if (pos.X > midW) q += 1;
        if (pos.Y > midH) q += 2;

        return q;
    }

    private static V2 WrapAround(V2 pos, int roomWidth, int roomHeight)
    {
        var x = pos.X;
        while (x < 0) x += roomWidth;
        while (x >= roomWidth) x -= roomWidth;
        
        var y = pos.Y;
        while (y < 0) y += roomHeight;
        while (y >= roomHeight) y -= roomHeight;

        return new V2(x, y);
    }

    public static IEnumerable<Robot> Parse(string input)
    {
        var regex = GetRobotRegex();
        foreach (var line in input.GetLines())
        {
            var match = regex.Match(line);
            var px = int.Parse(match.Groups[1].ValueSpan);
            var py = int.Parse(match.Groups[2].ValueSpan);
            var vx = int.Parse(match.Groups[3].ValueSpan);
            var vy = int.Parse(match.Groups[4].ValueSpan);

            yield return new Robot(new V2(px, py), new V2(vx, vy));
        } 
    }

    public class Robot(V2 pos, V2 vel)
    {
        public V2 Pos { get; set; } = pos;
        public V2 Vel { get; set; } = vel;
    }

    [GeneratedRegex(@"p=(-?\d+),(-?\d+) v=(-?\d+),(-?\d+)")]
    private static partial Regex GetRobotRegex();
}