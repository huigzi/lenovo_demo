using Core.ViewModel;
using NAudio.Wave;
using System.Linq;

namespace Core
{
    public class MusicPlayer
    {
        private WaveOut wavePlay;
        private MainViewModel viewModel;
        public int Index { get; set; }
        public PlayerState CurrentStatus { get; set; }
        public float Volume { get; set; }

        public MusicPlayer(MainViewModel mainViewModel)
        {
            viewModel = mainViewModel;
            wavePlay = new WaveOut { Volume = 0.5f };
            wavePlay.Init(new AudioFileReader($"Music" + "/" + viewModel.MusicModels.ToList()[1].MusicName));
            CurrentStatus = PlayerState.Stop;
            Volume = 0.2f;
        }

        public void ListLeft()
        {
            viewModel.ScrollLeft();
        }

        public void ListRight()
        {
            viewModel.ScrollRight();
        }

        public void UpdateVolume()
        {
            if (wavePlay == null) return;

            wavePlay.Volume += 0.2f;
            Volume += 0.2f;

            if (Volume > 0.8f)
            {
                wavePlay.Volume = 0.2f;
                Volume = 0.2f;
            }
        }

        public void Play()
        {
            wavePlay = new WaveOut {Volume = 0.2f};
            wavePlay.Init(new AudioFileReader($"Music" + "/" + viewModel.MusicModels.ToList()[1].MusicName));
            wavePlay.Play();
            CurrentStatus = PlayerState.Start;
        }

        public void Stop()
        {
            wavePlay?.Stop();
            wavePlay?.Dispose();
            CurrentStatus = PlayerState.Stop;
        }

        public void Pause()
        {
            wavePlay.Pause();
            CurrentStatus = PlayerState.Pause;
        }

        public void Resume()
        {
            wavePlay.Resume();
            CurrentStatus = PlayerState.Start;
        }

    }
}
