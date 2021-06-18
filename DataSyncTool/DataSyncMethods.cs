using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;

namespace DataSyncTool
{
    internal class DataSyncMethods
    {
        public List<string> getTableColumns(string tableName)
        {
            List<string> listOfcolumns = new List<string>();
            using (
                SqlConnection connection =
                    new SqlConnection(ConfigurationManager.ConnectionStrings["sqlConnection"].ToString()))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    string.Format(
                        "SELECT c.name AS column_name, i.name AS index_name, c.is_identity  FROM sys.indexes i inner join sys.index_columns ic  ON i.object_id = ic.object_id AND i.index_id = ic.index_id  inner join sys.columns c ON ic.object_id = c.object_id AND c.column_id = ic.column_id WHERE i.is_primary_key = 1  and i.object_ID = OBJECT_ID('{0}') ",
                        tableName);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listOfcolumns.Add(reader.GetString(0));
                    }
                }
                command.CommandText =
                    string.Format(
                        "select c.name from sys.columns c inner join sys.tables t on t.object_id = c.object_id and t.name = '{0}' and t.type = 'U'",
                        tableName);
                //connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!listOfcolumns.Contains(reader.GetString(0)))
                            listOfcolumns.Add(reader.GetString(0));
                    }
                }
            }

            return listOfcolumns;
        }

        public string getColumnDataType(string tableName, string columnName)
        {
            String dataType = null;
            using (
                SqlConnection connection =
                    new SqlConnection(ConfigurationManager.ConnectionStrings["sqlConnection"].ToString()))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    string.Format(
                        "SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME ='{0}' AND COLUMN_NAME = '{1}'",
                        tableName, columnName);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataType = reader.GetString(0);
                    }
                }
            }
            return dataType;

        }

        public DataSet generateJoin(string tableName)
        {
            using (
                SqlConnection connection =
                    new SqlConnection(ConfigurationManager.ConnectionStrings["sqlConnection"].ToString()))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    string.Format(
                        "SELECT OBJECT_NAME (FK.referenced_object_id) 'ReferencedTable', COL_NAME(FK.referenced_object_id, FKC.referenced_column_id) 'ReferencedColumn', COL_NAME(FK.parent_object_id,FKC.parent_column_id) 'ReferringColumn', OBJECT_NAME(FK.parent_object_id) 'ReferringTable' FROM sys.foreign_keys AS FK     INNER JOIN sys.foreign_key_columns AS FKC  ON FKC.constraint_object_id = FK.OBJECT_ID WHERE FK.parent_object_id = OBJECT_ID('{0}') ",
                        tableName);

                connection.Open();

                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(ds);

                return ds;
            }
        }

        public string getPrimaryKeyColumnName(string tableName)
        {
            string name = null;
            using (
                SqlConnection connection =
                    new SqlConnection(ConfigurationManager.ConnectionStrings["sqlConnection"].ToString()))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    string.Format(
                        "select COLUMN_NAME from information_schema.KEY_COLUMN_USAGE WHERE TABLE_NAME = '{0}' AND CONSTRAINT_NAME LIKE 'P%' ",
                        tableName);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        name = reader.GetString(0);
                    }
                }
            }
            return name;
        }

        public List<string> getTableNames()
        {
            List<string> listOfcolumns = new List<string>();
            using (
                SqlConnection connection =
                    new SqlConnection(ConfigurationManager.ConnectionStrings["sqlConnection"].ToString()))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    string.Format(
                        "SELECT name FROM sys.Tables");
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listOfcolumns.Add(reader.GetString(0));
                    }
                }
            }
            return listOfcolumns;
        }
    }
}


