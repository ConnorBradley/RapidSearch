using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework.XamlTypes;

namespace PLINQSearching
{
    public static class Blacklist
    {
        /// <summary>
        /// List of folders to ignore
        /// TODO: Add an insert/delete function in settings
        /// </summary>
        public static string[] Folders { get; } = {"packages", "bin", "obj"};

        public static string[] Extensions { get; } = {".exe", ".dll", ".jpg", ".png", ".gif", ".ico", ".snk"};
    }
}
