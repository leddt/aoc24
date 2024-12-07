namespace Aoc24;

public class Day7(ITestOutputHelper output)
{
    private const string Sample = """
                                  190: 10 19
                                  3267: 81 40 27
                                  83: 17 5
                                  156: 15 6
                                  7290: 6 8 6 15
                                  161011: 16 10 13
                                  192: 17 8 14
                                  21037: 9 7 18 13
                                  292: 11 6 16 20
                                  """;

    [Fact] public void TestPart1() => Assert.Equal(3749, RunPart1(Sample));
    [Fact] public void TestPart2() => Assert.Equal(11387, RunPart2(Sample));

    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/7.txt");

        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    long RunPart1(string input)
    {
        var equations = Parse(input);
        return equations.Where(x => HasSolution(x, Op.Add, Op.Mul)).Sum(x => x.Result);
    }

    long RunPart2(string input)
    {
        var equations = Parse(input);
        return equations.Where(x => HasSolution(x, Op.Add, Op.Mul, Op.Cat)).Sum(x => x.Result);
    }

    private IEnumerable<Equation> Parse(string input)
    {
        return input.Split('\n', StringSplitOptions.TrimEntries).Select(ParseEquation);
    }


    Equation ParseEquation(string line)
    {
        var parts = line.Split(':');

        var result = long.Parse(parts[0]);
        var values = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        return new Equation(result, values);
    }

    bool HasSolution(Equation eq, params Op[] ops)
    {
        return GetAllSolutions(eq.Values, ops).Any(s => s == eq.Result);
    }

    IEnumerable<long> GetAllSolutions(int[] values, Op[] validOps)
    {
        foreach (var ops in GetOps(values.Length - 1, validOps))
            yield return Evaluate(values, ops);
    }

    IEnumerable<Op[]> GetOps(int length, Op[] validOps)
    {
        if (length == 0)
        {
            yield return [];
            yield break;
        }

        foreach (var other in GetOps(length - 1, validOps))
        foreach (var op in validOps)
        {
            yield return [op, ..other];
        }
    }

    long Evaluate(int[] values, Op[] ops)
    {
        long result = values[0];

        for (var i = 1; i < values.Length; i++)
        {
            result = ops[i - 1] switch
            {
                Op.Add => result + values[i],
                Op.Mul => result * values[i],
                Op.Cat => long.Parse($"{result}{values[i]}"),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        return result;
    }

    record Equation(long Result, int[] Values);
    enum Op { Add, Mul, Cat }
}