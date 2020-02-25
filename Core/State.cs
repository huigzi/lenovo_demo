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
            musicPlayer = new MusicPlayer($"Music" + "/" + mainViewModel.MusicModels.ToList()[1].MusicName + ".wav");
            this.mainViewModel = mainViewModel;
            this.mainViewModel.Volume = 0.2f;
            lastState = State.SomeOne;
        }

        public void StateChanged(State state)
        {
            if (state == lastState) return;

            mainViewModel.BackGround = (int)state;

            switch (state)
            {
                case State.DoubleClickMiddle:

                    mainViewModel.Status = $"中间双击";
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
                            musicPlayer.Play($"Music" + "/" + mainViewModel.MusicModels.ToList()[1].MusicName + ".wav");
                            musicPlayer.CurrentStatus = PlayerState.Start;
                            break;
                        default:
                            break;
                    }
                    break;

                case State.DoubleClickLeft:
                    mainViewModel.Status = $"左边双击";
                    mainViewModel.Volume = musicPlayer.DownVolume();
                    break;

                case State.DoubleClickRight:
                    mainViewModel.Status = $"右边双击";
                    mainViewModel.Volume = musicPlayer.UpVolume();
                    break;

                case State.LeftSweep:

                    mainViewModel.Status = $"左滑";
                    mainViewModel.ScrollLeft();

                    switch (musicPlayer.CurrentStatus)
                    {
                        case PlayerState.Start:
                            musicPlayer.Stop();
                            musicPlayer.Play($"Music" + "/" + mainViewModel.MusicModels.ToList()[1].MusicName + ".wav");
                            break;
                        case PlayerState.Pause:
                            musicPlayer.Stop();
                            musicPlayer.CurrentStatus = PlayerState.Stop;
                            break;
                    }
                    break;

                case State.RightSweep:

                    mainViewModel.Status = $"右划";
                    mainViewModel.ScrollRight();

                    switch (musicPlayer.CurrentStatus)
                    {
                        case PlayerState.Start:
                            musicPlayer.Stop();
                            musicPlayer.Play($"Music" + "/" + mainViewModel.MusicModels.ToList()[1].MusicName + ".wav");
                            break;
                        case PlayerState.Pause:
                            musicPlayer.Stop();
                            musicPlayer.CurrentStatus = PlayerState.Stop;
                            break;
                    }
                    break;

                case State.OtherGesture:
                    mainViewModel.Status = $"其它类型";
                    break;

                default:
                    break;
            }

            lastState = state;
        }
    }
}
