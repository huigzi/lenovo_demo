using System;
using System.Collections.Generic;
using System.Linq;
using Algorithm;
using Core;
using MathNet.Numerics.Data.Matlab;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlgorithmTests
{
    [TestClass()]
    public class ProcessTest
    {
        [TestMethod()]
        public void DataProcessTest()
        {
            var ch1Data = MatlabReader.ReadAll<double>("gesture_standing100.mat")["ch1"];
            var ch2Data = MatlabReader.ReadAll<double>("gesture_standing100.mat")["ch2"];
            var result = new List<(State,int)>();

            var readConfiguration = new ReadConfiguration();

            var method = new GestureAndPresenceMethod(readConfiguration);

            var process = new AlgorithmFlow(method, readConfiguration);

            for (int i = 0; i < ch1Data.ColumnCount; i++)
            {
                var input1 = ch1Data.SubMatrix(0, ch1Data.RowCount, i, 1).ToRowMajorArray().Select(x => (Int16) x)
                    .ToArray();
                var input2 = ch2Data.SubMatrix(0, ch2Data.RowCount, i, 1).ToRowMajorArray().Select(x => (Int16) x)
                    .ToArray();

                if (i < 2)
                {
                    method.Initial((input1, input2));
                }
                else
                {
                    var temp = process.DataProcess((input1, input2));
                    result.Add((temp, i));
                }
            }

            var finalResult = result.Where(x => x.Item1 != State.SomeOne).ToArray();

            Assert.AreEqual((State.LeftSweep, 134), finalResult[0]);
            Assert.AreEqual((State.LeftSweep, 482), finalResult[1]);
            Assert.AreEqual((State.RightSweep, 614), finalResult[2]);
            Assert.AreEqual((State.LeftSweep, 774), finalResult[3]);

        }
    }
}
