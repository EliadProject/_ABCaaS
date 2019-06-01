using Microsoft.Kinect;
using Microsoft.Samples.Kinect.SpeechBasics;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KinectDrawing
{
    static class SpeechRecognition
    {
        static private KinectSensor _sensor = null;
        static private KinectAudioStream convertStream = null;
        static private SpeechRecognitionEngine speechEngine = null;
       

        static public SpeechRecognitionEngine init()
        {
            SpeechRecognitionEngine.InstalledRecognizers();
            // Only one sensor is supported
            _sensor = KinectSensor.GetDefault();
            if (_sensor != null)
            {
                // open the sensor
                _sensor.Open();

                // grab the audio stream
                IReadOnlyList<AudioBeam> audioBeamList = _sensor.AudioSource.AudioBeams;
                System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();

                // create the convert stream
                convertStream = new KinectAudioStream(audioStream);
            }
            else
            {

                return null;
            }

            RecognizerInfo ri = TryGetKinectRecognizer();
            if (null != ri)
            {
                speechEngine = new SpeechRecognitionEngine(ri.Id);
                var commands = new Choices();

                //define the vocabelery of the commfpytands
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

                speechEngine.LoadGrammar(g);

                // let the convertStream know speech is going active
                convertStream.SpeechActive = true;

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                ////speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);

                speechEngine.SetInputToAudioStream(
                    convertStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }

            return speechEngine;
            
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

        static public void close()
        {
            if (null != convertStream)
            {
                convertStream.SpeechActive = false;
            }

            if (null != speechEngine)
            {

                speechEngine.RecognizeAsyncStop();
            }
        }


    }
}
