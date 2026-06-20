ALTER SESSION SET CURRENT_SCHEMA = SYS; -- SYS.V_$SESSION

-- Bỏ qua lớp container bảo mật của Oracle 12c+ để tạo user local dễ dàng
ALTER SESSION SET "_ORACLE_SCRIPT"=true; 

-- XOÁ THIẾT LẬP FLASHBACK LÊN CÁC BẢNG ĐƯỢC AUDIT -> ĐỂ CÓ THỂ XOÁ ĐƯỢC USER ADMIN
ALTER TABLE ADMIN_PHANHE1.HSBA NO FLASHBACK ARCHIVE;
ALTER TABLE ADMIN_PHANHE1.HSBA_DV NO FLASHBACK ARCHIVE;
ALTER TABLE ADMIN_PHANHE1.DONTHUOC NO FLASHBACK ARCHIVE;

DROP FLASHBACK ARCHIVE fda_phanhe1;


DROP USER ADMIN_PHANHE1 CASCADE;
/



-- Tạo user dùng chung cho cả nhóm
CREATE USER ADMIN_PHANHE1 IDENTIFIED BY "Admin@123456" DEFAULT TABLESPACE USERS;

ALTER USER ADMIN_PHANHE1 QUOTA UNLIMITED ON USERS;

-- Cấp toàn quyền quản trị (DBA)
GRANT DBA TO ADMIN_PHANHE1;
GRANT ALTER SYSTEM TO ADMIN_PHANHE1;
GRANT SELECT ON v_$session TO ADMIN_PHANHE1;

-- Cấp quyền cứng để chạy Procedure DataPump
GRANT CREATE TABLE TO ADMIN_PHANHE1;
GRANT CREATE JOB TO ADMIN_PHANHE1;
GRANT READ, WRITE ON DIRECTORY BACKUP_DIR TO ADMIN_PHANHE1;
GRANT DATAPUMP_EXP_FULL_DATABASE TO ADMIN_PHANHE1;
GRANT DATAPUMP_IMP_FULL_DATABASE TO ADMIN_PHANHE1;


ALTER SESSION SET "_ORACLE_SCRIPT"=false;
ALTER SESSION SET CURRENT_SCHEMA = ADMIN_PHANHE1;



-- ==========================================
-- 1. TẠO CÁC BẢNG ĐỘC LẬP (Không có khóa ngoại)
-- ==========================================

-- Bảng KHOA
CREATE TABLE KHOA (
    MAKHOA VARCHAR2(20) PRIMARY KEY,
    TENKHOA NVARCHAR2(100)
);

-- Bảng NHÂN VIÊN
CREATE TABLE NHANVIEN (
    MANV VARCHAR2(20) PRIMARY KEY,
    HOTEN NVARCHAR2(100),
    PHAI NVARCHAR2(10),
    NGAYSINH DATE,
    CMND VARCHAR2(20),
    QUEQUAN NVARCHAR2(200),
    SODT VARCHAR2(15),
    VAITRO NVARCHAR2(50),
    CHUYENKHOA NVARCHAR2(100),
    COSO NVARCHAR2(50) -- Bổ sung cho OLS (HCM, HN, HP)
);

-- Bảng BỆNH NHÂN
CREATE TABLE BENHNHAN (
    MABN VARCHAR2(20) PRIMARY KEY,
    TENBN NVARCHAR2(100),
    PHAI NVARCHAR2(10),
    NGAYSINH DATE,
    CCCD VARCHAR2(20),
    SONHA NVARCHAR2(50),
    TENDUONG NVARCHAR2(100),
    QUANHUYEN NVARCHAR2(50),
    TINHTP NVARCHAR2(50),
    TIENSUBENH NVARCHAR2(1000),
    TIENSUBENHGD NVARCHAR2(1000),
    DIUNGTHUOC NVARCHAR2(1000)
);

-- Bảng THÔNG BÁO (Độc lập, dùng cho OLS)
CREATE TABLE THONG_BAO (
    MATB VARCHAR2(20) PRIMARY KEY,
    NOIDUNG NVARCHAR2(1000),
    NGAYGIO TIMESTAMP,
    DIADIEM NVARCHAR2(100),
    OLS_LABEL NUMBER(10) -- Cột lưu mã số của nhãn OLS
);


-- ==========================================
-- 2. TẠO CÁC BẢNG PHỤ THUỘC (Có khóa ngoại)
-- ==========================================

-- Bảng HỒ SƠ BỆNH ÁN (HSBA)
CREATE TABLE HSBA (
    MAHSBA VARCHAR2(20) PRIMARY KEY,
    MABN VARCHAR2(20),
    NGAY DATE,
    CHANDOAN NVARCHAR2(500),
    DIEUTRI NVARCHAR2(500),
    MABS VARCHAR2(20),
    MAKHOA VARCHAR2(20),
    KETLUAN NVARCHAR2(500),
    
    -- Ràng buộc khóa ngoại
    CONSTRAINT FK_HSBA_BENHNHAN FOREIGN KEY (MABN) REFERENCES BENHNHAN(MABN),
    CONSTRAINT FK_HSBA_NHANVIEN FOREIGN KEY (MABS) REFERENCES NHANVIEN(MANV),
    CONSTRAINT FK_HSBA_KHOA FOREIGN KEY (MAKHOA) REFERENCES KHOA(MAKHOA)
);

-- Bảng DỊCH VỤ HỒ SƠ BỆNH ÁN (HSBA_DV)
CREATE TABLE HSBA_DV (
    MAHSBA VARCHAR2(20),
    LOAIDV NVARCHAR2(100),
    NGAYDV DATE,
    MAKTV VARCHAR2(20),
    KETQUA NVARCHAR2(500),
    
    -- Khóa chính kết hợp
    PRIMARY KEY (MAHSBA, LOAIDV, NGAYDV),
    
    -- Ràng buộc khóa ngoại
    CONSTRAINT FK_HSBADV_HSBA FOREIGN KEY (MAHSBA) REFERENCES HSBA(MAHSBA),
    CONSTRAINT FK_HSBADV_NHANVIEN FOREIGN KEY (MAKTV) REFERENCES NHANVIEN(MANV)
);

-- Bảng ĐƠN THUỐC
CREATE TABLE DONTHUOC (
    MAHSBA VARCHAR2(20),
    TENTHUOC NVARCHAR2(100),
    NGAYDT DATE,
    LIEUDUNG NVARCHAR2(200),
    
    -- Khóa chính kết hợp
    PRIMARY KEY (MAHSBA, TENTHUOC, NGAYDT),
    
    -- Ràng buộc khóa ngoại
    CONSTRAINT FK_DONTHUOC_HSBA FOREIGN KEY (MAHSBA) REFERENCES HSBA(MAHSBA)
);

-- ========================================================
-- PHẦN 3: CHÈN DỮ LIỆU MẪU (DATA MOCKING)
-- ========================================================

-- KHOA
INSERT INTO KHOA VALUES ('K01', N'Khoa Tim mạch');
INSERT INTO KHOA VALUES ('K02', N'Khoa Thần kinh');
INSERT INTO KHOA VALUES ('K03', N'Khoa Tiêu hóa');

-- NHANVIEN
-- Giám đốc & Lãnh đạo
INSERT INTO NHANVIEN VALUES ('NV001', N'Nguyễn Văn Nam', N'Nam', TO_DATE('01-01-70', 'DD-MM-YY'), '079170000001', N'TP.HCM', '0901000001', N'Giám đốc', N'Quản lý chung', 'HCM');
INSERT INTO NHANVIEN VALUES ('NV002', N'Trần Phúc Mạnh', N'Nam', TO_DATE('15-05-75', 'DD-MM-YY'), '079175000002', N'TP.HCM', '0901000002', N'Lãnh đạo khoa', N'Tim mạch', 'HCM');
INSERT INTO NHANVIEN VALUES ('NV003', N'Lê Phương Thanh', N'Nữ', TO_DATE('20-08-80', 'DD-MM-YY'), '079180000003', N'Hà Nội', '0901000003', N'Lãnh đạo khoa', N'Thần kinh', 'HN');

-- Bác sĩ/Y tá
INSERT INTO NHANVIEN VALUES ('NV004', N'Phạm Nhật Minh', N'Nam', TO_DATE('10-10-85', 'DD-MM-YY'), '079185000004', N'Đà Nẵng', '0901000004', N'Bác sĩ/Y sĩ', N'Tim mạch', 'HCM');
INSERT INTO NHANVIEN VALUES ('NV005', N'Hoàng Thị Tuyết', N'Nữ', TO_DATE('05-12-88', 'DD-MM-YY'), '079188000005', N'Cần Thơ', '0901000005', N'Bác sĩ/Y sĩ', N'Thần kinh', 'HCM');
INSERT INTO NHANVIEN VALUES ('NV006', N'Bùi Quốc Huy', N'Nam', TO_DATE('14-02-87', 'DD-MM-YY'), '079187000006', N'Hải Phòng', '0901000006', N'Bác sĩ/Y sĩ', N'Tiêu hóa', 'HP');

-- Kỹ thuật viên & Điều phối viên
INSERT INTO NHANVIEN VALUES ('NV007', N'Đặng Trọng Trân', N'Nam', TO_DATE('25-03-90', 'DD-MM-YY'), '079190000007', N'TP.HCM', '0901000007', N'Kỹ thuật viên', N'Chẩn đoán hình ảnh', 'HCM');
INSERT INTO NHANVIEN VALUES ('NV008', N'Vũ Như Nguyệt', N'Nữ', TO_DATE('30-07-92', 'DD-MM-YY'), '079192000008', N'Hà Nội', '0901000008', N'Điều phối viên', N'Hành chính', 'HCM');

-- BENHNHAN
INSERT INTO BENHNHAN VALUES ('BN000001', N'Lý Văn Trường', N'Nam', TO_DATE('12-04-95', 'DD-MM-YY'), '079195000001', N'123', N'Nguyễn Trãi', N'Quận 5', N'TP.HCM', N'Không', N'Không', N'Penicillin');
INSERT INTO BENHNHAN VALUES ('BN000002', N'Trương Ngọc Bình', N'Nữ', TO_DATE('08-09-82', 'DD-MM-YY'), '079182000002', N'456', N'Lê Lợi', N'Quận 1', N'TP.HCM', N'Cao huyết áp', N'Tiểu đường', N'Không');
INSERT INTO BENHNHAN VALUES ('BN000003', N'Ngô Văn Thanh', N'Nam', TO_DATE('22-11-00', 'DD-MM-YY'), '079200000003', N'789', N'Trần Phú', N'Hà Đông', N'Hà Nội', N'Đau dạ dày', N'Không', N'Hải sản');

-- HSBA
INSERT INTO HSBA VALUES ('HS000001', 'BN000001', TO_DATE('10-04-26', 'DD-MM-YY'), N'Rối loạn nhịp tim', N'Theo dõi đồ thị tim, cấp thuốc', 'NV004', 'K01', N'Ổn định, cần tái khám sau 1 tuần');
INSERT INTO HSBA VALUES ('HS000002', 'BN000002', TO_DATE('11-04-26', 'DD-MM-YY'), N'Đau nửa đầu', N'Chụp MRI, cấp thuốc giảm đau', 'NV005', 'K02', N'Đau đầu căng cơ, không có tổn thương não');
INSERT INTO HSBA VALUES ('HS000003', 'BN000003', TO_DATE('12-04-26', 'DD-MM-YY'), N'Viêm dạ dày cấp', N'Nội soi dạ dày, cấp thuốc', 'NV006', 'K03', N'Viêm niêm mạc dạ dày vùng hang vị');

-- HSBA_DV
-- Dịch vụ do Kỹ thuật viên (NV007) thực hiện
INSERT INTO HSBA_DV VALUES ('HS000001', N'Đo điện tâm đồ (ECG)', TO_DATE('10-04-26', 'DD-MM-YY'), 'NV007', N'Nhịp tim hơi nhanh, xoang bình thường');
INSERT INTO HSBA_DV VALUES ('HS000002', N'Chụp MRI Sọ não', TO_DATE('11-04-26', 'DD-MM-YY'), 'NV007', N'Không phát hiện khối u hay tổn thương thực thể');
INSERT INTO HSBA_DV VALUES ('HS000003', N'Nội soi dạ dày', TO_DATE('12-04-26', 'DD-MM-YY'), 'NV007', N'Sung huyết hang vị mức độ nhẹ');

-- DONTHUOC
INSERT INTO DONTHUOC VALUES ('HS000001', N'Concor 5mg', TO_DATE('10-04-26', 'DD-MM-YY'), N'Ngày 1 viên, uống buổi sáng sau ăn');
INSERT INTO DONTHUOC VALUES ('HS000002', N'Paracetamol 500mg', TO_DATE('11-04-26', 'DD-MM-YY'), N'Ngày 2 viên, sáng/tối khi đau đầu');
INSERT INTO DONTHUOC VALUES ('HS000003', N'Omeprazole 20mg', TO_DATE('12-04-26', 'DD-MM-YY'), N'Ngày 1 viên, uống trước khi ăn sáng 30 phút');

-- THONG_BAO (Dành cho OLS - Hiện tại để trống nhãn OLS_LABEL chờ Phân hệ 2)
INSERT INTO THONG_BAO VALUES ('TB001', N'Họp giao ban toàn viện tháng 4', TO_TIMESTAMP('20-04-26 08:00:00', 'DD-MM-YY HH24:MI:SS'), N'Hội trường A', NULL);
INSERT INTO THONG_BAO VALUES ('TB002', N'Họp khẩn Ban Lãnh đạo Khoa Tim mạch', TO_TIMESTAMP('21-04-26 14:00:00', 'DD-MM-YY HH24:MI:SS'), N'Phòng họp 1 - Cơ sở HCM', NULL);

COMMIT;

-- ---------------------------------------------------------------------------------------------------------------------------------------------------
-- ------------------------------------------------------------------------ PROCEDURE ----------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------------------------------------
-- Tạo bộ đếm sequence cho bệnh nhân
CREATE SEQUENCE ADMIN_PHANHE1.SEQ_MABN 
START WITH 1 
INCREMENT BY 1 
NOCACHE 
NOCYCLE;

-- Tạo bộ đếm sequence cho nhân viên
CREATE SEQUENCE ADMIN_PHANHE1.SEQ_MANV 
START WITH 1 
INCREMENT BY 1 
NOCACHE 
NOCYCLE;

-- Tạo procedure thêm Bệnh nhân
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_TAO_BENHNHAN (
    p_TENBN IN NVARCHAR2,
    p_PHAI IN NVARCHAR2,
    p_NGAYSINH IN DATE,
    p_CCCD IN VARCHAR2,
    p_SONHA IN NVARCHAR2,
    p_TENDUONG IN NVARCHAR2,
    p_QUANHUYEN IN NVARCHAR2,
    p_TINHTP IN NVARCHAR2,
    p_TIENSUBENH IN NVARCHAR2,
    p_TIENSUBENHGD IN NVARCHAR2,
    p_DIUNGTHUOC IN NVARCHAR2,
    p_MATKHAU IN VARCHAR2,
    p_MABN_OUT OUT VARCHAR2 -- Tham số OUT cực kỳ quan trọng để trả mã về cho C#
)
AUTHID CURRENT_USER
IS
    v_MABN VARCHAR2(20);
    v_sql VARCHAR2(500);
    v_seq_val NUMBER;
BEGIN
    -- Bước 1: Sinh mã bệnh nhân tự động
    -- Lấy số đếm tiếp theo từ Sequence
    SELECT ADMIN_PHANHE1.SEQ_MABN.NEXTVAL INTO v_seq_val FROM DUAL;
    
    -- Định dạng mã bệnh nhân (Ví dụ: BN0001, BN0002...)
    v_MABN := 'BN' || TO_CHAR(v_seq_val, 'FM0000'); 
    
    -- Trả mã vừa tạo ra cho tham số OUT
    p_MABN_OUT := v_MABN;

    -- Bước 2: Thêm thông tin vào bảng BENHNHAN
    INSERT INTO ADMIN_PHANHE1.BENHNHAN 
        (MABN, TENBN, PHAI, NGAYSINH, CCCD, SONHA, TENDUONG, QUANHUYEN, TINHTP, TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC)
    VALUES 
        (v_MABN, p_TENBN, p_PHAI, p_NGAYSINH, p_CCCD, p_SONHA, p_TENDUONG, p_QUANHUYEN, p_TINHTP, p_TIENSUBENH, p_TIENSUBENHGD, p_DIUNGTHUOC);

    -- Bước 3: Tạo User Oracle (C## + mã bệnh nhân tự sinh)
    v_sql := 'CREATE USER C##' || v_MABN || ' IDENTIFIED BY "' || p_MATKHAU || '"';
    EXECUTE IMMEDIATE v_sql;

    -- Bước 4: Cấp quyền kết nối
    v_sql := 'GRANT CREATE SESSION TO C##' || v_MABN;
    EXECUTE IMMEDIATE v_sql;

    -- Bước 5: Gán role RBAC cho bệnh nhân nếu role đã được cài đặt
    BEGIN
        v_sql := 'GRANT ROLE_BENHNHAN TO C##' || v_MABN;
        EXECUTE IMMEDIATE v_sql;
    EXCEPTION
        WHEN OTHERS THEN NULL;
    END;

    -- Hoàn tất toàn bộ giao dịch
    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        -- Nếu bị lỗi ở bất kỳ bước nào, Rollback ngay lập tức
        ROLLBACK;
        RAISE; 
END;
/

-- Tạo procedure thêm Nhân viên
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_TAO_NHANVIEN (
    p_HOTEN IN NVARCHAR2,
    p_PHAI IN NVARCHAR2,
    p_NGAYSINH IN DATE,
    p_CMND IN VARCHAR2,
    p_QUEQUAN IN NVARCHAR2,
    p_SODT IN VARCHAR2,
    p_VAITRO IN NVARCHAR2,
    p_CHUYENKHOA IN NVARCHAR2,
    p_COSO IN NVARCHAR2,
    p_MATKHAU IN VARCHAR2,
    p_MANV_OUT OUT VARCHAR2 -- Tham số OUT trả mã NV về cho giao diện WinForm
)
AUTHID CURRENT_USER
IS
    v_MANV VARCHAR2(20);
    v_sql VARCHAR2(500);
    v_seq_val NUMBER;
BEGIN
    -- Bước 1: Sinh mã nhân viên tự động
    -- Lấy số đếm tiếp theo từ Sequence của Nhân Viên
    SELECT ADMIN_PHANHE1.SEQ_MANV.NEXTVAL INTO v_seq_val FROM DUAL;
    
    -- Định dạng mã (Ví dụ: NV0001, NV0002...)
    v_MANV := 'NV' || TO_CHAR(v_seq_val, 'FM0000'); 
    
    -- Trả mã vừa tạo ra cho tham số OUT để C# hứng lấy
    p_MANV_OUT := v_MANV;

    -- Bước 2: Thêm thông tin vào bảng NHANVIEN
    INSERT INTO ADMIN_PHANHE1.NHANVIEN 
        (MANV, HOTEN, PHAI, NGAYSINH, CMND, QUEQUAN, SODT, VAITRO, CHUYENKHOA, COSO)
    VALUES 
        (v_MANV, p_HOTEN, p_PHAI, p_NGAYSINH, p_CMND, p_QUEQUAN, p_SODT, p_VAITRO, p_CHUYENKHOA, p_COSO);

    -- Bước 3: Tạo User Oracle (C## + mã NV tự sinh)
    v_sql := 'CREATE USER C##' || v_MANV || ' IDENTIFIED BY "' || p_MATKHAU || '"';
    EXECUTE IMMEDIATE v_sql;

    -- Bước 4: Cấp quyền kết nối cơ bản
    v_sql := 'GRANT CREATE SESSION TO C##' || v_MANV;
    EXECUTE IMMEDIATE v_sql;

    -- Bước 5: Gán role RBAC cho kỹ thuật viên nếu role đã được cài đặt
    IF REGEXP_LIKE(LOWER(p_VAITRO), 'thu.*t.*vi') THEN
        BEGIN
            v_sql := 'GRANT ROLE_KYTHUATVIEN TO C##' || v_MANV;
            EXECUTE IMMEDIATE v_sql;
        EXCEPTION
            WHEN OTHERS THEN NULL;
        END;
    END IF;

    -- Hoàn tất toàn bộ giao dịch, lưu dữ liệu vĩnh viễn
    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        -- Nếu có lỗi (VD: thiếu trường bắt buộc, trùng CMND nếu có set UNIQUE...), tự động hủy toàn bộ thao tác
        ROLLBACK;
        RAISE; 
END;
/


-- Xem nhân viên theo mã
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XEM_MOT_NHANVIEN (
    p_MANV IN VARCHAR2,
    p_CURSOR OUT SYS_REFCURSOR -- Con trỏ chứa bảng dữ liệu trả về cho C#
)
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT MANV AS "Mã NV", 
           HOTEN AS "Họ Tên", 
           PHAI AS "Phái", 
           TO_CHAR(NGAYSINH, 'DD/MM/YYYY') AS "Ngày Sinh", 
           CMND AS "CMND/CCCD", 
           QUEQUAN AS "Quê Quán", 
           SODT AS "Số ĐT", 
           VAITRO AS "Vai Trò", 
           CHUYENKHOA AS "Chuyên Khoa", 
           COSO AS "Cơ Sở"
    FROM ADMIN_PHANHE1.NHANVIEN
    WHERE MANV = p_MANV;
END;
/

-- Xem tất cả nhân viên
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XEM_ALL_NHANVIEN (
    p_CURSOR OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT MANV AS "Mã NV", 
           HOTEN AS "Họ Tên", 
           PHAI AS "Phái", 
           TO_CHAR(NGAYSINH, 'DD/MM/YYYY') AS "Ngày Sinh", 
           CMND AS "CMND/CCCD", 
           QUEQUAN AS "Quê Quán", 
           SODT AS "Số ĐT", 
           VAITRO AS "Vai Trò", 
           CHUYENKHOA AS "Chuyên Khoa", 
           COSO AS "Cơ Sở"
    FROM ADMIN_PHANHE1.NHANVIEN
    ORDER BY MANV;
END;
/

-- Xem danh sách bệnh nhân theo mã bệnh nhân
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XEM_MOT_BENHNHAN (
    p_MABN IN VARCHAR2,
    p_CURSOR OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT MABN AS "Mã BN", 
           TENBN AS "Tên Bệnh Nhân", 
           PHAI AS "Phái", 
           TO_CHAR(NGAYSINH, 'DD/MM/YYYY') AS "Ngày Sinh", 
           CCCD AS "CCCD", 
           SONHA || ', ' || TENDUONG AS "Địa Chỉ", 
           QUANHUYEN AS "Quận/Huyện", 
           TINHTP AS "Tỉnh/TP", 
           TIENSUBENH AS "Tiền Sử Bệnh", 
           TIENSUBENHGD AS "TS Bệnh Gia Đình", 
           DIUNGTHUOC AS "Dị Ứng Thuốc"
    FROM ADMIN_PHANHE1.BENHNHAN
    WHERE MABN = p_MABN;
END;
/

-- Xem danh sách tất cả bệnh nhân
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XEM_ALL_BENHNHAN (
    p_CURSOR OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT MABN AS "Mã BN", 
           TENBN AS "Tên Bệnh Nhân", 
           PHAI AS "Phái", 
           TO_CHAR(NGAYSINH, 'DD/MM/YYYY') AS "Ngày Sinh", 
           CCCD AS "CCCD", 
           SONHA || ', ' || TENDUONG AS "Địa Chỉ", 
           QUANHUYEN AS "Quận/Huyện", 
           TINHTP AS "Tỉnh/TP", 
           TIENSUBENH AS "Tiền Sử Bệnh", 
           TIENSUBENHGD AS "TS Bệnh Gia Đình", 
           DIUNGTHUOC AS "Dị Ứng Thuốc"
    FROM ADMIN_PHANHE1.BENHNHAN
    ORDER BY MABN;
END;
/

-- Xóa bệnh nhân
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XOA_BENHNHAN (
    p_MABN IN VARCHAR2
)
AUTHID CURRENT_USER
IS
    v_sql VARCHAR2(500);
    v_username VARCHAR2(50) := 'C##' || UPPER(p_MABN);
BEGIN
    -- Kích hoạt cờ bỏ qua bảo mật Container 12c+
    EXECUTE IMMEDIATE 'ALTER SESSION SET "_ORACLE_SCRIPT"=true';

    -- Bước 0: Tự động ngắt tất cả các kết nối hiện tại của tài khoản này
    FOR rec IN (SELECT sid, serial# FROM v$session WHERE username = v_username)
    LOOP
        EXECUTE IMMEDIATE 'ALTER SYSTEM KILL SESSION ''' || rec.sid || ',' || rec.serial# || ''' IMMEDIATE';
    END LOOP;

    -- Bước 1: Xóa thông tin record trong bảng BENHNHAN
    DELETE FROM ADMIN_PHANHE1.BENHNHAN WHERE MABN = p_MABN;

    -- Bước 2: Xóa tài khoản Oracle
    v_sql := 'DROP USER ' || v_username || ' CASCADE';
    EXECUTE IMMEDIATE v_sql;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE;
END;
/

-- Xóa nhân viên
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XOA_NHANVIEN (
    p_MANV IN VARCHAR2
)
AUTHID CURRENT_USER
IS
    v_sql VARCHAR2(500);
    v_username VARCHAR2(50) := 'C##' || UPPER(p_MANV);
BEGIN
    -- Kích hoạt cờ bỏ qua bảo mật Container 12c+
    EXECUTE IMMEDIATE 'ALTER SESSION SET "_ORACLE_SCRIPT"=true';

    -- Bước 0: Tự động ngắt kết nối
    FOR rec IN (SELECT sid, serial# FROM v$session WHERE username = v_username)
    LOOP
        EXECUTE IMMEDIATE 'ALTER SYSTEM KILL SESSION ''' || rec.sid || ',' || rec.serial# || ''' IMMEDIATE';
    END LOOP;

    -- Bước 1: Xóa dữ liệu trong bảng NHANVIEN
    DELETE FROM ADMIN_PHANHE1.NHANVIEN WHERE MANV = p_MANV;

    -- Bước 2: Xóa tài khoản Oracle
    v_sql := 'DROP USER ' || v_username || ' CASCADE';
    EXECUTE IMMEDIATE v_sql;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE;
END;
/


-- Cập nhật thông tin bệnh nhân
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_SUA_BENHNHAN (
    p_MABN IN VARCHAR2, -- Vẫn phải truyền vào để làm điều kiện WHERE tìm đúng người
    p_TENBN IN NVARCHAR2,
    p_PHAI IN NVARCHAR2,
    p_NGAYSINH IN DATE,
    p_CCCD IN VARCHAR2,
    p_SONHA IN NVARCHAR2,
    p_TENDUONG IN NVARCHAR2,
    p_QUANHUYEN IN NVARCHAR2,
    p_TINHTP IN NVARCHAR2,
    p_TIENSUBENH IN NVARCHAR2,
    p_TIENSUBENHGD IN NVARCHAR2,
    p_DIUNGTHUOC IN NVARCHAR2,
    p_MATKHAU IN VARCHAR2 -- Nhận mật khẩu mới (có thể rỗng)
)
AUTHID CURRENT_USER
IS
    v_sql VARCHAR2(500);
BEGIN
    -- Bước 1: Cập nhật thông tin cá nhân trong bảng BENHNHAN
    -- (Hoàn toàn KHÔNG đụng chạm đến cột MABN)
    UPDATE ADMIN_PHANHE1.BENHNHAN 
    SET TENBN = p_TENBN,
        PHAI = p_PHAI,
        NGAYSINH = p_NGAYSINH,
        CCCD = p_CCCD,
        SONHA = p_SONHA,
        TENDUONG = p_TENDUONG,
        QUANHUYEN = p_QUANHUYEN,
        TINHTP = p_TINHTP,
        TIENSUBENH = p_TIENSUBENH,
        TIENSUBENHGD = p_TIENSUBENHGD,
        DIUNGTHUOC = p_DIUNGTHUOC
    WHERE MABN = p_MABN;

    -- Bước 2: Đổi mật khẩu tài khoản Oracle (Nếu người dùng có nhập mật khẩu)
    -- Kiểm tra nếu p_MATKHAU không bị rỗng (NULL) thì mới chạy lệnh ALTER USER
    IF p_MATKHAU IS NOT NULL AND TRIM(p_MATKHAU) <> '' THEN
        v_sql := 'ALTER USER C##' || p_MABN || ' IDENTIFIED BY "' || p_MATKHAU || '"';
        EXECUTE IMMEDIATE v_sql;
    END IF;

    -- Lưu dữ liệu
    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE;
END;
/

-- Cập nhật thông tin Nhân viên
-- Lệnh xóa (Nếu chưa có sẽ báo lỗi ORA-04043, bạn cứ bỏ qua không sao nhé)
DROP PROCEDURE ADMIN_PHANHE1.SP_SUA_NHANVIEN;

-- Lệnh tạo mới
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_SUA_NHANVIEN (
    p_MANV IN VARCHAR2, -- Mã NV dùng để xác định người cần sửa
    p_HOTEN IN NVARCHAR2,
    p_PHAI IN NVARCHAR2,
    p_NGAYSINH IN DATE,
    p_CMND IN VARCHAR2,
    p_QUEQUAN IN NVARCHAR2,
    p_SODT IN VARCHAR2,
    p_VAITRO IN NVARCHAR2,
    p_CHUYENKHOA IN NVARCHAR2,
    p_COSO IN NVARCHAR2,
    p_MATKHAU IN VARCHAR2 -- Nhận mật khẩu mới (có thể rỗng)
)
AUTHID CURRENT_USER
IS
    v_sql VARCHAR2(500);
BEGIN
    -- Bước 1: Cập nhật thông tin cá nhân trong bảng NHANVIEN
    UPDATE ADMIN_PHANHE1.NHANVIEN 
    SET HOTEN = p_HOTEN,
        PHAI = p_PHAI,
        NGAYSINH = p_NGAYSINH,
        CMND = p_CMND,
        QUEQUAN = p_QUEQUAN,
        SODT = p_SODT,
        VAITRO = p_VAITRO,
        CHUYENKHOA = p_CHUYENKHOA,
        COSO = p_COSO
    WHERE MANV = p_MANV;

    -- Bước 2: Đổi mật khẩu tài khoản Oracle (Nếu người dùng có nhập mật khẩu trên Form)
    IF p_MATKHAU IS NOT NULL AND TRIM(p_MATKHAU) <> '' THEN
        v_sql := 'ALTER USER C##' || p_MANV || ' IDENTIFIED BY "' || p_MATKHAU || '"';
        EXECUTE IMMEDIATE v_sql;
    END IF;

    -- Bước 3: Cập nhật role RBAC nếu vai trò được đổi sang/ra khỏi Kỹ thuật viên
    BEGIN
        IF REGEXP_LIKE(LOWER(p_VAITRO), 'thu.*t.*vi') THEN
            v_sql := 'GRANT ROLE_KYTHUATVIEN TO C##' || p_MANV;
        ELSE
            v_sql := 'REVOKE ROLE_KYTHUATVIEN FROM C##' || p_MANV;
        END IF;
        EXECUTE IMMEDIATE v_sql;
    EXCEPTION
        WHEN OTHERS THEN NULL;
    END;

    -- Hoàn tất và lưu dữ liệu
    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        -- Hoàn tác nếu có bất kỳ lỗi gì xảy ra (ví dụ: vi phạm ràng buộc dữ liệu)
        ROLLBACK;
        RAISE;
END;
/


-- Xem quyền trên toàn bộ nhân viên
GRANT SELECT ANY DICTIONARY TO ADMIN_PHANHE1;

-- Lệnh xóa (Chỉ chạy khi Procedure đã tồn tại, nếu không sẽ báo lỗi ORA-04043)
DROP PROCEDURE ADMIN_PHANHE1.SP_XEM_QUYEN_ALL_USER;

-- Lệnh tạo mới (hoặc ghi đè)
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XEM_QUYEN_ALL_USER (
    p_CURSOR OUT SYS_REFCURSOR
)
AUTHID CURRENT_USER 
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT p.GRANTEE AS "Tên Tài Khoản / Role",
           p.TABLE_NAME AS "Tên Bảng", 
           p.PRIVILEGE AS "Quyền", 
           p.GRANTABLE AS "Được Cấp Tiếp" 
    FROM DBA_TAB_PRIVS p
    JOIN DBA_OBJECTS o ON p.TABLE_NAME = o.OBJECT_NAME AND p.OWNER = o.OWNER
    WHERE p.GRANTEE LIKE 'C##%' 
      AND p.GRANTEE NOT IN ('C##ADMIN', 'ADMIN_PHANHE1', USER) 
      AND p.OWNER = 'ADMIN_PHANHE1'
      AND o.OBJECT_TYPE = 'TABLE'
    ORDER BY p.GRANTEE, p.TABLE_NAME;
END;
/
-- Xem quyền trên cột
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XEM_QUYEN_COT_ALL_USER (
    p_CURSOR OUT SYS_REFCURSOR
)
AUTHID CURRENT_USER -- Vẫn giữ cơ chế "động" theo tài khoản DBA đang đăng nhập
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT GRANTEE AS "Tên Tài Khoản / Role",
           TABLE_NAME AS "Tên Bảng", 
           COLUMN_NAME AS "Tên Cột", -- Điểm khác biệt mấu chốt ở đây
           PRIVILEGE AS "Quyền", 
           GRANTABLE AS "Được Cấp Tiếp" 
    FROM DBA_COL_PRIVS 
    WHERE GRANTEE LIKE 'C##%' 
      AND GRANTEE != 'C##ADMIN'
      AND OWNER = 'ADMIN_PHANHE1'
    ORDER BY GRANTEE, TABLE_NAME, COLUMN_NAME;
END;
/
-- Xem quyền trên view
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XEM_QUYEN_VIEW_ALL_USER (
    p_CURSOR OUT SYS_REFCURSOR
)
AUTHID CURRENT_USER
AS
BEGIN
    OPEN p_CURSOR FOR
    -- Kết hợp bảng phân quyền (p) và bảng loại đối tượng (o)
    SELECT p.GRANTEE AS "Tên Tài Khoản / Role",
           p.TABLE_NAME AS "Tên View",
           p.PRIVILEGE AS "Quyền",
           p.GRANTABLE AS "Được Cấp Tiếp"
    FROM DBA_TAB_PRIVS p
    JOIN DBA_OBJECTS o ON p.TABLE_NAME = o.OBJECT_NAME AND p.OWNER = o.OWNER
    WHERE p.GRANTEE LIKE 'C##%'
      AND p.GRANTEE NOT IN ('C##ADMIN', 'ADMIN_PHANHE1', USER)
      AND p.OWNER = 'ADMIN_PHANHE1'
      AND o.OBJECT_TYPE = 'VIEW' -- Chỉ lọc lấy View
    ORDER BY p.GRANTEE, p.TABLE_NAME;
END;
/
-- Xem quyền trên procedure/function
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XEM_QUYEN_PROC_ALL_USER (
    p_CURSOR OUT SYS_REFCURSOR
)
AUTHID CURRENT_USER
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT p.GRANTEE AS "Tên Tài Khoản / Role",
           p.TABLE_NAME AS "Tên Hàm / Thủ Tục",
           o.OBJECT_TYPE AS "Loại Đối Tượng", -- Hiển thị rõ nó là Procedure hay Function
           p.PRIVILEGE AS "Quyền",
           p.GRANTABLE AS "Được Cấp Tiếp"
    FROM DBA_TAB_PRIVS p
    JOIN DBA_OBJECTS o ON p.TABLE_NAME = o.OBJECT_NAME AND p.OWNER = o.OWNER
    WHERE p.GRANTEE LIKE 'C##%'
      AND p.GRANTEE NOT IN ('C##ADMIN', 'ADMIN_PHANHE1', USER)
      AND p.OWNER = 'ADMIN_PHANHE1'
      AND o.OBJECT_TYPE IN ('PROCEDURE', 'FUNCTION') -- Lọc lấy Procedure và Function
    ORDER BY p.GRANTEE, p.TABLE_NAME;
END;
/

-----------------------------------------------
-- NHẤT LÀ RẤT CHÂN THÀNH CẢM ƠN SỰ GIÚP ĐỠ CỦA BẠN ---
-----------------------------------------------
-- =================================================================================
-- [NHIỆM VỤ 3]: CẤP QUYỀN (GRANT PRIVILEGES) QUA PROCEDURE
-- =================================================================================

-- =================================================================================
-- 3.1 CẤP QUYỀN TRÊN BẢNG (TABLE) / VIEW
-- =================================================================================

-- -- Cấp quyền trên toàn bộ bảng hoặc view
-- GRANT <PRIVILEGE> 
-- ON ADMIN_PHANHE1.<OBJECT_NAME> 
-- TO <GRANTEE>;

-- -- Trong đó:
-- -- <PRIVILEGE>: SELECT | INSERT | UPDATE | DELETE
-- -- <OBJECT_NAME>: tên bảng hoặc view
-- -- <GRANTEE>: USER hoặc ROLE

-- -- Cấp quyền kèm WITH GRANT OPTION (chỉ áp dụng cho USER)
-- GRANT <PRIVILEGE> 
-- ON ADMIN_PHANHE1.<OBJECT_NAME> 
-- TO <USERNAME> 
-- WITH GRANT OPTION;


-- -- =================================================================================
-- -- 3.2 CẤP QUYỀN TRÊN CỘT (COLUMN-LEVEL)
-- -- =================================================================================

-- -- Chỉ áp dụng cho SELECT và UPDATE

-- -- Cấp quyền SELECT trên các cột cụ thể
-- GRANT SELECT (<COLUMN_1>, <COLUMN_2>) 
-- ON ADMIN_PHANHE1.<TABLE_NAME> 
-- TO <GRANTEE>;

-- -- Cấp quyền UPDATE trên các cột cụ thể
-- GRANT UPDATE (<COLUMN_1>, <COLUMN_2>) 
-- ON ADMIN_PHANHE1.<TABLE_NAME> 
-- TO <GRANTEE>;


-- -- =================================================================================
-- -- 3.3 CẤP QUYỀN TRÊN PROCEDURE / FUNCTION
-- -- =================================================================================

-- -- Chỉ sử dụng quyền EXECUTE
-- GRANT EXECUTE 
-- ON ADMIN_PHANHE1.<PROGRAM_NAME> 
-- TO <GRANTEE>;

-- -- <PROGRAM_NAME>: tên PROCEDURE hoặc FUNCTION


-- -- =================================================================================
-- -- 3.4 CẤP ROLE CHO USER
-- -- =================================================================================

-- -- Gán ROLE cho USER
-- GRANT <ROLE_NAME> 
-- TO <USERNAME>;

-- -- Gán ROLE kèm quyền quản trị (ADMIN OPTION)
-- GRANT <ROLE_NAME> 
-- TO <USERNAME> 
-- WITH ADMIN OPTION;

-- =================================================================================
-- CÁC CÂU TRUY VẤN HỖ TRỢ GIAO DIỆN (WINFORM)
-- =================================================================================
-- 1. Lấy danh sách cột của 1 bảng
SELECT COLUMN_NAME
FROM ALL_TAB_COLUMNS
WHERE OWNER = 'ADMIN_PHANHE1'
AND TABLE_NAME = UPPER(:p_table_name)
ORDER BY COLUMN_ID;


-- 2. Lấy danh sách TABLE
SELECT TABLE_NAME
FROM ALL_TABLES
WHERE OWNER = 'ADMIN_PHANHE1'
ORDER BY TABLE_NAME;


-- 3. Lấy danh sách VIEW
SELECT VIEW_NAME
FROM ALL_VIEWS
WHERE OWNER = 'ADMIN_PHANHE1'
ORDER BY VIEW_NAME;


-- 4. Lấy danh sách PROCEDURE và FUNCTION
SELECT OBJECT_NAME
FROM ALL_OBJECTS
WHERE OWNER = 'ADMIN_PHANHE1'
AND OBJECT_TYPE IN ('PROCEDURE', 'FUNCTION')
ORDER BY OBJECT_NAME;

-- =================================================================================
-- [NHIỆM VỤ 3]: PROCEDURE
-- =================================================================================
-- PROCEDURE cấp mọi loại quyền ---
CREATE OR REPLACE PROCEDURE SP_GRANT_ANY_OBJECT (
    p_GRANTEE       IN VARCHAR2,
    p_PRIVILEGE     IN VARCHAR2,
    p_OBJECT_NAME   IN VARCHAR2,
    p_COLUMNS       IN VARCHAR2,
    p_GRANT_OPTION  IN NUMBER   
)
AUTHID CURRENT_USER
AS
    v_sql                VARCHAR2(1000);
    v_grant_option_str   VARCHAR2(50) := '';
    v_column_str         VARCHAR2(500) := '';
    v_is_role            NUMBER;
    v_grantee            VARCHAR2(100) := UPPER(TRIM(p_GRANTEE));
    v_privilege          VARCHAR2(50)  := UPPER(TRIM(p_PRIVILEGE));
    v_obj                VARCHAR2(100) := UPPER(TRIM(p_OBJECT_NAME));
BEGIN
    -- Kiểm tra xem đối tượng được cấp quyền là User hay Role
    SELECT COUNT(*) INTO v_is_role FROM DBA_ROLES WHERE ROLE = v_grantee;

    -- Chỉ thêm WITH GRANT OPTION nếu là User (v_is_role = 0)
    IF p_GRANT_OPTION = 1 AND v_is_role = 0 THEN
        v_grant_option_str := ' WITH GRANT OPTION';
    END IF;

    -- Xử lý cấp quyền trên các cột cụ thể (chỉ áp dụng cho SELECT/UPDATE)
    IF (v_privilege IN ('SELECT', 'UPDATE')) AND p_COLUMNS IS NOT NULL THEN
        v_column_str := ' (' || UPPER(TRIM(p_COLUMNS)) || ')';
    END IF;

    -- Bảo mật: Kiểm tra tên hợp lệ để tránh SQL Injection
    v_grantee := DBMS_ASSERT.SIMPLE_SQL_NAME(v_grantee);
    v_obj     := DBMS_ASSERT.SIMPLE_SQL_NAME(v_obj);

    -- Xây dựng câu lệnh dynamic SQL
    v_sql := 'GRANT ' || v_privilege || v_column_str || 
             ' ON ADMIN_PHANHE1.' || v_obj || 
             ' TO ' || v_grantee || v_grant_option_str;

    DBMS_OUTPUT.PUT_LINE('Executing: ' || v_sql);
    
    -- Thực thi câu lệnh
    EXECUTE IMMEDIATE v_sql;
    
    -- Lưu ý: Lệnh GRANT thường không cần COMMIT vì nó là DDL (Data Definition Language)
    -- Nhưng nếu bạn muốn giữ COMMIT để đảm bảo tính nhất quán trong logic riêng thì có thể để lại.
    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('ERROR: ' || SQLERRM);
        RAISE;
END;
/

-- >>> 3.2 CÁC TRUY VẤN HỖ TRỢ LOAD DỮ LIỆU LÊN WINFORM
-- Lấy danh sách bảng
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_GET_LIST_TABLES (
    p_CURSOR OUT SYS_REFCURSOR
)
AUTHID CURRENT_USER
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT TABLE_NAME AS "Tên Bảng"
    FROM ALL_TABLES
    WHERE OWNER = 'ADMIN_PHANHE1'
    ORDER BY TABLE_NAME;
END;
/
-- Lấy danh sách cột của 1 bảng
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_GET_COLUMNS (
    p_TABLE_NAME IN VARCHAR2,
    p_CURSOR OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT COLUMN_NAME
    FROM ALL_TAB_COLUMNS
    WHERE OWNER = 'ADMIN_PHANHE1'
    AND TABLE_NAME = UPPER(p_TABLE_NAME)
    ORDER BY COLUMN_ID;
END;
/

-- lấy danh sách view
-- Lệnh xóa nếu đã tồn tại

CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_GET_LIST_VIEWS (
    p_CURSOR OUT SYS_REFCURSOR
)
AUTHID CURRENT_USER
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT OBJECT_NAME AS "Tên View"
    FROM ALL_OBJECTS
    WHERE OWNER = 'ADMIN_PHANHE1'
      AND OBJECT_TYPE = 'VIEW'
    ORDER BY OBJECT_NAME;
END;
/
-- lấy danh sách procedure / function
-- Lệnh xóa nếu đã tồn tại

-- Lệnh tạo mới
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_GET_LIST_PROCS_FUNCS (
    p_CURSOR OUT SYS_REFCURSOR
)
AUTHID CURRENT_USER
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT OBJECT_NAME AS "Tên Đối Tượng",
           OBJECT_TYPE AS "Loại Đối Tượng" -- Trả về chữ 'PROCEDURE' hoặc 'FUNCTION' để phân biệt
    FROM ALL_OBJECTS
    WHERE OWNER = 'ADMIN_PHANHE1'
      AND OBJECT_TYPE IN ('PROCEDURE', 'FUNCTION')
    ORDER BY OBJECT_TYPE, OBJECT_NAME;
END;
/

-- KẾT THÚC TEST 3 --


-- PROCEDURE 4 THU HỒI QUYỀN TỪ ROLE, USER ------------
------------------------------------------------------
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_REVOKE_PRIVILEGE (
    p_GRANTEE IN VARCHAR2,      
    p_PRIVILEGE IN VARCHAR2,    
    p_OBJECT_NAME IN VARCHAR2   
)
AUTHID CURRENT_USER
AS
    v_sql VARCHAR2(1000);
    v_grantee VARCHAR2(100) := UPPER(TRIM(p_GRANTEE));
    v_priv    VARCHAR2(50)  := UPPER(TRIM(p_PRIVILEGE));
    v_obj     VARCHAR2(100) := UPPER(TRIM(p_OBJECT_NAME));
BEGIN
    -- Chống SQL Injection
    v_grantee := DBMS_ASSERT.SIMPLE_SQL_NAME(v_grantee);

    -- SỬA Ở ĐÂY: Chỉ cần IS NOT NULL
    IF v_obj IS NOT NULL THEN
        v_obj := DBMS_ASSERT.SIMPLE_SQL_NAME(v_obj);
        v_sql := 'REVOKE ' || v_priv || 
                 ' ON ADMIN_PHANHE1.' || v_obj || 
                 ' FROM ' || v_grantee;
    ELSE
        -- Nhánh này chỉ chạy khi p_OBJECT_NAME thực sự là rỗng (thu hồi ROLE)
        v_sql := 'REVOKE ' || v_priv || ' FROM ' || v_grantee;
    END IF;

    DBMS_OUTPUT.PUT_LINE('Executing Revoke: ' || v_sql);
    EXECUTE IMMEDIATE v_sql;
    COMMIT;
END;
/


-- ================================================================================= TAB ROLE PROCEDURE =================================================================================
-- Xóa role --
create or replace PROCEDURE SP_XOA_ROLE (
    p_ten_role IN VARCHAR2
) 
IS
    v_count NUMBER;
BEGIN
    -- 1. Cho phép xóa cả các role được tạo bằng _ORACLE_SCRIPT (nếu cần)
    EXECUTE IMMEDIATE 'ALTER SESSION SET "_ORACLE_SCRIPT"=true';

    -- 2. Kiểm tra role có tồn tại không trước khi xóa
    SELECT COUNT(*) INTO v_count 
    FROM dba_roles 
    WHERE role = UPPER(p_ten_role);

    IF v_count > 0 THEN
        -- 3. Thực hiện xóa role
        EXECUTE IMMEDIATE 'DROP ROLE ' || p_ten_role;
        DBMS_OUTPUT.PUT_LINE('Đã xóa thành công role: ' || p_ten_role);
    ELSE
        -- Nếu không tìm thấy role, ném lỗi về cho C# biết
        RAISE_APPLICATION_ERROR(-20003, 'Role ' || p_ten_role || ' không tồn tại trong hệ thống.');
    END IF;

EXCEPTION
    WHEN OTHERS THEN
        -- Trả mã lỗi chi tiết về cho ứng dụng thay vì im lặng
        RAISE_APPLICATION_ERROR(-20004, 'Lỗi thực tế từ Oracle: ' || SQLERRM);
END;
/
-- Xem tất cả role
create or replace PROCEDURE SP_XEM_TAT_CA_ROLE (
  p_recordset OUT SYS_REFCURSOR
)
IS
BEGIN
  OPEN p_recordset FOR
    SELECT ROLE, ROLE_ID, PASSWORD_REQUIRED, AUTHENTICATION_TYPE
    FROM dba_roles
    WHERE ORACLE_MAINTAINED = 'Y'
    ORDER BY ROLE_ID DESC;
EXCEPTION
  WHEN OTHERS THEN
    RAISE_APPLICATION_ERROR(-20002, 'Lỗi khi lấy danh sách Role: ' || SQLERRM);
END;
/
-- Tạo role và cấp quyền
create or replace PROCEDURE SP_TAO_ROLE_VA_CAP_QUYEN (
    p_ten_role IN VARCHAR2,
    p_quyen    IN VARCHAR2, -- Ví dụ: 'INSERT', 'SELECT', 'UPDATE'
    p_ten_bang IN VARCHAR2  -- Ví dụ: 'NHANVIEN'
) 
IS
    v_count NUMBER;
    v_sql_grant VARCHAR2(500);
BEGIN
EXECUTE IMMEDIATE 'ALTER SESSION SET "_ORACLE_SCRIPT"=true';
    -- 1. Kiểm tra xem role đã tồn tại chưa
    SELECT COUNT(*) INTO v_count 
    FROM dba_roles 
    WHERE role = UPPER(p_ten_role);

    -- 2. Nếu chưa có -> Tạo role mới
    IF v_count = 0 THEN
        EXECUTE IMMEDIATE 'CREATE ROLE ' || p_ten_role;
    END IF;

    -- 3. Tạo câu lệnh gán quyền trên BẢNG cụ thể
    -- Cú pháp chuẩn: GRANT quyền ON bảng TO role
    v_sql_grant := 'GRANT ' || p_quyen || ' ON ' || p_ten_bang || ' TO ' || p_ten_role;

    -- 4. Thực thi gán quyền
    EXECUTE IMMEDIATE v_sql_grant;

EXCEPTION
    WHEN OTHERS THEN
        -- Ghi log lỗi vào DBMS_OUTPUT hoặc bạn có thể dùng RAISE_APPLICATION_ERROR để C# bắt lỗi
        RAISE_APPLICATION_ERROR(-20001, 'Lỗi khi xử lý Role: ' || SQLERRM);
END;
/



-- =================================================================
-- PHAN HE 2 - TASK 1: RBAC CHO KY THUAT VIEN VA BENH NHAN
-- =================================================================
-- Muc tieu:
-- 1. KTV chi xem cac dong HSBA_DV duoc phan cong cho minh va chi cap nhat KETQUA.
-- 2. KTV va Benh nhan chi xem/sua thong tin ca nhan cua chinh minh theo cac cot duoc phep.
-- 3. UI WinForms dang nhap bang user Oracle that va truy cap qua cac view duoc grant theo role.

ALTER SESSION SET "_ORACLE_SCRIPT"=true;

BEGIN
    EXECUTE IMMEDIATE 'CREATE ROLE ROLE_KYTHUATVIEN';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -1921 THEN RAISE; END IF;
END;
/

BEGIN
    EXECUTE IMMEDIATE 'CREATE ROLE ROLE_BENHNHAN';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -1921 THEN RAISE; END IF;
END;
/

CREATE OR REPLACE VIEW ADMIN_PHANHE1.V_RBAC_KTV_THONGTIN AS
SELECT MANV, HOTEN, PHAI, NGAYSINH, CMND, QUEQUAN, SODT, VAITRO, CHUYENKHOA, COSO
FROM ADMIN_PHANHE1.NHANVIEN
WHERE 'C##' || UPPER(MANV) = SYS_CONTEXT('USERENV', 'SESSION_USER')
WITH CHECK OPTION CONSTRAINT CK_V_RBAC_KTV_SELF;

CREATE OR REPLACE VIEW ADMIN_PHANHE1.V_RBAC_KTV_DICHVU AS
SELECT MAHSBA, LOAIDV, NGAYDV, MAKTV, KETQUA
FROM ADMIN_PHANHE1.HSBA_DV
WHERE 'C##' || UPPER(MAKTV) = SYS_CONTEXT('USERENV', 'SESSION_USER')
WITH CHECK OPTION CONSTRAINT CK_V_RBAC_KTV_DICHVU;

CREATE OR REPLACE VIEW ADMIN_PHANHE1.V_RBAC_BENHNHAN_THONGTIN AS
SELECT MABN, TENBN, PHAI, NGAYSINH, CCCD, SONHA, TENDUONG, QUANHUYEN, TINHTP,
       TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC
FROM ADMIN_PHANHE1.BENHNHAN
WHERE 'C##' || UPPER(MABN) = SYS_CONTEXT('USERENV', 'SESSION_USER')
WITH CHECK OPTION CONSTRAINT CK_V_RBAC_BENHNHAN_SELF;

GRANT SELECT ON ADMIN_PHANHE1.V_RBAC_KTV_THONGTIN TO ROLE_KYTHUATVIEN;
GRANT UPDATE (QUEQUAN, SODT, COSO) ON ADMIN_PHANHE1.V_RBAC_KTV_THONGTIN TO ROLE_KYTHUATVIEN;
GRANT SELECT ON ADMIN_PHANHE1.V_RBAC_KTV_DICHVU TO ROLE_KYTHUATVIEN;
GRANT UPDATE (KETQUA) ON ADMIN_PHANHE1.V_RBAC_KTV_DICHVU TO ROLE_KYTHUATVIEN;

GRANT SELECT ON ADMIN_PHANHE1.V_RBAC_BENHNHAN_THONGTIN TO ROLE_BENHNHAN;
GRANT UPDATE (SONHA, TENDUONG, QUANHUYEN, TINHTP, TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC)
ON ADMIN_PHANHE1.V_RBAC_BENHNHAN_THONGTIN TO ROLE_BENHNHAN;

CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_SYNC_TASK1_RBAC_USERS (
    p_DEFAULT_PASSWORD IN VARCHAR2 DEFAULT '123456'
)
AUTHID CURRENT_USER
IS
    v_sql VARCHAR2(1000);
    v_username VARCHAR2(128);
    v_count NUMBER;

    PROCEDURE ensure_user(p_username IN VARCHAR2) IS
        v_safe_username VARCHAR2(128);
    BEGIN
        v_safe_username := DBMS_ASSERT.SIMPLE_SQL_NAME(UPPER(TRIM(p_username)));
        SELECT COUNT(*) INTO v_count FROM DBA_USERS WHERE USERNAME = v_safe_username;

        IF v_count = 0 THEN
            v_sql := 'CREATE USER ' || v_safe_username ||
                     ' IDENTIFIED BY "' || REPLACE(p_DEFAULT_PASSWORD, '"', '""') || '"';
            EXECUTE IMMEDIATE v_sql;
        END IF;

        EXECUTE IMMEDIATE 'GRANT CREATE SESSION TO ' || v_safe_username;
    END;
BEGIN
    EXECUTE IMMEDIATE 'ALTER SESSION SET "_ORACLE_SCRIPT"=true';

    FOR rec IN (SELECT MANV, VAITRO FROM ADMIN_PHANHE1.NHANVIEN)
    LOOP
        v_username := 'C##' || UPPER(rec.MANV);
        ensure_user(v_username);

        IF REGEXP_LIKE(LOWER(rec.VAITRO), 'thu.*t.*vi') THEN
            EXECUTE IMMEDIATE 'GRANT ROLE_KYTHUATVIEN TO ' || DBMS_ASSERT.SIMPLE_SQL_NAME(v_username);
        ELSE
            BEGIN
                EXECUTE IMMEDIATE 'REVOKE ROLE_KYTHUATVIEN FROM ' || DBMS_ASSERT.SIMPLE_SQL_NAME(v_username);
            EXCEPTION
                WHEN OTHERS THEN NULL;
            END;
        END IF;
    END LOOP;

    FOR rec IN (SELECT MABN FROM ADMIN_PHANHE1.BENHNHAN)
    LOOP
        v_username := 'C##' || UPPER(rec.MABN);
        ensure_user(v_username);
        EXECUTE IMMEDIATE 'GRANT ROLE_BENHNHAN TO ' || DBMS_ASSERT.SIMPLE_SQL_NAME(v_username);
    END LOOP;
END;
/

BEGIN
    ADMIN_PHANHE1.SP_SYNC_TASK1_RBAC_USERS('123456');
END;
/

-- TEST NHANH TASK 1:
-- Dang nhap C##NV007 / 123456:
--   SELECT * FROM ADMIN_PHANHE1.V_RBAC_KTV_THONGTIN;
--   SELECT * FROM ADMIN_PHANHE1.V_RBAC_KTV_DICHVU;
--   UPDATE ADMIN_PHANHE1.V_RBAC_KTV_DICHVU SET KETQUA = N'Test KTV cap nhat ket qua' WHERE MAHSBA = 'HS000001';
--   UPDATE ADMIN_PHANHE1.V_RBAC_KTV_DICHVU SET MAKTV = 'NV008' WHERE MAHSBA = 'HS000001'; -- phai bi chan ORA-01031
-- Dang nhap C##BN000001 / 123456:
--   SELECT * FROM ADMIN_PHANHE1.V_RBAC_BENHNHAN_THONGTIN;
--   UPDATE ADMIN_PHANHE1.V_RBAC_BENHNHAN_THONGTIN SET SONHA = N'999';
--   UPDATE ADMIN_PHANHE1.V_RBAC_BENHNHAN_THONGTIN SET TENBN = N'Khong duoc sua'; -- phai bi chan ORA-01031

-- =================================================================
-- BACKUP && RESTORE (START)
-- =================================================================

-- THAY ĐÔI THÀNH ĐƯỜNG DẪN THÍCH HỢP TRONG WINDOW: 1 THƯ MỤC ĐỂ LƯU CÁC FILE BACKUP CD: C:\Backup_Oracle
CREATE OR REPLACE DIRECTORY BACKUP_DIR AS '/backup'; 
GRANT READ, WRITE ON DIRECTORY BACKUP_DIR TO system;
GRANT READ, WRITE ON DIRECTORY BACKUP_DIR TO ADMIN_PHANHE1;

-- =================================================================
-- BACKUP && RESTORE: DATA PUMP
-- =================================================================

CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_BACKUP_DATAPUMP 
AUTHID CURRENT_USER -- BẮT BUỘC: Ép Oracle giữ nguyên quyền của người gọi
IS
    v_dp_handle NUMBER;
BEGIN
    -- Thêm SS (Giây) vào tên Job để đảm bảo chạy 100 lần 1 phút vẫn không trùng tên
    v_dp_handle := DBMS_DATAPUMP.OPEN(
        operation   => 'EXPORT',
        job_mode    => 'SCHEMA',
        job_name    => 'JOB_EXPDP_' || TO_CHAR(SYSDATE, 'YYYYMMDD_HH24MISS') 
    );

    DBMS_DATAPUMP.ADD_FILE(
        handle    => v_dp_handle,
        filename  => 'BV_PHANHE1.dmp',
        directory => 'BACKUP_DIR',
        reusefile => 1 
    );
    DBMS_DATAPUMP.ADD_FILE(
        handle    => v_dp_handle,
        filename  => 'BV_PHANHE1.log',
        directory => 'BACKUP_DIR',
        filetype  => DBMS_DATAPUMP.KU$_FILE_TYPE_LOG_FILE,
        reusefile => 1
    );

    DBMS_DATAPUMP.METADATA_FILTER(
        handle => v_dp_handle,
        name   => 'SCHEMA_EXPR',
        value  => 'IN (''ADMIN_PHANHE1'')'
    );

    DBMS_DATAPUMP.START_JOB(v_dp_handle);
    DBMS_DATAPUMP.DETACH(v_dp_handle);
    
    DBMS_OUTPUT.PUT_LINE('Da gui yeu cau Backup Data Pump vao he thong.');
EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Loi Data Pump: ' || SQLERRM);
        RAISE;
END SP_BACKUP_DATAPUMP;
/



CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_RESTORE_DATAPUMP (
    p_table_name IN VARCHAR2 DEFAULT NULL 
) 
AUTHID CURRENT_USER
IS
    v_dp_handle NUMBER;
BEGIN
    v_dp_handle := DBMS_DATAPUMP.OPEN(
        operation => 'IMPORT',
        job_mode  => CASE WHEN p_table_name IS NULL THEN 'SCHEMA' ELSE 'TABLE' END,
        job_name  => 'JOB_IMPDP_' || TO_CHAR(SYSDATE, 'YYYYMMDD_HH24MISS')
    );

    DBMS_DATAPUMP.ADD_FILE(
        handle    => v_dp_handle,
        filename  => 'BV_PHANHE1.dmp',
        directory => 'BACKUP_DIR'
    );

    IF p_table_name IS NOT NULL THEN
        DBMS_DATAPUMP.METADATA_FILTER(
            handle => v_dp_handle,
            name   => 'NAME_EXPR',
            value  => 'IN (''' || UPPER(REPLACE(p_table_name, 'ADMIN_PHANHE1.', '')) || ''')'
        );
    ELSE
        DBMS_DATAPUMP.METADATA_FILTER(
            handle => v_dp_handle,
            name   => 'SCHEMA_EXPR',
            value  => 'IN (''ADMIN_PHANHE1'')'
        );
    END IF;

    DBMS_DATAPUMP.SET_PARAMETER(
        handle => v_dp_handle,
        name   => 'TABLE_EXISTS_ACTION',
        value  => 'REPLACE'
    );

    DBMS_DATAPUMP.START_JOB(v_dp_handle);
    DBMS_DATAPUMP.DETACH(v_dp_handle);
    
    DBMS_OUTPUT.PUT_LINE('Da gui yeu cau Restore Data Pump vao he thong.');
END SP_RESTORE_DATAPUMP;
/

-- CHECK 
-- SELECT OBJECT_NAME, OBJECT_TYPE, ORACLE_MAINTAINED 
-- FROM DBA_OBJECTS 
-- WHERE OWNER = 'ADMIN_PHANHE1';



-- =================================================================
-- BACKUP && RESTORE: FLASHBACK - KHÔI PHỤC DỮ LIỆU TỪ AUDIT LOG
-- =================================================================

-- CẤP CÁC QUYỀN CẦN THIẾT ĐỂ BACKUP VÀ RESTORE
-- TẠO VÀ CẤP QUYỀN ĐỂ DÙNG FLASHBACK RESTORE
-- 1. Tao mot kho luu tru ngam ten la FDA_PHANHE1, dung luong toi da 1GB, luu lich su 1 nam
CREATE FLASHBACK ARCHIVE fda_phanhe1 TABLESPACE USERS QUOTA 1G RETENTION 1 YEAR;
-- 2. Cap quyen cho user do an duoc su dung kho luu tru nay
GRANT FLASHBACK ARCHIVE ON fda_phanhe1 TO ADMIN_PHANHE1;
-- 3. Bat che do "Di chuyen dong" (Bat buoc de Flashback hoat dong)
ALTER TABLE ADMIN_PHANHE1.HSBA ENABLE ROW MOVEMENT;
ALTER TABLE ADMIN_PHANHE1.HSBA_DV ENABLE ROW MOVEMENT;
ALTER TABLE ADMIN_PHANHE1.DONTHUOC ENABLE ROW MOVEMENT;
-- 4. Gan kho luu tru FDA vao cac bang de Oracle bat dau ghi log lich su vinh vien
ALTER TABLE ADMIN_PHANHE1.HSBA FLASHBACK ARCHIVE fda_phanhe1;
ALTER TABLE ADMIN_PHANHE1.HSBA_DV FLASHBACK ARCHIVE fda_phanhe1;
ALTER TABLE ADMIN_PHANHE1.DONTHUOC FLASHBACK ARCHIVE fda_phanhe1;


CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_RESTORE_FLASHBACK (
    p_table_name IN VARCHAR2,
    p_safe_time  IN VARCHAR2 
)
AUTHID CURRENT_USER 
IS
    v_sql_query VARCHAR2(1000);
BEGIN
    -- Chỉ cho phép khôi phục những bảng nằm trong danh sách Audit (Bảo mật 2 lớp)
    IF UPPER(p_table_name) NOT IN ('ADMIN_PHANHE1.HSBA', 'ADMIN_PHANHE1.HSBA_DV', 'ADMIN_PHANHE1.DONTHUOC') THEN
        RAISE_APPLICATION_ERROR(-20001, 'Bao mat: Chi duoc phep khoi phuc cac bang nam trong dien Kiem toan!');
    END IF;

    v_sql_query := 'FLASHBACK TABLE ' || p_table_name || 
                   ' TO TIMESTAMP TO_TIMESTAMP(''' || p_safe_time || ''', ''YYYY-MM-DD HH24:MI:SS'')';
                   
    DBMS_OUTPUT.PUT_LINE('Thuc thi lenh: ' || v_sql_query);
    EXECUTE IMMEDIATE v_sql_query;
    DBMS_OUTPUT.PUT_LINE('=> Khoi phuc bang ' || p_table_name || ' thanh cong!');

EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Loi Flashback: ' || SQLERRM);
        RAISE; 
END SP_RESTORE_FLASHBACK;
/
-- =================================================================
-- BACKUP && RESTORE (END)
-- =================================================================





-- =====================================================================
-- AUDIT (START)
-- =====================================================================

SELECT object_name,
       policy_name,
       enabled,
       policy_text
FROM dba_audit_policies
WHERE object_name = 'HSBA_DV';



GRANT SELECT ON SYS.DBA_AUDIT_TRAIL TO ADMIN_PHANHE1;
GRANT SELECT ON SYS.DBA_FGA_AUDIT_TRAIL TO ADMIN_PHANHE1;

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
    -- NỬA DƯỚI: DỮ LIỆU TỪ FINE-GRAINED AUDIT (DÙNG BẢNG FGA GỐC)
    -- =========================================================
    SELECT 
        'FINE-GRAINED' AS "LOAI_AUDIT",
        DB_USER AS "NGUOI_DUNG", -- Đã đổi tên cột cho khớp
        TO_CHAR(EXTENDED_TIMESTAMP, 'YYYY/MM/DD HH24:MI:SS') AS "THOI_GIAN", 
        'POLICY: ' || POLICY_NAME AS "HANH_DONG", -- Đã đổi tên cột
        OBJECT_SCHEMA || '.' || OBJECT_NAME AS "DOI_TUONG",
        CAST(SQL_TEXT AS VARCHAR2(2000)) AS "CAU_LENH_SQL",
        CASE 
            WHEN POLICY_NAME LIKE '%BATHOPPHAP%' THEN 'Thành công (Hành vi bất hợp pháp)'
            WHEN POLICY_NAME LIKE '%HOPPHAP%' THEN 'Thành công (Hành vi đúng thẩm quyền)'
            ELSE 'Thành công (Đã ghi nhận)'
        END AS "CHI_TIET_TRANG_THAI",
        CAST(EXTENDED_TIMESTAMP AS TIMESTAMP) AS RAW_TIME 
    FROM 
        DBA_FGA_AUDIT_TRAIL -- Chuyển hướng sang bảng FGA truyền thống
    WHERE 
        POLICY_NAME IS NOT NULL 
        AND DB_USER NOT IN ('SYS', 'SYSTEM')
)
-- Sắp xếp bằng dữ liệu thời gian thật, dữ liệu đổ ra UI sẽ chuẩn xác tuyệt đối
ORDER BY RAW_TIME DESC;

-- select * from admin_phanhe1.v_all_audit_log;


-- =====================================================================
-- CÀI ĐẶT TÌNH HUỐNG STANDARD AUDIT
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
AUDIT SELECT ON V_ALL_AUDIT_LOG BY ACCESS;

-- =====================================================================
-- NGỮ CẢNH 4: Giám sát Stored Procedure
-- Ý nghĩa: Giám sát việc thực thi các thủ tục có tính rủi ro cao. 
-- Ví dụ: Giám sát xem ai đã chạy thủ tục tạo hồ sơ bệnh án mới.
-- =====================================================================
-- Lưu ý: Đổi 'SP_TAO_HSBA' thành tên Procedure thực tế của nhóm.
AUDIT EXECUTE ON ADMIN_PHANHE1.SP_RESTORE_FLASHBACK BY ACCESS;

-- =====================================================================
-- NGỮ CẢNH 5: Giám sát Cấu trúc (DDL) - Bảo vệ Schema
-- Ý nghĩa: Phát hiện các hành vi cố tình thay đổi cấu trúc bảng (CREATE, ALTER, DROP, TRUNCATE) 
-- =====================================================================
AUDIT TABLE BY ACCESS;


-- =====================================================================
-- BƯỚC 1: XÓA SẠCH CÁC POLICY CŨ BỊ LỖI CHÍNH TẢ ĐỂ LÀM SẠCH BẢNG
-- =====================================================================
BEGIN
    -- Xóa trên bảng DONTHUOC
    BEGIN DBMS_FGA.DROP_POLICY('ADMIN_PHANHE1', 'DONTHUOC', 'FGA_DONTHUOC_CAPNHAT_SAUDINH'); EXCEPTION WHEN OTHERS THEN NULL; END;
    
    -- Xóa trên bảng HSBA
    BEGIN DBMS_FGA.DROP_POLICY('ADMIN_PHANHE1', 'HSBA', 'FGA_HSBA_CAPNHAT_HOPPHAP'); EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN DBMS_FGA.DROP_POLICY('ADMIN_PHANHE1', 'HSBA', 'FGA_HSBA_CAPNHAT_BATHOPPHAP'); EXCEPTION WHEN OTHERS THEN NULL; END;
    
    -- Xóa trên bảng HSBA_DV
    BEGIN DBMS_FGA.DROP_POLICY('ADMIN_PHANHE1', 'HSBA_DV', 'FGA_HSBADV_INS_DEL_BATHOPPHAP'); EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN DBMS_FGA.DROP_POLICY('ADMIN_PHANHE1', 'HSBA_DV', 'FGA_HSBADV_UPD_COT_CAM_BATHOPPHAP'); EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN DBMS_FGA.DROP_POLICY('ADMIN_PHANHE1', 'HSBA_DV', 'FGA_HSBADV_UPD_MAKTV_BATHOPPHAP'); EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN DBMS_FGA.DROP_POLICY('ADMIN_PHANHE1', 'HSBA_DV', 'FGA_HSBADV_UPD_KETQUA_BATHOPPHAP'); EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN DBMS_FGA.DROP_POLICY('ADMIN_PHANHE1', 'HSBA_DV', 'FGA_HSBADV_KTV_CAPNHAT_HOPPHAP'); EXCEPTION WHEN OTHERS THEN NULL; END;
END;
/

-- =====================================================================
-- CÀI ĐẶT TÌNH HUỐNG FINE-GRAINED AUDIT
-- =====================================================================

-- 1. Hàm cho 3a (Bảng DONTHUOC)
CREATE OR REPLACE FUNCTION ADMIN_PHANHE1.FN_FGA_DONTHUOC_SAUDINH(p_MAHSBA VARCHAR2) RETURN VARCHAR2 AS
    v_is_bs VARCHAR2(10);
    v_count NUMBER;
BEGIN
    v_is_bs := SYS_CONTEXT('SYS_SESSION_ROLES', 'ROLE_YSI_BACSI');
    -- Dùng Subquery trong PL/SQL thì vô tư, không bị FGA cấm
    SELECT COUNT(*) INTO v_count FROM ADMIN_PHANHE1.HSBA 
    WHERE MAHSBA = p_MAHSBA AND 'C##' || UPPER(MABS) = SYS_CONTEXT('USERENV', 'SESSION_USER');
    
    IF v_is_bs = 'TRUE' AND v_count > 0 THEN RETURN 'TRUE'; ELSE RETURN 'FALSE'; END IF;
EXCEPTION WHEN OTHERS THEN RETURN 'FALSE'; END;
/

-- 2. Hàm cho 3b (Bảng HSBA - Hợp pháp)
CREATE OR REPLACE FUNCTION ADMIN_PHANHE1.FN_FGA_HSBA_HOPPHAP(p_MABS VARCHAR2) RETURN VARCHAR2 AS
BEGIN
    IF SYS_CONTEXT('SYS_SESSION_ROLES', 'ROLE_YSI_BACSI') = 'TRUE' AND 
       SYS_CONTEXT('USERENV', 'SESSION_USER') = 'C##' || UPPER(p_MABS) THEN
        RETURN 'TRUE';
    ELSE RETURN 'FALSE'; END IF;
EXCEPTION WHEN OTHERS THEN RETURN 'FALSE'; END;
/

-- 3. Hàm cho 3c (Bảng HSBA - Bất hợp pháp)
CREATE OR REPLACE FUNCTION ADMIN_PHANHE1.FN_FGA_HSBA_BATHOPPHAP(p_MABS VARCHAR2) RETURN VARCHAR2 AS
BEGIN
    IF SYS_CONTEXT('USERENV', 'SESSION_USER') != 'C##' || UPPER(p_MABS) THEN RETURN 'TRUE'; ELSE RETURN 'FALSE'; END IF;
EXCEPTION WHEN OTHERS THEN RETURN 'FALSE'; END;
/

-- 4. Hàm cho 3d.1 (Bảng HSBA_DV - Kiểm tra KHÔNG phải Bác sĩ)
CREATE OR REPLACE FUNCTION ADMIN_PHANHE1.FN_FGA_NOT_BACSI RETURN VARCHAR2 AS
BEGIN
    IF SYS_CONTEXT('SYS_SESSION_ROLES', 'ROLE_YSI_BACSI') = 'FALSE' THEN RETURN 'TRUE'; ELSE RETURN 'FALSE'; END IF;
EXCEPTION WHEN OTHERS THEN RETURN 'FALSE'; END;
/

-- 5. Hàm cho 3d.3 (Bảng HSBA_DV - Kiểm tra KHÔNG phải Điều phối viên)
CREATE OR REPLACE FUNCTION ADMIN_PHANHE1.FN_FGA_NOT_DPV RETURN VARCHAR2 AS
BEGIN
    IF SYS_CONTEXT('SYS_SESSION_ROLES', 'ROLE_DIEUPHOIVIEN') = 'FALSE' THEN RETURN 'TRUE'; ELSE RETURN 'FALSE'; END IF;
EXCEPTION WHEN OTHERS THEN RETURN 'FALSE'; END;
/

-- 6. Hàm cho 3d.4 (Bảng HSBA_DV - Kiểm tra KHÔNG phải KTV phụ trách)
CREATE OR REPLACE FUNCTION ADMIN_PHANHE1.FN_FGA_NOT_KTV_CHINHLU(p_MAKTV VARCHAR2) RETURN VARCHAR2 AS
BEGIN
    IF SYS_CONTEXT('USERENV', 'SESSION_USER') != 'C##' || UPPER(p_MAKTV) THEN RETURN 'TRUE'; ELSE RETURN 'FALSE'; END IF;
EXCEPTION WHEN OTHERS THEN RETURN 'FALSE'; END;
/

-- 7. Hàm cho 3d.5 (Bảng HSBA_DV - Kiểm tra KTV Hợp pháp)
CREATE OR REPLACE FUNCTION ADMIN_PHANHE1.FN_FGA_KTV_HOPPHAP(p_MAKTV VARCHAR2) RETURN VARCHAR2 AS
BEGIN
    IF SYS_CONTEXT('SYS_SESSION_ROLES', 'ROLE_KYTHUATVIEN') = 'TRUE' AND 
       SYS_CONTEXT('USERENV', 'SESSION_USER') = 'C##' || UPPER(p_MAKTV) THEN
        RETURN 'TRUE';
    ELSE RETURN 'FALSE'; END IF;
EXCEPTION WHEN OTHERS THEN RETURN 'FALSE'; END;
/


BEGIN
    -- 3a. Giám sát Bác sĩ cập nhật ĐƠN THUỐC sau khi đã chỉ định
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN_PHANHE1',
        object_name     => 'DONTHUOC',
        policy_name     => 'FGA_DONTHUOC_CAPNHAT_SAUDINH',
        audit_column    => 'MAHSBA, NGAYDT, TENTHUOC, LIEUDUNG',
        audit_condition => 'ADMIN_PHANHE1.FN_FGA_DONTHUOC_SAUDINH(MAHSBA) = ''TRUE''',
        statement_types => 'UPDATE'
    );

    -- 3b. Giám sát hành vi Bác sĩ cập nhật HỢP PHÁP trên HSBA
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN_PHANHE1',
        object_name     => 'HSBA',
        policy_name     => 'FGA_HSBA_CAPNHAT_HOPPHAP',
        audit_column    => 'CHANDOAN, DIEUTRI, KETLUAN',
        audit_condition => 'ADMIN_PHANHE1.FN_FGA_HSBA_HOPPHAP(MABS) = ''TRUE''',
        statement_types => 'UPDATE'
    );

    -- 3c. Giám sát hành vi cập nhật BẤT HỢP PHÁP trên HSBA
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN_PHANHE1',
        object_name     => 'HSBA',
        policy_name     => 'FGA_HSBA_CAPNHAT_BATHOPPHAP',
        audit_column    => 'CHANDOAN, DIEUTRI, KETLUAN',
        audit_condition => 'ADMIN_PHANHE1.FN_FGA_HSBA_BATHOPPHAP(MABS) = ''TRUE''',
        statement_types => 'UPDATE'
    );

    -- 3d.1. Bắt lỗi INSERT, DELETE bất hợp pháp trên HSBA_DV
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN_PHANHE1',
        object_name     => 'HSBA_DV',
        policy_name     => 'FGA_HSBADV_INS_DEL_BATHOPPHAP',
        -- Vì không truyền tham số cột nào vào nên bỏ ngoặc
        audit_condition => 'ADMIN_PHANHE1.FN_FGA_NOT_BACSI() = ''TRUE''',
        statement_types => 'INSERT, DELETE'
    );

    -- 3d.2. Bắt lỗi UPDATE bất hợp pháp lên các cột "Cấm sửa"
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN_PHANHE1',
        object_name     => 'HSBA_DV',
        policy_name     => 'FGA_HSBADV_UPD_COT_CAM_BATHOPPHAP',
        audit_column    => 'MAHSBA, LOAIDV, NGAYDV',
        audit_condition => NULL, -- Giữ nguyên NULL vì bắt tất cả
        statement_types => 'UPDATE'
    );

    -- 3d.3. Bắt lỗi UPDATE bất hợp pháp lên cột MAKTV
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN_PHANHE1',
        object_name     => 'HSBA_DV',
        policy_name     => 'FGA_HSBADV_UPD_MAKTV_BATHOPPHAP',
        audit_column    => 'MAKTV',
        audit_condition => 'ADMIN_PHANHE1.FN_FGA_NOT_DPV() = ''TRUE''',
        statement_types => 'UPDATE'
    );

    -- 3d.4. Bắt lỗi UPDATE bất hợp pháp lên cột KETQUA
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN_PHANHE1',
        object_name     => 'HSBA_DV',
        policy_name     => 'FGA_HSBADV_UPD_KETQUA_BATHOPPHAP',
        audit_column    => 'KETQUA',
        audit_condition => 'ADMIN_PHANHE1.FN_FGA_NOT_KTV_CHINHLU(MAKTV) = ''TRUE''',
        statement_types => 'UPDATE'
    );

    -- 3d.5. KTV cập nhật HỢP PHÁP
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN_PHANHE1',
        object_name     => 'HSBA_DV',
        policy_name     => 'FGA_HSBADV_KTV_CAPNHAT_HOPPHAP',
        audit_column    => 'KETQUA',
        audit_condition => 'ADMIN_PHANHE1.FN_FGA_KTV_HOPPHAP(MAKTV) = ''TRUE''',
        statement_types => 'UPDATE'
    );   
END;
/

-- =====================================================================
-- AUDIT (END)
-- =====================================================================
