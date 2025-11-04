
class MainProgramm
{
    enum Genre
    {
        Детектив;
        Триллер;
        Комедия;
    }
    class Book
    {
        public int id;
        public string name;
        public string author;
        public Genre genre;
        public int year;
        public int price;
        List<Book> Library = new List<Book>();

        static void Add
        {
            Console.WriteLine("Ввдите название произведения, фамилию автора, жанр из предложенных, год написания, цену за книгу");
            for (int i = 0; i<Genre.length; i++)
			{
                Console.WriteLine(Genre[i]);
			}

    Library.add

}

    }
}