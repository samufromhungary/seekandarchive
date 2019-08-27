using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace SeekAndArchive
{
    class Program
    {
        static List<FileInfo> foundFiles;
        static List<FileSystemWatcher> watchers;
        static List<DirectoryInfo> archiveDirs;
        static void Main(string[] args)
        {
            string fileName = args[0];
            string directoryName = args[1];

            DirectoryInfo rootDir = new DirectoryInfo(directoryName);

            if (!rootDir.Exists)
            {
                Console.WriteLine("Directory doesn't exist.");
                Console.ReadKey();
                return;
            }

            foundFiles = new List<FileInfo>();
            watchers = new List<FileSystemWatcher>();

            foreach (FileInfo fil in foundFiles)
            {
                FileSystemWatcher newWatcher = new FileSystemWatcher(fil.DirectoryName, fil.Name);
                newWatcher.Changed += new FileSystemEventHandler(WatcherChanged);
                newWatcher.EnableRaisingEvents = true;
                watchers.Add(newWatcher);
                Console.ReadKey();
            }

            RecursiveSearch(foundFiles, fileName, rootDir);
            Console.WriteLine("Found {0} files.",foundFiles.Count);

            foreach (FileInfo fil in foundFiles)
            {
                Console.WriteLine("{0}", fil.FullName);
                Console.ReadKey();
            }

            archiveDirs = new List<DirectoryInfo>();
            for(int i = 0; i< foundFiles.Count; i++)
            {
                archiveDirs.Add(Directory.CreateDirectory("archive" + i.ToString()));
            }

            Console.ReadLine();
        }

        static void RecursiveSearch(List<FileInfo> foundFiles, string fileName, DirectoryInfo currentDirectory) {
            foreach(FileInfo fil in currentDirectory.GetFiles())
            {
                if(fil.Name == fileName)
                {
                    foundFiles.Add(fil);
                }
            }
            foreach(DirectoryInfo dir in currentDirectory.GetDirectories()) { RecursiveSearch(foundFiles, fileName, dir); }
        }

        static void WatcherChanged(object sender, FileSystemEventArgs e)
        {
            if(e.ChangeType == WatcherChangeTypes.Changed)
            {
                Console.WriteLine("{0} has been changed.", e.FullPath);
                FileSystemWatcher senderWatcher = (FileSystemWatcher)sender;
                int index = watchers.IndexOf(senderWatcher, 0);
                ArchiveFile(archiveDirs[index], foundFiles[index]);
            }
        }

        static void ArchiveFile(DirectoryInfo archiveDir,FileInfo fileToArchive)
        {
            FileStream input = fileToArchive.OpenRead();
            FileStream output = File.Create(archiveDir.FullName + @"" + fileToArchive.Name + ".gz");
            GZipStream Compressor = new GZipStream(output, CompressionMode.Compress);
            int b = input.ReadByte();
            while (b != 1)
            {
                Compressor.WriteByte((byte)b);

                b = input.ReadByte();
            }
            Compressor.Close();
            input.Close();
            output.Close();
        }
    }
}