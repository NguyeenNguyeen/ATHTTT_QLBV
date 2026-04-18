-- ================================================================
-- TEST CÁC PROCEDURE PHÂN QUYỀN
-- ================================================================
-- Chạy script này trên tài khoản ADMIN_PHANHE1 hoặc SYSTEM (DBA)

-- 1. Kiểm tra các User/Role hiện tại
SELECT USERNAME FROM DBA_USERS WHERE USERNAME LIKE 'C##%' ORDER BY USERNAME;

-- 2. Kiểm tra các bảng trong schema ADMIN_PHANHE1
SELECT TABLE_NAME FROM ALL_TABLES WHERE OWNER='ADMIN_PHANHE1' ORDER BY TABLE_NAME;

-- 3. Kiểm tra các Procedure
SELECT OBJECT_NAME FROM ALL_OBJECTS WHERE OWNER='ADMIN_PHANHE1' AND OBJECT_TYPE='PROCEDURE' ORDER BY OBJECT_NAME;

-- ================================================================
-- TEST GRANT - CẤP QUYỀN
-- ================================================================

-- Test 1: Cấp SELECT trên BENHNHAN cho C##NV0001
BEGIN
  ADMIN_PHANHE1.SP_GRANT_PRIVILEGE(
    p_GRANTEE => 'C##NV0001',
    p_PRIVILEGE => 'SELECT',
    p_OBJECT_NAME => 'BENHNHAN',
    p_COLUMNS => '',
    p_GRANT_OPTION => 0
  );
  COMMIT;
  DBMS_OUTPUT.PUT_LINE('✓ Cấp SELECT trên BENHNHAN cho C##NV0001 - Thành công');
EXCEPTION WHEN OTHERS THEN
  DBMS_OUTPUT.PUT_LINE('✗ Lỗi: ' || SQLERRM);
END;
/

-- Test 2: Cấp SELECT trên 2 cột cụ thể (MANV, HOTEN) cho C##BN0001
BEGIN
  ADMIN_PHANHE1.SP_GRANT_PRIVILEGE(
    p_GRANTEE => 'C##BN0001',
    p_PRIVILEGE => 'SELECT',
    p_OBJECT_NAME => 'NHANVIEN',
    p_COLUMNS => 'MANV,HOTEN',
    p_GRANT_OPTION => 1
  );
  COMMIT;
  DBMS_OUTPUT.PUT_LINE('✓ Cấp SELECT (2 cột) trên NHANVIEN cho C##BN0001 - Thành công');
EXCEPTION WHEN OTHERS THEN
  DBMS_OUTPUT.PUT_LINE('✗ Lỗi: ' || SQLERRM);
END;
/

-- Test 3: Cấp UPDATE trên BENHNHAN cho C##NV0001
BEGIN
  ADMIN_PHANHE1.SP_GRANT_PRIVILEGE(
    p_GRANTEE => 'C##NV0001',
    p_PRIVILEGE => 'UPDATE',
    p_OBJECT_NAME => 'BENHNHAN',
    p_COLUMNS => '',
    p_GRANT_OPTION => 0
  );
  COMMIT;
  DBMS_OUTPUT.PUT_LINE('✓ Cấp UPDATE trên BENHNHAN cho C##NV0001 - Thành công');
EXCEPTION WHEN OTHERS THEN
  DBMS_OUTPUT.PUT_LINE('✗ Lỗi: ' || SQLERRM);
END;
/

-- Test 4: Cấp EXECUTE trên SP_TAO_BENHNHAN cho C##NV0001
BEGIN
  ADMIN_PHANHE1.SP_GRANT_PRIVILEGE(
    p_GRANTEE => 'C##NV0001',
    p_PRIVILEGE => 'EXECUTE',
    p_OBJECT_NAME => 'SP_TAO_BENHNHAN',
    p_COLUMNS => '',
    p_GRANT_OPTION => 0
  );
  COMMIT;
  DBMS_OUTPUT.PUT_LINE('✓ Cấp EXECUTE SP_TAO_BENHNHAN cho C##NV0001 - Thành công');
EXCEPTION WHEN OTHERS THEN
  DBMS_OUTPUT.PUT_LINE('✗ Lỗi: ' || SQLERRM);
END;
/

-- ================================================================
-- KIỂM TRA QUYỀN ĐƯỢC CẤP
-- ================================================================

-- Xem quyền trên bảng
BEGIN
  ADMIN_PHANHE1.SP_XEM_QUYEN_ALL_USER(:p_CURSOR);
END;
/

-- Xem quyền trên cột
BEGIN
  ADMIN_PHANHE1.SP_XEM_QUYEN_COT_ALL_USER(:p_CURSOR);
END;
/

-- Xem quyền trên procedure/function
BEGIN
  ADMIN_PHANHE1.SP_XEM_QUYEN_PROC_ALL_USER(:p_CURSOR);
END;
/

-- ================================================================
-- TEST REVOKE - THU HỒI QUYỀN
-- ================================================================

-- Test 1: Thu hồi SELECT trên BENHNHAN từ C##NV0001
BEGIN
  ADMIN_PHANHE1.SP_REVOKE_PRIVILEGE(
    p_GRANTEE => 'C##NV0001',
    p_PRIVILEGE => 'SELECT',
    p_OBJECT_NAME => 'BENHNHAN'
  );
  COMMIT;
  DBMS_OUTPUT.PUT_LINE('✓ Thu hồi SELECT trên BENHNHAN từ C##NV0001 - Thành công');
EXCEPTION WHEN OTHERS THEN
  DBMS_OUTPUT.PUT_LINE('✗ Lỗi: ' || SQLERRM);
END;
/

-- Test 2: Thu hồi UPDATE trên BENHNHAN từ C##NV0001
BEGIN
  ADMIN_PHANHE1.SP_REVOKE_PRIVILEGE(
    p_GRANTEE => 'C##NV0001',
    p_PRIVILEGE => 'UPDATE',
    p_OBJECT_NAME => 'BENHNHAN'
  );
  COMMIT;
  DBMS_OUTPUT.PUT_LINE('✓ Thu hồi UPDATE trên BENHNHAN từ C##NV0001 - Thành công');
EXCEPTION WHEN OTHERS THEN
  DBMS_OUTPUT.PUT_LINE('✗ Lỗi: ' || SQLERRM);
END;
/

-- ================================================================
-- TEST GRANT ROLE
-- ================================================================

-- Test 1: Cấp ROLE cho User (nếu role tồn tại)
BEGIN
  ADMIN_PHANHE1.SP_GRANT_ROLE_TO_USER(
    p_ROLE_NAME => 'CONNECT',
    p_USER_NAME => 'C##NV0001',
    p_ADMIN_OPTION => 0
  );
  COMMIT;
  DBMS_OUTPUT.PUT_LINE('✓ Cấp CONNECT role cho C##NV0001 - Thành công');
EXCEPTION WHEN OTHERS THEN
  DBMS_OUTPUT.PUT_LINE('✗ Lỗi: ' || SQLERRM);
END;
/

-- ================================================================
-- KIỂM TRA CUỐI CÙNG
-- ================================================================

-- Xem tất cả quyền của C##NV0001 trên bảng
SELECT * FROM DBA_TAB_PRIVS 
WHERE OWNER='ADMIN_PHANHE1' AND GRANTEE='C##NV0001' 
ORDER BY TABLE_NAME;

-- Xem tất cả quyền của C##NV0001 trên cột
SELECT * FROM DBA_COL_PRIVS 
WHERE OWNER='ADMIN_PHANHE1' AND GRANTEE='C##NV0001' 
ORDER BY TABLE_NAME, COLUMN_NAME;

-- Xem tất cả quyền của C##NV0001 trên procedure/function
SELECT * FROM DBA_TAB_PRIVS p
JOIN DBA_OBJECTS o ON p.TABLE_NAME=o.OBJECT_NAME AND p.OWNER=o.OWNER
WHERE p.OWNER='ADMIN_PHANHE1' AND p.GRANTEE='C##NV0001' AND o.OBJECT_TYPE IN ('PROCEDURE', 'FUNCTION')
ORDER BY p.TABLE_NAME;

-- ================================================================
-- DỌNG DỮ LIỆU CLEANUP (CÓ THỂ THỰC HIỆN NẾU CẦN)
-- ================================================================
-- Thu hồi tất cả quyền của C##NV0001 (nếu cần)
-- REVOKE ALL PRIVILEGES FROM C##NV0001;
