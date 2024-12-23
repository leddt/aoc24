namespace Aoc24;
using Connection = (string A, string B);

public class Day23(ITestOutputHelper output)
{
    private const string Sample = """
                                  kh-tc
                                  qp-kh
                                  de-cg
                                  ka-co
                                  yn-aq
                                  qp-ub
                                  cg-tb
                                  vc-aq
                                  tb-ka
                                  wh-tc
                                  yn-cg
                                  kh-ub
                                  ta-co
                                  de-co
                                  tc-td
                                  tb-wq
                                  wh-td
                                  ta-ka
                                  td-qp
                                  aq-cg
                                  wq-ub
                                  ub-vc
                                  de-ta
                                  wq-aq
                                  wq-vc
                                  wh-yn
                                  ka-de
                                  kh-ta
                                  co-tc
                                  wh-qp
                                  tb-vc
                                  td-yn
                                  """;
    
    [Fact] public void TestPart1() => Assert.Equal(7, RunPart1(Sample));
    [Fact] public void TestPart2() => Assert.Equal("co,de,ka,ta", RunPart2(Sample));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/23.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    int RunPart1(string input)
    {
        var connections = Parse(input).ToArray();
        var allComputers = connections.Select(c => c.A).Distinct().ToArray();
        var lookup = connections.ToLookup(x => x.A, x => x.B);

        var sets =
            from c1 in allComputers
            from c2 in lookup[c1]
            from c3 in lookup[c2]
            where c1[0] == 't' || c2[0] == 't' || c3[0] == 't'
            where lookup[c3].Contains(c1)
            select CreateSet(c1, c2, c3);

        return sets.Distinct().Count();

        (string, string, string) CreateSet(string a, string b, string c)
        {
            var tmp = new[] { a, b, c };
            Array.Sort(tmp);
            return (tmp[0], tmp[1], tmp[2]);
        }
    }

    string RunPart2(string input)
    {
        var connections = Parse(input).ToArray();
        var allComputers = connections.Select(c => c.A).Distinct().Order().ToArray();
        var lookup = connections.ToLookup(x => x.A, x => x.B);
        
        string[] largest = [];
        TryAllSets();
        
        return string.Join(',', largest);
        
        void TryAllSets(int startIndex = 0, string[]? current = null)
        {
            for (var i = startIndex; i < allComputers.Length; i++)
            {
                // Is the set still valid if we add this computer?
                string[] candidate = [..current ?? [], allComputers[i]];
                if (!IsValidSet(candidate)) continue;
                
                // Keep track of best set yet
                if (candidate.Length > largest.Length) 
                    largest = candidate;
                    
                // Try with more computers
                TryAllSets(i + 1, candidate);
            }
        }

        bool IsValidSet(string[] candidate)
        {
            for (var i = 0; i < candidate.Length; i++)
            for (var j = i + 1; j < candidate.Length; j++)
            {
                if (!lookup[candidate[i]].Contains(candidate[j]))
                    return false;
            }

            return true;
        }
    }

    IEnumerable<Connection> Parse(string input)
    {
        foreach (var line in input.GetLines())
        {
            var parts = line.Split('-');
            yield return (parts[0], parts[1]);
            yield return (parts[1], parts[0]);
        }
    }
}