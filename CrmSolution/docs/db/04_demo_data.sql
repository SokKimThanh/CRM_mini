TRUNCATE TABLE 
    interactions,
    stage_histories,
    sales_tasks,
    quote_items,
    quotes,
    opportunities,
    customer_assignments,
    contacts,
    customers,
    products
RESTART IDENTITY CASCADE;

INSERT INTO customers (id, code, name, industry, tax_code, phone, email, address, health_status, average_cycle_days, revenue_90d, order_count_90d) VALUES
(1, 'KH-0001', 'Công ty TNHH Cơ khí An Phát', 'Gia công cơ khí', '0101234567', '0912345678', 'contact@anphat.vn', 'KCN Quang Minh, Mê Linh, Hà Nội', 1, 30, 145000000, 4),
(2, 'KH-0002', 'Cơ khí Chính xác Việt Nhật', 'Chế tạo máy', '0102345678', '0913456789', 'info@vietnhat.vn', 'KCN Thăng Long, Đông Anh, Hà Nội', 1, 45, 280000000, 5),
(3, 'KH-0003', 'Khuôn Mẫu Tân Á', 'Khuôn mẫu', '0103456789', '0914567890', 'sales@tana.vn', 'KCN Tân Tạo, Bình Tân, TP.HCM', 2, 60, 42000000, 1),
(4, 'KH-0004', 'Bulong Ốc Vít Nam An', 'Sản xuất ốc vít', '0104567890', '0915678901', 'naman@ocvit.vn', 'KCN Biên Hòa, Đồng Nai', 0, 30, 0, 0),
(5, 'KH-0005', 'Dụng Cụ Hải Nam', 'Dụng cụ cầm tay', '0105678901', '0916789012', 'hainam@tools.vn', 'KCN Long Hậu, Long An', 0, 0, 0, 0),
(6, 'KH-0006', 'Cơ khí Thắng Lợi', 'Gia công CNC', '0106789012', '0917890123', 'info@thangloi.vn', 'KCN Sài Đồng, Long Biên, Hà Nội', 1, 30, 195000000, 3),
(7, 'KH-0007', 'Chế Tạo Máy Minh Đức', 'Chế tạo máy', '0107890123', '0918901234', 'minhduc@ctm.vn', 'KCN Từ Liêm, Hà Nội', 1, 60, 95000000, 2),
(8, 'KH-0008', 'Bơm Công Nghiệp Sài Gòn', 'Sản xuất bơm', '0108901234', '0919012345', 'saigon@pump.vn', 'KCN Tân Bình, TP.HCM', 2, 45, 68000000, 1),
(9, 'KH-0009', 'Cơ khí Hoàng Long', 'Gia công cơ khí', '0109012345', '0920123456', 'hoanglong@ck.vn', 'KCN VSIP, Bình Dương', 1, 30, 210000000, 6),
(10, 'KH-0010', 'Kỹ Thuật Tân Hưng', 'Kỹ thuật CN', '0110123456', '0921234567', 'tanhung@kt.vn', 'KCN Amata, Biên Hòa', 2, 60, 35000000, 1),
(11, 'KH-0011', 'Cơ Điện Lạnh Bình Minh', 'Cơ điện lạnh', '0111234567', '0922345678', 'binhminh@cdl.vn', 'Q. Tân Phú, TP.HCM', 0, 30, 0, 0),
(12, 'KH-0012', 'Đúc Kim Loại Phú Thịnh', 'Đúc kim loại', '0112345678', '0923456789', 'phuthinh@dk.vn', 'KCN Hiệp Phước, TP.HCM', 1, 45, 178000000, 4),
(13, 'KH-0013', 'Cơ khí Đông Á', 'Gia công', '0113456789', '0924567890', 'donga@ck.vn', 'KCN Đình Vũ, Hải Phòng', 1, 30, 240000000, 5),
(14, 'KH-0014', 'Máy Công Cụ Hà Nội', 'Máy công cụ', '0114567890', '0925678901', 'hn@machine.vn', 'Q. Hai Bà Trưng, Hà Nội', 2, 60, 52000000, 1),
(15, 'KH-0015', 'Thiết Bị Công Nghiệp Việt', 'Thiết bị CN', '0115678901', '0926789012', 'viet@tbcn.vn', 'KCN Long Bình, Đồng Nai', 0, 0, 0, 0),
(16, 'KH-0016', 'Cơ khí Đại Việt', 'Gia công CNC', '0116789012', '0927890123', 'daiviet@ck.vn', 'KCN Tân Đô, Long An', 1, 30, 128000000, 3),
(17, 'KH-0017', 'Khuôn Chính Xác Sài Gòn', 'Khuôn mẫu', '0117890123', '0928901234', 'saigon@khuon.vn', 'Q. Bình Thạnh, TP.HCM', 1, 45, 165000000, 3),
(18, 'KH-0018', 'Cơ khí Phương Đông', 'Gia công cơ khí', '0118901234', '0929012345', 'phuongdong@ck.vn', 'KCN Phú Mỹ, Bà Rịa - VT', 2, 30, 45000000, 1),
(19, 'KH-0019', 'Chế Tạo Khuôn Nam Phát', 'Khuôn mẫu', '0119012345', '0930123456', 'namphat@khuon.vn', 'KCN Tân Kim, Long An', 0, 30, 0, 0),
(20, 'KH-0020', 'Cơ khí Trường Thịnh', 'Gia công cơ khí', '0120123456', '0931234567', 'truongthinh@ck.vn', 'KCN Đồng Văn, Hà Nam', 1, 30, 220000000, 5);

SELECT setval('customers_id_seq', 20, true);

INSERT INTO contacts (id, customer_id, name, position, phone, email, is_primary) VALUES
(1, 1, 'Trần Văn Cường', 'Trưởng phòng thu mua', '0912345001', 'cuong@anphat.vn', TRUE),
(2, 1, 'Lê Thị Mai', 'Kế toán trưởng', '0912345002', 'mai@anphat.vn', FALSE),
(3, 2, 'Nguyễn Thị Hà', 'Kế toán trưởng', '0912345003', 'ha@vietnhat.vn', TRUE),
(4, 2, 'Phạm Văn Hùng', 'Kỹ thuật', '0912345004', 'hung@vietnhat.vn', FALSE),
(5, 3, 'Lê Văn Tuấn', 'Giám đốc kỹ thuật', '0912345005', 'tuan@tana.vn', TRUE),
(6, 4, 'Phạm Minh Đức', 'Nhân viên thu mua', '0912345006', 'duc@ocvit.vn', TRUE),
(7, 5, 'Hoàng Văn Nam', 'Chủ doanh nghiệp', '0912345007', 'nam@tools.vn', TRUE),
(8, 6, 'Đỗ Văn Khoa', 'Trưởng phòng KT', '0912345008', 'khoa@thangloi.vn', TRUE),
(9, 6, 'Nguyễn Văn Long', 'Thủ kho', '0912345009', 'long@thangloi.vn', FALSE),
(10, 7, 'Trịnh Văn Bình', 'Giám đốc', '0912345010', 'binh@ctm.vn', TRUE),
(11, 8, 'Vũ Thị Hương', 'Trưởng phòng mua', '0912345011', 'huong@pump.vn', TRUE),
(12, 9, 'Lý Văn Hải', 'Trưởng phòng KT', '0912345012', 'hai@hoanglong.vn', TRUE),
(13, 9, 'Ngô Thị Lan', 'Kế toán', '0912345013', 'lan@hoanglong.vn', FALSE),
(14, 10, 'Dương Văn Tùng', 'Giám đốc', '0912345014', 'tung@tanhung.vn', TRUE),
(15, 11, 'Trần Đình Quang', 'Trưởng phòng mua', '0912345015', 'quang@binhminh.vn', TRUE),
(16, 12, 'Hoàng Văn Sơn', 'Trưởng phòng KT', '0912345016', 'son@phuthinh.vn', TRUE),
(17, 13, 'Lê Đình Nam', 'Giám đốc kỹ thuật', '0912345017', 'nam@donga.vn', TRUE),
(18, 13, 'Nguyễn Thị Thu', 'Kế toán', '0912345018', 'thu@donga.vn', FALSE),
(19, 14, 'Bùi Văn Tài', 'Trưởng phòng mua', '0912345019', 'tai@machine.vn', TRUE),
(20, 15, 'Nguyễn Văn Bình', 'Chủ tịch', '0912345020', 'binh@tbcn.vn', TRUE),
(21, 16, 'Trần Quốc Bảo', 'TP Kinh doanh', '0912345021', 'bao@daiviet.vn', TRUE),
(22, 17, 'Phan Văn Hòa', 'Giám đốc', '0912345022', 'hoa@khuon.vn', TRUE),
(23, 18, 'Đặng Văn Lâm', 'Trưởng phòng KT', '0912345023', 'lam@phuongdong.vn', TRUE),
(24, 19, 'Võ Văn Đông', 'Chủ doanh nghiệp', '0912345024', 'dong@namphat.vn', TRUE),
(25, 20, 'Nguyễn Văn Hóa', 'Giám đốc', '0912345025', 'hoa@truongthinh.vn', TRUE),
(26, 20, 'Trần Thị Hồng', 'Kế toán', '0912345026', 'hong@truongthinh.vn', FALSE),
(27, 2, 'Lê Văn Đức', 'Thủ kho', '0912345027', 'duc@vietnhat.vn', FALSE),
(28, 7, 'Phạm Thị Xuân', 'Kế toán', '0912345028', 'xuan@ctm.vn', FALSE),
(29, 12, 'Bùi Văn Hậu', 'Kỹ thuật', '0912345029', 'hau@phuthinh.vn', FALSE),
(30, 16, 'Nguyễn Văn Quý', 'Thủ kho', '0912345030', 'quy@daiviet.vn', FALSE);

SELECT setval('contacts_id_seq', 30, true);

INSERT INTO products (id, code, name, category_id, unit, base_price, cost_price) VALUES
(1, 'DP-001', 'Dao phay Ø10 HSS', 1, 'Cái', 450000, 320000),
(2, 'DP-002', 'Mũi khoan mạ Titan Ø5', 1, 'Cây', 85000, 60000),
(3, 'DP-003', 'Mảnh tiện CNC Kyocera CNMG', 1, 'Hộp', 1200000, 850000),
(4, 'DP-004', 'Dao tiện ngoài 16x16mm', 1, 'Cây', 580000, 420000),
(5, 'DP-005', 'Mũi phay ngón Ø12', 1, 'Cây', 380000, 270000),
(6, 'DM-001', 'Đá mài 100x6x16', 2, 'Viên', 45000, 30000),
(7, 'DM-002', 'Đĩa cắt sắt 105x1.2', 2, 'Cái', 15000, 10000),
(8, 'DM-003', 'Bánh mài nỉ 150mm', 2, 'Cái', 125000, 85000),
(9, 'DM-004', 'Đá mài bàn 150x20x32', 2, 'Viên', 95000, 68000),
(10, 'DM-005', 'Đĩa cắt inox 125x1.0', 2, 'Cái', 18000, 12000),
(11, 'OV-001', 'Bulong M8x30 inox', 3, 'Con', 5000, 3200),
(12, 'OV-002', 'Đai ốc M10 thép', 3, 'Con', 3500, 2200),
(13, 'OV-003', 'Vít đầu dù M6x20', 3, 'Con', 2800, 1800),
(14, 'OV-004', 'Long đền phẳng M8', 3, 'Cái', 800, 500),
(15, 'OV-005', 'Bulong M12x50 inox 304', 3, 'Con', 12000, 8500),
(16, 'DB-001', 'Dầu cắt gọt KoolKut 20L', 4, 'Can', 650000, 480000),
(17, 'DB-002', 'Mỡ bôi trơn công nghiệp 1kg', 4, 'Hộp', 180000, 120000),
(18, 'DB-003', 'Dầu thủy lực AW68 200L', 4, 'Phuy', 8500000, 6800000),
(19, 'DB-004', 'Dầu làm mát CNC 20L', 4, 'Can', 720000, 550000),
(20, 'DB-005', 'Mỡ chịu nhiệt cao 500g', 4, 'Tuýp', 95000, 65000);

SELECT setval('products_id_seq', 20, true);

INSERT INTO opportunities (id, code, customer_id, contact_id, stage_id, title, estimated_value, probability, expected_close_date, source) VALUES
(1, 'OPP-0001', 1, 1, 1, 'Lô dao phay tháng 10', 35000000, 10, NOW() + INTERVAL '30 days', 'Triển lãm'),
(2, 'OPP-0002', 2, 3, 1, 'Cung cấp mũi khoan quý 4', 45000000, 10, NOW() + INTERVAL '25 days', 'Gọi điện'),
(3, 'OPP-0003', 3, 5, 2, 'Tư vấn đá mài cho xưởng khuôn', 28000000, 30, NOW() + INTERVAL '40 days', 'Giới thiệu'),
(4, 'OPP-0004', 6, 8, 2, 'Bulong inox cho dây chuyền', 65000000, 30, NOW() + INTERVAL '35 days', 'Web'),
(5, 'OPP-0005', 9, 12, 3, 'Báo giá dầu cắt gọt KoolKut', 35000000, 50, NOW() + INTERVAL '20 days', 'Gọi điện'),
(6, 'OPP-0006', 13, 17, 3, 'Đá mài bàn cho xưởng mới', 42000000, 50, NOW() + INTERVAL '22 days', 'Giới thiệu'),
(7, 'OPP-0007', 4, 6, 4, 'Đàm phán hợp đồng năm 2027', 180000000, 70, NOW() + INTERVAL '60 days', 'Web'),
(8, 'OPP-0008', 7, 10, 4, 'Dầu thủy lực AW68 số lượng lớn', 170000000, 70, NOW() + INTERVAL '55 days', 'Triển lãm'),
(9, 'OPP-0009', 5, 7, 5, 'Đơn đầu tiên dụng cụ cầm tay', 25000000, 100, NOW() - INTERVAL '5 days', 'Walk-in'),
(10, 'OPP-0010', 16, 21, 5, 'Mũi phay ngón Ø12 - 100 cây', 38000000, 100, NOW() - INTERVAL '3 days', 'Gọi điện'),
(11, 'OPP-0011', 8, 11, 6, 'Bơm công nghiệp - thua đối thủ', 95000000, 0, NOW() - INTERVAL '10 days', 'Web'),
(12, 'OPP-0012', 10, 14, 6, 'Kỹ thuật Tân Hưng - giá cao', 42000000, 0, NOW() - INTERVAL '8 days', 'Giới thiệu'),
(13, 'OPP-0013', 12, 16, 4, 'Đúc Phú Thịnh - đang đàm phán', 145000000, 70, NOW() + INTERVAL '45 days', 'Triển lãm'),
(14, 'OPP-0014', 17, 22, 3, 'Khuôn Sài Gòn - báo giá mảnh tiện', 52000000, 50, NOW() + INTERVAL '18 days', 'Web'),
(15, 'OPP-0015', 20, 25, 2, 'Trường Thịnh - cần dầu làm mát', 68000000, 30, NOW() + INTERVAL '38 days', 'Gọi điện');

SELECT setval('opportunities_id_seq', 15, true);

INSERT INTO stage_histories (opportunity_id, from_stage_id, to_stage_id, note) VALUES
(1, NULL, 1, 'Tạo cơ hội mới'),
(2, NULL, 1, 'Tạo cơ hội mới'),
(3, NULL, 1, 'Tạo cơ hội mới'),
(3, 1, 2, 'Gặp trực tiếp khách hàng'),
(4, NULL, 1, 'Tạo cơ hội mới'),
(4, 1, 2, 'Đã kết nối qua điện thoại'),
(5, NULL, 1, 'Tạo cơ hội mới'),
(5, 1, 2, 'Gọi điện tư vấn thông số dầu cắt'),
(5, 2, 3, 'Đã gửi báo giá chi tiết qua email'),
(6, NULL, 1, 'Tạo cơ hội mới'),
(6, 1, 2, 'Khảo sát nhu cầu thực tế xưởng'),
(6, 2, 3, 'Đã chốt danh sách sản phẩm và gửi báo giá'),
(7, NULL, 1, 'Tạo cơ hội mới'),
(7, 1, 2, 'Đàm phán sơ bộ về chính sách chiết khấu'),
(7, 2, 3, 'Báo giá chính thức theo khối lượng năm'),
(7, 3, 4, 'Bắt đầu vòng đàm phán hợp đồng khung'),
(8, NULL, 1, 'Tạo cơ hội mới'),
(8, 1, 2, 'Khảo sát hệ thống thủy lực nhà máy'),
(8, 2, 3, 'Báo giá gói 20 phuy dầu AW68'),
(8, 3, 4, 'Đàm phán điều khoản thanh toán 60 ngày'),
(9, NULL, 1, 'Tạo cơ hội mới'),
(9, 1, 2, 'Tiếp khách tại showroom'),
(9, 2, 3, 'Gửi báo giá dụng cụ cầm tay'),
(9, 3, 4, 'Thống nhất bảng giá ưu đãi mở điểm bán'),
(9, 4, 5, 'Ký hợp đồng và nhận tiền đặt cọc'),
(10, NULL, 1, 'Tạo cơ hội mới'),
(10, 1, 2, 'Khách hàng gọi đặt bổ sung vật tư tiêu hao'),
(10, 2, 3, 'Gửi báo giá giao ngay trong ngày'),
(10, 3, 4, 'Khách duyệt báo giá qua Zalo'),
(10, 4, 5, 'Xuất kho và hoàn tất thanh toán tiền mặt'),
(11, NULL, 1, 'Tạo cơ hội mới'),
(11, 1, 2, 'Liên hệ phòng kỹ thuật'),
(11, 2, 6, 'Thua đối thủ địa phương về giá và chi phí vận chuyển'),
(12, NULL, 1, 'Tạo cơ hội mới'),
(12, 1, 2, 'Gặp trực tiếp quản đốc'),
(12, 2, 6, 'Ngân sách công ty khách bị cắt giảm'),
(13, NULL, 1, 'Tạo cơ hội mới'),
(13, 1, 2, 'Khảo sát nhà máy đúc Phú Thịnh'),
(13, 2, 3, 'Gửi bảng giá đại lý cấp 1'),
(13, 3, 4, 'Đang làm rõ điều khoản công nợ gối đầu'),
(14, NULL, 1, 'Tạo cơ hội mới'),
(14, 1, 2, 'Liên hệ qua thông tin để lại trên website'),
(14, 2, 3, 'Gửi báo giá mảnh tiện Kyocera'),
(15, NULL, 1, 'Tạo cơ hội mới'),
(15, 1, 2, 'Gọi điện trao đổi quy cách dầu làm mát');

UPDATE customers 
SET assigned_to_user_id = (SELECT "Id" FROM "AspNetUsers" WHERE "Email" = 'sales1@crm.local')
WHERE code IN ('KH-0001','KH-0002','KH-0006','KH-0009','KH-0013','KH-0016','KH-0020');

UPDATE customers 
SET assigned_to_user_id = (SELECT "Id" FROM "AspNetUsers" WHERE "Email" = 'sales2@crm.local')
WHERE code IN ('KH-0003','KH-0004','KH-0007','KH-0010','KH-0012','KH-0017');

UPDATE customers 
SET assigned_to_user_id = (SELECT "Id" FROM "AspNetUsers" WHERE "Email" = 'manager@crm.local')
WHERE assigned_to_user_id IS NULL;