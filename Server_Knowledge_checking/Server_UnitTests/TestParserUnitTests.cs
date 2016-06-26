using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server_Knowledge_checking.Utilities;
using System.Collections.Generic;
using System.IO;

namespace ServerUnitTests
{
    [TestClass]
    public class TestReadFileMethod
    {
        [TestMethod]
        public void ReadFileWithPosiitiveResult()
        {
            // Given
            string pathToFileWhichIsRead = "../../Utilities/ex_doc.txt";
            // When
            List<string> listWithActualResult = TestParser.Instance.ReadFile(pathToFileWhichIsRead);
            List<string> listWithExpectedResult = new List<string>();
            listWithExpectedResult.Add("1");
            listWithExpectedResult.Add("2");
            listWithExpectedResult.Add("3");
            // Then
            CollectionAssert.AreEqual(listWithExpectedResult, listWithActualResult);
        }

[ExpectedException(typeof(System.IO.DirectoryNotFoundException))]
[TestMethod]
public void ReadFileWithRaisedDirectoryNotFoundException()
{
    // Given
    string pathToFileWhichIsRead = "C:/notexistedpath/ex_doc.txt";
    // When
    List<string> listWithActualResult = TestParser.Instance.ReadFile(pathToFileWhichIsRead);
    //Then
    // Exception powinien zostać przechwycony
}
}

    [TestClass]
    public class TestSpliAndCheckMethod
    {
        [TestMethod]
        public void SplitAndCheckPosiitiveResult()
        {
            // Given
            string pathToFileWhichIsRead = "..\\..\\Utilities\\Pictures\\ex_image.png";
            string contentOfSplittedElement = "Pictures\\ex_image.png | Czym jest opór wewnętrzny baterii(Odpowiedz na podstawie rysunku pomocniczego) ?";
            string pathToFolderWhereFileShouldExist = "..\\..\\Utilities\\";
            Directory.CreateDirectory("..\\..\\Utilities\\Pictures");
            File.Create(pathToFileWhichIsRead).Dispose();
            // When
            bool resultOfSplittingAndChecking = TestParser.Instance.SplitAndCheck(contentOfSplittedElement, pathToFolderWhereFileShouldExist);
            // Then
            Assert.AreEqual(resultOfSplittingAndChecking, true);
            // Metoda czyszcząca
            Directory.Delete("..\\..\\Utilities\\Pictures", true);
        }

        [TestMethod]
        public void SplitAndCheckNegative()
        {
            // Given
            string contentOfSplittedElement = "Pictures\\ex_image.png | Czym jest opór wewnętrzny baterii(Odpowiedz na podstawie rysunku pomocniczego) ?";
            string pathToFolderWhereFileShouldNotExist = "..\\..\\Not_Existed\\";
            // When
            bool resultOfSplittingAndChecking = TestParser.Instance.SplitAndCheck(contentOfSplittedElement, pathToFolderWhereFileShouldNotExist);
            // Then
            Assert.AreEqual(resultOfSplittingAndChecking, false);
        }


    }
}