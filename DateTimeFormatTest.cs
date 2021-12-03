using LinqToDB;
using LinqToDB.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace example.linq2db.sqlite.datetimeformat {
    [TestClass]
    public class DateTimeFormatTest {

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            //always use a new clean database in the TestDir..
            DataConnection.DefaultSettings = new MockDBSettings(context.TestDir);
            MyDB.Init();
        }

        [TestMethod]
        public async Task CheckDateTimeFormat() {
            DateTimeOffset d = new(2021, 1, 1, 6, 6, 6, TimeSpan.Zero);

            using MyDB db = new();

            db.Insert(new TestTable() { Id = 1, HistStartTime = d });
            TestTable? result = await db.TestTables.FirstOrDefaultAsync();

            Assert.AreEqual(d, result?.HistStartTime);

            MyClass? resultString = db.Query<MyClass>("select HistStartTime from TestTable").FirstOrDefault();
            Assert.AreEqual(d.ToString("o"), resultString?.HistStartTime);
        }

        private class MyClass {
            public string? HistStartTime { get; set; }
        }
    }
}