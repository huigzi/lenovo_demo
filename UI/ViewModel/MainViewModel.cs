using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using UI.Entity;

namespace UI.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ModelsInitial();
            ScrollLeftCommand = new RelayCommand(ScrollLeft);
            ScrollRightCommand = new RelayCommand(ScrollRight);
        }

        public ObservableCollection<MusicModel> MusicModels { get; set; }

        public RelayCommand ScrollLeftCommand { get; set; }
        public RelayCommand ScrollRightCommand { get; set; }

        private void ModelsInitial()
        {
            MusicModels = new ObservableCollection<MusicModel>();
            MusicModels.Add(new MusicModel {FilePath = "Profile/people1.jpg", MusicName = "psychopass.wav", Color = "#DBF2EF" });
            MusicModels.Add(new MusicModel {FilePath = "Profile/people2.jpg", MusicName = "IdealLife.wav", Color = "#FEA3B2" });
            MusicModels.Add(new MusicModel {FilePath = "Profile/people3.jpg", MusicName = "SybilaSystem.wav", Color = "#AABF86"});
        }

        private void ScrollLeft()
        {
            var tempItem = MusicModels[0];
            MusicModels.RemoveAt(0);
            MusicModels.Add(tempItem);
        }

        private void ScrollRight()
        {
            var tempItem = MusicModels[2];
            MusicModels.RemoveAt(2);
            MusicModels.Insert(0, tempItem);
        }
    }
}