
namespace ConsoleApp01.Outlook
{
    using Microsoft.Office.Interop.Outlook;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    class ReadOutlookEmail
    {
        private static void ReadMailItems()
        {
            Application outlookApplication = null;
            NameSpace outlookNamespace = null;
            //MAPIFolder inboxFolder = null;
            MAPIFolder inboxFolder1 = null;
            //Items mailItems = null;
            List<string> outputList = new List<string>();

            try
            {
                outlookApplication = new Application();
                outlookNamespace = outlookApplication.GetNamespace("MAPI");
                //inboxFolder = outlookNamespace.GetDefaultFolder(OlDefaultFolders.olFolderSentMail);
                //mailItems = inboxFolder.Items;


                inboxFolder1 = outlookNamespace.Folders["user@email.com"].Folders["Inbox"];

                foreach (object item in inboxFolder1.Items)
                {
                    try
                    {
                        if (item is Microsoft.Office.Interop.Outlook.MailItem)
                        {
                            Microsoft.Office.Interop.Outlook.MailItem mailitem = (Microsoft.Office.Interop.Outlook.MailItem)item;
                            if (mailitem.SenderEmailAddress.Contains("@microsoft.com"))
                            {
                                if (outputList.Contains(mailitem.SenderEmailAddress))
                                { }
                                else
                                {
                                    Console.WriteLine(mailitem.SenderEmailAddress);
                                    outputList.Add(mailitem.SenderEmailAddress);
                                }

                                Marshal.ReleaseComObject(item);
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {

                    }

                }

                System.IO.File.WriteAllLines("D:\\SavedLists.txt", outputList);
                Console.ReadLine();


                //foreach (MailItem item in mailItems)
                //{
                //    if (item.SenderEmailAddress.ToString().Contains("@microsoft.com"))
                //    {
                //        //var stringBuilder = new StringBuilder();
                //        //stringBuilder.AppendLine("From: " + item.SenderEmailAddress);
                //        //stringBuilder.AppendLine("To: " + item.To);
                //        //stringBuilder.AppendLine("CC: " + item.CC);
                //        //stringBuilder.AppendLine("");
                //        //stringBuilder.AppendLine("Subject: " + item.Subject);
                //        //stringBuilder.AppendLine(item.Body);

                //        Console.WriteLine(item.SenderEmailAddress.ToString());
                //        //Marshal.ReleaseComObject(item);
                //    }
                //}
            }
            catch (System.Exception ex)
            {

            }
            finally
            {

                //ReleaseComObject(mailItems);
                //ReleaseComObject(inboxFolder);
                //ReleaseComObject(outlookNamespace);
                //ReleaseComObject(outlookApplication);
            }
        }

    }
}
