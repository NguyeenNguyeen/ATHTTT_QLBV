# HUONG DAN TASK 5 - Audit UI, Backup/Recover, Connection, Test Tong Hop

Tai lieu nay danh cho **Nhu - Task 5**. Task 5 khong chi lam giao dien Audit + Backup/Recover, ma con gom viec chuan hoa connection, xem/test ket qua ung dung, huong dan build/run va tong ket demo cho ca he thong.

## 1. Viec da tich hop trong source

### 1.1. UI Audit + Backup/Recover

Da them tab moi trong man hinh admin `Form1`: **Audit + Backup/Recover**.

Tab nay co cac chuc nang:

- **Test connection**: kiem tra app co ket noi duoc vao schema `ADMIN_PHANHE1` hay khong.
- **Tai audit log**: doc view `ADMIN_PHANHE1.V_ALL_AUDIT_LOG`.
- **Loc audit** theo loai `ALL`, `STANDARD`, `FINE-GRAINED`, user, object va so dong.
- **Backup Data Pump**: goi procedure `ADMIN_PHANHE1.SP_BACKUP_DATAPUMP`.
- **Restore Data Pump**: goi procedure `ADMIN_PHANHE1.SP_RESTORE_DATAPUMP(p_filename, p_table_name)`.
- **Flashback restore**: goi procedure `ADMIN_PHANHE1.SP_RESTORE_FLASHBACK(p_table_name, p_safe_time)`.
- **RMAN backup/restore .bat**: mo script trong `backup_restore/window`.

### 1.2. Chuan hoa connection

Da chuan hoa cac man admin de dung chung:

```csharp
OracleHelper.AdminConnectionString
```

Cac file lien quan:

- `ADMIN/OracleHelper.cs`
- `ADMIN/Form1.cs`
- `ADMIN/AddUserForm.cs`

Mac dinh app ket noi:

- User: `ADMIN_PHANHE1`
- Password: `Admin@123456`
- Host: `localhost`
- Port: `1521`
- Service name: `orcl21`

Neu may ban dung PDB/service khac, khong can sua code. Set bien moi truong truoc khi chay app.

Vi du PowerShell:

```powershell
$env:ATBM_DB_SERVICE="orclpdb1"
dotnet run --project ADMIN\ADMIN.csproj
```

Neu Oracle cua ban dung SID `xe`:

```powershell
$env:ATBM_DB_SID="xe"
dotnet run --project ADMIN\ADMIN.csproj
```

Neu muon override toan bo connection string:

```powershell
$env:ATBM_ADMIN_CONN="User Id=ADMIN_PHANHE1;Password=Admin@123456;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orclpdb1)));"
dotnet run --project ADMIN\ADMIN.csproj
```

## 2. Cach cai database tren may

### 2.1. Mo Oracle va tao thu muc backup

Neu chay Oracle tren Windows local, tao thu muc:

```powershell
New-Item -ItemType Directory -Force C:\Backup_Oracle
```

Trong script SQL hien tai, dong `CREATE OR REPLACE DIRECTORY BACKUP_DIR AS '/backup';` phu hop Docker/Linux. Neu chay Windows local, sua thanh:

```sql
CREATE OR REPLACE DIRECTORY BACKUP_DIR AS 'C:\Backup_Oracle';
GRANT READ, WRITE ON DIRECTORY BACKUP_DIR TO SYSTEM;
GRANT READ, WRITE ON DIRECTORY BACKUP_DIR TO ADMIN_PHANHE1;
```

### 2.2. Chay script database tong

Mo SQL Developer hoac SQL*Plus bang `SYS AS SYSDBA` hoac `SYSTEM` co quyen DBA, sau do chay:

```sql
@oracle/CQ2026-CQ16-PH1-Database.sql
```

Neu dung SQL Developer:

1. Mo file `oracle/CQ2026-CQ16-PH1-Database.sql`.
2. Chon connection dung CDB/PDB cua do an.
3. Bam **Run Script** (`F5`), khong bam Run Statement.
4. Doi den khi script tao xong user, bang, data, role, VPD/OLS, audit, backup procedure.

### 2.3. Kiem tra object quan trong

Dang nhap `ADMIN_PHANHE1`, chay:

```sql
SELECT object_name, object_type, status
FROM all_objects
WHERE owner = 'ADMIN_PHANHE1'
  AND object_name IN (
    'V_ALL_AUDIT_LOG',
    'SP_BACKUP_DATAPUMP',
    'SP_RESTORE_DATAPUMP',
    'SP_RESTORE_FLASHBACK'
  )
ORDER BY object_type, object_name;
```

Ket qua mong doi: cac object tren co `STATUS = VALID`.

Kiem tra audit view:

```sql
SELECT *
FROM ADMIN_PHANHE1.V_ALL_AUDIT_LOG
WHERE ROWNUM <= 20;
```

## 3. Cach build/run app

Tai root repo:

```powershell
dotnet build ADMIN\ADMIN.csproj
dotnet run --project ADMIN\ADMIN.csproj
```

Neu chay file exe sau khi build:

```powershell
.\ADMIN\bin\Debug\net10.0-windows\ADMIN.exe
```

Dang nhap admin bang tai khoan co quyen vao phan he 1, thuong la:

- Username: `ADMIN_PHANHE1`
- Password: `Admin@123456`

Neu login form khong dua vao form admin, dang nhap bang user admin/common user phu hop script cua nhom.

## 4. Test Task 5 tren UI

### 4.1. Test connection

1. Chay app.
2. Dang nhap admin.
3. Mo tab **Audit + Backup/Recover**.
4. Bam **Test connection**.

Ket qua mong doi: o log duoi man hinh hien `Ket noi ADMIN_PHANHE1 thanh cong.`

### 4.2. Test doc audit log

1. Trong tab **Audit + Backup/Recover**.
2. Chon `ALL`.
3. De trong User/Object.
4. So dong de `100`.
5. Bam **Tai audit log**.

Ket qua mong doi: bang hien cac cot:

- `LOAI_AUDIT`
- `NGUOI_DUNG`
- `THOI_GIAN`
- `HANH_DONG`
- `DOI_TUONG`
- `CAU_LENH_SQL`
- `CHI_TIET_TRANG_THAI`

Loc nhanh:

- Nhap Object: `HSBA`, bam **Tai audit log**.
- Chon type: `FINE-GRAINED`, bam **Tai audit log**.

### 4.3. Tao du lieu audit de test

Neu log dang trong, tao hanh vi de audit ghi nhan.

Vi du dang nhap bang user bac si/y si va sua HSBA tren app, hoac chay SQL test cua thanh vien audit. Sau do quay lai admin, bam **Tai audit log**.

Co the test bang SQL:

```sql
UPDATE ADMIN_PHANHE1.HSBA
SET CHANDOAN = CHANDOAN
WHERE ROWNUM = 1;

UPDATE ADMIN_PHANHE1.DONTHUOC
SET LIEUDUNG = LIEUDUNG
WHERE ROWNUM = 1;

COMMIT;
```

Sau do doc:

```sql
SELECT *
FROM ADMIN_PHANHE1.V_ALL_AUDIT_LOG
WHERE ROWNUM <= 20;
```

### 4.4. Test Backup Data Pump

Dieu kien:

- `BACKUP_DIR` da tro dung thu muc that.
- `ADMIN_PHANHE1` co quyen `READ`, `WRITE` tren directory.
- `ADMIN_PHANHE1` co quyen Data Pump trong script.

Tren UI:

1. Bam **Backup Data Pump**.
2. Doi job chay xong.
3. Mo thu muc backup, vi du `C:\Backup_Oracle`.

Ket qua mong doi: co file dang:

```text
BV_PHANHE1_YYYYMMDD_HH24MISS.dmp
BV_PHANHE1_YYYYMMDD_HH24MISS.log
```

Kiem tra job Data Pump:

```sql
SELECT owner_name, job_name, operation, job_mode, state
FROM dba_datapump_jobs
ORDER BY job_name DESC;
```

### 4.5. Test Restore Data Pump

Can than: restore co the replace data/schema. Nen chi test khi da backup va chap nhan rollback.

Tren UI:

1. O **File restore**, nhap dung ten file `.dmp`, vi du `BV_PHANHE1_20260623_230000.dmp`.
2. O **Bang restore**, de trong neu restore schema, hoac nhap `HSBA` neu chi restore bang.
3. Bam **Restore Data Pump**.
4. Xac nhan.

Ket qua mong doi: log hien `Da gui job restore Data Pump.`

### 4.6. Test Flashback restore dua tren audit log

Flashback trong script chi cho phep 3 bang:

- `ADMIN_PHANHE1.HSBA`
- `ADMIN_PHANHE1.HSBA_DV`
- `ADMIN_PHANHE1.DONTHUOC`

Quy trinh demo an toan:

1. Ghi lai moc thoi gian an toan truoc khi sua sai:

```sql
SELECT TO_CHAR(SYSTIMESTAMP, 'YYYY-MM-DD HH24:MI:SS') AS safe_time
FROM dual;
```

2. Sua sai mot dong trong `HSBA`, `HSBA_DV` hoac `DONTHUOC`.
3. Kiem tra audit log da ghi nhan hanh vi.
4. Tren UI, chon bang can flashback.
5. Nhap ngay va gio `safe_time`.
6. Bam **Flashback restore** va xac nhan.
7. Query lai bang de kiem tra du lieu quay ve thoi diem an toan.

## 5. Test cac task con lai de tong ket he thong

### Task 1 - KTV/BN RBAC

- Login `C##NV007` hoac user ky thuat vien mau.
- KTV chi xem/sua thong tin ca nhan duoc phep va chi cap nhat `KETQUA` dich vu cua minh.
- Login `C##BN000001` hoac benh nhan mau.
- BN chi xem/sua thong tin cua chinh minh, khong sua cot dinh danh.

SQL test:

```sql
@oracle/test_task1_rbac.sql
```

### Task 2 - DPV/YBS VPD

- Login dieu phoi vien.
- Kiem tra xem/them/sua benh nhan, tao HSBA, dieu phoi bac si/ky thuat vien.
- Login bac si/y si.
- Kiem tra chi thay HSBA minh phu trach, update cac cot duoc phep.

### Task 3 - OLS

- Login cac user `U1` den `U8`.
- Mo giao dien thong bao OLS.
- Kiem tra moi user chi doc dung thong bao theo cap bac, khoa va co so.

### Task 4 - Audit + Backup/Recover SQL

- Chay cac cau lenh tao audit policy trong script tong.
- Tao hanh vi hop phap/bat hop phap.
- Query `V_ALL_AUDIT_LOG`.
- Chay backup/restore procedure hoac script `.bat`.

### Task 5 - UI + tong hop

- Mo tab **Audit + Backup/Recover**.
- Test connection.
- Tai audit log.
- Backup Data Pump.
- Flashback restore mot bang sau khi co audit log.
- Build/run lai app tu dau theo huong dan.

## 6. Loi thuong gap

### ORA-12514 hoac service khong ton tai

Sai service name. Kiem tra service:

```powershell
lsnrctl status
```

Sau do set:

```powershell
$env:ATBM_DB_SERVICE="ten_service_dung"
```

### ORA-01017 sai username/password

Kiem tra user:

```sql
SELECT username, account_status
FROM dba_users
WHERE username = 'ADMIN_PHANHE1';
```

Neu bi lock:

```sql
ALTER USER ADMIN_PHANHE1 ACCOUNT UNLOCK;
ALTER USER ADMIN_PHANHE1 IDENTIFIED BY Admin@123456;
```

### ORA-39087 directory name invalid

`BACKUP_DIR` chua ton tai hoac sai duong dan:

```sql
CREATE OR REPLACE DIRECTORY BACKUP_DIR AS 'C:\Backup_Oracle';
GRANT READ, WRITE ON DIRECTORY BACKUP_DIR TO ADMIN_PHANHE1;
```

### Audit view bi loi quyen

Can cap quyen cho `ADMIN_PHANHE1`:

```sql
GRANT SELECT ON SYS.DBA_AUDIT_TRAIL TO ADMIN_PHANHE1;
GRANT SELECT ON SYS.DBA_FGA_AUDIT_TRAIL TO ADMIN_PHANHE1;
```

### Flashback restore that bai

Kiem tra bang da bat flashback archive:

```sql
SELECT table_name, flashback_archive_name
FROM dba_flashback_archive_tables
WHERE owner_name = 'ADMIN_PHANHE1';
```

## 7. Checklist nop/demo

- Build app thanh cong: `dotnet build ADMIN\ADMIN.csproj`.
- DB script tong chay xong, object task 5 `VALID`.
- Connection dung tren may demo.
- Tab **Audit + Backup/Recover** mo duoc.
- Audit log doc duoc tu UI.
- Backup tao duoc file `.dmp`.
- Flashback restore demo duoc mot bang.
- Moi thanh vien test task cua minh truoc khi tong hop.
- Co anh chup man hinh hoac ghi chu ket qua cho bao cao.
