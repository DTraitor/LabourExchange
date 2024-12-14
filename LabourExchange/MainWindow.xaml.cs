using System.Windows;
using System.Windows.Controls;
using LabourExchange.DBModels;
using Application = LabourExchange.DBModels.Application;

namespace LabourExchange;

public partial class MainWindow : Window
{
    private Database _database;

    public MainWindow()
    {
        _database = new Database(
            @"Server=localhost\SQLEXPRESS;Database=Labour;TrustServerCertificate=True;Integrated Security=True;");
        InitializeComponent();
        TabsList.Items.Add(new TabItem(){ Header = "Users", Content = new ObjectsList(_database, typeof(User))});
        TabsList.Items.Add(new TabItem(){ Header = "Job Seekers", Content = new ObjectsList(_database, typeof(JobSeeker))});
        TabsList.Items.Add(new TabItem(){ Header = "Employers", Content = new ObjectsList(_database, typeof(Employer))});
        TabsList.Items.Add(new TabItem(){ Header = "Job Listings", Content = new ObjectsList(_database, typeof(JobListing))});
        TabsList.Items.Add(new TabItem(){ Header = "Applications", Content = new ObjectsList(_database, typeof(Application))});

    }
}
