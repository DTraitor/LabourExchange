using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LabourExchange.DBModels;

namespace LabourExchange;

public partial class ObjectsList : UserControl
{
    private readonly Database _database;
    private Type _type;

    public ObjectsList(Database database, Type type)
    {
        _database = database;
        _type = type;
        InitializeComponent();
        UpdateItems();
    }

    private void UpdateItems()
    {
        string searchText = SearchBox.Text.ToLower();
        if (searchText == "search...")
        {
            searchText = "";
        }

        ListBox.ItemsSource = null;
        ListBox.ItemsSource = _database.dict[_type].Where(entry => entry.MatchesSearch(searchText)).ToList();
    }

    private void OpenEditor(IDbObject entry)
    {
        CreateUpdateWindow editor = new(_database, entry);
        editor.ShowDialog();
        UpdateItems();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        OpenEditor((IDbObject)Activator.CreateInstance(_type, _database));
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        IDbObject entry = button?.DataContext as IDbObject;
        if (entry == null)
            return;
        OpenEditor(entry);
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = SearchBox.Text.ToLower();
        if (searchText == "search...")
        {
            return;
        }

        UpdateItems();
    }

    private void RemovePlaceholderText(object sender, RoutedEventArgs e)
    {
        if (SearchBox.Text == "Search...")
        {
            SearchBox.Text = "";
            SearchBox.Foreground = new SolidColorBrush(Colors.Black);
        }
    }

    private void AddPlaceholderText(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SearchBox.Text))
        {
            SearchBox.Text = "Search...";
            SearchBox.Foreground = new SolidColorBrush(Colors.Gray);
        }
    }
}