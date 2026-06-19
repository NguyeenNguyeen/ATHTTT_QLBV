

GRANT SELECT ON SYS.DBA_AUDIT_TRAIL TO ADMIN_PHANHE1;
GRANT SELECT ON AUDSYS.UNIFIED_AUDIT_TRAIL TO ADMIN_PHANHE1;

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
-- AUDIT SELECT ON V_HSBA_BACSI BY ACCESS;

-- =====================================================================
-- NGỮ CẢNH 4: Giám sát Thủ tục (Stored Procedure / Function)
-- Ý nghĩa: Giám sát việc thực thi các thủ tục có tính rủi ro cao. 
-- Ví dụ: Giám sát xem ai đã chạy thủ tục tạo hồ sơ bệnh án mới.
-- =====================================================================
-- Lưu ý: Đổi 'SP_TAO_HSBA' thành tên Procedure thực tế của nhóm.
-- AUDIT EXECUTE ON SP_TAO_HSBA BY ACCESS;

-- =====================================================================
-- NGỮ CẢNH 5: Giám sát Cấu trúc (DDL) - Bảo vệ Schema
-- Ý nghĩa: Phát hiện các hành vi cố tình thay đổi cấu trúc bảng (CREATE, ALTER, DROP, TRUNCATE) 
-- =====================================================================
AUDIT TABLE BY ACCESS;




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


-- CHỈ CẦN 1 TRANG ĐỂ SELECT * FROM VIEW ADMIN_PHANHE1.V_ALL_AUDIT_LOG ĐỂ XEM TẤT CẢ CÁC LOGS

CREATE OR REPLACE VIEW ADMIN_PHANHE1.V_ALL_AUDIT_LOG AS
SELECT 
    "LOAI_AUDIT",
    "NGUOI_DUNG",
    "THOI_GIAN",
    "HANH_DONG",
    "DOI_TUONG",
    "CAU_LENH_SQL",
    "CHI_TIET_TRANG_THAI"
FROM (
    -- =========================================================
    -- NỬA TRÊN: DỮ LIỆU TỪ STANDARD AUDIT
    -- =========================================================
    SELECT 
        'STANDARD' AS "LOAI_AUDIT",
        USERNAME AS "NGUOI_DUNG",
        -- Định dạng giống Flashback: YYYY/MM/DD HH24:MI:SS
        TO_CHAR(EXTENDED_TIMESTAMP, 'YYYY/MM/DD HH24:MI:SS') AS "THOI_GIAN",
        ACTION_NAME AS "HANH_DONG",
        OWNER || '.' || OBJ_NAME AS "DOI_TUONG",
        CAST(SQL_TEXT AS VARCHAR2(2000)) AS "CAU_LENH_SQL", 
        CASE RETURNCODE 
            WHEN 0 THEN 'Thành công' 
            ELSE 'Thất bại (Mã lỗi: ' || RETURNCODE || ')' 
        END AS "CHI_TIET_TRANG_THAI",
        CAST(EXTENDED_TIMESTAMP AS TIMESTAMP) AS RAW_TIME 
    FROM 
        DBA_AUDIT_TRAIL
    WHERE 
        USERNAME NOT IN ('SYS', 'SYSTEM', 'DBSNMP', 'SYSMAN') 

    UNION ALL

    -- =========================================================
    -- NỬA DƯỚI: DỮ LIỆU TỪ FINE-GRAINED AUDIT (FGA)
    -- =========================================================
    SELECT 
        'FINE-GRAINED' AS "LOAI_AUDIT",
        DBUSERNAME AS "NGUOI_DUNG",
        -- Đã đồng bộ định dạng giống Flashback: YYYY/MM/DD HH24:MI:SS
        TO_CHAR(EVENT_TIMESTAMP, 'YYYY/MM/DD HH24:MI:SS') AS "THOI_GIAN",
        'POLICY: ' || FGA_POLICY_NAME AS "HANH_DONG",
        OBJECT_SCHEMA || '.' || OBJECT_NAME AS "DOI_TUONG",
        CAST(SQL_TEXT AS VARCHAR2(2000)) AS "CAU_LENH_SQL",
        CASE 
            WHEN FGA_POLICY_NAME LIKE '%BATHOPPHAP%' THEN 'Cảnh báo Đỏ: Hành vi bất hợp pháp'
            WHEN FGA_POLICY_NAME LIKE '%HOPPHAP%' THEN 'Bình thường: Cập nhật đúng thẩm quyền'
            ELSE 'Theo dõi hệ thống'
        END AS "CHI_TIET_TRANG_THAI",
        CAST(EVENT_TIMESTAMP AS TIMESTAMP) AS RAW_TIME 
    FROM 
        UNIFIED_AUDIT_TRAIL
    WHERE 
        FGA_POLICY_NAME IS NOT NULL 
        AND DBUSERNAME NOT IN ('SYS', 'SYSTEM')
        AND (
            (FGA_POLICY_NAME = 'FGA_HSBA_CAPNHAT_HOPPHAP' AND RETURN_CODE = 0)
            OR 
            (FGA_POLICY_NAME != 'FGA_HSBA_CAPNHAT_HOPPHAP')
        )
)
-- Sắp xếp bằng dữ liệu thời gian thật, dữ liệu đổ ra UI sẽ chuẩn xác tuyệt đối
ORDER BY RAW_TIME DESC;

select * from admin_phanhe1.v_all_audit_log;

