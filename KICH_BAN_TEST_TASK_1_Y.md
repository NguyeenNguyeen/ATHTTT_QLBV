# KICH BAN TEST TASK 1 - Y - RBAC KTV / BENH NHAN

Tai lieu nay danh cho **Y - Task 1** de chuan bi demo va van dap phan **RBAC cho Ky thuat vien (KTV) va Benh nhan (BN)**.

## 0. Can demo bao nhieu la du?

Khi van dap, nen demo ngan gon theo 5 minh chung:

1. Kiem tra object RBAC ton tai va `VALID`.
2. Login KTV, chi thay thong tin ca nhan va dich vu duoc giao.
3. KTV cap nhat thanh cong cot duoc phep.
4. Login BN, chi thay ho so cua chinh minh.
5. BN cap nhat thanh cong cot duoc phep, cac cot dinh danh bi khoa/khong duoc cap quyen.

Neu thay hoi sau, chay them SQL de chung minh lenh trai phep bi Oracle chan, khong chi bi UI chan.

## 1. Tom tat Task 1 de noi khi van dap

Task 1 cai dat RBAC cho 2 nhom nguoi dung:

- `ROLE_KYTHUATVIEN`: danh cho nhan vien co vai tro **Ky thuat vien**.
- `ROLE_BENHNHAN`: danh cho nguoi dung la **Benh nhan**.

Thay vi cap quyen truc tiep tren bang goc, he thong tao cac view bao mat:

- `V_RBAC_KTV_THONGTIN`: KTV chi xem thong tin ca nhan cua chinh minh.
- `V_RBAC_KTV_DICHVU`: KTV chi xem dich vu `HSBA_DV` duoc giao cho minh qua cot `MAKTV`.
- `V_RBAC_BENHNHAN_THONGTIN`: BN chi xem thong tin benh nhan cua chinh minh.

Cac view dung dieu kien:

```sql
USER = 'C##' || MANV
USER = 'C##' || MABN
```

va dung `WITH CHECK OPTION` de chan viec update lam dong du lieu khong con thuoc ve user hien tai.

## 2. Chuan bi truoc khi demo

### 2.1. Build app

Tai thu muc goc repo:

```powershell
dotnet build ADMIN\ADMIN.csproj
```

Ket qua mong doi:

```text
Build succeeded.
0 Error(s)
```

### 2.2. Chay script database tong

Neu DB chua co du lieu/task:

```sql
@oracle/CQ2026-CQ16-PH1-Database.sql
```

Chay bang SQL Developer voi connection `SYS AS SYSDBA` hoac `SYSTEM` co quyen DBA.

### 2.3. Kiem tra object RBAC cua Task 1

Dang nhap SQL Developer bang:

```text
Username: ADMIN_PHANHE1
Password: Admin@123456
Service name: orcl21 hoac orclpdb1 tuy may
```

Chay:

```sql
SELECT object_name, object_type, status
FROM user_objects
WHERE object_name IN (
    'V_RBAC_KTV_THONGTIN',
    'V_RBAC_KTV_DICHVU',
    'V_RBAC_BENHNHAN_THONGTIN',
    'SP_SYNC_TASK1_RBAC_USERS'
)
ORDER BY object_type, object_name;
```

Ket qua mong doi:

- 3 view RBAC va procedure sync user ton tai.
- Tat ca `STATUS = VALID`.

Kiem tra role:

```sql
SELECT role
FROM dba_roles
WHERE role IN ('ROLE_KYTHUATVIEN', 'ROLE_BENHNHAN')
ORDER BY role;
```

Ket qua mong doi:

```text
ROLE_BENHNHAN
ROLE_KYTHUATVIEN
```

Kiem tra quyen cap cho role:

```sql
SELECT owner, table_name, privilege, grantee
FROM dba_tab_privs
WHERE owner = 'ADMIN_PHANHE1'
  AND grantee IN ('ROLE_KYTHUATVIEN', 'ROLE_BENHNHAN')
  AND table_name LIKE 'V_RBAC_%'
ORDER BY grantee, table_name, privilege;
```

Kiem tra quyen update theo cot:

```sql
SELECT owner, table_name, column_name, privilege, grantee
FROM dba_col_privs
WHERE owner = 'ADMIN_PHANHE1'
  AND grantee IN ('ROLE_KYTHUATVIEN', 'ROLE_BENHNHAN')
  AND table_name LIKE 'V_RBAC_%'
ORDER BY grantee, table_name, column_name;
```

Ket qua mong doi:

- `ROLE_KYTHUATVIEN` co `SELECT` tren `V_RBAC_KTV_THONGTIN`, `V_RBAC_KTV_DICHVU`.
- `ROLE_KYTHUATVIEN` chi co `UPDATE` cac cot `QUEQUAN`, `SODT`, `COSO`, `KETQUA`.
- `ROLE_BENHNHAN` co `SELECT` tren `V_RBAC_BENHNHAN_THONGTIN`.
- `ROLE_BENHNHAN` chi co `UPDATE` cac cot dia chi/tien su benh/di ung thuoc, khong co quyen update `TENBN`, `CCCD`, `NGAYSINH`.

## 3. Tai khoan demo de dung

### KTV mau

```text
Username: C##NV007
Password: 123456
Vai tro DB: ROLE_KYTHUATVIEN
Nhan vien: NV007 - Dang Trong Tran
```

Dich vu mau duoc giao cho `NV007`:

- `HS000001` - Do dien tam do (ECG)
- `HS000002` - Chup MRI So nao
- `HS000003` - Noi soi da day

### Benh nhan mau

```text
Username: C##BN000001
Password: 123456
Vai tro DB: ROLE_BENHNHAN
Benh nhan: BN000001 - Ly Van Truong
```

Neu dang nhap bi sai mat khau/user lock, dang nhap admin va reset:

```sql
ALTER USER C##NV007 IDENTIFIED BY "123456" ACCOUNT UNLOCK;
ALTER USER C##BN000001 IDENTIFIED BY "123456" ACCOUNT UNLOCK;
```

## 4. Test Case 01 - Login KTV va kiem tra dung form

### Muc tieu

Chung minh user KTV duoc nhan dien dung role va vao dung giao dien RBAC.

### Buoc test

1. Chay app:

```powershell
dotnet run --project ADMIN\ADMIN.csproj
```

2. Dang nhap:

```text
Username: C##NV007
Password: 123456
```

### Ket qua mong doi

- Dang nhap thanh cong.
- App mo form **He thong Ky thuat vien | RBAC**.
- Tieu de/chao mung hien user `C##NV007`.

### Hinh nen chup

Chup man hinh form KTV sau khi login.

## 5. Test Case 02 - KTV chi xem thong tin ca nhan cua minh

### Muc tieu

Chung minh `V_RBAC_KTV_THONGTIN` chi tra ve 1 dong cua chinh KTV dang login.

### Buoc test tren UI

1. Sau khi login `C##NV007`.
2. Xem khu vuc thong tin ca nhan.

### Ket qua mong doi

- Chi thay thong tin cua `NV007`.
- Khong co danh sach tat ca nhan vien.

### SQL kiem chung

Dang nhap SQL Developer bang `C##NV007/123456`, chay:

```sql
SELECT USER AS logged_user FROM dual;

SELECT MANV, HOTEN, VAITRO, SODT, COSO
FROM ADMIN_PHANHE1.V_RBAC_KTV_THONGTIN;
```

Ket qua mong doi:

- `LOGGED_USER = C##NV007`.
- Query chi ra 1 dong `MANV = NV007`.

## 6. Test Case 03 - KTV chi xem dich vu duoc phan cong

### Muc tieu

Chung minh KTV chi xem duoc cac dong `HSBA_DV` co `MAKTV = NV007`.

### Buoc test tren UI

1. Login `C##NV007`.
2. Xem bang danh sach dich vu/xet nghiem.

### Ket qua mong doi

- Tat ca dong hien thi deu co `MAKTV = NV007`.
- Khong thay dong cua KTV khac hoac dong chua phan cong.

### SQL kiem chung

Dang nhap `C##NV007`, chay:

```sql
SELECT MAHSBA, LOAIDV, MAKTV, KETQUA
FROM ADMIN_PHANHE1.V_RBAC_KTV_DICHVU
ORDER BY MAHSBA, LOAIDV;
```

Ket qua mong doi: moi dong deu co `MAKTV = NV007`.

## 7. Test Case 04 - KTV cap nhat thanh cong cot duoc phep

### Muc tieu

Chung minh KTV co quyen update cot an toan trong view, dung RBAC.

### Buoc test tren UI

1. Login `C##NV007`.
2. Sua `SODT` hoac `COSO` trong thong tin ca nhan.
3. Bam cap nhat.
4. Sua `KETQUA` cua mot dich vu duoc giao.
5. Bam luu.

### Ket qua mong doi

- Cap nhat thong tin ca nhan thanh cong.
- Cap nhat `KETQUA` thanh cong.

### SQL kiem chung

Dang nhap `C##NV007`, chay:

```sql
UPDATE ADMIN_PHANHE1.V_RBAC_KTV_THONGTIN
SET SODT = '0911111111';

UPDATE ADMIN_PHANHE1.V_RBAC_KTV_DICHVU
SET KETQUA = N'TASK1_KTV_TEST_OK'
WHERE ROWNUM = 1;

ROLLBACK;
```

Ket qua mong doi:

- 2 cau `UPDATE` chay thanh cong.
- `ROLLBACK` de khong lam thay doi du lieu demo.

## 8. Test Case 05 - KTV bi chan khi sua cot khong duoc phep

### Muc tieu

Chung minh bao mat nam o DB, khong chi khoa UI.

### Buoc test SQL

Dang nhap `C##NV007`, chay:

```sql
UPDATE ADMIN_PHANHE1.V_RBAC_KTV_THONGTIN
SET HOTEN = N'TEN KHONG DUOC SUA';
```

Ket qua mong doi:

- Bi Oracle chan, thuong gap loi:

```text
ORA-01031: insufficient privileges
```

Thu sua cot phan cong KTV:

```sql
UPDATE ADMIN_PHANHE1.V_RBAC_KTV_DICHVU
SET MAKTV = 'NV001'
WHERE ROWNUM = 1;
```

Ket qua mong doi:

- Bi Oracle chan vi role khong co quyen update cot `MAKTV`, hoac bi `WITH CHECK OPTION` chan neu lam dong khong con thuoc ve KTV.

Thu doc bang goc:

```sql
SELECT COUNT(*)
FROM ADMIN_PHANHE1.HSBA_DV;
```

Ket qua mong doi:

- Bi chan neu user khong duoc cap SELECT truc tiep tren bang goc.

## 9. Test Case 06 - Login Benh nhan va kiem tra dung form

### Muc tieu

Chung minh user BN duoc nhan dien dung role va vao dung form benh nhan.

### Buoc test

1. Thoat app hoac logout/chay lai app.
2. Dang nhap:

```text
Username: C##BN000001
Password: 123456
```

### Ket qua mong doi

- Dang nhap thanh cong.
- App mo form **Cong thong tin Benh nhan | RBAC**.
- Chi hien ho so cua `BN000001`.

### Hinh nen chup

Chup man hinh form benh nhan sau khi login.

## 10. Test Case 07 - BN chi xem thong tin cua chinh minh

### Muc tieu

Chung minh benh nhan khong xem duoc ho so cua benh nhan khac.

### Buoc test UI

1. Login `C##BN000001`.
2. Xem thong tin benh nhan.

### Ket qua mong doi

- Chi hien `BN000001`.
- Khong thay `BN000002`, `BN000003`.

### SQL kiem chung

Dang nhap SQL Developer bang `C##BN000001/123456`, chay:

```sql
SELECT USER AS logged_user FROM dual;

SELECT MABN, TENBN, SONHA, TENDUONG, QUANHUYEN, TINHTP, TIENSUBENH, DIUNGTHUOC
FROM ADMIN_PHANHE1.V_RBAC_BENHNHAN_THONGTIN;
```

Ket qua mong doi:

- Chi co 1 dong `MABN = BN000001`.

## 11. Test Case 08 - BN cap nhat thanh cong cot duoc phep

### Muc tieu

Chung minh benh nhan duoc sua thong tin khong dinh danh cua chinh minh.

### Buoc test UI

1. Login `C##BN000001`.
2. Sua mot trong cac truong:

```text
SONHA
TENDUONG
QUANHUYEN
TINHTP
TIENSUBENH
TIENSUBENHGD
DIUNGTHUOC
```

3. Bam cap nhat.

### Ket qua mong doi

- Cap nhat thanh cong.
- Cac truong dinh danh nhu ho ten, ngay sinh, CCCD khong cho sua tren UI.

### SQL kiem chung

Dang nhap `C##BN000001`, chay:

```sql
UPDATE ADMIN_PHANHE1.V_RBAC_BENHNHAN_THONGTIN
SET SONHA = N'999',
    TENDUONG = N'Duong test RBAC',
    TIENSUBENH = N'Test RBAC';

ROLLBACK;
```

Ket qua mong doi:

- `UPDATE` thanh cong.
- `ROLLBACK` de khong lam thay doi du lieu demo.

## 12. Test Case 09 - BN bi chan khi sua cot dinh danh hoac doc bang goc

### Muc tieu

Chung minh DB chi cap quyen update theo cot, khong cho sua thong tin nhay cam.

### Buoc test SQL

Dang nhap `C##BN000001`, chay:

```sql
UPDATE ADMIN_PHANHE1.V_RBAC_BENHNHAN_THONGTIN
SET TENBN = N'TEN BI CHAN';
```

Ket qua mong doi:

```text
ORA-01031: insufficient privileges
```

Thu doc bang goc:

```sql
SELECT COUNT(*)
FROM ADMIN_PHANHE1.BENHNHAN;
```

Ket qua mong doi:

- Bi Oracle chan neu user khong co SELECT truc tiep tren bang goc.

## 13. Test Case 10 - Chay script SQL test tu dong

### Muc tieu

Chung minh toan bo Task 1 bang SQL, co ca test thanh cong va test bi chan.

### Cach chay

Tai root repo, dung SQL*Plus:

```powershell
sqlplus /nolog @oracle/test_task1_rbac.sql
```

Neu service may ban khong phai `orcl21`, mo file `oracle/test_task1_rbac.sql` va sua cac chuoi connect:

```text
@//127.0.0.1:1521/orcl21
```

thanh service dung tren may, vi du:

```text
@//127.0.0.1:1521/orclpdb1
```

### Ket qua mong doi

Script se:

- Kiem tra view/role/quyen RBAC.
- Tao tam KTV `NV7777` va user `C##NV7777`.
- Test KTV chi thay du lieu cua minh.
- Test KTV update cot duoc phep.
- Test cac lenh trai phep bi chan.
- Test BN chi thay ho so cua minh.
- Cleanup du lieu test.

Cuoi script co:

```text
DONE TASK 1 RBAC TEST
```

## 14. Cac hinh nen chup khi demo

Nen chup toi thieu 6 hinh:

1. SQL object RBAC `VALID`.
2. Login `C##NV007` vao form KTV.
3. KTV chi thay dich vu `MAKTV = NV007`.
4. KTV cap nhat `KETQUA` thanh cong.
5. Login `C##BN000001` vao form BN.
6. BN cap nhat thong tin duoc phep thanh cong.

Neu co them thoi gian:

7. SQL `ORA-01031` khi KTV/BN update cot khong duoc phep.
8. Script `test_task1_rbac.sql` chay xong.

## 15. Checklist nhanh truoc khi vao van dap

- [ ] App build thanh cong.
- [ ] `V_RBAC_KTV_THONGTIN` valid.
- [ ] `V_RBAC_KTV_DICHVU` valid.
- [ ] `V_RBAC_BENHNHAN_THONGTIN` valid.
- [ ] `ROLE_KYTHUATVIEN` ton tai.
- [ ] `ROLE_BENHNHAN` ton tai.
- [ ] Login `C##NV007/123456` duoc.
- [ ] Login `C##BN000001/123456` duoc.
- [ ] KTV chi thay thong tin/dich vu cua minh.
- [ ] BN chi thay thong tin cua minh.
- [ ] KTV/BN update thanh cong cot duoc phep.
- [ ] KTV/BN bi chan khi update cot khong duoc phep.

## 16. Cau hoi van dap hay gap va cach tra loi ngan

### 1. Vi sao Task 1 dung RBAC?

Vi yeu cau phan quyen theo vai tro: Ky thuat vien va Benh nhan. RBAC phu hop vi minh tao role, cap quyen cho role, sau do gan role cho user tuong ung.

### 2. Neu chi dung role thi lam sao gioi han moi user chi thay du lieu cua minh?

Role chi giai quyet quyen thao tac. De gioi han dong du lieu theo tung user, em cap quyen tren view co dieu kien `USER = 'C##' || MANV/MABN`, khong cap truc tiep tren bang goc.

### 3. `WITH CHECK OPTION` dung de lam gi?

No chan user update lam dong du lieu sau update khong con thoa dieu kien cua view. Vi du KTV khong the doi `MAKTV` sang nguoi khac de day dong ra khoi pham vi cua minh.

### 4. KTV duoc sua nhung cot nao?

KTV duoc sua `QUEQUAN`, `SODT`, `COSO` tren thong tin ca nhan va `KETQUA` tren dich vu duoc giao.

### 5. Benh nhan duoc sua nhung cot nao?

Benh nhan duoc sua dia chi va tien su: `SONHA`, `TENDUONG`, `QUANHUYEN`, `TINHTP`, `TIENSUBENH`, `TIENSUBENHGD`, `DIUNGTHUOC`.

### 6. Benh nhan khong duoc sua gi?

Khong duoc sua cac cot dinh danh/nhay cam: `MABN`, `TENBN`, `PHAI`, `NGAYSINH`, `CCCD`.

### 7. Bao mat nam o UI hay DB?

Nam o DB. UI chi giup thao tac de hon. Neu dung SQL Developer dang nhap bang user KTV/BN va update cot trai phep, Oracle van chan bang quyen tren view/cot.

### 8. Procedure `SP_SYNC_TASK1_RBAC_USERS` lam gi?

Procedure dong bo user Oracle theo du lieu `NHANVIEN` va `BENHNHAN`, tao user `C##MANV`/`C##MABN`, cap `CREATE SESSION`, gan role dung, va thu hoi role KTV neu nhan vien khong con la KTV.

### 9. Vi sao username co prefix `C##`?

Do Oracle multitenant/common user. Script dung user dang `C##NV007`, nen view so sanh `USER` voi `'C##' || MANV`.

### 10. Neu co KTV moi thi can lam gi?

Them nhan vien co `VAITRO = 'Ky thuat vien'`, tao/dong bo user bang `SP_SYNC_TASK1_RBAC_USERS` hoac procedure them nhan vien, sau do user moi se duoc gan `ROLE_KYTHUATVIEN`.

## 17. Loi thuong gap

### Login sai mat khau hoac user bi lock

Dang nhap admin va chay:

```sql
ALTER USER C##NV007 IDENTIFIED BY "123456" ACCOUNT UNLOCK;
ALTER USER C##BN000001 IDENTIFIED BY "123456" ACCOUNT UNLOCK;
```

### Khong vao dung form KTV/BN

Kiem tra role cua user:

```sql
SELECT grantee, granted_role
FROM dba_role_privs
WHERE grantee IN ('C##NV007', 'C##BN000001');
```

Neu thieu role, chay:

```sql
BEGIN
    ADMIN_PHANHE1.SP_SYNC_TASK1_RBAC_USERS('123456');
END;
/
```

### SQL Developer bao ORA-01031 khi query bang goc

Day la ket qua dung neu dang nhap bang KTV/BN. User chi duoc query view RBAC, khong duoc query truc tiep bang goc.

### View khong co dong nao

Kiem tra username dang login co khop ma nhan vien/benh nhan khong:

```sql
SELECT USER FROM dual;
```

Neu login `C##NV007`, view KTV can co dong `MANV = NV007`. Neu login `C##BN000001`, view BN can co dong `MABN = BN000001`.
