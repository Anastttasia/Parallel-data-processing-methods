using Task4;

int seed = 42;
int booksCount = 10000;
int maxAuthorLenght = 10;
int maxTitleLenght = 15;

int resourcePoolCapacity = 10;

Random rand = new Random(seed);

LibraryCatalog library = new LibraryCatalog();
ResourcePool pool = new ResourcePool(10);
CrossProcessSync sync = new CrossProcessSync();

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

SynchronizationBenchmark benchmark = new SynchronizationBenchmark();

benchmark.CompareAllPrimitives(library, pool, sync);

