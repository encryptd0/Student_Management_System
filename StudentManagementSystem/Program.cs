using System;
using System.Collections.Generic;

namespace StudentManagementApp
{
    // Student data 
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Course { get; set; }

        public Student(int id, string fullName, int age, string course)
        {
            Id = id;
            FullName = fullName;
            Age = age;
            Course = course;
        }

        public override string ToString()
        {
            return $"ID: {Id} | Name: {FullName} | Age: {Age} | Course: {Course}";
        }
    }

    // Manager that handles the CRUD opperations 
    public class StudentManager
    {
        // Instantiate the .txt file
        private readonly string filePath = "students.txt";

        private List<Student> students = new();
        private int nextId = 1;

        // Save data to .txt file
        private void SaveAllToFile() 
        {
            try
            {
                var lines = students.Select(s => $"{s.Id}|{s.FullName}|{s.Age}|{s.Course}");
                File.WriteAllLines(filePath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        // Load data from .txt file
        public void LoadAllFromFile() 
        {
            try
            {
                if (!File.Exists(filePath))
                    return; // No file yet, nothing to load

                var lines = File.ReadAllLines(filePath);
                students = lines
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line =>
                    {
                        var parts = line.Split('|');
                        return new Student(
                            int.Parse(parts[0]),
                            parts[1],
                            int.Parse(parts[2]),
                            parts[3]
                        );
                    })
                    .ToList();

                // Update nextId so it continues from the last student
                if (students.Count > 0)
                    nextId = students.Max(s => s.Id) + 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }

        // Create
        public void AddStudent(string name, int age, string course)
        {
            var student = new Student(nextId++, name, age, course);
            students.Add(student); // Save student to in-memory list
            SaveAllToFile(); // Persist the list to the text file
            Console.WriteLine($"Student {name} added successfully.\n");
        }

        // Read
        public void ViewAllStudents()
        {
            if (students.Count == 0)
            {
                Console.WriteLine("No students found.\n");
                return;
            }

            Console.WriteLine("Student List: ");
            foreach (var s in students)
                Console.WriteLine(s);
            Console.WriteLine();
        }

        // Read (Single)
        public void GetStudentById(int id)
        {
            var student = students.Find(s => s.Id == id);
            if (student == null)
                Console.WriteLine("Student not found.\n");
            else
                Console.WriteLine(student + "\n");
        }

        // Update
        public void UpdateStudent(int id, string newName, int newAge, string newCourse)
        {
            
            var student = students.Find(s => s.Id == id);
            if (student == null)
            {
                Console.WriteLine("Student not found.\n");
                return;
            }
            
            student.FullName = newName;
            student.Age = newAge;
            student.Course = newCourse;
            SaveAllToFile(); // Persist changes to the .txt file
            Console.WriteLine("Student updated successfully.\n");
        }

        // Delete
        public void RemoveStudent(int id)
        {
            var student = students.Find(s => s.Id == id);
            if (student == null)
            {
                Console.WriteLine("Student not found.\n");
                return;
            }

            students.Remove(student);
            SaveAllToFile(); // Persist changes to the .txt file
            Console.WriteLine("Student removed successfully.\n");
        }
    }

    // Main program
    class Program
    {
        static void Main()
        {
            var manager = new StudentManager();
            manager.LoadAllFromFile(); // Load any previously saved students
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("===== Student Management System =====");
                Console.WriteLine("1️⃣ Add Student");
                Console.WriteLine("2️⃣ View All Students");
                Console.WriteLine("3️⃣ Search Student by ID");
                Console.WriteLine("4️⃣ Update Student");
                Console.WriteLine("5️⃣ Remove Student");
                Console.WriteLine("0️⃣ Exit");
                Console.Write("Select an option: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter name: ");
                        string name = Console.ReadLine();
                        Console.Write("Enter age: ");
                        int age = int.Parse(Console.ReadLine());
                        Console.Write("Enter course: ");
                        string course = Console.ReadLine();
                        manager.AddStudent(name, age, course);
                        break;

                    case "2":
                        manager.ViewAllStudents();
                        break;

                    case "3":
                        Console.Write("Enter student ID: ");
                        int idToView = int.Parse(Console.ReadLine());
                        manager.GetStudentById(idToView);
                        break;

                    case "4":
                        Console.Write("Enter student ID: ");
                        int idToUpdate = int.Parse(Console.ReadLine());
                        Console.Write("Enter new name: ");
                        string newName = Console.ReadLine();
                        Console.Write("Enter new age: ");
                        int newAge = int.Parse(Console.ReadLine());
                        Console.Write("Enter new course: ");
                        string newCourse = Console.ReadLine();
                        manager.UpdateStudent(idToUpdate, newName, newAge, newCourse);
                        break;

                    case "5":
                        Console.Write("Enter student ID: ");
                        int idToDelete = int.Parse(Console.ReadLine());
                        manager.RemoveStudent(idToDelete);
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
