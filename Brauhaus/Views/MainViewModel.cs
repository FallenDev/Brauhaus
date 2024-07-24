using Brauhaus.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;

namespace Brauhaus.Views
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Journals> _jsonObjects;
        private Journals _selectedJournal;

        public ObservableCollection<Journals> JsonObjects
        {
            get => _jsonObjects;
            set
            {
                _jsonObjects = value;
                OnPropertyChanged(nameof(JsonObjects));
            }
        }

        public Journals SelectedJournal
        {
            get => _selectedJournal;
            set
            {
                _selectedJournal = value;
                OnPropertyChanged(nameof(SelectedJournal));
            }
        }

        public MainViewModel()
        {
            _jsonObjects = [];
            LoadJsonFiles();
        }

        private void LoadJsonFiles()
        {
            var jsonDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\Data";
            var journalsDir = $"{jsonDirectory}\\Journals";
            var meadRecipes = $"{jsonDirectory}\\Mead";
            var wineRecipes = $"{jsonDirectory}\\Wine";

            CheckDirectoriesAndCreate(jsonDirectory);
            CheckDirectoriesAndCreate(journalsDir);
            CheckDirectoriesAndCreate(meadRecipes);
            CheckDirectoriesAndCreate(wineRecipes);

            foreach (var file in Directory.GetFiles(journalsDir, "*.json"))
            {
                var jsonContent = File.ReadAllText(file);
                if (jsonContent.Length == 0) continue;
                var journal = JsonSerializer.Deserialize<Journals>(jsonContent)!;
                _jsonObjects.Add(new Journals
                {
                    Id = journal.Id, Name = journal.Name, StartDate = journal.StartDate, 
                    SGravity = journal.SGravity, RackOneDate = journal.RackOneDate, RackTwoDate = journal.RackTwoDate,
                    FGravity = journal.FGravity, BGravity = journal.BGravity, BottleDate = journal.BottleDate, 
                    Content = journal.Content
                });
            }

            // Create logic for mead and wine recipes
        }

        internal static void CheckDirectoriesAndCreate(string directory)
        {
            if (Directory.Exists(directory)) return;
            Directory.CreateDirectory(directory);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
