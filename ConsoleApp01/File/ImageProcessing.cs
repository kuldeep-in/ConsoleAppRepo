
namespace ConsoleApp01
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using MetadataExtractor;
    using System.Linq;

    //using Microsoft.WindowsAPICodePack.Shell;

    public static class ImageProcessing
    {
        public static void RenameImagesByDate()
        {
            int i = 0;
            var dateList = new List<DateTime>();
            //foreach (string sourceImage in System.IO.Directory.GetFiles("C://Users//HP//Downloads//Norway/", "*.jpg"))
            foreach (string sourceImage in System.IO.Directory.GetFiles("C://Users//insin//Downloads//iphone//20/"))
            {
                i++;
                if (sourceImage.EndsWith(".jpg") || sourceImage.EndsWith(".jpeg") || sourceImage.EndsWith(".JPG"))
                {
                    DateTime dt = GetImageCreatedDate(sourceImage, "ImageDate");
                    string targetfileName = "";

                    // if no date found in metadata compare with default value returned from function
                    if (dt.ToString("ddMMyyyy") == "11011950")
                    {
                        targetfileName = System.IO.Path.GetFileName(sourceImage).ToString();
                        targetfileName = targetfileName.Remove(0, 4);
                        Console.WriteLine($"{sourceImage} -{i}-> {targetfileName} <NoDate>");
                    }
                    // if date found in metadata
                    else
                    {
                        // if file with the same data exists in target, increamenting generated date
                        while (dateList.Contains(dt))
                        {
                            dt = dt.AddSeconds(1);
                            Console.WriteLine($"Duplicate Date {dt} for {sourceImage}");
                        }
                        dateList.Add(dt);

                        targetfileName = $"{dt:yyyyMMdd_HHmmss}.jpg";
                        Console.WriteLine($"{sourceImage} -{i}-> {targetfileName}");

                    }
                    string filePath = Path.Combine("C:", "Users", "insin", "Downloads", "op", targetfileName);
                    System.IO.File.Copy(sourceImage, filePath, true);
                }
                else
                {
                    Console.WriteLine($"{sourceImage} -{i}- >>>>>>>>>>>>");
                }
            }
            Console.ReadLine();
        }

        public static void RenameMOVbyDate()
        {
            int i = 0;
            var dateList = new List<DateTime>();
            foreach (string sourceImage in System.IO.Directory.GetFiles("C://Users//insin//Downloads//iphone//20/"))
            {
                i++;
                if (sourceImage.EndsWith(".mov") || sourceImage.EndsWith(".MOV") || sourceImage.EndsWith(".mp4"))
                {
                    DateTime dt = GetImageCreatedDate(sourceImage, "MOVDate");
                    string targetfileName = "";

                    // if no date found in metadata compare with default value returned from function
                    if (dt.ToString("ddMMyyyy") == "11011950")
                    {
                        targetfileName = System.IO.Path.GetFileName(sourceImage).ToString();
                        targetfileName = targetfileName.Remove(0, 4);
                        Console.WriteLine($"{sourceImage} -{i}-> {targetfileName} <NoDate>");
                    }
                    // if date found in metadata
                    else
                    {
                        // if file with the same data exists in target, increamenting generated date
                        while (dateList.Contains(dt))
                        {
                            dt = dt.AddSeconds(1);
                            Console.WriteLine($"Duplicate Date {dt} for {sourceImage}");
                        }
                        dateList.Add(dt);

                        targetfileName = $"MOV_{dt:yyyyMMdd_HHmmss}.MOV";
                        Console.WriteLine($"{sourceImage} -{i}-> {targetfileName}");

                    }
                    string filePath = Path.Combine("C:", "Users", "insin", "Downloads", "op", targetfileName);
                    System.IO.File.Copy(sourceImage, filePath, true);
                }
                else
                {
                    Console.WriteLine($"{sourceImage} -{i}- >>>>>>>>>>>>");
                }

            }
            Console.ReadLine();
        }

        public static DateTime GetImageCreatedDate(string filePath, string metadatatype)
        {
            var metaDirectories = MetadataExtractor.ImageMetadataReader.ReadMetadata(filePath);
            //foreach (var directory in metaDirectories)
            //    foreach (var tag in directory.Tags)
            //        Console.WriteLine($"-------{directory.Name} - {tag.Name} = {tag.Description}");
            try
            {
                if (metadatatype == "ImageDate")
                {
                    foreach (var directory in metaDirectories)
                    {
                        foreach (var tag in directory.Tags)
                        {
                            if (tag.Name == "Date/Time Original")
                            {
                                if (string.IsNullOrEmpty(tag.Description))
                                    continue;
                                string d = tag.Description.Split(" ")[0].Replace(":", "-");
                                string t = tag.Description.Split(" ")[1];
                                return DateTime.Parse($"{d} {t}");
                            }
                        }
                    }
                }
                else if (metadatatype == "MOVDate")
                {
                    foreach (var directory in metaDirectories.Where(x => x.Name == "QuickTime Metadata Header"))
                        foreach (var tag in directory.Tags)
                        {
                            if (tag.Name == "Creation Date")
                            {
                                if (string.IsNullOrEmpty(tag.Description))
                                    continue;
                                string d = tag.Description.Split(" ")[0].Replace(":", "-");
                                string dd = tag.Description.Split(" ")[2];
                                string MMM = tag.Description.Split(" ")[1];
                                string yyyy = tag.Description.Split(" ")[5];
                                string t = tag.Description.Split(" ")[3];
                                return DateTime.Parse($"{dd} {MMM} {yyyy} {t}");
                            }
                        }
                }
            }
            catch (System.Exception)
            {

                return DateTime.Parse("Jan 11, 1950");
            }
            return DateTime.Parse("Jan 11, 1950");

            //throw new InvalidOperationException($"Date not found in {filePath}");
        }
    }
}
