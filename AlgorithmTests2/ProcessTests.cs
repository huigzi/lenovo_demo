using Microsoft.VisualStudio.TestTools.UnitTesting;
using Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Data.Matlab;
namespace Algorithm.Tests
{
    [TestClass()]
    public class ProcessTests
    {
        [TestMethod()]
        public void DataProcessTest()
        {
            var ch1Data = MatlabReader.ReadAll<double>("gesture_standing100.mat")["ch1"];
            var ch2Data = MatlabReader.ReadAll<double>("gesture_standing100.mat")["ch2"];

            Assert.Fail();
        }
    }
}