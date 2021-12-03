using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Data;
using LinqToDB.Mapping;
using System;
using System.Globalization;
using System.Linq;

namespace example.linq2db.sqlite.datetimeformat;
public class MyDB : DataConnection {
    private static readonly IFormatProvider _Provider = CultureInfo.InvariantCulture.DateTimeFormat;
    private static readonly Lazy<MappingSchema> _mappingSchema = new(CreateMapping);

    internal static class TableNames {
        public const string TestTable = "TestTable";
        public const string TestTable2 = "TestTable2";
    }

    public MyDB() : base("MyDB", _mappingSchema.Value) { }


    public static void Init() {
        CreateTables();
    }

    public static MappingSchema CreateMapping() {
        MappingSchema? ret = new();
        FluentMappingBuilder? m = ret.GetFluentMappingBuilder();
        m.Entity<TestTable>()
            .Property(x => x.HistStartTime)
            .HasConversion(
                x => x.HasValue ? x.Value.ToString("O", _Provider) : string.Empty,
                x => string.IsNullOrWhiteSpace(x) ? null : DateTimeOffset.ParseExact(x, "O", _Provider));
        return ret;
    }

    public static void CreateTables() {
        using MyDB? db = new();

        //Fetching the schema
        LinqToDB.SchemaProvider.ISchemaProvider? sp = db.DataProvider.GetSchemaProvider();
        LinqToDB.SchemaProvider.DatabaseSchema? dbSchema = sp.GetSchema(db);

        if (!dbSchema.Tables.Any(t => t.TableName == MyDB.TableNames.TestTable)) { db.CreateTable<TestTable>(); }
        if (!dbSchema.Tables.Any(t => t.TableName == MyDB.TableNames.TestTable2)) { db.CreateTable<TestTable2>(); }
    }

    public ITable<TestTable> TestTables => GetTable<TestTable>();
    public ITable<TestTable2> TestTables2 => GetTable<TestTable2>();
}

public class DateTimeOffsetConverterAttribute : ValueConverterAttribute {
    public DateTimeOffsetConverterAttribute() : base() {
        ValueConverter = new DateTimeOffsetConverter();
        ConverterType = typeof(DateTimeOffsetConverter);
    }
}

internal class DateTimeOffsetConverter : ValueConverter<DateTimeOffset?, string?> {
    private static readonly IFormatProvider _Provider = CultureInfo.InvariantCulture.DateTimeFormat;
    public DateTimeOffsetConverter() : base(
        input => input.HasValue ? input.Value.ToString("O", _Provider) : string.Empty,
        input => string.IsNullOrWhiteSpace(input) ? null : DateTimeOffset.ParseExact(input, "O", _Provider),
        false) {
    }
}

[Table(MyDB.TableNames.TestTable)]
public partial class TestTable {

    [Column("ID"), PrimaryKey, Identity] public int Id { get; set; }

    [Column("HistStartTime"), Nullable] public DateTimeOffset? HistStartTime { get; set; }
}


[Table(MyDB.TableNames.TestTable2)]
public partial class TestTable2 {

    [Column("ID"), PrimaryKey, Identity] public int Id { get; set; }

    [Column("HistStartTime"), Nullable, DateTimeOffsetConverter] public DateTimeOffset? HistStartTime { get; set; }
}

