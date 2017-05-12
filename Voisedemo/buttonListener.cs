using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace SmartHome
{
    class buttonListener
    {
        mqttCloud mqttMain;
        private const int BUTTON_PIN = 19;
        private GpioPin inputRedPin;
        private GpioPinValue pinValue;
        public buttonListener(mqttCloud mqttCurrent)
        {
            mqttMain = mqttCurrent;
            initGPIO();
        }
        public void initGPIO()
        {
            var gpio = GpioController.GetDefault();
            //initial button

            inputRedPin = gpio.OpenPin(BUTTON_PIN);
            if (inputRedPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                inputRedPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                inputRedPin.SetDriveMode(GpioPinDriveMode.Input);

            inputRedPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);

            // Register for the ValueChanged event so our buttonPinValueChanged 
            // function is called when the button is pressed
            inputRedPin.ValueChanged += buttonPinValueChanged;
            Debug.WriteLine("button ok");

        }
        private void buttonPinValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            // toggle the state of the LED every time the button is pressed

            if (e.Edge == GpioPinEdge.FallingEdge)
            {

                if (pinValue == GpioPinValue.Low)
                {

                    mqttMain.controlDevice(0, true);
                }
                //(pinValue == GpioPinValue.High)
                else
                {
                    mqttMain.controlDevice(0, false);
                }
            }
        }
    }
}
