using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDrawing
{
    class PythonProcess
    {
        static private PythonProcess pythonProcess;
        private Process proc;
        private PythonProcess()
        {

            proc = new Process();
            proc.StartInfo.FileName = @"C:\Anaconda3\envs\tensorenviron\python.exe";
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
        }
        static public void initProcess()
        {
            if (pythonProcess == null)
                pythonProcess = new PythonProcess();
        }
        static public PythonProcess getInstance()
        {
            if (pythonProcess == null)
                pythonProcess = new PythonProcess();
            return pythonProcess;
        }
        static public char runPython(string img_path)
        {
            string fileName = @"label_image.py " + img_path;// img_path;
            PythonProcess pythonProc = PythonProcess.getInstance();
            pythonProc.proc.StartInfo.Arguments = fileName;
            pythonProc.proc.Start();
            char letter = pythonProc.proc.StandardOutput.ReadToEnd().Split(new[] { '\r', '\n' }).FirstOrDefault()[0];
            return letter;
        }
    }
}