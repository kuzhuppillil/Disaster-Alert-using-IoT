

int pulsePin = 1;            

volatile int BPM;                  
volatile int Signal;                
volatile int IBI = 600;             
volatile boolean Pulse = false;    
volatile boolean QS = false;       

void setup(){

  Serial.begin(9600);           
  interruptSetup();                  
  
}


void loop()
{
  if (QS == true){    
   serialOutputWhenBeatHappens();   
        QS = false;                     
  }
delay(20);
}






