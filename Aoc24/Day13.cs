using MathNet.Numerics.LinearAlgebra;
using P = (long X, long Y);

namespace Aoc24;

public partial class Day13(ITestOutputHelper output)
{
    private const string Sample = """
                                  Button A: X+94, Y+34
                                  Button B: X+22, Y+67
                                  Prize: X=8400, Y=5400
                                  
                                  Button A: X+26, Y+66
                                  Button B: X+67, Y+21
                                  Prize: X=12748, Y=12176
                                  
                                  Button A: X+17, Y+86
                                  Button B: X+84, Y+37
                                  Prize: X=7870, Y=6450
                                  
                                  Button A: X+69, Y+23
                                  Button B: X+27, Y+71
                                  Prize: X=18641, Y=10279
                                  """;
    
    [Fact] public void TestPart1() => Assert.Equal(480, RunPart1(Sample));
    [Fact] public void TestPart2() => Assert.Equal(875318608908, RunPart2(Sample));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/13.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    long RunPart1(string input)
    {
        var machines = Parse(input);
        return machines.Select(CalculateCost).Sum();
    }

    long RunPart2(string input)
    {
        var machines = Parse(input, 10_000_000_000_000);
        return machines.Select(CalculateCost).Sum();
    }

    long CalculateCost(Machine m)
    {
        // Using Math.Net to solve these equations, following docs here: https://numerics.mathdotnet.com/LinearEquations
        // EQ1: (a * AX) + (b * BX) = PrizeX
        // EQ2: (a * AY) + (b * BY) = PrizeY

        var buttonValues = Matrix<double>.Build.DenseOfArray(new double[,]
        {
            { m.A.X, m.B.X },
            { m.A.Y, m.B.Y }
        });
        
        var targetResult = Vector<double>.Build.Dense([m.Prize.X, m.Prize.Y]);
        var solution = buttonValues.Solve(targetResult);
        if (targetResult == null) return 0;

        var aPresses = Math.Round(solution[0], 3);
        var bPresses = Math.Round(solution[1], 3);
        
        // Ensure solution is whole numbers
        if (aPresses % 1 != 0 || bPresses % 1 != 0) return 0;

        return (long)(aPresses * 3 + bPresses);
    }
    
    record Machine(P A, P B, P Prize);

    IEnumerable<Machine> Parse(string input, long prizeOffset = 0)
    {
        var lines = input.GetLines();
        var regex = GetXyRegex();

        var x = 0;
        while (x < lines.Length)
        {
            var a = regex.Match(lines[x]);
            var b = regex.Match(lines[x + 1]);
            var prize = regex.Match(lines[x + 2]);

            yield return new Machine(ToP(a), ToP(b), ToP(prize, prizeOffset));
            
            x += 4;
        }

        P ToP(Match m, long offset = 0) => (
            long.Parse(m.Groups[1].ValueSpan) + offset,
            long.Parse(m.Groups[2].ValueSpan) + offset);
    }

    [GeneratedRegex(@"X.(\d+), Y.(\d+)")]
    private static partial Regex GetXyRegex();
}