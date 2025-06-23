<?php
// config.php - Database configuration

class DatabaseConfig {
    private static $host = 'localhost';
    private static $dbname = 'event_management';
    private static $username = 'root';  // Change this to your database username
    private static $password = '';      // Change this to your database password
    private static $charset = 'utf8mb4';
    
    public static function getConnection() {
        try {
            $dsn = "mysql:host=" . self::$host . ";dbname=" . self::$dbname . ";charset=" . self::$charset;
            $options = [
                PDO::ATTR_ERRMODE            => PDO::ERRMODE_EXCEPTION,
                PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
                PDO::ATTR_EMULATE_PREPARES   => false,
            ];
            
            return new PDO($dsn, self::$username, self::$password, $options);
        } catch (PDOException $e) {
            error_log("Database connection failed: " . $e->getMessage());
            throw new Exception("Database connection failed");
        }
    }
}

// Enable CORS for API requests
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type, Authorization');

// Handle preflight requests
if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    exit(0);
}
?>