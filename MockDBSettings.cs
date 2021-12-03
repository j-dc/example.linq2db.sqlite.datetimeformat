using LinqToDB.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace example.linq2db.sqlite.datetimeformat;

public class MockDBSettings : ILinqToDBSettings {
    private readonly string RootPath;
    public MockDBSettings(string rootPath) {
        RootPath = rootPath;
    }

    public IEnumerable<IDataProviderSettings> DataProviders
        => Enumerable.Empty<IDataProviderSettings>();

    public string DefaultConfiguration => "Localstorage";
    public string DefaultDataProvider => "Sqlite.MS";


    public IEnumerable<IConnectionStringSettings> ConnectionStrings {
        get {
            yield return new ConnectionStringSettings("MyDB", "SQLite.Classic", $@"Data Source={RootPath}\mydb.db");
        }
    }
}

public class ConnectionStringSettings : IConnectionStringSettings {
    public ConnectionStringSettings(string name, string provider, string connectionString) {
        Name = name;
        ProviderName = provider;
        ConnectionString = connectionString;

    }
    public string ConnectionString { get; set; }
    public string Name { get; set; }
    public string ProviderName { get; set; }
    public bool IsGlobal => false;
}
