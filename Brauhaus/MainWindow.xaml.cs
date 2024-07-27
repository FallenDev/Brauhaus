using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Brauhaus.Models;
using Brauhaus.Views;

namespace Brauhaus;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        DataContext = _viewModel;
        JsonFileListView.ItemsSource = _viewModel.JsonObjects;
        MRecipesListView.ItemsSource = _viewModel.MRecipes;
    }
        
    private void AddJournalButton_Click(object sender, RoutedEventArgs e)
    {
        var newJournal = new Journals
        {
            Id = Random.Shared.Next(),
            Name = MwName.Text,
            BrewSize = MwBrewSize.Text,
            AcidPct = MwAcidPct.Text,
            Filtered = MwFilter.Text,
            StartDate = MwStartDate.DisplayDate,
            SGravity = MwSg.Text,
            RackOneDate = MwFirstRack.DisplayDate,
            RackTwoDate = MwSecondRack.DisplayDate,
            FGravity = MwFg.Text,
            BGravity = MwBg.Text,
            BottleDate = MwBottleDate.DisplayDate,
            Content = MwContent.Text
        };

        _viewModel.JsonObjects.Add(newJournal);
        SaveJsonToFile("Journals", newJournal, newJournal.Id);
    }

    private void EditSaveJournalButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.JsonObjects.Count == 0)
        {
            AddJournalButton_Click(sender, e);
            return;
        }

        var id = GetSelectedJournalId();
        var journal = _viewModel.JsonObjects.FirstOrDefault(j => j.Id == id);

        if (journal == null)
        {
            AddJournalButton_Click(sender, e);
            return;
        }

        var newJournal = new Journals
        {
            Id = journal.Id,
            Name = MwName.Text,
            BrewSize = MwBrewSize.Text,
            AcidPct = MwAcidPct.Text,
            Filtered = MwFilter.Text,
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
        MainViewModel.CheckDirectoriesAndCreate(jsonDir);

        var jsonFile = $"{jsonDir}\\{name}.json";
        var content = JsonSerializer.Serialize(obj);
        var encryptedContent = EncryptionHelper.EncryptString(content, MainViewModel.BrauKey);

        File.WriteAllText(jsonFile, encryptedContent);
    }

    private void DeleteJournalButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.JsonObjects.Count == 0) return;
        var id = GetSelectedJournalId();
        var journal = _viewModel.JsonObjects.FirstOrDefault(j => j.Id == id);
        var location = $"{AppDomain.CurrentDomain.BaseDirectory}\\Data\\Journals\\{id}.json";
        if (journal == null) return;
            
        _viewModel.JsonObjects.Remove(journal);

        if (File.Exists(location))
            File.Delete(location);
    }

    [GeneratedRegex("[^0-9.-]+", RegexOptions.Compiled)]
    private static partial Regex NumericRegex();
    private static bool IsTextAllowed(string text) => !NumericRegex().IsMatch(text);

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

    private void JsonFileListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if ((sender as ListView)?.SelectedItem is not Journals selectedJournal) return;
        _viewModel.SelectedJournal = selectedJournal;
    }

    private string GetSelectedMRecipe()
    {
        if (MRecipesListView.SelectedItem is MRecipes selectedRecipe)
        {
            return selectedRecipe.Name;
        }

        return "";
    }

    private void MRecipeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if ((sender as ListView)?.SelectedItem is not MRecipes selectedRecipe) return;
        _viewModel.SelectedMRecipe = selectedRecipe;
    }
}