using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Core.Entity;
using Core.Interface;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using OxyPlot;

namespace Core.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    public class MainViewModel : ViewModelBase, IValidationExceptionHandler
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        private ISaveData<byte[]> saveData;
        private Dispatcher dispatcher;

        public MainViewModel(ISaveData<byte[]> saveData)
        {
            this.saveData = saveData;
            BackGround = 5;
            presentWord = "开始手势采集，准备好了嘛";
            countWord = string.Empty;

            ModelsInitial();
            ScrollLeftCommand = new RelayCommand(ScrollLeft);
            ScrollRightCommand = new RelayCommand(ScrollRight);
            SaveDataCommand = new RelayCommand(SaveData);
            dispatcher = Dispatcher.CurrentDispatcher;

            Messenger.Default.Register<string>(this, "PresentUpdate", PresentUpdate);
            Messenger.Default.Register<string>(this, "PresentUpdate2", PresentUpdate2);
        }

        public ObservableCollection<MusicModel> MusicModels { get; set; }

        public IList<DataPoint> Points1 { get; set; }
        public IList<DataPoint> Points2 { get; set; }

        private string presentWord;

        public string PresentWord
        {
            get => presentWord;
            set
            {
                presentWord = value;
                RaisePropertyChanged();
            }
        }

        private string countWord;

        public string CountWord
        {
            get => countWord;
            set
            {
                countWord = value;
                RaisePropertyChanged();
            }
        }


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

        private string status;

        public string Status
        {
            get => status;
            set
            {
                status = value;
                RaisePropertyChanged();
            }
        }

        private int backGround;

        public int BackGround
        {
            get => backGround;
            set
            {
                backGround = value;
                RaisePropertyChanged();
            }
        }

        private bool isValid;

        public bool IsValid
        {
            get => isValid;
            set
            {
                isValid = value;
                RaisePropertyChanged();
            }
        }

        private string name;

        public string Name
        {
            get => name;
            set
            {
                name = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ScrollLeftCommand { get; set; }
        public RelayCommand ScrollRightCommand { get; set; }
        public RelayCommand SaveDataCommand { get; set; }

        private void PresentUpdate(string message)
        {
            PresentWord = message;
        }

        private void PresentUpdate2(string count)
        {
            CountWord = count;
        }

        private void ModelsInitial()
        {
            MusicModels = new ObservableCollection<MusicModel>
            {
                new MusicModel {FilePath = "Profile/people1.jpg", MusicName = "Psychopass", Color = "#DBF2EF"},
                new MusicModel {FilePath = "Profile/people2.jpg", MusicName = "IdealLife", Color = "#FEA3B2"},
                new MusicModel {FilePath = "Profile/people3.jpg", MusicName = "SybilaSystem", Color = "#AABF86"}
            };

            Points1 = new ObservableCollection<DataPoint>();
            Points2 = new ObservableCollection<DataPoint>();

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
                var tempItem = MusicModels[0];
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

        public void Plot(List<float> tr1, List<float> tr2)
        {
            dispatcher.Invoke(() =>
            {
                Points1.Clear();
                Points2.Clear();

                for (int i = 0; i < tr1.Count; i++)
                {
                    Points1.Add(new DataPoint(i + 1, tr1[i]));
                }

                for (int i = 0; i < tr2.Count; i++)
                {
                    Points2.Add(new DataPoint(i + 1, tr2[i]));
                }
            });
        }
    }
}