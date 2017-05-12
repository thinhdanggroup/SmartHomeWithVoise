#pragma once
#include "Arduino.h"
#include "BH1750FVI.h"
#include "dht.h"

#define DHT11_PIN 5

class sensorBoard
{
  private:
    BH1750FVI LightSensor;
    dht DHT;
    void setupDht()
    {
      int chk = DHT.read11(DHT11_PIN);
      switch (chk)
      {
        case DHTLIB_OK:  
        Serial.print("OK,\t"); 
        break;
        case DHTLIB_ERROR_CHECKSUM: 
        Serial.print("Checksum error,\t"); 
        break;
        case DHTLIB_ERROR_TIMEOUT: 
        Serial.print("Time out error,\t"); 
        break;
        default: 
        Serial.print("Unknown error,\t"); 
        break;
      }
    }
    void setupBH1750()
    {
      LightSensor.begin();
      LightSensor.SetAddress(Device_Address_H);
      LightSensor.SetMode(Continuous_H_resolution_Mode);
    }
  public:
    sensorBoard()
    {
      setupDht();
      setupBH1750();
    }
    void refreshData()
    {
      int chk = DHT.read11(DHT11_PIN);
      if (chk!= DHTLIB_OK)
      {
        Serial.println("Loi dht 11");
      }
    }
    float  getLux() 
    {
      return LightSensor.GetLightIntensity();
    }
    float  getHumidity() 
    {
      return DHT.humidity;
    }
    float getTemperature()
    {
      return DHT.temperature;
    }
    String makeFrame(float lux,float temp,float humid)
    {
      String data_send="";
      
      data_send +=(int)lux; 
      data_send +=' ';  
      
      data_send +=(int)temp; 
      data_send +=' ';
      
      data_send +=(int)humid; 
      data_send +=' ';

      data_send +='\n';
      return data_send;
    }
};


