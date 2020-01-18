using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Algorithm
{
    public class Process
    {
        private int k1;
        private int k2;

        private readonly int flt_edge = 30;
        private readonly int pd_thre1 = 10;
        private readonly int pd_thre2 = 10;
        private readonly float eu = 0.095555555555556f;
        private readonly int int1_dot = 314;
        private readonly int int2_dot = 1256;

        private readonly ArrayList result;

        private State currentState = State.SomeOne;

        private readonly List<float> pdR1;
        private readonly List<float> pdR2;

        private int recvCount = 0;

        private readonly GestureAndPresenceMethod gestureAndPresenceMethod;

        public Process(GestureAndPresenceMethod gestureAndPresenceMethod, IReadFile readConfigration)
        {
            this.gestureAndPresenceMethod = gestureAndPresenceMethod;
            result = new ArrayList(3) {0, null, null};

            var list = readConfigration.ReadXmlFile();

            pdR1 = new List<float>();
            pdR2 = new List<float>();

            k1 = int1_dot;
            k2 = int2_dot;

            for (int i = 0; i < 40; i++)
            {
                pdR1.Add(0f);
                pdR2.Add(0f);
            }
        }

        public ArrayList DataProcess(byte[] bytes)
        {
            var tmpTuple = gestureAndPresenceMethod.Byte2Int16(bytes);

            var s1Bd = tmpTuple.Item1.BandPassFilter().LowPassFilter().Skip(flt_edge).ToList();
            var s2Bd = tmpTuple.Item2.BandPassFilter().LowPassFilter().Skip(flt_edge).ToList();

            var r1 = s1Bd.CalculateR(pd_thre1, eu, int1_dot, int2_dot);
            var r2 = s2Bd.CalculateR(pd_thre2, eu, int1_dot, int2_dot);

            pdR1.Add(r1);
            pdR2.Add(r2);
            pdR1.RemoveAt(0);
            pdR2.RemoveAt(0);

            recvCount++;

            if (recvCount > 19)
            {
                recvCount = 0;
                currentState = currentState == State.NoOne ? gestureAndPresenceMethod.NoneToPresence(pdR1, pdR2) : gestureAndPresenceMethod.PresenceToNone(pdR1, pdR2);
            }

            if (currentState == State.SomeOne)
            {
                if (gestureAndPresenceMethod.TrackCount < 5)
                {
                    gestureAndPresenceMethod.FindTrackStartPoint(s1Bd, s2Bd, ref k1, ref k2);
                }
                else
                {
                    var result = gestureAndPresenceMethod.TrackComplete(s1Bd, s2Bd, ref k1, ref k2)
                        .Bind(gestureAndPresenceMethod.FindGestureStartPoint)
                        .Bind(gestureAndPresenceMethod.FindGestureStopPoint)
                        .Match(
                            x => gestureAndPresenceMethod.GestureKind(x),
                            () => State.SomeOne
                        );
                    //todo 增加手势间隔判断
                }
            }

            return null;
        }
    }
}
