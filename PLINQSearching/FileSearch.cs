﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Shapes;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;

namespace PLINQSearching
{
    public static class FileSearch
    {
        public static DataTable results = new DataTable();

        public static string workingDirectory = "";

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


        /// <summary>
        /// Basic search using the .Contains method
        /// </summary>
        /// <param name="searchTerm">string to search with</param>
        /// <param name="currentDirectory"></param>
        /// <returns></returns>
        public static List<LineDetails> SearchFiles(string searchTerm, string currentDirectory = null)
        {
            if (currentDirectory == null)
            {
                currentDirectory = GetSolutionDirectory(GetCurrentDTE());
            }
            
            var results =  GetAllFilesInFolder(currentDirectory).Where(line => line.LineContent.Contains(searchTerm)).ToList();
            ResultsStorage.SearchResultsChanged = true;
            return results;
        }

        /// <summary>
        /// Search using the .IndexOf method and StringComparison.OrdinalIgnoreCase
        /// This is meant to be really fast due to the use of unmanaged code
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static List<LineDetails> IndexOfSearch(string searchTerm)
        {
            
            var results =  GetAllFilesInFolder(GetSolutionDirectory(GetCurrentDTE()))
                .Where(line => line.LineContent.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) > 0).ToList();
            ResultsStorage.SearchResultsChanged = true;
            return results;
        }

        public static string GetSolutionDirectory(DTE dte)
        {

            var unformattedDirectory = dte.Solution.FullName;
            

             workingDirectory = !string.IsNullOrEmpty(unformattedDirectory) ? unformattedDirectory.Remove(unformattedDirectory.LastIndexOf("\\", StringComparison.Ordinal)) : "error";
            return workingDirectory;
        }
    }
}