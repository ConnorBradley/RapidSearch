using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidSearching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidSearching.Tests
{
    [TestClass()]
    public class FileSearchTests
    {
        private const string TEST_DIRECTORY =
            @"C:\Users\conno\OneDrive\Documents\Visual Studio 2015\Projects\RapidSearchRelease\RapidSearch\TestFolder\";
       
        [TestMethod()]
        public void GetAllFilesInFolderTest()
        {
            var actual = FileSearch.GetAllFilesInFolder(TEST_DIRECTORY);
            var expected = new List<LineDetails>
            {
                new LineDetails(new FileInfo("C:\\Users\\conno\\OneDrive\\Documents\\Visual Studio 2015\\Projects\\RapidSearchRelease\\RapidSearch\\TestFolder\\TestFolder\\Test1.txt"), 1, "a"),
                new LineDetails(new FileInfo("C:\\Users\\conno\\OneDrive\\Documents\\Visual Studio 2015\\Projects\\RapidSearchRelease\\RapidSearch\\TestFolder\\TestFolder\\Test2.txt"), 1, "a"),
                new LineDetails(new FileInfo("C:\\Users\\conno\\OneDrive\\Documents\\Visual Studio 2015\\Projects\\RapidSearchRelease\\RapidSearch\\TestFolder\\TestFolder\\Test3.txt"), 1, "b")
            };
            //Note - you can't compare FileInfo variables as they will never equal.
            Assert.AreEqual(actual[0].LineContent, expected[0].LineContent);
            Assert.AreEqual(actual[0].LineNo, expected[0].LineNo);
            Assert.AreEqual(actual[1].LineContent, expected[1].LineContent);
            Assert.AreEqual(actual[1].LineNo, expected[1].LineNo);
            Assert.AreEqual(actual[2].LineContent, expected[2].LineContent);
            Assert.AreEqual(actual[2].LineNo, expected[2].LineNo);
        }

        [TestMethod()]
        public void SearchFilesTest()
        {

            var solutionContents = FileSearch.GetAllFilesInFolder(TEST_DIRECTORY);
            var actual = FileSearch.BoyerMooreSearch("a", solutionContents, TEST_DIRECTORY);
            var expected = new List<LineDetails>
            {
                new LineDetails(new FileInfo("C:\\Users\\conno\\OneDrive\\Documents\\Visual Studio 2015\\Projects\\RapidSearchRelease\\RapidSearch\\TestFolder\\TestFolder\\Test1.txt"), 1, "a"),
                new LineDetails(new FileInfo("C:\\Users\\conno\\OneDrive\\Documents\\Visual Studio 2015\\Projects\\RapidSearchRelease\\RapidSearch\\TestFolder\\TestFolder\\Test2.txt"), 1, "a")
            };
            //Note - you can't compare FileInfo variables as they will never equal.
            Assert.AreEqual(actual[0].LineContent, expected[0].LineContent);
            Assert.AreEqual(actual[0].LineNo, expected[0].LineNo);
            Assert.AreEqual(actual[1].LineContent, expected[1].LineContent);
            Assert.AreEqual(actual[1].LineNo, expected[1].LineNo);
        }
    }
}