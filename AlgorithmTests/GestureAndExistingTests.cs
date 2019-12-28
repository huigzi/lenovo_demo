using System;
using System.Collections.Generic;
using Algorithm;
using Core;
using MathNet.Numerics.Data.Matlab;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlgorithmTests
{
    [TestClass()]
    public class GestureAndExistingTests
    {
        [TestMethod()]
        public void GestureAndExistingTest()
        {
            byte[] bytes = new byte[10800];
            var adcData = MatlabReader.ReadAll<double>("swp.mat");

            var ch1 = adcData["ch1"];
            var ch2 = adcData["ch2"];

            List<int> result1 = new List<int>();

            var method = new Process(new GestureAndPresenceMethod(new ReadTestXml()), new ReadTestXml());

            for (int i = 0; i < 200; i++)
            {
                for (int j = 0; j < 2700; j++)
                {
                    bytes[4 * j] = (byte)(ch1[j, i] % 256);
                    bytes[4 * j + 1] = (byte)(ch1[j, i] / 256);
                    bytes[4 * j + 2] = (byte)(ch2[j, i] % 256);
                    bytes[4 * j + 3] = (byte)(ch2[j, i] / 256);
                }

                var res = method.DataProcess(bytes);

                result1.Add((int)res[0]);
            }

            var result = MatlabReader.ReadAll<double>("swpResult.mat");
            var swpResult = result["swpResult"].ToArray();

            for (int i = 0; i < result1.Count; i++)
            {
                if (Math.Abs(result1[i] - swpResult[0, i]) > 10e-15)
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod()]
        public void CalculateTest()
        {
            byte[] bytes = new byte[10800];
            var adcData = MatlabReader.ReadAll<double>("present.mat");

            var ch1 = adcData["ch1"];
            var ch2 = adcData["ch2"];

            List<int> result = new List<int>();

            var method = new Process(new GestureAndPresenceMethod(new ReadTestXml()), new ReadTestXml());

            for (int i = 0; i < 600; i++)
            {
                for (int j = 0; j < 2700; j++)
                {
                    bytes[4 * j] = (byte)(ch1[j, i] % 256);
                    bytes[4 * j + 1] = (byte)(ch1[j, i] / 256);
                    bytes[4 * j + 2] = (byte)(ch2[j, i] % 256);
                    bytes[4 * j + 3] = (byte)(ch2[j, i] / 256);
                }

                var res = method.DataProcess(bytes);

                result.Add((int)res[0]);
            }

            var result1 = MatlabReader.ReadAll<double>("presentResult.mat");
            var presentResult = result1["presentResult"].ToArray();

            for (int i = 0; i < result.Count; i++)
            {
                if (Math.Abs(result[i] - presentResult[0, i]) > 10e-15)
                {
                    Assert.Fail();
                }
            }
        }
    }
}