using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KinectDrawing.Game;
using KinectDrawing.Game.LevelClasses;
using Microsoft.Speech.Recognition;

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

        private SpeechRecognitionEngine speechEngine = null;
        private Sounds.Sounds s;

        private LevelNode currentLevel;
        private int sectionLevel;
        private int levelIndex;


        public MainWindow()
        {
            s = new Sounds.Sounds();

            InitializeComponent();

            this.speechEngine = SpeechRecognition.init();
            this.speechEngine.SpeechRecognized += this.SpeechRecognized;
            
           
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

            }

            //Init GameFlow
            currentLevel = GameFlow.createGameFlow();
            sectionLevel = 1;
            levelIndex = 0;

            updateLettersOnScreen();
        }

        

        private void updateLettersOnScreen()
        {

            LevelLbl.Content = currentLevel.letters.ToString(); 
        }
        
        /// <summary>
        /// Execute initialization tasks.
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
           
        }
        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        //Do 
       
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
            SpeechRecognition.close();
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
            string fileName = @"C:\Anaconda3\envs\tensorenviron\label_image.py " + img_path;
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
         public void predict()
        {  
            //exporting trail
            Polyline newTrain = trail;
            newTrain.Measure(new Size(200, 200));
            newTrain.Arrange(new Rect(new Size(1200, 800)));

            RenderTargetBitmap RTbmap = new RenderTargetBitmap(_width, _height, 96.0, 96.0, PixelFormats.Default);
            RTbmap.Render(newTrain);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(RTbmap));

            string img_name = @"images\imgs" + img_num++ + @".jpg";
            using (var file = File.OpenWrite(img_name))
            {
                encoder.Save(file);
                file.Close();
                if (isPaintingCorrect(img_name))  //Correct !
                {
                    handleSuccessPredict();
                }
                else
                {
                    failAndRestart();
                }


            }
        }
        /// <summary>
        /// method thats determine how to continue with the game
        /// </summary>
        private void handleSuccessPredict()
        {
            if(levelIndex>=currentLevel.letters.Length)
            {
                //the current level is finish
                nextLevel();
            }
            else
            {
                //not finished level - move to next letter
                levelIndex++;
            }
        }

        private void Export_Trail(Object sender, RoutedEventArgs e)
        {
            predict();
        }
        private bool isPaintingCorrect(string img_path)
        {

            char predictLetter = MachineLearning.predict(img_path);
            MessageBox.Show("The Letter is: " + predictLetter.ToString().ToUpper());

            //compare to level's letter.
            char levelLetter = getLetter();
            if (levelLetter.Equals(predictLetter))
                return true;

            return false;
        }

        private char getLetter()
        {
            return currentLevel.letters[levelIndex];
        }

         public void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                //check the speech to the commands
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "Check":
                            predict();
                            break;
                    case "Erase":
                        //erase points
                        trail.Points.Clear();
                        break;
                    case "Toggle":
                        isDrawing = !isDrawing;
                        break;
                }
            }
        }


        private void nextLevel()
        {
            //Good animation

            this.s.playCorrectVoice(); // When the kid answer is correct
            currentLevel = currentLevel.next;
            updateLettersOnScreen(); //update screen
            restart(); //erase painting
        }

        private void failAndRestart()
        {
            //Fail animation
            this.s.playNotCorrectVoice(); // When the kid answer is incorrect
            statusLbl.Content = "Incorrect!! try again please";
            restart();
        }
        private void restart()
        {
            //erase polygon
            trail.Points.Clear();
        }

    }


}


