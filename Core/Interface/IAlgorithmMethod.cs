using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace Core.Interface
{
    public interface IAlgorithmMethod<T> where T : struct
    {
        int TrackCount { get; set; }
        int IntervalFlag { get; set; }

        (short[], short[]) Byte2Int16(byte[] bytes);

        (short[], short[]) Int16PreProcess((short[], short[]) data);

        void Initial(byte[] bytes);

        State PresenceToNone(List<T> r1, List<T> r2);

        State NoneToPresence(List<T> r1, List<T> r2);

        int FindTrackStartPoint(List<T> sbd1, List<T> sbd2, ref int k1, ref int k2);

        Option<(List<T>, List<T>)> TrackComplete(List<T> sbd1, List<T> sbd2, ref int k1, ref int k2);

        Option<(List<T>, List<T>)> CheckInterval((List<float>, List<float>) data);

        Option<(int, FindChannelNum, List<T>, List<T>)> FindGestureStartPoint(
            (List<T>, List<T>) trackline);

        Option<(FindChannelNum, List<T>, List<T>)> FindGestureStopPoint(
            (int, FindChannelNum, List<T>, List<T>) data);

        State GestureKind((FindChannelNum, List<T>, List<T>) data);
    }
}
