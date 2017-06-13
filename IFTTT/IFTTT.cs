using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IFTTT
{
    public class IFTTT
    {
        private USB usb;
        public IFTTT()
        {
            usb = USB.Instance;
            Console.WriteLine("Opening stream...");
            usb.Open();
            Console.WriteLine("Stream opened!");
        }
        public byte[] Read()
        {
            return usb.Read();
        }
        public void Write(byte [] instruction)
        {
            usb.Write(instruction);
        }
        public static string MakeRequest(string url, string data)
        {
            string returnMessage;
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Content-Type: application/json");
                returnMessage = client.UploadString(url, data);
            }
            return returnMessage;
        }
        public byte[] CreateCommand(string command)
        {
            string[] bytes = command.Split(' ');
            byte[] packet = new byte[65];
            packet[1] = (byte)Convert.ToInt16(bytes[0]);
            packet[2] = (byte)Convert.ToInt16(bytes[1]);
            packet[3] = (byte)Convert.ToInt16(bytes[2]);
            packet[4] = (byte)Convert.ToInt16(bytes[3]);
            packet[5] = (byte)Convert.ToInt16(bytes[4]);
            return packet;
        }
        public void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Console.WriteLine(e.Topic);
            e.Message.ToList().ForEach(x => Console.Write("{0:X2} ", x));
            byte[] packet = CreateCommand(System.Text.Encoding.ASCII.GetString(e.Message));
            //packet[1] = 0x01;
            Write(packet);
            Console.WriteLine();
            Console.WriteLine(System.Text.Encoding.ASCII.GetString(e.Message));
            Console.WriteLine();
        }
    }
}
