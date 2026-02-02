namespace Acm.DB;

public interface IDatabase
{
    public List<Transaction> LoadTransactions();

    public bool SaveTransactions(List<Transaction> transactions, bool upsert);
}
