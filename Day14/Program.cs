using Aoc24;
using System.Text;

const int roomWidth = 101;
const int roomHeight = 103;

var robots = Day14.Parse(File.ReadAllText("input.txt")).ToList();

Console.Clear();

var t = 0;
var speed = 1;

while (true)
{
    t += speed;
    
    Day14.MoveRobots(robots, roomWidth, roomHeight, speed);
    Render(t);

    // If the robots are clustered in the center, stop and wait for input
    // Press left to go back to the previous cluster, or right to go to the next
    if (IsClustered())
    {
        var key = Console.ReadKey(true);
        speed = key.Key == ConsoleKey.LeftArrow ? -1 : 1;
    }
}

// Detects if most robots are in the mid-section horizontaly
bool IsClustered()
{
    var isMid = robots
        .GroupBy(x => x.Pos.X is >= 33 and <= 67)
        .ToDictionary(x => x.Key, x => x.Count());
    
    return isMid[true] > isMid[false];
}

void Render(int tick)
{
    var allPos = robots.Select(r => r.Pos).ToHashSet();
    var sb = new StringBuilder((roomWidth + 1) * roomHeight * 2);
        
    for (var y = 0; y < roomHeight; y++)
    {
        for (var x = 0; x < roomWidth; x++)
        {
            sb.Append(' ');
            if (allPos.Contains(new V2(x, y)))
                sb.Append('*');
            else
                sb.Append(' ');
        }

        sb.AppendLine();
    }

    var top = Console.CursorTop;
    
    Console.WriteLine("Tick: {0:0000000000}", tick);
    Console.WriteLine(sb.ToString());

    Console.CursorTop = top;
    Console.CursorLeft = 0;
}