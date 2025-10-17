using System;
using System.Collections.Generic;
using System.IO;

namespace StudentManagementApp
{
    // Ranks for heroes
    public enum Rank
    {
        C = 1,
        B,
        A,
        S
    }

    // Threat levels
    public enum ThreatLevel
    {
        Low = 1,
        Medium,
        High,
        Critical
    }

    // Hero data (converted from Student)
    public class Hero
    {
        public int HeroId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Quirk { get; set; }
        public double ExamScore { get; set; }
        public Rank Rank { get; set; }
        public ThreatLevel ThreatLevel { get; set; }

        public Hero(int heroId, string name, int age, string quirk, double examScore, Rank rank, ThreatLevel threatLevel)
        {
            HeroId = heroId;
            Name = name;
            Age = age;
            Quirk = quirk;
            ExamScore = examScore;
            Rank = rank;
            ThreatLevel = threatLevel;
        }

        public override string ToString()
        {
            return $"HeroID: {HeroId} | Name: {Name} | Age: {Age} | Quirk: {Quirk} | ExamScore: {ExamScore} | Rank: {Rank} | ThreatLevel: {ThreatLevel}";
        }
    }

    // Manager that handles the CRUD operations for heroes
    public class HeroManager
    {
        private List<Hero> heroes = new();
        private int nextId = 1;
        private const string FileName = "superheroes.txt";

        public HeroManager()
        {
            LoadAll();
            if (heroes.Count > 0)
                nextId = heroes[^1].HeroId + 1;
        }

        // Load all heroes from file
        public void LoadAll()
        {
            heroes.Clear();
            if (!File.Exists(FileName))
                return;

            foreach (var line in File.ReadAllLines(FileName))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("HeroId")) continue; // skip header
                var parts = line.Split(',');
                if (parts.Length != 7) continue;
                int heroId = int.Parse(parts[0]);
                string name = parts[1];
                int age = int.Parse(parts[2]);
                string quirk = parts[3];
                double examScore = double.Parse(parts[4]);
                Rank rank = Enum.Parse<Rank>(parts[5]);
                ThreatLevel threat = Enum.Parse<ThreatLevel>(parts[6]);
                heroes.Add(new Hero(heroId, name, age, quirk, examScore, rank, threat));
            }
        }

        // Save all heroes to file
        public void SaveAll()
        {
            using var writer = new StreamWriter(FileName, false);
            writer.WriteLine("HeroId,Name,Age,Quirk,ExamScore,Rank,ThreatLevel");
            foreach (var h in heroes)
            {
                writer.WriteLine($"{h.HeroId},{h.Name},{h.Age},{h.Quirk},{h.ExamScore},{h.Rank},{h.ThreatLevel}");
            }
        }

        // Calculate Rank and ThreatLevel based on score
        public (Rank, ThreatLevel) CalculateRankAndThreat(double score)
        {
            if (score >= 81) return (Rank.S, ThreatLevel.Critical);
            if (score >= 61) return (Rank.A, ThreatLevel.High);
            if (score >= 41) return (Rank.B, ThreatLevel.Medium);
            return (Rank.C, ThreatLevel.Low);
        }

        // Create
        public void AddHero(string name, int age, string quirk, double examScore)
        {
            var (rank, threat) = CalculateRankAndThreat(examScore);
            var hero = new Hero(nextId++, name, age, quirk, examScore, rank, threat);
            heroes.Add(hero);
            SaveAll();
            Console.WriteLine("Hero added successfully.\n");
        }

        // Read
        public void ViewAllHeroes()
        {
            if (heroes.Count == 0)
            {
                Console.WriteLine("No heroes found.\n");
                return;
            }

            Console.WriteLine("Hero List: ");
            foreach (var h in heroes)
                Console.WriteLine(h);
            Console.WriteLine();
        }

        // Read (Single)
        public void GetHeroById(int id)
        {
            var hero = heroes.Find(h => h.HeroId == id);
            if (hero == null)
                Console.WriteLine("Hero not found.\n");
            else
                Console.WriteLine(hero + "\n");
        }

        // Update
        public void UpdateHero(int id, string newName, int newAge, string newQuirk, double newExamScore)
        {
            var hero = heroes.Find(h => h.HeroId == id);
            if (hero == null)
            {
                Console.WriteLine("Hero not found.\n");
                return;
            }

            var (newRank, newThreat) = CalculateRankAndThreat(newExamScore);
            hero.Name = newName;
            hero.Age = newAge;
            hero.Quirk = newQuirk;
            hero.ExamScore = newExamScore;
            hero.Rank = newRank;
            hero.ThreatLevel = newThreat;
            SaveAll();
            Console.WriteLine("Hero updated successfully.\n");
        }

        // Delete
        public void RemoveHero(int id)
        {
            var hero = heroes.Find(h => h.HeroId == id);
            if (hero == null)
            {
                Console.WriteLine("Hero not found.\n");
                return;
            }

            heroes.Remove(hero);
            SaveAll();
            Console.WriteLine("Hero removed successfully.\n");
        }
    }

    // Main program
    class Program
    {
        static void Main()
        {
            var manager = new HeroManager();
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("===== Hero Management System =====");
                Console.WriteLine("1️.) To Add Hero");
                Console.WriteLine("2️.) To View All Heroes");
                Console.WriteLine("3️.) To Search Hero by ID");
                Console.WriteLine("4️.) To Update Hero");
                Console.WriteLine("5️.) To Remove Hero");
                Console.WriteLine("0️.) To Exit");
                Console.Write("Please select an option: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter name: ");
                        string name = Console.ReadLine();

                        Console.Write("Enter age: ");
                        if (!int.TryParse(Console.ReadLine(), out int age))
                        {
                            Console.WriteLine("Invalid age.\n");
                            break;
                        }

                        Console.Write("Enter quirk: ");
                        string quirk = Console.ReadLine();

                        Console.Write("Enter exam score: ");
                        if (!double.TryParse(Console.ReadLine(), out double examScore))
                        {
                            Console.WriteLine("Invalid exam score.\n");
                            break;
                        }

                        manager.AddHero(name, age, quirk, examScore);
                        break;

                    case "2":
                        manager.ViewAllHeroes();
                        break;

                    case "3":
                        Console.Write("Enter hero ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int idToView))
                        {
                            Console.WriteLine("Invalid ID.\n");
                            break;
                        }
                        manager.GetHeroById(idToView);
                        break;

                    case "4":
                        Console.Write("Enter hero ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int idToUpdate))
                        {
                            Console.WriteLine("Invalid ID.\n");
                            break;
                        }

                        Console.Write("Enter new name: ");
                        string newName = Console.ReadLine();

                        Console.Write("Enter new age: ");
                        if (!int.TryParse(Console.ReadLine(), out int newAge))
                        {
                            Console.WriteLine("Invalid age.\n");
                            break;
                        }

                        Console.Write("Enter new quirk: ");
                        string newQuirk = Console.ReadLine();

                        Console.Write("Enter new exam score: ");
                        if (!double.TryParse(Console.ReadLine(), out double newExamScore))
                        {
                            Console.WriteLine("Invalid exam score.\n");
                            break;
                        }

                        manager.UpdateHero(idToUpdate, newName, newAge, newQuirk, newExamScore);
                        break;

                    case "5":
                        Console.Write("Enter hero ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int idToDelete))
                        {
                            Console.WriteLine("Invalid ID.\n");
                            break;
                        }
                        manager.RemoveHero(idToDelete);
                        break;

                    case "0":
                        exit = true;
                        Console.WriteLine("👋 Exiting program...");
                        break;

                    default:
                        Console.WriteLine("⚠️ Invalid option. Try again.\n");
                        break;
                }
            }
        }
    }
}
