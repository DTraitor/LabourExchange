using System.ComponentModel;
using Microsoft.Data.SqlClient;

namespace LabourExchange.DBModels;

public class JobSeeker : IDbObject
{
    Database _database;

    public int SeekerId { get; set; }
    public string About { get; set; }
    public string Skills { get; set; }
    public string Experience { get; set; }
    public string Education { get; set; }

    public User User { get; set; }

    public JobSeeker() { }

    public JobSeeker(Database data)
    {
        _database = data;
    }

    public string GetCreateTableQuery()
    {
        return @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='JobSeekers' AND xtype='U')
            CREATE TABLE JobSeekers (
                JobSeekerId INT PRIMARY KEY FOREIGN KEY REFERENCES Users(UserId),
                About NVARCHAR(255) NOT NULL,
                Skills NVARCHAR(255) NOT NULL,
                Experience NVARCHAR(255) NOT NULL,
                Education NVARCHAR(255) NOT NULL
            )";
    }

    public string GetSelectAll()
    {
        return "SELECT * FROM JobSeekers";
    }

    public SqlCommand GetDeleteQuery()
    {
        SqlCommand command = new SqlCommand("DELETE FROM JobSeekers WHERE JobSeekerId = @JobSeekerId");
        command.Parameters.Add(new SqlParameter("@JobSeekerId", SeekerId));
        return command;
    }

    public SqlCommand GetInsertUpdateQuery()
    {
        SqlCommand command = new SqlCommand(@"
            IF NOT EXISTS (SELECT * FROM JobSeekers WHERE JobSeekerId = @JobSeekerId)
            BEGIN
                INSERT INTO JobSeekers (JobSeekerId, About, Skills, Experience, Education)
                VALUES (@JobSeekerId, @About, @Skills, @Experience, @Education)
            END
            ELSE
            BEGIN
                UPDATE JobSeekers
                SET About = @About, Skills = @Skills, Experience = @Experience, Education = @Education
                WHERE JobSeekerId = @JobSeekerId
            END");
        command.Parameters.Add(new SqlParameter("@JobSeekerId", SeekerId));
        command.Parameters.Add(new SqlParameter("@About", About));
        command.Parameters.Add(new SqlParameter("@Skills", Skills));
        command.Parameters.Add(new SqlParameter("@Experience", Experience));
        command.Parameters.Add(new SqlParameter("@Education", Education));
        return command;
    }

    public IDbObject CreateFromReader(SqlDataReader reader, Database data)
    {
        return new JobSeeker()
        {
            _database = data,
            SeekerId = reader.GetInt32(0),
            About = reader.GetString(1),
            Skills = reader.GetString(2),
            Experience = reader.GetString(3),
            Education = reader.GetString(4),

            User = (User)data.dict[typeof(User)].FirstOrDefault(x => ((User)x).UserId == reader.GetInt32(0))
        };
    }

    public Dictionary<string, string> GetFields()
    {
        return new()
        {
            { "SeekerId", "User" },
            { "About", "string" },
            { "Skills", "string" },
            { "Experience", "string" },
            { "Education", "string" }
        };
    }

    public object GetField(string fieldName)
    {
        switch (fieldName)
        {
            case "SeekerId": return SeekerId;
            case "About": return About;
            case "Skills": return Skills;
            case "Experience": return Experience;
            case "Education": return Education;
            default: return null;
        }
    }

    public void SetFields(Dictionary<string, string> fields)
    {
        SeekerId = int.Parse(fields["SeekerId"]);
        About = fields["About"];
        Skills = fields["Skills"];
        Experience = fields["Experience"];
        Education = fields["Education"];

        User = (User)_database.dict[typeof(User)].FirstOrDefault(x => SeekerId == ((User)x).UserId);
    }

    public bool MatchesSearch(string search)
    {
        return About.ToLower().Contains(search) || Skills.ToLower().Contains(search) || Experience.ToLower().Contains(search) || Education.ToLower().Contains(search);
    }

    public int GetPrimaryKey() => SeekerId;

    public string ObjName => User.Name;

    public Dictionary<string, string> Desc => new()
    {
        { "SeekerId", User.Name },
        { "About", About },
        { "Skills", Skills },
        { "Experience", Experience },
        { "Education", Education }
    };
}