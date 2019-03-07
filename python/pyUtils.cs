using IronPython.Hosting;
using System;
using System.Diagnostics;
using System.IO;

namespace python
{
    public class PyUtils
    {
        static public string freqToNote(float frequence)
        {
            var ipy = Python.CreateRuntime();
            dynamic python = ipy.UseFile(@"D:\programmation\c#\TFE\python\script\note.py");
            var note = python.freqToData((int)frequence);
            // Console.WriteLine(yolo);
            //return frequence.ToString() + "hz - " + yolo + "\n";
            return note;
        }

        static public string getfreq(string file)
        {
            string python = @"D:\Programme file(x86)\python\python.exe";
            string script = @"D:\programmation\c#\TFE\python\script\freqs.py";

            ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(python);

            myProcessStartInfo.UseShellExecute = false;
            myProcessStartInfo.RedirectStandardOutput = true;
            myProcessStartInfo.Arguments = script + " " + file;

            Process myProcess = new Process();
            myProcess.StartInfo = myProcessStartInfo;

            myProcess.Start();
            StreamReader myStreamReader = myProcess.StandardOutput;
            string myString = myStreamReader.ReadLine();

            myProcess.WaitForExit();
            myProcess.Close();
            return(myString);
        }
    }
}
