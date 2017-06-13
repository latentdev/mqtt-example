#include <Adafruit_CircuitPlayground.h>

byte packet[64];

void setup() {
  // put your setup code here, to run once:
  CircuitPlayground.begin();
}

void loop() {
  // put your main code here, to run repeatedly:
  Read();
  Update();

  Write();
}

void Update()
{
  packet[0] = 0;
  packet[1] = CircuitPlayground.slideSwitch();
  packet[2] = CircuitPlayground.leftButton();
  packet[3] = CircuitPlayground.rightButton();
  packet[4] = CircuitPlayground.lightSensor();
}
void Write()
{
  RawHID.send(packet,0);
}
void Read()
{
  byte buffer[64];
  int bytesRecieved = RawHID.recv(buffer,0);
  if(bytesRecieved >0)
  {
    Options(buffer);
  }
}

void Options(byte buffer [])
{
  switch(buffer[0])
  {
    case 0x01:
    {
      CircuitPlayground.strip.setPixelColor(0,255,255,255);
      CircuitPlayground.strip.show();
      break;
    }
    case 0x02:
    {
      CircuitPlayground.strip.setPixelColor(buffer[1],buffer[2],buffer[3],buffer[4]);
      CircuitPlayground.strip.show();
      break;
    }
    case 0x03:
    {
      for(int i=0;i<buffer[1];i++)
      {
          CircuitPlayground.strip.setPixelColor(i,buffer[2],buffer[3],buffer[4]);
      }
      CircuitPlayground.strip.show();
      break;
    }
    case 0x04:
    {
      CircuitPlayground.strip.clear();
      CircuitPlayground.strip.show();
      break;
    }
    case 0x05:
    {
      byte temp [64];
      temp[0] = 1;
      temp[1] = CircuitPlayground.temperatureF();
      RawHID.send(temp,0);
    }
    default:
      break;
  }

}

