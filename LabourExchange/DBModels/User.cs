using System.ComponentModel;
using Microsoft.Data.SqlClient;

namespace LabourExchange.DBModels;

public class User : IDbObject
{
    private Database _database;

    public int UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string UserType { get; set; }

    public User() { }

    public User(Database data)
    {
        _database = data;
    }

    public string GetCreateTableQuery()
    {
        return @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
            CREATE TABLE Users (
                UserId INT PRIMARY KEY IDENTITY,
                Name NVARCHAR(255) NOT NULL,
                Email NVARCHAR(255) NOT NULL,
                Phone NVARCHAR(255) NOT NULL,
                UserType NVARCHAR(255) NOT NULL
            )";
    }

    public string GetSelectAll()
    {
        return "SELECT * FROM Users";
    }

    public SqlCommand GetDeleteQuery()
    {
        SqlCommand command = new SqlCommand("DELETE FROM Users WHERE UserId = @UserId");
        command.Parameters.Add(new SqlParameter("@UserId", UserId));
        return command;
    }

    public SqlCommand GetInsertUpdateQuery()
    {
        SqlCommand command = new SqlCommand(@"
            IF NOT EXISTS (SELECT * FROM Users WHERE UserId = @UserId)
            BEGIN
                INSERT INTO Users (Name, Email, Phone, UserType)
                VALUES (@Name, @Email, @Phone, @UserType)
            END
            ELSE
            BEGIN
                UPDATE Users
                SET Name = @Name, Email = @Email, Phone = @Phone, UserType = @UserType
                WHERE UserId = @UserId
            END");
        command.Parameters.Add(new SqlParameter("@UserId", UserId));
        command.Parameters.Add(new SqlParameter("@Name", Name));
        command.Parameters.Add(new SqlParameter("@Email", Email));
        command.Parameters.Add(new SqlParameter("@Phone", Phone));
        command.Parameters.Add(new SqlParameter("@UserType", UserType));
        return command;
    }

    public IDbObject CreateFromReader(SqlDataReader reader, Database data)
    {
        return new User()
        {
            _database = data,
            UserId = reader.GetInt32(0),
            Name = reader.GetString(1),
            Email = reader.GetString(2),
            Phone = reader.GetString(3),
            UserType = reader.GetString(4)
        };
    }

    public Dictionary<string, string> GetFields()
    {
        return new()
        {
            { "Name", "string" },
            { "Email", "string"},
            { "Phone", "string" },
            { "UserType", "string" }
        };
    }

    public object GetField(string fieldName)
    {
        switch (fieldName)
        {
            case "Name": return Name;
            case "Email": return Email;
            case "Phone": return Phone;
            case "UserType": return UserType;
            default: return null;
        }
    }

    public void SetFields(Dictionary<string, string> fields)
    {
        Name = fields["Name"];
        Email = fields["Email"];
        Phone = fields["Phone"];
        UserType = fields["UserType"];
    }

    public bool MatchesSearch(string search)
    {
        return Name.ToLower().Contains(search) || Email.ToLower().Contains(search) || Phone.ToLower().Contains(search) || UserType.ToLower().Contains(search);
    }

    public int GetPrimaryKey() => UserId;

    public string ObjName => Name;

    public Dictionary<string, string> Desc => new()
    {
        { "Name", Name },
        { "Email", Email },
        { "Phone", Phone },
        { "UserType", UserType }
    };
}