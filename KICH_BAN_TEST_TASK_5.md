# KICH BAN TEST TASK 5 - Audit UI + Backup/Recover

Tai lieu nay dung de test va demo phan cua **Nhu - Task 5**: UI Audit, Backup/Recover, chuan hoa connection va test tong hop he thong.

## 0. Nen test bao nhieu la du?

Kich ban nay viet day du de phong truong hop thay/co hoi dong hoi sau. Khi demo that te, khong bat buoc chay het tat ca.

### Nhóm test bắt buộc nên chụp hình/demo

Chi can 6 nhom nay la da bao phu Task 5:

1. **Test connection**: chung minh UI ket noi duoc DB.
2. **Tai audit log ALL**: chung minh UI doc duoc `V_ALL_AUDIT_LOG`.
3. **Loc audit theo object HSBA**: chung minh bo loc object hoat dong.
4. **Standard Audit**: chung minh co script SQL cai dat standard audit va UI doc duoc log `STANDARD`.
5. **Loc audit FINE-GRAINED**: chung minh xem duoc FGA log.
6. **Backup Data Pump**: chung minh tao duoc file backup `.dmp`.
7. **Flashback restore**: chung minh recover du lieu dua tren moc thoi gian/audit.

### Nhóm test mở rộng

Nhung test con lai dung khi can giai thich sau hon:

- Tao audit log moi bang SQL.
- Restore Data Pump mot bang/schema.
- Mo RMAN backup/restore script.
- Test tong hop task 1, 2, 3 sau recover.

Neu chi can nop minh chung UI Task 5, 4 hinh dau cua ban hien tai da dat phan **Audit UI**. Can them toi thieu 1 hinh backup Data Pump va 1 hinh flashback restore de tron phan **Backup/Recover**.

## 1. Chuan bi truoc khi test

### 1.1. Build app

Tai thu muc goc repo:

```powershell
dotnet build ADMIN\ADMIN.csproj
```

Ket qua mong doi:

```text
Build succeeded.
0 Error(s)
```

### 1.2. Chuan bi database

Can chay script tong:

```sql
@oracle/CQ2026-CQ16-PH1-Database.sql
```

Sau do chay them script Standard Audit rieng cho Task 5:

```sql
@oracle/task5_standard_audit.sql
```

Muc dich cua file `task5_standard_audit.sql`:

- Cai dat Standard Audit ro rang cho Task 5.
- Audit cac bang quan trong: `HSBA`, `HSBA_DV`, `DONTHUOC`.
- Audit viec doc audit log qua view.
- Audit cac procedure backup/recover: `SP_BACKUP_DATAPUMP`, `SP_RESTORE_DATAPUMP`, `SP_RESTORE_FLASHBACK`.
- Tao view rieng `ADMIN_PHANHE1.V_TASK5_STANDARD_AUDIT_LOG` de demo SQL/Oracle.

Nen chay script nay bang `SYS AS SYSDBA` hoac `SYSTEM` co quyen DBA. Neu script doi `audit_trail = DB, EXTENDED`, can restart database de Oracle bat dau ghi audit day du SQL text.

### 1.3. Kiem tra cac object cua task 5

Mo **Oracle SQL Developer** hoac **SQL*Plus** va dang nhap bang connection co quyen xem object cua schema `ADMIN_PHANHE1`.

Khuyen nghi dung mot trong hai cach sau:

**Cach 1 - Dang nhap bang ADMIN_PHANHE1**

Dung khi script tong da tao xong user `ADMIN_PHANHE1`.

```text
Username: ADMIN_PHANHE1
Password: Admin@123456
Host: localhost
Port: 1521
Service name: orcl21 hoac orclpdb1 tuy may
```

Neu dung SQL*Plus:

```powershell
sqlplus ADMIN_PHANHE1/Admin@123456@localhost:1521/orcl21
```

**Cach 2 - Dang nhap bang SYSTEM hoac SYS AS SYSDBA**

Dung khi can kiem tra quyen, directory, audit trail hoac debug loi object.

```text
Username: SYSTEM
Password: mat khau SYSTEM cua may ban
Role: default
```

Hoac:

```text
Username: SYS
Role: SYSDBA
```

Sau khi dang nhap, chay cau lenh kiem tra:

```sql
SELECT object_name, object_type, status
FROM all_objects
WHERE owner = 'ADMIN_PHANHE1'
  AND object_name IN (
    'V_ALL_AUDIT_LOG',
    'V_TASK5_STANDARD_AUDIT_LOG',
    'SP_BACKUP_DATAPUMP',
    'SP_RESTORE_DATAPUMP',
    'SP_RESTORE_FLASHBACK'
  )
ORDER BY object_name;
```

Ket qua mong doi:

- `V_ALL_AUDIT_LOG` ton tai va `VALID`.
- `V_TASK5_STANDARD_AUDIT_LOG` ton tai va `VALID`.
- `SP_BACKUP_DATAPUMP` ton tai va `VALID`.
- `SP_RESTORE_DATAPUMP` ton tai va `VALID`.
- `SP_RESTORE_FLASHBACK` ton tai va `VALID`.

### 1.3.1. Kiem tra Standard Audit da cai dat

Dang nhap SQL Developer bang `SYS AS SYSDBA` hoac `SYSTEM`, chay:

```sql
SELECT owner, object_name, object_type,
       sel, ins, upd, del, exe
FROM dba_obj_audit_opts
WHERE owner = 'ADMIN_PHANHE1'
  AND object_name IN (
      'HSBA',
      'HSBA_DV',
      'DONTHUOC',
      'V_ALL_AUDIT_LOG',
      'V_TASK5_STANDARD_AUDIT_LOG',
      'SP_BACKUP_DATAPUMP',
      'SP_RESTORE_DATAPUMP',
      'SP_RESTORE_FLASHBACK'
  )
ORDER BY object_type, object_name;
```

Ket qua mong doi:

- `HSBA`, `HSBA_DV`, `DONTHUOC` co audit `SEL/INS/UPD/DEL`.
- Cac procedure backup/recover co audit `EXE`.
- View audit log co audit `SEL`.

Kiem tra audit session/DDL:

```sql
SELECT audit_option, success, failure
FROM dba_stmt_audit_opts
WHERE audit_option IN ('CREATE TABLE', 'DROP TABLE', 'ALTER TABLE', 'TABLE', 'SESSION')
ORDER BY audit_option;
```

Ket qua mong doi: co cau hinh audit session that bai va audit DDL/table.

### 1.4. Chuan bi thu muc backup

#### Muc dich

Phan nay dung rieng cho **Task 5 - Backup/Recover**.

Khi bam nut **Backup Data Pump** tren UI, procedure `SP_BACKUP_DATAPUMP` se goi Oracle Data Pump de xuat du lieu schema `ADMIN_PHANHE1` ra file backup. Oracle can mot thu muc that tren may/deployment server de ghi cac file:

```text
BV_PHANHE1_YYYYMMDD_HH24MISS.dmp
BV_PHANHE1_YYYYMMDD_HH24MISS.log
```

Thu muc `C:\Backup_Oracle` chinh la noi luu cac file do khi test tren Windows local. Sau nay khi restore, UI/procedure `SP_RESTORE_DATAPUMP` se doc lai file `.dmp` trong thu muc nay.

#### Tai sao Task 5 can, cac task khac khong can?

- Task 1, 2, 3 chu yeu test **quyen truy cap, RBAC, VPD, OLS**. Cac task do chi doc/ghi du lieu truc tiep trong database, khong can tao file backup tren he dieu hanh.
- Task 4/5 co phan **backup va recover**, nen bat buoc can mot vi tri ngoai database de Oracle ghi file backup va doc file restore.
- Trong Oracle, Data Pump khong ghi file theo duong dan tuy y tu C#; no ghi thong qua object `DIRECTORY` cua Oracle, o day la `BACKUP_DIR`.

Neu test tren Windows local:

```powershell
New-Item -ItemType Directory -Force C:\Backup_Oracle
```

Kiem tra/sua Oracle directory:

```sql
CREATE OR REPLACE DIRECTORY BACKUP_DIR AS 'C:\Backup_Oracle';
GRANT READ, WRITE ON DIRECTORY BACKUP_DIR TO ADMIN_PHANHE1;
```

Kiem tra:

```sql
SELECT directory_name, directory_path
FROM all_directories
WHERE directory_name = 'BACKUP_DIR';
```

Ket qua mong doi: `BACKUP_DIR` tro den `C:\Backup_Oracle`.

## 2. Test Case 01 - Test Connection tren UI

### Muc tieu

Xac nhan app ket noi duoc vao Oracle bang connection da chuan hoa trong `OracleHelper`.

### Buoc test

1. Chay app:

```powershell
dotnet run --project ADMIN\ADMIN.csproj
```

2. Dang nhap bang user admin:

```text
Username: ADMIN_PHANHE1
Password: Admin@123456
```

3. Mo tab **Audit + Backup/Recover**.
4. Bam nut **Test connection**.

### Ket qua mong doi

O khung log phia duoi hien:

```text
Ket noi ADMIN_PHANHE1 thanh cong.
```

Neu loi service name, set lai bien moi truong roi chay lai app:

```powershell
$env:ATBM_DB_SERVICE="orclpdb1"
dotnet run --project ADMIN\ADMIN.csproj
```

Hoac neu may dung SID `xe`:

```powershell
$env:ATBM_DB_SID="xe"
dotnet run --project ADMIN\ADMIN.csproj
```

## 3. Test Case 02 - Doc Audit Log tren UI

### Muc tieu

Xac nhan UI doc duoc nhat ky audit tu view `ADMIN_PHANHE1.V_ALL_AUDIT_LOG`.

### Buoc test

1. Mo tab **Audit + Backup/Recover**.
2. Chon:

```text
Loai audit: ALL
Nguoi dung: de trong
Doi tuong: de trong
So dong: 100
```

3. Bam **Tai audit log**.

### Ket qua mong doi

Bang du lieu hien cac cot:

- `LOAI_AUDIT`
- `NGUOI_DUNG`
- `THOI_GIAN`
- `HANH_DONG`
- `DOI_TUONG`
- `CAU_LENH_SQL`
- `CHI_TIET_TRANG_THAI`

Khung log hien dang:

```text
Da tai N dong audit log.
```

### SQL kiem chung

```sql
SELECT *
FROM ADMIN_PHANHE1.V_ALL_AUDIT_LOG
WHERE ROWNUM <= 20;
```

## 4. Test Case 03 - Loc Audit Log theo Object

### Muc tieu

Xac nhan UI loc duoc audit theo bang/object.

### Buoc test

1. Trong tab **Audit + Backup/Recover**.
2. Nhap:

```text
Loai audit: ALL
Doi tuong: HSBA
So dong: 100
```

3. Bam **Tai audit log**.

### Ket qua mong doi

Bang chi hien cac dong co `DOI_TUONG` lien quan `HSBA`, vi du:

- `ADMIN_PHANHE1.HSBA`
- `ADMIN_PHANHE1.HSBA_DV`

## 5. Test Case 04 - Loc Audit Log theo Fine-Grained Audit

### Muc tieu

Xac nhan UI loc duoc rieng log FGA.

### Buoc test

1. Chon:

```text
Loai audit: FINE-GRAINED
Nguoi dung: de trong
Doi tuong: de trong
So dong: 100
```

2. Bam **Tai audit log**.

### Ket qua mong doi

Cot `LOAI_AUDIT` cua cac dong hien thi la:

```text
FINE-GRAINED
```

Cot `HANH_DONG` co the hien policy, vi du:

```text
POLICY: FGA_HSBA_CAPNHAT_HOPPHAP
```

## 5A. Test Case 04A - Standard Audit bang SQL

### Muc tieu

Chung minh Task 5 co phan Oracle/SQL rieng: Standard Audit duoc cai dat tren table, view va procedure.

### Dieu kien

Da chay:

```sql
@oracle/task5_standard_audit.sql
```

Neu `audit_trail` vua duoc doi sang `DB, EXTENDED`, restart database roi moi test.

### Buoc test 1 - Tao log STANDARD thanh cong tren table HSBA

Dang nhap SQL Developer bang `ADMIN_PHANHE1`, chay:

```sql
SELECT TO_CHAR(SYSTIMESTAMP, 'YYYY-MM-DD HH24:MI:SS') AS before_standard_test
FROM dual;

UPDATE ADMIN_PHANHE1.HSBA
SET CHANDOAN = CHANDOAN
WHERE ROWNUM = 1;

COMMIT;
```

Ket qua mong doi:

- Cau `UPDATE` thanh cong.
- Standard Audit ghi log hanh dong `UPDATE` tren `ADMIN_PHANHE1.HSBA`.

### Buoc test 2 - Tao log STANDARD khi doc audit view

Van dang nhap `ADMIN_PHANHE1`, chay:

```sql
SELECT *
FROM ADMIN_PHANHE1.V_TASK5_STANDARD_AUDIT_LOG
WHERE ROWNUM <= 5;
```

Ket qua mong doi:

- Query chay duoc.
- Chinh hanh vi doc view audit log cung se duoc standard audit ghi nhan do da audit `SELECT` tren view.

### Buoc test 3 - Kiem tra log STANDARD bang SQL

Chay:

```sql
SELECT NGUOI_DUNG, THOI_GIAN, HANH_DONG, DOI_TUONG, TRANG_THAI, CAU_LENH_SQL
FROM ADMIN_PHANHE1.V_TASK5_STANDARD_AUDIT_LOG
WHERE DOI_TUONG LIKE 'ADMIN_PHANHE1.HSBA%'
   OR DOI_TUONG LIKE 'ADMIN_PHANHE1.V_TASK5_STANDARD_AUDIT_LOG%'
FETCH FIRST 20 ROWS ONLY;
```

Ket qua mong doi:

- Co dong `HANH_DONG = UPDATE`, `DOI_TUONG = ADMIN_PHANHE1.HSBA`, `TRANG_THAI = Thanh cong`.
- Co dong `HANH_DONG = SELECT`, `DOI_TUONG = ADMIN_PHANHE1.V_TASK5_STANDARD_AUDIT_LOG`.

### Buoc test 4 - Kiem tra log STANDARD tren UI

1. Mo app va dang nhap admin.
2. Mo tab **Audit + Backup/Recover**.
3. Chon:

```text
Loai audit: STANDARD
Doi tuong: HSBA
So dong: 100
```

4. Bam **Tai audit log**.

Ket qua mong doi:

- Bang UI chi hien cac dong `LOAI_AUDIT = STANDARD`.
- Co dong lien quan `ADMIN_PHANHE1.HSBA`.

### Buoc test 5 - Tao log STANDARD that bai

Muc nay dung khi van dap can chung minh audit ca hanh vi that bai.

Dang nhap SQL Developer bang user khong co quyen bang goc, vi du `C##NV007/123456`, chay:

```sql
SELECT COUNT(*)
FROM ADMIN_PHANHE1.HSBA;
```

Ket qua mong doi:

- Cau query bi chan, thuong gap `ORA-01031: insufficient privileges`.
- Standard Audit co the ghi dong `TRANG_THAI = That bai (...)` cho hanh vi truy cap bi tu choi.

Sau do dang nhap lai `ADMIN_PHANHE1` hoac dung UI filter:

```text
Loai audit: STANDARD
Nguoi dung: C##NV007
Doi tuong: HSBA
```

Ket qua mong doi: neu audit_trail da bat dung va policy co hieu luc, thay log that bai cua `C##NV007`.

## 6. Test Case 05 - Tao Audit Log moi bang SQL

### Muc tieu

Tao hanh vi update de audit ghi nhan, sau do doc lai tren UI.

### Buoc test SQL

Dang nhap bang `ADMIN_PHANHE1` hoac user co quyen phu hop, chay:

```sql
SELECT TO_CHAR(SYSTIMESTAMP, 'YYYY-MM-DD HH24:MI:SS') AS before_test
FROM dual;

UPDATE ADMIN_PHANHE1.HSBA
SET CHANDOAN = CHANDOAN
WHERE ROWNUM = 1;

UPDATE ADMIN_PHANHE1.DONTHUOC
SET LIEUDUNG = LIEUDUNG
WHERE ROWNUM = 1;

COMMIT;
```

### Buoc test UI

1. Quay lai app.
2. Mo tab **Audit + Backup/Recover**.
3. Bam **Tai audit log**.
4. Thu loc `Doi tuong = HSBA`, sau do `Doi tuong = DONTHUOC`.

### Ket qua mong doi

Audit log co them dong lien quan `HSBA` hoac `DONTHUOC`.

## 7. Test Case 06 - Backup Data Pump

### Muc tieu

Xac nhan UI goi duoc procedure backup Data Pump va tao file `.dmp`.

### Buoc test

1. Mo tab **Audit + Backup/Recover**.
2. Bam **Backup Data Pump**.
3. Doi job backup chay xong.
4. Mo thu muc:

```text
C:\Backup_Oracle
```

### Ket qua mong doi

Trong thu muc backup co file dang:

```text
BV_PHANHE1_YYYYMMDD_HH24MISS.dmp
BV_PHANHE1_YYYYMMDD_HH24MISS.log
```

Khung log UI hien:

```text
Da gui job backup Data Pump. File .dmp nam trong Oracle DIRECTORY BACKUP_DIR.
```

### SQL kiem chung

```sql
SELECT owner_name, job_name, operation, job_mode, state
FROM dba_datapump_jobs
ORDER BY job_name DESC;
```

## 8. Test Case 07 - Restore Data Pump mot bang

### Muc tieu

Xac nhan UI goi duoc restore Data Pump cho mot bang cu the.

### Canh bao

Restore co the ghi de du lieu. Chi test sau khi da co file backup va chap nhan rollback du lieu test.

### Buoc test

1. Lay ten file `.dmp` vua tao, vi du:

```text
BV_PHANHE1_20260623_230000.dmp
```

2. Tren UI nhap:

```text
File restore: BV_PHANHE1_20260623_230000.dmp
Bang restore: HSBA
```

3. Bam **Restore Data Pump**.
4. Bam **Yes** de xac nhan.

### Ket qua mong doi

Khung log UI hien:

```text
Da gui job restore Data Pump.
```

SQL kiem tra bang van truy van duoc:

```sql
SELECT COUNT(*) AS total_hsba
FROM ADMIN_PHANHE1.HSBA;
```

## 9. Test Case 08 - Flashback Restore dua tren moc thoi gian an toan

### Muc tieu

Xac nhan UI co the khoi phuc bang ve moc thoi gian an toan bang procedure `SP_RESTORE_FLASHBACK`.

### Buoc 1 - Ghi moc thoi gian an toan

Chay SQL:

```sql
SELECT TO_CHAR(SYSTIMESTAMP, 'YYYY-MM-DD HH24:MI:SS') AS safe_time
FROM dual;
```

Ghi lai gia tri `safe_time`, vi du:

```text
2026-06-23 23:10:00
```

### Buoc 2 - Sua sai du lieu test

Lay mot dong test:

```sql
SELECT MAHSBA, CHANDOAN
FROM ADMIN_PHANHE1.HSBA
WHERE ROWNUM = 1;
```

Sua sai:

```sql
UPDATE ADMIN_PHANHE1.HSBA
SET CHANDOAN = 'TEST SAI DU LIEU TASK 5'
WHERE ROWNUM = 1;

COMMIT;
```

Kiem tra da sai:

```sql
SELECT MAHSBA, CHANDOAN
FROM ADMIN_PHANHE1.HSBA
WHERE CHANDOAN = 'TEST SAI DU LIEU TASK 5';
```

### Buoc 3 - Flashback tren UI

1. Mo tab **Audit + Backup/Recover**.
2. Chon:

```text
Bang flashback: ADMIN_PHANHE1.HSBA
Ngay: ngay trong safe_time
Gio: gio trong safe_time, dinh dang HH:mm:ss
```

3. Bam **Flashback restore**.
4. Bam **Yes** de xac nhan.

### Ket qua mong doi

Khung log hien:

```text
Da flashback ADMIN_PHANHE1.HSBA ve YYYY-MM-DD HH:mm:ss.
```

Kiem tra lai:

```sql
SELECT MAHSBA, CHANDOAN
FROM ADMIN_PHANHE1.HSBA
WHERE CHANDOAN = 'TEST SAI DU LIEU TASK 5';
```

Ket qua mong doi: khong con dong nao co gia tri `TEST SAI DU LIEU TASK 5`.

## 10. Test Case 09 - Mo RMAN Backup Script tu UI

### Muc tieu

Xac nhan nut UI mo duoc script RMAN backup `.bat`.

### Buoc test

1. Mo tab **Audit + Backup/Recover**.
2. Bam **RMAN backup .bat**.

### Ket qua mong doi

Mo console chay file:

```text
backup_restore/window/run_rman_backup.bat
```

Neu RMAN cau hinh dung, file backup nam tai:

```text
C:\Backup_Oracle
```

Neu khong co RMAN tren may test, chi can demo UI mo script va giai thich day la backup cap he thong.

## 11. Test Case 10 - Mo RMAN Restore Script tu UI

### Muc tieu

Xac nhan nut UI mo duoc script RMAN restore `.bat`.

### Canh bao

Khong nen chay RMAN restore tren database dang demo neu khong co moi truong rieng, vi script co the shutdown/mount/recover database.

### Buoc test an toan

1. Mo tab **Audit + Backup/Recover**.
2. Bam **RMAN restore .bat** chi khi co moi truong test rieng.
3. Neu demo tren may chinh, chi mo file `.bat` de trinh bay noi dung script, khong thuc thi.

### Ket qua mong doi

Neu chay trong moi truong test rieng, script thuc hien:

- `SHUTDOWN ABORT`
- `STARTUP MOUNT`
- `RESTORE DATABASE`
- `RECOVER DATABASE`
- `ALTER DATABASE OPEN`

## 12. Test Case 11 - Test tong hop sau khi recover

### Muc tieu

Dam bao sau backup/recover, cac phan he chinh van hoat dong.

### Buoc test nhanh

1. Dang nhap admin.
2. Mo tab **User**, tim danh sach `Nhan vien` va `Benh nhan`.
3. Mo tab **Thong tin quyen**, bam **Xem quyen**.
4. Dang nhap user KTV, vi du `C##NV007`, test xem/sua ket qua dich vu.
5. Dang nhap user benh nhan, vi du `C##BN000001`, test xem/sua thong tin ca nhan.
6. Dang nhap user OLS `U1` den `U8`, test thong bao.
7. Quay lai admin, mo tab **Audit + Backup/Recover**, bam **Tai audit log**.

### Ket qua mong doi

- App khong crash.
- Du lieu van doc duoc.
- Cac chinh sach RBAC/VPD/OLS van co hieu luc.
- Audit log tiep tuc ghi nhan thao tac.

## 13. Checklist ket qua demo Task 5

Danh dau khi demo:

- [ ] Build app thanh cong.
- [ ] Chay `oracle/task5_standard_audit.sql` thanh cong.
- [ ] Kiem tra `V_TASK5_STANDARD_AUDIT_LOG` valid.
- [ ] Kiem tra `DBA_OBJ_AUDIT_OPTS` co audit tren `HSBA`, `HSBA_DV`, `DONTHUOC`.
- [ ] Test connection thanh cong.
- [ ] Doc duoc `V_ALL_AUDIT_LOG` tren UI.
- [ ] Loc duoc audit theo `STANDARD` / `FINE-GRAINED`.
- [ ] Tao duoc log `STANDARD` bang SQL va xem lai tren UI.
- [ ] Loc duoc audit theo user/object.
- [ ] Tao them audit log moi va doc lai duoc.
- [ ] Backup Data Pump tao file `.dmp`.
- [ ] Restore Data Pump goi procedure thanh cong.
- [ ] Flashback restore khoi phuc du lieu ve moc an toan.
- [ ] Nut RMAN backup/restore mo dung script.
- [ ] Test nhanh lai cac task 1, 2, 3 sau khi recover.

## 14. Loi thuong gap khi test

### ORA-12514

Sai service name. Kiem tra:

```powershell
lsnrctl status
```

Set lai:

```powershell
$env:ATBM_DB_SERVICE="orclpdb1"
```

### ORA-01017

Sai password hoac user bi lock:

```sql
ALTER USER ADMIN_PHANHE1 ACCOUNT UNLOCK;
ALTER USER ADMIN_PHANHE1 IDENTIFIED BY Admin@123456;
```

### ORA-39087

Sai `BACKUP_DIR`:

```sql
CREATE OR REPLACE DIRECTORY BACKUP_DIR AS 'C:\Backup_Oracle';
GRANT READ, WRITE ON DIRECTORY BACKUP_DIR TO ADMIN_PHANHE1;
```

### UI bao khong tim thay file `.bat`

Kiem tra cac file nay con trong repo:

```text
backup_restore/window/run_rman_backup.bat
backup_restore/window/run_rman_restore.bat
```

Neu chay exe o thu muc publish rieng, copy 2 file `.bat` vao cung thu muc voi `ADMIN.exe`.
