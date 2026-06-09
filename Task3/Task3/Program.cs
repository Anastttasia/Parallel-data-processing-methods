using Task3;

int seed = 42;
int transferCount = 100000;

List<decimal> transfers = new List<decimal>();

Random rand = new Random(seed);

SynchronizationBenchmark benchmark = new SynchronizationBenchmark();


for (int i = 0; i < transferCount; i++)
{
    transfers.Add((decimal)(10.0 + rand.NextDouble() * 990.0) * (i % 2 == 0 ? 1.0M : -1.0M));
}

Console.WriteLine("=== Результаты тестирования синхронизации ===");
Console.WriteLine($"Количество транзакций: {transferCount}");

benchmark.CompareAllApproaches(transfers);


//DEADLOCK
//List<BankAccount> accounts = new List<BankAccount>();

//for (int i = 0; i < 10; i++)
//{
//    accounts.Add(new BankAccount());
//}

//TransactionProcessor.ProcessConcurrentTransfers(accounts, transferCount);
