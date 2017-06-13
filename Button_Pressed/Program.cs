using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IFTTT;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Button_Pressed
{
    public class IFTTTValues
    {
        public string value1 { get; set; }
        public string value2 { get; set; }
        public string value3 { get; set; }
        public string value4 { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            bool run = true;
            bool sensor = false;
            const string IFTTT_API_KEY = "Q2RC7_-B87aDK59KT3IgR";
            const string IFTTT_URL =
            "https://maker.ifttt.com/trigger/button_pressed/with/key/" + IFTTT_API_KEY;
            const string light_sensor = "https://maker.ifttt.com/trigger/light_sensor/with/key/" + IFTTT_API_KEY;
            const string temp_sensor = "https://maker.ifttt.com/trigger/temp_sensor/with/key/" + IFTTT_API_KEY;

            const string ADAFRUIT_USERNAME = "latentdev";
            const string ADAFRUIT_API_KEY = "87ac964ea0664cb1872921c8a96a41ea";
            const string ADAFRUIT_FEED = "ifttt-feed";

            IFTTT.IFTTT ifttt = new IFTTT.IFTTT();
            MqttClient client = new MqttClient("io.adafruit.com");
            client.MqttMsgPublishReceived += ifttt.client_MqttMsgPublishReceived;
            Console.WriteLine("Listening....");
            byte ret = client.Connect(Guid.NewGuid().ToString(), ADAFRUIT_USERNAME,ADAFRUIT_API_KEY);
            //Connect will return 0 if successful
            if (ret == 0)
            {
                client.Subscribe(new string[] { ADAFRUIT_USERNAME + "/feeds/" + ADAFRUIT_FEED },
                new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                //Console.ReadLine();
            }
            while (run)
            {



                    
                

                byte[] packet = ifttt.Read();
                if (packet != null)
                {
                    if (packet[1] == 0)
                    {
                        IFTTTValues values = new IFTTTValues();
                        values.value1 = "Slide Switch: " + BitConverter.ToBoolean(packet, 2);
                        values.value2 = "Left Button: " + BitConverter.ToBoolean(packet, 3);
                        values.value3 = "Right Button: " + BitConverter.ToBoolean(packet, 4);
                        values.value3 = "Light Sensor: " + BitConverter.ToBoolean(packet, 5);
                        if (BitConverter.ToBoolean(packet, 3) == true || BitConverter.ToBoolean(packet, 4) == true)
                        {
                            string data = "{\"value1\" :\"" + values.value1 + "\"," + "\"value2\" :\"" + values.value2 + "\"," + "\"value3\" :\"" + values.value3 + "\"," + "\"value4\" :\"" + values.value4 + "\"}";

                            Console.WriteLine(IFTTT.IFTTT.MakeRequest(IFTTT_URL, data));
                        }
                        if (packet[5] > 100 && sensor == false)
                        {
                            string data = "";
                            Console.WriteLine(IFTTT.IFTTT.MakeRequest(light_sensor, data));
                            sensor = true;
                        }
                        if (packet[5] < 100)
                        {
                            sensor = false;
                        }
                    }
                    if(packet[1] == 1)
                    {
                        IFTTTValues values = new IFTTTValues();
                        values.value1 = "Temperature: " + BitConverter.ToInt16(packet, 2)+"F";
                        string data = "{\"value1\" :\"" + values.value1 +"\"}";

                        Console.WriteLine(IFTTT.IFTTT.MakeRequest(temp_sensor, data));
                    }
                }
            }
            client.Disconnect();
        }

    }
}
