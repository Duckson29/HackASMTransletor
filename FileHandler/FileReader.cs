using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HackASMTransletor
{
    /// <summary>
    /// This reads the text file with the asm
    /// </summary>
    class FileReader
    {
        /// <summary>
        /// This reads all lines from the input file and removeds all "//" and whiteSpaces from the asm.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[] ReadAsmFile(string path)
        {
            string[] context = File.ReadAllLines(path);
            List<string> result = new List<string>();
            foreach (var lines in context)
            {
                lines.Trim();
                if (!lines.StartsWith("/"))
                {
                    result.Add(lines);
                }             
            }
            return result.ToArray();
        }        

    }
}
