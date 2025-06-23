<?php
// utils.php - Additional utility functions and maintenance scripts

require_once 'config.php';

class EventUtilities {
    private $db;
    
    public function __construct() {
        $this->db = DatabaseConfig::getConnection();
    }
    
    /**
     * Clean up old events (events that ended more than 30 days ago)
     */
    public function cleanupOldEvents() {
        $sql = "DELETE FROM events WHERE end_time < DATE_SUB(NOW(), INTERVAL 30 DAY)";
        $stmt = $this->db->prepare($sql);
        $result = $stmt->execute();
        $deletedCount = $stmt->rowCount();
        
        return [
            'success' => $result,
            'deleted_count' => $deletedCount,
            'message' => "Cleaned up {$deletedCount} old events"
        ];
    }
    
    /**
     * Get events summary report
     */
    public function getEventsSummaryReport() {
        $sql = "SELECT 
                    e.id,
                    e.name,
                    e.workshop_leader,
                    e.start_time,
                    e.end_time,
                    e.max_participants,
                    COUNT(er.id) as registered_count,
                    ROUND((COUNT(er.id) / e.max_participants) * 100, 2) as fill_percentage,
                    CASE 
                        WHEN e.end_time < NOW() THEN 'Completed'
                        WHEN e.start_time > NOW() THEN 'Upcoming'
                        ELSE 'Ongoing'
                    END as status
                FROM events e
                LEFT JOIN event_registrations er ON e.id = er.event_id
                GROUP BY e.id
                ORDER BY e.start_time DESC";
        
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        return $stmt->fetchAll();
    }
    
    /**
     * Get participant activity report
     */
    public function getParticipantActivityReport() {
        $sql = "SELECT 
                    p.name,
                    p.email,
                    p.student_number,
                    p.student_program,
                    COUNT(er.id) as events_registered,
                    GROUP_CONCAT(e.name SEPARATOR ', ') as event_names
                FROM participants p
                LEFT JOIN event_registrations er ON p.id = er.participant_id
                LEFT JOIN events e ON er.event_id = e.id
                GROUP BY p.id
                ORDER BY events_registered DESC, p.name ASC";
        
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        return $stmt->fetchAll();
    }
    
    /**
     * Get workshop leader statistics
     */
    public function getWorkshopLeaderStats() {
        $sql = "SELECT 
                    workshop_leader,
                    COUNT(*) as total_events,
                    SUM(max_participants) as total_capacity,
                    COUNT(DISTINCT er.participant_id) as unique_participants,
                    AVG(
                        (SELECT COUNT(*) FROM event_registrations WHERE event_id = e.id) / e.max_participants * 100
                    ) as avg_fill_rate
                FROM events e
                LEFT JOIN event_registrations er ON e.id = er.event_id
                GROUP BY workshop_leader
                ORDER BY total_events DESC";
        
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        return $stmt->fetchAll();
    }
    
    /**
     * Generate attendance report for a specific event
     */
    public function getEventAttendanceReport($eventId) {
        $sql = "SELECT 
                    e.name as event_name,
                    e.workshop_leader,
                    e.start_time,
                    e.end_time,
                    e.location,
                    p.name as participant_name,
                    p.email,
                    p.student_number,
                    p.student_program,
                    er.registered_at
                FROM events e
                LEFT JOIN event_registrations er ON e.id = er.event_id
                LEFT JOIN participants p ON er.participant_id = p.id
                WHERE e.id = ?
                ORDER BY er.registered_at ASC";
        
        $stmt = $this->db->prepare($sql);
        $stmt->execute([$eventId]);
        return $stmt->fetchAll();
    }
    
    /**
     * Export data to JSON format
     */
    public function exportToJSON() {
        $data = [
            'events' => [],
            'participants' => [],
            'registrations' => [],
            'export_date' => date('Y-m-d H:i:s')
        ];
        
        // Get all events
        $stmt = $this->db->query("SELECT * FROM events ORDER BY start_time");
        $data['events'] = $stmt->fetchAll();
        
        // Get all participants
        $stmt = $this->db->query("SELECT * FROM participants ORDER BY name");
        $data['participants'] = $stmt->fetchAll();
        
        // Get all registrations
        $stmt = $this->db->query("SELECT * FROM event_registrations ORDER BY registered_at");
        $data['registrations'] = $stmt->fetchAll();
        
        return json_encode($data, JSON_PRETTY_PRINT);
    }
    
    /**
     * Validate database integrity
     */
    public function validateDatabaseIntegrity() {
        $issues = [];
        
        // Check for orphaned registrations
        $sql = "SELECT COUNT(*) as count FROM event_registrations er 
                LEFT JOIN events e ON er.event_id = e.id 
                LEFT JOIN participants p ON er.participant_id = p.id 
                WHERE e.id IS NULL OR p.id IS NULL";
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $orphanedRegs = $stmt->fetch()['count'];
        
        if ($orphanedRegs > 0) {
            $issues[] = "Found {$orphanedRegs} orphaned registrations";
        }
        
        // Check for events with invalid dates
        $sql = "SELECT COUNT(*) as count FROM events WHERE start_time >= end_time";
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $invalidDates = $stmt->fetch()['count'];
        
        if ($invalidDates > 0) {
            $issues[] = "Found {$invalidDates} events with invalid dates";
        }
        
        // Check for duplicate student numbers
        $sql = "SELECT student_number, COUNT(*) as count 
                FROM participants 
                GROUP BY student_number 
                HAVING count > 1";
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $duplicates = $stmt->fetchAll();
        
        if (!empty($duplicates)) {
            $issues[] = "Found duplicate student numbers: " . implode(', ', array_column($duplicates, 'student_number'));
        }
        
        return [
            'is_valid' => empty($issues),
            'issues' => $issues
        ];
    }
    
    /**
     * Get system statistics
     */
    public function getSystemStats() {
        $stats = [];
        
        // Database size
        $sql = "SELECT 
                    ROUND(SUM(data_length + index_length) / 1024 / 1024, 2) AS db_size_mb
                FROM information_schema.tables 
                WHERE table_schema = 'event_management'";
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $stats['database_size_mb'] = $stmt->fetch()['db_size_mb'];
        
        // Table counts
        $tables = ['events', 'participants', 'event_registrations', 'admin_users'];
        foreach ($tables as $table) {
            $stmt = $this->db->prepare("SELECT COUNT(*) as count FROM {$table}");
            $stmt->execute();
            $stats["{$table}_count"] = $stmt->fetch()['count'];
        }
        
        // Recent activity (last 7 days)
        $sql = "SELECT COUNT(*) as count FROM events WHERE created_at >= DATE_SUB(NOW(), INTERVAL 7 DAY)";
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $stats['events_last_7_days'] = $stmt->fetch()['count'];
        
        $sql = "SELECT COUNT(*) as count FROM event_registrations WHERE registered_at >= DATE_SUB(NOW(), INTERVAL 7 DAY)";
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $stats['registrations_last_7_days'] = $stmt->fetch()['count'];
        
        return $stats;
    }
}

// Handle utility requests if called directly
if (basename($_SERVER['PHP_SELF']) == 'utils.php') {
    $utils = new EventUtilities();
    $action = $_GET['action'] ?? '';
    
    header('Content-Type: application/json');
    
    try {
        switch ($action) {
            case 'cleanup':
                echo json_encode($utils->cleanupOldEvents());
                break;
                
            case 'summary_report':
                echo json_encode($utils->getEventsSummaryReport());
                break;
                
            case 'participant_report':
                echo json_encode($utils->getParticipantActivityReport());
                break;
                
            case 'leader_stats':
                echo json_encode($utils->getWorkshopLeaderStats());
                break;
                
            case 'attendance_report':
                $eventId = $_GET['event_id'] ?? null;
                if (!$eventId) {
                    throw new Exception('Event ID is required');
                }
                echo json_encode($utils->getEventAttendanceReport($eventId));
                break;
                
            case 'export_json':
                header('Content-Type: application/json');
                header('Content-Disposition: attachment; filename="event_data_export.json"');
                echo $utils->exportToJSON();
                break;
                
            case 'validate':
                echo json_encode($utils->validateDatabaseIntegrity());
                break;
                
            case 'system_stats':
                echo json_encode($utils->getSystemStats());
                break;
                
            default:
                throw new Exception('Invalid action');
        }
    } catch (Exception $e) {
        http_response_code(400);
        echo json_encode(['error' => $e->getMessage()]);
    }
}
?>