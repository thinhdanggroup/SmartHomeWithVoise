using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;

namespace SmartHome
{
    class mqttCloud
    {
        static MqttClient client;
        private ConcurrentQueue<string> buffer = new ConcurrentQueue<string>();
        private string luxStatus = "";
        private string tempStatus = "";
        private string humidityStatus = "";
        private DispatcherTimer mqttTimer = new DispatcherTimer();
        private DispatcherTimer statusUploader = new DispatcherTimer();
        private const int LED_PIN = 5;
        private const int AIR_PIN = 6;
        private GpioPin pinLed;
        private GpioPin pinAir;
        private GpioPinValue pinValue;
        private bool autoControlStatus = false;
        public mqttCloud()
        {
            Debug.WriteLine("Hello World! Mqtt");
            client = new MqttClient("m13.cloudmqtt.com", 18822, false, new MqttSslProtocols());
            byte code;
            try
            {
                code = client.Connect(Guid.NewGuid().ToString(), "dyeeeykc", "v5Jn559-ygOm");
            }
            catch (Exception)
            {
                bool flag = true;
                while (flag)
                {
                    flag = false;
                    try
                    {
                        code = client.Connect(Guid.NewGuid().ToString(), "dyeeeykc", "v5Jn559-ygOm");
                    }
                    catch (Exception)
                    {
                        flag = true;
                    }
                }
            }
            client.MqttMsgPublished += client_MqttMsgPublished;
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            //initGPIO();

            mqttTimer.Interval = TimeSpan.FromSeconds(1);
            mqttTimer.Tick += mqttTimerTick;
            mqttTimer.Start();

            statusUploader.Interval = TimeSpan.FromSeconds(10);
            statusUploader.Tick += statusTick;
            statusUploader.Start();

            ushort msgId = client.Subscribe(new string[] { "control" },
            new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }
        /*private void initGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                pinLed = null;
                Debug.WriteLine("There is no GPIO controller on this device.");
                return;
            }
            // initial Led
            pinLed = gpio.OpenPin(LED_PIN);
            pinValue = GpioPinValue.Low;
            pinLed.Write(pinValue);
            pinLed.SetDriveMode(GpioPinDriveMode.Output);
            //initial Air
            pinAir = gpio.OpenPin(AIR_PIN);
            pinValue = GpioPinValue.Low;
            pinAir.Write(pinValue);
            pinAir.SetDriveMode(GpioPinDriveMode.Output);

            Debug.WriteLine("GPIO pin initialized correctly.");

        }
        */
        public void controlDevice(int device, bool status)
        {
            if (status == true) pinValue = GpioPinValue.High;
            else pinValue = GpioPinValue.Low;
            if (device == 0)
            {
                pinLed.Write(pinValue);
                pinLed.SetDriveMode(GpioPinDriveMode.Output);
            }
            else
            {
                pinAir.Write(pinValue);
                pinAir.SetDriveMode(GpioPinDriveMode.Output);
            }
            statusPublish();
        }

        private void mqttTimerTick(object sender, object e)
        {
            while (!buffer.IsEmpty)
            {
                string data;
                while (buffer.TryDequeue(out data))
                {
                    publishData(data);
                }
            }
            if (autoControlStatus == true)
            {
                autoControl();
            }
        }
        private void statusTick(object sender, object e)
        {
            statusPublish();
            sensorPublish();
        }
        private void statusPublish()
        {
            string led;
            string air;
            if (pinLed.Read().ToString() == "High")
            {
                led = "LED ON";
            }
            else led = "LED OFF";
            if (pinAir.Read().ToString() == "High")
            {
                air = "AIR ON";
            }
            else air = "AIR OFF";
            string s = DateTime.Now.ToString();
            sendData(led + " " + air + "\n" + s, "status");
        }
        private void sensorPublish()
        {
            sendData(luxStatus, "lux");
            sendData(tempStatus, "temp");
            sendData(humidityStatus, "humidity");
        }
        public void publishData(string data)
        {
            string[] temp = data.Split('|');
            //Debug.WriteLine(temp[0] + "   " + temp[1]);
            client.Publish(temp[0], // topic
                                          Encoding.UTF8.GetBytes(temp[1]), // message body
                                          MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
                                          false);
        }
        public void sendData(string content, string topic)
        {
            buffer.Enqueue(topic + "|" + content);
        }
        void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            //Debug.WriteLine("MessageId = " + e.MessageId + " Published = " + e.IsPublished);
            //string s = DateTime.Now.ToString();
        }
        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string ReceivedMessage = Encoding.UTF8.GetString(e.Message);
            doCommand(ReceivedMessage);
            Debug.WriteLine(ReceivedMessage);
        }
        private void doCommand(string data)
        {
            if (data.StartsWith("LI ON"))
            {
                Debug.WriteLine("Light On");
                controlDevice(0, true);
            }
            else if (data.StartsWith("LI OFF"))
            {
                Debug.WriteLine("Light Off ");
                controlDevice(0, false);
            }
            else if (data.StartsWith("AC ON"))
            {
                Debug.WriteLine("Air Condition On");
                controlDevice(1, true);
            }
            else if (data.StartsWith("AC OFF"))
            {
                Debug.WriteLine("Air Condition Off");
                controlDevice(1, false);
            }
            else if (data.StartsWith("CHECK"))
            {
                Debug.WriteLine("resubmited");
                statusPublish();
            }
            else if (data.StartsWith("AUTO ON"))
            {
                Debug.WriteLine("auto On");
                autoControlStatus = true;
            }
            else if (data.StartsWith("AUTO OFF"))
            {
                Debug.WriteLine("auto Off");
                autoControlStatus = false;
            }
            else
            {
                Debug.WriteLine("do command have a problem!!!!");
            }
        }
        public void updateData(string humidityCurrent, string luxCurrent, string tempCurrent)
        {
            luxStatus = luxCurrent;
            tempStatus = tempCurrent;
            humidityStatus = humidityCurrent;
        }
        private void autoControl()
        {
            float luxCurrent = float.Parse(luxStatus);
            float tempCurent = float.Parse(tempStatus);
            if (luxCurrent >= 300.0)
            {
                controlDevice(0, false);
            }
            else
            {
                controlDevice(0, false);
            }
            if (tempCurent >= 27.0)
            {
                controlDevice(1, true);
            }
            else
            {
                controlDevice(1, false);
            }
        }

    }
    
}
