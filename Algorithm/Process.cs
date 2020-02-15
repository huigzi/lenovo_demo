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

        private readonly int flt_edge;
        private readonly int pd_thre1;
        private readonly int pd_thre2;
        private readonly float eu;
        private readonly int int1_dot;
        private readonly int int2_dot;

        private State currentState = State.SomeOne;

        private readonly List<float> pdR1;
        private readonly List<float> pdR2;

        private int recvCount = 0;

        private readonly GestureAndPresenceMethod gestureAndPresenceMethod;

        public Process(GestureAndPresenceMethod gestureAndPresenceMethod, IReadFile readConfigration)
        {
            this.gestureAndPresenceMethod = gestureAndPresenceMethod;

            var list = readConfigration.ReadJsonFile();

            pdR1 = new List<float>();
            pdR2 = new List<float>();

            int1_dot = list.ConfigurationGroupInt["int1_dot"];
            int2_dot = list.ConfigurationGroupInt["int2_dot"];
            flt_edge = list.ConfigurationGroupInt["flt_edge"];
            pd_thre1 = list.ConfigurationGroupInt["pd_thre1"];
            pd_thre2 = list.ConfigurationGroupInt["pd_thre2"];

            eu = list.ConfigurationGroupFloat["eu"];

            k1 = int1_dot;
            k2 = int2_dot;

            for (int i = 0; i < 40; i++)
            {
                pdR1.Add(0f);
                pdR2.Add(0f);
            }
        }

        //public ArrayList DataProcess(byte[] bytes)
        public State DataProcess((short[], short[]) bytes)
        {
            var tmpTuple = gestureAndPresenceMethod.Byte2Int16(bytes);

            var s1Bd = tmpTuple.Item1.BandPassFilter().LowPassFilter().Skip(flt_edge - 1).ToList();
            var s2Bd = tmpTuple.Item2.BandPassFilter().LowPassFilter().Skip(flt_edge - 1).ToList();

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
                    return gestureAndPresenceMethod.TrackComplete(s1Bd, s2Bd, ref k1, ref k2)
                        .Bind(gestureAndPresenceMethod.CheckInterval)
                        .Bind(gestureAndPresenceMethod.FindGestureStartPoint)
                        .Bind(gestureAndPresenceMethod.FindGestureStopPoint)
                        .Match(
                            (x) =>
                            {
                                gestureAndPresenceMethod.IntervalFlag = 1;
                                return gestureAndPresenceMethod.GestureKind(x);
                            },
                            () => State.SomeOne
                        );
                }
            }

            return currentState;
        }
    }
}
