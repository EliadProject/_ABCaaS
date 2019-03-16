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
            }
        }

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
    

    private void Erase_Click(object sender, RoutedEventArgs e)
    {
        trail.Points.Clear();
    }

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

        string img_name = "../../imgs" + img_num++ + ".jpg";
        using (var file = File.OpenWrite(img_name))
        {
                encoder.Save(file);
                runPythonRetrain(img_name);
        }
    }
}
}
