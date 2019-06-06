using IronPython.Runtime.Operations;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace python
{
    public class Latex
    {
        private List<Note> _Note = new List<Note>();  // all notes of the musique
        private List<string> _TexData = new List<string>();
        private string _Path;

        public Latex(string pathparam, Uri Lily)
        {
            string template = File.ReadAllText(@"D:\programmation\c#\TFE\python\LaTex\Template.tex");
            _TexData = new List<string>(template.Split(new string[] { "\r\n" }, StringSplitOptions.None));
            _Path = pathparam;

            Lily lilyFile = new Lily(Lily.LocalPath);
            _Note = lilyFile.GetNotes();  // save notes
        }

        /// <summary>
        /// compile the latex creating a pdf
        /// </summary>
        public void CompileLaTexFile()
        {
#if DEBUG
            string latexCompiler = @"D:\programmation\c#\TFE\python\LaTex\miktex\bin\miktex-pdflatex.exe";
            string file = _Path;
#else
            string latexCompiler = @"D:\LaTex\miktex\bin\miktex-pdflatex.exe";
            string file = _Path;
#endif

            ProcessStartInfo startInfo = new ProcessStartInfo(latexCompiler)
            {
                Arguments = @"-aux-directory=D:\jsp -output-directory=D:\jsp\tablature " + file,
                UseShellExecute = false
            };
            Process.Start(startInfo);
        }

        /// <summary>
        /// write latex file and create a pdf of the tablature
        /// </summary>
        public void BuildLaTex()
        {
            File.WriteAllLines(_Path, _TexData);  // write all the lignes in the lilypond files
            CompileLaTexFile();
        }

        /// <summary>
        /// build a row of the tablature
        /// </summary>
        public void BuildRow() {
            List<List<string>> NoteGesture = new List<List<string>>();  // list with possible gesture for each notes
            try
            {
                _Note.ForEach(delegate (Note elem)
                {
                    Dictionary<string, string> gesture = elem.GetGesture();
                    NoteGesture.Add(gesture.Values.ToList());
                });
            }
            catch (Exception e)
            {
                throw e;
            }
            for (int i = 0; i < Math.Ceiling((double)NoteGesture.Count/16); i++)
            {
                initRow();
                Dictionary<string, Dictionary<int, string>> formatedData = new Dictionary<string, Dictionary<int, string>>();
                if (NoteGesture.Count > (15 * i) + 15) {  // if there is more than 15 notes left in list
                    formatedData = divideNotes(NoteGesture.GetRange(15*i,15));  // probleme range
                }
                else
                {  // if end of musique is in next ligne
                    formatedData  = divideNotes(NoteGesture.GetRange(15*i,NoteGesture.Count-(15*i)));  // probleme range
                }
                South(formatedData["downTab"]);
                North(formatedData["upTab"]);
                if (NoteGesture.Count < 16) break;
            }
        }

        /// <summary>
        /// write init latex code for a tablature
        /// </summary>
        public void initRow()
        {
            if (FindLigneContaining(@"\end{tikzpicture}") != null)
            {
                _TexData.Reverse();
                for (int ligne = 0; ligne < _TexData.Count; ligne++)
                {
                    if (_TexData[ligne].Contains(@"\end{tikzpicture}"))
                    {
                        _TexData.Reverse();
                        _TexData.InsertRange(_TexData.Count - (ligne), new List<string>{ @"\begin{tikzpicture}",
                                                            @"\draw[thick] (0,0) -- (15,0) node[anchor=north west] {}; % main ligne",
                                                            @"\draw (0cm, 1pt) -- (0cm, -1pt) node [anchor=south] {$Do$};  % init ligne Do",
                                                            @"\draw (0cm, 1pt) -- (0cm, -1pt) node [anchor=north] {$Sol$};  % init ligne Sol",
                                                            @"\draw [line width=0.5mm ] (0.5cm, 10pt) -- (0.5cm, -10pt) node {}; % thick vertical ligne",
                                                            @"\draw (0.55cm, 10pt) -- (0.55cm, -10pt) node {}; % second start vertical ligne",
                                                            @"\end{tikzpicture}" });
                        return;
                    }
                }
                throw new KeyNotFoundException();
            }
            else { 
                _TexData.InsertRange((FindLigneContaining("begin{center}") + 1).Value, new List<string>{ @"\begin{tikzpicture}",
                                                            @"\draw[thick] (0,0) -- (15,0) node[anchor=north west] {}; % main ligne",
                                                            @"\draw (0cm, 1pt) -- (0cm, -1pt) node [anchor=south] {$Do$};  % init ligne Do",
                                                            @"\draw (0cm, 1pt) -- (0cm, -1pt) node [anchor=north] {$Sol$};  % init ligne Sol",
                                                            @"\draw [line width=0.5mm ] (0.5cm, 10pt) -- (0.5cm, -10pt) node {}; % thick vertical ligne",
                                                            @"\draw (0.55cm, 10pt) -- (0.55cm, -10pt) node {}; % second start vertical ligne",
                                                            @"\end{tikzpicture}" });
            }
        }

        /// <summary>
        /// get top and bottom notes
        /// </summary>
        /// <param name="Gesture"></param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<int, string>> divideNotes(List<List<string>> Gesture)
        {
            Dictionary<int, string> uptab = new Dictionary<int, string>();
            Dictionary<int, string> downtab = new Dictionary<int, string>();
            for(int i = 0;i < Gesture.Count;i++)
            {
                downtab.Add(i, Gesture[i].Last());
                uptab.Add(i, Gesture[i].First());
            }
            return new Dictionary<string, Dictionary<int, string>>() { ["upTab"] =uptab , ["downTab"]=downtab};
        }
        
        /// <summary>
        /// add notes for Do row
        /// </summary>
        public void South(Dictionary<int, string> Gesture)
        {
            Dictionary<int, string> noteTirre = Gesture.Where(x => x.Value.Reverse().ToArray()[0] == 't').ToDictionary(x => x.Key, x => x.Value);
            Dictionary<int, string> notePousse = Gesture.Where(x => x.Value.Reverse().ToArray()[0] == 'p').ToDictionary(x => x.Key, x => x.Value);
            List<string> text = createArray("South", Gesture.Values.ToList());
            // pull notes loop
            text.Add(@"\foreach \x in {");
            for (int cpt = 0; cpt < noteTirre.Keys.Count; cpt++)
            {
                text[2]+= (noteTirre.Keys.ElementAt(cpt)+1);
                if (cpt < noteTirre.Keys.Count - 1) text[2] += ",";
            }
            //text[0] += string.Join(",", noteTirre.Keys);  // build the array for the loop
            text[2].Substring(0, text[2].Length -1);
            text[2] += "}";
            text.Add(@"\node[below] at (\x,-0.5){$\smash{\overline{\South(\x)}}$};");  // display all the node to pull

            // push notes loop
            text.Add(@"\foreach \x in {");
            for (int cpt = 0; cpt < notePousse.Keys.Count; cpt++)
            {
                text[4] += (notePousse.Keys.ElementAt(cpt) + 1);
                if (cpt < notePousse.Keys.Count - 1) text[4] += ",";
            }
            //text[2] += string.Join(",", notePousse.Keys);  // build the array for the loop
            text[4].Substring(0, text[4].Length - 1);
            text[4] += "}";
            text.Add(@"\node[below] at (\x,-0.25){$\South(\x)$};");  // display all the node to push

            _TexData.Reverse();
            for (int ligne = 0; ligne < _TexData.Count; ligne++)
            {
                if (_TexData[ligne].Contains(@"\end{tikzpicture}")) {
                    _TexData.Reverse();
                    _TexData.InsertRange(_TexData.Count - (ligne + 1), text);
                    return;
                }
            }
            throw new KeyNotFoundException();
        }

        /// <summary>
        /// add notes for Sol row
        /// </summary>
        public void North(Dictionary<int, string> Gesture)
        {
            Dictionary<int, string> noteTirre = Gesture.Where(x => x.Value.Reverse().ToArray()[0] == 't').ToDictionary(x => x.Key, x => x.Value);
            Dictionary<int, string> notePousse = Gesture.Where(x => x.Value.Reverse().ToArray()[0] == 'p').ToDictionary(x => x.Key, x => x.Value);
            List<string> text = createArray("North", Gesture.Values.ToList());
            // pull notes loop
            text.Add(@"\foreach \x in {");
            for (int cpt = 0; cpt < noteTirre.Keys.Count; cpt++)
            {
                text[2] += (noteTirre.Keys.ElementAt(cpt) + 1);
                if (cpt < noteTirre.Keys.Count - 1) text[2] += ",";
            }
            //text[0] += string.Join(",", noteTirre.Keys);  // build the array for the loop
            text[2].Substring(0, text[2].Length - 1);
            text[2] += "}";
            text.Add(@"\node[above] at (\x,0.25){$\smash{\overline{\North(\x)}}$};");  // display all the node to pull

            // push notes loop
            text.Add(@"\foreach \x in {");
            for (int cpt = 0; cpt < notePousse.Keys.Count; cpt++)
            {
                text[4] += (notePousse.Keys.ElementAt(cpt) + 1);
                if (cpt < notePousse.Keys.Count - 1) text[4] += ",";
            }
            //text[2] += string.Join(",", notePousse.Keys);  // build the array for the loop
            text[4].Substring(0, text[4].Length - 1);
            text[4] += "}";
            text.Add(@"\node[above] at (\x,0.25){$\North(\x)$};");  // display all the node to push

            _TexData.Reverse();
            for (int ligne = 0; ligne < _TexData.Count; ligne++)
            {
                if (_TexData[ligne].Contains(@"\end{tikzpicture}"))
                {
                    _TexData.Reverse();
                    _TexData.InsertRange(_TexData.Count - (ligne + 1), text);
                    return;
                }
            }
            throw new KeyNotFoundException();
        }

        public List<string> createArray(string arrayName, List<string> array)
        {
            List<string> text = new List<string> { @"\newarray\"+ arrayName };  // define the array
            text.Add(@"\readarray{"+arrayName+"}{");  // add values in the array
            foreach(string data in array)
            {
                text[1] += data.Substring(0,data.Length-1) + "&";  //arrayjob use & as separation charactere
            }
            text[1].Substring(0, text[1].Length - 1);
            text[1] += "}";
            return text;
        }

        /// <summary>
        /// find ligne containing param
        /// </summary>
        /// <param name="contain">string to find in latex file</param>
        /// <returns></returns>
        public int? FindLigneContaining(string contain)
        {
            for (int ligne = 0; ligne < _TexData.Count; ligne++)
            {
                if (_TexData[ligne].Contains(contain)) return ligne;
            }
            return null;
        }
    }
}
