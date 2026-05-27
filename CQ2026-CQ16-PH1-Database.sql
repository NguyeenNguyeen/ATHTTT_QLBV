DROP TABLE THONG_BAO;
DROP TABLE HSBA_DV;
DROP TABLE DONTHUOC;
/
DROP TABLE HSBA;
/
DROP TABLE KHOA;
DROP TABLE NHANVIEN;
DROP TABLE BENHNHAN;
/

DROP USER ADMIN_PHANHE1 CASCADE;
/

-- Bỏ qua lớp container bảo mật của Oracle 12c+ để tạo user local dễ dàng
ALTER SESSION SET "_ORACLE_SCRIPT"=true; 

-- Tạo user dùng chung cho cả nhóm
CREATE USER ADMIN_PHANHE1 IDENTIFIED BY "Admin@123456";

-- Cấp toàn quyền quản trị (DBA)
GRANT DBA TO ADMIN_PHANHE1;

GRANT ALTER SYSTEM TO ADMIN_PHANHE1;

GRANT SELECT ON v_$session TO ADMIN_PHANHE1;
/


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
