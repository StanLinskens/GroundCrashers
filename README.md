GroundCrashers

GroundCrashers is a school project that combines a web-based game with RFID hardware integration. The game is developed using HTML, CSS, and JavaScript, while the RFID reader is programmed in C++.
Features

    Web Game: An interactive game accessible through a web browser.

    RFID Integration: Uses an RFID reader to interact with the game, allowing players to scan RFID tags as part of the gameplay.

    Modular Design: The project is organized into separate components for easy maintenance and scalability.

Project Structure

GroundCrashers/
├── Game/
│   └── groundCrashers_game/   # Web game source code
├── Website/                   # Website files
├── data/                      # Data files used by the game
├── rfid-reader/               # RFID reader code
├── rfid-reader.ino            # Arduino sketch for RFID reader
├── .vscode/                   # VS Code configuration
├── .gitignore
└── README.md

Getting Started
Prerequisites

    A web browser to run the game.

    An RFID reader compatible with Arduino.

    Arduino IDE to upload the RFID reader code.

Running the Game

    Clone the repository:

    git clone https://github.com/StanLinskens/GroundCrashers.git

    Navigate to the game directory:

    cd GroundCrashers/Game/groundCrashers_game

    Open index.html in your web browser to start the game.

Setting Up the RFID Reader

    Connect your RFID reader to the Arduino board.

    Open rfid-reader.ino in the Arduino IDE.

    Upload the sketch to your Arduino board.

    Ensure the Arduino is communicating with the game as intended.
