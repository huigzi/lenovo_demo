using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;

namespace Algorithm
{
    public class GestureAndPresenceMethod
    {
        private static float[] a1 = new float[3] { 1f, -1.8139f, 0.8239f };
        private static float[] a2 = new float[3] { 1f, -1.9125f, 0.9231f };
        private static float[] b1 = new float[3] { 0.0026f, 0.0051f, 0.0026f };
        private static float[] b2 = new float[3] { 0.0026f, 0.0051f, 0.0026f };

        private readonly float es;
        private readonly int dthres;
        private readonly int thres;
        private readonly int det_thres;
        private readonly int fb;
        private readonly int ff;
        private readonly int mv_thres;
        private readonly int lhp;
        private readonly int trough_thre;
        private readonly int tail_thres;
        private readonly int tail_len;
        private readonly int tailn_thres;
        private readonly int head_thres;
        private readonly int head_len;
        private readonly int headln_thres;

        List<short[]> ch1;
        List<short[]> ch2;

        public GestureAndPresenceMethod(IReadFile readConfigration)
        {
            const float fs = 180e3f;
            const int c = 340;
            es = c / fs * 50;

            var list = readConfigration.ReadXmlFile();

            dthres = list[0];
            thres = list[1];
            det_thres = list[2];
            mv_thres = list[4];
            fb = list[5];
            ff = list[6];
            lhp = list[7];
            trough_thre = list[8];
            tail_thres = list[9];
            tail_len = list[10];
            tailn_thres = list[11];
            head_thres = list[12];
            head_len = list[13];
            headln_thres = list[14];

            ch1 = new List<short[]> {new short[2700], new short[2700], new short[2700]};
            ch2 = new List<short[]> {new short[2700], new short[2700], new short[2700]};

        }

        public Tuple<List<short[]>, List<short[]>> PrePorcessData(byte[] bytes)
        {
            ch1.RemoveAt(0);
            ch2.RemoveAt(0);

            ch1.Add(new short[2700]);
            ch2.Add(new short[2700]);

            int k = 0;

            for (int i = 0; i < bytes.Length; i = i + 4)
            {
                ch1[2][k] = (short) (BitConverter.ToUInt16(bytes, i) - 2048);
                ch2[2][k] = (short) (BitConverter.ToUInt16(bytes, i + 2) - 2048);
                k++;
            }

            return new Tuple<List<short[]>, List<short[]>>(ch1, ch2);
        }

        private float RangeDetector(IList<short> temp, int thresArg)
        {
            float[,] wi = new float[2, 3] {{0f, 0f, 0f}, {0f, 0f, 0f}};
            float[] env = new float[3] {0f, 0f, 0f};
            short temp2 = 0;

            for (int i = 156; i < 1047; i++)
            {
                //1st stage
                env[0] = env[1];
                env[1] = env[2];

                wi[0, 2] = wi[0, 1];
                wi[0, 1] = wi[0, 0];

                if (temp[i] < 0)
                {
                    temp2 = (short)(-1 * temp[i]);
                }
                else
                {
                    temp2 = temp[i];
                }

                wi[0, 0] = temp2 - a1[1] * wi[0, 1] - a1[2] * wi[0, 2];
                var y = b1[0] * wi[0, 0] + b1[1] * wi[0, 1] + b1[2] * wi[0, 2];

                //2st stage
                wi[1, 2] = wi[1, 1];
                wi[1, 1] = wi[1, 0];
                wi[1, 0] = y - a2[1] * wi[1, 1] - a2[2] * wi[1, 2];
                env[2] = b2[0] * wi[1, 0] + b2[1] * wi[1, 1] + b2[2] * wi[1, 2];

                if (env[1] <= thresArg) continue;
                if (env[1] > env[2] && env[1] > env[0])
                {
                    return i * es;
                }
            }

            return 0;
        }

        public void Detection(Tuple<List<short[]>, List<short[]>> tuple, List<float> res1, List<float> res2, List<float> r, List<float> theta)
        {
            var ch1Data = tuple.Item1;
            var ch2Data = tuple.Item2;

            var subFrame1 = new short[2700];
            Parallel.For(0, 2700, i => { subFrame1[i] = (short) (ch1Data[2][i] * 2 - ch1Data[0][i] - ch1Data[1][i]); });

            var subFrame2 = new short[2700];
            Parallel.For(0, 2700, i => { subFrame2[i] = (short) (ch2Data[2][i] * 2 - ch2Data[0][i] - ch2Data[1][i]); });

            res1.Add(RangeDetector(subFrame1, dthres));
            res2.Add(RangeDetector(subFrame2, dthres));
            var res3 = RangeDetector(ch1Data[2],thres);
            var res4 = RangeDetector(ch2Data[2],thres);

            if (res3 > 0 && res4 > 0)
            {
                r.Add((res3 + res4) / 2);
                theta.Add(res3 - res4);
            }
            else
            {
                r.Add(-1);
                theta.Add(-1);
            }
        }

        public State PresenceToNone(List<float> res1, List<float> res2)
        {
            int detNumR1 = 0;
            int detNumR2 = 0;

            Parallel.For(0, res1.Count, i =>
            {
                if (res1[i] > 0) detNumR1++;
                if (res2[i] > 0) detNumR2++;

            });

            if (detNumR1 > det_thres || detNumR2 > det_thres)
            {
                return State.SomeOne;
            }

            return State.NoOne;
        }

        public State NoneToPresence(List<float> res1, List<float> res2)
        {
            int detNumR1 = 0;
            int detNumR2 = 0;
            int flag = 0;

            Parallel.For(0, res1.Count, i =>
            {
                if (res1[i] > 0) detNumR1++;
                if (res2[i] > 0) detNumR2++;
            });

            if (detNumR1 > det_thres)
            {
                var temp1 = res1.OrderByDescending(x => x).ToList();
                double diff1 = Math.Abs(temp1[1] - temp1[detNumR1 - 2]);

                if (diff1 < mv_thres)
                {
                    flag++;
                }
            }

            if (detNumR2 > det_thres)
            {
                var temp2 = res2.OrderByDescending(x => x).ToList();
                double diff2 = Math.Abs(temp2[1] - temp2[detNumR2 - 2]);

                if (diff2 < mv_thres)
                {
                    flag++;
                }
            }

            return flag > 0 ? State.SomeOne : State.NoOne;
        }

        public List<float> Smooth(List<float> data, int times = 7)
        {
            var result = new List<float>
                {data[0], data.GetRange(0, 3).Sum() / 3, data.GetRange(0, 5).Sum() / 5};

            for (int i = 3; i < data.Count - 3; i++)
            {
                var sum = data.GetRange(i - 3, times).Sum();
                result.Add(sum / times);
            }

            result.Add(data.GetRange(data.Count - 5, 5).Sum() / 5);
            result.Add(data.GetRange(data.Count - 3, 3).Sum() / 3);
            result.Add(data[data.Count - 1]);

            return result;

        }

        public State NoneToGesture(List<float> r, List<float> theta, ref List<float> gestureOutput)
        {

            float hdiffJu = 0;

            var mean = r.GetRange(0, lhp).Sum() / lhp;

            if (r[lhp + 2] > mean - trough_thre)
            {
                return State.SomeOne;
            }

            var gestureSmooth = Smooth(r).GetRange(lhp + 2, fb + ff + 1);

            for (int i = 0; i < head_len; i++)
            {
                hdiffJu += ((gestureSmooth[i] - gestureSmooth[i + 1]) > head_thres ? 1 : 0);
            }

            if (hdiffJu >= headln_thres)
            {
                for (int i = fb + ff + 1; i > tail_len; i--)
                {
                    var tail = new List<float>(gestureSmooth.GetRange(i - tail_len - 1, tail_len + 1));

                    float tdiffJu = 0;

                    for (int j = 0; j < tail_len; j++)
                    {
                        tdiffJu += ((tail[j + 1] - tail[j]) > tail_thres ? 1 : 0);
                    }

                    if (tdiffJu >= tailn_thres)
                    {
                        int pp = 0;
                        int np = 0;

                        var gestT = new List<float>(theta.GetRange(1, i - 1));
                        gestureOutput = gestureSmooth.GetRange(1, i - 1);

                        for (int k = 2; k < i - 2; k++)
                        {
                            if (gestureSmooth[k] >= gestureSmooth[k + 1] &&
                                gestureSmooth[k] >= gestureSmooth[k - 1])
                            {
                                pp++;
                            }
                            else if (gestureSmooth[k] <= gestureSmooth[k + 1] &&
                                     gestureSmooth[k] <= gestureSmooth[k - 1])
                            {
                                np++;
                            }
                        }

                        return GestureKind(pp, np, gestT);
                    }
                }
            }

            return State.SomeOne;
        }

        public State GestureKind(int pp, int np, List<float> gestT)
        {

            switch (pp)
            {
                case 1 when np == 2:
                    return State.DoubleClick;

                case 0 when np == 1:
                {
                    var maxGesture = gestT.Max();
                    var minGesture = gestT.Min();

                    var maxIdx = gestT.IndexOf(maxGesture);
                    var minIdx = gestT.IndexOf(minGesture);

                    if (maxGesture > 0 && minGesture < 0)
                    {
                        return maxIdx > minIdx ? State.LeftSweep : State.RightSweep;
                    }

                    break;
                }

                default:
                {
                    if (pp >= 2 && np >= 2)
                    {
                        return State.Circle;
                    }

                    break;
                }
            }

            return State.OtherGesture;
        }

    }

    
}
