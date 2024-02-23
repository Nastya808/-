using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



class Program
{
    static object lockObject = new object(); 

    static void Main(string[] args)
    {
        // 1
        Thread firstThread = new Thread(() =>
        {
            string filePath = args[0];
            int sentenceCount = CountSentences(filePath);
            Console.WriteLine($"Количество предложений в файле: {sentenceCount}");
        });
        firstThread.Start();

        Thread secondThread = new Thread(() =>
        {
            firstThread.Join(); 
            ModifyFile(args[0]);
            Console.WriteLine("Модификация файла завершена");
        });
        secondThread.Start();

        // 2
        int[] data = { 5, 2, 7, 4, 1 };
        int searchNumber = 7;

        Task firstTask = Task.Run(() =>
        {
            SortArray(data);
            Console.WriteLine("Массив отсортирован");
        });

        Task secondTask = firstTask.ContinueWith((task) =>
        {
            lock (lockObject) 
            {
                bool found = Array.BinarySearch(data, searchNumber) >= 0;
                Console.WriteLine($"Число {searchNumber} {(found ? "найдено" : "не найдено")} в массиве");
            }
        });

        // 3
        Task.Run(() => DecrementNumbers());

        // 4
        Task task1 = DecrementNumbersAsync();
        Task task2 = DecrementNumbersAsync();
        Task task3 = DecrementNumbersAsync();
        Task.WhenAll(task1, task2, task3).Wait(); 
    }

    static int CountSentences(string filePath)
    {
        int count = 0;
        lock (lockObject) 
        {
            string text = File.ReadAllText(filePath);
            count = text.Split('.', '!', '?').Length - 1;
        }
        return count;
    }

    static void ModifyFile(string filePath)
    {
        lock (lockObject) 
        {
            string text = File.ReadAllText(filePath);
            text = text.Replace('!', '#');
            File.WriteAllText(filePath, text);
        }
    }

    static void SortArray(int[] array)
    {
        lock (lockObject) 
        {
            Array.Sort(array);
        }
    }

    static void DecrementNumbers()
    {
        List<int> numbers = new List<int> { 10, 20, 30, 40, 50 };
        lock (lockObject) 
        {
            foreach (var num in numbers)
            {
                Console.WriteLine(num - 1);
            }
        }
    }

    static async Task DecrementNumbersAsync()
    {
        List<int> numbers = new List<int> { 100, 200, 300, 400, 500 };
        await Task.Run(() =>
        {
            lock (lockObject) 
            {
                foreach (var num in numbers)
                {
                    Console.WriteLine(num - 5);
                }
            }
        });
    }
}
