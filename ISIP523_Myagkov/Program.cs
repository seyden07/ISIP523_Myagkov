using System;
using System.Collections.Generic;
using System.Linq;

class Program
{

    enum Genre
    {
        Fantasy = 1,
    ScienceFiction,
    Mystery,
    Romance,
        Horror,
    Biography,
    History
    }

    class Book
    {
        private static int nextId = 1;

        public int Id { get; private set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public Genre Genre { get; set; }
    public int Year { get; set; }
    public decimal Price { get; set; }

        public Book()
        {
            Id = nextId++;
			}

        public override string ToString()
{
            return $"ID: {Id}, Название: \"{Title}\", Автор: {Author}, Жанр: {Genre}, Год: {Year}, Цена: {Price:C}";
        }
    }

    class Library
    {
        private List<Book> books = new List<Book>();


        public void AddTestData()
        {
            books.AddRange(new[]
            {
                new Book { Title = "Властелин Колец", Author = "Джон Р. Р. Толкин", Genre = Genre.Fantasy, Year = 1954, Price = 850 },
                new Book { Title = "Преступление и наказание", Author = "Федор Достоевский", Genre = Genre.Romance, Year = 1866, Price = 300 },
                new Book { Title = "Солярис", Author = "Станислав Лем", Genre = Genre.ScienceFiction, Year = 1961, Price = 420 },
                new Book { Title = "Дракула", Author = "Брэм Стокер", Genre = Genre.Horror, Year = 1897, Price = 390 },
                new Book { Title = "Стив Джобс", Author = "Уолтер Айзексон", Genre = Genre.Biography, Year = 2011, Price = 720 }
            });
        }


        private bool IsValidYear(int year)
        {
            return year >= 1000 && year <= DateTime.Now.Year;
        }


        private bool IsValidPrice(decimal price)
        {
            return price >= 0;
        }


        private bool IsValidString(string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        public bool AddBook(string title, string author, Genre genre, int year, decimal price)
        {
            if (!IsValidString(title) || !IsValidString(author) || !IsValidYear(year) || !IsValidPrice(price))
            {
                return false;
            }

            books.Add(new Book
            {
                Title = title.Trim(),
                Author = author.Trim(),
                Genre = genre,
                Year = year,
                Price = price
            });
            return true;
        }


        public bool RemoveBook(int id)
        {
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                books.Remove(book);
                return true;
            }
            return false;
        }


        public List<Book> FindByTitle(string title)
        {
            return books.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
        }


        public List<Book> FindByAuthor(string author)
        {
            return books.Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase)).ToList();
        }


        public List<Book> FindByGenre(Genre genre)
        {
            return books.Where(b => b.Genre == genre).ToList();
        }


        public List<Book> SortByTitle()
        {
            return books.OrderBy(b => b.Title).ToList();
        }


        public List<Book> SortByYear()
        {
            return books.OrderBy(b => b.Year).ToList();
        }


        public Book GetMostExpensiveBook()
        {
            return books.OrderByDescending(b => b.Price).FirstOrDefault();
        }

        public Book GetCheapestBook()
        {
            return books.OrderBy(b => b.Price).FirstOrDefault();
        }


        public Dictionary<string, int> GetBooksByAuthor()
        {
            return books.GroupBy(b => b.Author)
                       .ToDictionary(g => g.Key, g => g.Count());
        }

        public List<Book> GetAllBooks()
        {
            return new List<Book>(books);
        }

    }

    class LibraryUI
    {
        private Library library = new Library();

        public void Run()
        {
            library.AddTestData();

            Console.WriteLine("\nДобро пожаловать в систему учета книг библиотеки!");

            bool continueWorking = true;
            while (continueWorking)
            {
                ShowMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowAllBooks();
                        break;
                    case "2":
                        AddBook();
                        break;
                    case "3":
                        RemoveBook();
                        break;
                    case "4":
                        FindBooks();
                        break;
                    case "5":
                        SortBooks();
                        break;
                    case "6":
                        ShowPriceExtremes();
                        break;
                    case "7":
                        ShowAuthorStatistics();
                        break;
                    case "0":
                        continueWorking = false;
                        break;
                    default:
                        Console.WriteLine("\nНеверный выбор. Попробуйте снова.");
                        break;
                }

                if (continueWorking)
                {
                    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            Console.WriteLine("\nПрограмма завершена. Спасибо за использование!");
        }

        private void ShowMenu()
        {
            Console.WriteLine("\n======== Меню ========");
            Console.WriteLine("1. Показать все книги");
            Console.WriteLine("2. Добавить книгу");
            Console.WriteLine("3. Удалить книгу по ID");
            Console.WriteLine("4. Найти книги");
            Console.WriteLine("5. Сортировать книги");
            Console.WriteLine("6. Самая дорогая/дешевая книга");
            Console.WriteLine("7. Статистика по авторам");
            Console.WriteLine("0. Выйти");
            Console.Write("\nВаш выбор: ");
        }

        private void ShowAllBooks()
        {
            var books = library.GetAllBooks();
            if (books.Count == 0)
            {
                Console.WriteLine("\nВ библиотеке нет книг.");
                return;
            }

            Console.WriteLine($"\n=== Все книги ({books.Count}) ===");
            foreach (var book in books)
            {
                Console.WriteLine(book);
            }
        }

        private void AddBook()
        {
            Console.WriteLine("\n=== Добавление новой книги ===");

            try
            {
                Console.Write("Введите название книги: ");
                string title = Console.ReadLine();

                Console.Write("\nВведите автора: ");
                string author = Console.ReadLine();

                Console.WriteLine("\nДоступные жанры:");
                foreach (Genre genre in Enum.GetValues(typeof(Genre)))
                {
                    Console.WriteLine($"  {(int)genre} - {genre}");
                }
                Console.Write("\nВыберите жанр (число): ");
                if (!Enum.TryParse(Console.ReadLine(), out Genre genreChoice) || !Enum.IsDefined(typeof(Genre), genreChoice))
                {
                    Console.WriteLine("\nОшибка: Неверный выбор жанра.");
                    return;
                }

                Console.Write("\nВведите год издания: ");
                if (!int.TryParse(Console.ReadLine(), out int year))
                {
                    Console.WriteLine("\nОшибка: Год должен быть числом.");
                    return;
                }

                Console.Write("\nВведите цену: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal price))
                {
                    Console.WriteLine("\nОшибка: Цена должна быть числом.");
                    return;
                }

                if (library.AddBook(title, author, genreChoice, year, price))
                {
                    Console.WriteLine("\nКнига успешно добавлена!");
                }
                else
                {
                    Console.WriteLine("\nОшибка: Проверьте корректность введенных данных.");
                    Console.WriteLine("- Название и автор не должны быть пустыми");
                    Console.WriteLine("- Год должен быть от 1000 до текущего года");
                    Console.WriteLine("- Цена не должна быть отрицательной");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка при вводе данных: {ex.Message}");
            }
        }

        private void RemoveBook()
        {
            Console.WriteLine("\n=== Удаление книги ===");
            ShowAllBooks();

            Console.Write("Введите ID книги для удаления: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                if (library.RemoveBook(id))
                {
                    Console.WriteLine("\nКнига успешно удалена!");
                }
                else
                {
                    Console.WriteLine("\nКнига с указанным ID не найдена.");
                }
            }
            else
            {
                Console.WriteLine("\nОшибка: ID должен быть числом.");
            }
        }

    }

}