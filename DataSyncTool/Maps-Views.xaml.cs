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
using System.IO;


namespace DataSyncTool
{
    /// <summary>
    /// Interaction logic for Maps_Views.xaml
    /// </summary>
    public partial class Maps_Views : Page
    {
        public Maps_Views()
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
                            lbAvailableTables.Items.Add(reader.GetString(0));
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
                    //Comment on View
                    sbViewScript.AppendLine("--------------------------------------------------------------------------------------------------------------------");
                    sbViewScript.AppendLine(string.Format("-- <copyright file='View{0}.sql' company='GTI'>",item.ToString()));
                    sbViewScript.AppendLine("--   Copyright © 2016 Microsoft.");
                    sbViewScript.AppendLine("-- </copyright>");
                    sbViewScript.AppendLine("-- <summary>");
                    sbViewScript.AppendLine(string.Format("--   Creates view on {0} table.",item.ToString().ToLower()));
                    sbViewScript.AppendLine("-- </summary> ");
                    sbViewScript.AppendLine("--------------------------------------------------------------------------------------------------------------------");



                    //Actual View content
                    sbViewScript.AppendLine(string.Format("CREATE VIEW [dbo].[View{0}]", item.ToString()));
                    sbViewScript.AppendLine("AS");
                    sbViewScript.AppendLine("\tSELECT");
                    bool firstColumn = true;

                    DataSyncMethods dsm = new DataSyncMethods();
                    List<string> columns = dsm.getTableColumns(item.ToString());
                    foreach (string column in columns)
                    {
                        // add indentation
                        sbViewScript.Append("\t\t");
                        if (firstColumn)
                        {
                            firstColumn = false;
                            sbViewScript.AppendLine(string.Format("[{0}]", column.ToString()));
                            //sbViewScript.Append("AS [id]");
                        }
                        else
                        {
                            sbViewScript.Append(",");
                            sbViewScript.AppendLine(string.Format("[{0}]", column.ToString()));
                        }

                    }             
                    if(!columns.Contains("IsDeleted"))
                    {
                        sbViewScript.AppendLine("\t\t,[IsDeleted]");

                    }
                           

                    sbViewScript.AppendLine("\tFROM");
                    sbViewScript.AppendLine(string.Format("\t\t[{0}]", item.ToString()));
                    sbViewScript.AppendLine("\t WHERE [IsDeleted] = 0");
                    sbViewScript.AppendLine("GO");

                    string filePath = txtTarget.Text + "\\View" + item.ToString() + ".sql";
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

        private void lbAvailableTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


    }
}
