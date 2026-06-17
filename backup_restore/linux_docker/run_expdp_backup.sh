#!/bin/bash
CONTAINER_NAME="oracle-xe"
DB_USER="system"
DB_PASS="MatKhauCuaSYS" # Đổi thành pass của user system

echo "========================================="
echo "BAT DAU SAO LUU DU LIEU (DATA PUMP EXPDP)"
echo "========================================="

# Xóa file cũ để tránh lỗi "File already exists" của Data Pump
docker exec -i $CONTAINER_NAME rm -f /backup/BV_PHANHE1.dmp
docker exec -i $CONTAINER_NAME rm -f /backup/BV_PHANHE1.log

# Thực thi expdp xuất dữ liệu của toàn bộ lược đồ ADMIN_PHANHE1
docker exec -i $CONTAINER_NAME expdp $DB_USER/$DB_PASS directory=BACKUP_DIR dumpfile=BV_PHANHE1.dmp logfile=BV_PHANHE1.log schemas=ADMIN_PHANHE1

echo "========================================="
echo "SAO LUU DATA PUMP HOAN TAT!"
echo "========================================="
