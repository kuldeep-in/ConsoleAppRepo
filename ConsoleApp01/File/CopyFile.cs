
namespace ConsoleApp01.File
{
    using System.IO;

    public static class CopyFile
    {
        public static void CopyFiles()
        {
            string dirPAth = "D:\\abc";
            DirectoryInfo mainDir = new DirectoryInfo(dirPAth);
            int cnt = 001;
            foreach (DirectoryInfo dayDir in mainDir.GetDirectories())
            {
                foreach (FileInfo file in dayDir.GetFiles())
                {
                    file.CopyTo(dayDir.FullName + cnt.ToString().PadLeft(4, '0') + ".jpg");
                }
            }
        }
    }
}
