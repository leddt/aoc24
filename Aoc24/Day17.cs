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
        var vm = Parse(input, out _);
        var result = string.Join(',', Execute(vm));
        return result;
    }

    int RunPart2(string input)
    {
        var a = 0;
        var vm = Parse(input, out var targetProgram);
        string result;

        do
        {
            vm.Reset(a++);
            result = string.Join(',', Execute(vm));
        } while (result != targetProgram);

        return a - 1;
    }

    private IEnumerable<long> Execute(VM vm)
    {
        while (vm.IP < vm.Program.Length)
        {
            switch (vm.ReadInst())
            {
                case Inst.Halt:
                    yield break;
                case Inst.Adv:
                    vm.A /= (long)Math.Pow(2, vm.ReadCombo());
                    break;
                case Inst.Bxl:
                    vm.B ^= vm.ReadLiteral();
                    break;
                case Inst.Bst:
                    vm.B = vm.ReadCombo() % 8;
                    break;
                case Inst.Jnz:
                    if (vm.A == 0)
                    {
                        vm.ReadLiteral();
                        // Do nothing
                    }
                    else
                    {
                        vm.IP = vm.ReadLiteral();
                    }
                    break;
                case Inst.Bxc:
                    vm.B ^= vm.C;
                    vm.ReadLiteral();
                    break;
                case Inst.Out:
                    yield return vm.ReadCombo() % 8;
                    break;
                case Inst.Bdv:
                    vm.B = vm.A / (long)Math.Pow(2, vm.ReadCombo());
                    break;
                case Inst.Cdv:
                    vm.C = vm.A / (long)Math.Pow(2, vm.ReadCombo());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    VM Parse(string input, out string originalProgram)
    {
        var lines = input.GetLines();

        var a = long.Parse(lines[0].Split(':', StringSplitOptions.TrimEntries)[1]);
        var b = long.Parse(lines[1].Split(':', StringSplitOptions.TrimEntries)[1]);
        var c = long.Parse(lines[2].Split(':', StringSplitOptions.TrimEntries)[1]);
        
        originalProgram = lines[4].Split(':', StringSplitOptions.TrimEntries)[1];
        var instructions = originalProgram.Split(',').Select(long.Parse);

        return new VM(a, b, c, instructions.ToArray());
    }

    class VM(long a, long b, long c, long[] program)
    {
        public long A { get; set; } = a;
        public long B { get; set; } = b;
        public long C { get; set; } = c;
        public long IP { get; set; } = 0;
        public long[] Program { get; set; } = program;

        public Inst ReadInst() => IP >= Program.Length ? Inst.Halt : (Inst)Program[IP++];

        public long ReadLiteral()
        {
            if (IP >= Program.Length) throw new InvalidOperationException();
            return Program[IP++];
        }
        
        public long ReadCombo()
        {
            var val = ReadLiteral();

            return val switch
            {
                <= 3 => val,
                4 => A,
                5 => B,
                6 => C,
                _ => throw new InvalidOperationException()
            };
        }

        public void Reset(long a, long b = 0, long c = 0)
        {
            A = a;
            B = b;
            C = c;
            IP = 0;
        }
    }
    
    enum Inst { Halt = -1, Adv = 0, Bxl, Bst, Jnz, Bxc, Out, Bdv, Cdv }
}