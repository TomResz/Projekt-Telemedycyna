#include <Arduino.h>
#include <HX711.h>
#include <WiFiManager.h>
#include <PubSubClient.h>
#include <DNSServer.h>
#include <WiFiUdp.h>
#include <NTPClient.h>
#include <time.h>
#define LOG_ALL

#define DOUT_PIN 4
#define SCK_PIN 5
#define MESSAGE_BUFFOR_SIZE 5

const float A = 1.008915, B = -0.0114;

HX711 scale;
WiFiManager wm;

String mqttServer;
String mqttUser;
String mqttPassword;

WiFiUDP ntpUDP;
NTPClient timeClient(ntpUDP, "pool.ntp.org", 3600);

float weight = 0.0f;
float weights[MESSAGE_BUFFOR_SIZE];
String timestamps[MESSAGE_BUFFOR_SIZE];
int measurementCount = 0;

WiFiClient espClient;
PubSubClient mqttClient(espClient);

void setupMQTT()
{
  mqttClient.setServer(mqttServer.c_str(), 1883);

  while (!mqttClient.connected())
  {
    Serial.print("Connecting to MQTT...");
    if (mqttClient.connect("HX711Client"))
    {
      Serial.println("connected");
    }
    else
    {
      Serial.print("failed, rc=");
      Serial.print(mqttClient.state());
      Serial.println(" try again in 5 seconds");
      delay(5000);
    }
  }
}

void sendMQTTMessage(float weights[], String timestamps[], int count)
{
  if (!mqttClient.connected())
  {
    setupMQTT();
  }
  String message = "[";

  for (int i = 0; i < count; i++)
  {
    if (weights[i] < 0.8f)
    {
      weights[i] = 0.0f;
    }

    float valueInKilograms = weights[i] / 1000.0f;
    if (i > 0)
    {
      message += ",";
    }
    message += "{\"w\": " + String(valueInKilograms, 3) + ", \"c\": \"" + timestamps[i] + "\"}";
  }
  message += "]";
  const char *payload = message.c_str();

#ifdef LOG_ALL
  Serial.println("Current buffor size:" + String(strlen(payload)));
  Serial.println(payload);
#endif

  boolean response = mqttClient.publish("/w", payload);

#ifdef LOG_ALL
  if (response)
  {
    Serial.println("Message sent!");
    Serial.println("Message content:" + message);
  }
  else
  {
    Serial.println("Failed to send message");
  }
#endif
}

String getFormattedTime()
{
  struct tm timeinfo;
  if (!getLocalTime(&timeinfo))
  {
    return "1970-01-01T00:00:00.000Z";
  }

  char timeBuffer[30];
  strftime(timeBuffer, 
      sizeof(timeBuffer),
      "%Y-%m-%dT%H:%M:%S",
       &timeinfo);

  int millisPart = millis() % 1000;
  String formattedTime = String(timeBuffer) + "." + String(millisPart) + "Z";
  return formattedTime;
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
  scale.set_scale(645.59f);
  scale.tare();
}

void setupWifi()
{
  WiFiManagerParameter custom_mqttServer("server", "Enter MQTT server URL", "192.168.0.89", 64);

  wm.addParameter(&custom_mqttServer);
  wm.startConfigPortal("PROJEKT_TELEMEDYCYNA_RES_ADAM", "password123");

  mqttServer = custom_mqttServer.getValue();
  mqttClient.setBufferSize(512);

  uint16_t initBufforSize = mqttClient.getBufferSize();

  Serial.println("Current mqtt buffor " + String(initBufforSize));
  Serial.println("Wifi connected.");

  timeClient.begin();
  timeClient.update();
}

void setupLocalTime()
{
  configTime(3600, 3600, "pool.ntp.org");
  delay(2000);
}

void setup()
{
  Serial.begin(115200);
  setupWifi();
  setupHx711();
  setupMQTT();
  setupLocalTime();
}


void loop()
{
  if (measurementCount < MESSAGE_BUFFOR_SIZE){
    delay(1);
    weights[measurementCount] = (scale.get_units(2) - B) / A ;
    timestamps[measurementCount] = getFormattedTime();
    measurementCount++;
  }
  else{
    sendMQTTMessage(weights, timestamps, measurementCount);
    measurementCount = 0;
  }

  mqttClient.loop();
}
