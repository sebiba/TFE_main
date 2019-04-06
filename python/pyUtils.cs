using IronPython.Hosting;
using System;
using System.Diagnostics;
using System.IO;

namespace python
{
    public class PyUtils
    {
        /// <summary>
        /// convert a frequency into the corresponding note
        /// </summary>
        /// <param name="frequence">frequency</param>
        /// <returns>note corresponding to the param in english</returns>
        static public string FreqToNote(float frequence)
        {
            try
            {
#if DEBUG
                dynamic python = Python.CreateRuntime().UseFile(@"D:\programmation\c#\TFE\python\script\note.py");
#else
                dynamic python = Python.CreateRuntime().UseFile(@"script\note.py");
#endif
                return python.freqToData((int)frequence);
            }catch(Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// get all frequency inside a wav file
        /// </summary>
        /// <param name="file">path to a wav file</param>
        /// <returns>string with all frequencies found</returns>
        static public string Getfreq(string file)
        {
#if DEBUG
            string python = @"D:\Programme file(x86)\python\python.exe";
            string script = @"D:\programmation\c#\TFE\python\script\freqs.py";
#else
            string python = @"python\python.exe";
            string script = @"script\freqs.py";
#endif

            if (!File.Exists(file)) return "Erreur: File doesn't exist";  // if file doesn't exist
            if (Path.GetExtension(file) != ".wav") return "Erreur: File with wrong extension";  // if file is wrong extension

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
