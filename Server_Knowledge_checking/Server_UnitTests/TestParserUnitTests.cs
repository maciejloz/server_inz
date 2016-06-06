using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server_Knowledge_checking.Utilities;
using System.Collections.Generic;
using System.IO;

namespace ServerUnitTests
{
    [TestClass]
    public class TestReadFile
    {
        [TestMethod]
        public void ReadFilePosiitive()
        {
            // Arrange
            string pathToFile = "../../Utilities/ex_doc.txt";
            // Act
            List<string> actual = TestParser.Instance.ReadFile(pathToFile);
            List<string> expected = new List<string>();
            expected.Add("1");
            expected.Add("2");
            expected.Add("3");
            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [ExpectedException(typeof(System.IO.DirectoryNotFoundException))]
        [TestMethod]
        public void ReadFileRaisedDirectoryNotFoundException()
        {
            // Arrange
            string pathToFile = "C:/notexistedpath/ex_doc.txt";
            // Act
            List<string> actual = TestParser.Instance.ReadFile(pathToFile);
        }
    }

    [TestClass]
    public class TestParserUnitTests
    {
        [TestMethod]
        public void SplitAndCheckPosiitive()
        {
            // Arrange
            string filePath = "..\\..\\Utilities\\Pictures\\ex_image.png";
            string content = "Pictures\\ex_image.png | Czym jest opór wewnętrzny baterii(Odpowiedz na podstawie rysunku pomocniczego) ?";
            string folderPath = "..\\..\\Utilities\\";
            Directory.CreateDirectory("..\\..\\Utilities\\Pictures");
            File.Create(filePath).Dispose();
            // Act
            bool result = TestParser.Instance.SplitAndCheck(content, folderPath);
            // Assert
            Assert.AreEqual(result, true);
            // CleanUp
            Directory.Delete("..\\..\\Utilities\\Pictures", true);
        }

        [TestMethod]
        public void SplitAndCheckNegative()
        {
            // Arrange
            string content = "Pictures\\ex_image.png | Czym jest opór wewnętrzny baterii(Odpowiedz na podstawie rysunku pomocniczego) ?";
            string folderPath = "..\\..\\Not_Existed\\";
            // Act
            bool result = TestParser.Instance.SplitAndCheck(content, folderPath);
            // Assert
            Assert.AreEqual(result, false);
        }


    }
}