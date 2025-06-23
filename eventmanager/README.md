# Event Management System - Database Setup Guide

## Prerequisites

- XAMPP, WAMP, or LAMP server
- PHP 7.4 or higher
- MySQL 5.7 or higher
- phpMyAdmin (usually included with XAMPP/WAMP)

## Installation Steps

### 1. Setup Your Local Server

1. Download and install XAMPP from [https://www.apachefriends.org/](https://www.apachefriends.org/)
2. Start Apache and MySQL services from XAMPP Control Panel

### 2. Create Database

#### Option A: Using phpMyAdmin (Recommended)

1. Open your browser and go to `http://localhost/phpmyadmin`
2. Click "New" to create a new database
3. Name it `event_management`
4. Click "Create"
5. Select your new database
6. Go to "SQL" tab
7. Copy and paste the entire SQL schema from the "Database Schema" file
8. Click "Go" to execute

#### Option B: Using MySQL Command Line

```bash
mysql -u root -p
```

Then run the SQL schema commands.

### 3. File Structure

Create the following file structure in your web server directory (usually `htdocs` for XAMPP):

```
your-project-folder/
├── index.html          (your existing HTML file)
├── config.php          (database configuration)
├── api.php            (main API endpoint)
├── style.css          (your existing CSS)
└── script.js          (updated JavaScript file)
```

### 4. Configure Database Connection

1. Open `config.php`
2. Update the database credentials if needed:
   ```php
   private static $host = 'localhost';        // Usually localhost
   private static $dbname = 'event_management'; // Database name
   private static $username = 'root';         // Your MySQL username
   private static $password = '';             // Your MySQL password (empty for XAMPP default)
   ```

### 5. Update Your HTML

Make sure your HTML file includes the updated JavaScript. Replace your existing script section with the new EventManager code.

### 6. Test the Installation

1. Open your browser
2. Go to `http://localhost/your-project-folder/`
3. Test the following:
   - Admin login (password: admin123)
   - Create events
   - Register participants
   - View statistics
   - Export data

## Database Structure

The system uses 4 main tables:

- **events**: Stores event information
- **participants**: Stores participant details
- **event_registrations**: Links participants to events (many-to-many)
- **admin_users**: Stores admin credentials

## API Endpoints

The system provides the following API endpoints:

- `GET api.php?action=get_events` - Retrieve all events
- `POST api.php?action=add_event` - Create new event
- `POST api.php?action=register_participant` - Register participant
- `POST api.php?action=remove_participant` - Remove participant
- `POST api.php?action=clear_events` - Clear all events
- `GET api.php?action=get_statistics` - Get statistics
- `POST api.php?action=admin_login` - Admin authentication
- `GET api.php?action=export_csv` - Export data as CSV

## Security Considerations

For production use, consider:

1. **Change default admin password**
2. **Use environment variables for database credentials**
3. **Implement proper session management**
4. **Add input validation and sanitization**
5. **Use HTTPS**
6. **Implement rate limiting**

## Troubleshooting

### Database Connection Issues

1. Check if MySQL service is running
2. Verify database credentials in `config.php`
3. Ensure database `event_management` exists
4. Check PHP error logs

### Permission Issues

1. Ensure web server has read/write permissions to your project folder
2. Check file permissions (755 for directories, 644 for files)

### API Not Working

1. Check if `mod_rewrite` is enabled in Apache
2. Verify PHP version compatibility
3. Check browser console for JavaScript errors
4. Enable PHP error reporting for debugging

## Backup and Restore

### Backup Database

```bash
mysqldump -u root -p event_management > backup.sql
```

### Restore Database

```bash
mysql -u root -p event_management < backup.sql
```

## Migration from Local Storage

The new system automatically handles data in the database. Your existing local storage data will not be migrated automatically. If you need to migrate existing data, you'll need to manually re-enter it through the admin interface.

## Support

If you encounter issues:

1. Check the browser console for JavaScript errors
2. Check PHP error logs
3. Verify database connection
4. Ensure all files are properly uploaded
5. Test API endpoints directly using tools like Postman