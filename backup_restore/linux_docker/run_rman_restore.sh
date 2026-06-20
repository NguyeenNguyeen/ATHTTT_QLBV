#!/bin/bash
CONTAINER_NAME="oracle-xe"

echo "========================================="
echo "KHOI PHUC HE THONG SAU THAM HOA (RMAN)"
echo "========================================="

echo "1. Dang tat Database va dua ve trang thai MOUNT..."
docker exec -i $CONTAINER_NAME sqlplus / as sysdba <<EOF
SHUTDOWN ABORT;
STARTUP MOUNT;
EXIT;
EOF

echo "2. Dang tien hanh khoi phuc du lieu (Restore & Recover)..."
docker exec -i $CONTAINER_NAME rman target / <<EOF
RUN {
    RESTORE DATABASE;
    RECOVER DATABASE;
}
EOF

echo "3. Mo lai Database cho nguoi dung..."
docker exec -i $CONTAINER_NAME sqlplus / as sysdba <<EOF
ALTER DATABASE OPEN;
EXIT;
EOF

echo "========================================="
echo "KHOI PHUC HOAN TAT!"
echo "========================================="
