using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Shapes;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using System.Text.RegularExpressions;
using System.Windows.Markup;

namespace PLINQSearching
{
    public static class FileSearch
    {
        public static DataTable results = new DataTable();
        private static Dictionary<char, int> _table = new Dictionary<char, int>();
        public static string workingDirectory = "";
        private static SkipTable _skipTable;

        private static DTE GetCurrentDTE(IServiceProvider provider)
        {
            var vs = (DTE) provider.GetService(typeof(DTE));
            return vs;
        }

        /// <summary>
        /// Get the visual studio element
        /// </summary>
        /// <returns></returns>
        public static DTE GetCurrentDTE()
        {
            return GetCurrentDTE(ServiceProvider.GlobalProvider);
        }

        /// <summary>
        /// Public for unit testing
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="listToAppend"></param>
        /// <returns></returns>
        public static List<LineDetails> GetAllFilesInFolder(string directory,
            List<LineDetails> listToAppend = default(List<LineDetails>))
        {
   

                if (listToAppend == default(List<LineDetails>))
                    listToAppend = new List<LineDetails>();

                foreach (var d in Directory.GetDirectories(directory))
                {
                    //skip this iteration if the directory is blacklisted
                    if (Blacklist.Folders.Any(d.Contains)) continue;
                    foreach (var f in Directory.GetFiles(d))
                    {
                        var file = new FileInfo(f);
                        //skip this iteration if the extension is blacklisted (most likely due to it not being a text file)
                        if (Blacklist.Extensions.Any(file.Extension.Contains)) continue;

                        var lines = File.ReadAllLines(file.FullName);
                        var lineNumber = 0;
                        foreach (var line in lines)
                        {
                            lineNumber++;
                            var lineDetails = new LineDetails(file, lineNumber, line);
                            listToAppend.Add(lineDetails);
                        }
                    }

                    GetAllFilesInFolder(d, listToAppend);
                }

                return listToAppend;
            
        }


        public static List<LineDetails> RegExStringSearch(string searchTerm, List<LineDetails> solutionContent)
        {

            var results = new List<LineDetails>();
            Parallel.ForEach(solutionContent, line =>
            {
                if (line.LineContent != String.Empty)
                {
                    Match match = Regex.Match(line.LineContent, searchTerm);

                    if (match.Success)
                    {
                        results.Add(line);
                    }
                }
            });

            return results;
        }


        /// <summary>
        /// Search using the .IndexOf method and StringComparison.OrdinalIgnoreCase
        /// This is meant to be really fast due to the use of unmanaged code
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static List<LineDetails> IndexOfSearch(string searchTerm, List<LineDetails> solutionContents, string currentDirectory = null)
        {
            if (currentDirectory == null)
            {
                currentDirectory = GetSolutionDirectory(GetCurrentDTE());
            }

            var res = new List<LineDetails>();

            Parallel.ForEach(solutionContents, line =>
            {
                if (line.LineContent.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    res.Add(line);
                }
            });

            return res;

            //var results =  GetAllFilesInFolder(GetSolutionDirectory(GetCurrentDTE()))
            //    .Where(line => line.LineContent.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) > 0).ToList();
            //ResultsStorage.SearchResultsChanged = true;
            //return results;
        }

        public static List<LineDetails> BoyerMooreSearch2(string searchTerm, List<LineDetails> solutionContents, string currentDirectory = null)
        {
            if (currentDirectory == null)
            {
                currentDirectory = GetSolutionDirectory(GetCurrentDTE());
            }

            var retVal = new List<LineDetails>();

           Initialize(searchTerm);


            Parallel.ForEach(solutionContents, line =>
            {
                if (SearchBoyer(line, searchTerm))
                {
                    retVal.Add(line);
                }
            });

            return retVal;
        }


        public static void Initialize(string pattern)
        {
              // Create multi-stage skip table
            _skipTable = new SkipTable(pattern.Length);
            // Initialize skip table for this pattern

                for (int i = 0; i < pattern.Length - 1; i++)
                {
                    _skipTable[Char.ToLower(pattern[i])] = (byte)(pattern.Length - i - 1);
                    _skipTable[Char.ToUpper(pattern[i])] = (byte)(pattern.Length - i - 1);
                }
        }


        /// <summary>
        /// Searches for the current pattern within the given text
        /// starting at the specified index.
        /// </summary>
        /// <param name="text">Text to search</param>
        /// <returns></returns>
        public static bool SearchBoyer(LineDetails text, string searchTerm)
        {
            var i = 0;
            // Loop while there's still room for search term
            while (i <= (text.LineContent.Length - searchTerm.Length))
            {
                // Look if we have a match at this position
                int j = searchTerm.Length - 1;

                    while (j >= 0 && Char.ToUpper(searchTerm[j]) == Char.ToUpper(text.LineContent[i + j]))
                        j--;
                
                if (j < 0)
                {
                    // Match found
                    return true;
                }

                // Advance to next comparision
                i += Math.Max(_skipTable[text.LineContent[i + j]] - searchTerm.Length + 1 + j, 1);
            }
            // No match found
            return false;
        }

 

        public static string GetSolutionDirectory(DTE dte)
        {

            var unformattedDirectory = dte.Solution.FullName;
            

             workingDirectory = !string.IsNullOrEmpty(unformattedDirectory) ? unformattedDirectory.Remove(unformattedDirectory.LastIndexOf("\\", StringComparison.Ordinal)) : "error";
            return workingDirectory;
        }
    }
}