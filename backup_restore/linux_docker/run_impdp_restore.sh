#!/bin/bash
CONTAINER_NAME="oracle-xe"
DB_USER="system"
DB_PASS="MatkhaucuaSYS"

echo "========================================="
echo "KHOI PHUC DU LIEU MUC LUAN LY (DATA PUMP IMPDP)"
echo "========================================="

# Thực thi khôi phục. 
# Có thể khôi phục 1 bảng cụ thể bằng cách thêm tham số: tables=ADMIN_PHANHE1.DONTHUOC
docker exec -i $CONTAINER_NAME impdp $DB_USER/$DB_PASS directory=BACKUP_DIR dumpfile=BV_PHANHE1.dmp logfile=imp_BV_PHANHE1.log schemas=ADMIN_PHANHE1 TABLE_EXISTS_ACTION=REPLACE

echo "========================================="
echo "KHOI PHUC HOAN TAT!"
echo "========================================="
