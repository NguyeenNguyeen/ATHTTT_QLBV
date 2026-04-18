# HƯỚNG DẪN TÍCH HỢP PHÂN QUYỀN TRONG WINFORM

## 📋 Tổng Quan

Hệ thống đã được tích hợp các tính năng phân quyền từ Oracle Procedure vào WinForm với 4 tabs chính:
- **User**: Quản lý User/Nhân viên & Bệnh nhân
- **Role**: Quản lý Roles (đang phát triển)
- **Grant**: Cấp quyền
- **Revoke**: Thu hồi quyền
- **Thông tin quyền**: Xem quyền của User/Role

---

## 🔐 Tab GRANT - Cấp Quyền

### Các thành phần UI:
1. **Grantee (User/Role)**: ComboBox chứa danh sách User (C## prefix)
2. **Loại đối tượng**: ComboBox với 4 loại
   - TABLE
   - VIEW
   - PROCEDURE
   - FUNCTION
3. **Tên đối tượng**: ComboBox dynamic (tự động load theo loại đối tượng)
4. **Quyền**: CheckedListBox
   - Nếu chọn PROCEDURE/FUNCTION: chỉ có EXECUTE
   - Nếu chọn TABLE/VIEW: SELECT, INSERT, UPDATE, DELETE
5. **Cột**: CheckedListBox (chỉ hiện với SELECT/UPDATE)
6. **WITH GRANT OPTION**: Checkbox cho phép User này cấp quyền cho người khác

### Cách sử dụng:
```
1. Chọn User/Role từ Grantee
2. Chọn Loại đối tượng (TABLE/VIEW/PROCEDURE/FUNCTION)
3. Chọn Tên đối tượng (tự load)
4. Chọn Quyền (có thể chọn nhiều)
5. (Tuỳ chọn) Chọn cột cụ thể nếu muốn phân quyền mức cột
6. (Tuỳ chọn) Check "WITH GRANT OPTION" nếu muốn User này có quyền cấp quyền
7. Click "Thực thi GRANT"
```

### Procedure được gọi:
- `ADMIN_PHANHE1.SP_GRANT_PRIVILEGE`
  - p_GRANTEE: User/Role nhận quyền
  - p_PRIVILEGE: SELECT|INSERT|UPDATE|DELETE|EXECUTE
  - p_OBJECT_NAME: Tên bảng/view/procedure/function
  - p_COLUMNS: Danh sách cột (nếu có)
  - p_GRANT_OPTION: 1=có, 0=không

---

## 🚫 Tab REVOKE - Thu Hồi Quyền

### Giao diện tương tự Grant nhưng:
- Không có "WITH GRANT OPTION"
- Chỉ cần chọn quyền cần thu hồi (không cần chọn cột)

### Cách sử dụng:
```
1. Chọn User/Role từ Grantee
2. Chọn Loại đối tượng
3. Chọn Tên đối tượng
4. Chọn Quyền cần thu hồi
5. Click "Thực thi REVOKE"
```

### Procedure được gọi:
- `ADMIN_PHANHE1.SP_REVOKE_PRIVILEGE`
  - p_GRANTEE: User/Role mất quyền
  - p_PRIVILEGE: Quyền cần thu hồi
  - p_OBJECT_NAME: Tên đối tượng

---

## 📊 Tab "Thông tin quyền" - Xem Chi Tiết Quyền

### 4 loại quyền có thể xem:
1. **Xem quyền trên bảng**: SELECT từ DBA_TAB_PRIVS
2. **Xem quyền trên cột**: SELECT từ DBA_COL_PRIVS
3. **Xem quyền trên view**: SELECT từ DBA_TAB_PRIVS (VIEW)
4. **Xem quyền trên procedure/function**: SELECT từ DBA_TAB_PRIVS (PROC/FUNC)

### Cách sử dụng:
```
1. Chọn Loại quyền cần xem
2. (Tuỳ chọn) Nhập mã User/Role để lọc
3. Click "Xem quyền"
4. DataGridView hiển thị kết quả
```

---

## 🛠️ Classes Helper

### 1. OracleHelper.cs
Static class chứa các method helper:
- `ExecuteStoredProcedure()`: Thực thi Procedure
- `ExecuteQuery()`: Thực thi Query
- `ExecuteStoredProcedureWithCursor()`: Thực thi Procedure với RefCursor output

### 2. PermissionManager.cs
Class quản lý quyền:
- `GrantPrivilege()`: Cấp quyền
- `RevokePrivilege()`: Thu hồi quyền
- `GrantRoleToUser()`: Cấp ROLE cho User
- `GetAllGrantees()`: Lấy danh sách User/Role
- `GetObjectNamesByType()`: Lấy danh sách đối tượng
- `GetColumnsOfTable()`: Lấy cột của bảng
- `GetTablePrivileges()`: Xem quyền bảng
- Và các method khác

### 3. Form1.cs
Main form chứa logic UI:
- `LoadGranteeList()`: Load danh sách User/Role
- `LoadObjectNames()`: Load danh sách đối tượng theo type
- `LoadColumns()`: Load danh sách cột
- `cbGrantObjectType_SelectedIndexChanged()`: Event khi chọn Object Type
- `cbGrantObjectName_SelectedIndexChanged()`: Event khi chọn Object Name
- `cbRevokeObjectType_SelectedIndexChanged()`: Event Revoke
- `cbRevokeObjectName_SelectedIndexChanged()`: Event Revoke
- `btnGrantExecute_Click()`: Xử lý GRANT
- `btnRevokeExecute_Click()`: Xử lý REVOKE

---

## ⚠️ Lưu Ý Quan Trọng

1. **User**: Phải có prefix `C##` (ví: C##NV0001, C##BN0005)
2. **Quyền Column-level**: Chỉ áp dụng cho SELECT & UPDATE
3. **WITH GRANT OPTION**: Chỉ dành cho USER, không áp dụng cho ROLE
4. **Connection**: Sử dụng tài khoản `ADMIN_PHANHE1` để kết nối
5. **Error Handling**: Tất cả Exception đều hiển thị MessageBox

---

## 📝 Ví Dụ Sử Dụng

### Cấp quyền SELECT trên bảng NHANVIEN cho User C##NV0001:
```
1. Grantee: C##NV0001
2. Loại đối tượng: TABLE
3. Tên đối tượng: NHANVIEN
4. Quyền: ☑ SELECT
5. Cột: (để trống = tất cả cột)
6. WITH GRANT OPTION: ☐ (không check)
7. Click "Thực thi GRANT"
```

### Cấp quyền SELECT chỉ cột MANV, HOTEN cho User C##BN0001:
```
1. Grantee: C##BN0001
2. Loại đối tượng: TABLE
3. Tên đối tượng: NHANVIEN
4. Quyền: ☑ SELECT
5. Cột: ☑ MANV, ☑ HOTEN
6. WITH GRANT OPTION: ☐
7. Click "Thực thi GRANT"
```

### Cấp quyền EXECUTE procedure SP_TAO_BENHNHAN cho C##NV0001:
```
1. Grantee: C##NV0001
2. Loại đối tượng: PROCEDURE
3. Tên đối tượng: SP_TAO_BENHNHAN
4. Quyền: ☑ EXECUTE
5. Click "Thực thi GRANT"
```

---

## 🔗 Procedure Oracle Được Sử Dụng

```sql
-- Cấp quyền
ADMIN_PHANHE1.SP_GRANT_PRIVILEGE

-- Thu hồi quyền
ADMIN_PHANHE1.SP_REVOKE_PRIVILEGE

-- Cấp ROLE
ADMIN_PHANHE1.SP_GRANT_ROLE_TO_USER

-- Xem quyền
ADMIN_PHANHE1.SP_XEM_QUYEN_ALL_USER
ADMIN_PHANHE1.SP_XEM_QUYEN_COT_ALL_USER
ADMIN_PHANHE1.SP_XEM_QUYEN_VIEW_ALL_USER
ADMIN_PHANHE1.SP_XEM_QUYEN_PROC_ALL_USER
```

---

## 🎯 Roadmap Tương Lai

- [ ] Tab Role: Thêm/Sửa/Xóa Role
- [ ] Validation tốt hơn cho User/Role name
- [ ] Logging tất cả thao tác phân quyền
- [ ] Batch Grant/Revoke cho nhiều User
- [ ] Report quyền chi tiết
- [ ] Backup/Restore quyền

---

**Phiên bản**: 1.0  
**Ngày cập nhật**: 2026-04-18  
**Tác giả**: Copilot
