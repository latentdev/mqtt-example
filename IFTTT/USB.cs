using HidSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFTTT
{
    class USB
    {
        private static USB instance;
        private HidDevice device;
        private HidStream stream;

        private USB()
        {
            HidDeviceLoader deviceLoader = new HidDeviceLoader();
            IEnumerable<HidDevice> devices = deviceLoader.GetDevices();
            device = devices.Where(x => (x.VendorID == 5824 && x.ProductID == 1158 && x.ProductName == "Teensyduino RawHID")).FirstOrDefault();
        }

        public static USB Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new USB();
                }
                return instance;
            }
        }

        public void ChangeDevice(string deviceName)
        {
            HidDeviceLoader deviceLoader = new HidDeviceLoader();
            IEnumerable<HidDevice> devices = deviceLoader.GetDevices();
            device = devices.Where(x => (x.ProductName == deviceName)).FirstOrDefault();
        }


        public bool Close()
        {
            try
            {
                stream.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public IEnumerable<HidDevice> GetDeviceList()
        {
            HidDeviceLoader deviceLoader = new HidDeviceLoader();
            return deviceLoader.GetDevices();
        }

        public bool Open()
        {
            try
            {
                stream = device.Open();
                stream.ReadTimeout = System.Threading.Timeout.Infinite;
                //stream.WriteTimeout = System.Threading.Timeout.Infinite;

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public byte[] Read()
        {
            if (stream != null)
            {
                if (stream.CanRead)
                {
                    byte[] byteArray = stream.Read();
                    return byteArray;
                }
                else return null;
            }
            else
                return null;
        }

        public void Write(byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }
    }
}

