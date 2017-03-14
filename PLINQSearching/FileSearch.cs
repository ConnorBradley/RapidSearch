using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Shapes;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace PLINQSearching
{
    public static class FileSearch
    {
        public static DataTable results = new DataTable();

        public static string workingDirectory = "";

        public static DTE getCurrentDTE(IServiceProvider provider)
        {
            var vs = (DTE) provider.GetService(typeof(DTE));
            return vs;
        }


        public static DTE getCurrentDTE()
        {
            return getCurrentDTE(ServiceProvider.GlobalProvider);
        }


        private static List<LineDetails> GetAllFilesInFolder(string directory,
            List<LineDetails> listToAppend = default(List<LineDetails>))
        {
            if (listToAppend == default(List<LineDetails>))
                listToAppend = new List<LineDetails>();

            foreach (var d in Directory.GetDirectories(directory))
            {
                foreach (var f in Directory.GetFiles(d))
                {
                    var file = new FileInfo(f);
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

 

        public static List<LineDetails> SearchFiles(string searchTerm)
        {
            var dir = GetSolutionDirectory(getCurrentDTE());
            //var fileContents = from file in GetAllFilesInFolder(dir)
            //    from line in File.ReadAllLines(file.FullName)
            //    where line.Contains(searchTerm)
            //    select new {File = file, Line = line};

            return GetAllFilesInFolder(dir).Where(line => line.LineContent.Contains(searchTerm)).ToList();
        }

        public static string GetSolutionDirectory(DTE dte)
        {

            var unformattedDirectory = dte.Solution.FullName;
            

             workingDirectory = !string.IsNullOrEmpty(unformattedDirectory) ? unformattedDirectory.Remove(unformattedDirectory.LastIndexOf("\\", StringComparison.Ordinal)) : "error";
            return workingDirectory;
        }
    }
}