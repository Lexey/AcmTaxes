using Newtonsoft.Json;
using NLog;

namespace Acm.DB;

public class Database(string dbPath) : IDatabase
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private record struct MergeResult(List<Transaction> Transactions, bool Updated);

    public List<Transaction> LoadTransactions()
    {
        if (!File.Exists(dbPath))
        {
            Logger.Info("Db {0} does not exist. Assuming it is empty", dbPath);
            return [];
        }

        using var sr = new StreamReader(dbPath);
        using var reader = new JsonTextReader(sr);
        return new JsonSerializer().Deserialize<List<Transaction>>(reader) ?? [];
    }

    public bool SaveTransactions(List<Transaction> transactions, bool upsert)
    {
        var duplicateId = transactions.CountBy(t => t.Id).FirstOrDefault(g => g.Value > 1);
        if (duplicateId.Value > 0)
        {
            throw new InvalidDataException($"There are new transactions with the same id {duplicateId.Key}");
        }

        var db = new Database(dbPath);
        var existing = db.LoadTransactions();
        Logger.Info("Loaded {0} transactions from the db", existing.Count);
        if (existing.Count > 0)
        {
            Logger.Info("Merging transactions");
            var mergeResult = MergeTransactions(existing, transactions, upsert);
            if (!mergeResult.Updated)
            {
                Logger.Info("There are no new transactions. Db update skipped");
                return false;
            }
            transactions = mergeResult.Transactions;
        }

        transactions = transactions.OrderBy(t => t.SettlementDate).ThenBy(t => t.Id).ToList();

        db.SaveTransactions(transactions);
        Logger.Info("Saved {0} transactions to the db", transactions.Count);
        return true;
    }

    private static MergeResult MergeTransactions(List<Transaction> existing, List<Transaction> transactions, bool upsert)
    {
        var result = new List<Transaction>();
        var updated = false;
        var existingById = existing.ToDictionary(t => t.Id, t => t);
        foreach (var t in transactions)
        {
            if (existingById.TryGetValue(t.Id, out var existingTransaction))
            {
                if (existingTransaction == t)
                {
                    continue;
                }
                if (!upsert)
                {
                    Logger.Error($"Duplicate transactions: existing = {existingTransaction.ToString()}, new = {t.ToString()}");
                    throw new InvalidDataException($"A transaction with the same id {t.Id} but different data already exists in the Db");
                }
                existingById[t.Id] = t;
                updated = true;
            }
            else
            {
                result.Add(t);
                updated = true;
            }
        }
        result.AddRange(existingById.Select(kv => kv.Value));
        return new MergeResult(result, updated);
    }


    private void SaveTransactions(List<Transaction> transactions)
    {
        using var sw = new StreamWriter(dbPath);
        using var writer = new JsonTextWriter(sw);
        var serializer = new JsonSerializer
        {
            Formatting = Formatting.Indented
        };
        serializer.Serialize(writer, transactions);
    }
}
