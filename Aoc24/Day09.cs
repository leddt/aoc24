namespace Aoc24;

public class Day09(ITestOutputHelper output)
{
    private const string Sample = """
                                  2333133121414131402
                                  """;
    
    [Fact] public void TestPart1() => Assert.Equal(1928, RunPart1(Sample));
    [Fact] public void TestPart2() => Assert.Equal(2858, RunPart2(Sample));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/9.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    long RunPart1(string input)
    {
        var fs = ParseFileSystem(input, asSingleByte: true);

        for (var i = fs.Count - 1; i >= 0; i--)
        {
            if (fs[i].Id == null) continue;

            for (var j = 0; j < i; j++)
            {
                if (fs[j].Id != null) continue;

                // Swap blocks
                (fs[i], fs[j]) = (fs[j], fs[i]);
                break;
            }
        }

        return ComputeChecksum(fs);
    }

    long RunPart2(string input)
    {
        var fs = ParseFileSystem(input, asSingleByte: false);
        
        for (var i = fs.Count - 1; i >= 0; i--)
        {
            if (fs[i].Id == null) continue;

            for (var j = 0; j < i; j++)
            {
                if (fs[j].Id != null) continue;
                if (fs[j].Length < fs[i].Length) continue;

                if (fs[j].Length != fs[i].Length)
                {
                    // Split free space
                    var a = new Block(null, fs[i].Length);
                    var b = new Block(null, fs[j].Length - fs[i].Length);

                    fs.RemoveAt(j);
                    fs.InsertRange(j, [a, b]);

                    i++;
                }
                
                // Swap blocks
                (fs[i], fs[j]) = (fs[j], fs[i]);
                break;
            }
        }

        return ComputeChecksum(fs);
    }

    private static List<Block> ParseFileSystem(string input, bool asSingleByte)
    {
        var fs = new List<Block>();
        
        for (var i = 0; i < input.Length; i++)
        {
            var length = int.Parse(input[i..(i + 1)]);
            if (length == 0) continue;

            if (i % 2 == 0)
                AddBlock(i / 2, length);
            else
                AddBlock(null, length);
        }

        return fs;

        void AddBlock(int? id, int length)
        {
            if (asSingleByte)
                fs.AddRange(Enumerable.Repeat(new Block(id, 1), length));
            else
                fs.Add(new Block(id, length));
        }
    }

    private static long ComputeChecksum(List<Block> fs)
    {
        var offset = 0;
        var sum = 0L;

        foreach (var block in fs)
        {
            for (var i = offset; i < offset + block.Length; i++)
                sum += i * block.Id ?? 0;

            offset += block.Length;
        }

        return sum;
    }

    record Block(int? Id, int Length);
}