#include <WiFi.h>
#include <SPI.h>
#include <MFRC522.h>
#include <LiquidCrystal_I2C.h>

// RFID
#define SS_PIN  5
#define RST_PIN 27
MFRC522 rfid(SS_PIN, RST_PIN);

// LCD
LiquidCrystal_I2C lcd_i2c(0x27, 20, 4);

// WiFi credentials
const char* ssid     = "POCO F5";
const char* password = "Wout007!";

// Server
WiFiServer server(80);

// RFID UID storage
String lastUID = "No tag scanned yet";

void setup() {
  Serial.begin(115200);

  // RFID & LCD init
  SPI.begin();
  rfid.PCD_Init();
  lcd_i2c.init();
  lcd_i2c.backlight();

  // Show startup message
  lcd_i2c.setCursor(0, 0);
  lcd_i2c.print("Tap RFID tag...");

  // WiFi
  WiFi.begin(ssid, password);
  Serial.print("Connecting to WiFi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  Serial.println("\nWiFi connected!");
  Serial.print("IP: ");
  Serial.println(WiFi.localIP());
  server.begin();
}

void loop() {
  // Check RFID
  if (rfid.PICC_IsNewCardPresent() && rfid.PICC_ReadCardSerial()) {
    // Build UID string
    String uidString = "";
    for (int i = 0; i < rfid.uid.size; i++) {
      if (rfid.uid.uidByte[i] < 0x10) uidString += "0";
      uidString += String(rfid.uid.uidByte[i], HEX);
    }
    uidString.toUpperCase(); // make it uppercase for readability
    lastUID = uidString;

    // Show on LCD
    lcd_i2c.clear();
    lcd_i2c.setCursor(0, 0);
    lcd_i2c.print("RFID scanned:");
    lcd_i2c.setCursor(0, 1);
    lcd_i2c.print("UID: ");
    lcd_i2c.print(lastUID);

    Serial.println("New RFID tag scanned:");
    Serial.println(lastUID);

    // Stop communication
    rfid.PICC_HaltA();
    rfid.PCD_StopCrypto1();
  }

  // Handle web request
  WiFiClient client = server.available();
  if (client) {
    Serial.println("New Client Connected");

    while (client.connected()) {
      if (client.available()) {
        String request = client.readStringUntil('\r');
        Serial.println("Request: " + request);

        // Send response
        client.println("HTTP/1.1 200 OK");
        client.println("Content-type:text/plain");
        client.println();
        //client.print("Latest RFID UID: ");
        client.println(lastUID);
        break;
      }
    }

    delay(1);
    client.stop();
    Serial.println("Client Disconnected");
  }
}
