using Brauhaus.Models;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace Brauhaus.Views;

public class MainViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private ObservableCollection<Journals> _jsonObjects;
    private Journals _selectedJournal;
    public static string BrauKey = "";

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
        BrauKey = CheckGuidFileAndCreate();

        foreach (var file in Directory.GetFiles(journalsDir, "*.json"))
        {
            var jsonContent = File.ReadAllText(file);
            if (jsonContent.Length == 0) continue;
            var decryptedContent = EncryptionHelper.DecryptString(jsonContent, BrauKey);
            var journal = JsonSerializer.Deserialize<Journals>(decryptedContent)!;

            _jsonObjects.Add(new Journals
            {
                Id = journal.Id,
                Name = journal.Name,
                BrewSize = journal.BrewSize,
                AcidPct = journal.AcidPct,
                Filtered = journal.Filtered,
                StartDate = journal.StartDate,
                SGravity = journal.SGravity,
                RackOneDate = journal.RackOneDate,
                RackTwoDate = journal.RackTwoDate,
                FGravity = journal.FGravity,
                BGravity = journal.BGravity,
                BottleDate = journal.BottleDate,
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

    private static string CheckGuidFileAndCreate()
    {
        var directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var path = Path.Combine(directory, "Brewhaus.txt");

        if (!Directory.Exists(directory)) return "Denied";
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateKey();
        BrauKey = Convert.ToBase64String(aes.Key);
        File.WriteAllText(path, BrauKey);

        return BrauKey;
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}