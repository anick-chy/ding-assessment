using System.Text.Json;
using Ding.Assessment.Core.Domain;

namespace Ding.Assessment.Core.Infrastructure;

public sealed class JsonAccountRepository : IAccountRepository
{
    internal sealed class TransactionRecord
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Type { get; set; }
        public decimal Amount { get; set; }
    }

    private readonly string _filePath;
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public JsonAccountRepository(string filePath)
    {
        _filePath = filePath;

        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    public void AddTransaction(Transaction transaction)
    {
        List<TransactionRecord> records = ReadAll();

        records.Add(new TransactionRecord
        {
            Id = transaction.Id,
            CreatedAt = transaction.CreatedAt,
            Type = (int)transaction.Type,
            Amount = transaction.Amount,
        });

        WriteAll(records);
    }

    public IReadOnlyCollection<Transaction> GetTransactions()
    {
        List<TransactionRecord> records = ReadAll();

        var transactions = records
            .OrderBy(r => r.CreatedAt)
            .Select(r => Transaction.Restore(
                r.Id,
                (TransactionType)r.Type,
                r.Amount,
                r.CreatedAt))
            .ToList();

        return transactions.AsReadOnly();
    }

    private List<TransactionRecord> ReadAll()
    {
        string json = File.ReadAllText(_filePath);

        return JsonSerializer.Deserialize<List<TransactionRecord>>(json, _options)
               ?? [];
    }

    private void WriteAll(List<TransactionRecord> records)
    {
        string json = JsonSerializer.Serialize(records, _options);
        File.WriteAllText(_filePath, json);
    }
}
