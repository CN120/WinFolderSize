using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;


namespace FolderSize
{
    class Program
    {
        static readonly IDictionary<String, long> sizeMap = new Dictionary<String, long>();
        static readonly IDictionary<String, long> timeMap = new Dictionary<String, long>();
        static int fileCount = new();
        static readonly EnumerationOptions EnumOps = new();
        static void Main()
        {
            do
            {
                Console.WriteLine();
                Console.Write("Enter a folder path: ");
                DirectoryInfo directory = new(Console.ReadLine());

                //timer
                var timer = System.Diagnostics.Stopwatch.StartNew();

                if (directory.Exists)
                {
                    // populate sizeMap and timeMap
                    // GetDirSize(directory);
                    fileCount = 0;
                    Console.WriteLine("{0} \nSize:\t\t{1}", directory, DirSizeToString(GetDirSize(directory)));
                    Console.WriteLine("Contains:\t{0} files", fileCount);
                }
                else {
                    Console.WriteLine("<ERROR> That path does not exists");
                    return;
                }
                
                // Print out contents of the dictionary
                //foreach (KeyValuePair<String, long> dir_and_size in sizeMap)
                //{
                //    string dirSizeStr = DirSizeToString(dir_and_size.Value);
                //    if (dirSizeStr.Length < 8)
                //        Console.WriteLine("{0}\t\t{1}", dirSizeStr, dir_and_size.Key);
                //    else
                //        Console.WriteLine("{0}\t{1}", dirSizeStr, dir_and_size.Key);
                //}

                timer.Stop();
                Console.WriteLine("Time Elapsed:\t{0} seconds\n", timer.ElapsedMilliseconds * 0.001);

                Console.Write("Run again? (y/n):");
            } while (Console.ReadLine().Contains("y"));
        }

        static long GetDirSize(in DirectoryInfo dir)
        {
            long totalSize = 0;
            
            // this serves as the base case because when EnumDirs() returns 0 directories, GetDirSize() is not run and Sum() returns 0
            totalSize += dir.EnumerateDirectories("*", EnumOps).Sum(Folder => GetDirSize(Folder));
            

            if (timeMap.TryGetValue(dir.FullName, out long storedLastWriteTime))
            {
                if (dir.LastWriteTime.Ticks == storedLastWriteTime)
                {
                    return sizeMap[dir.FullName];
                }
                else
                {
                    // this is inside a try simply for the one case in which dir is a top level directory and therefore does not have a parent
                    try{ timeMap[dir.Parent.FullName] = 0; }
                    catch (NullReferenceException nre) { /* Object reference not set to an instance of an object. */ }
                }
            }

            totalSize += dir.EnumerateFiles("*", EnumOps).Sum(File => File.Length);
            fileCount += dir.EnumerateFiles("*", EnumOps).Count();

            sizeMap[dir.FullName] = totalSize;
            timeMap[dir.FullName] = dir.LastWriteTime.Ticks;

            return totalSize;
        }

        static long GetDirSizeNoCache(in DirectoryInfo dir)
        {
            long totalSize = 0;

            // this serves as the base case because when EnumDirs() returns 0 directories, GetDirSize() is not run and Sum() returns 0
            totalSize += dir.EnumerateDirectories("*", EnumOps).Sum(Folder => GetDirSizeNoCache(Folder));

            totalSize += dir.EnumerateFiles("*", EnumOps).Sum(File => File.Length);

            return totalSize;
        }

        static string DirSizeToString(long sizeInBytes)
        {
            double adjustedSize = (double)sizeInBytes;
            string sizeString;
            if (adjustedSize > 1024)
            {
                adjustedSize /= 1024;
                if (adjustedSize > 1024)
                {
                    adjustedSize /= 1024;
                    if (adjustedSize > 1024)
                    {
                        adjustedSize /= 1024;
                        if (adjustedSize > 1024)
                        {
                            adjustedSize /= 1024;
                            sizeString = string.Format("{0} TB", adjustedSize.ToString("F3").Substring(0, 4).TrimEnd('.'));
                        }
                        else { sizeString = string.Format("{0} GB", adjustedSize.ToString("F3").Substring(0, 4).TrimEnd('.')); }
                    }
                    else { sizeString = string.Format("{0} MB", adjustedSize.ToString("F3").Substring(0, 4).TrimEnd('.')); }
                }
                else { sizeString = string.Format("{0} KB", adjustedSize.ToString("F3").Substring(0, 4).TrimEnd('.')); }
            }
            else { sizeString = string.Format("{0} bytes", adjustedSize.ToString("0")); }
            
            return sizeString;
        }
    }

}
