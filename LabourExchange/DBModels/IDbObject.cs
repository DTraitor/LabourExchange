using System.ComponentModel;
using Microsoft.Data.SqlClient;

namespace LabourExchange.DBModels;

public interface IDbObject
{
    public string GetCreateTableQuery();
    public string GetSelectAll();
    public SqlCommand GetDeleteQuery();
    public SqlCommand GetInsertUpdateQuery();
    public IDbObject CreateFromReader(SqlDataReader reader, Database data);
    public Dictionary<string, string> GetFields();
    public object GetField(string fieldName);
    public void SetFields(Dictionary<string, string> fields);
    public bool MatchesSearch(string search);
    public int GetPrimaryKey();

    public string ObjName { get; }

    public Dictionary<string, string> Desc { get; }
}