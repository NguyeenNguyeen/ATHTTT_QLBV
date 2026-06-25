# Huong dan ky thuat build va run he thong

Tai lieu nay dung khi nop source code + script SQL cho nguoi moi hoac giang vien da co Oracle. Repo hien tai duoc chuan hoa theo moi truong:

- Oracle CDB service: `orcl21`
- Oracle PDB service: `orcl21pdb1`
- PDB target cua do an: `ORCL21PDB1`
- Port: `1521`
- Schema chinh: `ADMIN_PHANHE1`
- Mat khau schema chinh: `Admin@123456`
- Thu muc backup Windows: `C:\Backup_Oracle`

Neu may cham co ten PDB/service khac, can sua dong `ORCL21PDB1` trong script SQL va `orcl21pdb1` trong connection string cua WinForms.

## 1. Kiem tra Oracle dang chay

Mo PowerShell/CMD:

```powershell
lsnrctl status
```

Can thay service `orcl21` va `orcl21pdb1`.

Dang nhap SYSDBA:

```powershell
sqlplus / as sysdba
```

Kiem tra instance va PDB:

```sql
SELECT instance_name, status, database_status FROM v$instance;
SELECT name, open_mode FROM v$database;
SHOW PDBS;
```

Mo PDB neu chua open:

```sql
ALTER PLUGGABLE DATABASE ORCL21PDB1 OPEN;
ALTER PLUGGABLE DATABASE ORCL21PDB1 SAVE STATE;
SHOW PDBS;
```

Ket qua mong doi: `ORCL21PDB1` co `OPEN MODE = READ WRITE`.

## 2. Bat cac tinh nang Oracle can thiet

Phan nay chay bang `SYS AS SYSDBA` o CDB root, truoc khi chay script do an.

Kiem tra dang o root:

```sql
SHOW CON_NAME;
```

Neu khong phai `CDB$ROOT`, ket noi lai:

```powershell
sqlplus / as sysdba
```

Bat Standard Audit chi tiet:

```sql
ALTER SYSTEM SET audit_trail = DB, EXTENDED SCOPE = SPFILE;
```

Bat OLS mot lan cho database:

```sql
EXEC LBACSYS.CONFIGURE_OLS;
EXEC LBACSYS.OLS_ENFORCEMENT.ENABLE_OLS;
```

Bat ARCHIVELOG de RMAN hot backup/recover chay duoc:

```sql
SHUTDOWN IMMEDIATE;
STARTUP MOUNT;
ALTER DATABASE ARCHIVELOG;
ALTER DATABASE OPEN;
```

Mo lai PDB sau khi database open:

```sql
ALTER PLUGGABLE DATABASE ORCL21PDB1 OPEN;
ALTER PLUGGABLE DATABASE ORCL21PDB1 SAVE STATE;
```

Kiem tra ARCHIVELOG:

```sql
ARCHIVE LOG LIST;
```

Ket qua mong doi: `Database log mode` la `Archive Mode`.

Dam bao Oracle Scheduler khong bi tat:

```sql
SHOW PARAMETER job_queue_processes;
ALTER SYSTEM SET job_queue_processes = 100 SCOPE = BOTH;
```

## 3. Tao thu muc backup tren Windows

Mo PowerShell:

```powershell
New-Item -ItemType Directory -Force C:\Backup_Oracle
```

Script SQL chinh da tao Oracle directory:

```sql
CREATE OR REPLACE DIRECTORY BACKUP_DIR AS 'C:\Backup_Oracle';
```

Neu can kiem tra sau khi chay script:

```sql
ALTER SESSION SET CONTAINER = ORCL21PDB1;

SELECT directory_name, directory_path
FROM dba_directories
WHERE directory_name = 'BACKUP_DIR';
```

## 4. Chay script CSDL chinh

File chinh:

```text
oracle/CQ2026-CQ16-PH1-Database.sql
```

## 5. Kiem tra sau khi chay script

Dang nhap SYSDBA va chuyen vao PDB:

```sql
ALTER SESSION SET CONTAINER = ORCL21PDB1;
```

Kiem tra user:

```sql
SELECT username, account_status
FROM dba_users
WHERE username IN ('ADMIN_PHANHE1', 'U1', 'U2', 'U3', 'U4', 'U5', 'U6', 'U7', 'U8')
ORDER BY username;
```

Kiem tra object chinh:

```sql
SELECT object_name, object_type, status
FROM dba_objects
WHERE owner = 'ADMIN_PHANHE1'
  AND object_name IN (
    'BENHNHAN',
    'NHANVIEN',
    'HSBA',
    'HSBA_DV',
    'DONTHUOC',
    'THONGBAO',
    'V_ALL_AUDIT_LOG',
    'SP_BACKUP_DATAPUMP',
    'SP_RESTORE_DATAPUMP',
    'SP_RESTORE_FLASHBACK'
  )
ORDER BY object_type, object_name;
```

Tat ca object quan trong nen co `STATUS = VALID`.

Kiem tra audit view:

```sql
SELECT *
FROM ADMIN_PHANHE1.V_ALL_AUDIT_LOG
WHERE ROWNUM <= 10;
```

Kiem tra backup directory:

```sql
SELECT directory_name, directory_path
FROM dba_directories
WHERE directory_name = 'BACKUP_DIR';
```

Ket qua mong doi: `DIRECTORY_PATH = C:\Backup_Oracle`.

## 6. Build WinForms

Can .NET SDK co ho tro `net10.0-windows`.

Tai thu muc goc repo:

```powershell
dotnet restore ADMIN\ADMIN.csproj
dotnet build ADMIN\ADMIN.csproj -v:minimal
```

Neu build bao loi file `ADMIN.exe` dang bi lock, dong app WinForms dang chay roi build lai. Co the kiem tra compile khong tao apphost bang:

```powershell
dotnet build ADMIN\ADMIN.csproj -v:minimal -p:UseAppHost=false
```

## 7. Chay ung dung

Chay bang Visual Studio hoac:

```powershell
dotnet run --project ADMIN\ADMIN.csproj
```

Connection string trong app da dung PDB service:

```text
SERVICE_NAME=orcl21pdb1
```

Tai khoan admin:

```text
Username: ADMIN_PHANHE1
Password: Admin@123456
```

Tai khoan OLS:

```text
Username: U1
Password: User@123
```

Tuong tu `U2` den `U8`, password mac dinh la `User@123`.

Tai khoan nhan vien/benh nhan RBAC/VPD duoc tao tu data mau, password mac dinh:

```text
123456
```

Co the xem danh sach:

```sql
ALTER SESSION SET CONTAINER = ORCL21PDB1;

SELECT username
FROM dba_users
WHERE username LIKE 'NV%' OR username LIKE 'BN%'
ORDER BY username;
```

## 8. Test nhanh cac chuc nang chinh

Admin:

1. Dang nhap `ADMIN_PHANHE1/Admin@123456`.
2. Mo tab quan tri user/role.
3. Xem user, role, cap quyen, thu hoi quyen.
4. Mo tab `Audit + Backup/Recover`.
5. Bam tai audit log.

Audit:

```sql
ALTER SESSION SET CONTAINER = ORCL21PDB1;

UPDATE ADMIN_PHANHE1.HSBA
SET CHANDOAN = CHANDOAN
WHERE ROWNUM = 1;
COMMIT;

SELECT *
FROM ADMIN_PHANHE1.V_ALL_AUDIT_LOG
WHERE ROWNUM <= 20;
```

Backup Data Pump:

1. Mo app bang admin.
2. Mo tab `Audit + Backup/Recover`.
3. Bam `Backup Data Pump`.
4. Kiem tra file `.dmp` trong `C:\Backup_Oracle`.

RMAN backup:

```powershell
backup_restore\window\run_rman_backup.bat
```

Can database dang o `ARCHIVELOG`.

OLS:

1. Dang nhap `U1/User@123`.
2. Kiem tra U1 doc duoc toan bo thong bao phu hop.
3. Dang nhap cac user `U2` den `U8` de doi chieu so dong thong bao nhin thay.

## 9. Loi thuong gap

`ORA-65096: invalid common user or role name`

- Script dang chay o CDB root thay vi PDB.
- Kiem tra dau script co `ALTER SESSION SET CONTAINER = ORCL21PDB1`.
- Kiem tra PDB da open.

`ORA-12514: listener does not currently know of service`

- Service `orcl21pdb1` chua dang ky voi listener.
- Chay:

```sql
ALTER PLUGGABLE DATABASE ORCL21PDB1 OPEN;
ALTER PLUGGABLE DATABASE ORCL21PDB1 SAVE STATE;
```

`ORA-01017: invalid username/password`

- Sai PDB service hoac user chua duoc tao trong PDB.
- Kiem tra:

```sql
ALTER SESSION SET CONTAINER = ORCL21PDB1;
SELECT username, account_status FROM dba_users WHERE username = 'ADMIN_PHANHE1';
```

Backup Data Pump khong tao file:

- Kiem tra `C:\Backup_Oracle` ton tai.
- Kiem tra Oracle directory:

```sql
SELECT directory_name, directory_path
FROM dba_directories
WHERE directory_name = 'BACKUP_DIR';
```

RMAN loi vi khong co archive log:

```sql
ARCHIVE LOG LIST;
```

Neu la `No Archive Mode`, chay lai buoc bat `ARCHIVELOG`.

OLS loi package/policy:

- Dam bao da chay:

```sql
EXEC LBACSYS.CONFIGURE_OLS;
EXEC LBACSYS.OLS_ENFORCEMENT.ENABLE_OLS;
```

- Sau do restart database va mo lai `ORCL21PDB1`.

