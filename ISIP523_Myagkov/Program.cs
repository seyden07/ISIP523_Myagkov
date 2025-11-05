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

    }

}