using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDrawing
{
    static class MachineLearning
    {
        static public char predict (string img_path)
        {
            /*
             Comments: Images folder path: C:\Users\admin\Desktop\_ABCaaS\KinectDrawing\imgs
                       Python Anaconda path: C:\Users\admin\Anaconda3\envs\tensorenviron\python.exe
                       Label_image.py script path: C:\Users\admin\Desktop\_ABCaaS\KinectDrawing\model\label_image.py
                       Alphabet images path to put all the A-Z images: C:\Users\admin\Anaconda3\envs\tensorenviron\categories

                       Retrain our model: python retrain.py --bottleneck_dir=bottlenecks --how_many_training_steps=500 --model_dir=inception --summaries_dir=training_summaries/basic --output_graph=retrained_graph.pb --output_labels=retrained_labels.txt --image_dir=categories
             */
            string fileName = @"print.py";// img_path;
              /*
            string ProcessStartInfo = ConfigurationManager.AppSettings["ProcessStartInfo"];

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(@ProcessStartInfo, fileName)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            p.Start();


            char letter = p.StandardOutput.ReadToEnd().Split(new[] { '\r', '\n' }).FirstOrDefault()[0];
            p.WaitForExit();
            */
            char letter = PythonProcess.runPython(img_path);
            return letter;
        }
    }
}
