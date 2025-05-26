#include <SPI.h>
#include <MFRC522.h>
#include <LiquidCrystal_I2C.h>


#define SS_PIN  5  // ESP32 pin GPIO5 
#define RST_PIN 27 // ESP32 pin GPIO27 

MFRC522 rfid(SS_PIN, RST_PIN);
LiquidCrystal_I2C lcd_i2c(0x27, 20, 4); // I2C address 0x27, 16 column and 2 rows

void setup() {
  Serial.begin(115200);
  SPI.begin(); // init SPI bus
  rfid.PCD_Init(); // init MFRC522

  lcd_i2c.init(); //innit lcd
  lcd_i2c.backlight(); //start backlight

  Serial.println("Tap an RFID/NFC tag on the RFID-RC522 reader");
}

void loop() {
  if (rfid.PICC_IsNewCardPresent()) { // new tag is available
    if (rfid.PICC_ReadCardSerial()) { // UID is gelezen
      MFRC522::PICC_Type piccType = rfid.PICC_GetType(rfid.uid.sak);
      Serial.print("RFID/NFC Tag Type: ");
      Serial.println(rfid.PICC_GetTypeName(piccType));

      // print UID in Serial Monitor
      Serial.print("UID:");

      String uidString = "";
      for (int i = 0; i < rfid.uid.size; i++) {
        Serial.print(rfid.uid.uidByte[i] < 0x10 ? " 0" : " ");
        Serial.print(rfid.uid.uidByte[i], HEX);

        // bouw string op voor lcd
        if (rfid.uid.uidByte[i] < 0x10) uidString += "0";
        uidString += String(rfid.uid.uidByte[i], HEX);
      }

      Serial.println();

      // UID op LCD tonen
      lcd_i2c.clear();
      lcd_i2c.setCursor(0, 0);
      lcd_i2c.print("RFID scanned:");
      lcd_i2c.setCursor(0, 1);
      lcd_i2c.print(rfid.PICC_GetTypeName(piccType));
      lcd_i2c.setCursor(0, 2);
      lcd_i2c.print("UID:");
      lcd_i2c.setCursor(0, 3);
      lcd_i2c.print(uidString);

      rfid.PICC_HaltA(); // stop tag
      rfid.PCD_StopCrypto1(); // stop encryptie
    }
  }
}
