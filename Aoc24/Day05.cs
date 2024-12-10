namespace Aoc24;

public class Day05(ITestOutputHelper output)
{
    private const string Sample = """
                                  47|53
                                  97|13
                                  97|61
                                  97|47
                                  75|29
                                  61|13
                                  75|53
                                  29|13
                                  97|29
                                  53|29
                                  61|53
                                  97|53
                                  61|29
                                  47|13
                                  75|47
                                  97|75
                                  47|61
                                  75|61
                                  47|29
                                  75|13
                                  53|13
                                  
                                  75,47,61,53,29
                                  97,61,53,29,13
                                  75,29,13
                                  75,97,47,61,53
                                  61,13,29
                                  97,13,75,29,47
                                  """;
    
    [Fact] public void TestPart1() => Assert.Equal(143, RunPart1(Sample));
    [Fact] public void TestPart2() => Assert.Equal(123, RunPart2(Sample));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/5.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
        output.WriteLine($"Part 2: {RunPart2(input)}");
    }

    int RunPart1(string input)
    {
        var (rules, updates) = Parse(input);

        return updates.Where(x => IsValidUpdate(x, rules)).Sum(x => x.GetMiddlePage());
    }

    int RunPart2(string input)
    {
        var (rules, updates) = Parse(input);
        var comparer = new PageComparer(rules);

        return updates
            .Where(x => !IsValidUpdate(x, rules))
            .Select(x => GetFixedUpdate(x, comparer))
            .Sum(x => x.GetMiddlePage());
    }

    (List<Rule>, List<Update>) Parse(string input)
    {
        var rules = new List<Rule>();
        var updates = new List<Update>();

        foreach (var line in input.GetLines())
        {
            if (line == "") continue;
    
            if (line.Contains('|') && line.Split('|').Select(int.Parse).ToArray() is [var a, var b])
            {
                rules.Add(new Rule(a, b));
            }
            else if (line.Contains(',') && line.Split(',').Select(int.Parse).ToArray() is {} pages)
            {
                updates.Add(new Update(pages));
            }
        }

        return (rules, updates);
    }

    bool IsValidUpdate(Update update, IEnumerable<Rule> rules) =>
        !rules.Any(rule => UpdateViolatesRule(update, rule));

    bool UpdateViolatesRule(Update update, Rule rule)
    {
        var beforeIndex = Array.IndexOf(update.Pages, rule.Before);
        var afterIndex = Array.IndexOf(update.Pages, rule.After);
        if (beforeIndex == -1 || afterIndex == -1) return false;
    
        return beforeIndex > afterIndex;
    }

    Update GetFixedUpdate(Update update, PageComparer comparer)
    {
        var sortedPages = update.Pages.Order(comparer).ToArray();
        return new Update(sortedPages);
    }

    record Rule(int Before, int After)
    {
        public bool AppliesTo(int x, int y) => (Before == x || Before == y) &&
                                               (After == x || After == y);
    }

    record Update(int[] Pages)
    {
        public int GetMiddlePage() => Pages[Pages.Length / 2];
    }

    class PageComparer(IEnumerable<Rule> rules) : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            var rule = rules.FirstOrDefault(r => r.AppliesTo(x, y));

            if (rule == null) return 0;
            return rule.Before == x ? -1 : 1;
        }
    }
}