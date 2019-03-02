using IronPython.Hosting;
using System;
using System.Diagnostics;
using System.IO;

namespace test
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(freqToNote(200));
            getfreq();
        }
        
        static string freqToNote(int frequence)
        {
            var ipy = Python.CreateRuntime();
            dynamic test = ipy.UseFile("note.py");
            var yolo = test.freqToData(200);
            // Console.WriteLine(yolo);
            return frequence.ToString() + "hz - " + yolo;
        }

        static void getfreq()
        {
            string python = @"D:\Programme file(x86)\python 3.7\python.exe";
            string script = "freqs.py";

            ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(python);

            myProcessStartInfo.UseShellExecute = false;
            myProcessStartInfo.RedirectStandardOutput = true;
            myProcessStartInfo.Arguments = script;

            Process myProcess = new Process();
            myProcess.StartInfo = myProcessStartInfo;

            myProcess.Start();
            StreamReader myStreamReader = myProcess.StandardOutput;
            string myString = myStreamReader.ReadLine();

            myProcess.WaitForExit();
            myProcess.Close();
            Console.WriteLine("Value received from script: " + myString);
        }
    }
}