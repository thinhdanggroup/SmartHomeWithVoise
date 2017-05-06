using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Magellanic.I2C;
using Windows.UI.Xaml.Controls;

namespace Voisedemo
{
    class sensorLightX
    {
        public sensorLightX()
        {
            Debug.WriteLine("Hello sensor light");
            initialSensorLight();
            //Loaded += MainPage_Loaded;
        }
        private async void initialSensorLight()
        {
            try
            {
                var lightSensitivityMeter = new BH1750FVI(AddPinConnection.PIN_LOW);
                Debug.WriteLine("new bh1750 success");
                await lightSensitivityMeter.Initialize();

                lightSensitivityMeter.PowerOn();
                lightSensitivityMeter.Reset();

                lightSensitivityMeter.SetMode(MeasurementMode.ContinuouslyHighResolutionMode);

                while (true)
                {
                    var lux = lightSensitivityMeter.GetLightLevel();

                    Debug.WriteLine("Lux = " + lux);

                    Task.Delay(1000).Wait();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("loi initial sendor light");
                Debug.WriteLine(ex.Message);
            }
        }

    }
    public class BH1750FVI : AbstractI2CDevice
    {
        private byte I2C_ADDRESS;

        public BH1750FVI(AddPinConnection pinConnection)
        {
            I2C_ADDRESS = (byte)pinConnection;
        }

        public override byte[] GetDeviceId()
        {
            throw new NotImplementedException();
        }

        public override byte GetI2cAddress()
        {
            return I2C_ADDRESS;
        }

        public int GetLightLevel()
        {
            var readBuffer = new byte[2];

            this.Slave.WriteRead(new byte[] { I2C_ADDRESS }, readBuffer);

            var lightLevel = readBuffer[0] << 8 | readBuffer[1];

            return (int)(lightLevel / 1.2f);
        }

        public void PowerOff()
        {
            this.Slave.Write(new byte[] { 0x00 });
        }

        public void PowerOn()
        {
            this.Slave.Write(new byte[] { 0x01 });
        }

        public void Reset()
        {
            this.Slave.Write(new byte[] { 0x07 });
        }

        public void SetMode(MeasurementMode measurementMode)
        {
            this.Slave.Write(new byte[] { (byte)measurementMode });
            Task.Delay(10).Wait();
        }

        internal Task Initialize()
        {
            throw new NotImplementedException();
        }
    }
    public enum MeasurementMode
    {
        ContinuouslyHighResolutionMode = 0x10,
        ContinuouslyHighResolutionMode2 = 0x11,
        ContinuouslyLowResolutionMode = 0x13,
        OneTimeHighResolutionMode = 0x20,
        OneTimeHighResolutionMode2 = 0x21,
        OneTimeLowResolutionMode = 0x23
    }
    public enum AddPinConnection
    {
        PIN_HIGH = 0x5C,
        PIN_LOW = 0x23
    }
}

/*using Magellanic.I2C;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Voisedemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class sensorLightX : Page
    {
        public sensorLightX()
        {
            Debug.WriteLine("Hello sensor light");
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }
        private async void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Debug.WriteLine("t toi day roi");
            try
            {
                var lightSensitivityMeter = new BH1750FVI(AddPinConnection.PIN_LOW);

                await lightSensitivityMeter.Initialize();

                lightSensitivityMeter.PowerOn();
                lightSensitivityMeter.Reset();

                lightSensitivityMeter.SetMode(MeasurementMode.ContinuouslyHighResolutionMode);

                while (true)
                {
                    var lux = lightSensitivityMeter.GetLightLevel();

                    Debug.WriteLine("Lux = " + lux);

                    Task.Delay(1000).Wait();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

    }
    public class BH1750FVI : AbstractI2CDevice
    {
        private byte I2C_ADDRESS;

        public BH1750FVI(AddPinConnection pinConnection)
        {
            I2C_ADDRESS = (byte)pinConnection;
        }

        public override byte[] GetDeviceId()
        {
            throw new NotImplementedException();
        }

        public override byte GetI2cAddress()
        {
            return I2C_ADDRESS;
        }

        public int GetLightLevel()
        {
            var readBuffer = new byte[2];

            this.Slave.WriteRead(new byte[] { I2C_ADDRESS }, readBuffer);

            var lightLevel = readBuffer[0] << 8 | readBuffer[1];

            return (int)(lightLevel / 1.2f);
        }

        public void PowerOff()
        {
            this.Slave.Write(new byte[] { 0x00 });
        }

        public void PowerOn()
        {
            this.Slave.Write(new byte[] { 0x01 });
        }

        public void Reset()
        {
            this.Slave.Write(new byte[] { 0x07 });
        }

        public void SetMode(MeasurementMode measurementMode)
        {
            this.Slave.Write(new byte[] { (byte)measurementMode });
            Task.Delay(10).Wait();
        }

        internal Task Initialize()
        {
            throw new NotImplementedException();
        }
    }
    public enum MeasurementMode
    {
        ContinuouslyHighResolutionMode = 0x10,
        ContinuouslyHighResolutionMode2 = 0x11,
        ContinuouslyLowResolutionMode = 0x13,
        OneTimeHighResolutionMode = 0x20,
        OneTimeHighResolutionMode2 = 0x21,
        OneTimeLowResolutionMode = 0x23
    }
    public enum AddPinConnection
    {
        PIN_HIGH = 0x5C,
        PIN_LOW = 0x23
    }
}*/