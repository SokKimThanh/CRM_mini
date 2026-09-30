-- =====================================================================
-- CRM B2B SYSTEM — SEED DATA MẪU
-- Database Engine : PostgreSQL 16+ / 18+
-- Target Database : crm_db
-- File Path       : Documents/db/seeds/02_demo_data.sql
-- =====================================================================
SET
    TIME ZONE 'UTC';

SET
    client_encoding = 'UTF8';

-- PHẦN 1: DỌN DẸP DỮ LIỆU CŨ & RESET IDENTITY TRƯỚC KHI NẠP
TRUNCATE TABLE interactions,
stage_histories,
sales_tasks,
quote_items,
quotes,
opportunities,
customer_assignments,
contacts,
customers,
products RESTART IDENTITY CASCADE;

-- PHẦN 2: SEED 5 KHÁCH HÀNG DOANH NGHIỆP B2B (NHÀ MÁY CƠ KHÍ)
INSERT INTO
    customers (
        id,
        code,
        name,
        industry,
        tax_code,
        phone,
        email,
        website,
        address,
        health_status,
        average_cycle_days,
        assigned_to_user_id,
        revenue_90d,
        order_count_90d,
        current_debt,
        note,
        is_active,
        is_deleted
    )
VALUES
    (
        1,
        'KH-0001',
        'Công ty TNHH Cơ Khí Chính Xác An Phát',
        'Gia công phay/tiện CNC',
        '0101234567',
        '0912345678',
        'contact@anphat.vn',
        'https://anphatcnc.vn',
        'KCN Quang Minh, Mê Linh, Hà Nội',
        1,
        30,
        'seed-user-1',
        145000000.00,
        4,
        15000000.00,
        'Khách hàng VIP mảng gia công linh kiện xe máy',
        TRUE,
        FALSE
    ),
    (
        2,
        'KH-0002',
        'Công ty Cổ phần Chế Tạo Máy Việt Nhật',
        'Chế tạo máy tự động',
        '0102345678',
        '0913456789',
        'info@vietnhat.vn',
        'https://vietnhatmachinery.com',
        'KCN Thăng Long, Đông Anh, Hà Nội',
        1,
        45,
        'seed-user-1',
        280000000.00,
        5,
        0.00,
        'Nhà máy FDI Nhật Bản, thanh toán rất đúng hạn',
        TRUE,
        FALSE
    ),
    (
        3,
        'KH-0003',
        'Tân Á Precision Mold Co.',
        'Khuôn mẫu chính xác',
        '0103456789',
        '0914567890',
        'sales@tana.vn',
        'https://tanamold.vn',
        'KCN Tân Tạo, Bình Tân, TP.HCM',
        2,
        60,
        'seed-user-2',
        42000000.00,
        1,
        22000000.00,
        'Đang chậm tiến độ thanh toán đợt gần nhất',
        TRUE,
        FALSE
    ),
    (
        4,
        'KH-0004',
        'Nhà máy Phụ Tùng Nam An',
        'Sản xuất bulong ốc vít',
        '0104567890',
        '0915678901',
        'naman@ocvit.vn',
        'https://namanfastener.com',
        'KCN Biên Hòa 2, Đồng Nai',
        3,
        30,
        'seed-user-2',
        0.00,
        0,
        55000000.00,
        'Cảnh báo nợ quá hạn 45 ngày, cần thu hồi công nợ',
        TRUE,
        FALSE
    ),
    (
        5,
        'KH-0005',
        'Xưởng Cơ Khí Tổng Hợp Hải Nam',
        'Gia công cơ khí nhỏ lẻ',
        '0105678901',
        '0916789012',
        'hainam@tools.vn',
        NULL,
        'KCN Long Hậu, Cần Giuộc, Long An',
        0,
        0,
        'seed-user-1',
        0.00,
        0,
        0.00,
        'Khách hàng mới tiếp cận qua triển lãm',
        TRUE,
        FALSE
    );

SELECT
    setval(
        'customers_id_seq',
        (
            SELECT
                COALESCE(MAX(id), 1)
            FROM
                customers
        ),
        true
    );

-- PHẦN 3: SEED 5 NGƯỜI LIÊN HỆ ĐẠI DIỆN (CONTACTS)
INSERT INTO
    contacts (
        id,
        customer_id,
        name,
        position,
        phone,
        email,
        birthday,
        is_primary,
        note
    )
VALUES
    (
        1,
        1,
        'Trần Văn Cường',
        'Trưởng phòng Thu mua',
        '0912345001',
        'cuong@anphat.vn',
        '1985-05-12',
        TRUE,
        'Quyết định việc chọn nhà cung cấp dao cụ'
    ),
    (
        2,
        2,
        'Nguyễn Thị Hà',
        'Kế toán trưởng',
        '0912345002',
        'ha@vietnhat.vn',
        '1988-11-24',
        TRUE,
        'Phụ trách hồ sơ thầu và công nợ'
    ),
    (
        3,
        3,
        'Lê Văn Tuấn',
        'Giám đốc Kỹ thuật',
        '0912345003',
        'tuan@tana.vn',
        '1980-03-18',
        TRUE,
        'Trực tiếp đánh giá chất lượng đá mài và dao phay'
    ),
    (
        4,
        4,
        'Phạm Minh Đức',
        'Nhân viên Mua hàng',
        '0912345004',
        'duc@ocvit.vn',
        '1992-09-02',
        TRUE,
        'Đầu mối nhận báo giá và theo dõi giao hàng'
    ),
    (
        5,
        5,
        'Hoàng Văn Nam',
        'Chủ cơ sở xưởng',
        '0912345005',
        'nam@tools.vn',
        '1979-07-15',
        TRUE,
        'Liên hệ trực tiếp qua Zalo hoặc điện thoại'
    );

SELECT
    setval(
        'contacts_id_seq',
        (
            SELECT
                COALESCE(MAX(id), 1)
            FROM
                contacts
        ),
        true
    );

-- PHẦN 4: SEED 10 SẢN PHẨM CÔNG NGHIỆP CƠ KHÍ (PRODUCTS)
INSERT INTO
    products (
        id,
        code,
        name,
        category_id,
        unit,
        base_price,
        cost_price,
        description,
        is_active
    )
VALUES
    (
        1,
        'DP-001',
        'Dao phay ngón HSS Ø10 4 me cắt',
        1,
        'Cái',
        450000.00,
        320000.00,
        'Thép gió HSS phủ TiAlN dùng phay thép carbon',
        TRUE
    ),
    (
        2,
        'DP-002',
        'Mũi khoan xoắn mạ Titan Ø5.0',
        1,
        'Mũi',
        85000.00,
        60000.00,
        'Khoan thép hợp kim và inox chất lượng cao',
        TRUE
    ),
    (
        3,
        'DP-003',
        'Mảnh tiện CNC Kyocera CNMG 120408',
        1,
        'Hộp',
        1200000.00,
        850000.00,
        'Quy cách 10 viên/hộp, tiện thô thép kết cấu',
        TRUE
    ),
    (
        4,
        'DM-001',
        'Đá mài phẳng Norton 180x13x31.75',
        2,
        'Viên',
        185000.00,
        130000.00,
        'Hạt mài nhôm oxit trắng WA chuyên mài khuôn',
        TRUE
    ),
    (
        5,
        'DM-002',
        'Đĩa cắt kim loại Hải Dương 105x1.2mm',
        2,
        'Viên',
        15000.00,
        10000.00,
        'Lưới sợi thủy tinh gia cường chống vỡ',
        TRUE
    ),
    (
        6,
        'DM-003',
        'Bánh mài nỉ xám đánh bóng kim loại 150mm',
        2,
        'Cái',
        125000.00,
        85000.00,
        'Tạo xước hairline và đánh bóng bề mặt inox',
        TRUE
    ),
    (
        7,
        'OV-001',
        'Bulong lục giác ngoài M8x30 Inox 304',
        3,
        'Con',
        5000.00,
        3200.00,
        'Chống gỉ sét, kèm đai ốc và lông đền phẳng',
        TRUE
    ),
    (
        8,
        'OV-002',
        'Đai ốc thép cường độ cao M10 Grade 8.8',
        3,
        'Con',
        3500.00,
        2200.00,
        'Dùng lắp ráp khung giàn máy hạng nặng',
        TRUE
    ),
    (
        9,
        'DB-001',
        'Dầu cắt gọt pha nước KoolKut 20L',
        4,
        'Thùng',
        650000.00,
        480000.00,
        'Nhũ tương làm mát bôi trơn gia công CNC',
        TRUE
    ),
    (
        10,
        'DB-002',
        'Mỡ bôi trơn chịu nhiệt SKF LGMT 2/1',
        4,
        'Hộp',
        280000.00,
        200000.00,
        'Quy cách 1kg chịu nhiệt độ lên đến 120°C',
        TRUE
    );

SELECT
    setval(
        'products_id_seq',
        (
            SELECT
                COALESCE(MAX(id), 1)
            FROM
                products
        ),
        true
    );

-- PHẦN 5: SEED 5 CƠ HỘI BÁN HÀNG PHỦ 5 STAGES (OPPORTUNITIES)
INSERT INTO
    opportunities (
        id,
        code,
        customer_id,
        contact_id,
        stage_id,
        title,
        description,
        estimated_value,
        probability,
        assigned_to_user_id,
        expected_close_date,
        actual_close_date,
        source,
        is_won,
        is_deleted
    )
VALUES
    (
        1,
        'OPP-0001',
        1,
        1,
        1,
        'Lô dao phay ngón định kỳ Quý 4',
        'Đơn hàng bổ sung kho vật tư quý cuối năm',
        35000000.00,
        10,
        'seed-user-1',
        NOW() + INTERVAL '30 days',
        NULL,
        'Hội chợ VIMEXPO',
        FALSE,
        FALSE
    ),
    (
        2,
        'OPP-0002',
        2,
        2,
        2,
        'Mảnh tiện CNC Kyocera số lượng lớn',
        'Cung cấp cho xưởng sản xuất khung máy tự động',
        85000000.00,
        30,
        'seed-user-1',
        NOW() + INTERVAL '45 days',
        NULL,
        'Khách hàng cũ gọi lại',
        FALSE,
        FALSE
    ),
    (
        3,
        'OPP-0003',
        3,
        3,
        3,
        'Hợp đồng đá mài Norton làm khuôn',
        'Báo giá lô đá mài WA và nỉ đánh bóng inox',
        42000000.00,
        50,
        'seed-user-2',
        NOW() + INTERVAL '15 days',
        NULL,
        'Kỹ sư xưởng giới thiệu',
        FALSE,
        FALSE
    ),
    (
        4,
        'OPP-0004',
        4,
        4,
        4,
        'Đấu thầu cung cấp bulong đai ốc 2027',
        'Hợp đồng nguyên tắc cung ứng vật tư 12 tháng',
        180000000.00,
        70,
        'seed-user-2',
        NOW() + INTERVAL '60 days',
        NULL,
        'Website / Google Ads',
        FALSE,
        FALSE
    ),
    (
        5,
        'OPP-0005',
        5,
        5,
        5,
        'Đơn hàng thử nghiệm dầu làm mát và mỡ SKF',
        'Chốt đơn hàng thử nghiệm đầu mối xưởng mới',
        25000000.00,
        100,
        'seed-user-1',
        NOW() - INTERVAL '2 days',
        NOW() - INTERVAL '2 days',
        'Khách vãng lai đến kho',
        TRUE,
        FALSE
    );

SELECT
    setval(
        'opportunities_id_seq',
        (
            SELECT
                COALESCE(MAX(id), 1)
            FROM
                opportunities
        ),
        true
    );

-- PHẦN 6: SEED 14 DÒNG LỊCH SỬ CHUYỂN GIAI ĐOẠN (STAGE_HISTORIES)
INSERT INTO
    stage_histories (
        opportunity_id,
        from_stage_id,
        to_stage_id,
        changed_by_user_id,
        note,
        changed_at
    )
VALUES
    -- Cơ hội 1 (Stage 1)
    (
        1,
        NULL,
        1,
        'seed-user-1',
        'Ghi nhận nhu cầu ban đầu tại gian hàng triển lãm',
        NOW() - INTERVAL '10 days'
    ),
    -- Cơ hội 2 (Stage 1 -> 2)
    (
        2,
        NULL,
        1,
        'seed-user-1',
        'Khách liên hệ lại báo có nhu cầu mảnh tiện CNC',
        NOW() - INTERVAL '15 days'
    ),
    (
        2,
        1,
        2,
        'seed-user-1',
        'Đã cử nhân viên kỹ thuật qua đo kiểm phôi',
        NOW() - INTERVAL '8 days'
    ),
    -- Cơ hội 3 (Stage 1 -> 2 -> 3)
    (
        3,
        NULL,
        1,
        'seed-user-2',
        'Tiếp nhận yêu cầu từ bộ phận kỹ thuật xưởng',
        NOW() - INTERVAL '20 days'
    ),
    (
        3,
        1,
        2,
        'seed-user-2',
        'Đã hẹn khảo sát mẫu đá mài phẳng tại xưởng',
        NOW() - INTERVAL '12 days'
    ),
    (
        3,
        2,
        3,
        'seed-user-2',
        'Gửi bảng báo giá chi tiết BG-0001 qua email',
        NOW() - INTERVAL '5 days'
    ),
    -- Cơ hội 4 (Stage 1 -> 2 -> 3 -> 4)
    (
        4,
        NULL,
        1,
        'seed-user-2',
        'Nhận hồ sơ yêu cầu chào giá gói thầu bulong',
        NOW() - INTERVAL '25 days'
    ),
    (
        4,
        1,
        2,
        'seed-user-2',
        'Họp bàn điều khoản kỹ thuật và chứng chỉ xuất xứ Co/Cq',
        NOW() - INTERVAL '18 days'
    ),
    (
        4,
        2,
        3,
        'seed-user-2',
        'Nộp hồ sơ báo giá cạnh tranh BG-0002',
        NOW() - INTERVAL '10 days'
    ),
    (
        4,
        3,
        4,
        'seed-user-2',
        'Bắt đầu thương thảo chiết khấu thanh toán trả chậm',
        NOW() - INTERVAL '3 days'
    ),
    -- Cơ hội 5 (Stage 1 -> 2 -> 3 -> 4 -> 5 - Chốt thành công)
    (
        5,
        NULL,
        1,
        'seed-user-1',
        'Khách ghé lấy catalogue dầu làm mát',
        NOW() - INTERVAL '30 days'
    ),
    (
        5,
        1,
        2,
        'seed-user-1',
        'Cung cấp can mẫu 5 lít chạy thử nghiệm',
        NOW() - INTERVAL '22 days'
    ),
    (
        5,
        2,
        3,
        'seed-user-1',
        'Gửi báo giá combo dầu cắt gọt và mỡ bôi trơn',
        NOW() - INTERVAL '14 days'
    ),
    (
        5,
        3,
        4,
        'seed-user-1',
        'Thống nhất giao hàng trong vòng 24 giờ',
        NOW() - INTERVAL '7 days'
    ),
    (
        5,
        4,
        5,
        'seed-user-1',
        'Khách ký biên bản bàn giao và thanh toán tiền mặt',
        NOW() - INTERVAL '2 days'
    );

-- PHẦN 7: SEED 2 BÁO GIÁ (QUOTES) & DÒNG SẢN PHẨM (QUOTE_ITEMS)
INSERT INTO
    quotes (
        id,
        code,
        opportunity_id,
        customer_id,
        contact_id,
        created_by_user_id,
        quote_date,
        valid_until,
        status,
        sub_total,
        discount_amount,
        vat_rate,
        vat_amount,
        total_amount,
        payment_terms
    )
VALUES
    (
        1,
        'BG-0001',
        3,
        3,
        3,
        'seed-user-2',
        CURRENT_DATE - 5,
        CURRENT_DATE + 25,
        2,
        43500000.00,
        1500000.00,
        10.00,
        4200000.00,
        46200000.00,
        'Thanh toán chuyển khoản trong 30 ngày sau nghiệm thu'
    ),
    (
        2,
        'BG-0002',
        4,
        4,
        4,
        'seed-user-2',
        CURRENT_DATE - 3,
        CURRENT_DATE + 30,
        1,
        168000000.00,
        8000000.00,
        10.00,
        16000000.00,
        176000000.00,
        'Tạm ứng 30%, thanh toán 70% sau khi giao đủ chứng từ CO/CQ'
    );

SELECT
    setval(
        'quotes_id_seq',
        (
            SELECT
                COALESCE(MAX(id), 1)
            FROM
                quotes
        ),
        true
    );

INSERT INTO
    quote_items (
        id,
        quote_id,
        product_id,
        product_name,
        product_code,
        unit,
        quantity,
        unit_price,
        discount_amount,
        total_line,
        sort_order
    )
VALUES
    -- Chi tiết báo giá BG-0001
    (
        1,
        1,
        4,
        'Đá mài phẳng Norton 180x13x31.75',
        'DM-001',
        'Viên',
        100.00,
        185000.00,
        500000.00,
        18000000.00,
        1
    ),
    (
        2,
        1,
        6,
        'Bánh mài nỉ xám đánh bóng kim loại 150mm',
        'DM-003',
        'Cái',
        200.00,
        125000.00,
        1000000.00,
        24000000.00,
        2
    ),
    (
        3,
        1,
        5,
        'Đĩa cắt kim loại Hải Dương 105x1.2mm',
        'DM-002',
        'Viên',
        100.00,
        15000.00,
        0.00,
        1500000.00,
        3
    ),
    -- Chi tiết báo giá BG-0002
    (
        4,
        2,
        7,
        'Bulong lục giác ngoài M8x30 Inox 304',
        'OV-001',
        'Con',
        20000.00,
        5000.00,
        5000000.00,
        95000000.00,
        1
    ),
    (
        5,
        2,
        8,
        'Đai ốc thép cường độ cao M10 Grade 8.8',
        'OV-002',
        'Con',
        20000.00,
        3500.00,
        3000000.00,
        67000000.00,
        2
    ),
    (
        6,
        2,
        10,
        'Mỡ bôi trơn chịu nhiệt SKF LGMT 2/1',
        'DB-002',
        'Hộp',
        25.00,
        280000.00,
        0.00,
        7000000.00,
        3
    );

SELECT
    setval(
        'quote_items_id_seq',
        (
            SELECT
                COALESCE(MAX(id), 1)
            FROM
                quote_items
        ),
        true
    );

-- PHẦN 8: SEED 5 NHIỆM VỤ BÁN HÀNG (SALES_TASKS)
INSERT INTO
    sales_tasks (
        id,
        title,
        description,
        type,
        priority,
        status,
        customer_id,
        contact_id,
        opportunity_id,
        quote_id,
        due_date,
        assigned_to_user_id
    )
VALUES
    (
        1,
        'Gọi điện xác nhận tiến độ xem báo giá BG-0001',
        'Liên hệ anh Tuấn để giải đáp thắc mắc về đơn giá đá mài',
        3,
        2,
        0,
        3,
        3,
        3,
        1,
        NOW() + INTERVAL '1 day',
        'seed-user-2'
    ),
    (
        2,
        'Demo dao phay ngón HSS trực tiếp tại xưởng An Phát',
        'Mang mẫu dao phay chạy thử trên máy CNC Makino',
        2,
        3,
        0,
        1,
        1,
        1,
        NULL,
        NOW() + INTERVAL '2 days',
        'seed-user-1'
    ),
    (
        3,
        'Theo dõi kết quả phê duyệt hồ sơ thầu bulong',
        'Hỏi thăm anh Đức về biên bản mở thầu nội bộ',
        3,
        2,
        1,
        4,
        4,
        4,
        2,
        NOW() + INTERVAL '3 days',
        'seed-user-2'
    ),
    (
        4,
        'Đốc thúc thu hồi công nợ quá hạn nhà máy Nam An',
        'Gửi công văn đối chiếu công nợ 55 triệu quá hạn 45 ngày',
        5,
        3,
        0,
        4,
        4,
        NULL,
        NULL,
        NOW() + INTERVAL '12 hours',
        'seed-user-2'
    ),
    (
        5,
        'Gọi điện chăm sóc sau giao hàng mỡ SKF và dầu làm mát',
        'Khảo sát độ hài lòng của cơ sở Hải Nam sau 3 ngày dùng dầu',
        1,
        1,
        2,
        5,
        5,
        5,
        NULL,
        NOW() - INTERVAL '1 day',
        'seed-user-1'
    );

SELECT
    setval(
        'sales_tasks_id_seq',
        (
            SELECT
                COALESCE(MAX(id), 1)
            FROM
                sales_tasks
        ),
        true
    );

-- PHẦN 9: SEED 5 NHẬT KÝ TƯƠNG TÁC (INTERACTIONS)
INSERT INTO
    interactions (
        id,
        customer_id,
        contact_id,
        opportunity_id,
        user_id,
        type,
        content,
        duration_minutes,
        outcome,
        interacted_at
    )
VALUES
    (
        1,
        1,
        1,
        1,
        'seed-user-1',
        1,
        'Trao đổi qua điện thoại về kế hoạch thay dao cụ định kỳ Quý 4 của xưởng CNC',
        15,
        2,
        NOW() - INTERVAL '4 days'
    ),
    (
        2,
        2,
        2,
        2,
        'seed-user-1',
        4,
        'Nhắn tin qua Zalo gửi thông số kỹ thuật mảnh tiện Kyocera phủ PVD chống mài mòn',
        10,
        1,
        NOW() - INTERVAL '3 days'
    ),
    (
        3,
        3,
        3,
        3,
        'seed-user-2',
        2,
        'Gặp mặt trực tiếp tại KCN Tân Tạo để kiểm tra độ nhẵn bóng khuôn mẫu',
        60,
        3,
        NOW() - INTERVAL '2 days'
    ),
    (
        4,
        4,
        4,
        4,
        'seed-user-2',
        3,
        'Gửi email bản scan chứng chỉ xuất xứ Co/Cq của lô bulong M8x30 Inox',
        20,
        1,
        NOW() - INTERVAL '1 day'
    ),
    (
        5,
        5,
        5,
        5,
        'seed-user-1',
        5,
        'Trực tiếp ghé thăm xưởng bàn giao thùng dầu làm mát và hỗ trợ kỹ thuật pha dầu',
        45,
        1,
        NOW() - INTERVAL '6 hours'
    );

SELECT
    setval(
        'interactions_id_seq',
        (
            SELECT
                COALESCE(MAX(id), 1)
            FROM
                interactions
        ),
        true
    );

-- PHẦN 10: TRUY VẤN KIỂM TRA ĐỐI SOÁT DỮ LIỆU SEED
SELECT
    'Khách hàng (Customers)' AS bang,
    COUNT(*) AS so_luong
FROM
    customers
UNION
ALL
SELECT
    'Người liên hệ (Contacts)',
    COUNT(*)
FROM
    contacts
UNION
ALL
SELECT
    'Sản phẩm (Products)',
    COUNT(*)
FROM
    products
UNION
ALL
SELECT
    'Cơ hội bán hàng (Opportunities)',
    COUNT(*)
FROM
    opportunities
UNION
ALL
SELECT
    'Lịch sử giai đoạn (Stage Hist.)',
    COUNT(*)
FROM
    stage_histories
UNION
ALL
SELECT
    'Báo giá (Quotes)',
    COUNT(*)
FROM
    quotes
UNION
ALL
SELECT
    'Chi tiết báo giá (Quote Items)',
    COUNT(*)
FROM
    quote_items
UNION
ALL
SELECT
    'Nhiệm vụ bán hàng (Sales Tasks)',
    COUNT(*)
FROM
    sales_tasks
UNION
ALL
SELECT
    'Nhật ký tương tác (Interactions)',
    COUNT(*)
FROM
    interactions;

SELECT
    'Khách hàng (Customers)' AS bang,
    COUNT(*) AS so_luong
FROM
    customers
UNION
ALL
SELECT
    'Người liên hệ (Contacts)',
    COUNT(*)
FROM
    contacts
UNION
ALL
SELECT
    'Sản phẩm (Products)',
    COUNT(*)
FROM
    products
UNION
ALL
SELECT
    'Cơ hội bán hàng (Opportunities)',
    COUNT(*)
FROM
    opportunities
UNION
ALL
SELECT
    'Lịch sử giai đoạn (Stage Hist.)',
    COUNT(*)
FROM
    stage_histories
UNION
ALL
SELECT
    'Báo giá (Quotes)',
    COUNT(*)
FROM
    quotes
UNION
ALL
SELECT
    'Chi tiết báo giá (Quote Items)',
    COUNT(*)
FROM
    quote_items
UNION
ALL
SELECT
    'Nhiệm vụ bán hàng (Sales Tasks)',
    COUNT(*)
FROM
    sales_tasks
UNION
ALL
SELECT
    'Nhật ký tương tác (Interactions)',
    COUNT(*)
FROM
    interactions;