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

}