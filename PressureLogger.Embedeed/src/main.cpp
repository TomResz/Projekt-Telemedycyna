// #include <WiFiManager.h> 
// #include <HTTPClient.h>
// #include <DNSServer.h> 

// String serverName;
// float weight;

// WiFiManager wm;

// void sendPOSTRequest()
// {
//   if (WiFi.status() == WL_CONNECTED)
//   {
//     HTTPClient http;

//     weight = random(100, 1000) / 100.0;

//     http.begin(serverName.c_str()); 
//     http.addHeader("Content-Type", "application/json");

//     String requestBody = "{\"weight\": " + String(weight, 2) + "}";
//     Serial.println(requestBody);

//     int httpResponseCode = http.POST(requestBody);

//     if (httpResponseCode > 0)
//     {
//       String response = http.getString();
//       Serial.println(httpResponseCode);
//     }
//     else
//     {
//       Serial.println("Błąd podczas wysyłania POST: " + String(httpResponseCode));
//     }

//     http.end();
//   }
//   else
//   {
//     Serial.println("Brak połączenia z WiFi");
//   }
// }

// void setup()
// {
//   Serial.begin(115200);

//   WiFiManagerParameter custom_server("server", "Wprowadź URL serwera", "https://192.168.0.89:6001/api/pressure", 64);
  
//   wm.addParameter(&custom_server);

//   if (!wm.autoConnect("ESP32-ConfigAP", "password123")) {
//     Serial.println("Błąd łączenia lub anulowano połączenie");
//     ESP.restart();
//   }

//   serverName = custom_server.getValue();
//   Serial.println("Połączono z WiFi!");
//   Serial.print("Serwer URL: ");
//   Serial.println(serverName);
// }

// void loop()
// {
//   sendPOSTRequest();
//   delay(5000);
// }

#include <Arduino.h>
#include <HX711.h>

#define DOUT_PIN 4   
#define SCK_PIN 5   

// Inicjalizacja HX711
HX711 scale;

void setup() {
    Serial.begin(115200);

    scale.begin(DOUT_PIN, SCK_PIN);
    
    delay(1000);
  
    while (!scale.is_ready())
    {
        Serial.println("HX711 nie jest gotowy, sprawdź połączenia.");
        delay(1000);
    }
    Serial.println("HX711 gotowy.");
  
    scale.set_scale(2280.f);   
    scale.tare();              
    Serial.println("Czujnik skalibrowany.");
}

void loop() {
    // Odczyt wartości z czujnika
    float waga = scale.get_units(10);  // Średnia z 10 odczytów dla dokładności
    Serial.print("Waga: ");
    Serial.print(waga, 2);  // Wyświetla wagę z dokładnością do dwóch miejsc po przecinku
    Serial.println(" g");
    delay(1000);             // Przerwa 1 sekunda
}