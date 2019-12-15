using Microsoft.VisualStudio.TestTools.UnitTesting;
using GP_Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GP_Dal.Models;

namespace GP_Dal.Tests
{
    [TestClass()]
    public class DalTests
    {
        [TestMethod()]
        public void GetAggregateDataTest()
        {
            Dal dal = new Dal();
            AggData aggData = dal.GetAggregateData(Dal.AggMode.all, "7YQRY6");
            Assert.IsNotNull(aggData);

            aggData = dal.GetAggregateData(Dal.AggMode.all, "doesn't exist");
            Assert.IsNull(aggData);
        }
    }
}