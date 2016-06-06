using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows;

namespace Server_Knowledge_checking.Utilities
{
    public class TestParser
    {
        const string patternForClassifyQuestion = @"Rodzaj:""(.)"".*";
        const string patternForOpenQuestion = @".*Tresc:""(.*?)""; .*""(\d{2,3})"".";
        const string patternForCloseQuestion = @".*Tresc:""(.*?)""; Odpowiedz_A:""(.*?)""; Odpowiedz_B:""(.*?)""; Odpowiedz_C:""(.*?)""; Odpowiedz_D:""(.*?)""; Odpowiedz_E:""(.*?)""; Prawidlowa:""([ABCDE])""; Czas:""(\d{2,3})"".";
        private static TestParser single_oInstance = null;

        public static TestParser Instance
        {
            get
            {
                if (single_oInstance == null)
                    single_oInstance = new TestParser();
                return single_oInstance;
            }
        }

        public int ParseTest(string pathToFile, string folderPath)
        {
            List<string> testFileLines = ReadFile(pathToFile);
            Regex rExtractToClassify;
            Regex rExtractForOpen;
            Regex rExtractForClose;
            Match mExtract;
            Match mExtractForOpen;
            Match mExtractForClose;
            Group g0;
            Group g1;
            Group g2;
            Group g3;
            Group g4;
            Group g5;
            Group g6;
            Group g7;
            Group g8;

            foreach (string line in testFileLines)
            {
                rExtractToClassify = new Regex(patternForClassifyQuestion, RegexOptions.IgnoreCase);
                mExtract = rExtractToClassify.Match(line);
                g0 = mExtract.Groups[0];
                g1 = mExtract.Groups[1];

                if (g1.ToString() == "O")
                {
                    rExtractForOpen = new Regex(patternForOpenQuestion, RegexOptions.IgnoreCase);
                    mExtractForOpen = rExtractForOpen.Match(line);
                    g0 = mExtractForOpen.Groups[0];
                    g1 = mExtractForOpen.Groups[1];
                    g2 = mExtractForOpen.Groups[2];
                    if (Int32.Parse(g2.ToString()) > 300 || Int32.Parse(g2.ToString()) < 10)
                        return testFileLines.IndexOf(line);
                    if (!SplitAndCheck(g1.ToString(), folderPath))
                        return testFileLines.IndexOf(line);
                }
                else if (g1.ToString() == "Z")
                {
                    rExtractForClose = new Regex(patternForCloseQuestion, RegexOptions.IgnoreCase);
                    mExtractForClose = rExtractForClose.Match(line);
                    g0 = mExtractForClose.Groups[0];
                    g1 = mExtractForClose.Groups[1];
                    g2 = mExtractForClose.Groups[2];
                    g3 = mExtractForClose.Groups[3];
                    g4 = mExtractForClose.Groups[4];
                    g5 = mExtractForClose.Groups[5];
                    g6 = mExtractForClose.Groups[6];
                    g7 = mExtractForClose.Groups[7];
                    g8 = mExtractForClose.Groups[8];

                    if (g7.ToString() == "")
                        return testFileLines.IndexOf(line);
                    if (Int32.Parse(g8.ToString()) > 300 || Int32.Parse(g8.ToString()) < 10)
                        return testFileLines.IndexOf(line);
                    if (!SplitAndCheck(g1.ToString(), folderPath))
                        return testFileLines.IndexOf(line);
                    for (int index = 2; index <= 6; index++)
                    {
                        if (mExtractForClose.Groups[index].ToString().Length > 9 && mExtractForClose.Groups[index].ToString().Substring(0, 9) == "Pictures\\")
                            if (!checkIfFileExists(folderPath + mExtractForClose.Groups[index].ToString()))
                                return testFileLines.IndexOf(line);
                    }
                }
                else
                {
                    if(testFileLines.IndexOf(line) > 0)
                        return testFileLines.IndexOf(line);
                }
            }
            return 0;
        }

        public List<string> ReadFile(string pathToReadFile)
        {
            List<string> lines = new List<string>();

            using (StreamReader sr = new StreamReader(pathToReadFile))
            {
                while (!sr.EndOfStream)
                {
                    lines.Add(sr.ReadLine());
                }
            }
            return lines;
        }

        public Boolean SplitAndCheck(string content, string folderPath)
        {
            Boolean result = true;
            string questionContent;
            string imageQuestionContent;
            string[] partsOfContent = content.Split('|');
            if (partsOfContent.Length == 2)
            {
                imageQuestionContent = partsOfContent[0];
                if (checkIfFileExists(folderPath + imageQuestionContent) == false)
                    result = false;
                questionContent = partsOfContent[1];
                if (questionContent == "")
                    result = false;
            }
            else
            {
                imageQuestionContent = partsOfContent[0];
                if(imageQuestionContent.Length >= 9 && imageQuestionContent.Substring(0, 9) == "Pictures\\")
                {
                    if (checkIfFileExists(imageQuestionContent) == false)
                        result = false;
                }
            }
            return result;
        }

        public Boolean checkIfFileExists(string path)
        {
            Boolean result = false;
            if (File.Exists(path))
                result = true;
            return result;
        }
    }
}
