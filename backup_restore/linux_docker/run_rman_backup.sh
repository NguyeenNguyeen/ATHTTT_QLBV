#!/bin/bash
CONTAINER_NAME="oracle-xe"

echo "========================================="
echo "BAT DAU SAO LUU HE THONG (RMAN HOT BACKUP)"
echo "========================================="

# Đẩy lệnh RMAN vào thẳng container
docker exec -i $CONTAINER_NAME rman target / <<EOF
RUN {
    # Nén và lưu file backup vào thư mục /backup
    BACKUP AS COMPRESSED BACKUPSET FORMAT '/backup/DB_FULL_%U.bkp' DATABASE PLUS ARCHIVELOG;
    
    # Dọn dẹp các bản backup cũ hơn 7 ngày
    DELETE NOPROMPT OBSOLETE RECOVERY WINDOW OF 7 DAYS;
}
EOF

echo "========================================="
echo "SAO LUU RMAN HOAN TAT!"
echo "========================================="
