using System.ComponentModel;
using Microsoft.Data.SqlClient;

namespace LabourExchange.DBModels;

public class Application : IDbObject
{
    private Database _database;

    public int ApplicationId { get; set; }
    public int SeekerId { get; set; }
    public int JobListingId { get; set; }
    public DateTime ApplicationDate { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }

    public JobListing JobListing { get; set; }
    public JobSeeker JobSeeker { get; set; }

    public Application() { }

    public Application(Database data)
    {
        _database = data;
    }

    public string GetCreateTableQuery()
    {
        return @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Applications' AND xtype='U')
            CREATE TABLE Applications (
                ApplicationId INT PRIMARY KEY IDENTITY,
                JobSeekerId INT FOREIGN KEY REFERENCES JobSeekers(JobSeekerId),
                JobListingId INT FOREIGN KEY REFERENCES JobListings(JobListingId),
                ApplicationDate DATETIME NOT NULL,
                Status NVARCHAR(255) NOT NULL,
                Message NVARCHAR(255) NOT NULL
            )";
    }

    public string GetSelectAll()
    {
        return "SELECT * FROM Applications";
    }

    public SqlCommand GetDeleteQuery()
    {
        SqlCommand command = new SqlCommand("DELETE FROM Applications WHERE ApplicationId = @ApplicationId");
        command.Parameters.Add(new SqlParameter("@ApplicationId", ApplicationId));
        return command;
    }

    public SqlCommand GetInsertUpdateQuery()
    {
        SqlCommand command = new SqlCommand(@"
            IF NOT EXISTS (SELECT * FROM Applications WHERE ApplicationId = @ApplicationId)
            BEGIN
                INSERT INTO Applications (JobSeekerId, JobListingId, ApplicationDate, Status, Message)
                VALUES (@JobSeekerId, @JobListingId, @ApplicationDate, @Status, @Message)
            END
            ELSE
            BEGIN
                UPDATE Applications
                SET JobSeekerId = @JobSeekerId, JobListingId = @JobListingId, ApplicationDate = @ApplicationDate, Status = @Status, Message = @Message
                WHERE ApplicationId = @ApplicationId
            END");
        command.Parameters.Add(new SqlParameter("@ApplicationId", ApplicationId));
        command.Parameters.Add(new SqlParameter("@JobSeekerId", JobSeeker.SeekerId));
        command.Parameters.Add(new SqlParameter("@JobListingId", JobListing.JobListingId));
        command.Parameters.Add(new SqlParameter("@ApplicationDate", ApplicationDate));
        command.Parameters.Add(new SqlParameter("@Status", Status));
        command.Parameters.Add(new SqlParameter("@Message", Message));
        return command;
        ;
    }

    public IDbObject CreateFromReader(SqlDataReader reader, Database data)
    {
        return new Application()
        {
            _database = data,
            ApplicationId = reader.GetInt32(0),
            SeekerId = reader.GetInt32(1),
            JobListingId = reader.GetInt32(2),
            ApplicationDate = reader.GetDateTime(3),
            Status = reader.GetString(4),
            Message = reader.GetString(5),

            JobSeeker = (JobSeeker)data.dict[typeof(JobSeeker)].FirstOrDefault(x => reader.GetInt32(1) == ((JobSeeker)x).SeekerId),
            JobListing = (JobListing)data.dict[typeof(JobListing)].FirstOrDefault(x => reader.GetInt32(2) == ((JobListing)x).JobListingId)
        };
    }

    public Dictionary<string, string> GetFields()
    {
        return new()
        {
            { "SeekerId", "JobSeeker" },
            { "JobListingId", "JobListing" },
            { "ApplicationDate", "DateTime" },
            { "Status", "string" },
            { "Message", "string" }
        };
    }

    public object GetField(string fieldName)
    {
        switch (fieldName)
        {
            case "SeekerId": return SeekerId;
            case "JobListingId": return JobListingId;
            case "ApplicationDate": return ApplicationDate;
            case "Status": return Status;
            case "Message": return Message;
            default: return null;
        }
    }

    public void SetFields(Dictionary<string, string> fields)
    {
        SeekerId = int.Parse(fields["SeekerId"]);
        JobListingId = int.Parse(fields["JobListingId"]);
        ApplicationDate = DateTime.Parse(fields["ApplicationDate"]);
        Status = fields["Status"];
        Message = fields["Message"];

        JobSeeker = (JobSeeker)_database.dict[typeof(JobSeeker)].FirstOrDefault(x => SeekerId == ((JobSeeker)x).SeekerId);
        JobListing = (JobListing)_database.dict[typeof(JobListing)].FirstOrDefault(x => JobListingId == ((JobListing)x).JobListingId);
    }

    public bool MatchesSearch(string search)
    {
        return JobSeeker.MatchesSearch(search) || JobListing.MatchesSearch(search) || ApplicationDate.ToString().Contains(search) || Status.ToLower().Contains(search) || Message.ToLower().Contains(search);
    }

    public int GetPrimaryKey() => ApplicationId;

    public string ObjName => "Application";

    public Dictionary<string, string> Desc => new()
    {
        { "SeekerId", JobSeeker.ObjName },
        { "JobListingId", JobListing.ObjName },
        { "ApplicationDate", ApplicationDate.ToString() },
        { "Status", Status },
        { "Message", Message }
    };
}
