using System.Collections.Concurrent;

namespace Task5
{
    class Book
    {
        public string author;
        public string title;
    }

    internal class ConcurrentLibraryCatalog
    {
        private ConcurrentDictionary<string, Book> _catalog = new ConcurrentDictionary<string, Book>();

        public bool AddBook(string title, string author)
        {
            if (_catalog.ContainsKey(title)) return false;

            Book book = new Book();

            book.author = author;
            book.title = title;

            return _catalog.TryAdd(title, book);
        }

        public bool RemoveBook(string title)
        {
            if (!_catalog.ContainsKey(title)) return false;

            Book? book;

            return _catalog.TryRemove(title, out book);
        }

        public bool UpdateBook(string title, string newTitle, string newAuthor)
        {
            if (!_catalog.ContainsKey(title)) return false;

            Book oldBook = _catalog[title];

            Book newBook = new Book();

            newBook.author = newAuthor;
            newBook.title = newTitle;

            return _catalog.TryUpdate(title, newBook, oldBook);
        }

        public List<Book> SearchBooks(string keyword)
        {

            List<Book> books = new List<Book>();

            foreach (var item in _catalog)
            {
                if (item.Value.title == keyword || item.Value.author == keyword) books.Add(item.Value);
            }

            return books;
        }

        public List<Book> GetAllBooks()
        {

            return _catalog.Values.ToList();
        }

        public int GetBookCount()
        {

            return _catalog.Count;
        }

        public void ClearCatalog()
        {

            _catalog.Clear();
        }

        public bool TryGetBook(string title, out Book book)
        {
            return _catalog.TryGetValue(title, out book);
        }
    }
}
