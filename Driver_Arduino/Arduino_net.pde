// Arduino_Net code
// This code is used with the Arduino_Net .Net class
//
// V 1.2.0
//
// www.gyps.nl/arduino

#include <Servo.h>
#include <AFMotor.h>

const int dd = 14;                      // number of digital ports
const int aa = 6;                       // number of analog ports
const int ss = 2;                       // number of servos

int D[dd];                              // define digital enables
int R[dd];                              // define digital directions
int T[dd];                              // define digital triggers
int V[dd];                              // define digital values
int A[aa];                              // define analog enables
int S[aa];                              // define analog triggers
int W[aa];                              // define analog values

int cp = 0;  			        // char pointer
boolean cfound = false;		        // complete command found
byte stringIn[200];                     // commandbuffer
long previousMillis = 0;                // intervalcheck for watchdog
long interval = 1000;                   // interval between watchdogs

Servo NServo[ss];                       // define servo objects
AF_DCMotor NMotor1(1, MOTOR12_1KHZ);    // define motor1 object
AF_DCMotor NMotor2(2, MOTOR12_1KHZ);    // define motor2 object
AF_DCMotor NMotor3(3, MOTOR12_1KHZ);    // define motor3 object
AF_DCMotor NMotor4(4, MOTOR12_1KHZ);    // define motor4 object

#define SOC 40
#define EOC 41

void setup() {
  for (int i=0; i < dd; i++) {
    D[i]=0;		                // enable digital port (default to 0)
    R[i]=0;		                // direction of digital port (0=output, 1=input) (default to 0)
    T[i]=0;		                // enable digital trigger (default to 0)
    V[i]=-1;		                // value of digital port (default to -1 for first trigger)
  }
  for (int i=0; i < aa; i++) {
    A[i]=0;		                // enable analog port (default to 0)
    S[i]=0;		                // enable analog trigger (default to 0)
    W[i]=-1;		                // value of analog port (default to -1 for first trigger)
  }
  Serial.flush();
  Serial.begin(9600);
  delay(500);
  Serial.print("(H)");
}

void loop() {
  int sa;
  byte bt;
  sa = Serial.available();	        // count serial buffer bytes
  if (sa > 0) {			        // if buffer not empty process buffer content
    for (int i=0; i < sa; i++){
      bt = Serial.read();		// read one byte from the serial buffer
      stringIn[cp] = bt;
      cp++;
      if (bt == EOC) {		        // check for last command char )
        cfound = true;
        break;         		        // end for-loop
      }
    } 
  }

  if (cfound) {
    if (int(stringIn[0]) == SOC) {	//check if first char of command is (
      exCommand();
    }
    cleanstring();
    cfound = false;
  }

  for (int i=0; i<dd; i++) {           // check all digital ports
    if ((D[i] == 1) && (R[i] == 1)) {  // is port enabled and input?
      int dr = digitalRead(i);
      if (dr != V[i]) {                // value changed?
        V[i] = dr;                     // store value
        if (T[i] == 1) {               // enable trigger?
          Serial.print("(D");
          Serial.print(byte(i));
          Serial.print(byte(V[i]));
          Serial.print(")");
        }
      }
    }
  }
  for (int i=0; i<aa; i++) {            // check analog values
    if (A[i] == 1) {
      int ar = analogRead(i);
      if (ar != W[i]) {                 // value changed?
        if (S[i] > 0) {                 // analog trigger set?
          if (((ar - W[i]) > S[i]) || ((W[i] - ar) > S[i])) {
            Serial.print("(A");
            Serial.print(byte(i));
            Serial.print(highByte(ar));
            Serial.print(lowByte(ar));
            Serial.print(")");
          }
        }
        W[i] = ar;                      // store value
      }
    }
  }

  if (millis() - previousMillis > interval) {
    previousMillis = millis();   
    Serial.print("(H)");                // send watchdog trigger
  }

}

void cleanstring(void) {
  for (int i=0; i<=200; i++) {
    stringIn[i] = 0;    	        // null out string
    cp = 0;
  }
}

void exCommand(void) {
  char c = stringIn[1];                // command type
  int n = int(stringIn[2]);            // portnumber
  int v = int(stringIn[3]);            // value
  
  switch (c) {
    case 'D':				// enable digital pin
      if (v == 1) {
        D[n] = 1;
      } 
      else {
        D[n] = 0;
      }
      break;
    case 'A':				// enable analog pin
      if (v == 1) {
        A[n] = 1;
      } 
      else {
        A[n] = 0;
      }
      break;
    case 'Q':
      if (v == 1) {
        D[n] = 0;                       // disable digital pin when used for servo
        NServo[n-9].attach(n);          // servos start at pin 9
      }
      else {
        NServo[n-9].detach();           // servos start at pin 9
      }
      break;
    case 'R':				// set direction of digital pin
      switch (v) {
      case 0:			        // input
        R[n] = 0;
        pinMode(n, INPUT);
      case 1:				// output digital
        R[n] = 1;
        pinMode(n, OUTPUT);
        break;
      case 2:				// output analog
        R[n] = 2;
        pinMode(n, OUTPUT);
        break;
      }
      break;
    case 'T':				// set trigger on digital pin
      if (v == 1) {
        T[n] = 1;
      } 
      else {
        T[n] = 0;
      }
      break;
    case 'S':				// set trigger on analog pin
      S[n] = v;
      break;
    case 'V':				// get value of digital pin
      V[n] = digitalRead(n);
      Serial.print("(D");
      Serial.print(byte(n));
      Serial.print(byte(V[n]));
      Serial.print(")");
      break;
    case 'W':				// get value of analog pin
      W[n] = analogRead(n);
      Serial.print("(A");
      Serial.print(byte(n));
      Serial.print(highByte(W[n]));
      Serial.print(lowByte(W[n]));
      Serial.print(")");
      break;
    case 'P':                            // write value to digital port
      switch ( R[n]) {
        case 1:                          // digital output
          digitalWrite(n, v);
          break;
        case 2:                          // PWM output
          analogWrite(n, v);
          break;
      }
      break; 
    case 'O':
      NServo[n-9].write(v);              // write value to servo
      break;
    case 'F':
      switch (n) {
        case 1:
          NMotor1.setSpeed(v);
          break;
        case 2:
          NMotor2.setSpeed(v);
          break;
        case 3:
          NMotor3.setSpeed(v);
          break;
        case 4:
          NMotor4.setSpeed(v);
          break;
      }
      break;
    case 'M':
      switch (n) {
        case 1:
          NMotor1.run(v);
          break;
        case 2:
          NMotor2.run(v);
          break;
        case 3:
          NMotor3.run(v);
          break;
        case 4:
          NMotor4.run(v);
          break;
      }
      break;
  }
}







