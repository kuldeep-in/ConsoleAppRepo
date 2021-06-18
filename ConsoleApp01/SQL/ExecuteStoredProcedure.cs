using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace ConsoleApp01.SQL
{
    class ExecuteStoredProcedure
    {
        private static void WriteDatasetToTextFile()
        {
            DataSet ds = new DataSet();
            using (SqlConnection connection = new SqlConnection("Server=gti-maps-tststab.database.windows.net,1433;Database=Gti.Database.Maps;User ID=dbadmin;Password=Sup3rC0mpl3x!;Encrypt=False;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[UspGetGuidanceSearchResults]", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@memberFirmCategoryElementId", 39272);
                    command.Parameters.AddWithValue("@languageid", 1033);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);

                    adapter.Fill(ds);
                    WriteDatasetToTextFile(ds);
                }
            }
        }

        private static void WriteDatasetToTextFile(DataSet ds)
        {
            StringBuilder content = new StringBuilder();

            foreach (DataTable tbl in ds.Tables)
            {
                content.AppendLine(tbl.TableName);
                foreach (DataColumn col in tbl.Columns)
                {
                    content.Append($"{col.ColumnName}|");
                }
                content.AppendLine();
                foreach (DataRow row in tbl.Rows)
                {
                    foreach (DataColumn col in tbl.Columns)
                    {
                        content.Append($"{row[col]}|");
                    }
                    content.AppendLine();
                }
            }
            System.IO.File.WriteAllText("D:\\Temp\\QueryOutput.txt", content.ToString());
        }
    }
}
