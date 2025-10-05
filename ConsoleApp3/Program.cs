using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiskAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            //string rootPath = @"C:\Users\All Users";
            //string rootPath = @"C:\Users\umutg";
            string rootPath = @"C:\Users\umutg\AppData";

            Console.WriteLine($"Taranıyor: {rootPath}");
            Console.WriteLine("Lütfen bekleyin, bu işlem biraz sürebilir...\n");

            var folderSizes = new List<(string Path, long Size)>();

            try
            {
                foreach (var dir in Directory.EnumerateDirectories(rootPath))
                {
                    try
                    {
                        long size = GetDirectorySize(dir);
                        folderSizes.Add((dir, size));
                        Console.WriteLine($"{dir} - {FormatSize(size)}");
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine($"Erişim reddedildi: {dir}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Hata ({dir}): {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Genel hata: {ex.Message}");
            }

            Console.WriteLine("\n=== En büyük klasörler ===");
            foreach (var item in folderSizes.OrderByDescending(f => f.Size).Take(20))
            {
                Console.WriteLine($"{FormatSize(item.Size),15}  {item.Path}");
            }

            Console.WriteLine("\nTamamlandı. Enter’a basarak çıkabilirsiniz.");
            Console.ReadLine();
        }

        static long GetDirectorySize(string dirPath)
        {
            long size = 0;

            try
            {
                // Dosyaları topla
                var files = Directory.EnumerateFiles(dirPath, "*", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    try
                    {
                        size += new FileInfo(file).Length;
                    }
                    catch { /* Dosya silinmiş veya erişilemez olabilir */ }
                }

                // Alt klasörleri tara
                var subDirs = Directory.EnumerateDirectories(dirPath);
                foreach (var sub in subDirs)
                {
                    size += GetDirectorySize(sub);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Sistem klasörlerine erişim engellenebilir
            }

            return size;
        }

        static string FormatSize(long bytes)
        {
            double size = bytes;
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            int unitIndex = 0;
            while (size >= 1024 && unitIndex < units.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }
            return $"{size:0.##} {units[unitIndex]}";
        }
    }
}
