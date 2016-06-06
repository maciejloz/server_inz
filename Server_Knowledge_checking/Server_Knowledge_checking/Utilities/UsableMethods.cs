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
    public static class UsableMethods
    {
        public static string zipPath;
        private static string unzippedTestPath;
        public static bool IS_OK = false;
        public static string directoryPath;
        public static string reportPath;
        private static string _courseName;
        private static string _groupName;
        private static string _testName;
        public static string folderPath;
        public static string fileName;

        public static int OpenTest(string courseName_copy, string groupName_copy, string testName_copy)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "TXT Files (*.tst) |*.tst";
            Nullable<bool> resultOfDialog = openFileDialog.ShowDialog();
            int resultOfParsing;
            _courseName = courseName_copy;
            _groupName = groupName_copy;
            _testName = testName_copy;

            if (resultOfDialog == true)
            {
                string pathOfTest = openFileDialog.FileName;
                SelectDirectoryPath(pathOfTest);
                try
                {
                    resultOfParsing = TestParser.Instance.ParseTest(pathOfTest, folderPath);
                }
                catch(Exception ex)
                {
                    resultOfParsing = -1;
                }

                if(resultOfParsing == 0)
                {
                    ZipDirectory(folderPath, fileName);
                    if(IS_OK == true)
                        unzipTest();
                }
                return resultOfParsing;
            }
            return -1;
        }

        public static void CancelTest()
        {
            _courseName = "";
            _groupName = "";
            _testName = "";
            if(Directory.Exists(directoryPath))
                Directory.Delete(directoryPath, true);
            directoryPath = "";
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
            folderPath = g1.ToString();
            fileName = g2.ToString();
        }

        private static void ZipDirectory(string folderPath, string fileName)
        {
            zipPath = "C:\\Testy\\" + _courseName + "\\" + _groupName + "\\" + _testName + "\\" + fileName + ".zip";
            directoryPath = "C:\\Testy\\" + _courseName + "\\" + _groupName + "\\" + "\\" + _testName;
            try
            {
                Directory.CreateDirectory(directoryPath);
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
            catch (NotSupportedException)
            {
                IS_OK = false;
                System.Windows.MessageBox.Show("Prawdopodobnie podano błędna nazwę grupy", "Błąd nazwy", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void unzipTest()
        {
            ZipFile.ExtractToDirectory(zipPath, directoryPath);
        }

    }
}
