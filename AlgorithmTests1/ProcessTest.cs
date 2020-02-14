using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Algorithm;
using Core;
using MathNet.Numerics.Data.Matlab;

namespace UnitMethodTests
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

            var process = new Process(method, readConfiguration);

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

            var finalresult = result.Where(x => x.Item1 != State.SomeOne).ToArray();

        }
    }
}
