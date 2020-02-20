using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Algorithm;
using Core;
using MathNet.Numerics.Data.Matlab;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlgorithmTests
{
    [TestClass]
    public class UnitMethodTests
    {
        private GestureAndPresenceMethod method = new GestureAndPresenceMethod(new ReadConfiguration());

        private readonly short[] signalData = MatlabReader.ReadAll<double>("signal.mat")["signal"].ToRowMajorArray()
            .Select(x => (Int16) (x)).ToArray();

        private readonly List<float> s1Bd = MatlabReader.ReadAll<double>("s1bd.mat")["s1bd"].ToRowMajorArray().Select(x => (float)x)
            .ToList();

        private readonly List<float> s2Bd = MatlabReader.ReadAll<double>("s2bd.mat")["s2bd"].ToRowMajorArray().Select(x => (float)x)
            .ToList();

        private readonly List<float> trackline1 = MatlabReader.ReadAll<double>("track1.mat")["track1_win"].ToColumnMajorArray()
            .Select(x => (float)x).ToList();

        private readonly List<float> trackline2 = MatlabReader.ReadAll<double>("track2.mat")["track2_win"].ToColumnMajorArray()
            .Select(x => (float)x).ToList();

        private readonly List<float> tracklineGesture1 = MatlabReader.ReadAll<double>("track1_gesture.mat")["track1_win"].ToColumnMajorArray()
            .Select(x => (float)x).ToList();

        private readonly List<float> tracklineGesture2 = MatlabReader.ReadAll<double>("track2_gesture.mat")["track2_win"].ToColumnMajorArray()
            .Select(x => (float)x).ToList();

        [TestMethod()]
        public void FilterTest()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var result1 = signalData.BandPassFilter().LowPassFilter();
            sw.Stop();
            //获取运行时间[毫秒]  
            var times = sw.Elapsed.TotalMilliseconds;
        }

        [TestMethod()]
        public void NoneToPresenceTest()
        {
            var r1 = new List<float>
            {
                71.4755555555555f, 43.8600000000000f, 40.3244444444444f, 46.8222222222222f, 38.8911111111111f,
                49.1155555555556f, 44.9111111111111f, 45.7711111111111f, 46.8222222222222f, 112.373333333333f,
                112.086666666667f, 112.182222222222f, 112.086666666667f, 111.608888888889f, 111.417777777778f,
                110.271111111111f, 109.984444444444f, 0, 40.6111111111111f, 39.4644444444444f,
                0, 0, 0, 0, 0,
                0, 0, 0, 0, 0,
                0, 0, 0, 0, 0,
                0, 0, 0, 0, 0
            };

            var r2 = new List<float>
            {
                71.4755555555555f, 43.8600000000000f, 40.3244444444444f, 46.8222222222222f, 38.8911111111111f,
                49.1155555555556f, 44.9111111111111f, 45.7711111111111f, 46.8222222222222f, 112.373333333333f,
                112.086666666667f, 112.182222222222f, 112.086666666667f, 111.608888888889f, 111.417777777778f,
                110.271111111111f, 109.984444444444f, 0, 40.6111111111111f, 39.4644444444444f,
                0, 0, 0, 0, 0,
                0, 0, 0, 0, 0,
                0, 0, 0, 0, 0,
                0, 0, 0, 0, 0
            };

            var state = method.PresenceToNone(r1, r2);

            Assert.AreEqual(State.SomeOne, state);
        }

        [TestMethod()]
        public void PresenceToNoneTest()
        {
            var r1 = new List<float>
            {
                87.2422222222222f, 0, 89.9177777777778f, 87.3377777777778f, 87.7200000000000f,
                87.9111111111111f, 0, 88.8666666666667f, 86.0955555555556f, 88.9622222222222f,
                85.5222222222222f, 0, 0, 0, 90.3000000000000f,
                88.1977777777778f, 89.1533333333333f, 90.0133333333333f, 85.2355555555556f, 88.7711111111111f,
                88.9622222222222f, 94.1222222222222f, 89.4400000000000f, 88.4844444444444f, 0,
                0, 0, 0, 0, 85.1400000000000f,
                0, 0, 0, 0, 0,
                0, 0, 0, 0, 0
            };

            var r2 = new List<float>
            {
                87.2422222222222f, 0, 89.9177777777778f, 87.3377777777778f, 87.7200000000000f,
                87.9111111111111f, 0, 88.8666666666667f, 86.0955555555556f, 88.9622222222222f,
                85.5222222222222f, 0, 0, 0, 90.3000000000000f,
                88.1977777777778f, 89.1533333333333f, 90.0133333333333f, 85.2355555555556f, 88.7711111111111f,
                88.9622222222222f, 94.1222222222222f, 89.4400000000000f, 88.4844444444444f, 0,
                0, 0, 0, 0, 85.1400000000000f,
                0, 0, 0, 0, 0,
                0, 0, 0, 0, 0
            };

            var state = method.NoneToPresence(r1, r2);

            Assert.AreEqual(State.SomeOne, state);
        }

        [TestMethod()]
        public void FindTrackStartPointTest()
        {
            int k1 = 314, k2 = 1256;

            method.FindTrackStartPoint(s1Bd, s2Bd, ref k1, ref k2);

            Assert.AreEqual(342, k1);
            Assert.AreEqual(708, k2);
        }

        [TestMethod()]
        public void GestureTrackTest()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var result = method.FindGestureStartPoint((trackline1, trackline2))
                .Bind(method.FindGestureStopPoint);

            sw.Stop();
            //获取运行时间[毫秒]  
            var times = sw.Elapsed.TotalMilliseconds;

            Assert.AreEqual(false, result.IsSome);
        }

        [TestMethod()]
        public void GestureKindTest()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var result2 = method.FindGestureStartPoint((tracklineGesture1, tracklineGesture2))
                .Bind(method.FindGestureStopPoint)
                .Match(
                    x => method.GestureKind(x),
                    () => State.SomeOne
                );

            sw.Stop();
            //获取运行时间[毫秒]  
            var times = sw.Elapsed.TotalMilliseconds;

            Assert.AreEqual(State.LeftSweep, result2);
        }

    }
}