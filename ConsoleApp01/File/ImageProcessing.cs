
namespace ConsoleApp01
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    //using Microsoft.WindowsAPICodePack.Shell;

    public static class ImageProcessing
    {
        public static void RenameImagesByDate()
        {
            int i = 0;
            var dateList = new List<DateTime>();
            //foreach (string sourceImage in System.IO.Directory.GetFiles("C://Users//HP//Downloads//Norway/", "*.jpg"))
            foreach (string sourceImage in System.IO.Directory.GetFiles("C://Users//insin//Downloads//img//00/"))
            {
                i++;
                if (sourceImage.EndsWith(".jpg") || sourceImage.EndsWith(".jpeg") || sourceImage.EndsWith(".JPG"))
                {
                    DateTime dt = GetImageCreatedDate(sourceImage);
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
                    string filePath = Path.Combine("C:", "Users", "insin", "Downloads", "img", "10", targetfileName);
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
        { }

        public static DateTime GetImageCreatedDate(string filePath)
        {
            var directories = MetadataExtractor.ImageMetadataReader.ReadMetadata(filePath);
            try
            {
                foreach (var directory in directories)
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
            catch (System.Exception)
            {

                return DateTime.Parse("Jan 11, 1950");
            }
            return DateTime.Parse("Jan 11, 1950");

            //throw new InvalidOperationException($"Date not found in {filePath}");
        }
    }
}
