#include <WiFi.h>
#include <SPI.h>
#include <MFRC522.h>
#include <NfcAdapter.h>  // NDEF / NfcAdapter layer
#include <LiquidCrystal_I2C.h>

// ================================
// GROUNDCRASHERS NFC CARD READER
// ================================
// This system reads/writes creature IDs to NFC cards
// for the Groundcrashers fighting game. Each card
// represents a unique creature that can battle.

// --------------------
// HARDWARE CONFIGURATION
// --------------------
#define SS_PIN 5    // RC522 SDA pin - NFC reader chip select
#define RST_PIN 27  // RC522 RST pin - NFC reader reset

MFRC522 rfid(SS_PIN, RST_PIN);
NfcAdapter nfc(&rfid);                   // NDEF layer for structured data
LiquidCrystal_I2C lcd_i2c(0x27, 20, 4);  // 20x4 character display

// --------------------
// NETWORK CONFIGURATION
// --------------------
const char* ssid = "POCO F5";  
const char* password = "Wout007!";
WiFiServer server(80);

// --------------------
// GAME STATE MANAGEMENT
// --------------------
bool waitingForCreatureWrite = false;   // Are we waiting to write a creature ID?
uint32_t pendingCreatureID = 0;         // Which creature ID to write next
unsigned long lastCardInteraction = 0;  // Prevent spam reading

// --------------------
// GAME CONSTANTS
// --------------------
const unsigned long CARD_COOLDOWN = 1000;  // 1 second between card reads
const unsigned long READ_TIMEOUT = 15000;  // 15 seconds to present card
const unsigned long DISPLAY_DELAY = 2000;  // How long to show results

void setup() {
  Serial.begin(115200);
  Serial.println("=== GROUNDCRASHERS CARD READER STARTING ===");

  // Initialize NFC hardware
  SPI.begin();
  rfid.PCD_Init();
  nfc.begin();

  Serial.println("[HARDWARE] NFC reader initialized");

  // Initialize battle arena display
  lcd_i2c.init();
  lcd_i2c.backlight();
  showBootupSequence();

  // Connect to the game network
  connectToGameNetwork();

  // Start the creature card server
  server.begin();
  Serial.println("[SERVER] Creature card server started on port 80");
  Serial.println("[SERVER] Endpoints:");
  Serial.println("  /read - Summon creature from card");
  Serial.println("  /write?id=XXX - Bind creature to card");

  showReadyScreen();
}

void loop() {
  // Handle game client connections (Unity game requests)
  handleGameClientRequests();

  // Process pending creature card writes
  handleCreatureCardWrites();

  // Optional: Show card info for debugging
  debugCardPresence();

  delay(50);  // Small delay to prevent overwhelming the NFC reader
}

// ================================
// GAME CLIENT REQUEST HANDLING
// ================================
void handleGameClientRequests() {
  WiFiClient client = server.available();
  if (!client) return;

  Serial.println("[CLIENT] Game client connected");
  showDisplayMessage("GAME CONNECTED", "Processing...", 1000);

  String request = client.readStringUntil('\r');
  Serial.print("[REQUEST] ");
  Serial.println(request);
  client.flush();

  // --------- CREATURE BINDING (WRITE) ----------
  if (request.indexOf("GET /write?id=") != -1) {
    handleCreatureBindingRequest(client, request);
  }
  // --------- CREATURE SUMMONING (READ) ----------
  else if (request.indexOf("GET /read") != -1) {
    handleCreatureSummoningRequest(client);
  }
  // --------- UNKNOWN COMMAND ----------
  else {
    Serial.println("[ERROR] Unknown game command received");
    client.println("HTTP/1.1 404 Not Found");
    client.println("Content-Type: text/plain");
    client.println();
    client.println("GROUNDCRASHERS: Unknown command");
  }

  delay(1);
  client.stop();
  Serial.println("[CLIENT] Game client disconnected");
}

// ================================
// CREATURE BINDING (WRITE OPERATIONS)
// ================================
void handleCreatureBindingRequest(WiFiClient& client, String& request) {
  // Extract creature ID from request
  int idx = request.indexOf("GET /write?id=") + 15;
  String idStr = request.substring(idx);
  int space = idStr.indexOf(' ');
  if (space > 0) idStr = idStr.substring(0, space);

  uint32_t creatureID = (uint32_t)idStr.toInt();

  if (creatureID > 0) {
    pendingCreatureID = creatureID;
    waitingForCreatureWrite = true;

    Serial.print("[BIND] Preparing to bind Creature ID ");
    Serial.print(creatureID);
    Serial.println(" to card");

    showDisplayMessage("CREATURE BINDING", "Present card to", "bind Creature #" + String(creatureID), "");

    // Respond to game immediately
    client.println("HTTP/1.1 200 OK");
    client.println("Content-Type: text/plain");
    client.println();
    client.println("READY_TO_BIND:" + String(creatureID));
  } else {
    Serial.println("[ERROR] Invalid creature ID for binding");
    client.println("HTTP/1.1 400 Bad Request");
    client.println("Content-Type: text/plain");
    client.println();
    client.println("INVALID_CREATURE_ID");

    showDisplayMessage("BINDING ERROR", "Invalid Creature ID", 2000);
  }
}

// ================================
// CREATURE SUMMONING (READ OPERATIONS)
// ================================
void handleCreatureSummoningRequest(WiFiClient& client) {
  Serial.println("[SUMMON] Waiting for creature card...");
  showDisplayMessage("SUMMON CREATURE", "Present your card", "to enter battle!", "");

  String creatureData = waitForCreatureCard(READ_TIMEOUT);

  client.println("HTTP/1.1 200 OK");
  client.println("Content-Type: text/plain");
  client.println();

  if (creatureData.length() > 0) {
    Serial.print("[SUMMON] Creature #");
    Serial.print(creatureData);
    Serial.println(" summoned for battle!");

    client.print(creatureData);  // Send creature ID to game

    showDisplayMessage("CREATURE SUMMONED!", "Creature #" + creatureData, "Ready for battle!", "");
    delay(DISPLAY_DELAY);
  } else {
    Serial.println("[SUMMON] No creature card presented - summoning failed");
    client.print("NO_CREATURE");

    showDisplayMessage("SUMMON FAILED", "No card detected", "Try again!", "");
    delay(DISPLAY_DELAY);
  }

  showReadyScreen();
}

// ================================
// CREATURE CARD WRITE PROCESSING
// ================================
void handleCreatureCardWrites() {
  if (!waitingForCreatureWrite) return;

  // Check if a card is presented for binding
  if (rfid.PICC_IsNewCardPresent() && rfid.PICC_ReadCardSerial()) {
    Serial.println("[BIND] Card detected for creature binding");

    showDisplayMessage("BINDING...", "Writing creature", "data to card...", "");

    bool bindingSuccess = bindCreatureToCard(String(pendingCreatureID));

    if (bindingSuccess) {
      Serial.print("[BIND] SUCCESS! Creature #");
      Serial.print(pendingCreatureID);
      Serial.println(" bound to card");

      showDisplayMessage("BINDING SUCCESS!", "Creature #" + String(pendingCreatureID), "bound to card!", "Ready for battle!");
    } else {
      Serial.print("[BIND] FAILED! Could not bind Creature #");
      Serial.println(pendingCreatureID);

      showDisplayMessage("BINDING FAILED!", "Could not write", "creature data", "Try another card");
    }

    delay(DISPLAY_DELAY);
    showReadyScreen();

    // Reset binding state
    waitingForCreatureWrite = false;
    pendingCreatureID = 0;

    rfid.PICC_HaltA();
    rfid.PCD_StopCrypto1();
  }
}

// ================================
// CARD DATA OPERATIONS
// ================================
bool bindCreatureToCard(const String& creatureID) {
  // Convert creature ID to card data format
  char creatureBuffer[32];
  creatureID.toCharArray(creatureBuffer, sizeof(creatureBuffer));

  // Create NDEF message with creature data
  NdefMessage message = NdefMessage();
  message.addTextRecord(creatureBuffer);

  // Write creature data to card
  bool success = nfc.write(message);

  if (success) {
    Serial.print("[CARD] Creature data written: ");
    Serial.println(creatureID);
  } else {
    Serial.println("[CARD] Failed to write creature data");
  }

  return success;
}

String waitForCreatureCard(unsigned long timeoutMillis) {
  unsigned long startTime = millis();

  while (millis() - startTime < timeoutMillis) {
    if (rfid.PICC_IsNewCardPresent() && rfid.PICC_ReadCardSerial()) {
      Serial.println("[CARD] Creature card detected");

      // Prevent rapid re-reading of same card
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

          // Look for text records (creature data)
          if (record.getTnf() == 0x01 && record.getType()[0] == 'T') {
            int payloadLength = record.getPayloadLength();
            const byte* payload = record.getPayload();

            // Extract creature ID from text record
            int languageLength = payload[0] & 0x3F;
            String creatureID = "";
            for (int i = languageLength + 1; i < payloadLength; i++) {
              creatureID += (char)payload[i];
            }

            Serial.print("[CARD] Creature ID read: ");
            Serial.println(creatureID);
            return creatureID;
          }
        }
      }

      Serial.println("[CARD] No creature data found on card");
      return "";
    }

    delay(100);  // Small delay in card detection loop
  }

  Serial.println("[CARD] Timeout waiting for creature card");
  return "";
}

// ================================
// DISPLAY MANAGEMENT
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
// NETWORK SETUP
// ================================
void connectToGameNetwork() {
  Serial.println("[NETWORK] Connecting to game network...");
  showDisplayMessage("CONNECTING...", "Joining game network", 0);

  WiFi.begin(ssid, password);
  int attempts = 0;

  while (WiFi.status() != WL_CONNECTED && attempts < 40) {
    delay(500);
    Serial.print(".");
    attempts++;
  }

  if (WiFi.status() == WL_CONNECTED) {
    Serial.println();
    Serial.println("[NETWORK] Connected to game network!");
    Serial.print("[NETWORK] Card reader IP: ");
    Serial.println(WiFi.localIP());

    showDisplayMessage("NETWORK READY", "IP: " + WiFi.localIP().toString(), 2000);
  } else {
    Serial.println();
    Serial.println("[NETWORK] Failed to connect to game network!");
    showDisplayMessage("NETWORK ERROR", "Connection failed", "Check settings", "");
    delay(3000);
  }
}

// ================================
// DEBUG FUNCTIONS
// ================================
void debugCardPresence() {
  // Optional: Log when cards are detected (for debugging)
  static unsigned long lastDebugTime = 0;
  if (millis() - lastDebugTime > 5000) {  // Every 5 seconds
    if (rfid.PICC_IsNewCardPresent() && rfid.PICC_ReadCardSerial()) {
      Serial.println("[DEBUG] Card present - dumping info:");
      rfid.PICC_DumpToSerial(&(rfid.uid));
      rfid.PICC_HaltA();
      rfid.PCD_StopCrypto1();
      lastDebugTime = millis();
    }
  }
}