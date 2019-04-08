using System;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System.Globalization;
namespace KinectDrawing
{
    class speech_recognition
    {
        static SpeechSynthesizer ss = new SpeechSynthesizer();
        static SpeechRecognitionEngine sre;
        static bool done = false;
        static bool speechOn = true;
       /* static void Main(string[] args)
        {
            try
            {
                //The SpeechSynthesizer object was instantiated when it was declared. 
                //Using a synthesizer object is quite simple.
                //The SetOutput­ToDefaultAudioDevice method sends output to your machine’s speakers
                //The Speak method accepts a string and then, well, speaks.
                ss.SetOutputToDefaultAudioDevice();
                Console.WriteLine("\n(Speaking: I am awake)");
                ss.Speak("I am awake");


                //setting recognizer to english and audio device to deafult computer device
                CultureInfo ci = new CultureInfo("en-us");
                sre = new SpeechRecognitionEngine(ci);
                sre.SetInputToDefaultAudioDevice();
                sre.SpeechRecognized += sre_SpeechRecognized;


                Choices ch_StartStopCommands = new Choices();
                ch_StartStopCommands.Add("start practice");
                ch_StartStopCommands.Add("stop practice");
                GrammarBuilder gb_StartStop = new GrammarBuilder();
                gb_StartStop.Append(ch_StartStopCommands);
                Grammar g_StartStop = new Grammar(gb_StartStop);

                Choices ch_actions = new Choices();
                ch_actions.Add("i finished");
                ch_actions.Add("clean screen");
                GrammarBuilder gb_actions = new GrammarBuilder();
                gb_actions.Append(ch_actions);
                Grammar g_WhatIsXplusY = new Grammar(gb_actions);

                sre.LoadGrammarAsync(g_StartStop);
                sre.LoadGrammarAsync(g_WhatIsXplusY);
                sre.RecognizeAsync(RecognizeMode.Multiple);
                while (done == false) {; }
                Console.WriteLine("\nHit <enter> to close shell\n");
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        } // Main

        private static void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            throw new NotImplementedException();
        }

        static void sre_SpeechRecognized(object sender,
          SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            float confidence = e.Result.Confidence;
            Console.WriteLine("\nRecognized: " + txt);
            if (confidence < 0.60) return;
            if (txt.IndexOf("speech on") >= 0)
            {
                Console.WriteLine("Speech is now ON");
                speechOn = true;
            }
            if (txt.IndexOf("speech off") >= 0)
            {
                Console.WriteLine("Speech is now OFF");
                speechOn = false;
            }
            if (speechOn == false) return;

            if (txt.IndexOf("What") >= 0 && txt.IndexOf("plus") >= 0)
            {
                string[] words = txt.Split(' ');
                int num1 = int.Parse(words[2]);
                int num2 = int.Parse(words[4]);
                int sum = num1 + num2;
                Console.WriteLine("(Speaking: " + words[2] + " plus " +
                  words[4] + " equals " + sum + ")");
                ss.SpeakAsync(words[2] + " plus " + words[4] +
                  " equals " + sum);
            }

        } // sre_SpeechRecognized
        */
    } // Program
} // ns
