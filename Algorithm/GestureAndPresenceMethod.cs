using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Interface;
using LanguageExt;

namespace Algorithm
{
    public class GestureAndPresenceMethod : IAlgorithmMethod<float>
    {
        private readonly int pdn_thre1;
        private readonly int pdn_thre2;
        private readonly int migr_thre1;
        private readonly int migr_thre2;
        private readonly int np_count_thre;

        private readonly int tr_thremi;
        private readonly int init_len;
        private readonly float eu;
        private readonly int track_len ;
        private readonly int gatel;
        private readonly int gateu;
        private readonly int tr_threm0;
        private readonly int thre2;
        private readonly int int1_dot;
        private readonly int int2_dot;
        private readonly float alpha;
        private readonly int lost_thre;
        private readonly int ref_lenl;
        private readonly int fp;
        private readonly int fronts;
        private readonly int backs;
        private readonly int gest_thre;
        private readonly float ndiff_thre;
        private readonly int diff_win;
        private readonly int dthre;
        private readonly int dev_thres;
        private readonly int dev_thref;
        private readonly int non_thre;
        private readonly int backd;
        private readonly int dx;
        private readonly float R;
        private readonly float dc_thre;
        private readonly float pdiff_thre;
        private int thre1;

        private int npCount = 0;
        public int TrackCount { get; set; } = 0;
        public int IntervalFlag { get; set; } = 0;
        private int lostCount = 0;

        private readonly float[] gest1Ref =
        {
            1.000000000000000f, 0.993844170297569f, 0.975528258147577f, 0.945503262094184f, 0.904508497187474f,
            0.853553390593274f, 0.793892626146237f, 0.726995249869773f, 0.654508497187474f, 0.578217232520115f,
            0.500000000000000f, 0.421782767479885f, 0.345491502812526f, 0.273004750130227f, 0.206107373853763f,
            0.146446609406726f, 0.095491502812526f, 0.054496737905816f, 0.024471741852423f, 0.006155829702431f,
            0.000000000000000f, 0.006155829702431f, 0.024471741852423f, 0.054496737905816f, 0.095491502812526f,
            0.146446609406726f, 0.206107373853763f, 0.273004750130227f, 0.345491502812526f, 0.421782767479885f,
            0.500000000000000f, 0.578217232520115f, 0.654508497187474f, 0.726995249869773f, 0.793892626146236f,
            0.853553390593274f, 0.904508497187474f, 0.945503262094184f, 0.975528258147577f, 0.993844170297569f,
            1.000000000000000f
        };

        private readonly float[] gest2Ref =
        {
            1.000000000000000f, 0.972370634160430f, 0.904846342427666f, 0.807736391220236f, 0.717091339858728f,
            0.610645285344996f, 0.495086624765342f, 0.377676333759009f, 0.265791732714771f, 0.166462942599017f,
            0.085931156600455f, 0.029256483008567f, 0.000000000000000f, 0.000000000000000f, 0.029256483008567f,
            0.085931156600455f, 0.166462942599017f, 0.246994728597579f, 0.322767596235473f, 0.389321647494712f,
            0.442776236494811f, 0.480073804745482f, 0.499171998791488f, 0.499171998791488f, 0.499171998791488f,
            0.499171998791488f, 0.499171998791488f, 0.499171998791488f, 0.499171998791488f, 0.480073804745482f,
            0.442776236494811f, 0.389321647494711f, 0.322767596235473f, 0.246994728597578f, 0.166462942599017f,
            0.085931156600455f, 0.029256483008567f, 0.000000000000000f, 0.000000000000000f, 0.029256483008567f,
            0.085931156600455f, 0.166462942599017f, 0.265791732714771f, 0.377676333759009f, 0.495086624765341f,
            0.610645285344996f, 0.717091339858728f, 0.807736391220236f, 0.904846342427665f, 0.972370634160430f,
            1.000000000000000f
        };

        List<short[]> ch1;
        List<short[]> ch2;
        List<float> trackingLine1;
        List<float> trackingLine2;

        private enum GestureLikely
        {
            Sweep = 0,
            DoubleClick = 1,
            Indistinguishable = 2,
            None = 4
        }

        public GestureAndPresenceMethod(IReadFile readConfigrationFile)
        {
            var configurationData = readConfigrationFile.ReadFile();

            pdn_thre1 = configurationData.ConfigurationGroupInt["pdn_thre1"];
            pdn_thre2 = configurationData.ConfigurationGroupInt["pdn_thre2"]; 
            migr_thre1 = configurationData.ConfigurationGroupInt["migr_thre1"];
            migr_thre2 = configurationData.ConfigurationGroupInt["migr_thre2"];
            np_count_thre = configurationData.ConfigurationGroupInt["np_count_thre"];
            tr_thremi = configurationData.ConfigurationGroupInt["tr_thremi"]; 
            init_len = configurationData.ConfigurationGroupInt["init_len"]; 
            eu = configurationData.ConfigurationGroupFloat["eu"];
            track_len = configurationData.ConfigurationGroupInt["track_len"]; 
            gatel = configurationData.ConfigurationGroupInt["gatel"];
            gateu = configurationData.ConfigurationGroupInt["gateu"];
            tr_threm0 = configurationData.ConfigurationGroupInt["tr_threm0"]; 
            thre2 = configurationData.ConfigurationGroupInt["thre2"]; 
            int1_dot = configurationData.ConfigurationGroupInt["int1_dot"];
            int2_dot = configurationData.ConfigurationGroupInt["int2_dot"];
            alpha = configurationData.ConfigurationGroupFloat["alpha"];
            lost_thre = configurationData.ConfigurationGroupInt["lost_thre"];
            ref_lenl = configurationData.ConfigurationGroupInt["ref_lenl"];
            fp = configurationData.ConfigurationGroupInt["fp"];
            fronts = configurationData.ConfigurationGroupInt["fronts"];
            backs = configurationData.ConfigurationGroupInt["backs"];
            gest_thre = configurationData.ConfigurationGroupInt["gest_thre"];
            ndiff_thre = configurationData.ConfigurationGroupFloat["ndiff_thre"];
            diff_win = configurationData.ConfigurationGroupInt["diff_win"];
            dthre = configurationData.ConfigurationGroupInt["dthre"];
            dev_thres = configurationData.ConfigurationGroupInt["dev_thres"];
            dev_thref = configurationData.ConfigurationGroupInt["dev_thref"];
            non_thre = configurationData.ConfigurationGroupInt["non_thre"];
            backd = configurationData.ConfigurationGroupInt["backd"];
            dx = configurationData.ConfigurationGroupInt["dx"];


            R = configurationData.ConfigurationGroupFloat["R"];
            dc_thre = configurationData.ConfigurationGroupFloat["dc_thre"];
            pdiff_thre = configurationData.ConfigurationGroupFloat["pdiff_thre"];

            ch1 = new List<short[]> {new short[1400], new short[1400]};
            ch2 = new List<short[]> {new short[1400], new short[1400]};
            trackingLine1 = new List<float>();
            trackingLine2 = new List<float>();

            for (int i = 0; i < track_len; i++)
            {
                trackingLine1.Add(0);
                trackingLine2.Add(0);
            }

            thre1 = tr_thremi;
        }

        public void Initial((short[], short[]) data)
        {
            ch1.Add(data.Item1);
            ch2.Add(data.Item2);

            ch1.RemoveAt(0);
            ch2.RemoveAt(0);
        }

        public void Initial(byte[] bytes)
        {
            ch1.Add(new short[1400]);
            ch2.Add(new short[1400]);

            int k = 0;

            for (int i = 0; i < bytes.Length; i = i + 4)
            {
                ch1[2][k] = (short)BitConverter.ToUInt16(bytes, i);
                ch2[2][k] = (short)BitConverter.ToUInt16(bytes, i + 2);
                k++;
            }

            ch1.RemoveAt(0);
            ch2.RemoveAt(0);
        }

        public (short[], short[]) Byte2Int16(byte[] bytes)
        {
            ch1.Add(new short[1400]);
            ch2.Add(new short[1400]);

            int k = 0;

            for (int i = 0; i < bytes.Length; i = i + 4)
            {
                ch1[2][k] = (short)BitConverter.ToUInt16(bytes, i);
                ch2[2][k] = (short)BitConverter.ToUInt16(bytes, i + 2);
                k++;
            }

            var subFrame1 = new short[1400];
            Parallel.For(0, subFrame1.Length, i => { subFrame1[i] = (short)(ch1[2][i] - ch1[0][i]); });

            var subFrame2 = new short[1400];
            Parallel.For(0, subFrame2.Length, i => { subFrame2[i] = (short)(ch2[2][i] - ch2[0][i]); });

            ch1.RemoveAt(0);
            ch2.RemoveAt(0);

            return (subFrame1, subFrame2);
        }

        public (short[], short[]) Int16PreProcess((short[], short[]) data)
        {

            ch1.Add(data.Item1);
            ch2.Add(data.Item2);

            var subFrame1 = new short[1400];
            Parallel.For(0, subFrame1.Length, i => { subFrame1[i] = (short) (ch1[2][i] - ch1[0][i]); });

            var subFrame2 = new short[1400];
            Parallel.For(0, subFrame2.Length, i => { subFrame2[i] = (short) (ch2[2][i] - ch2[0][i]); });

            ch1.RemoveAt(0);
            ch2.RemoveAt(0);

            return (subFrame1, subFrame2);
        }

        public virtual (short[], short[]) TransFormMatData((short[], short[]) data)
        {
            ch1.Add(data.Item1);
            ch2.Add(data.Item2);

            var subFrame1 = new short[1400];
            Parallel.For(0, subFrame1.Length, i => { subFrame1[i] = (short)(ch1[2][i] - ch1[0][i]); });

            var subFrame2 = new short[1400];
            Parallel.For(0, subFrame2.Length, i => { subFrame2[i] = (short)(ch2[2][i] - ch2[0][i]); });

            ch1.RemoveAt(0);
            ch2.RemoveAt(0);

            return (subFrame1, subFrame2);
        }

        public State PresenceToNone(List<float> r1, List<float> r2)
        {
            var tempResult1 = r1.FindMt();
            var tempResult2 = r2.FindMt();

            if (tempResult1.Item1 < pdn_thre1 && tempResult2.Item1 < pdn_thre2)
            {
                npCount++;
                if (npCount <= np_count_thre - 1) return State.SomeOne;
                npCount = 0;
                return State.NoOne;
            }
            
            npCount = 0;
            return State.SomeOne;
        }

        public State NoneToPresence(List<float> r1, List<float> r2)
        {
            var tempResult1 = r1.FindMt();
            var tempResult2 = r2.FindMt();

            float sum1 = 0, sum2 = 0;

            if (tempResult1.Item1 > pdn_thre1)
            {
                var temp = tempResult1.Item2.OrderByDescending(x => x).ToList();

                if (temp[1] - temp[temp.Count - 2] < migr_thre1)
                {
                    sum1 = temp[1];
                }
            }

            if (tempResult2.Item1 > pdn_thre2)
            {
                var temp = tempResult2.Item2.OrderByDescending(x => x).ToList();

                if (temp[1] - temp[temp.Count - 2] < migr_thre2)
                {
                    sum2 = temp[1];
                }
            }

            if (sum1 > 0 || sum2 > 0)
            {
                return State.SomeOne;
            }
            else
            {
                return State.NoOne;
            }
        }

        public int FindTrackStartPoint(List<float> sbd1, List<float> sbd2, ref int k1, ref int k2)
        {

            if (k1 < int1_dot - 1)
            {
                k1 = int1_dot - 1;
            }
            if (k2 > int2_dot - 1)
            {
                k2 = int2_dot - 1;
            }

            var result1 = sbd2.CalculateR(thre1, eu, k1, k2);

            if (result1 > 0)
            {
                TrackCount++;
                trackingLine1.Add(result1);
                trackingLine1.RemoveAt(0);


                k1 = (int) Math.Ceiling((result1 - gatel) / eu);
                k2 = (int) Math.Ceiling((result1 + gateu) / eu);

                thre1 = tr_threm0;

                var result2 = sbd1.CalculateR(thre2, eu, int1_dot, int2_dot);
                trackingLine2.Add(result2);
                trackingLine2.RemoveAt(0);

                if (TrackCount == init_len)
                {
                    lostCount = 0;

                }

                return TrackCount;
            }

            TrackCount = 0;
            thre1 = tr_thremi;
            return 0;
        }

        public Option<(List<float>, List<float>)> TrackComplete(List<float> sbd1, List<float> sbd2, ref int k1, ref int k2)
        {
            if (k1 < int1_dot - 1)
            {
                k1 = int1_dot - 1;
            }
            if (k2 > int2_dot - 1)
            {
                k2 = int2_dot - 1;
            }

            var result1 = sbd2.CalculateR(thre1, eu, k1, k2);

            if (result1 > 0)
            {
                TrackCount++;

                trackingLine1.Add(result1);
                trackingLine1.RemoveAt(0);

                k1 = (int) Math.Ceiling((result1 - gatel) / eu);
                k2 = (int) Math.Ceiling((result1 + gateu) / eu);

                lostCount = 0;
            }
            else
            {
                lostCount++;

                result1 = trackingLine1[trackingLine1.Count - 1] * (1 + alpha) -
                          alpha * trackingLine1[trackingLine1.Count - 2];

                TrackCount++;

                trackingLine1.Add(result1);
                trackingLine1.RemoveAt(0);

                k1 = (int) Math.Ceiling((result1 - gatel) / eu);
                k2 = (int) Math.Ceiling((result1 + gateu) / eu);

                if (lostCount > lost_thre)
                {
                    TrackCount = 0;
                    lostCount = 0;
                    k1 = int1_dot;
                    k2 = int2_dot;

                    var result2 = sbd1.CalculateR(thre2, eu, int1_dot, int2_dot);

                    trackingLine2.Add(result2);
                    trackingLine2.RemoveAt(0);

                    return Option<(List<float>, List<float>)>.None;
                }
            }

            var result3 = sbd1.CalculateR(thre2, eu, int1_dot, int2_dot);

            trackingLine2.Add(result3);
            trackingLine2.RemoveAt(0);

            return TrackCount > track_len ? Option<(List<float>, List<float>)>.Some((trackingLine1, trackingLine2)) : Option<(List<float>, List<float>)>.None;
        }

        public Option<(List<float>, List<float>)> CheckInterval((List<float>, List<float>) data)
        {
            if (IntervalFlag == 0) return data;

            IntervalFlag++;

            if (IntervalFlag > 60)
            {
                IntervalFlag = 0;
            }

            return Option<(List<float>, List<float>)>.None;
        }

        public Option<(int, FindChannelNum, List<float>, List<float>)> FindGestureStartPoint((List<float>, List<float>) trackline)
        {
            if (trackline.Item2[0] == 0)
            {
                trackline.Item2[0] = trackline.Item2[1];
            }

            if (trackline.Item2[trackline.Item2.Count - 1] == 0)
            {
                trackline.Item2[trackline.Item2.Count - 1] = trackline.Item2[trackline.Item2.Count - 2];
            }

            for (int i = 2; i < trackline.Item2.Count - 1; i++)
            {
                if (trackline.Item2[i] == 0)
                {
                    trackline.Item2[i] = trackline.Item2[i - 1] * (1 + alpha) - alpha * trackline.Item2[i - 2];
                }
            }

            var medianFilterResult1 = trackline.Item1.MedianFilter(fp);
            var medianFilterResult2 = trackline.Item2.MedianFilter(fp);

            var med1 = medianFilterResult1.OrderBy(x => x).ToList()[medianFilterResult1.Count() / 2];
            var min1 = medianFilterResult1.Min();

            if (med1 - min1 > 10)
            {
                for(int i = 0; i < medianFilterResult1.Count; i++)
                {
                    if (medianFilterResult1[i] > med1 + dx)
                    {
                        medianFilterResult1[i] = med1 + dx;
                    }
                }
            }

            var med2 = medianFilterResult2.OrderBy(x => x).ToList()[medianFilterResult2.Count() / 2];
            var min2 = medianFilterResult2.Min();

            if (med2 - min2 > 10)
            {
                for (int i = 0; i < medianFilterResult2.Count; i++)
                {
                    if (medianFilterResult2[i] > med2 + dx)
                    {
                        medianFilterResult2[i] = med2 + dx;
                    }
                }
            }

            var track1Mf = medianFilterResult1.GetRange(fp, track_len - 2 * fp);
            var track2Mf = medianFilterResult1.GetRange(fp, track_len - 2 * fp);


            var rref1Temp = track1Mf.GetRange(0, ref_lenl).OrderByDescending(x => x).ToArray();

            if (rref1Temp[1] - rref1Temp[rref1Temp.Length - 2] < dev_thres)
            {
                var rref1 = rref1Temp[ref_lenl / 2];

                for (int i = ref_lenl + 1; i < ref_lenl + fronts; i++)
                {
                    if (!(track1Mf[i] < rref1 - gest_thre)) continue;

                    var temp1 = track1Mf.GetRange(i - 4, 6 + diff_win);

                    var resultFlag = new List<int>();

                    for (int j = 0; j < temp1.Count - 1; j++)
                    {
                        resultFlag.Add(temp1[j + 1] - temp1[j] < ndiff_thre ? 1 : 0);
                    }

                    for (int j = 0; j < 6; j++)
                    {
                        var sum = resultFlag.GetRange(j, diff_win).Sum();

                        if (sum >= diff_win - dthre)
                        {
                            return Option<(int, FindChannelNum, List<float>, List<float>)>.Some((i + j - 8,
                                FindChannelNum.Channel1Start, track1Mf, track2Mf));
                        }

                    }
                }
            }

            var rref2Temp = track2Mf.GetRange(0, ref_lenl).OrderByDescending(x => x).ToArray();

            if (rref2Temp[1] - rref2Temp[rref2Temp.Length - 2] < dev_thres)
            {
                var rref2 = rref2Temp[ref_lenl / 2];

                for (int i = ref_lenl + 1; i < ref_lenl + fronts; i++)
                {
                    if (!(track2Mf[i] < rref2 - gest_thre)) continue;

                    var temp2 = track2Mf.GetRange(i - 4, 6 + diff_win);

                    var resultFlag = new List<int>();

                    for (int j = 0; j < temp2.Count - 1; j++)
                    {
                        resultFlag.Add(temp2[j + 1] - temp2[j] < ndiff_thre ? 1 : 0);
                    }

                    for (int j = 0; j < 6; j++)
                    {
                        var sum = resultFlag.GetRange(j, diff_win).Sum();

                        if (sum >= diff_win - dthre)
                        {
                            return Option<(int, FindChannelNum, List<float>, List<float>)>.Some((i + j - 8,
                                FindChannelNum.Channel2Start, track1Mf, track2Mf));
                        }

                    }
                }
            }

            return Option<(int, FindChannelNum, List<float>, List<float>)>.None;
        }

        public Option<(FindChannelNum, List<float>, List<float>)> FindGestureStopPoint((int, FindChannelNum, List<float>, List<float>) data)
        {
            int bulgeDet = 0;

            if (data.Item2 == FindChannelNum.Channel1Start && bulgeDet == 0)
            {
                var track1Mf = data.Item3.ToList();

                for (int k = track1Mf.Count(); k > track1Mf.Count() - backd; k--)
                {
                    var rref1Temp = track1Mf.GetRange(k - ref_lenl - 1, ref_lenl + 1).OrderByDescending(x => x).ToArray();

                    if (!(rref1Temp[1] - rref1Temp[rref1Temp.Length - 2] < dev_thref)) continue;

                    var rref1 = rref1Temp[ref_lenl / 2];

                    for (int i = k - ref_lenl - 1; i > k - ref_lenl - backs; i--)
                    {
                        if (!(track1Mf[i] < rref1 - gest_thre)) continue;

                        var temp1 = track1Mf.GetRange(i - diff_win + 1, diff_win + 1);

                        var resultFlag = new List<int>();

                        for (int j = 0; j < temp1.Count - 1; j++)
                        {
                            resultFlag.Add(temp1[j + 1] - temp1[j] > pdiff_thre ? 1 : 0);
                        }

                        var sum = resultFlag.Sum();

                        if (sum < diff_win - dthre) continue;

                        var result1 = data.Item3.GetRange(data.Item1, i + 5 - data.Item1 + 1);
                        var result2 = data.Item4.GetRange(data.Item1, i + 5 - data.Item1 + 1);

                        return Option<(FindChannelNum, List<float>, List<float>)>.Some((FindChannelNum.Channel1End, result1, result2));
                    }

                }

            }

            if (data.Item2 == FindChannelNum.Channel2Start && bulgeDet == 0)
            {
                var track2Mf = data.Item4.ToList();

                for (int k = track2Mf.Count(); k > track2Mf.Count() - backd; k--)
                {
                    var rref2Temp = track2Mf.GetRange(k - ref_lenl - 1, ref_lenl + 1).OrderByDescending(x => x).ToArray();

                    if (!(rref2Temp[1] - rref2Temp[rref2Temp.Length - 2] < dev_thref)) continue;

                    var rref2 = rref2Temp[ref_lenl / 2];

                    for (int i = k - ref_lenl - 1; i > k - ref_lenl - backs; i--)
                    {
                        if (!(track2Mf[i] < rref2 - gest_thre)) continue;

                        var temp2 = track2Mf.GetRange(i - diff_win + 1, diff_win + 1);

                        var resultFlag = new List<int>();

                        for (int j = 0; j < temp2.Count - 1; j++)
                        {
                            resultFlag.Add(temp2[j + 1] - temp2[j] > pdiff_thre ? 1 : 0);
                        }


                        var sum = resultFlag.Sum();

                        if (sum < diff_win - dthre) continue;

                        var result1 = data.Item3.GetRange(data.Item1, i + 5 - data.Item1 + 1);
                        var result2 = data.Item4.GetRange(data.Item1, i + 5 - data.Item1 + 1);

                        return Option<(FindChannelNum, List<float>, List<float>)>.Some((FindChannelNum.Channel2End, result1, result2));
                    }
                }
            }

            return Option<(FindChannelNum, List<float>, List<float>)>.None;
        }

        public State GestureKind((FindChannelNum, List<float>, List<float>) data)
        {
            var gesture = new List<float>();

            switch (data.Item1)
            {
                case FindChannelNum.Channel1End:
                    gesture.AddRange(data.Item2);
                    break;
                case FindChannelNum.Channel2End:
                    gesture.AddRange(data.Item3);
                    break;
                default:
                    return State.SomeOne;
            }

            var temp1 = gesture.Normalize();

            var s1 = temp1.Dtw(gest1Ref);
            var s2 = temp1.Dtw(gest2Ref);

            GestureLikely temp2;

            if (s1 >= non_thre && s2 >= non_thre)
            {
                return State.SomeOne;
            }

            if (s1 < s2)
            {
                temp2 = GestureLikely.Sweep;
            }
            else if (s1 > s2)
            {
                temp2 = GestureLikely.DoubleClick;
            }
            else
            {
                temp2 = GestureLikely.Indistinguishable;
            }

            var track1F = data.Item2;
            var track2F = data.Item3;

            var x = track1F.Select((t, i) => track2F[i] * (t - track2F[i]) / R).ToList();

            if (temp2 == GestureLikely.DoubleClick)
            {
                var nbz = x.Count(item => item > 0);

                if (nbz > dc_thre * track1F.Count)
                {
                    return State.DoubleClickRight;
                }
                else if (nbz < (1 - dc_thre) * track1F.Count)
                {
                    return State.DoubleClickLeft;
                }
                else
                {
                    return State.DoubleClickMiddle;
                }
            }
            else if (temp2 == GestureLikely.Sweep)
            {
                var leftPart = x.Take(x.Count / 2).OrderBy(item => item).ToArray();
                var rightPart = x.Skip(x.Count / 2).OrderBy(item => item).ToArray();

                var ml = leftPart[leftPart.Length / 2];
                var mr = rightPart[rightPart.Length / 2];

                return ml > mr ? State.LeftSweep : State.RightSweep;

            }

            return State.SomeOne;

        }
    }

    
}
