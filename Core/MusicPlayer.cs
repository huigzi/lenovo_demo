using NAudio.Wave;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class MusicPlayer
    {
        public List<string> MusicList;
        private  WaveOut wavePlay;

        public int Index { get; set; }
        public PlayerState CurrentStatus { get; set; }
        public float Volume { get; set; }

        public MusicPlayer()
        {
            MusicList = new List<string>();
            CurrentStatus = PlayerState.Stop;
        }

        public void PlayerInitial()
        {
            MusicList.Add("music1.wav");
            MusicList.Add("music2.wav");
            MusicList.Add("music3.wav");

            wavePlay = new WaveOut { Volume = 0.2f };
            wavePlay.Init(new AudioFileReader(MusicList[0]));

            Volume = 0.2f;
        }

        public void ListLeft()
        {
            MusicList.Add(MusicList[0]);
            MusicList.RemoveAt(0);
        }

        public void ListRight()
        {
            MusicList.Insert(0, MusicList.Last());
            MusicList.RemoveAt(3);
        }

        public void UpdateVolume()
        {
            if (wavePlay == null) return;

            wavePlay.Volume += 0.2f;
            Volume += 0.2f;

            if (wavePlay.Volume > 0.8f)
            {
                wavePlay.Volume = 0.2f;
                Volume = 0.2f;
            }
        }

        public void Play()
        {
            wavePlay = new WaveOut {Volume = 0.2f};
            wavePlay.Init(new AudioFileReader(MusicList[Index]));
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
