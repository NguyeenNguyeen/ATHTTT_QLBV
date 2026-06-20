@echo off
echo =========================================
echo KHOI PHUC DU LIEU MUC LUAN LY (DATA PUMP IMPDP)
echo =========================================

impdp system/MatKhauCuaSYS directory=BACKUP_DIR dumpfile=BV_PHANHE1.dmp logfile=imp_BV_PHANHE1.log schemas=ADMIN_PHANHE1 TABLE_EXISTS_ACTION=REPLACE

echo KHOI PHUC HOAN TAT!
exit
