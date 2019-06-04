using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            string fileName = @"label_image.py " + img_path;// img_path;

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(@"C:\Users\tamir\Anaconda3\envs\tensorflow\python.exe", fileName)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            p.Start();
            //print the ouput of the tensorflow process
            Debug.WriteLine(p.StandardOutput.ReadToEnd());
            char letter = 'b';
            try
            {
                 letter= p.StandardOutput.ReadToEnd().Split(new[] { '\r', '\n' }).FirstOrDefault()[0];
            }
            catch (IOException e)
            {
                // Extract some information from this exception, and then 
                // throw it to the parent method.
                if (e.Source != null)
                    Console.WriteLine("IOException source: {0}", e.Source);
                throw;
                
            }
            p.WaitForExit();
            return letter;
        }
    }
}
