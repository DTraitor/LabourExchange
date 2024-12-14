using System.ComponentModel;
using Microsoft.Data.SqlClient;

namespace LabourExchange.DBModels;

public class Employer : IDbObject
{
    private Database _database;

    public int EmployerId { get; set; }
    public string CompanyName { get; set; }
    public string Description { get; set; }
    public string Industry { get; set; }
    public string Website { get; set; }

    public User User { get; set; }

    public Employer() { }

    public Employer(Database data)
    {
        _database = data;
    }

    public string GetCreateTableQuery()
    {
        return @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Employers' AND xtype='U')
            CREATE TABLE Employers (
                EmployerId INT PRIMARY KEY FOREIGN KEY REFERENCES Users(UserId),
                CompanyName NVARCHAR(255) NOT NULL,
                Description NVARCHAR(255) NOT NULL,
                Industry NVARCHAR(255) NOT NULL,
                Website NVARCHAR(255) NOT NULL
            )";
    }

    public string GetSelectAll()
    {
        return "SELECT * FROM Employers";
    }

    public SqlCommand GetDeleteQuery()
    {
        SqlCommand command = new SqlCommand("DELETE FROM Employers WHERE EmployerId = @EmployerId");
        command.Parameters.Add(new SqlParameter("@EmployerId", EmployerId));
        return command;
    }

    public SqlCommand GetInsertUpdateQuery()
    {
        SqlCommand command = new SqlCommand(@"
            IF NOT EXISTS (SELECT * FROM Employers WHERE EmployerId = @EmployerId)
            BEGIN
                INSERT INTO Employers (EmployerId, CompanyName, Description, Industry, Website)
                VALUES (@EmployerId, @CompanyName, @Description, @Industry, @Website)
            END
            ELSE
            BEGIN
                UPDATE Employers
                SET CompanyName = @CompanyName, Description = @Description, Industry = @Industry, Website = @Website
                WHERE EmployerId = @EmployerId
            END");
        command.Parameters.Add(new SqlParameter("@EmployerId", EmployerId));
        command.Parameters.Add(new SqlParameter("@CompanyName", CompanyName));
        command.Parameters.Add(new SqlParameter("@Description", Description));
        command.Parameters.Add(new SqlParameter("@Industry", Industry));
        command.Parameters.Add(new SqlParameter("@Website", Website));
        return command;
    }

    public IDbObject CreateFromReader(SqlDataReader reader, Database data)
    {
        return new Employer()
        {
            _database = data,
            EmployerId = reader.GetInt32(0),
            CompanyName = reader.GetString(1),
            Description = reader.GetString(2),
            Industry = reader.GetString(3),
            Website = reader.GetString(4),

            User = (User)data.dict[typeof(User)].FirstOrDefault(x => reader.GetInt32(0) == ((User)x).UserId)
        };
    }

    public Dictionary<string, string> GetFields()
    {
        return new()
        {
            { "EmployerId", "User" },
            { "CompanyName", "string" },
            { "Description", "string" },
            { "Industry", "string" },
            { "Website", "string" }
        };
    }

    public object GetField(string fieldName)
    {
        switch (fieldName)
        {
            case "EmployerId": return EmployerId;
            case "CompanyName": return CompanyName;
            case "Description": return Description;
            case "Industry": return Industry;
            case "Website": return Website;
            default: return null;
        }
    }

    public void SetFields(Dictionary<string, string> fields)
    {
        EmployerId = int.Parse(fields["EmployerId"]);
        CompanyName = fields["CompanyName"];
        Description = fields["Description"];
        Industry = fields["Industry"];
        Website = fields["Website"];

        User = (User)_database.dict[typeof(User)].FirstOrDefault(x => EmployerId == ((User)x).UserId);
    }

    public bool MatchesSearch(string search)
    {
        return CompanyName.ToLower().Contains(search) || Description.ToLower().Contains(search) || Industry.ToLower().Contains(search) || Website.ToLower().Contains(search);
    }

    public int GetPrimaryKey() => EmployerId;

    public string ObjName => CompanyName;

    public Dictionary<string, string> Desc => new()
    {
        { "EmployerId", User.ObjName },
        { "CompanyName", CompanyName },
        { "Description", Description },
        { "Industry", Industry },
        { "Website", Website }
    };

}