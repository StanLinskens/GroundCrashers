#include <WiFi.h>
#include <SPI.h>
#include <MFRC522.h>
#include <LiquidCrystal_I2C.h>

// --------------------
// HARDWARE PINS
// --------------------
#define SS_PIN  5    // MFRC522 SDA pin
#define RST_PIN 27   // MFRC522 RST pin

MFRC522 rfid(SS_PIN, RST_PIN);
LiquidCrystal_I2C lcd_i2c(0x27, 20, 4);

// --------------------
// WIFI CREDENTIALS
// --------------------
const char* ssid     = "POCO F5";
const char* password = "Wout007!";

WiFiServer server(80);

// --------------------
// GLOBAL STATE
// --------------------
bool waitingForWrite = false;
uint32_t pendingWriteID = 0;

// The card sector & block we’ll use. 
// MIFARE Classic sectors are 0–15; each sector has 4 blocks of 16 bytes.
// We must authenticate on a sector trailer (e.g. last block of that sector). 
// Here we'll store data in SECTOR 1, BLOCK 0 (which is absolute block #4).
// Remember: block numbering is linear: sector 0 = blocks 0–3, sector 1 = blocks 4–7, etc.
const byte targetSector = 1;
const byte targetBlock  = 4;  

// Default key for MIFARE Classic (factory default). 
// Change if your cards/blocks use a different key.
MFRC522::MIFARE_Key keyA = {
  {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF}
};

void setup() {
  Serial.begin(115200);
  SPI.begin();
  rfid.PCD_Init();

  // Initialize the LCD and show a startup message
  lcd_i2c.init();
  lcd_i2c.backlight();
  lcd_i2c.clear();
  lcd_i2c.setCursor(0, 0);
  lcd_i2c.print("ESP32 RFID Ready");

  // Connect to WiFi
  WiFi.begin(ssid, password);
  Serial.print("Connecting to WiFi");
  int wifiTimeout = 0;
  while (WiFi.status() != WL_CONNECTED && wifiTimeout < 40) {
    delay(500);
    Serial.print(".");
    wifiTimeout++;
  }
  if (WiFi.status() == WL_CONNECTED) {
    Serial.println("\nWiFi connected!");
    Serial.print("IP address: ");
    Serial.println(WiFi.localIP());

    lcd_i2c.clear();
    lcd_i2c.setCursor(0, 0);
    lcd_i2c.print("WiFi connected");
    lcd_i2c.setCursor(0, 1);
    lcd_i2c.print(WiFi.localIP().toString());
  } else {
    Serial.println("\nFailed to connect to WiFi");
    lcd_i2c.clear();
    lcd_i2c.setCursor(0, 0);
    lcd_i2c.print("WiFi FAIL");
  }

  server.begin();
}

void loop() {
  // Check for HTTP clients
  WiFiClient client = server.available();
  if (client) {
    Serial.println("Client connected");
    String request = client.readStringUntil('\r');
    Serial.print("Request: ");
    Serial.println(request);
    client.flush();

    // Very simple parsing: GET /write?id=NNN
    if (request.indexOf("GET /write?id=") != -1) {
      int idx = request.indexOf("GET /write?id=") + 15;
      String idStr = request.substring(idx);
      int amp = idStr.indexOf(' ');
      if (amp > 0) idStr = idStr.substring(0, amp);
      uint32_t newID = (uint32_t) idStr.toInt();
      if (newID > 0) {
        pendingWriteID = newID;
        waitingForWrite = true;
        Serial.print("Will write ID: ");
        Serial.println(pendingWriteID);
        // Respond immediately but actual write happens when card is tapped
        client.println("HTTP/1.1 200 OK");
        client.println("Content-Type: text/plain");
        client.println();
        client.println("OK: Ready to write ID " + String(pendingWriteID));
      } else {
        client.println("HTTP/1.1 400 Bad Request");
        client.println("Content-Type: text/plain");
        client.println();
        client.println("Invalid ID");
      }
    }
    // GET /read
    else if (request.indexOf("GET /read") != -1) {
  // Don’t send “Reading...” here.
  // First, wait for the card up to 10 seconds.
  Serial.println("Waiting for card to read ID...");
  uint32_t readID = waitForCardAndReadID(10000); // 10 second timeout

  // Now send the HTTP response headers:
  client.println("HTTP/1.1 200 OK");
  client.println("Content-Type: text/plain");
  client.println();

  if (readID != UINT32_MAX) {
    // Send only the raw number, no extra prefix
    client.print(readID);
    Serial.print("Read ID: ");
    Serial.println(readID);
  } else {
    client.print("ERROR: Timeout or no card");
    Serial.println("Read timeout/no card");
  }
}

    // Unrecognized endpoint
    else {
      client.println("HTTP/1.1 404 Not Found");
      client.println("Content-Type: text/plain");
      client.println();
      client.println("Unknown endpoint");
    }

    delay(1);
    client.stop();
    Serial.println("Client disconnected");
  }

  // If we’re waiting to write an ID, check for card tap
  if (waitingForWrite) {
    if (rfid.PICC_IsNewCardPresent() && rfid.PICC_ReadCardSerial()) {
      // Write the 4‐byte ID into the card
      bool ok = writeIDToCard(pendingWriteID);
      if (ok) {
        Serial.print("Successfully wrote ID ");
        Serial.println(pendingWriteID);
        lcd_i2c.clear();
        lcd_i2c.setCursor(0, 0);
        lcd_i2c.print("Wrote ID:");
        lcd_i2c.setCursor(0, 1);
        lcd_i2c.print(pendingWriteID);
      } else {
        Serial.println("Failed to write ID");
        lcd_i2c.clear();
        lcd_i2c.setCursor(0, 0);
        lcd_i2c.print("Write FAILED");
      }
      delay(1500);
      lcd_i2c.clear();
      lcd_i2c.setCursor(0, 0);
      lcd_i2c.print("Tap RFID card...");
      waitingForWrite = false;
      pendingWriteID = 0;
      rfid.PICC_HaltA();
      rfid.PCD_StopCrypto1();
    }
  }
}

//---------------------------------------------------------------------------------
// Helper: Authenticate + write a 4‐byte integer into BLOCK “targetBlock”
//---------------------------------------------------------------------------------
bool writeIDToCard(uint32_t creatureID) {
  // Authenticate for block
  MFRC522::StatusCode status;
  status = rfid.PCD_Authenticate(
    MFRC522::PICC_CMD_MF_AUTH_KEY_A,
    targetBlock,
    &keyA,
    &(rfid.uid)
  );
  if (status != MFRC522::STATUS_OK) {
    Serial.print(F("Auth failed: "));
    Serial.println(rfid.GetStatusCodeName(status));
    return false;
  }

  // Prepare a 16‐byte buffer; first 4 bytes is our ID in big‐endian
  byte buffer[16];
  buffer[0] = (byte)((creatureID >> 24) & 0xFF);
  buffer[1] = (byte)((creatureID >> 16) & 0xFF);
  buffer[2] = (byte)((creatureID >> 8) & 0xFF);
  buffer[3] = (byte)(creatureID & 0xFF);
  // Zero out the rest
  for (int i = 4; i < 16; i++) buffer[i] = 0x00;

  // Write the block
  status = rfid.MIFARE_Write(targetBlock, buffer, 16);
  if (status != MFRC522::STATUS_OK) {
    Serial.print(F("MIFARE_Write failed: "));
    Serial.println(rfid.GetStatusCodeName(status));
    return false;
  }
  return true;
}

//---------------------------------------------------------------------------------
// Helper: Wait until a card is tapped (or timeout) and then read 4‐byte int
// from BLOCK “targetBlock”. Returns UINT32_MAX on failure/timeout.
//---------------------------------------------------------------------------------
uint32_t waitForCardAndReadID(unsigned long timeoutMillis) {
  unsigned long start = millis();
  while (millis() - start < timeoutMillis) {
    if (rfid.PICC_IsNewCardPresent() && rfid.PICC_ReadCardSerial()) {
      MFRC522::StatusCode status;
      status = rfid.PCD_Authenticate(
        MFRC522::PICC_CMD_MF_AUTH_KEY_A,
        targetBlock,
        &keyA,
        &(rfid.uid)
      );
      if (status != MFRC522::STATUS_OK) {
        Serial.print(F("Auth failed: "));
        Serial.println(rfid.GetStatusCodeName(status));
        rfid.PICC_HaltA();
        rfid.PCD_StopCrypto1();
        return UINT32_MAX;
      }

      byte blockBuffer[18];
      byte size = sizeof(blockBuffer);
      status = rfid.MIFARE_Read(targetBlock, blockBuffer, &size);
      if (status != MFRC522::STATUS_OK) {
        Serial.print(F("MIFARE_Read failed: "));
        Serial.println(rfid.GetStatusCodeName(status));
        rfid.PICC_HaltA();
        rfid.PCD_StopCrypto1();
        return UINT32_MAX;
      }

      // Extract the first 4 bytes into a uint32
      uint32_t readValue = 0;
      readValue |= ((uint32_t)blockBuffer[0] << 24);
      readValue |= ((uint32_t)blockBuffer[1] << 16);
      readValue |= ((uint32_t)blockBuffer[2] << 8);
      readValue |= ((uint32_t)blockBuffer[3]);

      rfid.PICC_HaltA();
      rfid.PCD_StopCrypto1();
      return readValue;
    }
  }
  // timeout
  return UINT32_MAX;
}
