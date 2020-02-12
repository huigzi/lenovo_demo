using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace Algorithm
{
    public static class BasicMethod
    {
        private static readonly float[] ABp1 = {1f, -0.090292374147464f, 0.777012688479490f};
        private static readonly float[] ABp2 = {1f, -0.529211627791274f, 0.785710792359040f};

        private static readonly float[] BBp1 = {0.155824798862912f, -0.311649597725824f, 0.155824798862912f};
        private static readonly float[] BBp2 = {0.155824798862912f, 0.311649597725824f, 0.155824798862912f};

        private static readonly float[] ALp1 = {1f, -1.874333706443614f, 0.878910638325473f};
        private static readonly float[] ALp2 = {1f, -1.943253631193138f, 0.947998858506101f};

        private static readonly float[] BLp1 = {0.001165079991193f, 0.002330160021464f, 0.001165079974324f};
        private static readonly float[] BLp2 = {0.001165079991193f, 0.002330159943306f, 0.001165080008061f};

        public static List<float> BandPassFilter(this short[] data)
        {
            float[,] wi = {{0, 0, 0}, {0, 0, 0}};
            var result = new List<float>();

            foreach (var item in data)
            {
                //1st stage
                wi[0, 2] = wi[0, 1];
                wi[0, 1] = wi[0, 0];
                wi[0, 0] = item - ABp1[1] * wi[0, 1] - ABp1[2] * wi[0, 2];

                var y = BBp1[0] * wi[0, 0] + BBp1[1] * wi[0, 1] + BBp1[2] * wi[0, 2];

                //2st stage
                wi[1, 2] = wi[1, 1];
                wi[1, 1] = wi[1, 0];
                wi[1, 0] = y - ABp2[1] * wi[1, 1] - ABp2[2] * wi[1, 2];

                result.Add(Math.Abs(BBp2[0] * wi[1, 0] + BBp2[1] * wi[1, 1] + BBp2[2] * wi[1, 2]));
            }

            return result;
        }

        public static List<float> LowPassFilter(this List<float> data)
        {
            float[,] wi = { { 0, 0, 0 }, { 0, 0, 0 } };
            var result = new List<float>();

            foreach (var item in data)
            {
                //1st stage
                wi[0, 2] = wi[0, 1];
                wi[0, 1] = wi[0, 0];
                wi[0, 0] = item - ALp1[1] * wi[0, 1] - ALp1[2] * wi[0, 2];

                var y = BLp1[0] * wi[0, 0] + BLp1[1] * wi[0, 1] + BLp1[2] * wi[0, 2];

                //2st stage
                wi[1, 2] = wi[1, 1];
                wi[1, 1] = wi[1, 0];
                wi[1, 0] = y - ALp2[1] * wi[1, 1] - ALp2[2] * wi[1, 2];

                result.Add(Math.Abs(BLp2[0] * wi[1, 0] + BLp2[1] * wi[1, 1] + BLp2[2] * wi[1, 2]));
            }

            return result;
        }

        public static float CalculateR(this List<float> data, int pdThread, float eu, int startPosition, int stopPosition)
        {

            for (int i = startPosition; i < stopPosition; i++)
            {
                if (!(data[i] >= data[i - 1])) continue;
                if (!(data[i] >= data[i + 1])) continue;
                if (data[i] >= pdThread)
                {
                    return (i + 1) * eu;
                }
            }

            return 0;
        }

        public static List<float> MedianFilter(this List<float> data, int windowLength)
        {
            var result = new List<float>();
            var temp = new List<float>();

            for (int i = 0; i < windowLength; i++)
            {
                temp.Add(0);
            }

            temp.AddRange(data);

            for (int i = 0; i < windowLength; i++)
            {
                temp.Add(0);
            }

            for (int i = windowLength; i < windowLength + data.Count; i++)
            {
                var temp2 =
                    temp.GetRange(i - windowLength, 2 * windowLength + 1).OrderByDescending(x => x).ToArray()[windowLength + 1];
                result.Add(temp2);
            }

            return result;

        }

        public static (int, List<float>) FindMt(this List<float> r)
        {
            int tempResult1 = 0;
            var tempResult2 = new List<float>();

            foreach (var t in r)
            {
                if (!(t > 0)) continue;
                tempResult1++;
                tempResult2.Add(t);
            }

            return (tempResult1, tempResult2);
        }

        public static List<float> Normalize(this List<float> data)
        {
            var max = data.Max();
            var min = data.Min();
            var mean = data.Average();

            var temp = data.Select(x => x - mean).Select(x => x / (max - min)).Select(x => x - min).ToList();

            return temp;
        }

        public static float Dtw(this List<float> data, float[] reference)
        {
            var d = new List<float[]>();

            for (int i = 0; i < data.Count; i++)
            {
                d.Add(new float[reference.Length]);
            }

            d[0][0] = Math.Abs(data[0] - reference[0]);

            for (int i = 1; i < data.Count; i++)
            {
                d[i][0] = d[i - 1][0] + Math.Abs(data[i] - reference[0]);
            }

            for (int i = 1; i < reference.Length; i++)
            {
                d[0][i] = d[0][i - 1] + Math.Abs(data[0] - reference[i]);
            }

            for (int i = 1; i < data.Count; i++)
            {
                for (int j = 1; j < reference.Length; j++)
                {
                    var resultTemp = new List<float> {d[i - 1][j], d[i][j - 1], d[i - 1][j - 1]};

                    d[i][j] = resultTemp.Min() + Math.Abs(data[i] - reference[j]);
                }
            }

            return d[data.Count - 1][reference.Length - 1];

        }

    }
}
