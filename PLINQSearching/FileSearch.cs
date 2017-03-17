using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Shapes;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace PLINQSearching
{
    public static class FileSearch
    {
        public static DataTable results = new DataTable();

        public static string workingDirectory = "";

        public static DTE GetCurrentDTE(IServiceProvider provider)
        {
            var vs = (DTE) provider.GetService(typeof(DTE));
            return vs;
        }


        public static DTE GetCurrentDTE()
        {
            return GetCurrentDTE(ServiceProvider.GlobalProvider);
        }


        private static List<LineDetails> GetAllFilesInFolder(string directory,
            List<LineDetails> listToAppend = default(List<LineDetails>))
        {
            try
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
            catch (Exception e)
            {
                throw;
            }
        }

 
        /// <summary>
        /// Basic search using the .Contains method
        /// </summary>
        /// <param name="searchTerm">string to search with</param>
        /// <returns></returns>
        public static List<LineDetails> SearchFiles(string searchTerm)
        {
            return GetAllFilesInFolder(GetSolutionDirectory(GetCurrentDTE())).Where(line => line.LineContent.Contains(searchTerm)).ToList();
        }

        /// <summary>
        /// Search using the .IndexOf method and StringComparison.OrdinalIgnoreCase
        /// This is meant to be really fast due to the use of unmanaged code
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static List<LineDetails> IndexOfSearch(string searchTerm)
        {
            return GetAllFilesInFolder(GetSolutionDirectory(GetCurrentDTE()))
                .Where(line => line.LineContent.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) > 0).ToList();
        }

        public static string GetSolutionDirectory(DTE dte)
        {

            var unformattedDirectory = dte.Solution.FullName;
            

             workingDirectory = !string.IsNullOrEmpty(unformattedDirectory) ? unformattedDirectory.Remove(unformattedDirectory.LastIndexOf("\\", StringComparison.Ordinal)) : "error";
            return workingDirectory;
        }
    }
}