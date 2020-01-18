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
    public class BasicMethodTests
    {
        [TestMethod()]
        public void BandPassFilterTest()
        {
            var signalData = MatlabReader.ReadAll<double>("signal.mat")["signal"].ToRowMajorArray()
                .Select(x => (Int16) (x)).ToArray();

            var result1 = signalData.BandPassFilter().LowPassFilter();

            Assert.Fail();
        }
    }
}