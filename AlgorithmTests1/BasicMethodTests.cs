using Microsoft.VisualStudio.TestTools.UnitTesting;
using Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Data.Matlab;
using Core;

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

            var method = new GestureAndPresenceMethod(new ReadConfiguration());

            //Test NoneToPresence PresenceToNone
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

            if (state != State.SomeOne)
            {
                Assert.Fail("Function PresenceToNone test failed");
            }

            r1 = new List<float>
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

            r2 = new List<float>
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

            state = method.NoneToPresence(r1, r2);

            if (state != State.SomeOne)
            {
                Assert.Fail("Function PresenceToNone test failed");
            }


            //Test FindTrackStartPoint

            int k1 = 314, k2 = 1256;

            var s1Bd = MatlabReader.ReadAll<double>("s1bd.mat")["s1bd"].ToRowMajorArray().Select(x => (float) x)
                .ToList();
            var s2Bd = MatlabReader.ReadAll<double>("s2bd.mat")["s2bd"].ToRowMajorArray().Select(x => (float) x)
                .ToList();

            method.FindTrackStartPoint(s1Bd, s2Bd, ref k1, ref k2);

            if (k1 != 342 || k2 != 708)
            {
                Assert.Fail();
            }

            //
        }
    }
}