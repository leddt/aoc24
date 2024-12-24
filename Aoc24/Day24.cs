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
    
    [Fact] public void TestPart1() => Assert.Equal(2024, RunPart1(Sample1));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/24.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        // I solved part 2 manually by finding outliers in the gates. See input/24_scratchpad.txt
    }

    long RunPart1(string input)
    {
        var wires = Parse(input);
        var bits = GetBits('z', wires.ToDictionary(x => x.Name));

        return BitsToValue(bits);
    }

    private IEnumerable<bool> GetBits(char prefix, IDictionary<string, Wire> wires) => wires.Values
        .Where(x => x.Name[0] == prefix)
        .OrderBy(x => x.Name)
        .Select(x => x.GetValue(wires));

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

    private static IEnumerable<Wire> Parse(string input)
    {
        var wires = new List<Wire>();

        foreach (var line in input.GetLines())
        {
            if (line.Contains(':'))
            {
                var parts = line.Split(':', StringSplitOptions.TrimEntries);
                wires.Add(new Input(parts[0], parts[1] == "1"));
            }

            if (line.Contains("->"))
            {
                var parts = line.Split("->", StringSplitOptions.TrimEntries);
                var name = parts[1];
                var eqParts = parts[0].Split(' ');
                
                wires.Add(eqParts switch
                {
                    [var left, "AND", var right] => new AndGate(name, left, right),
                    [var left, "OR", var right] => new OrGate(name, left, right),
                    [var left, "XOR", var right] => new XorGate(name, left, right),
                    _ => throw new ArgumentOutOfRangeException()
                });
            }
        }

        return wires;
    }

    abstract record Wire(string Name)
    {
        public abstract bool GetValue(IDictionary<string, Wire> wires);
    }

    record Input(string Name, bool Value) : Wire(Name)
    {
        public override bool GetValue(IDictionary<string, Wire> wires) => Value;
    }

    abstract record Gate(string Name, string Left, string Right, Func<bool,bool,bool> Op) : Wire(Name)
    {
        public override bool GetValue(IDictionary<string, Wire> wires) =>
            Op(wires[Left].GetValue(wires), wires[Right].GetValue(wires));
    }

    record AndGate(string Name, string Left, string Right) : Gate(Name, Left, Right, (l, r) => l && r);
    record OrGate(string Name, string Left, string Right) : Gate(Name, Left, Right, (l, r) => l || r);
    record XorGate(string Name, string Left, string Right) : Gate(Name, Left, Right, (l, r) => l != r);
}