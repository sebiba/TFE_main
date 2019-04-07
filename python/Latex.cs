using IronPython.Runtime.Operations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace python
{
    public class Latex
    {
        List<string> TexData = new List<string>();
        string path;
        public Latex(string pathparam)
        {
            string template = File.ReadAllText(@"D:\programmation\c#\TFE\python\LaTex\Template.tex");
            TexData = new List<string>(template.Split(new string[] { "\r\n" }, StringSplitOptions.None));
            path = pathparam;
        }

        /// <summary>
        /// compile the latex creating a pdf
        /// </summary>
        public void CompileLaTexFile()
        {
#if DEBUG
            string latexCompiler = @"D:\programmation\c#\TFE\python\LaTex\miktex\bin\miktex-pdflatex.exe";
            string file = path;
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
            File.WriteAllLines(path, TexData);  // write all the lignes in the lilypond files
            CompileLaTexFile();
        }

        /// <summary>
        /// write init latex code for a tablature
        /// </summary>
        public void initRow(Uri Lilypath)
        {
            TexData.InsertRange((FindLigneContaining("begin{center}") + 1).Value, new List<string>{ @"\begin{tikzpicture}",
                                                            @"\draw[thick] (0,0) -- (10,0) node[anchor=north west] {}; % main ligne",
                                                            @"\draw (0cm, 1pt) -- (0cm, -1pt) node [anchor=south] {$Do$};  % init ligne Do",
                                                            @"\draw (0cm, 1pt) -- (0cm, -1pt) node [anchor=north] {$Sol$};  % init ligne Sol",
                                                            @"\draw [line width=0.5mm ] (0.5cm, 10pt) -- (0.5cm, -10pt) node {}; % thick vertical ligne",
                                                            @"\draw (0.55cm, 10pt) -- (0.55cm, -10pt) node {}; % second start vertical ligne",
                                                            @"\end{tikzpicture}" });
            South();
            BuildLaTex();
        }

        /// <summary>
        /// add notes for Do row
        /// </summary>
        public void South()
        {
            TexData.InsertRange((FindLigneContaining(@"\end{tikzpicture}")).Value, new List<string> { @"\foreach \x in { 1,3}",
                                                                            @"\draw(\x cm, 1pt)--(\x cm, -1pt) node[anchor = south] {$\x$};"
                                                                            });
        }

        public void North()
        {
        }

        /// <summary>
        /// find ligne containing param
        /// </summary>
        /// <param name="contain">string to find in latex file</param>
        /// <returns></returns>
        public int? FindLigneContaining(string contain)
        {
            for (int ligne = 0; ligne < TexData.Count; ligne++)
            {
                if (TexData[ligne].Contains(contain)) return ligne;
            }
            return null;
        }
    }
}
