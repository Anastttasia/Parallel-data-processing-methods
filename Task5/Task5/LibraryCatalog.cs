namespace Task4
{
    internal class LibraryCatalog
    {
        private Dictionary<string, string> _catalog = new Dictionary<string, string>();
        private ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

        public void AddBook(string title, string author)
        {
            _locker.EnterWriteLock();
            try
            {
                if (!_catalog.ContainsKey(title))
                    _catalog.Add(title, author);
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        public void RemoveBook(string title)
        {
            _locker.EnterWriteLock();
            try
            {
                if (_catalog.ContainsKey(title))
                    _catalog.Remove(title);
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        public void UpdateBook(string title, string newTitle, string newAuthor)
        {
            _locker.EnterWriteLock();
            try
            {
                if (_catalog.ContainsKey(title))
                {
                    _catalog.Remove(title);
                    _catalog.Add(newTitle, newAuthor);
                }
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        public List<Tuple<string, string>> SearchBooks(string keyword)
        {
            List<Tuple<string, string>> books = new List<Tuple<string, string>>();
            _locker.EnterReadLock();
            try
            {
                foreach (var book in _catalog)
                {
                    if (book.Key.Contains(keyword) || book.Value.Contains(keyword))
                        books.Add(new Tuple<string, string>(book.Key, book.Value));
                }
            }
            finally
            {
                _locker.ExitReadLock();
            }
            return books;
        }

        public List<Tuple<string, string>> GetAllBooks()
        {
            List<Tuple<string, string>> books = new List<Tuple<string, string>>();
            _locker.EnterReadLock();
            try
            {
                foreach (var book in _catalog)
                {
                    books.Add(new Tuple<string, string>(book.Key, book.Value));
                }
            }
            finally
            {
                _locker.ExitReadLock();
            }
            return books;
        }

        public bool TryAddBook(string title, string author, int timeoutMs)
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_catalog, timeoutMs, ref lockTaken);
                if (lockTaken)
                {
                    if (!_catalog.ContainsKey(title))
                        _catalog.Add(title, author);
                }
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_catalog);
            }
            return lockTaken;
        }

        public bool TrySearchBooks(string keyword, int timeoutMs)
        {
            bool lockTaken = false;
            bool searched = false;
            try
            {
                Monitor.TryEnter(_catalog, timeoutMs, ref lockTaken);
                if (lockTaken)
                {
                    foreach (var book in _catalog)
                    {
                        if (book.Key.Contains(keyword) || book.Value.Contains(keyword))
                            searched = true;
                    }
                }
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_catalog);
            }
            return lockTaken;
        }

    }
}
