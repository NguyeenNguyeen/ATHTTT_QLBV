
-- =====================================================================
-- STANDARD AUDIT
-- =====================================================================

-- xem cấu hình audit hiện tại có DB_EXTENDED chưa, chưa thì đổi bằng dòng ALTER bên dưới
SHOW PARAMETER audit_trail;

-- Nếu audit_trail chưa được bật hoặc không phải là DB hoặc DB, EXTENDED thì cần bật bằng câu lệnh sau:
ALTER SYSTEM SET audit_trail = DB, EXTENDED SCOPE = SPFILE;
-- Cần tắt và khởi động lại database để thay đổi có hiệu lực:
-- SHUTDOWN IMMEDIATE;
-- STARTUP;

-- =====================================================================
-- NGỮ CẢNH 1: Giám sát Hệ thống - Đăng nhập thất bại (Session)
-- Ý nghĩa: Phát hiện các cuộc tấn công Brute-force dò mật khẩu vào hệ thống.
-- =====================================================================
AUDIT SESSION WHENEVER NOT SUCCESSFUL;

-- =====================================================================
-- NGỮ CẢNH 2: Giám sát Bảng (Table) - Thay đổi dữ liệu hồ sơ bệnh án
-- Ý nghĩa: Bảng HSBA là dữ liệu cốt lõi. Giám sát mọi hành vi Thêm/Xóa/Sửa 
-- (cả thành công lẫn thất bại) trên bảng này.
-- =====================================================================
AUDIT INSERT, UPDATE, DELETE ON ADMIN_PHANHE1.HSBA BY ACCESS;

-- =====================================================================
-- NGỮ CẢNH 3: Giám sát View - Truy xuất dữ liệu nhạy cảm
-- Ý nghĩa: Giám sát hành vi truy vấn (SELECT) trên View hồ sơ bệnh án 
-- (Giả sử bạn có 1 view tên là V_HSBA_BACSI để bác sĩ xem hồ sơ).
-- Ghi log mỗi khi có người đọc dữ liệu này.
-- =====================================================================
-- Lưu ý: Đổi 'V_HSBA_BACSI' thành tên View thực tế của nhóm.
AUDIT SELECT ON V_HSBA_BACSI BY ACCESS;

-- =====================================================================
-- NGỮ CẢNH 4: Giám sát Thủ tục (Stored Procedure / Function)
-- Ý nghĩa: Giám sát việc thực thi các thủ tục có tính rủi ro cao. 
-- Ví dụ: Giám sát xem ai đã chạy thủ tục tạo hồ sơ bệnh án mới.
-- =====================================================================
-- Lưu ý: Đổi 'SP_TAO_HSBA' thành tên Procedure thực tế của nhóm.
AUDIT EXECUTE ON SP_TAO_HSBA BY ACCESS;

-- =====================================================================
-- NGỮ CẢNH 5: Giám sát Cấu trúc (DDL) - Bảo vệ Schema
-- Ý nghĩa: Phát hiện các hành vi cố tình thay đổi cấu trúc bảng (CREATE, ALTER, DROP, TRUNCATE) 
-- =====================================================================
AUDIT TABLE BY ACCESS;

-- view tổng hợp log audit chuẩn hóa để UI dễ đọc hơn
CREATE OR REPLACE VIEW V_STANDARD_AUDIT_LOG AS
SELECT 
    USERNAME AS "NGUOI_DUNG",               -- Ai đã thực hiện?
    TO_CHAR(EXTENDED_TIMESTAMP, 'DD/MM/YYYY HH24:MI:SS') AS "THOI_GIAN", -- Khi nào?
    ACTION_NAME AS "HANH_DONG",             -- Làm gì? (SELECT, UPDATE, LOGON...)
    OWNER AS "CHU_SO_HUU",                  -- Schema nào?
    OBJ_NAME AS "DOI_TUONG",                -- Tác động lên Bảng/View/SP nào?
    SQL_TEXT AS "CAU_LENH_SQL",             -- Câu SQL thực tế đã chạy là gì?
    CASE RETURNCODE 
        WHEN 0 THEN 'Thành công' 
        ELSE 'Thất bại (Mã lỗi: ' || RETURNCODE || ')' 
    END AS "TRANG_THAI"                     -- Thành công hay thất bại?
FROM 
    DBA_AUDIT_TRAIL
WHERE 
    -- Lọc bỏ các log hệ thống nội bộ của Oracle để UI không bị rác
    USERNAME NOT IN ('SYS', 'SYSTEM', 'DBSNMP', 'SYSMAN') 
ORDER BY 
    EXTENDED_TIMESTAMP DESC;

select * from V_STANDARD_AUDIT_LOG;



-- =====================================================================
-- FINE-GRAINED AUDIT
-- =====================================================================

BEGIN
    -- =====================================================================
    -- 3a. Giám sát Bác sĩ cập nhật ĐƠN THUỐC sau khi đã chỉ định
    -- Ý nghĩa: Bất kỳ lệnh UPDATE nào trên các cột cấu thành đơn thuốc đều bị ghi log.
    -- =====================================================================
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN_PHANHE1',
        object_name     => 'DONTHUOC',
        policy_name     => 'FGA_DONTHUOC_CAPNHAT_SAUDINH',
        audit_column    => 'MAHSBA, NGAYDT, TENTHUOC, LIEUDUNG',
        -- ĐIỀU KIỆN BAO GỒM 3 LỚP:
        -- 1. Phải là người có Role Bác sĩ.
        -- 2. Loại trừ Admin/DBA.
        -- 3. SUBQUERY: Kiểm tra user đang đăng nhập có đúng là bác sĩ phụ trách cái MAHSBA của đơn thuốc này không.
        audit_condition => '
            SYS_CONTEXT(''SYS_SESSION_ROLES'', ''ROLE_YSI_BACSI'') = ''TRUE'' 
            AND EXISTS (
                SELECT 1 
                FROM ADMIN_PHANHE1.HSBA h 
                WHERE h.MAHSBA = MAHSBA 
                AND ''C##'' || UPPER(h.MABS) = SYS_CONTEXT(''USERENV'', ''SESSION_USER'')
            )',
        statement_types => 'UPDATE'
    );

    -- =====================================================================
    -- 3b. Giám sát hành vi Bác sĩ cập nhật HỢP PHÁP trên HSBA
    -- Ý nghĩa: Bác sĩ điều trị (MABS) tự cập nhật Chẩn đoán/Điều trị/Kết luận 
    -- cho chính hồ sơ mà mình phụ trách.
    -- (Giả định user đăng nhập có dạng C##NV004 và MABS lưu là NV004)
    -- =====================================================================
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN_PHANHE1',
        object_name     => 'HSBA',
        policy_name     => 'FGA_HSBA_CAPNHAT_HOPPHAP',
        audit_column    => 'CHANDOAN, DIEUTRI, KETLUAN',
        -- Điều kiện: Tên user đăng nhập TRÙNG khớp với mã Bác sĩ phụ trách hồ sơ
        audit_condition => '
        SYS_CONTEXT(''SYS_SESSION_ROLES'', ''ROLE_YSI_BACSI'') = ''TRUE''
        AND SYS_CONTEXT(''USERENV'', ''SESSION_USER'') = ''C##'' || UPPER(MABS)',
        statement_types => 'UPDATE'
    );

    -- =====================================================================
    -- 3c. Giám sát hành vi cập nhật BẤT HỢP PHÁP trên HSBA
    -- Ý nghĩa: Một người không phải là Bác sĩ phụ trách hồ sơ đó (hoặc KTV/Điều phối viên)
    -- lén sửa Chẩn đoán/Điều trị/Kết luận.
    -- =====================================================================
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN_PHANHE1',
        object_name     => 'HSBA',
        policy_name     => 'FGA_HSBA_CAPNHAT_BATHOPPHAP',
        audit_column    => 'CHANDOAN, DIEUTRI, KETLUAN',
        -- Điều kiện: Tên user KHÔNG TRÙNG với mã Bác sĩ phụ trách VÀ không phải DBA
        audit_condition => 'SYS_CONTEXT(''USERENV'', ''SESSION_USER'') != ''C##'' || UPPER(MABS)',
        statement_types => 'UPDATE'
    );

    -- =====================================================================
    -- 3d. Giám sát hành vi Thêm/Xóa/Sửa BẤT HỢP PHÁP trên HSBA_DV
    -- Ý nghĩa: Lưu vết mọi hành vi can thiệp vào bảng Dịch vụ khi người đó 
    -- không phải là người có quyền cao nhất (DBA). Lỗi báo cấm quyền (RBAC/VPD) 
    -- sẽ được ném ra ở UI, nhưng log hệ thống vẫn sẽ tóm được hành vi này.
    -- =====================================================================

    -- =====================================================================
    -- 3d.1. Bắt lỗi INSERT, DELETE bất hợp pháp trên HSBA_DV
    -- Logic: Chỉ Bác sĩ (và Admin) mới được quyền Thêm/Xóa. 
    -- Nếu người làm KHÔNG PHẢI Bác sĩ (ví dụ KTV, ĐPV lén xóa) -> Ghi log!
    -- =====================================================================
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN_PHANHE1',
        object_name     => 'HSBA_DV',
        policy_name     => 'FGA_HSBADV_INS_DEL_BATHOPPHAP',
        -- Điều kiện: Trả về TRUE (ghi log) nếu KHÔNG có Role Bác sĩ VÀ không phải Admin
        audit_condition => 'SYS_CONTEXT(''SYS_SESSION_ROLES'', ''ROLE_YSI_BACSI'') = ''FALSE''',
        statement_types => 'INSERT, DELETE'
    );

    -- =====================================================================
    -- 3d.2. Bắt lỗi UPDATE bất hợp pháp lên các cột "Cấm sửa"
    -- Logic: Bác sĩ chỉ được thêm/xóa (không được sửa). KTV chỉ được sửa cột KETQUA.
    -- Vậy nếu có ai đó SỬA các cột gốc (MAHSBA, MADV, NGAY, MAKTV...) thì 100% là bất hợp pháp.
    -- (Giả định bảng có các cột này, bạn có thể điều chỉnh tên cột cho khớp đồ án)
    -- =====================================================================
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN_PHANHE1',
        object_name     => 'HSBA_DV',
        policy_name     => 'FGA_HSBADV_UPD_COT_CAM_BATHOPPHAP',
        audit_column    => 'MAHSBA, MADV, NGAY, MAKTV', -- Liệt kê TẤT CẢ CÁC CỘT TRỪ cột KETQUA
        -- Điều kiện: Cứ sửa các cột này là bắt lỗi tất cả mọi người 
        audit_condition => NULL,
        statement_types => 'UPDATE'
    );

    -- =====================================================================
    -- 3d.3. Bắt lỗi UPDATE bất hợp pháp lên cột KETQUA
    -- Logic: Chỉ KTV mới được quyền sửa cột KẾT QUẢ.
    -- Nếu người sửa KHÔNG PHẢI KTV (ví dụ Bác sĩ, ĐPV lén ghi kết quả khống) -> Ghi log!
    -- =====================================================================
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN_PHANHE1',
        object_name     => 'HSBA_DV',
        policy_name     => 'FGA_HSBADV_UPD_KETQUA_BATHOPPHAP',
        audit_column    => 'KETQUA',
        -- Điều kiện: Trả về TRUE (ghi log) nếu KHÔNG có Role KTV VÀ không phải Admin
        audit_condition => 'SYS_CONTEXT(''USERENV'', ''SESSION_USER'') != ''C##'' || UPPER(MAKTV)',
        statement_types => 'UPDATE'
    );

    -- HO TRO
    DBMS_FGA.ADD_POLICY(
        object_schema => 'ADMIN_PHANHE1',
        object_name => 'HSBA_DV',
        policy_name => 'FGA_HSBADV_KTV_CAPNHAT_HOPPHAP',
        audit_column => 'KETQUA',
        audit_condition => 'SYS_CONTEXT(''SYS_SESSION_ROLE'', ''ROLE_KYTHUATVIEN'') = ''TRUE''
        AND SYS_CONTEXT(''USERENV'', ''SESSION_USER'') = ''C##'' || UPPER(MAKTV)',
        statement_types => 'UPDATE'
    );   
END;
/

-- view tổng hợp log audit chuẩn hóa để UI dễ đọc hơn
CREATE OR REPLACE VIEW V_FGA_LOG AS
SELECT 
    DBUSERNAME AS "NGUOI_DUNG",
    TO_CHAR(EVENT_TIMESTAMP, 'DD/MM/YYYY HH24:MI:SS') AS "THOI_GIAN",
    FGA_POLICY_NAME AS "CHINH_SACH_KIEM_TOAN",
    OBJECT_SCHEMA || '.' || OBJECT_NAME AS "BANG_BI_TAC_DONG",
    SQL_TEXT AS "CAU_LENH_SQL",
    CASE 
        WHEN FGA_POLICY_NAME LIKE '%BATHOPPHAP%' THEN 'Cảnh báo Đỏ: Hành vi bất hợp pháp'
        WHEN FGA_POLICY_NAME LIKE '%HOPPHAP%' THEN 'Bình thường: Cập nhật đúng thẩm quyền'
        ELSE 'Theo dõi hệ thống'
    END AS "PHAN_LOAI_CANH_BAO"
FROM 
    UNIFIED_AUDIT_TRAIL
WHERE 
    FGA_POLICY_NAME IS NOT NULL 
    AND DBUSERNAME NOT IN ('SYS', 'SYSTEM')
    -- Điểm mấu chốt để bắt lỗi câu 3b nằm ở đây:
    AND (
        (FGA_POLICY_NAME = 'FGA_HSBA_CAPNHAT_HOPPHAP' AND RETURN_CODE = 0) -- Chỉ lấy dòng thành công
        OR 
        (FGA_POLICY_NAME != 'FGA_HSBA_CAPNHAT_HOPPHAP') -- Các policy khác lấy hết (thành công/thất bại)
    )
ORDER BY 
    EVENT_TIMESTAMP DESC;



