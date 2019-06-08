using IronPython.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

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
                throw e;
            }
        }

        /// <summary>
        /// get all frequency inside a wav file
        /// </summary>
        /// <param name="file">path to a wav file</param>
        /// <returns>string with all frequencies found</returns>
        static public List<Note> Getfreq(string file)
        {
#if DEBUG
            string python = @"D:\Programme file(x86)\python\python.exe";
            string script = @"D:\programmation\c#\TFE\python\script\freqs.py";
#else
            string python = @"python\python.exe";
            string script = @"script\freqs.py";
#endif

            if (!File.Exists(file)) throw new FileNotFoundException();// return "Erreur: File doesn't exist";  // if file doesn't exist
            if (Path.GetExtension(file) != ".wav") throw new IOException(); // return "Erreur: File with wrong extension";  // if file is wrong extension

            ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(python);

            myProcessStartInfo.UseShellExecute = false;
            myProcessStartInfo.RedirectStandardOutput = true;
            myProcessStartInfo.Arguments = script + " " + file;

            Process myProcess = new Process
            {
                StartInfo = myProcessStartInfo
            };

            myProcess.Start();
            StreamReader myStreamReader = myProcess.StandardOutput;
            List<Note> Notes = new List<Note>();
            try { 
                myStreamReader.ReadLine().TrimEnd('|').Replace("||", "\n").Split('\n').ToList().ForEach(delegate(string note) {
                    Notes.Add(new Note(Math.Abs(float.Parse(note.Split(':').Last(), CultureInfo.InvariantCulture))));
                });
            }
            catch(Exception e)
            {
                throw e;
            }

            myProcess.WaitForExit();
            myProcess.Close();
            return(Notes);
        }
    }
}
