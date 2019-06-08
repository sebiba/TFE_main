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
            try
            {
                value = PyUtils.FreqToNote(Frequence);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
#endregion constructeur

        public Dictionary<string, string> GetGesture()
        {
            try
            {
                    List<string> Lines = File.ReadAllLines(@"D:\programmation\c#\TFE\python\tablature.csv").ToList();
                    for(int i = 0;i < Lines.Count();i++)  // loop on all the ligne from the file
                    {
                        if (Lines[i].Split(';').First().ToLower() == value[0].ToString().ToLower() && Lines[i].Split(';').ElementAt(1) == "2")
                        {
                            if ((value.Contains("is") || value.Contains("#")) && Lines[i+1].Split(';').First() == "")
                            {
                                return new Dictionary<string, string>() { { "Do", Lines[i+1].Split(';').ElementAt(2) == "" ? "0" : Lines[i+1].Split(';').ElementAt(2) }, { "Sol", Lines[i+1].Split(';').Last() == "" ? "0" : Lines[i+1].Split(';').Last() } };
                            }
                            else if((value.Contains("es") || value.Contains("b")) && Lines[i-1].Split(';').First() == "")
                            {
                                return new Dictionary<string, string>() { { "Do", Lines[i-1].Split(';').ElementAt(2) == "" ? "0" : Lines[i-1].Split(';').ElementAt(2) }, { "Sol", Lines[i-1].Split(';').Last() == "" ? "0" : Lines[i-1].Split(';').Last() } };
                            }
                            return new Dictionary<string, string>() { { "Do", Lines[i].Split(';').ElementAt(2) == "" ? "0" : Lines[i].Split(';').ElementAt(2) }, { "Sol", Lines[i].Split(';').Last() == "" ? "0" : Lines[i].Split(';').Last() } };
                        }
                    }
                throw new KeyNotFoundException();
            }catch(IOException e)
            {
                throw e;
            }
        }

        public bool Equals(Note obj)
        {
            if (this.value == obj.value) return true;
            return false;
        }
    }
}
