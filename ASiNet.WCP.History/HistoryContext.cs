using ASiNet.WCP.History.Actions;
using ASiNet.WCP.History.Primitives;
using Microsoft.EntityFrameworkCore;

namespace ASiNet.WCP.History;

public class HistoryContext : DbContext
{
    public DbSet<HistoryRecord> Records { get; set; }

    public DbSet<TextRecord> TextRecords { get; set; }

    public T AddRecord<T>(T record) where T : HistoryRecord
    {
        Add(record);
        SaveChanges();
        return record;
    }

    public HistoryRecord? GetRecord(long id) => Records.FirstOrDefault(x => x.Id == id);

    public IEnumerable<HistoryRecord> EnumerateRecords()
    {
        return Records;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={Path.Join(AppDomain.CurrentDomain.BaseDirectory, "history.db")}");
        base.OnConfiguring(optionsBuilder);
    }
}
