-- Sample Test Data for Users Table
-- Run this after successfully running migrations

-- Insert sample users
INSERT INTO users (email, password_hash, name, avatar_url, is_active, is_super_admin, type, jira_id, created_at, is_deleted)
VALUES 
    ('alice.johnson@company.com', '$2a$10$samplehash1', 'Alice Johnson', 'https://ui-avatars.com/api/?name=Alice+Johnson', true, true, 'Internal', 'JIRA-001', CURRENT_TIMESTAMP, false),
    ('bob.smith@company.com', '$2a$10$samplehash2', 'Bob Smith', 'https://ui-avatars.com/api/?name=Bob+Smith', true, false, 'Internal', 'JIRA-002', CURRENT_TIMESTAMP, false),
    ('charlie.davis@company.com', '$2a$10$samplehash3', 'Charlie Davis', 'https://ui-avatars.com/api/?name=Charlie+Davis', true, false, 'External', 'JIRA-003', CURRENT_TIMESTAMP, false),
    ('diana.wilson@company.com', '$2a$10$samplehash4', 'Diana Wilson', 'https://ui-avatars.com/api/?name=Diana+Wilson', false, false, 'Internal', 'JIRA-004', CURRENT_TIMESTAMP, false),
    ('edward.brown@company.com', '$2a$10$samplehash5', 'Edward Brown', 'https://ui-avatars.com/api/?name=Edward+Brown', true, false, 'Contractor', NULL, CURRENT_TIMESTAMP, false);

-- Update last_login for some users
UPDATE users 
SET last_login = CURRENT_TIMESTAMP - INTERVAL '1 day'
WHERE email IN ('alice.johnson@company.com', 'bob.smith@company.com');

UPDATE users 
SET last_login = CURRENT_TIMESTAMP - INTERVAL '7 days'
WHERE email = 'charlie.davis@company.com';

-- Verify data
SELECT id, name, email, type, is_active, is_super_admin, created_at, last_login 
FROM users 
WHERE is_deleted = false
ORDER BY id;
