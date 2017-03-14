using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLINQSearching
{
    public class LineDetails
    {
        public FileInfo FileInfo { get; set; }
        public int LineNo { get; set; }
        public string LineContent { get; set; }


        public LineDetails(FileInfo fileInfo, int lineNo, string lineContent)
        {
            this.FileInfo = fileInfo;
            this.LineNo = lineNo;
            this.LineContent = lineContent;
        }
    }
}
