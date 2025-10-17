using System;
using System.IO;
using StudentManagementApp;

class HeroManagerTest
{
    static void Main()
    {
        // Use a test file to avoid overwriting real data
        const string testFile = "superheroes_test.txt";
        if (File.Exists(testFile)) File.Delete(testFile);

        // Patch HeroManager to use test file
        var manager = new HeroManagerTestable(testFile);

        // 1. Add sample heroes
        manager.AddHero("All Might", 40, "One For All", 95);
        manager.AddHero("Shigaraki", 21, "All For One", 80);
        manager.AddHero("Bakugo", 16, "Explosion", 62);
        manager.AddHero("Mineta", 16, "Pop Off", 35);

        // 2. Save and reload
        manager.SaveAll();
        manager.LoadAll();

        // 3. Assert count
        if (manager.HeroCount != 4)
            Console.WriteLine($"Test failed: Expected 4 heroes, got {manager.HeroCount}");
        else
            Console.WriteLine("Test passed: Correct hero count.");

        // 4. Assert fields
        var hero = manager.GetHeroByIdForTest(1);
        if (hero == null || hero.Name != "All Might" || hero.Rank != Rank.S || hero.ThreatLevel != ThreatLevel.Critical)
            Console.WriteLine("Test failed: All Might fields incorrect.");
        else
            Console.WriteLine("Test passed: All Might fields correct.");

        hero = manager.GetHeroByIdForTest(4);
        if (hero == null || hero.Name != "Mineta" || hero.Rank != Rank.C || hero.ThreatLevel != ThreatLevel.Low)
            Console.WriteLine("Test failed: Mineta fields incorrect.");
        else
            Console.WriteLine("Test passed: Mineta fields correct.");

        // Clean up
        if (File.Exists(testFile)) File.Delete(testFile);
    }
}

// Minimal testable subclass to allow file override and access to internal data
class HeroManagerTestable : HeroManager
{
    private readonly string _fileName;
    public HeroManagerTestable(string fileName)
    {
        typeof(HeroManager).GetField("FileName", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(null, fileName);
        LoadAll();
    }

    public int HeroCount => typeof(HeroManager)
        .GetField("heroes", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
        ?.GetValue(this) is List<Hero> list ? list.Count : 0;

    public Hero GetHeroByIdForTest(int id)
    {
        var list = typeof(HeroManager)
            .GetField("heroes", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.GetValue(this) as List<Hero>;
        return list?.Find(h => h.HeroId == id);
    }
}