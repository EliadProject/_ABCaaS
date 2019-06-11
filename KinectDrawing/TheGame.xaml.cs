using Microsoft.Kinect;
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
using KinectDrawing.Sounds;
using KinectDrawing.Game.LevelClasses;
using Microsoft.Samples.Kinect.SpeechBasics;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.AudioFormat;
using Path = System.IO.Path;
using WpfAnimatedGif;
using System.Windows.Media.Animation;
using System.Configuration;
using System.Windows.Threading;

namespace KinectDrawing
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class TheGame : Page
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

        private int screenWidth;
        private bool isRightHand;


        public TheGame(bool isRightHand)
        {
            this.isRightHand = isRightHand;

            s = new Sounds.Sounds();

            InitializeComponent();

            //Init GameFlow
            currentLevel = GameFlow.createGameFlow();

            Loaded += delegate
            {
                if (currentLevel.getLetter().ToString().Length > 1)
                {
                    getScreenSize(currentLevel.getLetter());
                }
            };

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

                changeLetter();
            }
        }


        private void changeLetter()
        {
            LevelLbl.Content = "Letter / Word: " + currentLevel.getLetter();
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

        private void getScreenSize(string letter)
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;

            /*
            Line1.Y1 = 0;
            Line1.Y2 = cameraSize.ActualHeight;
            Line1.X1 = cameraSize.ActualWidth / 3; //screenWidth / 3
            Line1.X2 = cameraSize.ActualWidth / 3; // screenWidth / 3;

            Line2.Y1 = 0;
            Line2.Y2 = cameraSize.ActualHeight;
            Line2.X1 = (cameraSize.ActualWidth * 2) / 3;
            Line2.X2 = (cameraSize.ActualWidth * 2) / 3; 
*/

            for (int i = 1; i <= letter.Length; i++)
            {
                Line line = new Line();
                line.Stroke = Brushes.White;
                line.StrokeThickness = 4;

                line.Y1 = 0;
                line.Y2 = cameraSize.ActualHeight;
                line.X1 = (cameraSize.ActualWidth * i) / letter.Length;
                line.X2 = (cameraSize.ActualWidth * i) / letter.Length;

                canvas.Children.Add(line);
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

        public void splitImageByThree(RenderTargetBitmap RTbmap, PngBitmapEncoder encoder, string img_name)
        {
            var output = @"images";
            using (Stream imageStreamSource = new FileStream(
                img_name, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                PngBitmapDecoder decoder = new PngBitmapDecoder(
                    imageStreamSource,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];



                var widthPicture = (RTbmap.PixelWidth / this.currentLevel.getLetter().Length);
                var heightPicture = RTbmap.PixelHeight;

                for (int i = 2; i <= this.currentLevel.getLetter().Length + 1; i++)
                {
                    CroppedBitmap croppedBitmap = new CroppedBitmap(
                        bitmapSource,
                        new System.Windows.Int32Rect((i - 2) * widthPicture, 0, widthPicture, heightPicture));

                    PngBitmapEncoder encoder2 = new PngBitmapEncoder();
                    var frame = BitmapFrame.Create(croppedBitmap);
                    encoder2.Frames.Add(frame);
                    var fileName = Path.Combine(output, "imgs_" + i.ToString() + ".png");
                    using (var fileToWrite = File.OpenWrite(fileName))
                    {
                        encoder2.Save(fileToWrite);
                        fileToWrite.Close();
                    }
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
                        Joint hand;
                        if (isRightHand)
                            hand = body.Joints[JointType.HandRight];
                        else
                            hand = body.Joints[JointType.HandLeft];

                        if (hand.TrackingState != TrackingState.NotTracked)
                        {
                            CameraSpacePoint handRightPosition = hand.Position;
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
                            double brushX, brushY;
                            if ((newPoint.X - brush.Width/ 2.0) < 0)
                                brushX = 0;
                            else
                                brushX = newPoint.X - brush.Width / 2.0;
                            Canvas.SetLeft(brush,brushX);

                            if (newPoint.Y - brush.Height < 0)
                                brushY = 0;
                            else
                                brushY = newPoint.Y - brush.Height;
                            Canvas.SetTop(brush, brushY);

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
            newTrain.Arrange(new Rect(new Size(_width, _height)));

            RenderTargetBitmap RTbmap = new RenderTargetBitmap(_width, _height, 96.0, 96.0, PixelFormats.Default);
            RTbmap.Render(newTrain);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(RTbmap));

            string img_name = @"images\imgs" + img_num++ + @".jpg";
            using (var file = File.OpenWrite(img_name))
            {
                encoder.Save(file);
                file.Close();
                if (currentLevel.getLetter().ToString().Length > 1)
                {
                    splitImageByThree(RTbmap, encoder, img_name);
                    var isSplitCorrect = false;
                    var isAllSplitCorrect = true;
                    int count = 1;

                    while (isAllSplitCorrect && count <= currentLevel.getLetter().Length)
                    {
                        isAllSplitCorrect = isPaintingCorrectbySplit(@"images\imgs_" + ((count + 1)).ToString() + ".png", count - 1);
                        if (!isAllSplitCorrect)  //Correct !
                        {
                            failAndRestart();
                        }
                        count++;
                    }

                    if (isAllSplitCorrect) // All 3 sub pictures are corret letter 
                        nextLevel();
                    //else
                        //failAndRestart();

                }
                else {
                    if (isPaintingCorrect(img_name))  //Correct !
                    {
                        nextLevel();
                    }
                    else
                    {
                        failAndRestart();
                    }
                }
            }
        }

        private void Export_Trail(Object sender, RoutedEventArgs e)
        {

            predict();
        }


        private bool isPaintingCorrect(string img_path)
        {

            char predictLetter = MachineLearning.predict(img_path);
            //MessageBox.Show("The Letter is: " + predictLetter.ToString().ToUpper());

            //compare to level's letter.
            string levelLetter = currentLevel.getLetter().ToString();
            string chr = predictLetter.ToString().ToUpper();
            if (levelLetter.Equals(chr))
                return true;

            return false;
        }

        private bool isPaintingCorrectbySplit(string img_path, int imageIndex)
        {

            char predictLetter = MachineLearning.predict(img_path);
            //MessageBox.Show("The Letter is: " + predictLetter.ToString().ToUpper());

            //compare to level's letter.
            string levelLetter = currentLevel.getLetter().ToString().Substring(imageIndex, 1).ToUpper();
            string chr = predictLetter.ToString().ToUpper();
            if (levelLetter.Equals(chr))
                return true;

            return false;
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
                        {
                            predict();
                            break;
                        }
                    case "Erase":

                        //erase points
                        trail.Points.Clear();

                        break;
                    case "Start":
                        isDrawing = true;
                        break;
                    case "Stop":
                        isDrawing = false;
                        break;
                }
            }
        }


        private void nextLevel()
        {
            //Good animation
            Animation("AnimationSuccess");

            this.s.playCorrectVoice(); // When the kid answer is correct
            currentLevel = currentLevel.next;
            changeLetter();
            restart();
        }

        private void Animation(string folder)
        {
            Animated.Visibility = Visibility.Visible;
            //read the URI from AppSettings
            var rnd = new Random();
            int num = Int32.Parse(ConfigurationManager.AppSettings[folder+"Num"]);
            var uri = ConfigurationManager.AppSettings[folder] + rnd.Next(1,num) + ".gif";
            int sec = Int32.Parse(ConfigurationManager.AppSettings["AnimateSeconds"]);
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(uri, UriKind.Relative);
            image.EndInit();
            ImageBehavior.SetAnimatedSource(Animated, image);
            ImageBehavior.SetRepeatBehavior(Animated, new RepeatBehavior(TimeSpan.FromSeconds(sec)));

            Task taskAnimate = Task.Run(() => {
                System.Threading.Thread.Sleep(sec * 1000);
                //The calling thread cannot access the object because different thread owns it
                this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    Animated.Visibility = Visibility.Hidden;
                }));
            });


        }
        public void setIsHandRight(bool isRightHand)
        {
            this.isRightHand = isRightHand;
        }

        private void failAndRestart()
        {
            //Fail animation
            Animation("AnimationFail");

            this.s.playNotCorrectVoice(); // When the kid answer is incorrect
            //statusLbl.Content = "Incorrect! try again please";
            restart();
        }
        private void restart()
        {
            //erase polygon
            getScreenSize(currentLevel.getLetter());
            trail.Points.Clear();
        }

        private void goToMenu(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.GoBack(); 
            //load dimenstions of menu 
          //  Application.Current.MainWindow.Height = Double.Parse(ConfigurationManager.AppSettings["MenuHeight"]);
           // Application.Current.MainWindow.Width = Double.Parse(ConfigurationManager.AppSettings["MenuWidth"]);
        }
    }
}

