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
        private List<Note> _Note = new List<Note>();
        private List<string> _TexData = new List<string>();
        private string _Path;

        public Latex(string pathparam, Uri Lily)
        {
            string template = File.ReadAllText(@"D:\programmation\c#\TFE\python\LaTex\Template.tex");
            _TexData = new List<string>(template.Split(new string[] { "\r\n" }, StringSplitOptions.None));
            _Path = pathparam;

            Lily lilyFile = new Lily(Lily.LocalPath);
            _Note = lilyFile.GetNotes();
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
            string python = @"python\python.exe";
            string script = @"script\freqs.py";
#endif

            ProcessStartInfo startInfo = new ProcessStartInfo(latexCompiler)
            {
                Arguments = "-aux-directory=D:\\jsp -output-directory=D:\\jsp " + file,
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

        public void BuildRow() {
            initRow();
            List<List<string>> NoteGesture = new List<List<string>>();
            _Note.ForEach(delegate (Note elem)
            {
                Dictionary<string, string> gesture = elem.GetGesture();
               NoteGesture.Add(gesture.Values.ToList());
            });
            South(NoteGesture);
            North(NoteGesture);
        }

        /// <summary>
        /// write init latex code for a tablature
        /// </summary>
        public void initRow()
        {
            _TexData.InsertRange((FindLigneContaining("begin{center}") + 1).Value, new List<string>{ @"\begin{tikzpicture}",
                                                            @"\draw[thick] (0,0) -- (15,0) node[anchor=north west] {}; % main ligne",
                                                            @"\draw (0cm, 1pt) -- (0cm, -1pt) node [anchor=south] {$Do$};  % init ligne Do",
                                                            @"\draw (0cm, 1pt) -- (0cm, -1pt) node [anchor=north] {$Sol$};  % init ligne Sol",
                                                            @"\draw [line width=0.5mm ] (0.5cm, 10pt) -- (0.5cm, -10pt) node {}; % thick vertical ligne",
                                                            @"\draw (0.55cm, 10pt) -- (0.55cm, -10pt) node {}; % second start vertical ligne",
                                                            @"\end{tikzpicture}" });
        }

        /// <summary>
        /// add notes for Do row
        /// </summary>
        public void South(List<List<string>>Gesture)
        {
            createArray("NoteSouthPull", Gesture.Where(x => x.First()[1] == 't').Select(x => x.First()).ToList());
            createArray("NoteSouthPush", Gesture.Where(x => x.First()[1] == 'p').Select(x => x.First()).ToList());
            List<string> text = new List<string> { @"\foreach \x in {" };

            for (int cpt=1; cpt < 5; cpt++) text[0] += cpt + ",";
            text[0] += "}";

            text.Add(@"\node[below] at (\x,-0.5){$\smash{\overline{\NoteSouthPull(\x)}}$};");
            text.Add(@"\foreach \x in {");
            for (int cpt = 5; cpt < Gesture.Count; cpt++) text[2] += cpt + ",";
            text[2] += Gesture.Count + "}";
            text.Add(@"\node[below] at (\x+4,-0.5){$\NoteSouthPush(\x)$};");
            _TexData.InsertRange((FindLigneContaining(@"\end{tikzpicture}")).Value, text);
        }

        public void North(List<List<string>> Gesture)
        {
            createArray("NoteNorth", Gesture.Select(x => x.Last()).ToList());
            List<string> text = new List<string> { @"\foreach \x in {" };

            for (int cpt = 1; cpt < Gesture.Count; cpt++) text[0] += cpt + ",";
            text[0] += Gesture.Count + "}";

            text.Add(@"\node[above] at (\x,0.25){$\NoteNorth(\x)$};");
            _TexData.InsertRange((FindLigneContaining(@"\end{tikzpicture}")).Value, text);
        }

        public void createArray(string arrayName, List<string> array)
        {
            List<string> text = new List<string> { @"\newarray\"+ arrayName };  // define the array
            text.Add(@"\readarray{"+arrayName+"}{");  // add values in the array
            foreach(string data in array)
            {
                text[1] += data + "&";  //arrayjob use & as separation charactere
            }
            text[1].Remove(text[1].Length - 1);
            text[1] += "}";
            _TexData.InsertRange((FindLigneContaining(@"\begin{tikzpicture}")).Value+1, text);
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
