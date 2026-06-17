@echo off
echo =========================================
echo KHOI PHUC HE THONG SAU THAM HOA (RMAN)
echo =========================================

:: 1. Tao cac file nhap tam thoi (SQL va RMAN)
echo SHUTDOWN ABORT; > prep_temp.sql
echo STARTUP MOUNT; >> prep_temp.sql
echo EXIT; >> prep_temp.sql

echo RUN { > rman_restore_temp.rman
echo RESTORE DATABASE; >> rman_restore_temp.rman
echo RECOVER DATABASE; >> rman_restore_temp.rman
echo } >> rman_restore_temp.rman

echo ALTER DATABASE OPEN; > open_temp.sql
echo EXIT; >> open_temp.sql

:: 2. Thuc thi tat DB va dua ve MOUNT
echo 1/3: Dang tat Database va dua ve trang thai MOUNT...
sqlplus / as sysdba @prep_temp.sql

:: 3. Thuc thi RMAN khoi phuc
echo 2/3: Dang tien hanh khoi phuc du lieu tu file Backup...
rman target / cmdfile="rman_restore_temp.rman" log="rman_restore.log"

:: 4. Thuc thi mo lai DB
echo 3/3: Mo lai Database cho nguoi dung...
sqlplus / as sysdba @open_temp.sql

:: 5. Don dep tat ca cac file tam
del prep_temp.sql
del rman_restore_temp.rman
del open_temp.sql

echo =========================================
echo KHOI PHUC HOAN TAT!
echo =========================================
exit
