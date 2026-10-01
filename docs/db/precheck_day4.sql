SELECT 'categories_count' AS check_item, COUNT(*) AS actual, 4 AS expected FROM categories
UNION ALL
SELECT 'stages_count', COUNT(*), 6 FROM opportunity_stages
UNION ALL
SELECT 'users_count', COUNT(*), 5 FROM "AspNetUsers";