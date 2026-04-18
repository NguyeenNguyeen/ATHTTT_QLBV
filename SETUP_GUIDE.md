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
3. Chạy các file SQL này **lần lượt**:
   ```
   1. ATHTTT_QLBV/oracle/create_tables.sql
   2. ATHTTT_QLBV/oracle/insert_data.sql
   3. ATHTTT_QLBV/oracle/SQL_FINAL.sql
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

### Kiểm tra file Form1.cs dòng 273 (OracleHelper.AdminConnectionString):

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

## Bước 5: Test Tính Năng GRANT/REVOKE

### 5.1 Test Tab GRANT

```
1. Nhấp Tab "Grant"
2. Grantee: Chọn từ dropdown (ví: C##NV0001)
3. Loại đối tượng: TABLE
4. Tên đối tượng: BENHNHAN
5. Quyền: ☑ SELECT
6. WITH GRANT OPTION: ☐ (không check)
7. Click "Thực thi GRANT"
8. Nếu thành công: MessageBox sẽ hiện "Cấp quyền thành công cho C##NV0001!"
```

### 5.2 Test Tab REVOKE

```
1. Nhấp Tab "Revoke"
2. Grantee: C##NV0001
3. Loại đối tượng: TABLE
4. Tên đối tượng: BENHNHAN
5. Quyền: ☑ SELECT
6. Click "Thực thi REVOKE"
7. Nếu thành công: MessageBox sẽ hiện "Thu hồi quyền thành công từ C##NV0001!"
```

### 5.3 Test Tab "Thông tin quyền"

```
1. Nhấp Tab "Thông tin quyền"
2. Loại quyền: "Xem quyền trên bảng"
3. Mã user/Role (tuỳ chọn): C##NV0001
4. Click "Xem quyền"
5. DataGridView sẽ hiện danh sách quyền
```

---

## ✅ Checklist Hoàn Thành

- [ ] Project compile thành công (0 lỗi)
- [ ] Oracle Database hoạt động bình thường
- [ ] Tất cả Procedure đã được tạo
- [ ] Application chạy mà không crash
- [ ] Tab Grant có thể load danh sách User/Role
- [ ] Tab Grant có thể load Object Name khi chọn Type
- [ ] Tab Grant có thể load Column khi chọn Table
- [ ] Có thể cấp quyền thành công
- [ ] Có thể thu hồi quyền thành công
- [ ] Tab "Thông tin quyền" có thể xem quyền được cấp

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

## 📚 Tài Liệu Tham Khảo

- [HUONG_DAN_PHAN_QUYEN.md](HUONG_DAN_PHAN_QUYEN.md) - Chi tiết tính năng
- [TOMA_TAT_TICH_HOP.md](TOMA_TAT_TICH_HOP.md) - Tóm tắt thay đổi
- [test_phan_quyen.sql](oracle/test_phan_quyen.sql) - SQL test script

---

**Nếu gặp vấn đề, kiểm tra thứ tự này**:
1. ✅ Compile thành công?
2. ✅ Oracle Database hoạt động?
3. ✅ Connection String đúng?
4. ✅ Procedure tồn tại?
5. ✅ User C## tồn tại?
6. ✅ Quyền DBA của ADMIN_PHANHE1?

Nếu vẫn không được, cung cấp error message cho tôi để debug.
