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
            DataConnection.TurnTraceSwitchOn();
            DataConnection.WriteTraceLine = (s1, s2, t) => {
                Console.WriteLine($"Level {t}, Action: {s1}, Source: {s2}");
            };

            //always use a new clean database in the TestDir..
            DataConnection.DefaultSettings = new MockDBSettings(context.TestDir);
            MyDB.Init();
        }

        [TestMethod]
        public async Task CheckDateTimeFormat() {
            DateTimeOffset d = new(2021, 1, 1, 6, 6, 6, TimeSpan.Zero);

            using MyDB db = new();

            _ = await db.InsertAsync(new TestTable() { Id = 1, HistStartTime = d });
            TestTable? result = await db.TestTables.FirstOrDefaultAsync();

            Assert.AreEqual(d, result?.HistStartTime);

            MyClass? resultString = db.Query<MyClass>("select HistStartTime from TestTable").FirstOrDefault();
            Assert.AreEqual(d.ToString("o"), resultString?.HistStartTime);
        }


        [TestMethod]
        public async Task CheckDateTimeFormat2() {
            DateTimeOffset d = new(2021, 1, 1, 6, 6, 7, TimeSpan.Zero);

            using MyDB db = new();

            db.Insert(new TestTable2() { Id = 1, HistStartTime = d });
            TestTable2? result = await db.TestTables2.FirstOrDefaultAsync();

            Assert.AreEqual(d, result?.HistStartTime);

            MyClass? resultString = db.Query<MyClass>("select HistStartTime from TestTable2").FirstOrDefault();
            Assert.AreEqual(d.ToString("o"), resultString?.HistStartTime);
        }

        private class MyClass {
            public string? HistStartTime { get; set; }
        }
    }
}