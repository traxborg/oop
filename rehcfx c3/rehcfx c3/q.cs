using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ElectronicDeanery
{
   
    public class Student
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{LastName} {FirstName}";
    }

    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Guid> StudentIds { get; set; } = new List<Guid>();
    }

    public class Room
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public int Capacity { get; set; }
        public List<Guid> Occupants { get; set; } = new List<Guid>();
    }

    public class Dormitory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public List<Room> Rooms { get; set; } = new List<Room>();
    }

   
    public class DllNode<T>
    {
        public T Data { get; set; }
        public DllNode<T> Prev { get; set; }
        public DllNode<T> Next { get; set; }
        public DllNode(T data) { Data = data; }
    }

    public class DoublyLinkedList<T> : IEnumerable<T>
    {
        private DllNode<T> head;
        private DllNode<T> tail;
        public int Count { get; private set; }

        public void AddLast(T item)
        {
            var node = new DllNode<T>(item);
            if (tail == null) head = tail = node;
            else { tail.Next = node; node.Prev = tail; tail = node; }
            Count++;
        }

        public bool Remove(Predicate<T> match)
        {
            var cur = head;
            while (cur != null)
            {
                if (match(cur.Data))
                {
                    if (cur.Prev != null) cur.Prev.Next = cur.Next; else head = cur.Next;
                    if (cur.Next != null) cur.Next.Prev = cur.Prev; else tail = cur.Prev;
                    Count--;
                    return true;
                }
                cur = cur.Next;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var cur = head;
            while (cur != null) { yield return cur.Data; cur = cur.Next; }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        public void Clear() { head = tail = null; Count = 0; }
    }

   
    public class DeaneryService
    {
        private DoublyLinkedList<Student> students = new DoublyLinkedList<Student>();
        private Dictionary<Guid, Group> groups = new Dictionary<Guid, Group>();
        private Dictionary<Guid, Dormitory> dorms = new Dictionary<Guid, Dormitory>();

        
        public Student CreateStudent(string firstName, string lastName)
        {
            var s = new Student { Id = Guid.NewGuid(), FirstName = firstName, LastName = lastName };
            students.AddLast(s);
            return s;
        }

        public bool DeleteStudent(Guid studentId) => students.Remove(st => st.Id == studentId);
        public IEnumerable<Student> GetStudents() => students;

        
        public Group CreateGroup(string name)
        {
            var g = new Group { Id = Guid.NewGuid(), Name = name };
            groups[g.Id] = g;
            return g;
        }

        public IEnumerable<Group> GetGroups() => groups.Values;

      
        public Dormitory CreateDorm(string name, string address)
        {
            var d = new Dormitory { Id = Guid.NewGuid(), Name = name, Address = address };
            dorms[d.Id] = d;
            return d;
        }

        public IEnumerable<Dormitory> GetDorms() => dorms.Values;

        
        public IEnumerable<Student> SearchStudentsByName(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) yield break;
            query = query.ToLowerInvariant();
            foreach (var s in students)
            {
                if ((s.FirstName?.ToLowerInvariant().Contains(query) ?? false) ||
                    (s.LastName?.ToLowerInvariant().Contains(query) ?? false)) yield return s;
            }
        }

       
        public void Save(string folder)
        {
            Directory.CreateDirectory(folder);
            using (var w = new StreamWriter(Path.Combine(folder, "students.db")))
            {
                foreach (var s in students)
                {
                    w.WriteLine($"{s.Id}|{s.FirstName}|{s.LastName}");
                }
            }
        }

        public void Load(string folder)
        {
            students.Clear();
            string sf = Path.Combine(folder, "students.db");
            if (!File.Exists(sf)) return;
            using (var reader = new StreamReader(sf))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var p = line.Split('|');
                    students.AddLast(new Student { Id = Guid.Parse(p[0]), FirstName = p[1], LastName = p[2] });
                }
            }
        }
    }

    
    class Program
    {
       
        const string DATA_FOLDER = "data";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var svc = new DeaneryService();
            svc.Load(DATA_FOLDER);
            while (true)
            {
                Console.WriteLine("\n=== Електронний деканат ===");
                Console.WriteLine("1. Студенти");
                Console.WriteLine("2. Групи");
                Console.WriteLine("3. Гуртожитки");
                Console.WriteLine("4. Пошук студентів");
                Console.WriteLine("5. Зберегти");
                Console.WriteLine("6. Вийти");
                Console.Write("> ");
                var ch = Console.ReadLine();
                switch (ch)
                {
                    case "1": StudentsMenu(svc); break;
                    case "2": GroupsMenu(svc); break;
                    case "3": DormMenu(svc); break;
                    case "4": SearchMenu(svc); break;
                    case "5": svc.Save(DATA_FOLDER); Console.WriteLine("Дані збережено."); break;
                    case "6": svc.Save(DATA_FOLDER); return;
                    default: Console.WriteLine("Невірна опція"); break;
                }
            }
        }

        static void StudentsMenu(DeaneryService svc)
        {
            Console.WriteLine("\n--- Студенти ---");
            Console.WriteLine("1. Додати студента");
            Console.WriteLine("2. Видалити студента");
            Console.WriteLine("3. Переглянути всіх студентів");
            Console.Write("> ");
            var ch = Console.ReadLine();
            switch (ch)
            {
                case "1":
                    Console.Write("Ім'я: "); string fn = Console.ReadLine();
                    Console.Write("Прізвище: "); string ln = Console.ReadLine();
                    var s = svc.CreateStudent(fn, ln);
                    Console.WriteLine("Студент доданий. ID: " + s.Id);
                    break;
                case "2":
                    Console.Write("ID студента: ");
                    if (Guid.TryParse(Console.ReadLine(), out Guid id))
                    {
                        if (svc.DeleteStudent(id)) Console.WriteLine("Студент видалений");
                        else Console.WriteLine("Студента не знайдено");
                    }
                    break;
                case "3":
                    foreach (var st in svc.GetStudents()) Console.WriteLine($"{st.Id} | {st.FullName}");
                    break;
            }
        }

        static void GroupsMenu(DeaneryService svc)
        {
            Console.WriteLine("\n--- Групи ---");
            Console.WriteLine("1. Додати групу");
            Console.WriteLine("2. Переглянути всі групи");
            Console.Write("> ");
            var ch = Console.ReadLine();
            switch (ch)
            {
                case "1":
                    Console.Write("Назва групи: "); string name = Console.ReadLine();
                    var g = svc.CreateGroup(name);
                    Console.WriteLine("Групу додано. ID: " + g.Id);
                    break;
                case "2":
                    foreach (var gr in svc.GetGroups()) Console.WriteLine($"{gr.Id} | {gr.Name}");
                    break;
            }
        }

        static void DormMenu(DeaneryService svc)
        {
            Console.WriteLine("\n--- Гуртожитки ---");
            Console.WriteLine("1. Додати гуртожиток");
            Console.WriteLine("2. Переглянути гуртожитки");
            Console.Write("> ");
            var ch = Console.ReadLine();
            switch (ch)
            {
                case "1":
                    Console.Write("Назва: "); string name = Console.ReadLine();
                    Console.Write("Адреса: "); string addr = Console.ReadLine();
                    var d = svc.CreateDorm(name, addr);
                    Console.WriteLine("Гуртожиток додано. ID: " + d.Id);
                    break;
                case "2":
                    foreach (var dorm in svc.GetDorms()) Console.WriteLine($"{dorm.Id} | {dorm.Name} | {dorm.Address}");
                    break;
            }
        }

        static void SearchMenu(DeaneryService svc)
        {
            Console.WriteLine("\n--- Пошук студентів ---");
            Console.WriteLine("1. За ім'ям або прізвищем");
            Console.Write("> ");
            var ch = Console.ReadLine();
            switch (ch)
            {
                case "1":
                    Console.Write("Пошуковий запит: "); string q = Console.ReadLine();
                    foreach (var st in svc.SearchStudentsByName(q)) Console.WriteLine($"{st.Id} | {st.FullName}");
                    break;
            }
        }
    }
}