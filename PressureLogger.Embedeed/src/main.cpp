#include <Arduino.h>
#include <HX711.h>
#include <WiFiManager.h>
#include <HTTPClient.h>
#include <DNSServer.h>
#define DOUT_PIN 4
#define SCK_PIN 5

HX711 scale;
WiFiManager wm;

String serverName;
String apiKey;

float weight = 0.0f;

void sendPOSTRequest(float weight)
{
  if (WiFi.status() == WL_CONNECTED)
  {
    float valueInKilograms = weight / 1000.0f;
    HTTPClient http;

    http.begin(serverName.c_str());
    http.addHeader("Content-Type", "application/json");
    http.addHeader("X-Api-Key", apiKey);

    String requestBody = "{\"weight\": " + String(valueInKilograms, 6) + "}";
    Serial.println(requestBody);

    int httpResponseCode = http.POST(requestBody);

    if (httpResponseCode != 201)
    {
      Serial.println("Błąd podczas wysyłania POST: " + String(httpResponseCode));
    }

    http.end();
  }
  else
  {
    Serial.println("Brak połączenia z WiFi");
  }
}

void setupHx711()
{
  scale.begin(DOUT_PIN, SCK_PIN);

  delay(1000);

  while (!scale.is_ready())
  {
    Serial.println("HX711 nie jest gotowy, sprawdź połączenia.");
    delay(1000);
  }
  Serial.println("HX711 gotowy.");

  // Tutaj wspolczynnik kalibracji trza wpisać
  // krucafiks
  scale.set_scale();


  scale.tare();

  Serial.println("Czujnik skalibrowany.");
}

void setupWifi()
{
  WiFiManagerParameter custom_server("server", "Wprowadź URL serwera", "https://192.168.0.89:6001/api/pressure", 64);
  WiFiManagerParameter custom_apiKey("api-key", "Wprowadź klucz APi", "example-api-key", 32);

  wm.addParameter(&custom_server);
  wm.addParameter(&custom_apiKey);

  wm.startConfigPortal("PROJEKT_TELEMEDYCYNA_RES_ADAM", "password123");

  serverName = custom_server.getValue();
  apiKey = custom_apiKey.getValue();

  Serial.println("Polaczono z wifi");
}

void setup()
{
  Serial.begin(115200);
  setupWifi();
  setupHx711();
}

void loop()
{
  weight = 0.0f;
  weight = scale.get_units(10);
  Serial.print("Weight: ");
  Serial.print(weight, 2);
  Serial.println(" g");
  sendPOSTRequest(weight);
  delay(1000);
}