using System;
using Core.ViewModel;
using NAudio.Wave;
using System.Linq;

namespace Core
{
    public class MusicPlayer
    {
        private WaveOut wavePlay;
        private float volume;

        public PlayerState CurrentStatus{ get; set; }

        public MusicPlayer(string musicFullPath)
        {
            wavePlay = new WaveOut { Volume = 0.5f };
            wavePlay.Init(new AudioFileReader(musicFullPath));
            CurrentStatus = PlayerState.Stop;
            volume = 0.2f;
        }

        public float UpVolume()
        {
            if (wavePlay == null) throw new NullReferenceException("wavePlay not found");

            try
            {
                if (volume >= 0.8f)
                {
                    wavePlay.Volume = 0.8f;
                    volume = 0.8f;
                }

                wavePlay.Volume += 0.2f;
                volume += 0.2f;

                return wavePlay.Volume;
            }
            catch (Exception e)
            {
                return volume;
            }
        }

        public float DownVolume()
        {
            if (wavePlay == null) throw new NullReferenceException("wavePlay not found");

            try
            {
                if (volume <= 0.2f)
                {
                    wavePlay.Volume = 0.2f;
                    volume = 0.2f;
                }

                wavePlay.Volume -= 0.2f;
                volume -= 0.2f;

                return wavePlay.Volume;
            }
            catch (Exception e)
            {
                return volume;
            }
        }

        public void Play(string musicFullPath)
        {
            wavePlay = new WaveOut {Volume = 0.2f};
            wavePlay.Init(new AudioFileReader(musicFullPath));
            wavePlay.Play();
            CurrentStatus = PlayerState.Start;
        }

        public void Stop()
        {
            if (wavePlay == null) throw new NullReferenceException("wavePlay not found");
            wavePlay.Stop();
            wavePlay.Dispose();
            CurrentStatus = PlayerState.Stop;
        }

        public void Pause()
        {
            if (wavePlay == null) throw new NullReferenceException("wavePlay not found");
            wavePlay.Pause();
            CurrentStatus = PlayerState.Pause;
        }

        public void Resume()
        {
            if (wavePlay == null) throw new NullReferenceException("wavePlay not found");
            wavePlay.Resume();
            CurrentStatus = PlayerState.Start;
        }

    }
}
