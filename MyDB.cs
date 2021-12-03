using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using System;
using System.Globalization;
using System.Linq;

namespace example.linq2db.sqlite.datetimeformat {
    public class MyDB : DataConnection {
        private static readonly IFormatProvider _Provider = CultureInfo.InvariantCulture.DateTimeFormat;
        private static readonly Lazy<MappingSchema> _mappingSchema = new(CreateMapping);

        internal static class TableNames {
            public const string TestTable = "TestTable";
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
                .HasConversion(x => x.HasValue ? x.Value.ToString("O", _Provider) : string.Empty, x => string.IsNullOrWhiteSpace(x) ? null : DateTimeOffset.ParseExact(x, "O", _Provider));
            return ret;
        }

        public static void CreateTables() {
            using MyDB? db = new();

            //Fetching the schema
            LinqToDB.SchemaProvider.ISchemaProvider? sp = db.DataProvider.GetSchemaProvider();
            LinqToDB.SchemaProvider.DatabaseSchema? dbSchema = sp.GetSchema(db);

            if (!dbSchema.Tables.Any(t => t.TableName == MyDB.TableNames.TestTable)) { db.CreateTable<TestTable>(); }
        }

        public ITable<TestTable> TestTables => GetTable<TestTable>();
    }

    [Table("TestTable")]
    public partial class TestTable {

        [Column("ID"), PrimaryKey, Identity] public int Id { get; set; }

        [Column("HistStartTime"), Nullable] public DateTimeOffset? HistStartTime { get; set; }
    }
}
