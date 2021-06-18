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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.IO;


namespace DataSyncTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Maps : Window
    {
        public Maps()
        {
            try
            {
                InitializeComponent();
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["sqlConnection"].ToString()))
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "select name from sys.tables where type = 'U'";
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            if(reader.GetString(0).Contains("Lookup") || reader.GetString(0).Contains("LKUP"))
                            { }
                            else
                            { lbAvailableTables.Items.Add(reader.GetString(0)); }
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
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

    private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            if (txtTarget.Text == null || txtTarget.Text == "")
            {
                MessageBox.Show("Please select the Target Path");
            }
            else
            {
                foreach (object item in lbAvailableTables.SelectedItems)
                {
                    StringBuilder sbViewScript = new StringBuilder();
                    StringBuilder sb=new StringBuilder();
                    string tName = item.ToString();
                    tName = tName.Replace("_", "");
                    sbViewScript.AppendLine(string.Format("CREATE VIEW [dbo].[View{0}Sync]", tName));
                    sbViewScript.AppendLine("AS");
                    sbViewScript.AppendLine("\tSELECT");
                    bool firstColumn = true;
                    bool includeEngagementId = false;
                    DataSyncMethods dsm = new DataSyncMethods();
                    List<string> columns = dsm.getTableColumns(item.ToString());
                    string primaryKeyColumnName = dsm.getPrimaryKeyColumnName(item.ToString());
                    foreach (string column in columns)
                    {
                        // add indentation
                        string DataTypeOfColumn = dsm.getColumnDataType(item.ToString(), column);
                        string columnAlias = column;
                        //string primaryKeyColumnAlias
                        string primaryKeyColumnAlias = rbClient.IsChecked==true ? "[id]":"[Id]";
                        columnAlias = column.Replace("_", "");
                        if (columnAlias == "EngagementMasterId")
                        {
                            columnAlias = "EngagementId";
                        }
                       
                 
                        if (firstColumn)
                        {
                            firstColumn = false;
                            if (DataTypeOfColumn.ToLower() == "uniqueidentifier")
                            {
                                if (column == primaryKeyColumnName)
                                {
                                        sbViewScript.Append("\t\t");
                                        sbViewScript.AppendLine(string.Format("CAST ([{0}].[{1}] AS VARCHAR(36)) AS {2}  ",
                                        item.ToString(), column.ToString(), primaryKeyColumnAlias));
                                }
                                else
                                {
                                         sbViewScript.Append("\t\t");
                                        sbViewScript.AppendLine(string.Format("CAST ([{0}].[{1}] AS VARCHAR(36)) AS {2} ",
                                        item.ToString(), column.ToString(), columnAlias));
                                }

                            }
                            else
                            {
                                if (column != "ClusterId")
                                {
                                    sbViewScript.Append("\t\t");
                                    sbViewScript.AppendLine(string.Format("[{0}].[{1}]", item.ToString(), column.ToString()));
                                }
                                
                                
                            }

                        }
                        else
                        {
                           
    
                            if (DataTypeOfColumn.ToLower() == "uniqueidentifier")
                            {
                                sbViewScript.Append("\t\t");
                                sbViewScript.Append(",");
                                if (column == primaryKeyColumnName)
                                {
                                    sbViewScript.AppendLine(string.Format("CAST ([{0}].[{1}] AS VARCHAR(36)) AS {2} ", item.ToString(), column.ToString(), primaryKeyColumnAlias));
                                }
                                else
                                {
                                    sbViewScript.AppendLine(string.Format("CAST ([{0}].[{1}] AS VARCHAR(36)) AS {2} ", item.ToString(), column.ToString(), columnAlias));
                                }
                                    
                            }
                            else
                            {
                                if (column == "IsDeleted")
                                {
                                    sbViewScript.Append("\t\t");
                                    sbViewScript.Append(",");
                                    sbViewScript.AppendLine(string.Format("[{0}].[{1}] AS [Deleted]", item.ToString(), column.ToString()));
                                }
                                else
                                {
                                    if (column != "ClusterId")
                                    {
                                        sbViewScript.Append("\t\t");
                                        sbViewScript.Append(",");
                                        sbViewScript.AppendLine(string.Format("[{0}].[{1}]", item.ToString(), column.ToString()));
                                    }
                                        
                                }
                                
                            }
                            
                        }

                    }

                    if(rbClient.IsChecked==true)
                    { sbViewScript.AppendLine("\t\t,0 AS [IsLocal]"); }

                    if (rbServer.IsChecked == true)
                    {
                        sbViewScript.AppendLine(string.Format("\t\t,[{0}].[{1}]",item.ToString(),"CreatedAt"));
                        sbViewScript.AppendLine(string.Format("\t\t,[{0}].[{1}]", item.ToString(), "UpdatedAt"));
                    }
                


                if (columns.Contains("EngagementId")|| columns.Contains("Engagement_Id") ||columns.Contains("Engagement_Master_Id") || columns.Contains("Engagement_ID"))
                    {
                        includeEngagementId = true;
                    }

                    if (includeEngagementId)
                    {
                       // sbViewScript.AppendLine("\t\t,[EngagementId]");
                        sbViewScript.AppendLine("\tFROM");
                        sbViewScript.AppendLine(string.Format("\t\t[{0}]", item.ToString()));
                    }

                    else
                    {
                       sb= recursiveFunction(item.ToString(),dsm);
                        if (sb.Length > 0)
                        {
                            sbViewScript.AppendLine(sb.ToString());
                        }
                        else
                        {
                            sbViewScript.AppendLine("\tFROM");
                            sbViewScript.AppendLine(item.ToString());
                        }
                    }
               
               

                    sbViewScript.AppendLine("GO");

                    string filePath = txtTarget.Text + "\\View" + tName+ "Sync"+".sql";
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    File.Create(filePath).Dispose();
                    using (TextWriter tw = new StreamWriter(filePath))
                    {
                        tw.WriteLine(sbViewScript);
                        tw.Close();
                    }
                }
            }
        }
        StringBuilder sbTrack = new StringBuilder();
        private string originalDataSetName = null;
        int iteration = 0;
        private StringBuilder recursiveFunction(string tableName, DataSyncMethods dsm)
        {
            string[] game;
            string duplicateTableName = tableName;



            if (duplicateTableName.Contains("."))
            {
                char[] delimiterChars = {'.'};
                game = duplicateTableName.Split(delimiterChars);
                duplicateTableName = game[0];
                if(sbTrack.Length==0)
                { 
                sbTrack.AppendLine("\tFROM");
                sbTrack.AppendLine(string.Format("\t\t[{0}]", game[2]));
                }
                sbTrack.AppendLine(String.Format(" INNER JOIN {0} ON {1}.{2}={3}.{4} ", game[0],
                    game[0], game[1], game[2], game[3]));
                
            }
            else
            {
                originalDataSetName = duplicateTableName;}
      

            DataSet ds = dsm.generateJoin(duplicateTableName.ToString());
            ds.DataSetName = duplicateTableName;
            bool carryOn= true;
            StringBuilder sb=new StringBuilder();
             string[] words;
            string joinTable, joinColumn, origTable, origColumn;
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (carryOn)
                    {                    
                        string referencedTable = row["ReferencedTable"].ToString();
                        string referencedColumn = row["ReferencedColumn"].ToString();
                        string referencingColumn = row["ReferringColumn"].ToString();
                        string referencingTable = row["ReferringTable"].ToString();

                        if (!string.IsNullOrEmpty(referencedColumn))
                        {
                            if (referencedColumn=="EngagementId" ||
                                referencedColumn=="Engagement_Id" ||
                                referencedColumn=="Engagement_Master_Id")
                            {
                                   carryOn = false;

                                if (tableName.Contains("."))
                                {
                                    char[] delimiterChars = { '.' };

                                    words = tableName.Split(delimiterChars);
                                    joinTable = words[0];
                                    joinColumn = words[1];
                                    origTable = words[2];
                                    origColumn = words[3];
                                    sb.AppendLine(String.Format("\t\t,CAST([{0}] AS VARCHAR(36)) AS [EngagementId]", referencingColumn));
                                    //sb.AppendLine("\tFROM");
                                    //sb.AppendLine(string.Format("\t\t[{0}]", origTable));
                                    //sb.AppendLine(String.Format(" INNER JOIN {0} ON {1}.{2}={3}.{4} ", joinTable,
                                    //    joinTable, joinColumn, origTable, origColumn));
                                    sb.AppendLine(sbTrack.ToString());
                                }
                                else
                                {
                                    sb.AppendLine(String.Format("\t\t,[{0}]", referencedColumn));
                                    sb.AppendLine("\tFROM");
                                    sb.AppendLine(string.Format("\t\t[{0}]", referencingColumn));
                                    sb.AppendLine(String.Format(" INNER JOIN {0} ON {1}.{2}={3}.{4} ", referencingTable,
                                        referencedTable, referencedColumn, referencingTable, referencingColumn));
                                }

                            }

                        }

                    }
                }

                if(carryOn)
                   { 
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (ds.DataSetName == originalDataSetName)
                    {
                        sbTrack.Clear();
                    }
                        if (carryOn)
                          { 
                            string referencedTable = row["ReferencedTable"].ToString();
                            string referencedColumn = row["ReferencedColumn"].ToString();
                            string referencingColumn = row["ReferringColumn"].ToString();
                            string referencingTable = row["ReferringTable"].ToString();
                         
                             if(referencedTable!= referencingTable)
                                sb= recursiveFunction(referencedTable+'.'+ referencedColumn+'.'+ referencingTable+'.'+ referencingColumn, dsm);
                      
                          
                            if (sb != null && sb.Length > 0)
                              {
                                  carryOn = false;
                              }
                          }

                        }
                   }

            }

            else
            {
                           
                List<string> columns = dsm.getTableColumns(duplicateTableName);
                
                if (columns.Contains("EngagementId") || columns.Contains("Engagement_Id") ||
                    columns.Contains("Engagement_Master_Id"))
                 {
                    carryOn = false;
                    if (tableName.Contains("."))
                    {
                        string referencedColumn=null;
                        char[] delimiterChars = { '.' };
                        if (columns.Contains("EngagementId"))
                        {
                            referencedColumn = "EngagementId";
                        }
                        if (columns.Contains("Engagement_Id"))
                        { referencedColumn = "Engagement_Id"; }

                        if (columns.Contains("Engagement_Master_Id"))
                        { referencedColumn = "Engagement_Master_Id"; }


                        words = tableName.Split(delimiterChars);
                        joinTable = words[0];
                        joinColumn = words[1];
                        origTable = words[2];
                        origColumn = words[3];
                        sb.AppendLine(String.Format("\t\t,CAST([{0}] AS VARCHAR(36)) AS [EngagementId]", referencedColumn));                     
                        sb.AppendLine(sbTrack.ToString());
                    }


                }

                if (tableName == "Account")
                {
                    sb.AppendLine(String.Format("\t\t,CAST([{0}] AS VARCHAR(36)) AS [EngagementId]", "Engagement_Master_Id"));
                    sb.AppendLine("\tFROM");
                    sb.AppendLine(string.Format("\t\t[{0}]", tableName));
                    sb.AppendLine(String.Format(" INNER JOIN Entity_Engagement ON Entity_Engagement.Entity_Engagement_Id=Account.Entity_Engagement_Id"));
                }
            }
            return sb;
        }


        private
            void lbAvailableTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        
    }
}
