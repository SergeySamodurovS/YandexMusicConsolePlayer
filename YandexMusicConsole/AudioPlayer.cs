using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yandex.Music.Api;
using Yandex.Music.Api.Models.Track;
using Yandex.Music.Api.Common;

namespace YandexMusicConsole
{
    class AudioPlayer
    {
        private string _title;
        private TimeSpan _time;
        private int TotalPosition = 0;
        private AuthStorage _authStorage;
        private YTrack _track;
        private TimeSpan _currentTime;

        public AudioPlayer(AuthStorage authStorage, YTrack track)
        {
            _authStorage = authStorage;
            _track = track;
            _currentTime = new TimeSpan();
        }

        public void Play()
        {
            Task.Factory.StartNew(() =>
            {
                using (var audioFile = new AudioFileReader($"{_authStorage.Debug.OutputDir}/{_track.Title}.mp3"))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();

                    SetTrack(_track.Title, audioFile.TotalTime);

                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Console.Clear();
                        SetCurrentPosition(audioFile.CurrentTime);
                        Show();
                        Thread.Sleep(1000);
                    }
                }
            });
        }

        public void SetTrack(string title, TimeSpan time)
        {
            _title = title;
            _time = time;

            if (_title.Length >= 12)
                _title = $"{_title.Substring(0, 12)}...";
        }

        public void SetCurrentPosition(TimeSpan time)
        {
            _currentTime = time;

            var current = time.TotalSeconds;
            var total = 58 / _time.TotalSeconds;

            //TotalPosition = (int)((_time.TotalSeconds / 100) * time.TotalSeconds);
            TotalPosition = (int)(total * current);
        }

        public void Show()
        {
            Console.WriteLine();
            Console.WriteLine($":.\t{_title}\t{new string(' ', 30)}\t\t{_currentTime.Minutes}:{_currentTime.Seconds} | {_time.Minutes}:{_time.Seconds}");
            Console.WriteLine($":.\t|{new string('-', TotalPosition)}>{new string('.', 58 - TotalPosition)}|");
        }
    }
}
