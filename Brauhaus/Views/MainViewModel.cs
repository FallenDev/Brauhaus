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
    private ObservableCollection<MRecipes> _mRecipes;
    private Journals _selectedJournal;
    private MRecipes _selectedMRecipe;
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

    public ObservableCollection<MRecipes> MRecipes
    {
        get => _mRecipes;
        set
        {
            _mRecipes = value;
            OnPropertyChanged(nameof(MRecipes));
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

    public MRecipes SelectedMRecipe
    {
        get => _selectedMRecipe;
        set
        {
            _selectedMRecipe = value;
            OnPropertyChanged(nameof(SelectedMRecipe));
        }
    }

    public MainViewModel()
    {
        _jsonObjects = [];
        _mRecipes = [];
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

        try
        {
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
                    RackOneDate = journal.RackOneDate ?? null,
                    RackTwoDate = journal.RackTwoDate ?? null,
                    FGravity = journal.FGravity,
                    BGravity = journal.BGravity,
                    BottleDate = journal.BottleDate ?? null,
                    Content = journal.Content
                });
            }
        }
        catch
        {
            // Ignored
        }

        try
        {
            foreach (var file in Directory.GetFiles(meadRecipes, "*.json"))
            {
                var jsonContent = File.ReadAllText(file);
                if (jsonContent.Length == 0) continue;
                var mead = JsonSerializer.Deserialize<MRecipes>(jsonContent)!;

                _mRecipes.Add(new MRecipes
                {
                    Name = mead.Name,
                    HoneyType = mead.HoneyType,
                    HoneyAmount = mead.HoneyAmount,
                    Hops = mead.Hops,
                    Herbs = mead.Herbs,
                    Tannins = mead.Tannins,
                    Yeast = mead.Yeast,
                    Stabilizer = mead.Stabilizer,
                    Nutrients = mead.Nutrients,
                    Additives = mead.Additives,
                    AcidPct = mead.AcidPct,
                    Instructions = mead.Instructions
                });
            }
        }
        catch
        {
            // Ignored
        }
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