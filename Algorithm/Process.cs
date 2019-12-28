using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Algorithm
{
    public class Process
    {
        private int startFrameCountNum = 0;
        private int presenceFrameCountNum = 0;
        private int gestureFrameCountNum = 0;
        private int detMiss = 0;

        private readonly int det_miss_thre;
        private readonly int fb;
        private readonly int ff;

        private ArrayList result;

        private State currentState = State.SomeOne;

        private readonly List<float> res1;
        private readonly List<float> res2;
        private readonly List<float> r;
        private readonly List<float> theta;

        private readonly GestureAndPresenceMethod gestureAndPresenceMethod;

        public Process(GestureAndPresenceMethod gestureAndPresenceMethod, IReadFile readConfigration)
        {
            this.gestureAndPresenceMethod = gestureAndPresenceMethod;
            result = new ArrayList(3) {0, null, null};

            var list = readConfigration.ReadXmlFile();
            det_miss_thre = list[3];
            fb = list[5];
            ff = list[6];

            res1 = new List<float>();
            res2 = new List<float>();
            r = new List<float>();
            theta = new List<float>();
        }

        public ArrayList DataProcess(byte[] bytes)
        {
            if (currentState != State.NoOne && currentState != State.SomeOne)
            {
                currentState = State.SomeOne;
            }

            var tmpTuple = gestureAndPresenceMethod.PrePorcessData(bytes);

            if (startFrameCountNum >= 2)
            {
                gestureAndPresenceMethod.Detection(tmpTuple, res1, res2, r, theta);
            }
            else
            {
                startFrameCountNum++;
                result[0] = currentState;
                return result;
            }

            presenceFrameCountNum++;

            if (presenceFrameCountNum >= 20)
            {
                if (currentState == State.NoOne)
                {
                    currentState = gestureAndPresenceMethod.NoneToPresence(res1, res2);
                }
                else
                {
                    if (gestureAndPresenceMethod.PresenceToNone(res1, res2) == State.NoOne)
                    {
                        detMiss++;
                    }
                    else
                    {
                        detMiss = 0;
                    }

                    if (detMiss > det_miss_thre)
                    {
                        currentState = State.NoOne;
                        detMiss = 0;

                        r.Clear();
                        theta.Clear();
                        gestureFrameCountNum = 0;
                    }
                }

                res1.RemoveRange(0, 10);
                res2.RemoveRange(0, 10);
                presenceFrameCountNum = 10;
            }


            if (currentState == State.SomeOne)
            {
                if (Math.Abs(r.Last()) > 10e-15)
                {
                    if (gestureFrameCountNum < fb + ff + 1)
                    {
                        gestureFrameCountNum++;
                    }
                    else
                    {
                        r.RemoveAt(0);
                        theta.RemoveAt(0);
                    }
                }
                else
                {
                    r.RemoveAt(r.Count - 1);
                    theta.RemoveAt(theta.Count - 1);
                }

                if (gestureFrameCountNum >= fb + ff + 1)
                {
                    currentState = gestureAndPresenceMethod.NoneToGesture(r, theta);

                    if (currentState != State.NoOne && currentState != State.SomeOne)
                    {
                        result[1] = new ArrayList(r);
                        result[2] = gestureAndPresenceMethod.Smooth(r, 7);

                        r.Clear();
                        theta.Clear();
                        gestureFrameCountNum = 0;
                    }
                }
            }
            else if (currentState == State.NoOne)
            {
                if (r.Count > 0)
                {
                    r.RemoveAt(r.Count - 1);
                    theta.RemoveAt(theta.Count - 1);
                }
            }

            result[0] = currentState;

            return result;
        }
    }
}
