using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Brauhaus.Models;

namespace Brauhaus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ObservableCollection<Journals> _jsonObjects;

        public MainWindow()
        {
            InitializeComponent();
            _jsonObjects = [];
            JsonFileListView.ItemsSource = _jsonObjects;
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

        private static void CheckDirectoriesAndCreate(string directory)
        {
            if (Directory.Exists(directory)) return;
            Directory.CreateDirectory(directory);
        }

        private void AddJournalButton_Click(object sender, RoutedEventArgs e)
        {
            var newJournal = new Journals
            {
                Id = Random.Shared.Next(),
                Name = MwName.Text,
                StartDate = MwStartDate.DisplayDate,
                SGravity = MwSg.Text,
                RackOneDate = MwFirstRack.DisplayDate,
                RackTwoDate = MwSecondRack.DisplayDate,
                FGravity = MwFg.Text,
                BGravity = MwBg.Text,
                BottleDate = MwBottleDate.DisplayDate,
                Content = MwContent.Text
            };

            _jsonObjects.Add(newJournal);

            SaveJsonToFile("Journals", newJournal, newJournal.Id);
        }

        private void EditSaveJournalButton_Click(object sender, RoutedEventArgs e)
        {
            if (_jsonObjects.Count == 0) return;
            var id = GetSelectedJournalId();
            var journal = _jsonObjects.FirstOrDefault(j => j.Id == id);

            if (journal == null) return;
            var newJournal = new Journals
            {
                Id = journal.Id,
                Name = MwName.Text,
                StartDate = MwStartDate.DisplayDate,
                SGravity = MwSg.Text,
                RackOneDate = MwFirstRack.DisplayDate,
                RackTwoDate = MwSecondRack.DisplayDate,
                FGravity = MwFg.Text,
                BGravity = MwBg.Text,
                BottleDate = MwBottleDate.DisplayDate,
                Content = MwContent.Text
            };

            SaveJsonToFile("Journals", newJournal, newJournal.Id);
        }

        private static void SaveJsonToFile(string directory, object obj, int name)
        {
            var jsonDir = $"{AppDomain.CurrentDomain.BaseDirectory}\\Data\\{directory}";
            CheckDirectoriesAndCreate(jsonDir);

            var jsonFile = $"{jsonDir}\\{name}.json";
            var content = JsonSerializer.Serialize(obj);
            File.WriteAllText(jsonFile, content);
        }

        private void DeleteJournalButton_Click(object sender, RoutedEventArgs e)
        {
            if (_jsonObjects.Count == 0) return;
            var id = GetSelectedJournalId();
            var journal = _jsonObjects.FirstOrDefault(j => j.Id == id);
            var location = $"{AppDomain.CurrentDomain.BaseDirectory}\\Data\\Journals\\{id}.json";
            if (journal == null) return;
            
            _jsonObjects.Remove(journal);

            if (File.Exists(location))
                File.Delete(location);
        }

        private static readonly Regex NumericRegex = new("[^0-9.-]+");
        private static bool IsTextAllowed(string text)
        {
            return !NumericRegex.IsMatch(text);
        }

        private void NumericPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var allowed = IsTextAllowed(e.Text);
            e.Handled = !allowed;
        }

        private int GetSelectedJournalId()
        {
            if (JsonFileListView.SelectedItem is Journals selectedJournal)
            {
                return selectedJournal.Id;
            }

            return -1;
        }
    }
}