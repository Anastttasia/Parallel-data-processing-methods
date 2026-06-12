using Task4;
using Task5;

int seed = 42;
int booksCount = 1000;
int maxAuthorLenght = 10;
int maxTitleLenght = 15;

int queueManagerCapacity = 1000;

int cacheElementsCount = 500;

Random rand = new Random(seed);

ConcurrentLibraryCatalog concurrentLibraryCatalog = new ConcurrentLibraryCatalog();
LibraryCatalog library = new LibraryCatalog();
TaskQueueManager taskQueueManager = new TaskQueueManager(queueManagerCapacity);
ConcurrentCache concurrentCache = new ConcurrentCache();

var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

for (int i = 0; i < booksCount; i++)
{
    char[] author = new char[rand.Next(maxAuthorLenght)];
    char[] title = new char[rand.Next(maxTitleLenght)];

    for (int j = 0; j < author.Length; j++)
    {
        author[j] = chars[rand.Next(chars.Length)];
    }

    for (int j = 0; j < title.Length; j++)
    {
        title[j] = chars[rand.Next(chars.Length)];
    }

    library.AddBook(new string(title), new string(author));
}

for (int i = 0; i < cacheElementsCount; i++)
{
    concurrentCache.AddToCache("some_key", new object());
}

CollectionBenchmark benchmark = new CollectionBenchmark();
benchmark.taskQueueManager = taskQueueManager;
benchmark.libraryCatalog = library;
benchmark.concurrentCache = concurrentCache;
benchmark.concurrentLibraryCatalog = concurrentLibraryCatalog;

benchmark.CompareAllCollections();

