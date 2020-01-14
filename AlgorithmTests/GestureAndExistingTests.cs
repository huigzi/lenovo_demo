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
        public void CalculateTest()
        {
            byte[] bytes = new byte[10800];
            var adcData = MatlabReader.ReadAll<double>("present.mat");

            var ch1 = adcData["ch1"];
            var ch2 = adcData["ch2"];

            List<int> result = new List<int>();

            //test read xml
            int[] configArray = new int[15] { 15, 15, 15, 12, 10, 4, 30, 5, 15, 2, 3, 3, 2, 3, 2 };
            ReadTestXml readTestConfig = new ReadTestXml();
            var configList = readTestConfig.ReadXmlFile();
            for (int i = 0; i < 15; i++)
            {
                if (configList[i] != configArray[i])
                {
                    Assert.Fail();
                }
            }

            //test PrePorcessData and Detection
            var chData = new Tuple<List<short[]>, List<short[]>>(new List<short[]> { new short[2700], new short[2700], new short[2700] }, 
                new List<short[]> { new short[2700], new short[2700], new short[2700] });
            GestureAndPresenceMethod testGestureAndPresenceMethod = new GestureAndPresenceMethod(new ReadTestXml());
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 2700; j++)
                {
                    bytes[4 * j] = (byte)(ch1[j, i] % 256);
                    bytes[4 * j + 1] = (byte)(ch1[j, i] / 256);
                    bytes[4 * j + 2] = (byte)(ch2[j, i] % 256);
                    bytes[4 * j + 3] = (byte)(ch2[j, i] / 256);
                }
                chData = testGestureAndPresenceMethod.PrePorcessData(bytes);
            }

            var res1 = new List<float>();
            var res2 = new List<float>();
            var r = new List<float>();
            var theta = new List<float>();
            testGestureAndPresenceMethod.Detection(chData, res1, res2, r, theta);

            if (Math.Abs(res1[0] - 48.63) > 0.01 || Math.Abs(res2[0] - 47.31) > 0.01 || Math.Abs(r[0] - 47.6) > 0.01 || Math.Abs(theta[0] - 2.26) > 0.01)
            {
                Assert.Fail();
            }

            //test PresenceToNone
            var res1PresenceToNone = new List<float> { 80f, 80f, 0f, 80f, 80f, 80f, 80f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
            var res2PresenceToNone = new List<float> { 80f, 80f, 80f, 80f, 80f, 80f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };

            if (testGestureAndPresenceMethod.PresenceToNone(res1PresenceToNone, res2PresenceToNone) != State.NoOne)
            {
                Assert.Fail();
            }

            //test NoneToPresence
            var res1NoneToPresence = new List<float> { 0f, 0f, 0f, 0f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f };
            var res2NoneToPresence = new List<float> { 0f, 0f, 0f, 0f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f, 80f };

            if (testGestureAndPresenceMethod.NoneToPresence(res1NoneToPresence, res2NoneToPresence) != State.SomeOne)
            {
                Assert.Fail();
            }

            //test NoneToGesture
            var r1NoneToGesture = new List<float> { 54.6f, 51.4f, 51.7f, 54.9f, 53.7f, 53.3f, 48.9f, 37.9f, 37.5f, 34f, 34f, 32f, 33f, 34f, 38f, 38f, 41f, 42f, 55f, 55f, 
                                                    55f, 55f, 54f, 54f, 54f, 54f, 53f, 53f, 54f, 53f, 53f, 53f, 53f, 53f, 53f, 53f, 53f, 52f, 53f, 53f, 54f, 49f, 39f };
            var theta1NoneToGesture = new List<float> { 0f, 0f, 0f, 0f, 0f, 0f, 0f, -1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 0f,
                                                        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, -1f};

            var r2NoneToGesture = new List<float> { 54.6f, 51.4f, 51.7f, 54.9f, 53.7f, 53.3f, 48.9f, 37.9f, 37.5f, 34f, 34f, 32f, 33f, 34f, 38f, 38f, 41f, 42f, 55f, 55f,
                                                    55f, 55f, 54f, 54f, 54f, 54f, 53f, 53f, 54f, 53f, 53f, 53f, 53f, 53f, 53f, 53f, 53f, 52f, 53f, 53f, 54f, 49f, 39f };
            var theta2NoneToGesture = new List<float> { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, -1f, 0f,
                                                        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, -1f};

            var r3NoneToGesture = new List<float> { 54.6f, 51.4f, 51.7f, 54.9f, 53.7f, 53.3f, 48.9f, 37.9f, 37.5f, 34f, 34f, 32f, 33f, 34f, 38f, 38f, 41f, 42f, 58f, 60f,
                                                    80f, 60f, 57f, 55f, 51f, 0f, 40f, 41f, 42f, 43f, 44f, 45f, 48f, 53f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f };

            var r4NoneToGesture = new List<float> { 54.6f, 51.4f, 51.7f, 54.9f, 53.7f, 53.3f, 48.9f, 37.9f, 37.5f, 34f, 34f, 32f, 33f, 34f, 38f, 38f, 41f, 42f, 58f, 60f,
                                                    80f, 60f, 57f, 55f, 51f, 0f, 40f, 41f, 42f, 43f, 44f, 80f, 65f, 54f, 43f, 35f, 0f, 34f, 45f, 54f, 57f, 60f, 60f };

            var tmp = new List<float>();

            if (testGestureAndPresenceMethod.NoneToGesture(r1NoneToGesture, theta1NoneToGesture, ref tmp) != State.LeftSweep)
            {
                Assert.Fail();
            }

            if (testGestureAndPresenceMethod.NoneToGesture(r2NoneToGesture, theta2NoneToGesture, ref tmp) != State.RightSweep)
            {
                Assert.Fail();
            }

            if (testGestureAndPresenceMethod.NoneToGesture(r3NoneToGesture, theta1NoneToGesture, ref tmp) != State.DoubleClick)
            {
                Assert.Fail();
            }

            if (testGestureAndPresenceMethod.NoneToGesture(r4NoneToGesture, theta1NoneToGesture, ref tmp) != State.Circle)
            {
                Assert.Fail();
            }

            //test Process
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