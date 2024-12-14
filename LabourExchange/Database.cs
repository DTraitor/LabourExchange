using System.ComponentModel;
using LabourExchange.DBModels;
using Microsoft.Data.SqlClient;

namespace LabourExchange;

public class Database
{
    public Dictionary<Type, BindingList<IDbObject>> dict = new();
    private string _connectionString;

    public Database(string connectionString)
    {
        _connectionString = connectionString;

        CreateTables();
        LoadData();
    }

    private void CreateTables()
    {
        //using var connection = new SqlConnection(_connectionString);
        //connection.Open();
        //using var command = connection.CreateCommand();
        //command.CommandText = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Labour') CREATE DATABASE Labour";
        //command.ExecuteNonQuery();
        //throw new NotImplementedException();

        dict[typeof(User)] = new BindingList<IDbObject>();
        CreateTable(new User());
        dict[typeof(JobSeeker)] = new BindingList<IDbObject>();
        CreateTable(new JobSeeker());
        dict[typeof(Employer)] = new BindingList<IDbObject>();
        CreateTable(new Employer());
        dict[typeof(JobListing)] = new BindingList<IDbObject>();
        CreateTable(new JobListing());
        dict[typeof(Application)] = new BindingList<IDbObject>();
        CreateTable(new Application());
    }

    private void CreateTable<T>(T temp) where T : IDbObject
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = temp.GetCreateTableQuery();
        command.ExecuteNonQuery();
    }

    private void LoadData()
    {
        LoadType(new User());
        LoadType(new JobSeeker());
        LoadType(new Employer());
        LoadType(new JobListing());
        LoadType(new Application());
    }

    private void LoadType(IDbObject temp)
    {
        dict[temp.GetType()].Clear();
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var command = new SqlCommand(temp.GetSelectAll(), connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            dict[temp.GetType()].Add(temp.CreateFromReader(reader, this));
        }
    }

    public void Delete(IDbObject entry)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = entry.GetDeleteQuery();
        command.Connection = connection;
        command.ExecuteNonQuery();

        LoadType(entry);
    }

    public void CreateOrUpdate(IDbObject entry)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = entry.GetInsertUpdateQuery();
        command.Connection = connection;
        command.ExecuteNonQuery();

        LoadType(entry);
    }
}