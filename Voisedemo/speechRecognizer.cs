using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media.SpeechRecognition;
using Windows.Storage;

namespace SmartHome
{
    
    class speechRecognizer
    {
        private SpeechRecognizer myRecognizer;
        mqttCloud mqtt;
        public speechRecognizer(mqttCloud mqttCurrent)
        {
            mqtt = mqttCurrent;
        }
        public async void initializeSpeechRecognizer()
        {
            // Initialize SpeechRecognizer Object
            myRecognizer = new SpeechRecognizer();

            // Register Event Handlers
            myRecognizer.StateChanged += myRecognizerStateChanged;
            myRecognizer.ContinuousRecognitionSession.ResultGenerated += myRecognizerResultGenerated;

            // Create Grammar File Object
            StorageFile GrammarContentFile = await Package.Current.InstalledLocation.GetFileAsync(@"Grammar\MyGrammar.xml");

            // Add Grammar Constraint from Grammar File
            SpeechRecognitionGrammarFileConstraint GrammarConstraint = new SpeechRecognitionGrammarFileConstraint(GrammarContentFile);
            myRecognizer.Constraints.Add(GrammarConstraint);

            // Compile Grammar
            SpeechRecognitionCompilationResult CompilationResult = await myRecognizer.CompileConstraintsAsync();

            // Write Debug Information
            Debug.WriteLine("Status: " + CompilationResult.Status.ToString());

            // If Compilation Successful, Start Continuous Recognition Session
            if (CompilationResult.Status == SpeechRecognitionResultStatus.Success)
            {
                await myRecognizer.ContinuousRecognitionSession.StartAsync();
            }
        }
        private void myRecognizerStateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            // Write Debug Information
            Debug.WriteLine("State current " + args.State);
        }

        private void myRecognizerResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            // Write Debug Information
            Debug.WriteLine(args.Result.Text);
            // Control device on recognized speech
            switch (args.Result.Text)
            {
                case "led on":
                    Debug.WriteLine("STATUS: LED ON");
                    //mqtt.sendData("Led on", "testiot");
                    mqtt.controlDevice(0, true);
                    break;
                case "led off":
                    Debug.WriteLine("STATUS: LED OFF");
                    //mqtt.sendData("Led off", "testiot");
                    mqtt.controlDevice(0, false);
                    break;
                case "air on":
                    Debug.WriteLine("STATUS: AIR ON");
                    // mqtt.sendData("Led off", "testiot");
                    mqtt.controlDevice(1, true);
                    break;
                case "air off":
                    Debug.WriteLine("STATUS: AIR OFF");
                    //mqtt.sendData("Led off", "testiot");
                    mqtt.controlDevice(1, false);
                    break;
                default:

                    break;
            }

            // Turn on/off obstacle detection

        }
    }
}
