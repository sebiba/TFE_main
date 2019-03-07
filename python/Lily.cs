using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace python
{
    public class Lily
    {
        private string _File;
        private List<string> data = new List<string>();

        public Lily(string path)
        {
            _File = path;
            ReadFile();
        }

        public void ReadFile(string path = null)
        {
            if (path == null) path = _File;
            if (File.Exists(_File))
            {
                // Create the file.
                using (StreamReader sr = File.OpenText(_File))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        data.Add(s);
                    }
                }
            }
            else
            {
                // Create the file.
                using (StreamReader sr = File.OpenText(@"D:\programmation\c#\TFE\python\Lily\template.ly"))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        data.Add(s);
                    }
                }
            }
        }

        public List<string> Tempo(List<string> input)
        {
            List<string> notes = new List<string>();
            int cpt = 1;
            for (int i = 1; i < input.Count; i++)
            {
                if (input[i] == input[i - 1]) cpt++;
                else
                {
                    if (cpt != 1)
                    {
                        if(cpt.CompareTo(16) > 0)
                        {
                            notes.Add(string.Concat(input[i - 1], 1));
                        }else if (cpt.CompareTo(8) > 0)
                        {
                            notes.Add(string.Concat(input[i - 1], 2));
                        }
                        else if (cpt.CompareTo(4) > 0)
                        {
                            notes.Add(string.Concat(input[i - 1], 4));
                        }
                        else if (cpt.CompareTo(2) > 0)
                        {
                            notes.Add(string.Concat(input[i - 1], 8));
                        }
                        else if (cpt.CompareTo(1) > 0)
                        {
                            notes.Add(string.Concat(input[i - 1], 16));
                        }
                        cpt = 1;
                    }
                }
            }
            return notes;
        }

        public string Format(List<string> input)
        {
            string concat = "";
            foreach (string note in Tempo(input))
            {
                concat = concat + " " + note.Replace("#", "is").Replace("b", "es");
            }
            return concat.ToLower();
        }

        public void SetNotes(List<string> notes)
        {
            for(int i=0;i<data.Count;i++)
            {
                if (data[i].Contains(@"\key")) {
                    
                    data[i+1] = "\t" + Format(notes);
                    break;
                }
            }
            File.WriteAllLines(_File, data);
        }

        public void Customise(string titre, string sTitre, string piece, string pdPage)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].Contains("\tsubtitle =") && sTitre != "") data[i] = "\tsubtitle = \"" + sTitre + "\"";
                if (data[i].Contains("\ttitle =") && titre!= "") data[i] = "\ttitle = \"" + titre + "\"";
                if (data[i].Contains("\tpiece =") && piece != "") data[i] = "\tpiece = \"" + piece + "\"";
                if (data[i].Contains("\ttagline =") && pdPage != "") data[i] = "\ttagline = \"" + pdPage + "\"";
            }
        }
    }
}
