using System;
using System.Collections.Generic;
using System.Text;

class Program
{
    // Класс для хранения статистики текста
    class TextStats
    {
        public int WordCount { get; set; }
        public string ShortestWord { get; set; }
        public int SentenceCount { get; set; }
        public int VowelsCount { get; set; }
        public int ConsonantsCount { get; set; }
        public string LongestWord { get; set; }
        public Dictionary<char, int> LetterFrequency { get; set; }
    }

    static List<TextStats> allStats = new List<TextStats>();

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("1. Ввести новый текст\n2. Показать статистику прошлых текстов\n3. Выход");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ProcessNewText();
                    break;
                case "2":
                    ShowPreviousStats();
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Неверный ввод");
                    break;
            }
        }
    }

    static void ProcessNewText()
    {
        string text;
        do
        {
            Console.WriteLine("Введите текст (минимум 100 символов):");
            text = Console.ReadLine();
        } while (text.Length < 100);

        TextStats stats = new TextStats();
        
        // Подсчет слов и поиск самого короткого/длинного слова
        string[] words = SplitTextIntoWords(text);
        stats.WordCount = words.Length;
        if (words.Length > 0)
        {
            stats.ShortestWord = words[0];
            stats.LongestWord = words[0];
            foreach (string word in words)
            {
                if (word.Length < stats.ShortestWord.Length)
                    stats.ShortestWord = word;
                if (word.Length > stats.LongestWord.Length)
                    stats.LongestWord = word;
            }
        }

        // Подсчет предложений
        stats.SentenceCount = CountSentences(text);

        // Подсчет гласных/согласных и частоты букв
        CountLettersAndFrequency(text, stats);

        allStats.Add(stats);
        DisplayCurrentStats(stats);
    }

    // Разделение текста на слова с обработкой знаков препинания
    static string[] SplitTextIntoWords(string text)
    {
        List<string> words = new List<string>();
        StringBuilder currentWord = new StringBuilder();

        foreach (char c in text)
        {
            if (char.IsLetter(c) || c == '\'')
            {
                currentWord.Append(c);
            }
            else
            {
                if (currentWord.Length > 0)
                {
                    words.Add(currentWord.ToString());
                    currentWord.Clear();
                }
            }
        }
        
        // Добавляем последнее слово
        if (currentWord.Length > 0)
            words.Add(currentWord.ToString());

        return words.ToArray();
    }

    // Подсчет предложений по разделителям
    static int CountSentences(string text)
    {
        int count = 0;
        char[] separators = { '.', '!', '?' };
        
        for (int i = 0; i < text.Length; i++)
        {
            if (Array.IndexOf(separators, text[i]) != -1)
            {
                // Проверяем, что это конец предложения, а не многоточие или др.
                if (i + 1 >= text.Length || text[i + 1] != text[i])
                    count++;
            }
        }
        return count;
    }

    // Анализ букв текста
    static void CountLettersAndFrequency(string text, TextStats stats)
    {
        stats.VowelsCount = 0;
        stats.ConsonantsCount = 0;
        stats.LetterFrequency = new Dictionary<char, int>();
        string vowels = "аеёиоуыэюяaeiou";

        foreach (char c in text.ToLower())
        {
            if (char.IsLetter(c))
            {
                // Обновляем статистику частоты
                if (!stats.LetterFrequency.ContainsKey(c))
                    stats.LetterFrequency[c] = 0;
                stats.LetterFrequency[c]++;

                // Считаем гласные/согласные
                if (vowels.Contains(c))
                    stats.VowelsCount++;
                else
                    stats.ConsonantsCount++;
            }
        }
    }

    static void DisplayCurrentStats(TextStats stats)
    {
        Console.WriteLine("\nТекущая статистика:");
        Console.WriteLine($"Количество слов: {stats.WordCount}");
        Console.WriteLine($"Самое короткое слово: {stats.ShortestWord}");
        Console.WriteLine($"Количество предложений: {stats.SentenceCount}");
        Console.WriteLine($"Гласные буквы: {stats.VowelsCount}");
        Console.WriteLine($"Согласные буквы: {stats.ConsonantsCount}");
        Console.WriteLine($"Самое длинное слово: {stats.LongestWord}");
        
        Console.WriteLine("Частота букв:");
        foreach (var pair in stats.LetterFrequency)
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}");
        }
        Console.WriteLine();
    }

    static void ShowPreviousStats()
    {
        if (allStats.Count == 0)
        {
            Console.WriteLine("Статистика предыдущих текстов отсутствует.");
            return;
        }

        for (int i = 0; i < allStats.Count; i++)
        {
            Console.WriteLine($"\n--- Текст #{i + 1} ---");
            DisplayCurrentStats(allStats[i]);
        }
    }
}