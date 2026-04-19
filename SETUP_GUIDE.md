# 🚀 HƯỚNG DẪN COMPILE & CHẠY

## Bước 1: Compile Project trên Visual Studio

1. Mở Visual Studio
2. Mở project `ADMIN.csproj` (tại `ADMIN/ADMIN.csproj`)
3. **Build** → **Build Solution** (hoặc Ctrl+Shift+B)
4. Kiểm tra Output window để xem có lỗi không

### Nếu có lỗi:

#### Lỗi: "Cannot find type or namespace OracleHelper"
- Đảm bảo file `OracleHelper.cs` nằm trong thư mục `ADMIN/`
- Namespace phải là `namespace ADMIN { }`

#### Lỗi: "Cannot find type or namespace PermissionManager"
- Đảm bảo file `PermissionManager.cs` nằm trong thư mục `ADMIN/`
- Namespace phải là `namespace ADMIN { }`

#### Lỗi: "Form1.Designer.cs has syntax errors"
- Mở Form1.Designer.cs
- Tìm dòng có `new Label()` hoặc `new ComboBox()`
- Kiểm tra xem có thiếu initialization hay không
- Xem dòng InitializeComponent() có đúng không

---

## Bước 2: Chuẩn Bị Database (Oracle)

### Chạy Script SQL Setup (nếu chưa chạy):

1. Mở Oracle SQL Developer hoặc SQL*Plus
2. Kết nối bằng tài khoản **SYSTEM** (hoặc DBA)
3. Chạy file SQL này:
   ```
   1. ATHTTT_QLBV/oracle/FINAL.sql
   
   ```

### Kiểm tra Procedure đã được tạo:

```sql
-- Kết nối bằng ADMIN_PHANHE1
SELECT OBJECT_NAME, OBJECT_TYPE 
FROM ALL_OBJECTS 
WHERE OWNER='ADMIN_PHANHE1' AND OBJECT_TYPE='PROCEDURE'
ORDER BY OBJECT_NAME;
```

Phải thấy ít nhất 10 procedure tên bắt đầu bằng `SP_`

---

## Bước 3: Cập Nhật Connection String (nếu cần)

### Kiểm tra file Form1.cs (OracleHelper.AdminConnectionString):

```csharp
public static readonly string AdminConnectionString = 
    @"User Id=ADMIN_PHANHE1;Password=Admin@123456;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl21)));";
```

**Sửa nếu cần**:
- `User Id`: Tài khoản Oracle (ADMIN_PHANHE1)
- `Password`: Mật khẩu
- `HOST`: IP database (127.0.0.1 = localhost)
- `PORT`: Port Oracle (1521 = default)
- `SERVICE_NAME`: orcl21 hoặc tên service của bạn

---

## Bước 4: Chạy Application

1. Mở Visual Studio
2. **Debug** → **Start Without Debugging** (Ctrl+F5)
3. Hoặc Build → nhị phân file `.exe` ở `bin/Debug/net10.0-windows/ADMIN.exe`

---


---

## ✅ Checklist Hoàn Thành

- [ ] Project compile thành công (0 lỗi)
- [ ] Oracle Database hoạt động bình thường
- [ ] Tất cả Procedure đã được tạo
- [ ] Application chạy mà không crash
- [ ] Có thể cấp quyền thành công
- [ ] Có thể thu hồi quyền thành công
- [ ] Có thể tạo quyền thành công
- [ ] Có thể xem thông tin quyền thành công
---

## 🆘 Troubleshooting

### Vấn đề: "Connection timeout"
**Giải pháp**:
- Kiểm tra Oracle service đang chạy: `sqlplus / as sysdba`
- Kiểm tra HOST/PORT/SERVICE_NAME đúng

### Vấn đề: "User ADMIN_PHANHE1 not found"
**Giải pháp**:
- Kiểm tra User tồn tại: `SELECT * FROM DBA_USERS WHERE USERNAME='ADMIN_PHANHE1';`
- Nếu không có, tạo bằng lệnh: `CREATE USER ADMIN_PHANHE1 IDENTIFIED BY Admin@123456;`

### Vấn đề: "Permission denied"
**Giải pháp**:
- ADMIN_PHANHE1 cần quyền DBA
- Chạy: `GRANT DBA TO ADMIN_PHANHE1;`

### Vấn đề: "Procedure SP_GRANT_PRIVILEGE not found"
**Giải pháp**:
- Kiểm tra Procedure tồn tại: 
  ```sql
  SELECT * FROM ALL_OBJECTS 
  WHERE OWNER='ADMIN_PHANHE1' AND OBJECT_NAME='SP_GRANT_PRIVILEGE';
  ```
- Nếu không có, chạy file `SQL_FINAL.sql`

### Vấn đề: "Dropdown không có item"
**Giải pháp**:
- Kiểm tra loadGranteeList() được gọi trong constructor
- Kiểm trace lỗi bằng cách thêm Try-Catch in MessageBox

---

