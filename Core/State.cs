using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public enum State
    {
        NoOne = 0,
        DoubleClickLeft = 1,
        LeftSweep = 2,
        RightSweep = 3,
        Circle = 4,
        SomeOne = 5,
        OtherGesture = 6,
        DoubleClickRight = 7,
        DoubleClickMiddle = 8,
        TestFlag = 9
    }

    public enum PlayerState
    {
        Stop = 0,
        Start = 1,
        Pause = 2
    }

    public class StatusMachine
    {
        public State LastState { get; set; }
        public StatusMachine() => LastState = State.SomeOne;
    }

}
