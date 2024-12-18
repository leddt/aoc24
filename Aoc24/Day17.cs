using System.Numerics;

namespace Aoc24;

public class Day17(ITestOutputHelper output)
{
    private const string Sample_1 = """
                                  Register A: 729
                                  Register B: 0
                                  Register C: 0
                                  
                                  Program: 0,1,5,4,3,0
                                  """;

    private const string Sample_2 = """
                                    Register A: 2024
                                    Register B: 0
                                    Register C: 0

                                    Program: 0,3,5,4,3,0
                                    """;
    
    [Fact] public void TestPart1() => Assert.Equal("4,6,3,5,6,3,5,2,1,0", RunPart1(Sample_1));
    [Fact] public void TestPart2() => Assert.Equal(117440, RunPart2(Sample_2));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/17.txt");
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    string RunPart1(string input)
    {
        var vm = Parse(input);
        var result = string.Join(',', vm.Execute());
        return result;
    }

    BigInteger RunPart2(string input)
    {
        var vm = Parse(input);

        return FindSolutions([], vm.Program).Select(Combine).Order().First();

        IEnumerable<int[]> FindSolutions(int[] soFar, int[] target)
        {
            for (var i = 0; i < 8; i++)
            {
                int[] candidate = [..soFar, i];
                
                vm.Reset(Combine(candidate));
                var result = vm.Execute().ToArray();

                if (result.First() != target[^(soFar.Length + 1)]) continue;
                
                if (result.Length == target.Length)
                {
                    yield return candidate;
                }
                else
                {
                    foreach (var s in FindSolutions(candidate, target))
                        yield return s;
                }
            }
        }

        BigInteger Combine(int[] values) => values.Aggregate(BigInteger.Zero, (a, c) => a * 8 + c);
    }

    Vm Parse(string input)
    {
        var lines = input.GetLines();

        var a = long.Parse(lines[0].Split(':', StringSplitOptions.TrimEntries)[1]);
        var b = long.Parse(lines[1].Split(':', StringSplitOptions.TrimEntries)[1]);
        var c = long.Parse(lines[2].Split(':', StringSplitOptions.TrimEntries)[1]);
        
        var program = lines[4].Split(':', StringSplitOptions.TrimEntries)[1];
        var instructions = program.Split(',').Select(int.Parse);

        return new Vm(a, b, c, instructions.ToArray());
    }

    class Vm(BigInteger a, BigInteger b, BigInteger c, int[] program)
    {
        public int[] Program => program;
        
        private int _ip = 0;
        private Inst ReadInst() => (Inst)program[_ip++];
        private int ReadLiteral() => program[_ip++];

        private BigInteger ReadCombo()
        {
            var val = ReadLiteral();

            return val switch
            {
                <= 3 => val,
                4 => a,
                5 => b,
                6 => c,
                _ => throw new InvalidOperationException()
            };
        }

        public void Reset(BigInteger newA)
        {
            a = newA;
            b = c = _ip = 0;
        }
        
        public IEnumerable<int> Execute()
        {
            while (_ip < program.Length)
            {
                switch (ReadInst())
                {
                    case Inst.Adv:
                        a /= (BigInteger)Math.Pow(2, (int)ReadCombo());
                        break;
                    case Inst.Bxl:
                        b ^= ReadLiteral() >>> 0;
                        break;
                    case Inst.Bst:
                        b = ReadCombo() % 8;
                        break;
                    case Inst.Jnz:
                        if (a == 0)
                            _ip++; // Do nothing
                        else
                            _ip = ReadLiteral();
                        break;
                    case Inst.Bxc:
                        b ^= c;
                        ReadLiteral();
                        break;
                    case Inst.Out:
                        yield return (int)(ReadCombo() % 8);
                        break;
                    case Inst.Bdv:
                        b = a / (BigInteger)Math.Pow(2, (int)ReadCombo());
                        break;
                    case Inst.Cdv:
                        c = a / (BigInteger)Math.Pow(2, (int)ReadCombo());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
    
    enum Inst { Adv = 0, Bxl, Bst, Jnz, Bxc, Out, Bdv, Cdv }
}