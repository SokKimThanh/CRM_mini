## Day 6 — 2026-10-05
1. **CQRS Tách biệt Tuyệt đối (K12):** Query chỉ đọc dùng `.AsNoTracking()` tối ưu tài nguyên; Command xử lý giao dịch và concurrency.
2. **Phòng thủ IDOR 2 Lớp (K39):** Lớp 1 (EF Core Global Query Filter) ngăn rò rỉ dữ liệu chéo Tenant; Lớp 2 (Handler Role Check) bảo vệ quyền sở hữu dữ liệu cấp nhân sự.
3. **Điều hướng Tường minh (K16, K17):** Luôn khai báo đầy đủ collection đối ứng và `.HasForeignKey()` kèm `.HasConstraintName()` để xóa sổ $100\%$ shadow property ngầm.
4. **Optimistic Concurrency với PostgreSQL xmin (K19):** Sử dụng cột hệ thống `xmin` làm concurrency token mà không cần tạo thêm cột trên bảng vật lý.
5. **Clean Architecture Dependency Flow:** Đặt interface dùng chung (`ITenantProvider`, `ICurrentUser`) tại Domain để ngăn chặn triệt để phụ thuộc vòng giữa Data và Business.
