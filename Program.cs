﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;


namespace FolderSize
{
    class Program
    {
        static IDictionary<String, long> sizeMap = new Dictionary<String, long>();
        static IDictionary<String, long> timeMap = new Dictionary<String, long>();
        static bool firstrun = true;
        static void Main(string[] args)
        {
            var quit = false;
            while (!quit)
            {

                Console.Write("Enter a folder path: ");
                string path = Console.ReadLine();
                
                //timer
                var watch = System.Diagnostics.Stopwatch.StartNew();

                DirectoryInfo d = new DirectoryInfo(path);
                // Determine whether the directory exists.
                if (!d.Exists)
                {
                    // Indicate that the directory already exists.
                    Console.WriteLine("<ERROR> That path does not exists");
                    return;
                }
                GetDirSize(d);
                foreach (KeyValuePair<String, long> kvp in sizeMap)
                {
                    //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                    var DirSize = DirSizeToString(kvp.Value);
                    //if (DirSize.Length < 8)
                    //{
                    //    Console.WriteLine("{0}\t\t{1}", DirSize, kvp.Key);
                    //}
                    //else
                    //{
                    //    Console.WriteLine("{0}\t{1}", DirSize, kvp.Key);
                    //}
                }
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                firstrun = false;
                Console.WriteLine("\n\nProgram took {0} seconds", elapsedMs / 1000);

                Console.Write("Quit program? (y/n)\n");
                string answer = Console.ReadLine();
                if (answer.Contains("y")){
                    quit = true;
                    Console.WriteLine("Quiting...");
                }
            }
        }

        static long GetDirSize(DirectoryInfo dir)
        {
            if (!firstrun){
                try
                {   
                    //Console.WriteLine("test1");
                    //if (sizeMap[dir] == (long)-1)
                    //{
                    //    return 0;
                    //}
                    Console.WriteLine("test2");
                    Console.WriteLine(dir.LastWriteTime.Ticks.ToString());
                    Console.WriteLine(timeMap[dir.FullName].ToString());
                    if (dir.LastWriteTime.Ticks == timeMap[dir.FullName])
                    {
                        Console.WriteLine("cache hit");
                        Console.WriteLine(dir.FullName);
                        return sizeMap[dir.FullName];
                    }
                }
                catch (Exception e) { Console.Error.WriteLine(e); }
            }
            


            long totalSize = 0;
            try
            {
                totalSize = dir.EnumerateFiles().Sum(File => File.Length);
            }
            catch
            {
                //sizeMap.Add(dir, (long)-1);
                Console.WriteLine(dir.FullName);
                return 0;
            }
            
            foreach(var sub_dir in dir.EnumerateDirectories())
            {
                totalSize += GetDirSize(sub_dir);
            }

            sizeMap.Add(dir.FullName, totalSize);
            timeMap.Add(dir.FullName, dir.LastWriteTime.Ticks);
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