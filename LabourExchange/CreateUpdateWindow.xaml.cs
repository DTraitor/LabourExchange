using System.Windows;
using System.Windows.Controls;
using LabourExchange.DBModels;

namespace LabourExchange;

public partial class CreateUpdateWindow : Window
{
    private Database _database;
    private IDbObject _editing;
    private Dictionary<string, FrameworkElement> _editParams = new();

    public CreateUpdateWindow(Database data, IDbObject editing)
    {
        _database = data;
        _editing = editing;
        InitializeComponent();
        GenerateEditPanel();
    }

    private void GenerateEditPanel()
    {
        foreach (var param in _editing.GetFields())
        {
            switch (param.Value)
            {
                case "string":
                    TextBox textBox = new();
                    textBox.Text = _editing.GetField(param.Key)?.ToString();
                    _editParams[param.Key] = textBox;
                    break;
                case "int":
                    TextBox intBox = new();
                    intBox.Text = _editing.GetField(param.Key).ToString();
                    _editParams[param.Key] = intBox;
                    break;
                case "DateTime":
                    DatePicker datePicker = new();
                    datePicker.SelectedDate = DateTime.Parse(_editing.GetField(param.Key).ToString());
                    _editParams[param.Key] = datePicker;
                    break;
                case "User":
                    ComboBox userBox = new();
                    userBox.DisplayMemberPath = "ObjName";
                    userBox.ItemsSource = _database.dict[typeof(User)];
                    userBox.SelectedItem = _database.dict[typeof(User)].FirstOrDefault(x => ((User)x).UserId == (int)_editing.GetField(param.Key));
                    _editParams[param.Key] = userBox;
                    break;
                case "Employer":
                    ComboBox employerBox = new();
                    employerBox.DisplayMemberPath = "ObjName";
                    employerBox.ItemsSource = _database.dict[typeof(Employer)];
                    employerBox.SelectedItem = _database.dict[typeof(Employer)].FirstOrDefault(x => ((Employer)x).EmployerId == (int)_editing.GetField(param.Key));
                    _editParams[param.Key] = employerBox;
                    break;
                case "JobSeeker":
                    ComboBox seekerBox = new();
                    seekerBox.DisplayMemberPath = "ObjName";
                    seekerBox.ItemsSource = _database.dict[typeof(JobSeeker)];
                    seekerBox.SelectedItem = _database.dict[typeof(JobSeeker)].FirstOrDefault(x => ((JobSeeker)x).SeekerId == (int)_editing.GetField(param.Key));
                    _editParams[param.Key] = seekerBox;
                    break;
                case "JobListing":
                    ComboBox listingBox = new();
                    listingBox.DisplayMemberPath = "ObjName";
                    listingBox.ItemsSource = _database.dict[typeof(JobListing)];
                    listingBox.SelectedItem = _database.dict[typeof(JobListing)].FirstOrDefault(x => ((JobListing)x).JobListingId == (int)_editing.GetField(param.Key));
                    _editParams[param.Key] = listingBox;
                    break;
            }
        }

        foreach (var editParam in _editParams)
        {
            StackPanel stackPanel = new();
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.Margin = new Thickness(5);
            stackPanel.Width = 400;
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;

            editParam.Value.Width = 250;

            stackPanel.Children.Add(new TextBlock() { Width = 100, Text = $"{editParam.Key}: " });
            stackPanel.Children.Add(editParam.Value);
            EditPanel.Children.Add(stackPanel);
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Dictionary<string, string> fields = new();
            foreach (var editParam in _editParams)
            {
                if (editParam.Value is TextBox)
                    fields[editParam.Key] = (editParam.Value as TextBox)?.Text;
                else if (editParam.Value is ComboBox)
                    fields[editParam.Key] = ((IDbObject)(editParam.Value as ComboBox)?.SelectedItem).GetPrimaryKey().ToString();
                else if (editParam.Value is DatePicker)
                    fields[editParam.Key] = (editParam.Value as DatePicker)?.SelectedDate.ToString();
                else
                    throw new NotImplementedException();
            }
            _editing.SetFields(fields);
            _database.CreateOrUpdate(_editing);
            this.DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    public void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _database.Delete(_editing);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        this.DialogResult = true;
        Close();
    }

    public void CloseEditPanel_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
        Close();
    }
}