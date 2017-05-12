#include <Wire.h>  
#include <math.h>
#include <stdlib.h>
#include "TimerOne.h"
#include "SensorController.cpp"

#define SLAVE_ADDRESS 0x40   // Define the i2c address
#define TIME_COUNT 50


String data_send = ""; //chứa nội dung dữ liệu gửi cho board trung tâm 

float lux=0; // biến lưu giá trị trung bình ánh sáng sau 5s
float temp=0; // biến lưu giá trị trung bình nhiệt độ sau 5s
float humid=0;
//////
int numEvent=0; // biến tạm hỗ trợ đếm thời gian
int fSendData=0;// cờ gửi dữ liệu
int fIsr = 0;// cờ có interupt
char sendFrame[19];



sensorBoard* psensor = new sensorBoard();
sensorBoard& sensor  = *psensor;
void setup()
{
  Serial.begin(9600);
  
  Wire.begin(SLAVE_ADDRESS); 
  Timer1.initialize(100000); // khởi tạo timer 1 đến 1 giây
  Timer1.attachInterrupt(timerIsr, 100000); // khai báo ngắt timer 1
}

void loop()
{
 // READ DATA
    Wire.onRequest(sendData);
    if (fSendData==1) {
      fSendData=0;
      lux=lux/TIME_COUNT;
      temp=temp/TIME_COUNT;
      humid = humid/TIME_COUNT;
      data_send = sensor.makeFrame(lux,temp,humid);
      Serial.println(data_send);
      numEvent=0;
      lux=0;
      temp=0;
    }
    if (fIsr ==1) {
      fIsr =0;
      //sensor.getLux();  
      sensor.refreshData();    
      lux=lux + sensor.getLux();
      temp=temp + sensor.getTemperature();
      humid=humid + sensor.getHumidity();
      
      numEvent=(numEvent+1)%TIME_COUNT;
      if (numEvent == 0) fSendData=1;      
    }
    //delay(50);
}
void sendData()
{
  data_send.toCharArray(sendFrame,17);
  Serial.print("Frame : ");
  Serial.println(sendFrame);
  Wire.write(sendFrame); 
}
void timerIsr()
{
    fIsr=1;
    //digitalWrite( 13, digitalRead( 13 ) ^ 1 );
}
