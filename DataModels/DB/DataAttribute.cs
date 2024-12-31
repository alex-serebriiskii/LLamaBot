using System.Runtime.CompilerServices;

namespace SchizoLlamaBot.DataModels.DB
{
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryField: Attribute
    {
        public string FieldName {get; set;}
        public string ColumnName {get; set;}
        public QueryField(string columnName, [CallerMemberName] string fieldName = "") 
        {
            FieldName = fieldName;
            ColumnName = columnName;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TableName: Attribute
    {
        public string Value{get; set;}
        public TableName(string tableName)
        {
            Value = tableName;
        }
    }
}