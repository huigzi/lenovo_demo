using System.Linq;
using Core.ViewModel;

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

    public enum FindChannelNum
    {
        None = 0,
        Channel1Start = 1,
        Channel2Start = 2,
        Channel1End = 3,
        Channel2End = 4
    }


    public class StatusMachine
    {
        private State lastState;

        private readonly MusicPlayer musicPlayer;
        private readonly MainViewModel mainViewModel;

        public StatusMachine(MainViewModel mainViewModel)
        {
            musicPlayer = new MusicPlayer($"Music" + "/" + mainViewModel.MusicModels.ToList()[1].MusicName);
            this.mainViewModel = mainViewModel;
            this.mainViewModel.Volume = 0.2f;
            lastState = State.SomeOne;
        }

        public void StateChanged(State state)
        {
            if (state == lastState) return;

            switch (state)
            {
                case State.DoubleClickMiddle:

                    switch (musicPlayer.CurrentStatus)
                    {
                        case PlayerState.Start:
                            musicPlayer.Pause();
                            musicPlayer.CurrentStatus = PlayerState.Pause;
                            break;
                        case PlayerState.Pause:
                            musicPlayer.Resume();
                            musicPlayer.CurrentStatus = PlayerState.Start;
                            break;
                        case PlayerState.Stop:
          
                            musicPlayer.Play($"Music" + "/" + mainViewModel.MusicModels.ToList()[1].MusicName);
                            musicPlayer.CurrentStatus = PlayerState.Start;
                            break;
                        default:
                            break;
                    }
                    break;

                case State.Circle:
                    mainViewModel.Volume = musicPlayer.UpdateVolume();
                    break;

                case State.LeftSweep:

                    mainViewModel.ScrollLeft();

                    switch (musicPlayer.CurrentStatus)
                    {
                        case PlayerState.Start:
                            musicPlayer.Stop();
                            musicPlayer.Play($"Music" + "/" + mainViewModel.MusicModels.ToList()[1].MusicName);
                            break;
                        case PlayerState.Pause:
                            musicPlayer.Stop();
                            musicPlayer.CurrentStatus = PlayerState.Stop;
                            break;
                    }
                    break;

                case State.RightSweep:

                    mainViewModel.ScrollRight();

                    switch (musicPlayer.CurrentStatus)
                    {
                        case PlayerState.Start:
                            musicPlayer.Stop();
                            musicPlayer.Play($"Music" + "/" + mainViewModel.MusicModels.ToList()[1].MusicName);
                            break;
                        case PlayerState.Pause:
                            musicPlayer.Stop();
                            musicPlayer.CurrentStatus = PlayerState.Stop;
                            break;
                    }
                    break;

                case State.OtherGesture:
                    break;

                default:
                    break;
            }

            lastState = state;
        }
    }
}
