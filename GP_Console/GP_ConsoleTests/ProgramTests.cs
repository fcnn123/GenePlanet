using Microsoft.VisualStudio.TestTools.UnitTesting;
using GP_Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GP_Dal.Models;

namespace GP_Console.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public async Task GetOnlineDataTestAsync()
        {
            float[] minmax = await GP_Console.Program.GetOnlineData(null, "7YQRY6", "localhost:8083", new Logger(Logger.LogMode.console));
            Assert.AreNotEqual(float.MaxValue, minmax[0]);
            Assert.AreNotEqual(float.MinValue, minmax[1]);

            minmax = await GP_Console.Program.GetOnlineData(null, "doesn't exist", "localhost:8083", new Logger(Logger.LogMode.console));
            Assert.AreEqual(float.MaxValue, minmax[0]);
            Assert.AreEqual(float.MinValue, minmax[1]);

            AggData aggData = new AggData { Min = 50, Max = 70 };
            minmax = await GP_Console.Program.GetOnlineData(aggData, "doesn't exist", "localhost:8083", new Logger(Logger.LogMode.console));
            Assert.AreNotEqual(float.MaxValue, minmax[0]);
            Assert.AreNotEqual(float.MinValue, minmax[1]);
        }
    }
}