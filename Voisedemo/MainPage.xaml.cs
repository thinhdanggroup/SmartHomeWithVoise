using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SmartHome
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static mqttCloud mqtt;
        private buttonListener button;
        private sensorListener sensor;
        private speechRecognizer recognizer;
        public MainPage()
        {
            this.InitializeComponent();
            //init
            /*try
            {
                mqtt = new mqttCloud();
            }
            catch (IOException)
            {
                mqtt = new mqttCloud();
            }*/
            mqtt = new mqttCloud();
            recognizer = new speechRecognizer(mqtt);
            button = new buttonListener(mqtt);
            sensor = new sensorListener(mqtt);
        }
       /*
        private async void initializeSpeechRecognizer()
        {
            // Initialize SpeechRecognizer Object
            MyRecognizer = new SpeechRecognizer();

            // Register Event Handlers
            MyRecognizer.StateChanged += myRecognizerStateChanged;
            MyRecognizer.ContinuousRecognitionSession.ResultGenerated += myRecognizerResultGenerated;

            // Create Grammar File Object
            StorageFile GrammarContentFile = await Package.Current.InstalledLocation.GetFileAsync(@"Grammar\MyGrammar.xml");

            // Add Grammar Constraint from Grammar File
            SpeechRecognitionGrammarFileConstraint GrammarConstraint = new SpeechRecognitionGrammarFileConstraint(GrammarContentFile);
            MyRecognizer.Constraints.Add(GrammarConstraint);

            // Compile Grammar
            SpeechRecognitionCompilationResult CompilationResult = await MyRecognizer.CompileConstraintsAsync();

            // Write Debug Information
            Debug.WriteLine("Status: " + CompilationResult.Status.ToString() );

            // If Compilation Successful, Start Continuous Recognition Session
            if (CompilationResult.Status == SpeechRecognitionResultStatus.Success)
            {
                await MyRecognizer.ContinuousRecognitionSession.StartAsync();
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
                    mqtt.controlDevice(0,true);
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
        */
    }
}
