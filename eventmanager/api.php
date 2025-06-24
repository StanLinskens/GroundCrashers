<?php
// api.php - Main API endpoint

require_once 'config.php';

class EventAPI {
    private $db;
    
    public function __construct() {
        $this->db = DatabaseConfig::getConnection();
    }
    
    public function handleRequest() {
        $method = $_SERVER['REQUEST_METHOD'];
        $action = $_GET['action'] ?? '';
        
        try {
            switch ($action) {
                case 'get_events':
                    return $this->getEvents();
                    
                case 'add_event':
                    return $this->addEvent();
                    
                case 'register_participant':
                    return $this->registerParticipant();
                    
                case 'remove_participant':
                    return $this->removeParticipant();
                    
                case 'clear_events':
                    return $this->clearAllEvents();
                    
                case 'get_statistics':
                    return $this->getStatistics();
                    
                case 'admin_login':
                    return $this->adminLogin();
                    
                case 'export_csv':
                    return $this->exportCSV();
                    
                default:
                    return $this->error('Invalid action', 400);
            }
        } catch (Exception $e) {
            error_log("API Error: " . $e->getMessage());
            return $this->error('Internal server error', 500);
        }
    }
    
    private function getEvents() {
        $sql = "SELECT 
                    e.*,
                    COUNT(er.id) as participant_count
                FROM events e
                LEFT JOIN event_registrations er ON e.id = er.event_id
                GROUP BY e.id
                ORDER BY e.start_time ASC";
        
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $events = $stmt->fetchAll();
        
        // Get participants for each event
        foreach ($events as &$event) {
            $event['participants'] = $this->getEventParticipants($event['id']);
        }
        
        return $this->success($events);
    }
    
    private function getEventParticipants($eventId) {
        $sql = "SELECT p.*, er.registered_at
                FROM participants p
                JOIN event_registrations er ON p.id = er.participant_id
                WHERE er.event_id = ?
                ORDER BY er.registered_at ASC";
        
        $stmt = $this->db->prepare($sql);
        $stmt->execute([$eventId]);
        return $stmt->fetchAll();
    }
    
    private function addEvent() {
        $data = json_decode(file_get_contents('php://input'), true);
        
        // Validate required fields
        $required = ['name', 'workshop_leader', 'start_time', 'end_time', 'max_participants'];
        foreach ($required as $field) {
            if (empty($data[$field])) {
                return $this->error("Field '$field' is required", 400);
            }
        }
        
        // Validate dates
        if (strtotime($data['start_time']) >= strtotime($data['end_time'])) {
            return $this->error('End time must be after start time', 400);
        }
        
        $sql = "INSERT INTO events (name, description, workshop_leader, start_time, end_time, max_participants, location) 
                VALUES (?, ?, ?, ?, ?, ?, ?)";
        
        $stmt = $this->db->prepare($sql);
        $result = $stmt->execute([
            $data['name'],
            $data['description'] ?? '',
            $data['workshop_leader'],
            $data['start_time'],
            $data['end_time'],
            $data['max_participants'],
            $data['location'] ?? ''
        ]);
        
        if ($result) {
            return $this->success(['id' => $this->db->lastInsertId(), 'message' => 'Event created successfully']);
        } else {
            return $this->error('Failed to create event', 500);
        }
    }
    
    private function registerParticipant() {
        $data = json_decode(file_get_contents('php://input'), true);
        
        // Validate required fields
        $required = ['name', 'email', 'student_number', 'student_program', 'event_id'];
        foreach ($required as $field) {
            if (empty($data[$field])) {
                return $this->error("Field '$field' is required", 400);
            }
        }
        
        $this->db->beginTransaction();
        
        try {
            // Check if event exists and has space
            $eventSql = "SELECT id, name, max_participants, 
                        (SELECT COUNT(*) FROM event_registrations WHERE event_id = ?) as current_participants
                        FROM events WHERE id = ?";
            $eventStmt = $this->db->prepare($eventSql);
            $eventStmt->execute([$data['event_id'], $data['event_id']]);
            $event = $eventStmt->fetch();
            
            if (!$event) {
                throw new Exception('Event not found');
            }
            
            if ($event['current_participants'] >= $event['max_participants']) {
                throw new Exception('Event is full');
            }
            
            // Check participant registration limit (max 2 events)
            $participantCheckSql = "SELECT COUNT(*) as event_count 
                                   FROM event_registrations er 
                                   JOIN participants p ON er.participant_id = p.id 
                                   WHERE p.email = ?";
            $participantStmt = $this->db->prepare($participantCheckSql);
            $participantStmt->execute([$data['email']]);
            $participantCheck = $participantStmt->fetch();
            
            if ($participantCheck['event_count'] >= 2) {
                throw new Exception('You can only register for a maximum of 2 events');
            }
            
            // Check if already registered for this event
            $duplicateCheckSql = "SELECT COUNT(*) as count 
                                 FROM event_registrations er 
                                 JOIN participants p ON er.participant_id = p.id 
                                 WHERE p.email = ? AND er.event_id = ?";
            $duplicateStmt = $this->db->prepare($duplicateCheckSql);
            $duplicateStmt->execute([$data['email'], $data['event_id']]);
            $duplicateCheck = $duplicateStmt->fetch();
            
            if ($duplicateCheck['count'] > 0) {
                throw new Exception('Already registered for this event');
            }
            
            // Insert or update participant
            $participantSql = "INSERT INTO participants (name, email, student_number, student_program) 
                              VALUES (?, ?, ?, ?) 
                              ON DUPLICATE KEY UPDATE 
                              name = VALUES(name), 
                              student_program = VALUES(student_program)";
            $participantStmt = $this->db->prepare($participantSql);
            $participantStmt->execute([
                $data['name'],
                $data['email'],
                $data['student_number'],
                $data['student_program']
            ]);
            
            // Get participant ID
            $participantId = $this->db->lastInsertId();
            if (!$participantId) {
                $getParticipantSql = "SELECT id FROM participants WHERE email = ?";
                $getParticipantStmt = $this->db->prepare($getParticipantSql);
                $getParticipantStmt->execute([$data['email']]);
                $participant = $getParticipantStmt->fetch();
                $participantId = $participant['id'];
            }
            
            // Register for event
            $registrationSql = "INSERT INTO event_registrations (event_id, participant_id) VALUES (?, ?)";
            $registrationStmt = $this->db->prepare($registrationSql);
            $registrationStmt->execute([$data['event_id'], $participantId]);
            
            $this->db->commit();
            return $this->success(['message' => "Successfully registered for '{$event['name']}'!"]);
            
        } catch (Exception $e) {
            $this->db->rollBack();
            return $this->error($e->getMessage(), 400);
        }
    }
    
    private function removeParticipant() {
        $data = json_decode(file_get_contents('php://input'), true);
        
        if (empty($data['event_id']) || empty($data['email'])) {
            return $this->error('Event ID and email are required', 400);
        }
        
        $sql = "DELETE er FROM event_registrations er
                JOIN participants p ON er.participant_id = p.id
                WHERE er.event_id = ? AND p.email = ?";
        
        $stmt = $this->db->prepare($sql);
        $result = $stmt->execute([$data['event_id'], $data['email']]);
        
        if ($result && $stmt->rowCount() > 0) {
            return $this->success(['message' => 'Participant removed successfully']);
        } else {
            return $this->error('Participant not found or already removed', 404);
        }
    }
    
    private function clearAllEvents() {
        // This will cascade delete all registrations
        $sql = "DELETE FROM events";
        $stmt = $this->db->prepare($sql);
        $result = $stmt->execute();
        
        if ($result) {
            return $this->success(['message' => 'All events cleared successfully']);
        } else {
            return $this->error('Failed to clear events', 500);
        }
    }
    
    private function getStatistics() {
        $stats = [];
        
        // Total events
        $stmt = $this->db->query("SELECT COUNT(*) as count FROM events");
        $stats['total_events'] = $stmt->fetch()['count'];
        
        // Total registrations
        $stmt = $this->db->query("SELECT COUNT(*) as count FROM event_registrations");
        $stats['total_registrations'] = $stmt->fetch()['count'];
        
        // Unique participants
        $stmt = $this->db->query("SELECT COUNT(DISTINCT participant_id) as count FROM event_registrations");
        $stats['unique_participants'] = $stmt->fetch()['count'];
        
        // Total capacity
        $stmt = $this->db->query("SELECT SUM(max_participants) as total FROM events");
        $stats['total_capacity'] = $stmt->fetch()['total'] ?? 0;
        
        // Full events
        $stmt = $this->db->query("SELECT COUNT(*) as count FROM events e 
                                 WHERE (SELECT COUNT(*) FROM event_registrations WHERE event_id = e.id) >= e.max_participants");
        $stats['full_events'] = $stmt->fetch()['count'];
        
        // Utilization rate
        $stats['utilization_rate'] = $stats['total_capacity'] > 0 
            ? round(($stats['total_registrations'] / $stats['total_capacity']) * 100) 
            : 0;
        
        return $this->success($stats);
    }
    
    private function adminLogin() {
        $data = json_decode(file_get_contents('php://input'), true);
        
        if (empty($data['password'])) {
            return $this->error('Password is required', 400);
        }
        
        // Simple password check (in production, use proper authentication)
        if ($data['password'] === 'admin123') {
            return $this->success(['message' => 'Login successful', 'role' => 'admin']);
        } else {
            return $this->error('Incorrect password', 401);
        }
    }
    
    private function exportCSV() {
        $events = $this->getEventsForExport();
        
        $csv = "Event Name,Description,Workshop Leader,Start Time,End Time,Location,Max Participants,Registrations,Participant Name,Email,Student Number,Program,Registration Date\n";
        
        foreach ($events as $event) {
            $baseInfo = sprintf('"%s","%s","%s","%s","%s","%s",%d,%d',
                $event['name'],
                $event['description'],
                $event['workshop_leader'],
                $event['start_time'],
                $event['end_time'],
                $event['location'],
                $event['max_participants'],
                count($event['participants'])
            );
            
            if (empty($event['participants'])) {
                $csv .= $baseInfo . ",,,,,\n";
            } else {
                foreach ($event['participants'] as $participant) {
                    $csv .= $baseInfo . sprintf(',"%s","%s",%d,"%s","%s"',
                        $participant['name'],
                        $participant['email'],
                        $participant['student_number'],
                        $participant['student_program'],
                        $participant['registered_at']
                    ) . "\n";
                }
            }
        }
        
        header('Content-Type: text/csv');
        header('Content-Disposition: attachment; filename="events-export.csv"');
        echo $csv;
        exit;
    }
    
    private function getEventsForExport() {
        $sql = "SELECT * FROM events ORDER BY start_time ASC";
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $events = $stmt->fetchAll();
        
        foreach ($events as &$event) {
            $event['participants'] = $this->getEventParticipants($event['id']);
        }
        
        return $events;
    }
    
    private function success($data) {
        return json_encode(['success' => true, 'data' => $data]);
    }
    
    private function error($message, $code = 400) {
        http_response_code($code);
        return json_encode(['success' => false, 'error' => $message]);
    }
}

// Handle the request
$api = new EventAPI();
echo $api->handleRequest();
?>