﻿using System;
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
        private List<string> _data = new List<string>();

        public Lily(string path)
        {
            _File = path;
            ReadFile();
        }

        /// <summary>
        /// read file passed in parameter or load a template lilypond file
        /// </summary>
        /// <param name="path">path to selected file</param>
        public List<string> ReadFile()
        {
            if (Path.GetExtension(_File) != ".ly") return new List<string> { "Erreur", "File with wrong extension" };  // if file is wrong extension
            if (File.Exists(_File))
            {
                if (_data.Count > 0) return _data;
                using (StreamReader sr = File.OpenText(_File))  // load file from _path
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)  // loop on all the ligne from the file
                    {
                        _data.Add(s);
                    }
                }
            }
            else  // if file doesn't exist
            {
                using (StreamReader sr = File.OpenText(@"D:\programmation\c#\TFE\python\Lily\template.ly"))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)  // loop on all the ligne from the file
                    {
                        _data.Add(s);
                    }
                }
            }
            return _data;
        }


        /// <summary>
        /// get the time of each notes and write
        /// </summary>
        /// <param name="input">list of all notes</param>
        /// <returns>list every notes with the right number for the time</returns>
        public List<string> Tempo(List<string> input)
        {
            List<string> notes = new List<string>();
            int cpt = 1;
            List<int> Freqstep = new List<int> { 16, 8, 4, 2 };
            List<int> Lilystep = new List<int> { 1, 2, 4, 8 };
            int i = 1;
            for ( i = 1; i < input.Count; i++)
            {
                if (input[i] == input[i - 1]) cpt++;
                else
                {
                    if (cpt != 1)
                    {
                        for (int x = 0; x < Freqstep.Capacity; x++)
                        {
                            while (cpt > Freqstep[x])
                            {
                                if (cpt % Freqstep[x] != cpt)
                                {
                                    cpt = cpt % Freqstep[x];
                                    notes.Add(string.Concat(input[i - 1], Lilystep[x]));
                                }
                            }
                        }
                        cpt = 1;
                    }
                }
            }
            if (cpt != 1)  // last range of note not transformed otherwise
            {
                for(int x=0; x<Freqstep.Capacity; x++)
                {
                    while (cpt > Freqstep[x])
                    {
                        if (cpt % Freqstep[x] != cpt)
                        {
                            cpt = cpt % Freqstep[x];
                            notes.Add(string.Concat(input[i - 1], Lilystep[x]));
                        }
                    }
                }
            }
            return notes;
        }

        /// <summary>
        /// replace # and b by is and es to suite lilypond format
        /// </summary>
        /// <param name="input">list of notes</param>
        /// <returns>list of notes in lilypond format</returns>
        public string Format(List<string> input)
        {
            string concat = "";
            foreach (string note in Tempo(input))  // loop on each notes
            {
                concat = concat + " " + note.Replace("#", "is").Replace("b", "es");
            }
            return concat.ToLower();
        }

        /// <summary>
        /// put notes into the lilypond file
        /// </summary>
        /// <param name="notes">notes to put in the file</param>
        /// <returns>what is written inside the lilypond files</returns>
        public string SetNotes(List<string> notes)
        {
            for(int i=0;i<_data.Count;i++)  // loop on each lignes
            {
                if (_data[i].Contains(@"\key")) {
                    
                    _data[i+1] = "\t" + Format(notes);
                    break;
                }
            }
            return Save();
        }

        public string Save(List<string> data = null)
        {
            if (data != null)
            {
                File.WriteAllLines(_File, data);  // write all the lignes in the lilypond files
                return string.Join("\n", data);
            }
            else File.WriteAllLines(_File, _data);  // write all the lignes in the lilypond files
            return string.Join("\n", _data);
        }

        /// <summary>
        /// customise the file
        /// </summary>
        /// <param name="titre">title of the partition</param>
        /// <param name="sTitre">subtitle of the partition</param>
        /// <param name="piece">name of the piece</param>
        /// <param name="pdPage">footer</param>
        public void Customise(string titre, string sTitre, string piece, string pdPage)
        {
            for (int i = 0; i < _data.Count; i++)  // loop on each lignes
            {
                if (_data[i].Contains("\tsubtitle =") && sTitre != "") _data[i] = "\tsubtitle = \"" + sTitre + "\"";
                if (_data[i].Contains("\ttitle =") && titre!= "") _data[i] = "\ttitle = \"" + titre + "\"";
                if (_data[i].Contains("\tpiece =") && piece != "") _data[i] = "\tpiece = \"" + piece + "\"";
                if (_data[i].Contains("tagline =") && pdPage != "") _data[i] = "\ttagline = \"" + pdPage + "\"";
            }
        }
    }
}