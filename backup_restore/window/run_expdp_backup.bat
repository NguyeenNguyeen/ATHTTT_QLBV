@echo off
echo =========================================
echo BAT DAU SAO LUU DU LIEU (DATA PUMP EXPDP)
echo =========================================

:: Tham số REUSE_DUMPFILES=YES giúp tự động ghi đè file cũ mà không báo lỗi
expdp system/MatKhauCuaSYS directory=BACKUP_DIR dumpfile=BV_PHANHE1.dmp logfile=BV_PHANHE1.log schemas=ADMIN_PHANHE1 REUSE_DUMPFILES=YES

echo SAO LUU HOAN TAT!
exit
