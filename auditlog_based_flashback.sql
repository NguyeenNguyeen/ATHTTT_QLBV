-- ==============================================================================
-- SCRIPT CÀI ĐẶT TÍNH NĂNG KHÔI PHỤC DỮ LIỆU TỪ AUDIT LOG (FLASHBACK)
-- ==============================================================================

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

