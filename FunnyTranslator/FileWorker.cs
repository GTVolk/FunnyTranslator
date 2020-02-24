using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FunnyTranslator
{
    class FileWorker
    {
        private string filename;

        public FileWorker(string filename)
        {
            this.filename = filename;
        }

        public string[] readAllLines()
        {
            return File.ReadAllLines(this.filename, Encoding.GetEncoding(850));
        }

        public List<string> readAllLinesToList()
        {
            return this.readAllLines().ToList();
        }

        public string readAllToString()
        {
            string outstr = "";
            using (StreamReader Sr = new StreamReader(this.filename, Encoding.Default))
            {
                while (!Sr.EndOfStream)
                    outstr += (char)Sr.Read();
            }
            return outstr;
        }

        public static void writeToFile(string strToWrite, string filename)
        {
            using (StreamWriter Sw = new StreamWriter(filename))
            {
                    Sw.WriteLine(strToWrite);
            }
        }
    }
}
