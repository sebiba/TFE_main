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

        public void BuildLaTex()
        {
            File.WriteAllLines(path, TexData);  // write all the lignes in the lilypond files
            CompileLaTexFile();
        }

        public void initRow()
        {
            int ligne = 0;
            for (ligne = 0; ligne < TexData.Count; ligne++)
            {
                if (TexData[ligne].Contains("begin{center}"))  break;
            }
            TexData.InsertRange(ligne + 1, new List<string>{ @"\begin{tikzpicture}",
                                                            @"\draw[thick] (0,0) -- (10,0) node[anchor=north west] {}; % main ligne",
                                                            @"\draw (0cm, 1pt) -- (0cm, -1pt) node [anchor=south] {$Do$};  % init ligne Do",
                                                            @"\draw (0cm, 1pt) -- (0cm, -1pt) node [anchor=north] {$Sol$};  % init ligne Sol",
                                                            @"\draw [line width=0.5mm ] (0.5cm, 10pt) -- (0.5cm, -10pt) node {}; % thick vertical ligne",
                                                            @"\draw (0.55cm, 10pt) -- (0.55cm, -10pt) node {}; % second start vertical ligne",
                                                            @"\end{tikzpicture}" });
            BuildLaTex();
        }

        public void South()
        {

        }

        public void North()
        {
        }
    }
}
