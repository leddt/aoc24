namespace Aoc24;

public class Day25(ITestOutputHelper output)
{
    private const string Sample = """
                                  #####
                                  .####
                                  .####
                                  .####
                                  .#.#.
                                  .#...
                                  .....
                                  
                                  #####
                                  ##.##
                                  .#.##
                                  ...##
                                  ...#.
                                  ...#.
                                  .....
                                  
                                  .....
                                  #....
                                  #....
                                  #...#
                                  #.#.#
                                  #.###
                                  #####
                                  
                                  .....
                                  .....
                                  #.#..
                                  ###..
                                  ###.#
                                  ###.#
                                  #####
                                  
                                  .....
                                  .....
                                  .....
                                  #....
                                  #.#..
                                  #.#.#
                                  #####
                                  """;
    
    [Fact] public void TestPart1() => Assert.Equal(3, RunPart1(Sample));
    
    [Fact]
    public void Solve()
    {
        var input = File.ReadAllText("inputs/25.txt");
        
        output.WriteLine($"Part 1: {RunPart1(input)}");
    }

    int RunPart1(string input)
    {
        var all = Parse(input).ToArray();
        var locks = all.OfType<Lock>();
        var keys = all.OfType<Key>().ToArray();

        return locks.Sum(l => keys.Count(k => !k.Overlaps(l)));
    }

    IEnumerable<Pinned> Parse(string input)
    {
        var lines = input.GetLines();
        for (var i = 0; i < lines.Length; i += 8)
        {
            var grid = Grid.Parse(lines[i..(i + 7)]);
            if (grid[0, 0] == '#')
                yield return ParseLock(grid);
            else
                yield return ParseKey(grid);
        }

        Lock ParseLock(Grid grid)
        {
            var pins = Enumerable.Range(0, grid.Width)
                .Select(p => grid.Col(p).Count(x => x == '#') - 1)
                .ToArray();
            return new Lock(pins);
        }

        Key ParseKey(Grid grid)
        {
            var pins = Enumerable.Range(0, grid.Width)
                .Select(p => grid.Col(p).Count(x => x == '#') - 1)
                .ToArray();
            return new Key(pins);
        }
    }

    abstract record Pinned(int[] Pins);
    record Lock(int[] Pins) : Pinned(Pins);

    record Key(int[] Pins) : Pinned(Pins)
    {
        public bool Overlaps(Lock l)
        {
            return Pins.Where((p, i) => l.Pins[i] + p > 5).Any();
        }
    }
}