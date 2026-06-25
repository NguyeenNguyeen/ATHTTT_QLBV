-- 1. Đảm bảo đứng đúng Pluggable Database cục bộ
ALTER SESSION SET CONTAINER = ORCLPDB1;
ALTER SESSION SET CURRENT_SCHEMA = SYS;

-- Vá lỗi đặc quyền phân tầng hệ thống
GRANT INHERIT PRIVILEGES ON USER SYS TO LBACSYS;

-- =====================================================================
-- BƯỚC 0: DỌN DẸP HỆ THỐNG CŨ
-- =====================================================================
ALTER SESSION SET "_ORACLE_SCRIPT" = true;

DECLARE v_count NUMBER;
BEGIN
    SELECT COUNT(*) INTO v_count FROM dba_sa_policies WHERE policy_name = 'OLS_BV';
    IF v_count > 0 THEN
        BEGIN SA_POLICY_ADMIN.REMOVE_TABLE_POLICY('OLS_BV', 'ADMIN_PHANHE1', 'THONGBAO'); EXCEPTION WHEN OTHERS THEN NULL; END;
        SA_SYSDBA.DROP_POLICY('OLS_BV', TRUE);
    END IF;
EXCEPTION WHEN OTHERS THEN NULL;
END;
/

BEGIN
    FOR i IN 1..8 LOOP
        FOR s IN (SELECT sid, serial# FROM v$session WHERE username = 'U' || i) LOOP
            EXECUTE IMMEDIATE 'ALTER SYSTEM KILL SESSION ''' || s.sid || ',' || s.serial# || ''' IMMEDIATE';
        END LOOP;
        BEGIN EXECUTE IMMEDIATE 'DROP USER U' || i || ' CASCADE'; EXCEPTION WHEN OTHERS THEN NULL; END;
    END LOOP;
    BEGIN EXECUTE IMMEDIATE 'DROP TABLE ADMIN_PHANHE1.THONGBAO CASCADE CONSTRAINTS'; EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN EXECUTE IMMEDIATE 'DROP USER ADMIN_PHANHE1 CASCADE'; EXCEPTION WHEN OTHERS THEN NULL; END;
END;
/

-- =====================================================================
-- BƯỚC TIỀN ĐỀ: TẮT CỜ SCRIPT ĐỂ TẠO ĐỐI TƯỢNG LOCAL (SỬA LỖI ORA-42901)
-- =====================================================================
ALTER SESSION SET "_ORACLE_SCRIPT" = false;

CREATE USER ADMIN_PHANHE1 IDENTIFIED BY "Admin@123456" DEFAULT TABLESPACE USERS;
ALTER USER ADMIN_PHANHE1 QUOTA UNLIMITED ON USERS;
GRANT DBA TO ADMIN_PHANHE1;

BEGIN
    FOR i IN 1..8 LOOP
        EXECUTE IMMEDIATE 'CREATE USER U' || i || ' IDENTIFIED BY "User@123"';
        EXECUTE IMMEDIATE 'GRANT CREATE SESSION TO U' || i;
    END LOOP;
END;
/

-- =====================================================================
-- BƯỚC 1: TẠO BẢNG LOCAL VÀ CHÈN DỮ LIỆU CŨ
-- =====================================================================
CREATE TABLE ADMIN_PHANHE1.THONGBAO (
    MATB VARCHAR2(20) PRIMARY KEY,
    NOIDUNG NVARCHAR2(1000),
    NGAYGIO TIMESTAMP,
    DIADIEM NVARCHAR2(100)
);

GRANT SELECT ON ADMIN_PHANHE1.THONGBAO TO U1, U2, U3, U4, U5, U6, U7, U8;

INSERT INTO ADMIN_PHANHE1.THONGBAO VALUES ('t1', N'Đây là thông báo gửi đến toàn bộ nhân viên', SYSTIMESTAMP, N'Hội trường A');
INSERT INTO ADMIN_PHANHE1.THONGBAO VALUES ('t2', N'Đây là thông báo gửi đến toàn bộ Ban giám đốc', SYSTIMESTAMP, N'Phòng họp VIP');
INSERT INTO ADMIN_PHANHE1.THONGBAO VALUES ('t3', N'Đây là thông báo gửi đến các lãnh đạo khoa', SYSTIMESTAMP, N'Phòng 201');
INSERT INTO ADMIN_PHANHE1.THONGBAO VALUES ('t4', N'Đây là thông báo gửi đến lãnh đạo Khoa tiêu hóa', SYSTIMESTAMP, N'Phòng Khoa TH');
INSERT INTO ADMIN_PHANHE1.THONGBAO VALUES ('t5', N'Đây là thông báo gửi đến nhân viên Khoa tiêu hóa ở Hồ Chí Minh', SYSTIMESTAMP, N'Cơ sở HCM');
INSERT INTO ADMIN_PHANHE1.THONGBAO VALUES ('t6', N'Đây là thông báo gửi đến nhân viên Khoa tiêu hóa ở Hà Nội', SYSTIMESTAMP, N'Cơ sở HN');
INSERT INTO ADMIN_PHANHE1.THONGBAO VALUES ('t7', N'Đây là thông báo gửi đến lãnh đạo Khoa tiêu hóa và Khoa thần kinh tại Hải Phòng', SYSTIMESTAMP, N'Cơ sở HP');
COMMIT;

-- =====================================================================
-- BƯỚC 2: TẠO POLICY VÀ THÀNH PHẦN OLS
-- =====================================================================
EXEC SA_SYSDBA.CREATE_POLICY(policy_name => 'OLS_BV', column_name => 'OLS_LABEL');

-- Levels
EXEC SA_COMPONENTS.CREATE_LEVEL('OLS_BV', 30, 'GD', 'Ban Giam Doc');
EXEC SA_COMPONENTS.CREATE_LEVEL('OLS_BV', 20, 'LD', 'Lanh Dao Khoa');
EXEC SA_COMPONENTS.CREATE_LEVEL('OLS_BV', 10, 'NV', 'Nhan Vien');

-- Compartments
EXEC SA_COMPONENTS.CREATE_COMPARTMENT('OLS_BV', 100, 'TH', 'Khoa Tieu Hoa');
EXEC SA_COMPONENTS.CREATE_COMPARTMENT('OLS_BV', 110, 'TK', 'Khoa Than Kinh');
EXEC SA_COMPONENTS.CREATE_COMPARTMENT('OLS_BV', 120, 'TM', 'Khoa Tim Mach');

-- Groups (SỬA ĐỔI: Chuyển dịch sang mô hình Phân cấp Parent-Child chuẩn)
EXEC SA_COMPONENTS.CREATE_GROUP('OLS_BV', 100, 'TOAN_VIEN', 'Toan Bo Benh Vien');
EXEC SA_COMPONENTS.CREATE_GROUP('OLS_BV', 10,  'HCM',       'Co So Ho Chi Minh', 'TOAN_VIEN');
EXEC SA_COMPONENTS.CREATE_GROUP('OLS_BV', 20,  'HN',        'Co So Ha Noi',      'TOAN_VIEN');
EXEC SA_COMPONENTS.CREATE_GROUP('OLS_BV', 30,  'HP',        'Co So Hai Phong',   'TOAN_VIEN');

-- =====================================================================
-- BƯỚC 3: ĐĂNG KÝ DANH SÁCH CHUỖI NHÃN HỢP LỆ
-- =====================================================================
EXEC SA_LABEL_ADMIN.CREATE_LABEL('OLS_BV', 1001, 'NV');
EXEC SA_LABEL_ADMIN.CREATE_LABEL('OLS_BV', 1002, 'GD');
EXEC SA_LABEL_ADMIN.CREATE_LABEL('OLS_BV', 1003, 'LD');
EXEC SA_LABEL_ADMIN.CREATE_LABEL('OLS_BV', 1004, 'LD:TH');
EXEC SA_LABEL_ADMIN.CREATE_LABEL('OLS_BV', 1005, 'NV:TH:HCM');
EXEC SA_LABEL_ADMIN.CREATE_LABEL('OLS_BV', 1006, 'NV:TH:HN');
EXEC SA_LABEL_ADMIN.CREATE_LABEL('OLS_BV', 1007, 'LD:TH,TK:HP');

-- =====================================================================
-- BƯỚC 4: GÁN QUYỀN CHO NGƯỜI DÙNG U1 -> U8 (SỬA ĐỔI: Ăn theo group phân cấp)
-- =====================================================================
-- U1: Giám đốc toàn viện
EXEC SA_USER_ADMIN.SET_LEVELS('OLS_BV', 'U1', 'GD', 'NV', 'GD', 'GD');
EXEC SA_USER_ADMIN.SET_COMPARTMENTS('OLS_BV', 'U1', 'TH,TK,TM', 'TH,TK,TM', 'TH,TK,TM', 'TH,TK,TM');
EXEC SA_USER_ADMIN.SET_GROUPS('OLS_BV', 'U1', 'TOAN_VIEN', 'TOAN_VIEN', 'TOAN_VIEN', 'TOAN_VIEN');

-- U2: Lãnh đạo Khoa tim mạch tại Hồ Chí Minh
EXEC SA_USER_ADMIN.SET_LEVELS('OLS_BV', 'U2', 'LD', 'NV', 'LD', 'LD');
EXEC SA_USER_ADMIN.SET_COMPARTMENTS('OLS_BV', 'U2', 'TM', 'TM', 'TM', 'TM');
EXEC SA_USER_ADMIN.SET_GROUPS('OLS_BV', 'U2', 'HCM', 'HCM', 'HCM', 'HCM');

-- U3: Lãnh đạo Khoa thần kinh tại Hà Nội
EXEC SA_USER_ADMIN.SET_LEVELS('OLS_BV', 'U3', 'LD', 'NV', 'LD', 'LD');
EXEC SA_USER_ADMIN.SET_COMPARTMENTS('OLS_BV', 'U3', 'TK', 'TK', 'TK', 'TK');
EXEC SA_USER_ADMIN.SET_GROUPS('OLS_BV', 'U3', 'HN', 'HN', 'HN', 'HN');

-- U4: Nhân viên thuộc Khoa thần kinh tại Hồ Chí Minh
EXEC SA_USER_ADMIN.SET_LEVELS('OLS_BV', 'U4', 'NV', 'NV', 'NV', 'NV');
EXEC SA_USER_ADMIN.SET_COMPARTMENTS('OLS_BV', 'U4', 'TK', 'TK', 'TK', 'TK');
EXEC SA_USER_ADMIN.SET_GROUPS('OLS_BV', 'U4', 'HCM', 'HCM', 'HCM', 'HCM');

-- U5: Nhân viên thuộc Khoa tim mạch tại Hồ Chí Minh
EXEC SA_USER_ADMIN.SET_LEVELS('OLS_BV', 'U5', 'NV', 'NV', 'NV', 'NV');
EXEC SA_USER_ADMIN.SET_COMPARTMENTS('OLS_BV', 'U5', 'TM', 'TM', 'TM', 'TM');
EXEC SA_USER_ADMIN.SET_GROUPS('OLS_BV', 'U5', 'HCM', 'HCM', 'HCM', 'HCM');

-- U6: Lãnh đạo phòng xem thông báo Khoa tim mạch tại HCM
EXEC SA_USER_ADMIN.SET_LEVELS('OLS_BV', 'U6', 'LD', 'NV', 'LD', 'LD');
EXEC SA_USER_ADMIN.SET_COMPARTMENTS('OLS_BV', 'U6', 'TM', 'TM', 'TM', 'TM');
EXEC SA_USER_ADMIN.SET_GROUPS('OLS_BV', 'U6', 'HCM', 'HCM', 'HCM', 'HCM');

-- U7: Lãnh đạo phòng xem toàn bộ thông báo phù hợp cấp bậc
EXEC SA_USER_ADMIN.SET_LEVELS('OLS_BV', 'U7', 'LD', 'NV', 'LD', 'LD');
EXEC SA_USER_ADMIN.SET_COMPARTMENTS('OLS_BV', 'U7', 'TH,TK,TM', 'TH,TK,TM', 'TH,TK,TM', 'TH,TK,TM');
EXEC SA_USER_ADMIN.SET_GROUPS('OLS_BV', 'U7', 'TOAN_VIEN', 'TOAN_VIEN', 'TOAN_VIEN', 'TOAN_VIEN');
-- U8: Nhân viên thuộc Khoa Tiêu hóa tại Hà Nội
EXEC SA_USER_ADMIN.SET_LEVELS('OLS_BV', 'U8', 'NV', 'NV', 'NV', 'NV');
EXEC SA_USER_ADMIN.SET_COMPARTMENTS('OLS_BV', 'U8', 'TH', 'TH', 'TH', 'TH');
EXEC SA_USER_ADMIN.SET_GROUPS('OLS_BV', 'U8', 'HN', 'HN', 'HN', 'HN');

-- =====================================================================
-- BƯỚC 5: ÁP DỤNG CHÍNH SÁCH ĐỆM NO_CONTROL (SỬA LỖI ORA-00904 CỐT LÕI)
-- =====================================================================
-- Phải chạy lệnh này trước để Oracle tạo cột ẩn OLS_LABEL vào bảng
BEGIN
    SA_POLICY_ADMIN.APPLY_TABLE_POLICY(
        policy_name    => 'OLS_BV',
        schema_name    => 'ADMIN_PHANHE1',
        table_name     => 'THONGBAO',
        table_options  => 'NO_CONTROL'
    );
END;
/

-- =====================================================================
-- BƯỚC 6: CẬP NHẬT NHÃN DỮ LIỆU CŨ (Chạy mượt mà vì cột đã tồn tại)
-- =====================================================================
UPDATE ADMIN_PHANHE1.THONGBAO SET OLS_LABEL = CHAR_TO_LABEL('OLS_BV', 'NV') WHERE MATB = 't1';
UPDATE ADMIN_PHANHE1.THONGBAO SET OLS_LABEL = CHAR_TO_LABEL('OLS_BV', 'GD') WHERE MATB = 't2';
UPDATE ADMIN_PHANHE1.THONGBAO SET OLS_LABEL = CHAR_TO_LABEL('OLS_BV', 'LD') WHERE MATB = 't3';
UPDATE ADMIN_PHANHE1.THONGBAO SET OLS_LABEL = CHAR_TO_LABEL('OLS_BV', 'LD:TH') WHERE MATB = 't4';
UPDATE ADMIN_PHANHE1.THONGBAO SET OLS_LABEL = CHAR_TO_LABEL('OLS_BV', 'NV:TH:HCM') WHERE MATB = 't5';
UPDATE ADMIN_PHANHE1.THONGBAO SET OLS_LABEL = CHAR_TO_LABEL('OLS_BV', 'NV:TH:HN') WHERE MATB = 't6';
UPDATE ADMIN_PHANHE1.THONGBAO SET OLS_LABEL = CHAR_TO_LABEL('OLS_BV', 'LD:TH,TK:HP') WHERE MATB = 't7';
COMMIT;

-- =====================================================================
-- BƯỚC 7: NÂNG CẤP LÊN HÀNG RÀO KIỂM SOÁT TOÀN DIỆN (FULL ENFORCEMENT)
-- =====================================================================
EXEC SA_POLICY_ADMIN.REMOVE_TABLE_POLICY('OLS_BV', 'ADMIN_PHANHE1', 'THONGBAO');

BEGIN
    SA_POLICY_ADMIN.APPLY_TABLE_POLICY(
        policy_name    => 'OLS_BV',
        schema_name    => 'ADMIN_PHANHE1',
        table_name     => 'THONGBAO',
        table_options  => 'READ_CONTROL,WRITE_CONTROL,CHECK_CONTROL'
    );
END;
/

-- Kích hoạt đồng bộ hóa bộ nhớ đệm chính sách
EXEC SA_POLICY_ADMIN.ENABLE_TABLE_POLICY('OLS_BV', 'ADMIN_PHANHE1', 'THONGBAO');
