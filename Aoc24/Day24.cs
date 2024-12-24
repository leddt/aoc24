namespace Aoc24;

public class Day24(ITestOutputHelper output)
{
    private const string Sample1 = """
                                  x00: 1
                                  x01: 0
                                  x02: 1
                                  x03: 1
                                  x04: 0
                                  y00: 1
                                  y01: 1
                                  y02: 1
                                  y03: 1
                                  y04: 1
                                  
                                  ntg XOR fgs -> mjb
                                  y02 OR x01 -> tnw
                                  kwq OR kpj -> z05
                                  x00 OR x03 -> fst
                                  tgd XOR rvg -> z01
                                  vdt OR tnw -> bfw
                                  bfw AND frj -> z10
                                  ffh OR nrd -> bqk
                                  y00 AND y03 -> djm
                                  y03 OR y00 -> psh
                                  bqk OR frj -> z08
                                  tnw OR fst -> frj
                                  gnj AND tgd -> z11
                                  bfw XOR mjb -> z00
                                  x03 OR x00 -> vdt
                                  gnj AND wpb -> z02
                                  x04 AND y00 -> kjc
                                  djm OR pbm -> qhw
                                  nrd AND vdt -> hwm
                                  kjc AND fst -> rvg
                                  y04 OR y02 -> fgs
                                  y01 AND x02 -> pbm
                                  ntg OR kjc -> kwq
                                  psh XOR fgs -> tgd
                                  qhw XOR tgd -> z09
                                  pbm OR djm -> kpj
                                  x03 XOR y03 -> ffh
                                  x00 XOR y04 -> ntg
                                  bfw OR bqk -> z06
                                  nrd XOR fgs -> wpb
                                  frj XOR qhw -> z04
                                  bqk OR frj -> z07
                                  y03 OR x01 -> nrd
                                  hwm AND bqk -> z03
                                  tgd XOR rvg -> z12
                                  tnw OR pbm -> gnj
                                  """;

    private const string Sample2 = """
                                   x00: 0
                                   x01: 1
                                   x02: 0
                                   x03: 1
                                   x04: 0
                                   x05: 1
                                   y00: 0
                                   y01: 0
                                   y02: 1
                                   y03: 1
                                   y04: 0
                                   y05: 1
                                   
                                   x00 AND y00 -> z05
                                   x01 AND y01 -> z02
                                   x02 AND y02 -> z01
                                   x03 AND y03 -> z03
                                   x04 AND y04 -> z04
                                   x05 AND y05 -> z00
                                   """;
    
    [Fact] public async Task TestPart1() => Assert.Equal(2024, await RunPart1(Sample1));
    // [Fact] public async Task TestPart2() => Assert.Equal("z00,z01,z02,z05", await RunPart2(Sample2, 2, (x, y, z) => (x & y) == z));
    
    [Fact]
    public async Task Solve()
    {
        var input = await File.ReadAllTextAsync("inputs/24.txt");
        
        output.WriteLine($"Part 1: {await RunPart1(input)}");
        // output.WriteLine($"Part 2: {await RunPart2(input, 4, (x, y, z) => x + y == z)}");
    }

    async ValueTask<long> RunPart1(string input)
    {
        var wires = await Parse(input);
        var bits = GetBits('z', wires);

        return BitsToValue(bits);
    }

    // async ValueTask<string> RunPart2(string input, int swapCount, Func<long, long, long, bool> testCase)
    // {
    //     var wires = await Parse(input);
    //     var gates = wires.Where(w => w.IsGate).ToArray();
    //
    //     return "";
    // }

    private IEnumerable<bool> GetBits(char prefix, IEnumerable<Wire> wires) => wires
        .Where(x => x.Name[0] == prefix)
        .OrderBy(x => x.Name)
        .Select(x => x.Value);

    private static long BitsToValue(IEnumerable<bool> bits)
    {
        var result = 0L;
        
        var v = 1L;
        foreach (var bit in bits)
        {
            if (bit) result += v;
            v *= 2;
        }

        return result;
    }

    private static async Task<Wire[]> Parse(string input)
    {
        var wires = new Dictionary<string, Wire>();

        foreach (var line in input.GetLines())
        {
            if (line.Contains(':'))
            {
                var parts = line.Split(':', StringSplitOptions.TrimEntries);
                GetWire(parts[0]).SetValue(parts[1] == "1");
            }

            if (line.Contains("->"))
            {
                var parts = line.Split("->", StringSplitOptions.TrimEntries);
                
                var eqParts = parts[0].Split(' ');
                var left = GetWire(eqParts[0]);
                var right = GetWire(eqParts[2]);
                var eq = GetEquationFunction(eqParts[1]);
                GetWire(parts[1]).SetGate(left, right, eq);
            }
        }

        await Task.WhenAll(wires.Values.Select(x => x.ResultTask));

        return wires.Values.ToArray();

        Wire GetWire(string name)
        {
            if (wires.TryGetValue(name, out var wire)) return wire;
            return wires[name] = new Wire(name);
        }

        Func<bool, bool, bool> GetEquationFunction(string op)
        {
            return op switch
            {
                "AND" => (l, r) => l && r,
                "OR" => (l, r) => l || r,
                "XOR" => (l, r) => l != r,
                _ => throw new ArgumentOutOfRangeException(nameof(op))
            };
        }
    }

    class Wire(string name)
    {
        public string Original { get; } = name;
        public string Name { get; set; } = name;
        
        public bool IsGate { get; private set; }
        public Wire? GateLeft { get; private set; }
        public Wire? GateRight { get; private set; }

        private readonly TaskCompletionSource _tcs = new();
        public Task ResultTask => _tcs.Task;

        public bool Value { get; private set; }

        public void SetValue(bool fixedValue)
        {
            Value = fixedValue;
            _tcs.SetResult();
        }

        public void SetGate(Wire left, Wire right, Func<bool, bool, bool> op)
        {
            IsGate = true;
            GateLeft = left;
            GateRight = right;

            Task.WhenAll(left.ResultTask, right.ResultTask)
                .ContinueWith(r =>
                {
                    Value = op(left.Value, right.Value);
                    _tcs.SetResult();
                });
        }

        public override string ToString() => Name == Original ? Name : $"{Name} ({Original})";
    }

    readonly record struct Pair(Wire A, Wire B);
}