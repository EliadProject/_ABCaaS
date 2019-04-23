﻿using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using KinectDrawing.Game;
using KinectDrawing.Game.LevelClasses;

using Microsoft.Samples.Kinect.SpeechBasics;
using System.Speech.Recognition;
using System.Speech.AudioFormat;

namespace KinectDrawing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private KinectSensor _sensor = null;
        private ColorFrameReader _colorReader = null;
        private BodyFrameReader _bodyReader = null;
        private IList<Body> _bodies = null;
        private bool isDrawing = true;

        private int _width = 0;
        private int _height = 0;
        private byte[] _pixels = null;
        private WriteableBitmap _bitmap = null;

        Point lastPoint;
        Point newPoint;

        private static int img_num = 1;
        private static string root_path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        /// <summary>
        /// Stream for 32b-16b conversion.
        /// </summary>
        private KinectAudioStream convertStream = null;

        /// <summary>
        /// Speech recognition engine using audio data from Kinect.
        /// </summary>
        private SpeechRecognitionEngine speechEngine = null;

        private LevelNode currentLevel;

        public MainWindow()
        {
            InitializeComponent();
           
            _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {
                _sensor.Open();

                _width = _sensor.ColorFrameSource.FrameDescription.Width;
                _height = _sensor.ColorFrameSource.FrameDescription.Height;

                _colorReader = _sensor.ColorFrameSource.OpenReader();
                _colorReader.FrameArrived += ColorReader_FrameArrived;

                _bodyReader = _sensor.BodyFrameSource.OpenReader();
                _bodyReader.FrameArrived += BodyReader_FrameArrived;

                _pixels = new byte[_width * _height * 4];
                _bitmap = new WriteableBitmap(_width, _height, 96.0, 96.0, PixelFormats.Bgra32, null);

                _bodies = new Body[_sensor.BodyFrameSource.BodyCount];

                camera.Source = _bitmap;

                //Init GameFlow
                currentLevel = GameFlow.createGameFlow();
                changeLetter();

            }
        }

        private void changeLetter()
        {
            LevelLbl.Text = "Letter: " + currentLevel.getLetter();
        }
        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>
        /// RecognizerInfo if found, <code>null</code> otherwise.
        /// </returns>
        private static RecognizerInfo TryGetKinectRecognizer()
        {
            IEnumerable<RecognizerInfo> recognizers;
           
            // This is required to catch the case when an expected recognizer is not installed.
            // By default - the x86 Speech Runtime is always expected. 
            try
            {
                recognizers = SpeechRecognitionEngine.InstalledRecognizers();
            }
            catch (COMException)
            {
                return null;
            }

            foreach (RecognizerInfo recognizer in recognizers)
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }
        /// <summary>
        /// Execute initialization tasks.
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Winows loadded");
            // Only one sensor is supported
            this._sensor = KinectSensor.GetDefault();
            if (this._sensor != null)
            {
                // open the sensor
                this._sensor.Open();

                // grab the audio stream
                IReadOnlyList<AudioBeam> audioBeamList = this._sensor.AudioSource.AudioBeams;
                System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();

                // create the convert stream
                this.convertStream = new KinectAudioStream(audioStream);
            }
            else
            {
                // on failure, set the status text
                  MessageBox.Show("Error1");
            
                return;
            }

            RecognizerInfo ri = TryGetKinectRecognizer();
            if (null != ri)
            {
                this.speechEngine = new SpeechRecognitionEngine(ri.Id);
                  var commands = new Choices();

                //define the vocabelery of the commands
                  commands.Add(new SemanticResultValue("check result", "Check"));
                commands.Add(new SemanticResultValue("check", "Check"));
                commands.Add(new SemanticResultValue("checks result", "Check"));
                commands.Add(new SemanticResultValue("Erase Screen", "Erase"));
                commands.Add(new SemanticResultValue("Erase", "Erase"));
                commands.Add(new SemanticResultValue("Erases", "Erase"));
                commands.Add(new SemanticResultValue("toggles", "Toggle"));
                  commands.Add(new SemanticResultValue("Toggle", "Toggle"));
                
                  var gb = new GrammarBuilder { Culture = ri.Culture };
                  gb.Append(commands);
                 
                  var g = new Grammar(gb);

               this.speechEngine.LoadGrammar(g);
                

                this.speechEngine.SpeechRecognized += this.SpeechRecognized;
                

                // let the convertStream know speech is going active
                this.convertStream.SpeechActive = true;

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                ////speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);

                this.speechEngine.SetInputToAudioStream(
                    this.convertStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                this.speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {
                
            }
        }
        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        //Do 
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                //check the speech to the commands
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "Check":
                        {
                            Polyline newTrain = trail;
                            newTrain.Measure(new Size(200, 200));
                            newTrain.Arrange(new Rect(new Size(1200, 800)));

                            RenderTargetBitmap RTbmap = new RenderTargetBitmap(_width, _height, 96.0, 96.0, PixelFormats.Default);
                            RTbmap.Render(newTrain);

                            var encoder = new PngBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(RTbmap));

                            string img_name = "../../imgs" + img_num++ + ".jpg";
                            using (var file = File.OpenWrite(img_name))
                            {
                                encoder.Save(file);
                                runPythonRetrain(img_name);
                            }
                            MessageBox.Show("Mazel Tov!");
                            break;
                            
                        }
                    case "Erase":
                        trail.Points.Clear();
                        MessageBox.Show("Erase your mind");
                        break;
                    case "Toggle":
                        isDrawing = !isDrawing;
                        MessageBox.Show("Toggle your self");
                        break;
                }
            }
        }
        /// <summary>
        /// Execute un-initialization tasks.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_colorReader != null)
            {
                _colorReader.Dispose();
            }

            if (_bodyReader != null)
            {
                _bodyReader.Dispose();
            }

            if (_sensor != null)
            {
                _sensor.Close();
            }
            if (null != this.convertStream)
            {
                this.convertStream.SpeechActive = false;
            }

            if (null != this.speechEngine)
            {
                
                this.speechEngine.RecognizeAsyncStop();
            }

            
        }

        private void ColorReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    frame.CopyConvertedFrameDataToArray(_pixels, ColorImageFormat.Bgra);

                    _bitmap.Lock();
                    Marshal.Copy(_pixels, 0, _bitmap.BackBuffer, _pixels.Length);
                    _bitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
                    _bitmap.Unlock();
                }
            }
        }

        private void BodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    frame.GetAndRefreshBodyData(_bodies);

                    Body body = _bodies.Where(b => b.IsTracked).FirstOrDefault();

                    if (body != null)
                    {
                        Joint handRight = body.Joints[JointType.HandLeft];


                        if (handRight.TrackingState != TrackingState.NotTracked)
                        {
                            CameraSpacePoint handRightPosition = handRight.Position;
                            ColorSpacePoint handRightPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(handRightPosition);

                            float x = handRightPoint.X;
                            float y = handRightPoint.Y;
                            newPoint = new Point(x, y);
                            if (!float.IsInfinity(x) && !float.IsInfinity(y))
                            {
                                if (isDrawing)
                                {

                                    //Auclid distance
                                    double distance = Math.Sqrt(Math.Pow((newPoint.X - lastPoint.X), 2) + Math.Pow((newPoint.Y - lastPoint.Y), 2));
                                    //draw only if it's the first run or the distance is between configured range 
                                    if ((lastPoint.X == 0 && lastPoint.Y == 0) || distance > 5 && distance < 30)
                                    {
                                        trail.Points.Add(newPoint);
                                    }
                                }
                                else
                                {
                                    // DRAW!
                                    trail.Points.Add(newPoint);
                                }
                                lastPoint = newPoint;

                            }
                            Canvas.SetLeft(brush, newPoint.X - brush.Width / 2.0);
                            Canvas.SetTop(brush, newPoint.Y - brush.Height);

                        }

                    }
                }
            }
        }

        //action button for erase_click
        
        private void Erase_Click(object sender, RoutedEventArgs e)
        {
            trail.Points.Clear();
        }
        //action button for toggle_click
        private void Toggle_Click(object sender, RoutedEventArgs e)
        {
            isDrawing = !isDrawing;
        }
        
        private void runPythonRetrain(string img_path)
        {
            string fileName = @"C:\Users\admin\Anaconda3\envs\tensorenviron\label_image.py " + img_path;
            // Example - C:\Users\admin\Anaconda3\envs\tensorenviron\1.jpg

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(@"python.exe", fileName)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            MessageBox.Show(output);

            Console.ReadLine();
        }


        private void Export_Trail(Object sender, RoutedEventArgs e)
        {
            Polyline newTrain = trail;
            newTrain.Measure(new Size(200, 200));
            newTrain.Arrange(new Rect(new Size(1200, 800)));

            RenderTargetBitmap RTbmap = new RenderTargetBitmap(_width, _height, 96.0, 96.0, PixelFormats.Default);
            RTbmap.Render(newTrain);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(RTbmap));

            string img_name = @"C:\Users\admin\Desktop\_ABCaaS\KinectDrawing\imgs\imgs" + img_num++ + ".jpg";
            using (var file = File.OpenWrite(img_name))
            {
                //encoder.Save(file);
                if (isPaintingCorrect(img_name))  //Correct !
                {
                    nextLevel();
                }
                else
                {
                    lbl.Text = "Incorrect!! try again please";
                    restart();
                }


            }

        }


        private bool isPaintingCorrect(string img_path)
        {
            /*
             Comments: Images folder path: C:\Users\admin\Desktop\_ABCaaS\KinectDrawing\imgs
                       Python Anaconda path: C:\Users\admin\Anaconda3\envs\tensorenviron\python.exe
                       Label_image.py script path: C:\Users\admin\Desktop\_ABCaaS\KinectDrawing\model\label_image.py
                       Alphabet images path to put all the A-Z images: C:\Users\admin\Anaconda3\envs\tensorenviron\categories

                       Retrain our model: python retrain.py --bottleneck_dir=bottlenecks --how_many_training_steps=500 --model_dir=inception --summaries_dir=training_summaries/basic --output_graph=retrained_graph.pb --output_labels=retrained_labels.txt --image_dir=categories
             */
            string fileName = @"C:\Users\admin\Desktop\_ABCaaS\KinectDrawing\model\label_image.py " + img_path;

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(@"C:\Users\admin\Anaconda3\envs\tensorenviron\python.exe", fileName)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            p.Start();

            char letter = p.StandardOutput.ReadToEnd().Split(new[] { '\r', '\n' }).FirstOrDefault()[0];
            p.WaitForExit();

            MessageBox.Show("The Letter is: " + letter.ToString().ToUpper());

            //compare to level's letter.
            char levelLetter = currentLevel.getLetter();
            if (levelLetter.Equals(letter))
                return true;

            return false;
        }

        private void nextLevel()
        {
            //Good animation
            currentLevel = currentLevel.next;
            changeLetter();
            restart();
        }

        private void failAndRestart()
        {
            //Fail animation
            restart();
        }
        private void restart()
        {
            //erase polygon
            trail.Points.Clear();
        }

    }


}


