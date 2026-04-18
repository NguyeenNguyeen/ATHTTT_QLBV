-- ==========================================
-- 1. T?O C�C B?NG ??C L?P (Kh�ng c� kh�a ngo?i)
-- ==========================================

-- B?ng KHOA
CREATE TABLE KHOA (
    MAKHOA VARCHAR2(20) PRIMARY KEY,
    TENKHOA NVARCHAR2(100)
);

-- B?ng NH�N VI�N
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
    COSO NVARCHAR2(50) -- B? sung cho OLS (HCM, HN, HP)
);

-- B?ng B?NH NH�N
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

-- B?ng TH�NG B�O (??c l?p, d�ng cho OLS)
CREATE TABLE THONG_BAO (
    MATB VARCHAR2(20) PRIMARY KEY,
    NOIDUNG NVARCHAR2(1000),
    NGAYGIO TIMESTAMP,
    DIADIEM NVARCHAR2(100),
    OLS_LABEL NUMBER(10) -- C?t l?u m� s? c?a nh�n OLS
);


-- ==========================================
-- 2. T?O C�C B?NG PH? THU?C (C� kh�a ngo?i)
-- ==========================================

-- B?ng H? S? B?NH �N (HSBA)
CREATE TABLE HSBA (
    MAHSBA VARCHAR2(20) PRIMARY KEY,
    MABN VARCHAR2(20),
    NGAY DATE,
    CHANDOAN NVARCHAR2(500),
    DIEUTRI NVARCHAR2(500),
    MABS VARCHAR2(20),
    MAKHOA VARCHAR2(20),
    KETLUAN NVARCHAR2(500),
    
    -- R�ng bu?c kh�a ngo?i
    CONSTRAINT FK_HSBA_BENHNHAN FOREIGN KEY (MABN) REFERENCES BENHNHAN(MABN),
    CONSTRAINT FK_HSBA_NHANVIEN FOREIGN KEY (MABS) REFERENCES NHANVIEN(MANV),
    CONSTRAINT FK_HSBA_KHOA FOREIGN KEY (MAKHOA) REFERENCES KHOA(MAKHOA)
);

-- B?ng D?CH V? H? S? B?NH �N (HSBA_DV)
CREATE TABLE HSBA_DV (
    MAHSBA VARCHAR2(20),
    LOAIDV NVARCHAR2(100),
    NGAYDV DATE,
    MAKTV VARCHAR2(20),
    KETQUA NVARCHAR2(500),
    
    -- Kh�a ch�nh k?t h?p
    PRIMARY KEY (MAHSBA, LOAIDV, NGAYDV),
    
    -- R�ng bu?c kh�a ngo?i
    CONSTRAINT FK_HSBADV_HSBA FOREIGN KEY (MAHSBA) REFERENCES HSBA(MAHSBA),
    CONSTRAINT FK_HSBADV_NHANVIEN FOREIGN KEY (MAKTV) REFERENCES NHANVIEN(MANV)
);

-- B?ng ??N THU?C
CREATE TABLE DONTHUOC (
    MAHSBA VARCHAR2(20),
    TENTHUOC NVARCHAR2(100),
    NGAYDT DATE,
    LIEUDUNG NVARCHAR2(200),
    
    -- Kh�a ch�nh k?t h?p
    PRIMARY KEY (MAHSBA, TENTHUOC, NGAYDT),
    
    -- R�ng bu?c kh�a ngo?i
    CONSTRAINT FK_DONTHUOC_HSBA FOREIGN KEY (MAHSBA) REFERENCES HSBA(MAHSBA)
);

-- ---------------------------------------------------------------------------------------------------------------------------------------------------
-- ------------------------------------------------------------------------ PROCEDURE ----------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------------------------------------
-- T?o b? ??m sequence cho b?nh nh�n
CREATE SEQUENCE ADMIN_PHANHE1.SEQ_MABN 
START WITH 1 
INCREMENT BY 1 
NOCACHE 
NOCYCLE;

-- T?o b? ??m sequence cho nh�n vi�n
CREATE SEQUENCE ADMIN_PHANHE1.SEQ_MANV 
START WITH 1 
INCREMENT BY 1 
NOCACHE 
NOCYCLE;

-- T?o procedure th�m B?nh nh�n
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
    p_MABN_OUT OUT VARCHAR2 -- Tham s? OUT c?c k? quan tr?ng ?? tr? m� v? cho C#
)
AUTHID CURRENT_USER
IS
    v_MABN VARCHAR2(20);
    v_sql VARCHAR2(500);
    v_seq_val NUMBER;
BEGIN
    -- B??c 1: Sinh m� b?nh nh�n t? ??ng
    -- L?y s? ??m ti?p theo t? Sequence
    SELECT ADMIN_PHANHE1.SEQ_MABN.NEXTVAL INTO v_seq_val FROM DUAL;
    
    -- ??nh d?ng m� b?nh nh�n (V� d?: BN0001, BN0002...)
    v_MABN := 'BN' || TO_CHAR(v_seq_val, 'FM0000'); 
    
    -- Tr? m� v?a t?o ra cho tham s? OUT
    p_MABN_OUT := v_MABN;

    -- B??c 2: Th�m th�ng tin v�o b?ng BENHNHAN
    INSERT INTO ADMIN_PHANHE1.BENHNHAN 
        (MABN, TENBN, PHAI, NGAYSINH, CCCD, SONHA, TENDUONG, QUANHUYEN, TINHTP, TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC)
    VALUES 
        (v_MABN, p_TENBN, p_PHAI, p_NGAYSINH, p_CCCD, p_SONHA, p_TENDUONG, p_QUANHUYEN, p_TINHTP, p_TIENSUBENH, p_TIENSUBENHGD, p_DIUNGTHUOC);

    -- B??c 3: T?o User Oracle (C## + m� b?nh nh�n t? sinh)
    v_sql := 'CREATE USER C##' || v_MABN || ' IDENTIFIED BY "' || p_MATKHAU || '"';
    EXECUTE IMMEDIATE v_sql;

    -- B??c 4: C?p quy?n k?t n?i
    v_sql := 'GRANT CREATE SESSION TO C##' || v_MABN;
    EXECUTE IMMEDIATE v_sql;

    -- Ho�n t?t to�n b? giao d?ch
    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        -- N?u b? l?i ? b?t k? b??c n�o, Rollback ngay l?p t?c
        ROLLBACK;
        RAISE; 
END;
/

-- T?o procedure th�m Nh�n vi�n
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
    p_MANV_OUT OUT VARCHAR2 -- Tham s? OUT tr? m� NV v? cho giao di?n WinForm
)
AUTHID CURRENT_USER
IS
    v_MANV VARCHAR2(20);
    v_sql VARCHAR2(500);
    v_seq_val NUMBER;
BEGIN
    -- B??c 1: Sinh m� nh�n vi�n t? ??ng
    -- L?y s? ??m ti?p theo t? Sequence c?a Nh�n Vi�n
    SELECT ADMIN_PHANHE1.SEQ_MANV.NEXTVAL INTO v_seq_val FROM DUAL;
    
    -- ??nh d?ng m� (V� d?: NV0001, NV0002...)
    v_MANV := 'NV' || TO_CHAR(v_seq_val, 'FM0000'); 
    
    -- Tr? m� v?a t?o ra cho tham s? OUT ?? C# h?ng l?y
    p_MANV_OUT := v_MANV;

    -- B??c 2: Th�m th�ng tin v�o b?ng NHANVIEN
    INSERT INTO ADMIN_PHANHE1.NHANVIEN 
        (MANV, HOTEN, PHAI, NGAYSINH, CMND, QUEQUAN, SODT, VAITRO, CHUYENKHOA, COSO)
    VALUES 
        (v_MANV, p_HOTEN, p_PHAI, p_NGAYSINH, p_CMND, p_QUEQUAN, p_SODT, p_VAITRO, p_CHUYENKHOA, p_COSO);

    -- B??c 3: T?o User Oracle (C## + m� NV t? sinh)
    v_sql := 'CREATE USER C##' || v_MANV || ' IDENTIFIED BY "' || p_MATKHAU || '"';
    EXECUTE IMMEDIATE v_sql;

    -- B??c 4: C?p quy?n k?t n?i c? b?n
    v_sql := 'GRANT CREATE SESSION TO C##' || v_MANV;
    EXECUTE IMMEDIATE v_sql;

    -- Ho�n t?t to�n b? giao d?ch, l?u d? li?u v?nh vi?n
    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        -- N?u c� l?i (VD: thi?u tr??ng b?t bu?c, tr�ng CMND n?u c� set UNIQUE...), t? ??ng h?y to�n b? thao t�c
        ROLLBACK;
        RAISE; 
END;
/


-- Xem nh�n vi�n theo m�
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XEM_MOT_NHANVIEN (
    p_MANV IN VARCHAR2,
    p_CURSOR OUT SYS_REFCURSOR -- Con tr? ch?a b?ng d? li?u tr? v? cho C#
)
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT MANV AS "M� NV", 
           HOTEN AS "H? T�n", 
           PHAI AS "Ph�i", 
           TO_CHAR(NGAYSINH, 'DD/MM/YYYY') AS "Ng�y Sinh", 
           CMND AS "CMND/CCCD", 
           QUEQUAN AS "Qu� Qu�n", 
           SODT AS "S? ?T", 
           VAITRO AS "Vai Tr�", 
           CHUYENKHOA AS "Chuy�n Khoa", 
           COSO AS "C? S?"
    FROM ADMIN_PHANHE1.NHANVIEN
    WHERE MANV = p_MANV;
END;
/

-- Xem t?t c? nh�n vi�n
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XEM_ALL_NHANVIEN (
    p_CURSOR OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT MANV AS "M� NV", 
           HOTEN AS "H? T�n", 
           PHAI AS "Ph�i", 
           TO_CHAR(NGAYSINH, 'DD/MM/YYYY') AS "Ng�y Sinh", 
           CMND AS "CMND/CCCD", 
           QUEQUAN AS "Qu� Qu�n", 
           SODT AS "S? ?T", 
           VAITRO AS "Vai Tr�", 
           CHUYENKHOA AS "Chuy�n Khoa", 
           COSO AS "C? S?"
    FROM ADMIN_PHANHE1.NHANVIEN
    ORDER BY MANV;
END;
/

-- Xem danh s�ch b?nh nh�n theo m� b?nh nh�n
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XEM_MOT_BENHNHAN (
    p_MABN IN VARCHAR2,
    p_CURSOR OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT MABN AS "M� BN", 
           TENBN AS "T�n B?nh Nh�n", 
           PHAI AS "Ph�i", 
           TO_CHAR(NGAYSINH, 'DD/MM/YYYY') AS "Ng�y Sinh", 
           CCCD AS "CCCD", 
           SONHA || ', ' || TENDUONG AS "??a Ch?", 
           QUANHUYEN AS "Qu?n/Huy?n", 
           TINHTP AS "T?nh/TP", 
           TIENSUBENH AS "Ti?n S? B?nh", 
           TIENSUBENHGD AS "TS B?nh Gia ?�nh", 
           DIUNGTHUOC AS "D? ?ng Thu?c"
    FROM ADMIN_PHANHE1.BENHNHAN
    WHERE MABN = p_MABN;
END;
/

-- Xem danh s�ch t?t c? b?nh nh�n
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XEM_ALL_BENHNHAN (
    p_CURSOR OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT MABN AS "M� BN", 
           TENBN AS "T�n B?nh Nh�n", 
           PHAI AS "Ph�i", 
           TO_CHAR(NGAYSINH, 'DD/MM/YYYY') AS "Ng�y Sinh", 
           CCCD AS "CCCD", 
           SONHA || ', ' || TENDUONG AS "??a Ch?", 
           QUANHUYEN AS "Qu?n/Huy?n", 
           TINHTP AS "T?nh/TP", 
           TIENSUBENH AS "Ti?n S? B?nh", 
           TIENSUBENHGD AS "TS B?nh Gia ?�nh", 
           DIUNGTHUOC AS "D? ?ng Thu?c"
    FROM ADMIN_PHANHE1.BENHNHAN
    ORDER BY MABN;
END;
/

-- X�a b?nh nh�n
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XOA_BENHNHAN (
    p_MABN IN VARCHAR2
)
AUTHID CURRENT_USER
IS
    v_sql VARCHAR2(500);
    v_username VARCHAR2(50) := 'C##' || UPPER(p_MABN);
BEGIN
    -- K�ch ho?t c? b? qua b?o m?t Container 12c+
    EXECUTE IMMEDIATE 'ALTER SESSION SET "_ORACLE_SCRIPT"=true';

    -- B??c 0: T? ??ng ng?t t?t c? c�c k?t n?i hi?n t?i c?a t�i kho?n n�y
    FOR rec IN (SELECT sid, serial# FROM v$session WHERE username = v_username)
    LOOP
        EXECUTE IMMEDIATE 'ALTER SYSTEM KILL SESSION ''' || rec.sid || ',' || rec.serial# || ''' IMMEDIATE';
    END LOOP;

    -- B??c 1: X�a th�ng tin record trong b?ng BENHNHAN
    DELETE FROM ADMIN_PHANHE1.BENHNHAN WHERE MABN = p_MABN;

    -- B??c 2: X�a t�i kho?n Oracle
    v_sql := 'DROP USER ' || v_username || ' CASCADE';
    EXECUTE IMMEDIATE v_sql;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE;
END;
/

-- X�a nh�n vi�n
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XOA_NHANVIEN (
    p_MANV IN VARCHAR2
)
AUTHID CURRENT_USER
IS
    v_sql VARCHAR2(500);
    v_username VARCHAR2(50) := 'C##' || UPPER(p_MANV);
BEGIN
    -- K�ch ho?t c? b? qua b?o m?t Container 12c+
    EXECUTE IMMEDIATE 'ALTER SESSION SET "_ORACLE_SCRIPT"=true';

    -- B??c 0: T? ??ng ng?t k?t n?i
    FOR rec IN (SELECT sid, serial# FROM v$session WHERE username = v_username)
    LOOP
        EXECUTE IMMEDIATE 'ALTER SYSTEM KILL SESSION ''' || rec.sid || ',' || rec.serial# || ''' IMMEDIATE';
    END LOOP;

    -- B??c 1: X�a d? li?u trong b?ng NHANVIEN
    DELETE FROM ADMIN_PHANHE1.NHANVIEN WHERE MANV = p_MANV;

    -- B??c 2: X�a t�i kho?n Oracle
    v_sql := 'DROP USER ' || v_username || ' CASCADE';
    EXECUTE IMMEDIATE v_sql;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE;
END;
/


-- C?p nh?t th�ng tin b?nh nh�n
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_SUA_BENHNHAN (
    p_MABN IN VARCHAR2, -- V?n ph?i truy?n v�o ?? l�m ?i?u ki?n WHERE t�m ?�ng ng??i
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
    p_MATKHAU IN VARCHAR2 -- Nh?n m?t kh?u m?i (c� th? r?ng)
)
AUTHID CURRENT_USER
IS
    v_sql VARCHAR2(500);
BEGIN
    -- B??c 1: C?p nh?t th�ng tin c� nh�n trong b?ng BENHNHAN
    -- (Ho�n to�n KH�NG ??ng ch?m ??n c?t MABN)
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

    -- B??c 2: ??i m?t kh?u t�i kho?n Oracle (N?u ng??i d�ng c� nh?p m?t kh?u)
    -- Ki?m tra n?u p_MATKHAU kh�ng b? r?ng (NULL) th� m?i ch?y l?nh ALTER USER
    IF p_MATKHAU IS NOT NULL AND TRIM(p_MATKHAU) <> '' THEN
        v_sql := 'ALTER USER C##' || p_MABN || ' IDENTIFIED BY "' || p_MATKHAU || '"';
        EXECUTE IMMEDIATE v_sql;
    END IF;

    -- L?u d? li?u
    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE;
END;
/

-- C?p nh?t th�ng tin Nh�n vi�n
-- L?nh x�a (N?u ch?a c� s? b�o l?i ORA-04043, b?n c? b? qua kh�ng sao nh�)
DROP PROCEDURE ADMIN_PHANHE1.SP_SUA_NHANVIEN;

-- L?nh t?o m?i
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_SUA_NHANVIEN (
    p_MANV IN VARCHAR2, -- M� NV d�ng ?? x�c ??nh ng??i c?n s?a
    p_HOTEN IN NVARCHAR2,
    p_PHAI IN NVARCHAR2,
    p_NGAYSINH IN DATE,
    p_CMND IN VARCHAR2,
    p_QUEQUAN IN NVARCHAR2,
    p_SODT IN VARCHAR2,
    p_VAITRO IN NVARCHAR2,
    p_CHUYENKHOA IN NVARCHAR2,
    p_COSO IN NVARCHAR2,
    p_MATKHAU IN VARCHAR2 -- Nh?n m?t kh?u m?i (c� th? r?ng)
)
AUTHID CURRENT_USER
IS
    v_sql VARCHAR2(500);
BEGIN
    -- B??c 1: C?p nh?t th�ng tin c� nh�n trong b?ng NHANVIEN
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

    -- B??c 2: ??i m?t kh?u t�i kho?n Oracle (N?u ng??i d�ng c� nh?p m?t kh?u tr�n Form)
    IF p_MATKHAU IS NOT NULL AND TRIM(p_MATKHAU) <> '' THEN
        v_sql := 'ALTER USER C##' || p_MANV || ' IDENTIFIED BY "' || p_MATKHAU || '"';
        EXECUTE IMMEDIATE v_sql;
    END IF;

    -- Ho�n t?t v� l?u d? li?u
    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        -- Ho�n t�c n?u c� b?t k? l?i g� x?y ra (v� d?: vi ph?m r�ng bu?c d? li?u)
        ROLLBACK;
        RAISE;
END;
/


-- Xem quy?n tr�n to�n b? nh�n vi�n
GRANT SELECT ANY DICTIONARY TO ADMIN_PHANHE1;

-- L?nh x�a (Ch? ch?y khi Procedure ?� t?n t?i, n?u kh�ng s? b�o l?i ORA-04043)
DROP PROCEDURE ADMIN_PHANHE1.SP_XEM_QUYEN_ALL_USER;

-- L?nh t?o m?i (ho?c ghi ?�)
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XEM_QUYEN_ALL_USER (
    p_CURSOR OUT SYS_REFCURSOR
)
AUTHID CURRENT_USER 
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT p.GRANTEE AS "T�n T�i Kho?n / Role",
           p.TABLE_NAME AS "T�n B?ng", 
           p.PRIVILEGE AS "Quy?n", 
           p.GRANTABLE AS "???c C?p Ti?p" 
    FROM DBA_TAB_PRIVS p
    JOIN DBA_OBJECTS o ON p.TABLE_NAME = o.OBJECT_NAME AND p.OWNER = o.OWNER
    WHERE p.GRANTEE LIKE 'C##%' 
      AND p.GRANTEE NOT IN ('C##ADMIN', 'ADMIN_PHANHE1', USER) 
      AND p.OWNER = 'ADMIN_PHANHE1'
      AND o.OBJECT_TYPE = 'TABLE'
    ORDER BY p.GRANTEE, p.TABLE_NAME;
END;
/
-- Xem quy?n tr�n c?t
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XEM_QUYEN_COT_ALL_USER (
    p_CURSOR OUT SYS_REFCURSOR
)
AUTHID CURRENT_USER -- V?n gi? c? ch? "??ng" theo t�i kho?n DBA ?ang ??ng nh?p
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT GRANTEE AS "T�n T�i Kho?n / Role",
           TABLE_NAME AS "T�n B?ng", 
           COLUMN_NAME AS "T�n C?t", -- ?i?m kh�c bi?t m?u ch?t ? ?�y
           PRIVILEGE AS "Quy?n", 
           GRANTABLE AS "???c C?p Ti?p" 
    FROM DBA_COL_PRIVS 
    WHERE GRANTEE LIKE 'C##%' 
      AND GRANTEE != 'C##ADMIN'
      AND OWNER = 'ADMIN_PHANHE1'
    ORDER BY GRANTEE, TABLE_NAME, COLUMN_NAME;
END;
/
-- Xem quy?n tr�n view
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XEM_QUYEN_VIEW_ALL_USER (
    p_CURSOR OUT SYS_REFCURSOR
)
AUTHID CURRENT_USER
AS
BEGIN
    OPEN p_CURSOR FOR
    -- K?t h?p b?ng ph�n quy?n (p) v� b?ng lo?i ??i t??ng (o)
    SELECT p.GRANTEE AS "T�n T�i Kho?n / Role",
           p.TABLE_NAME AS "T�n View",
           p.PRIVILEGE AS "Quy?n",
           p.GRANTABLE AS "???c C?p Ti?p"
    FROM DBA_TAB_PRIVS p
    JOIN DBA_OBJECTS o ON p.TABLE_NAME = o.OBJECT_NAME AND p.OWNER = o.OWNER
    WHERE p.GRANTEE LIKE 'C##%'
      AND p.GRANTEE NOT IN ('C##ADMIN', 'ADMIN_PHANHE1', USER)
      AND p.OWNER = 'ADMIN_PHANHE1'
      AND o.OBJECT_TYPE = 'VIEW' -- Ch? l?c l?y View
    ORDER BY p.GRANTEE, p.TABLE_NAME;
END;
/
-- Xem quy?n tr�n procedure/function
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_XEM_QUYEN_PROC_ALL_USER (
    p_CURSOR OUT SYS_REFCURSOR
)
AUTHID CURRENT_USER
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT p.GRANTEE AS "T�n T�i Kho?n / Role",
           p.TABLE_NAME AS "T�n H�m / Th? T?c",
           o.OBJECT_TYPE AS "Lo?i ??i T??ng", -- Hi?n th? r� n� l� Procedure hay Function
           p.PRIVILEGE AS "Quy?n",
           p.GRANTABLE AS "???c C?p Ti?p"
    FROM DBA_TAB_PRIVS p
    JOIN DBA_OBJECTS o ON p.TABLE_NAME = o.OBJECT_NAME AND p.OWNER = o.OWNER
    WHERE p.GRANTEE LIKE 'C##%'
      AND p.GRANTEE NOT IN ('C##ADMIN', 'ADMIN_PHANHE1', USER)
      AND p.OWNER = 'ADMIN_PHANHE1'
      AND o.OBJECT_TYPE IN ('PROCEDURE', 'FUNCTION') -- L?c l?y Procedure v� Function
    ORDER BY p.GRANTEE, p.TABLE_NAME;
END;
/

-----------------------------------------------
--NH?T L�N R?T TR�N TH�NH S? GI�P ?? C?A B?N---
-----------------------------------------------
-- =================================================================================
-- [NHI?M V? 3]: C?P QUY?N (GRANT PRIVILEGES) QUA PROCEDURE
-- =================================================================================

-- =================================================================================
-- 3.1 C?P QUY?N TR�N B?NG (TABLE) / VIEW
-- =================================================================================

-- C?p quy?n tr�n to�n b? b?ng ho?c view
GRANT <PRIVILEGE> 
ON ADMIN_PHANHE1.<OBJECT_NAME> 
TO <GRANTEE>;

-- Trong ?�:
-- <PRIVILEGE>: SELECT | INSERT | UPDATE | DELETE
-- <OBJECT_NAME>: t�n b?ng ho?c view
-- <GRANTEE>: USER ho?c ROLE

-- C?p quy?n k�m WITH GRANT OPTION (ch? �p d?ng cho USER)
GRANT <PRIVILEGE> 
ON ADMIN_PHANHE1.<OBJECT_NAME> 
TO <USERNAME> 
WITH GRANT OPTION;


-- =================================================================================
-- 3.2 C?P QUY?N TR�N C?T (COLUMN-LEVEL)
-- =================================================================================

-- Ch? �p d?ng cho SELECT v� UPDATE

-- C?p quy?n SELECT tr�n c�c c?t c? th?
GRANT SELECT (<COLUMN_1>, <COLUMN_2>) 
ON ADMIN_PHANHE1.<TABLE_NAME> 
TO <GRANTEE>;

-- C?p quy?n UPDATE tr�n c�c c?t c? th?
GRANT UPDATE (<COLUMN_1>, <COLUMN_2>) 
ON ADMIN_PHANHE1.<TABLE_NAME> 
TO <GRANTEE>;


-- =================================================================================
-- 3.3 C?P QUY?N TR�N PROCEDURE / FUNCTION
-- =================================================================================

-- Ch? s? d?ng quy?n EXECUTE
GRANT EXECUTE 
ON ADMIN_PHANHE1.<PROGRAM_NAME> 
TO <GRANTEE>;

-- <PROGRAM_NAME>: t�n PROCEDURE ho?c FUNCTION


-- =================================================================================
-- 3.4 C?P ROLE CHO USER
-- =================================================================================

-- G�n ROLE cho USER
GRANT <ROLE_NAME> 
TO <USERNAME>;

-- G�n ROLE k�m quy?n qu?n tr? (ADMIN OPTION)
GRANT <ROLE_NAME> 
TO <USERNAME> 
WITH ADMIN OPTION;

-- =================================================================================
-- C�C C�U TRUY V?N H? TR? GIAO DI?N (WINFORM)
-- =================================================================================
-- 1. L?y danh s�ch c?t c?a 1 b?ng
SELECT COLUMN_NAME
FROM ALL_TAB_COLUMNS
WHERE OWNER = 'ADMIN_PHANHE1'
AND TABLE_NAME = UPPER(:p_table_name)
ORDER BY COLUMN_ID;


-- 2. L?y danh s�ch TABLE
SELECT TABLE_NAME
FROM ALL_TABLES
WHERE OWNER = 'ADMIN_PHANHE1'
ORDER BY TABLE_NAME;


-- 3. L?y danh s�ch VIEW
SELECT VIEW_NAME
FROM ALL_VIEWS
WHERE OWNER = 'ADMIN_PHANHE1'
ORDER BY VIEW_NAME;


-- 4. L?y danh s�ch PROCEDURE v� FUNCTION
SELECT OBJECT_NAME
FROM ALL_OBJECTS
WHERE OWNER = 'ADMIN_PHANHE1'
AND OBJECT_TYPE IN ('PROCEDURE', 'FUNCTION')
ORDER BY OBJECT_NAME;

-- =================================================================================
-- [NHI?M V? 3]:  PROCEDURE
-- =================================================================================

-- >>> 3.1 PROCEDURE C?P QUY?N TR�N ??I T??NG (TABLE, VIEW, PROC, FUNC)
-- H? tr?: Ph�n quy?n m?c c?t cho SELECT/UPDATE, WITH GRANT OPTION cho User.
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_GRANT_PRIVILEGE (
    p_GRANTEE       IN VARCHAR2,
    p_PRIVILEGE     IN VARCHAR2,
    p_OBJECT_NAME   IN VARCHAR2,
    p_COLUMNS       IN VARCHAR2,
    p_GRANT_OPTION  IN NUMBER   -- 1: c�, 0: kh�ng
)
AUTHID CURRENT_USER
AS
    v_sql               VARCHAR2(1000);
    v_grant_option_str  VARCHAR2(50) := '';
    v_column_str        VARCHAR2(500) := '';
    v_is_role           NUMBER;

    v_grantee   VARCHAR2(100) := UPPER(TRIM(p_GRANTEE));
    v_privilege VARCHAR2(50)  := UPPER(TRIM(p_PRIVILEGE));
    v_obj       VARCHAR2(100) := UPPER(TRIM(p_OBJECT_NAME));
BEGIN
    -- Validate privilege
    IF v_privilege NOT IN ('SELECT','INSERT','UPDATE','DELETE','EXECUTE') THEN
        RAISE_APPLICATION_ERROR(-20001, 'INVALID PRIVILEGE');
    END IF;

    -- Check ROLE hay USER
    SELECT COUNT(*) INTO v_is_role 
    FROM DBA_ROLES 
    WHERE ROLE = v_grantee;

    -- WITH GRANT OPTION ch? �p d?ng cho USER
    IF p_GRANT_OPTION = 1 AND v_is_role = 0 THEN
        v_grant_option_str := ' WITH GRANT OPTION';
    END IF;

    -- Column-level ch? cho SELECT / UPDATE
    IF (v_privilege = 'SELECT' OR v_privilege = 'UPDATE') THEN
        IF p_COLUMNS IS NOT NULL AND TRIM(p_COLUMNS) <> '' THEN
            v_column_str := '(' || UPPER(TRIM(p_COLUMNS)) || ')';
        END IF;
    END IF;

    -- Ch?ng injection c? b?n
    v_grantee := DBMS_ASSERT.SIMPLE_SQL_NAME(v_grantee);
    v_obj     := DBMS_ASSERT.SIMPLE_SQL_NAME(v_obj);

    -- Build SQL
    v_sql := 'GRANT ' || v_privilege || v_column_str ||
             ' ON ADMIN_PHANHE1.' || v_obj ||
             ' TO ' || v_grantee || v_grant_option_str;

    EXECUTE IMMEDIATE v_sql;
END;
/
-- Procedure cấp quyền cấp cột (Column-level privilege)
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_GRANT_QUYEN_COT (
    p_TEN_BANG IN VARCHAR2,       -- Tên bảng (VD: NHANVIEN)
    p_TEN_COT IN VARCHAR2,        -- Tên cột (VD: SODT)
    p_TEN_TAIKHOAN IN VARCHAR2,   -- Người nhận quyền (VD: C##YTA_LAN)
    p_QUYEN IN VARCHAR2           -- Tên quyền (VD: UPDATE)
)
AUTHID CURRENT_USER
AS
    v_sql VARCHAR2(500);
BEGIN
    -- Cú pháp chuẩn của Oracle: GRANT UPDATE (SODT) ON ADMIN_PHANHE1.NHANVIEN TO C##YTA_LAN
    v_sql := 'GRANT ' || UPPER(p_QUYEN) || ' (' || UPPER(p_TEN_COT) || ') ON ADMIN_PHANHE1.' || UPPER(p_TEN_BANG) || ' TO ' || UPPER(p_TEN_TAIKHOAN);
    
    -- Thực thi câu lệnh SQL động
    EXECUTE IMMEDIATE v_sql;
    COMMIT;
    
EXCEPTION
    WHEN OTHERS THEN
        -- Bắt lỗi và ném về cho C# xử lý (VD: cấp sai quyền cho cột)
        RAISE;
END;
/
-- Procedure cấp quyền trên view
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_GRANT_VIEW_PRIV (
    p_VIEW_NAME IN VARCHAR2,
    p_GRANTEE IN VARCHAR2,
    p_PRIVILEGE IN VARCHAR2 -- SELECT, INSERT, UPDATE ho?c DELETE
)
AUTHID CURRENT_USER
AS
    v_sql VARCHAR2(500);
BEGIN
    -- C� ph�p: GRANT SELECT ON ADMIN_PHANHE1.V_TEN_VIEW TO C##USER
    v_sql := 'GRANT ' || p_PRIVILEGE || ' ON ADMIN_PHANHE1.' || p_VIEW_NAME || ' TO ' || p_GRANTEE;
    EXECUTE IMMEDIATE v_sql;
END;
/
-- procedure c?p quy?n tr�n procedure/function  
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_GRANT_PROC_FUNC_PRIV (
    p_OBJECT_NAME IN VARCHAR2,
    p_GRANTEE IN VARCHAR2
)
AUTHID CURRENT_USER
AS
    v_sql VARCHAR2(500);
BEGIN
    -- C� ph�p: GRANT EXECUTE ON ADMIN_PHANHE1.TEN_PROC TO C##USER
    v_sql := 'GRANT EXECUTE ON ADMIN_PHANHE1.' || p_OBJECT_NAME || ' TO ' || p_GRANTEE;
    EXECUTE IMMEDIATE v_sql;
END;
/
-- >>> 3.2 PROCEDURE C?P ROLE CHO USER
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_GRANT_ROLE_TO_USER (
    p_ROLE_NAME     IN VARCHAR2,
    p_USER_NAME     IN VARCHAR2,
    p_ADMIN_OPTION  IN NUMBER   -- 1: c�, 0: kh�ng
)
AUTHID CURRENT_USER
AS
    v_sql VARCHAR2(500);
BEGIN
    v_sql := 'GRANT ' || UPPER(p_ROLE_NAME) ||
             ' TO ' || UPPER(p_USER_NAME);

    IF p_ADMIN_OPTION = 1 THEN
        v_sql := v_sql || ' WITH ADMIN OPTION';
    END IF;

    EXECUTE IMMEDIATE v_sql;
END;
/

-- >>> 3.3 C�C TRUY V?N H? TR? LOAD D? LI?U L�N WINFORM
-- L?y danh s�ch b?ng
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_GET_LIST_TABLES (
    p_CURSOR OUT SYS_REFCURSOR
)
AUTHID CURRENT_USER
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT TABLE_NAME AS "T�n B?ng"
    FROM ALL_TABLES
    WHERE OWNER = 'ADMIN_PHANHE1'
    ORDER BY TABLE_NAME;
END;
/
-- L?y danh s�ch c?t c?a 1 b?ng
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

-- l?y danh s�ch view
-- L?nh x�a n?u ?� t?n t?i

CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_GET_LIST_VIEWS (
    p_CURSOR OUT SYS_REFCURSOR
)
AUTHID CURRENT_USER
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT OBJECT_NAME AS "T�n View"
    FROM ALL_OBJECTS
    WHERE OWNER = 'ADMIN_PHANHE1'
      AND OBJECT_TYPE = 'VIEW'
    ORDER BY OBJECT_NAME;
END;
/
-- l?y danh s�ch procedure / function
-- L?nh x�a n?u ?� t?n t?i

-- L?nh t?o m?i
CREATE OR REPLACE PROCEDURE ADMIN_PHANHE1.SP_GET_LIST_PROCS_FUNCS (
    p_CURSOR OUT SYS_REFCURSOR
)
AUTHID CURRENT_USER
AS
BEGIN
    OPEN p_CURSOR FOR
    SELECT OBJECT_NAME AS "T�n ??i T??ng",
           OBJECT_TYPE AS "Lo?i ??i T??ng" -- Tr? v? ch? 'PROCEDURE' ho?c 'FUNCTION' ?? ph�n bi?t
    FROM ALL_OBJECTS
    WHERE OWNER = 'ADMIN_PHANHE1'
      AND OBJECT_TYPE IN ('PROCEDURE', 'FUNCTION')
    ORDER BY OBJECT_TYPE, OBJECT_NAME;
END;
/

--K?T THUC TEST 3--


--PROCEDURE 4 THU HOI QUYEN TU ROLE, USER------------
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
    -- Ch?ng SQL Injection
    v_grantee := DBMS_ASSERT.SIMPLE_SQL_NAME(v_grantee);

    -- S?A ? ?�Y: Ch? c?n IS NOT NULL
    IF v_obj IS NOT NULL THEN
        v_obj := DBMS_ASSERT.SIMPLE_SQL_NAME(v_obj);
        v_sql := 'REVOKE ' || v_priv || 
                 ' ON ADMIN_PHANHE1.' || v_obj || 
                 ' FROM ' || v_grantee;
    ELSE
        -- Nh�nh n�y ch? ch?y khi p_OBJECT_NAME th?c s? l� r?ng (thu h?i ROLE)
        v_sql := 'REVOKE ' || v_priv || ' FROM ' || v_grantee;
    END IF;

    DBMS_OUTPUT.PUT_LINE('Executing Revoke: ' || v_sql);
    EXECUTE IMMEDIATE v_sql;
    COMMIT;
END;
/
--TEST
CREATE USER C##NV0001 IDENTIFIED BY 123456;

-- C?p quy?n c? b?n ?? t�i kho?n c� th? t?n t?i v� k?t n?i v�o DB
GRANT CREATE SESSION TO C##NV0001;