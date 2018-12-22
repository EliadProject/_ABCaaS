using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private Uri imagesPath = new Uri(@"C:\Users\Eliad\source\repos\_ABCaaS\imgs");

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
                        Joint handRight = body.Joints[JointType.WristLeft];


                        if (handRight.TrackingState != TrackingState.NotTracked)
                        {
                            CameraSpacePoint handRightPosition = handRight.Position;
                            ColorSpacePoint handRightPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(handRightPosition);

                            float x = handRightPoint.X;
                            float y = handRightPoint.Y;

                            if (!float.IsInfinity(x) && !float.IsInfinity(y))
                            {
                                if (isDrawing)
                                {
                                    // DRAW!
                                    trail.Points.Add(new Point { X = x, Y = y });
                                }
                                    Canvas.SetLeft(brush, x - brush.Width / 2.0);
                                    Canvas.SetTop(brush, y - brush.Height);
                                
                            }

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
        private void ExportTrail_Click(object sender, RoutedEventArgs e)
        {
            //ExportToPng(imagesPath, canvas);
            // Set Polygon.Points properties
            Image resultImage = new Image();

            
            
            trail.Measure(new Size(resultImage.Width, resultImage.Height));
            trail.Arrange(new Rect(new Size(resultImage.Width, resultImage.Height)));

            RenderTargetBitmap RTbmap = new RenderTargetBitmap((int)resultImage.Width,
              (int)resultImage.Height, 96, 96, PixelFormats.Pbgra32);
            
            

        }


        public void ExportToPng(Uri path, Canvas canvas)

        {

            if (path == null) return;

            // Save current canvas transform

            Transform transform = canvas.LayoutTransform;

            // reset current transform (in case it is scaled or rotated)

            canvas.LayoutTransform = null;

            // Get the size of canvas

            Size size = new Size(canvas.Width, canvas.Height);

            // Measure and arrange the canvas

            // VERY IMPORTANT

            canvas.Measure(size);

            canvas.Arrange(new Rect(size));

            // Create a render bitmap and push the canvas to it

            RenderTargetBitmap renderBitmap =

              new RenderTargetBitmap(

                (int)size.Width,

                (int)size.Height,

                96d,

                96d,

                PixelFormats.Pbgra32);

            renderBitmap.Render(canvas);

            // Create a file stream for saving image

            using (FileStream outStream = new FileStream(path.LocalPath, FileMode.Create))

            {

                // Use png encoder for our data

                PngBitmapEncoder encoder = new PngBitmapEncoder();

                // push the rendered bitmap to it

                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                // save the data to the stream

                encoder.Save(outStream);

            }

            // Restore previously saved layout

            canvas.LayoutTransform = transform;

        }


    }
}
