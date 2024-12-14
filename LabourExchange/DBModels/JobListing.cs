using System.ComponentModel;
using Microsoft.Data.SqlClient;

namespace LabourExchange.DBModels;

public class JobListing : IDbObject
{
    private Database _database;

    public int JobListingId { get; set; }
    public int EmployerId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string RequiredSkills { get; set; }
    public string Location { get; set; }
    public string EmploymentType { get; set; }
    public string SalaryStatus { get; set; }
    public int SalaryMin { get; set; }
    public int SalaryMax { get; set; }
    public DateTime PostedAt { get; set; }
    public string Status { get; set; }

    public Employer Employer { get; set; }

    public JobListing() { }

    public JobListing(Database data)
    {
        _database = data;
    }

    public string GetCreateTableQuery()
    {
        return @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='JobListings' AND xtype='U')
            CREATE TABLE JobListings (
                JobListingId INT PRIMARY KEY IDENTITY,
                EmployerId INT FOREIGN KEY REFERENCES Employers(EmployerId),
                Title NVARCHAR(255) NOT NULL,
                Description NVARCHAR(255) NOT NULL,
                RequiredSkills NVARCHAR(255) NOT NULL,
                Location NVARCHAR(255) NOT NULL,
                EmploymentType NVARCHAR(255) NOT NULL,
                SalaryStatus NVARCHAR(255) NOT NULL,
                SalaryMin INT,
                SalaryMax INT,
                PostedAt DATETIME NOT NULL,
                Status NVARCHAR(255) NOT NULL
            )";
    }

    public string GetSelectAll()
    {
        return "SELECT * FROM JobListings";
    }

    public SqlCommand GetDeleteQuery()
    {
        SqlCommand command = new SqlCommand("DELETE FROM JobListings WHERE JobListingId = @JobListingId");
        command.Parameters.Add(new SqlParameter("@JobListingId", JobListingId));
        return command;
    }

    public SqlCommand GetInsertUpdateQuery()
    {
        SqlCommand command = new SqlCommand(@"
            IF NOT EXISTS (SELECT * FROM JobListings WHERE JobListingId = @JobListingId)
            BEGIN
                INSERT INTO JobListings (EmployerId, Title, Description, RequiredSkills, Location, EmploymentType, SalaryStatus, SalaryMin, SalaryMax, PostedAt, Status)
                VALUES (@EmployerId, @Title, @Description, @RequiredSkills, @Location, @EmploymentType, @SalaryStatus, @SalaryMin, @SalaryMax, @PostedAt, @Status)
            END
            ELSE
            BEGIN
                UPDATE JobListings
                SET EmployerId = @EmployerId, Title = @Title, Description = @Description, RequiredSkills = @RequiredSkills, Location = @Location, EmploymentType = @EmploymentType, SalaryStatus = @SalaryStatus, SalaryMin = @SalaryMin, SalaryMax = @SalaryMax, PostedAt = @PostedAt, Status = @Status
                WHERE JobListingId = @JobListingId
            END");
        command.Parameters.Add(new SqlParameter("@JobListingId", JobListingId));
        command.Parameters.Add(new SqlParameter("@EmployerId", Employer.EmployerId));
        command.Parameters.Add(new SqlParameter("@Title", Title));
        command.Parameters.Add(new SqlParameter("@Description", Description));
        command.Parameters.Add(new SqlParameter("@RequiredSkills", RequiredSkills));
        command.Parameters.Add(new SqlParameter("@Location", Location));
        command.Parameters.Add(new SqlParameter("@EmploymentType", EmploymentType));
        command.Parameters.Add(new SqlParameter("@SalaryStatus", SalaryStatus));
        command.Parameters.Add(new SqlParameter("@SalaryMin", SalaryMin));
        command.Parameters.Add(new SqlParameter("@SalaryMax", SalaryMax));
        command.Parameters.Add(new SqlParameter("@PostedAt", PostedAt));
        command.Parameters.Add(new SqlParameter("@Status", Status));
        return command;
    }

    public IDbObject CreateFromReader(SqlDataReader reader, Database data)
    {
        return new JobListing()
        {
            _database = data,
            JobListingId = reader.GetInt32(0),
            EmployerId = reader.GetInt32(1),
            Title = reader.GetString(2),
            Description = reader.GetString(3),
            RequiredSkills = reader.GetString(4),
            Location = reader.GetString(5),
            EmploymentType = reader.GetString(6),
            SalaryStatus = reader.GetString(7),
            SalaryMin = reader.GetInt32(8),
            SalaryMax = reader.GetInt32(9),
            PostedAt = reader.GetDateTime(10),
            Status = reader.GetString(11),

            Employer = (Employer)data.dict[typeof(Employer)].FirstOrDefault(x => reader.GetInt32(1) == ((Employer)x).EmployerId)
        };
    }

    public Dictionary<string, string> GetFields()
    {
        return new()
        {
            { "EmployerId", "Employer" },
            { "Title", "string" },
            { "Description", "string" },
            { "RequiredSkills", "string" },
            { "Location", "string" },
            { "EmploymentType", "string" },
            { "SalaryStatus", "string" },
            { "SalaryMin", "int" },
            { "SalaryMax", "int" },
            { "PostedAt", "DateTime" },
            { "Status", "string" }
        };
    }

    public object GetField(string fieldName)
    {
        switch (fieldName)
        {
            case "EmployerId": return EmployerId;
            case "Title": return Title;
            case "Description": return Description;
            case "RequiredSkills": return RequiredSkills;
            case "Location": return Location;
            case "EmploymentType": return EmploymentType;
            case "SalaryStatus": return SalaryStatus;
            case "SalaryMin": return SalaryMin;
            case "SalaryMax": return SalaryMax;
            case "PostedAt": return PostedAt;
            case "Status": return Status;
            default: return null;
        }
    }

    public void SetFields(Dictionary<string, string> fields)
    {
        EmployerId = int.Parse(fields["EmployerId"]);
        Title = fields["Title"];
        Description = fields["Description"];
        RequiredSkills = fields["RequiredSkills"];
        Location = fields["Location"];
        EmploymentType = fields["EmploymentType"];
        SalaryStatus = fields["SalaryStatus"];
        SalaryMin = int.Parse(fields["SalaryMin"]);
        SalaryMax = int.Parse(fields["SalaryMax"]);
        PostedAt = DateTime.Parse(fields["PostedAt"]);
        Status = fields["Status"];

        Employer = (Employer)_database.dict[typeof(Employer)].FirstOrDefault(x => EmployerId == ((Employer)x).EmployerId);
    }

    public bool MatchesSearch(string search)
    {
        return Title.ToLower().Contains(search) || Description.ToLower().Contains(search) || RequiredSkills.ToLower().Contains(search) || Location.ToLower().Contains(search) || EmploymentType.ToLower().Contains(search) || SalaryStatus.ToLower().Contains(search) || SalaryMin.ToString().Contains(search) || SalaryMax.ToString().Contains(search) || PostedAt.ToString().Contains(search) || Status.ToLower().Contains(search);
    }

    public int GetPrimaryKey() => JobListingId;

    public string ObjName => Title;

    public Dictionary<string, string> Desc => new()
    {
        { "EmployerId", Employer.ObjName },
        { "Title", Title },
        { "Description", Description },
        { "RequiredSkills", RequiredSkills },
        { "Location", Location },
        { "EmploymentType", EmploymentType },
        { "SalaryStatus", SalaryStatus },
        { "SalaryMin", SalaryMin.ToString() },
        { "SalaryMax", SalaryMax.ToString() },
        { "PostedAt", PostedAt.ToString() },
        { "Status", Status }
    };
}