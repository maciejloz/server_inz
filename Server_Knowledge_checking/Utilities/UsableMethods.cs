using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Windows;

namespace Server_Knowledge_checking.Utilities
{
    static class UsableMethods
    {
        public static string zipPath;
        public static bool IS_OK = false;
        private static string _courseName;
        private static string _groupName;
        private static string _directoryPath;

        public static void OpenTest(string courseName_copy, string groupName_copy)
        {
            _courseName = courseName_copy;
            _groupName = groupName_copy;


            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "TXT Files (*.txt) |*.txt";
            Nullable<bool> resultOfDialog = openFileDialog.ShowDialog();

            if (resultOfDialog == true)
            {
                string pathOfTest = openFileDialog.FileName;
                SelectDirectoryPath(pathOfTest);
            }
        }

        public static void CancelTest()
        {
            _courseName = "";
            _groupName = "";
            Directory.Delete(_directoryPath, true);
            _directoryPath = "";
            zipPath = "";
        }

        private static void SelectDirectoryPath(string pathOfTest)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.courseName.Visibility = System.Windows.Visibility.Hidden;

            string patternToParse = @"(.*\\)(.*)\.";
            Regex rExtract = new Regex(patternToParse, RegexOptions.IgnoreCase);
            Match mExtract = rExtract.Match(pathOfTest);
            Group g0 = mExtract.Groups[0];
            Group g1 = mExtract.Groups[1];
            Group g2 = mExtract.Groups[2];

            string folderPath = g1.ToString();
            string fileName = g2.ToString();
            ZipDirectory(folderPath, fileName);
            //C:\\Users\\maciek\\Documents\\Testy\\Elektronika\\Grupa_1\\test_2\\test.txt
        }

        private static void ZipDirectory(string folderPath, string fileName)
        {
            zipPath = "C:\\Testy_Zipped\\" + _courseName + "\\" + _groupName + "\\" + fileName + ".zip";
            _directoryPath = "C:\\Testy_Zipped\\" + _courseName + "\\" + _groupName ;
            //if (Directory.Exists(directoryPath) == false)
                Directory.CreateDirectory(_directoryPath);
           // else
            //    MessageBox.Show("Ścieżka skompresowanego folderu istnieje", "Błąd ścieżki", MessageBoxButton.OK, MessageBoxImage.Error);

            try
            {
                ZipFile.CreateFromDirectory(folderPath, zipPath, CompressionLevel.Optimal, false);
                IS_OK = true;
            }
            catch(ArgumentException ex)
            {
                IS_OK = false;
                System.Windows.MessageBox.Show("Podaj właściwy test", "Błąd ścieżki", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (PathTooLongException ex)
            {
                IS_OK = false;
                System.Windows.MessageBox.Show("Podana ścieżka testu jest za długa" , "Błąd ścieżki", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException ex)
            {
                IS_OK = false;
                System.Windows.MessageBox.Show("Plik .zip z podanej ścieżki już istnieje", "Błąd ścieżki", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (System.UnauthorizedAccessException ex)
            {
                IS_OK = false;
                System.Windows.MessageBox.Show("Nie posiadasz praw do katalogu", "Błąd ścieżki", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //catch (DirectoryNotFoundException ex)
            //{
            //    System.Windows.MessageBox.Show("Nie znaleziono katalogu", "Błąd ścieżki", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
            //catch (EndOfStreamException ex)
            //{
            //    System.Windows.MessageBox.Show("nie znaleziono katalogu", "Błąd ścieżki", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
            //catch (FileLoadException ex)
            //{
            //    System.Windows.MessageBox.Show("nie znaleziono katalogu", "Błąd ścieżki", MessageBoxButton.OK, MessageBoxImage.Error);
            //}

            //ZipFile.ExtractToDirectory(zipPath, "C:\\Desktop");
        }

    }
}
