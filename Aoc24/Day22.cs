namespace Aoc24;
using Seq = (int,int,int,int);

public class Day22(ITestOutputHelper output)
{
    private const string Sample1 = """
                                  1
                                  10
                                  100
                                  2024
                                  """;
    private const string Sample2 = """
                                   1
                                   2
                                   3
                                   2024
                                   """;
    
    [Fact] public void TestPart1() => Assert.Equal(37327623, RunPart1(Sample1));
    [Fact] public void TestPart2() => Assert.Equal(23, RunPart2(Sample2));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/22.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    long RunPart1(string input)
    {
        var numbers = input.GetLines().Select(long.Parse);
        var result = 0L;
        
        foreach (var number in numbers)
        {
            var n = number;
            for (var i = 0; i < 2000; i++)
                n = Next(n);
            result += n;
        }

        return result;
    }

    long RunPart2(string input)
    {
        var numbers = input.GetLines().Select(long.Parse).ToArray();
        var prices = new int[numbers.Length][];
        var seqs = new Seq[numbers.Length][];

        for (var i = 0; i < numbers.Length; i++)
        {
            prices[i] = new int[2000];
            seqs[i] = new Seq[2000];
            
            var seq = new Seq(0, 0, 0, 0);
            var n = numbers[i];
            for (var j = 0; j < 2000; j++)
            {
                prices[i][j] = (int)(n % 10);

                if (j > 0)
                {
                    var change = prices[i][j] - prices[i][j - 1];
                    
                    var (_, b, c, d) = seq;
                    seq = (b, c, d, change);

                    if (j >= 4)
                        seqs[i][j] = seq;
                }
                
                n = Next(n);
            }
        }

        var allSeqs = new HashSet<Seq>();
        for (var i = 0; i < numbers.Length; i++)
        for (var j = 4; j < 2000; j++)
            allSeqs.Add(seqs[i][j]);

        // Brute force FTW
        return allSeqs.AsParallel().Max(GetSeqValue);

        long GetSeqValue(Seq seq)
        {
            var result = 0L;
            
            for (var i = 0; i < numbers.Length; i++)
            {
                var j = Array.IndexOf(seqs[i], seq);
                if (j >= 4)
                    result += prices[i][j];
            }

            return result;
        }
    }

    long Next(long secret)
    {
        secret = Prune(Mix(secret * 64, secret));
        secret = Prune(Mix(secret / 32, secret));
        secret = Prune(Mix(secret * 2048, secret));

        return secret;
    }

    long Mix(long n, long secret) => n ^ secret;
    long Prune(long secret) => secret % 16777216;
}