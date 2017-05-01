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

namespace RapidSearching
{
    public static class FileSearch
    {
        
        private static string _workingDirectory = "";
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
             if (directory == "error") return listToAppend;


            foreach (var d in Directory.GetDirectories(directory))
                {
                    //skip this iteration if the directory is blacklisted
                    if (Blacklist.Folders.Any(d.Contains)) continue;
                    foreach (var f in Directory.GetFiles(d))
                    {
                        var file = new FileInfo(f);
                        //skip this iteration if the extension is blacklisted (most likely due to it not being a text file)
                        if (Blacklist.Extensions.Any(file.Extension.Contains) || Blacklist.Files.Any(file.Name.Contains))
                        {
                            continue;
                        }

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
                    
                    Match match = Regex.Match(line.LineContent, searchTerm, RegexOptions.IgnoreCase);

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
;
        }


        public static List<LineDetails> BoyerMooreSearch(string searchTerm,  List<LineDetails> solutionContents, string currentDirectory = null)
        {
            if (currentDirectory == null)
            {
                currentDirectory = GetSolutionDirectory(GetCurrentDTE());
            }

            var retVal = new List<LineDetails>();

           Initialize(searchTerm);
 

            Parallel.ForEach(solutionContents, line =>
            {
                if (line.LineContent == String.Empty) return;
                var i = 0;

                while (i <= (line.LineContent.Length - searchTerm.Length))
                {

                    int index = searchTerm.Length - 1;

                    while (index >= 0 && char.ToUpper(searchTerm[index]) == char.ToUpper(line.LineContent[i + index]))
                    {
                        index--;
                    }
                    if (index < 0)
                    {
                        retVal.Add(line);
                    }

                    i += Math.Max(_skipTable[line.LineContent[i + index]] - searchTerm.Length + 1 + index, 1);
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
                    _skipTable[char.ToLower(pattern[i])] = (byte)(pattern.Length - i - 1);
                    _skipTable[char.ToUpper(pattern[i])] = (byte)(pattern.Length - i - 1);
                }
        }



 

        public static string GetSolutionDirectory(DTE dte)
        {

            var unformattedDirectory = dte.Solution.FullName;
            

             _workingDirectory = !string.IsNullOrEmpty(unformattedDirectory) ? unformattedDirectory.Remove(unformattedDirectory.LastIndexOf("\\", StringComparison.Ordinal)) : "error";
            return _workingDirectory;
        }
    }
}