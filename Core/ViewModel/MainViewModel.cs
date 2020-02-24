using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Core.Entity;
using Core.Interface;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OxyPlot;

namespace Core.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        private ISaveData<byte[]> saveData;
        private Dispatcher dispatcher;

        public MainViewModel(ISaveData<byte[]> saveData)
        {
            this.saveData = saveData;

            ModelsInitial();
            ScrollLeftCommand = new RelayCommand(ScrollLeft);
            ScrollRightCommand = new RelayCommand(ScrollRight);
            SaveDataCommand = new RelayCommand(SaveData);
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        public ObservableCollection<MusicModel> MusicModels { get; set; }
        public IList<DataPoint> Points { get; set; }


        private float volume;

        public float Volume
        {
            get => volume;
            set
            {
                volume = value;
                RaisePropertyChanged();
            }
        }

        private string color;

        public string Color
        {
            get => color; 
            set 
            {
                color = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ScrollLeftCommand { get; set; }
        public RelayCommand ScrollRightCommand { get; set; }
        public RelayCommand SaveDataCommand { get; set; }

        private void ModelsInitial()
        {
            MusicModels = new ObservableCollection<MusicModel>
            {
                new MusicModel {FilePath = "Profile/people1.jpg", MusicName = "psychopass.wav", Color = "#DBF2EF"},
                new MusicModel {FilePath = "Profile/people2.jpg", MusicName = "IdealLife.wav", Color = "#FEA3B2"},
                new MusicModel {FilePath = "Profile/people3.jpg", MusicName = "SybilaSystem.wav", Color = "#AABF86"}
            };

            Points = new List<DataPoint>
            {
                new DataPoint(0, 4),
                new DataPoint(10, 13),
                new DataPoint(20, 15),
                new DataPoint(30, 16),
                new DataPoint(40, 12),
                new DataPoint(50, 12)
            };
        }

        private void SaveData()
        {
            if (saveData.State == DataState.Started)
            {
                saveData.StopSave();
            }
            else
            {
                saveData.StartSave();
            }
        }

        public void ScrollLeft()
        {
            dispatcher.Invoke(() =>
            {
                MusicModel tempItem = MusicModels[0];
                MusicModels.RemoveAt(0);
                MusicModels.Add(tempItem);
            });
        }

        public void ScrollRight()
        {
            dispatcher.Invoke(() =>
            {
                var tempItem = MusicModels[2];
                MusicModels.RemoveAt(2);
                MusicModels.Insert(0, tempItem);
            });
        }
    }
}