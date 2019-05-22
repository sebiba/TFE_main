using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace python
{
    public class Note
    {
        public String value { get; set; }

#region constructeur
        public Note(string lettreParam)
        {
            value = lettreParam;
        }
        public Note(float Frequence)
        {
            value = PyUtils.FreqToNote(Frequence);
        }
#endregion constructeur

        public Dictionary<string, string> GetGesture()
        {
            try
            {
                using (StreamReader sr = File.OpenText(@"D:\programmation\c#\TFE\python\tablature.csv"))  // load file from _path
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)  // loop on all the ligne from the file
                    {
                        if (s.Split(';').First().ToLower() == value[0].ToString().ToLower())
                        {
                            return new Dictionary<string, string>() { { "Do", s.Split(';').ElementAt(2) == "" ? "0" : s.Split(';').ElementAt(2) }, { "Sol", s.Split(';').Last() == "" ? "0" : s.Split(';').Last() } };
                        }
                    }
                }
                throw new KeyNotFoundException();
            }catch(IOException e)
            {
                throw e;
            }
        }

    }
}
