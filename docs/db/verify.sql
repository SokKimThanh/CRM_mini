SELECT "Email" FROM "AspNetUsers" ORDER BY "Email";
SELECT "Name" FROM "AspNetRoles" ORDER BY "Name";
SELECT full_name, employee_code, role_code FROM user_profiles ORDER BY employee_code;
SELECT u."Email", r."Name" AS role FROM "AspNetUsers" u
  JOIN "AspNetUserRoles" ur ON u."Id" = ur."UserId"
  JOIN "AspNetRoles" r ON ur."RoleId" = r."Id"
  ORDER BY u."Email";