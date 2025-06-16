#include <WiFi.h>
#include <SPI.h>
#include <MFRC522.h>
#include <NfcAdapter.h>
#include <LiquidCrystal_I2C.h>

// ================================
// GROUNDCRASHERS NFC CARD READER
// ================================
#define SS_PIN 5    // RC522 SDA pin
#define RST_PIN 27  // RC522 RST pin

MFRC522 rfid(SS_PIN, RST_PIN);
NfcAdapter nfc(&rfid);
LiquidCrystal_I2C lcd_i2c(0x27, 20, 4);

WiFiServer server(80);

bool waitingForCreatureWrite = false;
uint32_t pendingCreatureID = 0;
unsigned long lastCardInteraction = 0;

const unsigned long CARD_COOLDOWN   = 1000;   // 1 s between reads
const unsigned long READ_TIMEOUT    = 15000;  // 15 s to present card
const unsigned long DISPLAY_DELAY   = 2000;   // 2 s to show messages

void setup() {
  Serial.begin(115200);
  Serial.println("=== GROUNDCRASHERS CARD READER (AP MODE) ===");

  // NFC init
  SPI.begin();
  rfid.PCD_Init();
  nfc.begin();
  Serial.println("[HARDWARE] NFC reader initialized");

  // LCD init
  lcd_i2c.init();
  lcd_i2c.backlight();
  showBootupSequence();

  // Start as Wi-Fi Access Point
  WiFi.softAP("Groundcrashers_AP", "creature123");
  IPAddress IP = WiFi.softAPIP();
  Serial.print("[NETWORK] AP IP: ");
  Serial.println(IP);
  showDisplayMessage("AP MODE ACTIVE", "IP: " + IP.toString(), 2000);

  // Start HTTP server
  server.begin();
  Serial.println("[SERVER] Listening on port 80");
  showReadyScreen();
}

void loop() {
  handleGameClientRequests();
  handleCreatureCardWrites();
  debugCardPresence();
  delay(50);
}

// --------------------
// HTTP REQUEST HANDLING
// --------------------
void handleGameClientRequests() {
  WiFiClient client = server.available();
  if (!client) return;

  Serial.println("[CLIENT] Connected");
  showDisplayMessage("GAME CONNECTED", "Processing...", 1000);

  String request = client.readStringUntil('\r');
  Serial.print("[REQUEST] ");
  Serial.println(request);
  client.flush();

  if (request.indexOf("GET /write?id=") != -1) {
    handleCreatureBindingRequest(client, request);
  }
  else if (request.indexOf("GET /read") != -1) {
    handleCreatureSummoningRequest(client);
  }
  else {
    Serial.println("[ERROR] Unknown command");
    client.println("HTTP/1.1 404 Not Found");
    client.println("Content-Type: text/plain");
    client.println();
    client.println("GROUNDCRASHERS: Unknown command");
  }

  delay(1);
  client.stop();
  Serial.println("[CLIENT] Disconnected");
}

// --------------------
// WRITE (BIND) HANDLER
// --------------------
void handleCreatureBindingRequest(WiFiClient& client, String& request) {
  int idx = request.indexOf("GET /write?id=") + 14;
  String idStr = request.substring(idx);
  int space = idStr.indexOf(' ');
  if (space > 0) idStr = idStr.substring(0, space);

  uint32_t creatureID = (uint32_t)idStr.toInt();
  if (creatureID > 0) {
    pendingCreatureID = creatureID;
    waitingForCreatureWrite = true;

    Serial.print("[BIND] Prepare to bind ID ");
    Serial.println(creatureID);
    showDisplayMessage("CREATURE BINDING", "Present card to", "bind #" + String(creatureID), "");

    client.println("HTTP/1.1 200 OK");
    client.println("Content-Type: text/plain");
    client.println();
    client.println("READY_TO_BIND:" + String(creatureID));
  }
  else {
    Serial.println("[ERROR] Invalid ID");
    client.println("HTTP/1.1 400 Bad Request");
    client.println("Content-Type: text/plain");
    client.println();
    client.println("INVALID_CREATURE_ID");
    showDisplayMessage("BINDING ERROR", "Invalid Creature ID", 2000);
  }
}

// --------------------
// READ (SUMMON) HANDLER
// --------------------
void handleCreatureSummoningRequest(WiFiClient& client) {
  Serial.println("[SUMMON] Awaiting card...");
  showDisplayMessage("SUMMON CREATURE", "Present your card", "to enter battle!", "");

  String creatureData = waitForCreatureCard(READ_TIMEOUT);

  client.println("HTTP/1.1 200 OK");
  client.println("Content-Type: text/plain");
  client.println();

  if (creatureData.length() > 0) {
    Serial.print("[SUMMON] Creature #");
    Serial.print(creatureData);
    Serial.println(" summoned!");

    client.print(creatureData);
    showDisplayMessage("CREATURE SUMMONED!", "Creature #" + creatureData, "Ready for battle!", "");
    delay(DISPLAY_DELAY);
  }
  else {
    Serial.println("[SUMMON] No card detected");
    client.print("NO_CREATURE");
    showDisplayMessage("SUMMON FAILED", "No card detected", "Try again!", "");
    delay(DISPLAY_DELAY);
  }

  showReadyScreen();
}

// --------------------
// PROCESS PENDING WRITE
// --------------------
void handleCreatureCardWrites() {
  if (!waitingForCreatureWrite) return;

  if (rfid.PICC_IsNewCardPresent() && rfid.PICC_ReadCardSerial()) {
    Serial.println("[BIND] Card detected");
    // <—– FIXED: use 4-string overload here (added an extra "")
    showDisplayMessage("BINDING...", "Writing data...", "", "");

    bool success = bindCreatureToCard(String(pendingCreatureID));

    if (success) {
      Serial.print("[BIND] SUCCESS! #");
      Serial.print(pendingCreatureID);
      Serial.println(" bound.");
      showDisplayMessage("BINDING SUCCESS!", "Creature #" + String(pendingCreatureID), "bound to card!", "Ready for battle!");
    }
    else {
      Serial.print("[BIND] FAILED! #");
      Serial.println(pendingCreatureID);
      showDisplayMessage("BINDING FAILED!", "Could not write", "creature data", "Try another card");
    }

    delay(DISPLAY_DELAY);
    showReadyScreen();

    waitingForCreatureWrite = false;
    pendingCreatureID = 0;

    rfid.PICC_HaltA();
    rfid.PCD_StopCrypto1();
  }
}

// --------------------
// NFC WRITE ROUTINE
// --------------------
bool bindCreatureToCard(const String& newCreatureID) {
  String currentData = "";

  // Step 1: Read existing message
  NfcTag tag = nfc.read();
  if (tag.hasNdefMessage()) {
    NdefMessage oldMessage = tag.getNdefMessage();
    for (int i = 0; i < oldMessage.getRecordCount(); i++) {
      NdefRecord record = oldMessage.getRecord(i);
      if (record.getTnf() == 0x01 && record.getType()[0] == 'T') {
        int payloadLength = record.getPayloadLength();
        const byte* payload = record.getPayload();
        int languageLen = payload[0] & 0x3F;
        for (int j = languageLen + 1; j < payloadLength; j++) {
          currentData += (char)payload[j];
        }
      }
    }
    Serial.print("[CARD] Existing data: ");
    Serial.println(currentData);
  }

  // Step 2: Append new ID
  if (currentData.length() > 0) {
    currentData += ",";
  }
  currentData += newCreatureID;

  // Step 3: Write updated string as a single NDEF record
  char buffer[128];
  currentData.toCharArray(buffer, sizeof(buffer));

  NdefMessage newMessage = NdefMessage();
  newMessage.addTextRecord(buffer);

  bool ok = nfc.write(newMessage);
  if (ok) {
    Serial.print("[CARD] Updated list: ");
    Serial.println(currentData);
  } else {
    Serial.println("[CARD] Write failed");
  }

  return ok;
}




// --------------------
// NFC READ ROUTINE
// --------------------
String waitForCreatureCard(unsigned long timeoutMillis) {
  unsigned long startTime = millis();
  while (millis() - startTime < timeoutMillis) {
    if (rfid.PICC_IsNewCardPresent() && rfid.PICC_ReadCardSerial()) {
      if (millis() - lastCardInteraction < CARD_COOLDOWN) {
        rfid.PICC_HaltA();
        rfid.PCD_StopCrypto1();
        continue;
      }
      lastCardInteraction = millis();

      NfcTag tag = nfc.read();
      rfid.PICC_HaltA();
      rfid.PCD_StopCrypto1();

      if (tag.hasNdefMessage()) {
        NdefMessage message = tag.getNdefMessage();
        for (int i = 0; i < message.getRecordCount(); i++) {
          NdefRecord record = message.getRecord(i);
          if (record.getTnf() == 0x01 && record.getType()[0] == 'T') {
            int payloadLength = record.getPayloadLength();
            const byte* payload = record.getPayload();
            int languageLen = payload[0] & 0x3F;
            String creatureID = "";
            for (int j = languageLen + 1; j < payloadLength; j++) {
              creatureID += (char)payload[j];
            }
            Serial.print("[CARD] Read ID: ");
            Serial.println(creatureID);
            return creatureID;
          }
        }
      }
      Serial.println("[CARD] No data found");
      return "";
    }
    delay(100);
  }
  Serial.println("[CARD] Read timeout");
  return "";
}

// ================================
// LCD DISPLAY FUNCTIONS
// ================================
void showBootupSequence() {
  lcd_i2c.clear();
  lcd_i2c.setCursor(0, 0);
  lcd_i2c.print("GROUNDCRASHERS");
  lcd_i2c.setCursor(0, 1);
  lcd_i2c.print("Card Reader v1.0");
  lcd_i2c.setCursor(0, 2);
  lcd_i2c.print("Initializing...");
  delay(2000);
}

void showReadyScreen() {
  lcd_i2c.clear();
  lcd_i2c.setCursor(0, 0);
  lcd_i2c.print("GROUNDCRASHERS");
  lcd_i2c.setCursor(0, 1);
  lcd_i2c.print("Ready for Battle!");
  lcd_i2c.setCursor(0, 2);
  lcd_i2c.print("Present creature");
  lcd_i2c.setCursor(0, 3);
  lcd_i2c.print("card to fight...");
}

void showDisplayMessage(String line1, String line2, unsigned long displayTime) {
  lcd_i2c.clear();
  lcd_i2c.setCursor(0, 0);
  lcd_i2c.print(line1);
  lcd_i2c.setCursor(0, 1);
  lcd_i2c.print(line2);
  delay(displayTime);
}

void showDisplayMessage(String line1, String line2, String line3, String line4) {
  lcd_i2c.clear();
  lcd_i2c.setCursor(0, 0);
  lcd_i2c.print(line1);
  lcd_i2c.setCursor(0, 1);
  lcd_i2c.print(line2);
  lcd_i2c.setCursor(0, 2);
  lcd_i2c.print(line3);
  lcd_i2c.setCursor(0, 3);
  lcd_i2c.print(line4);
}

// ================================
// DEBUG (OPTIONAL)
// ================================
void debugCardPresence() {
  static unsigned long lastDebugTime = 0;
  if (millis() - lastDebugTime > 5000) {
    if (rfid.PICC_IsNewCardPresent() && rfid.PICC_ReadCardSerial()) {
      Serial.println("[DEBUG] Card detected — UID dump:");
      rfid.PICC_DumpToSerial(&(rfid.uid));
      rfid.PICC_HaltA();
      rfid.PCD_StopCrypto1();
      lastDebugTime = millis();
    }
  }
}
