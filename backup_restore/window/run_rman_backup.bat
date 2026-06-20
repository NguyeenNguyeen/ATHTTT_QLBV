@echo off
echo =========================================
echo BAT DAU SAO LUU HE THONG (RMAN HOT BACKUP)
echo =========================================

:: Tao thu muc neu chua co tren may Windows (ĐƯỜNG DẪN NÀY LÀ ĐƯỜNG DẪN ĐƯỢC TẠO TRONG SCRIPT CREATE OR REPLACE DIRECTORY...)
if not exist "C:\Backup_Oracle" mkdir "C:\Backup_Oracle"

:: 1. Tao file RMAN tam thoi
echo RUN { > rman_backup_temp.rman
:: Luu y: Phai dung %%U de Windows khong hieu nham la bien moi truong
echo BACKUP AS COMPRESSED BACKUPSET FORMAT 'C:\Backup_Oracle\DB_FULL_%%U.bkp' DATABASE PLUS ARCHIVELOG; >> rman_backup_temp.rman
echo DELETE NOPROMPT OBSOLETE RECOVERY WINDOW OF 7 DAYS; >> rman_backup_temp.rman
echo } >> rman_backup_temp.rman

:: 2. Thuc thi RMAN va ghi log ra file de UI co the doc neu can
echo Dang chay RMAN, vui long cho...
rman target / cmdfile="rman_backup_temp.rman" log="rman_backup.log"

:: 3. Don dep file tam thoi
del rman_backup_temp.rman

echo =========================================
echo SAO LUU RMAN HOAN TAT! FILE NAM TAI C:\Backup_Oracle
echo =========================================
exit
