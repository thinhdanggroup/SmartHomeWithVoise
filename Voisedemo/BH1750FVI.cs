using Magellanic.I2C;
using System;
using System.Threading.Tasks;

namespace Magellanic.Sensors.BH1750FVI
{
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
    }
}
