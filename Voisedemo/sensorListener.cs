using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
using Windows.Devices.Enumeration;

using Windows.Devices.I2c;
using Porrey.Uwp.IoT.Devices.Arduino;
using System.Diagnostics;
using System.Threading;

namespace SmartHome
{
    class sensorListener
    {
        private I2cDevice device;

        private Timer periodicTimer;
        mqttCloud mqtt;
        public sensorListener(mqttCloud mqttCurrent)
        {
            mqtt = mqttCurrent;
            initI2C();
        }
        public async void initI2C()

        {

            var settings = new I2cConnectionSettings(0x40); // Arduino address

            settings.BusSpeed = I2cBusSpeed.StandardMode;

            string aqs = I2cDevice.GetDeviceSelector("I2C1");

            var dis = await DeviceInformation.FindAllAsync(aqs);

            device = await I2cDevice.FromIdAsync(dis[0].Id, settings);

            periodicTimer = new Timer(this.TimerCallback, null, 0, 5000); // Create a timmer

        }

        private void TimerCallback(object state)

        {
           
            byte[] RegAddrBuf = new byte[] { 0x40 };

            byte[] ReadBuf = new byte[18];

            try

            {

                device.Read(ReadBuf); // read the data

            }

            catch (Exception f)

            {

                Debug.WriteLine(f.Message);

            }
            //Debug.WriteLine("dasdas");
            char[] cArray = System.Text.Encoding.UTF8.GetString(ReadBuf, 0, 17).ToCharArray();  // Converte  Byte to Char
           // Debug.WriteLine("array : "+ cArray);
            String c = new String(cArray);

            //Debug.WriteLine(c);

            // refresh the screen, note Im using a textbock @ UI
            //Debug.WriteLine("ket qua " + c);
            send2mqtt(c);
            

        }
        private void send2mqtt(string data)
        {
            string[] splitData = data.Split(' ');
            if (splitData.Length >= 3)
            {
                Debug.WriteLine(splitData[0] + "|" + splitData[1] + "|" + splitData[2]);
                mqtt.updateData(splitData[0],splitData[1], splitData[2]);
            }
        }
    }
}
