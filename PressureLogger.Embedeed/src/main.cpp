#include <Arduino.h>
#include <HX711.h>
#include <WiFiManager.h>
#include <HTTPClient.h>
#include <DNSServer.h>
#include <WiFiUdp.h>
#include <NTPClient.h>

#define DOUT_PIN 4
#define SCK_PIN 5

HX711 scale;
WiFiManager wm;

String serverName;
String apiKey;

WiFiUDP ntpUDP;
NTPClient timeClient(ntpUDP, "pool.ntp.org", 3600);

float weight = 0.0f;

float weights[20];
String timestamps[20];
int measurementCount = 0;

void sendPOSTRequest(float weights[], String timestamps[], int count)
{

  if (WiFi.status() == WL_CONNECTED)
  {
    HTTPClient http;


    String requestBody = "[";
    for (int i = 0; i < count; i++)
    {
      if(weights[i] < 0.8f){
        weights[i] = 0.0f;
      }

      float valueInKilograms = weights[i] / 1000.0f;
      if (i > 0) 
      {
        requestBody += ","; 
      }
      requestBody += "{\"weight\": " + String(valueInKilograms, 6) + ", \"createdAt\": \"" + timestamps[i] + "\"}";
    }
    requestBody += "]";

    http.begin(serverName.c_str());
    http.addHeader("Content-Type", "application/json");
    http.addHeader("X-Api-Key", apiKey);

    int httpResponseCode = http.POST(requestBody);

    if (httpResponseCode != 201)
    {
      Serial.println("API error: " + String(httpResponseCode));
    }

    http.end();
  }
  else
  {
    Serial.println("Cannot connected to WLAN.");
  }
}

String getFormattedTime()
{
  unsigned long now = timeClient.getEpochTime();
  int millisPart = millis() % 1000;

  int year = 1970 + (now / 31556926);       
  int month = (now / 2629743) % 12 + 1;     
  int day = ((now / 86400L) % 30 + 1) - 18;

  int hour = (now % 86400L) / 3600;
  int minute = (now % 3600) / 60;
  int second = now % 60;

  char timeBuffer[30];
  snprintf(timeBuffer, sizeof(timeBuffer), "%04d-%02d-%02dT%02d:%02d:%02d.%03dZ",
           year, month, day, hour, minute, second, millisPart);

  return String(timeBuffer);
}

void setupHx711()
{
  scale.begin(DOUT_PIN, SCK_PIN);

  delay(1000);

  while (!scale.is_ready())
  {
    Serial.println("HX711 is not ready.");
    delay(1000);
  }
  Serial.println("HX711 is ready");

  // Tutaj wspolczynnik kalibracji trza wpisać
  // krucafiks
  scale.set_scale(645.59f);

  scale.tare();
}

void setupWifi()
{
  WiFiManagerParameter custom_server("server", "Wprowadź URL serwera", "https://192.168.0.89:6001/api/pressure/range", 64);
  WiFiManagerParameter custom_apiKey("api", "Wprowadź klucz APi", "qwerty", 64);

  wm.addParameter(&custom_server);
  wm.addParameter(&custom_apiKey);

  wm.startConfigPortal("PROJEKT_TELEMEDYCYNA_RES_ADAM", "password123");

  serverName = custom_server.getValue();
  apiKey = custom_apiKey.getValue();

  Serial.println("Wifi connected.");

  timeClient.begin();
  timeClient.update();
}

void setup()
{
  Serial.begin(115200);
  setupWifi();
  setupHx711();
  timeClient.begin();
  timeClient.update();
}

void loop()
{
  if (measurementCount < 20) {
    delay(1);
    weights[measurementCount] = scale.get_units(5);
    timestamps[measurementCount] = getFormattedTime();
    measurementCount++; 
  }
  else{
    sendPOSTRequest(weights, timestamps, measurementCount); 

    measurementCount = 0; 
  }
}