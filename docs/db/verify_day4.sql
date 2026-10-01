-- 1. Kiểm tra tổng số lượng bản ghi các bảng chính
SELECT 'Customers' AS table_name, COUNT(*) AS count_val, 20 AS target_val FROM customers
UNION ALL SELECT 'Contacts', COUNT(*), 30 FROM contacts
UNION ALL SELECT 'Products', COUNT(*), 20 FROM products
UNION ALL SELECT 'Opportunities', COUNT(*), 15 FROM opportunities
UNION ALL SELECT 'Stage Histories', COUNT(*), 45 FROM stage_histories
UNION ALL SELECT 'Assigned Customers', COUNT(*), 20 FROM customers WHERE assigned_to_user_id IS NOT NULL;

-- 2. Kiểm tra phân bổ Opportunities theo Pipeline Stage
SELECT 
    s.id AS stage_id,
    s.name AS stage_name,
    COUNT(o.id) AS opp_count,
    COALESCE(SUM(o.estimated_value), 0) AS total_value
FROM opportunity_stages s
LEFT JOIN opportunities o ON o.stage_id = s.id
GROUP BY s.id, s.name
ORDER BY s.id;

-- 3. Kiểm tra phân bổ Customer Health
SELECT health_status, COUNT(*) AS count_val 
FROM customers 
GROUP BY health_status 
ORDER BY health_status;

-- 4. Kiểm tra phân bổ nhân sự phụ trách
SELECT u."Email", COUNT(c.id) AS assigned_customer_count
FROM "AspNetUsers" u
JOIN customers c ON c.assigned_to_user_id = u."Id"
GROUP BY u."Email"
ORDER BY assigned_customer_count DESC;