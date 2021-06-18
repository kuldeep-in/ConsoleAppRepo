using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace DataSyncTool
{
    /// <summary>
    /// Interaction logic for GetTriggersFromViews.xaml
    /// </summary>
    public partial class GetTriggersFromViews : Window
    {
        public GetTriggersFromViews()
        {
            try {
                InitializeComponent();
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["sqlConnection"].ToString()))
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "select SCHEMA_NAME(schema_id) AS [schema],name AS [view] from sys.views where type = 'V'";
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if(reader.GetString(1).Contains("Sync"))
                            lbAvailableViews.Items.Add(reader.GetString(1));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void btnGetInsteadOfInsert_Click(object sender, RoutedEventArgs e)
        {
            if (txtTarget.Text == null || txtTarget.Text == "")
            {
                MessageBox.Show("Please select the Target Path");
            }
            else {
                foreach (object item in lbAvailableViews.SelectedItems)
                {
                    string tableName = item.ToString().Substring(4, item.ToString().Length - 8);
                    StringBuilder sbScript = new StringBuilder();

                    DataSyncMethods dsm = new DataSyncMethods();
                    List<string> tableNames = dsm.getTableNames();
                    bool tableExists = false;

                    foreach (string tName in tableNames)
                    {
                        if (!tableExists)
                        {
                            string tbName = tName;
                            tbName = tbName.Replace("_", "");
                            if (tableName == tbName)
                            {
                                tableName = tName;
                                tableExists = true;
                            }
                        }

                    }

                    sbScript.AppendLine(string.Format("CREATE TRIGGER [utrgInsteadOfInsert_{0}]", item.ToString()));
                    sbScript.AppendLine(string.Format("\tON [{0}]", item.ToString()));
                    sbScript.AppendLine("INSTEAD OF INSERT");
                    sbScript.AppendLine("AS");
                    sbScript.AppendLine("\tSET NOCOUNT ON;");
                    sbScript.AppendLine("\tDECLARE @timestamp DATETIME = getutcdate();");

                    if (rbClient.IsChecked == true)
                    {
                        sbScript.AppendLine("\tDECLARE @sequence BIGINT");
                        sbScript.AppendLine("\tSELECT @sequence = (ISNULL(MAX(op.[sequence]), 0)) FROM [__operations] op");
                        sbScript.AppendLine("\tCREATE TABLE #sequence");
                        sbScript.AppendLine("\t(");
                        sbScript.AppendLine("\t[identity] BIGINT IDENTITY(1,1),");
                        sbScript.AppendLine("\t[sequence] BIGINT NULL,");
                        sbScript.AppendLine("\t[id] VARCHAR(36)");
                        sbScript.AppendLine("\t)");
                    }

                    sbScript.AppendLine("");
                    sbScript.AppendLine(string.Format("\tINSERT INTO [{0}]", tableName));
                    sbScript.AppendLine("\t(");

               


                    List<string> columns = dsm.getTableColumns(tableName);
                    string primaryKeyColumnName = dsm.getPrimaryKeyColumnName(tableName);
                    string insertPrimaryKeyColumn = null;
                    string createtdAtColumn = "CreatedAt";
                    string updatedAtColumn = "UpdatedAt";
                    StringBuilder sbInsertColumnList = new StringBuilder();
                    sbInsertColumnList.Append("\t");
                    StringBuilder sbSelectColumnList = new StringBuilder();
                    sbSelectColumnList.Append("\t\t");

                    bool isFirstColumn = true;

                    insertPrimaryKeyColumn = rbClient.IsChecked == true ? "id" : "Id";
                    foreach (string column in columns)
                    {
                        string columnAlias = column;
                        columnAlias = column.Replace("_", "");
                        if (columnAlias == "EngagementMasterId")
                        {
                            columnAlias = "EngagementId";
                        }
                        string DataTypeOfColumn = dsm.getColumnDataType(tableName, column);
                        if (isFirstColumn)
                        {
                            isFirstColumn = false;
                            if (column != "ClusterId" || column != "Version")
                            {
                                sbInsertColumnList.AppendLine(String.Format("\t[{0}].[{1}]", tableName, column.ToString()));
                                if (column == primaryKeyColumnName)
                                {
                                    sbSelectColumnList.AppendLine(string.Format("[inserted].[{0}]", insertPrimaryKeyColumn));
                                }
                                else
                                {

                                    if (DataTypeOfColumn.ToLower() == "uniqueidentifier")
                                    {
                                        sbSelectColumnList.AppendLine(String.Format("[inserted].[{0}]", columnAlias));
                                    }
                                    else
                                    {
                                        sbSelectColumnList.AppendLine(String.Format("[inserted].[{0}]", column));
                                    }

                                }
                            }
                        }
                        else
                        {
                            if (column == "ClusterId")
                            { }
                            else if(column=="Version" && rbServer.IsChecked==true)
                            { }

                            else
                            {
                                sbInsertColumnList.Append("\t,");
                                sbSelectColumnList.Append("\t\t,");

                                sbInsertColumnList.AppendLine(string.Format("\t[{0}].[{1}]", tableName, column.ToString()));
                                if (column == primaryKeyColumnName)
                                {
                                    sbSelectColumnList.AppendLine(string.Format("[inserted].[{0}]", insertPrimaryKeyColumn));
                                }
                                else if (column == "IsDeleted")
                                {
                                    sbSelectColumnList.AppendLine(string.Format("[inserted].[{0}]", "Deleted"));
                                }
                                else
                                {
                                    if (DataTypeOfColumn.ToLower() == "uniqueidentifier")
                                    {
                                        sbSelectColumnList.AppendLine(String.Format("[inserted].[{0}]", columnAlias));
                                    }
                                    else
                                    {
                                        if (rbClient.IsChecked == true && (column == "CreatedOn" || column == "ModifiedOn" || column == "Created_Date" ||
                                            column == "Modified_Date"))
                                        {
                                            sbSelectColumnList.AppendLine(String.Format("ISNULL([inserted].[{0}],@timestamp)", column));
                                        }
                                        else
                                        {
                                            sbSelectColumnList.AppendLine(String.Format("[inserted].[{0}]", column));
                                        }

                                    }

                                }

                            }

                        }

                    }

                    if (rbServer.IsChecked == true)
                    {
                        sbInsertColumnList.Append("\t,");
                        sbSelectColumnList.Append("\t\t,");
                        //CreatedAt column
                        sbInsertColumnList.AppendLine(string.Format("\t[{0}].[{1}]", tableName, createtdAtColumn));
                        sbSelectColumnList.AppendLine(String.Format("ISNULL([inserted].[{0}] ,@timestamp) AS [{1}]", createtdAtColumn, createtdAtColumn));
                        sbInsertColumnList.Append("\t,");
                        sbSelectColumnList.Append("\t\t,");
                        //UpdatedAt column
                        sbInsertColumnList.AppendLine(string.Format("\t[{0}].[{1}]", tableName, updatedAtColumn));
                        sbSelectColumnList.AppendLine(String.Format("ISNULL([inserted].[{0}] ,@timestamp) AS [{1}]", updatedAtColumn, updatedAtColumn));
                    }


                    sbScript.Append(sbInsertColumnList.ToString());
                    sbScript.AppendLine("\t)");
                    sbScript.AppendLine("\tSELECT");
                    sbScript.Append(sbSelectColumnList.ToString());
                    sbScript.AppendLine("\tFROM");
                    sbScript.AppendLine("\t\tinserted");

                    if (rbClient.IsChecked == true)
                    {
                        sbScript.AppendLine("INSERT INTO #sequence");
                        sbScript.AppendLine(" ( id )");
                        sbScript.AppendLine("SELECT inserted.[id] FROM inserted WHERE inserted.IsLocal = 1");
                        sbScript.AppendLine("");


                        sbScript.AppendLine("UPDATE #sequence SET [sequence] = @sequence+[identity]");

                        sbScript.AppendLine("");
                        sbScript.AppendLine("\tINSERT INTO [__operations] (id, kind, [state], tableName, tableKind, itemId, item, createdAt, [sequence], [version])");
                        sbScript.AppendLine(string.Format("\tSELECT NEWID(),0,0,'{0}',0,[id],'',@timestamp,[sequence],1", item.ToString()));
                        sbScript.AppendLine("\tFROM #sequence");
                    }
                    sbScript.AppendLine("");

                    string filePath = txtTarget.Text + "\\utrgInsteadOfInsert_" + item.ToString() + ".sql";
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    File.Create(filePath).Dispose();
                    using (TextWriter tw = new StreamWriter(filePath))
                    {
                        tw.WriteLine(sbScript);
                        tw.Close();
                    }
                }
            }

        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog dlg =
               new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtTarget.Text = dlg.SelectedPath;
                }

            }

        }

        private void btnGetInsteadOfUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (txtTarget.Text == null || txtTarget.Text == "")
            {
                MessageBox.Show("Please select the Target Path");
            }
            else
            {
                foreach (object item in lbAvailableViews.SelectedItems)
                {
                    string tableName = item.ToString().Substring(4, item.ToString().Length - 8);
                    StringBuilder sbScript = new StringBuilder();

                    DataSyncMethods dsm = new DataSyncMethods();

                    List<string> tableNames = dsm.getTableNames();
                    bool tableExists = false;

                    foreach (string tName in tableNames)
                    {
                        if (!tableExists)
                        {
                            string tbName = tName;
                            tbName = tbName.Replace("_", "");
                            if (tableName == tbName)
                            {
                                tableName = tName;
                                tableExists = true;
                            }
                        }

                    }

                    sbScript.AppendLine(string.Format("CREATE TRIGGER [utrgInsteadOfUpdate_{0}]", item.ToString()));
                    sbScript.AppendLine(string.Format("\tON [{0}]", item.ToString()));
                    sbScript.AppendLine("INSTEAD OF UPDATE");
                    sbScript.AppendLine("AS");
                    sbScript.AppendLine("\tSET NOCOUNT ON;");
                    sbScript.AppendLine("\t DECLARE @timestamp DATETIME = getutcdate();");

                    if (rbClient.IsChecked == true)
                    {
                        sbScript.AppendLine("\tDECLARE @sequence BIGINT");
                        sbScript.AppendLine("\tSELECT @sequence = (ISNULL(MAX(op.[sequence]), 0)) FROM [__operations] op");
                        sbScript.AppendLine("\tCREATE TABLE #sequence");
                        sbScript.AppendLine("\t(");
                        sbScript.AppendLine("\t[identity] BIGINT IDENTITY(1,1),");
                        sbScript.AppendLine("\t[sequence] BIGINT NULL,");
                        sbScript.AppendLine("\t[id] VARCHAR(36)");
                        sbScript.AppendLine("\t)");
                    }


                    sbScript.AppendLine("");
                    sbScript.AppendLine(string.Format("\tUPDATE [{0}]", tableName));
                    sbScript.AppendLine("\tSET");




                    List<string> columns = dsm.getTableColumns(tableName);
                    string primaryKeyColumnName = dsm.getPrimaryKeyColumnName(tableName);
                    StringBuilder sbUpdateColumnList = new StringBuilder();
                    sbUpdateColumnList.Append("\t");


                    bool isFirstColumn = true;
                    string firstColumn = string.Empty;
                    foreach (string column in columns)
                    {
                        string DataTypeOfColumn = dsm.getColumnDataType(tableName, column);
                        string columnAlias = column;
                        columnAlias = column.Replace("_", "");
                        if (columnAlias == "EngagementMasterId")
                        {
                            columnAlias = "EngagementId";
                        }

                    
                        if (column != "ClusterId")
                        {
                            if (column != primaryKeyColumnName)
                            {
                                if (column == "IsDeleted")
                                {
                                    if (sbUpdateColumnList.ToString().Trim().Length > 0)
                                    {
                                        sbUpdateColumnList.Append("\t,");
                                    }
                                    sbUpdateColumnList.AppendLine(string.Format("\t[{0}].[{1}] = I.[{2}]", tableName, column.ToString(), "Deleted"));
                                }
                                else if(DataTypeOfColumn.ToLower()=="uniqueidentifier")
                                {
                                    if (sbUpdateColumnList.ToString().Trim().Length > 0)
                                    {
                                        sbUpdateColumnList.Append("\t,");
                                    }
                                    sbUpdateColumnList.AppendLine(string.Format("\t[{0}].[{1}] = I.[{2}]", tableName, column,columnAlias));
                                }
                                else
                                {
                                      if (column == "Version" && rbServer.IsChecked == true)
                                    {
                                    }
                                    else
                                    {
                                        if (sbUpdateColumnList.ToString().Trim().Length > 0)
                                        {
                                            sbUpdateColumnList.Append("\t,");
                                        }
                                        sbUpdateColumnList.AppendLine(string.Format("\t[{0}].[{1}] = I.[{1}]", tableName, column.ToString()));
                                    }
                                        
                                }

                            }
                        }
                    }

                    if(rbServer.IsChecked==true)
                    {
                        sbUpdateColumnList.Append("\t,");
                        sbUpdateColumnList.AppendLine(string.Format("\t[{0}].[{1}] = ISNULL(I.[{1}],[{0}].[{1}])", tableName, "CreatedAt"));
                        sbUpdateColumnList.Append("\t,");
                        sbUpdateColumnList.AppendLine(string.Format("\t[{0}].[{1}] = CONVERT(DATETIMEOFFSET, @timestamp)", tableName, "UpdatedAt"));

                    }

                    sbScript.Append(sbUpdateColumnList.ToString());
                    // sbScript.AppendLine("\t)");
                    sbScript.AppendLine(string.Format("\tFROM [{0}]", tableName));
                    if(rbClient.IsChecked==true)
                    {
                        sbScript.AppendLine(string.Format("\tINNER JOIN inserted AS I ON I.id = [{0}].[{1}]", tableName, primaryKeyColumnName));
                    }
                    else
                    {
                        sbScript.AppendLine(string.Format("\tINNER JOIN inserted AS I ON I.Id = [{0}].[{1}]", tableName, primaryKeyColumnName));
                    }
                    

                    if (rbClient.IsChecked == true)
                    {
                        sbScript.AppendLine("");
                        sbScript.AppendLine("INSERT INTO #sequence");
                        sbScript.AppendLine(" ( id )");
                        sbScript.AppendLine("SELECT inserted.[id] FROM inserted WHERE inserted.IsLocal = 1");
                        sbScript.AppendLine("");


                        sbScript.AppendLine("UPDATE #sequence SET [sequence] = @sequence+[identity]");

                        sbScript.AppendLine("");

                        sbScript.AppendLine("MERGE [__operations] AS [Target]");
                        sbScript.AppendLine("USING #sequence AS [Source]");

                        sbScript.AppendLine(string.Format("ON ([Target].tableName = '{0}'",item.ToString()));
                        sbScript.AppendLine("AND [Source].[id] = [Target].itemId");
                        sbScript.AppendLine("AND [Target].kind IN (0,1)	");
                        sbScript.AppendLine(")");

                        sbScript.AppendLine("WHEN MATCHED THEN");
                        sbScript.AppendLine("UPDATE SET");
                        sbScript.AppendLine("[Target].[version] = ISNULL([Target].[version],0)+1");


                        sbScript.AppendLine(" WHEN NOT MATCHED THEN");
                        sbScript.AppendLine("INSERT  (id, kind, [state], tableName, tableKind, itemId, item, createdAt, [sequence], [version])");
                        sbScript.AppendLine(string.Format("VALUES ( NEWID(),1,0,'{0}',0,[Source].[id],NULL,@timestamp,[Source].[sequence],1",item.ToString()));
                        sbScript.AppendLine(");");                 

                    }
        
                    sbScript.AppendLine("");

                    string filePath = txtTarget.Text + "\\utrgInsteadOfUpdate_" + item.ToString() + ".sql";
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    File.Create(filePath).Dispose();
                    using (TextWriter tw = new StreamWriter(filePath))
                    {
                        tw.WriteLine(sbScript);
                        tw.Close();
                    }
                }
            }
        }



        private void btnGetInsteadOfDelete_Click(object sender, RoutedEventArgs e)
        {
            if (txtTarget.Text == null || txtTarget.Text == "")
            {
                MessageBox.Show("Please select the Target Path");
            }
            else
            {
                foreach (object item in lbAvailableViews.SelectedItems)
                {
                    string tableName = item.ToString().Substring(4, item.ToString().Length - 8);
                    StringBuilder sbScript = new StringBuilder();
                    DataSyncMethods dsm = new DataSyncMethods();
                    List<string> columns = dsm.getTableColumns(tableName);
                    string firstColumn = columns.First();

                    sbScript.AppendLine(string.Format("CREATE TRIGGER [utrgInsteadOfDelete_{0}]", item.ToString()));
                    sbScript.AppendLine(string.Format("\tON [{0}]", item.ToString()));
                    sbScript.AppendLine("INSTEAD OF DELETE");
                    sbScript.AppendLine("AS");
                    sbScript.AppendLine("\tSET NOCOUNT ON;");

                    sbScript.AppendLine("");
                    //sbScript.AppendLine("\tSET IDENTITY_INSERT [dbo].[GTIL_Entity] ON");
                    //sbScript.AppendLine("");
                    sbScript.AppendLine(string.Format("\tDELETE [{0}]", tableName));

                    sbScript.AppendLine(string.Format("\tFROM [{0}]", tableName));
                    sbScript.AppendLine(string.Format("\tINNER JOIN deleted AS D ON D.id = [{0}].[{1}]", tableName, firstColumn));

                    if (rbClient.IsChecked == true)
                    {
                        sbScript.AppendLine("");
                        sbScript.AppendLine("\tINSERT INTO [__operations] (id, kind, [state], tableName, tableKind, itemId, item, createdAt, [sequence], [version])");
                        sbScript.AppendLine(string.Format("\tSELECT NEWID(),2,0,[{0}],0,inserted.[id],NULL,GETUTCDATE(),(SELECT (ISNULL(MAX(op.[sequence]), 0) + 1) FROM [__operations] op) AS [sequence],1", item.ToString()));
                        sbScript.AppendLine("\tFROM inserted WHERE inserted.IsLocal = 1");
                    }
                    sbScript.AppendLine("");

                    string filePath = txtTarget.Text + "\\utrgInsteadOfDelete_" + item.ToString() + ".sql";
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    File.Create(filePath).Dispose();
                    using (TextWriter tw = new StreamWriter(filePath))
                    {
                        tw.WriteLine(sbScript);
                        tw.Close();
                    }
                }
            }
        }
    }
}

