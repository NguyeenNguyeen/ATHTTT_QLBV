# Kịch bản demo KTV - BN - RBAC

## 1. Mục tiêu demo

Kịch bản này dùng để demo Task 1: RBAC cho hai vai trò:

- Kỹ thuật viên: chỉ xem thông tin cá nhân của mình, chỉ xem dịch vụ được phân công, chỉ cập nhật kết quả dịch vụ và một số thông tin liên hệ được phép.
- Bệnh nhân: chỉ xem hồ sơ cá nhân của mình, chỉ cập nhật địa chỉ, tiền sử bệnh, tiền sử bệnh gia đình và dị ứng thuốc.

Ý chính cần nói khi demo:

> Giao diện chỉ phân luồng người dùng vào đúng form. Bảo mật thật sự nằm ở Oracle: mỗi người dùng đăng nhập bằng user Oracle riêng, được gán role riêng, và chỉ có quyền trên các view đã lọc theo `SYS_CONTEXT('USERENV', 'SESSION_USER')`.

## 2. Chuẩn bị trước khi demo

Đăng nhập admin vào PDB:

```sql
CONNECT ADMIN_PHANHE1/"Admin@123456"@//localhost:1521/ORCLPDB1
```

Nếu máy demo dùng service khác, thay `ORCLPDB1` bằng service PDB đang dùng, ví dụ `ORCL21PDB1`.

Đồng bộ lại user/role RBAC:

```sql
EXEC ADMIN_PHANHE1.SP_SYNC_TASK1_RBAC_USERS('123456');
```

Tìm tài khoản có sẵn để demo:

```sql
SELECT grantee, granted_role
FROM dba_role_privs
WHERE granted_role IN ('ROLE_KYTHUATVIEN', 'ROLE_BENHNHAN')
ORDER BY granted_role, grantee;
```

Password mặc định:

```text
123456
```

Lưu ý: với script chính hiện tại, nên đăng nhập bằng mã user đang hiện trong Oracle, thường là `NVxxxx` và `BNxxxx`. Nếu kết quả query hiện `C##NVxxxx` hoặc `C##BNxxxx` thì dùng đúng tên đó để đăng nhập.

## 3. Giải thích nhanh cơ chế RBAC

Hệ thống tạo hai role:

```sql
ROLE_KYTHUATVIEN
ROLE_BENHNHAN
```

Kỹ thuật viên không được cấp quyền trực tiếp trên bảng gốc `NHANVIEN` và `HSBA_DV`, mà chỉ thao tác qua hai view:

```sql
V_RBAC_KTV_THONGTIN
V_RBAC_KTV_DICHVU
```

Bệnh nhân không được cấp quyền trực tiếp trên bảng gốc `BENHNHAN`, mà chỉ thao tác qua view:

```sql
V_RBAC_BENHNHAN_THONGTIN
```

Các view lọc dữ liệu theo user đang đăng nhập:

```sql
WHERE UPPER(MANV) = SYS_CONTEXT('USERENV', 'SESSION_USER')
WHERE UPPER(MAKTV) = SYS_CONTEXT('USERENV', 'SESSION_USER')
WHERE UPPER(MABN) = SYS_CONTEXT('USERENV', 'SESSION_USER')
```

Giải thích khi thuyết trình:

> `SESSION_USER` là user Oracle đang đăng nhập. Vì vậy nếu đăng nhập bằng `NV0005`, view kỹ thuật viên chỉ trả về dòng có `MANV = NV0005` hoặc `MAKTV = NV0005`. Nếu đăng nhập bằng `BN0001`, view bệnh nhân chỉ trả về dòng có `MABN = BN0001`.

Các view có `WITH CHECK OPTION`, nên người dùng không thể cập nhật làm dòng dữ liệu thoát khỏi phạm vi mình được thấy.

## 4. Demo bằng giao diện: Kỹ thuật viên

### Bước 1: Đăng nhập

Mở app WinForms và đăng nhập bằng tài khoản kỹ thuật viên:

```text
Username: NVxxxx
Password: 123456
```

Nếu database hiện user có tiền tố `C##`, đăng nhập theo đúng user đó:

```text
Username: C##NVxxxx
Password: 123456
```

Kết quả mong đợi: app tự mở màn hình `FormKyThuatVien`.

Nói khi demo:

> Sau khi login thành công, app đọc role hiện hành trong session. Nếu user có `ROLE_KYTHUATVIEN`, chương trình điều hướng sang form kỹ thuật viên. Từ lúc này các truy vấn chạy bằng connection của chính user kỹ thuật viên, không phải connection admin.

### Bước 2: Xem thông tin cá nhân

Mở tab `THÔNG TIN CÁ NHÂN`.

Kết quả mong đợi:

- Chỉ hiển thị thông tin của chính kỹ thuật viên đang đăng nhập.
- Không thấy thông tin của nhân viên khác.

Nói khi demo:

> Dữ liệu này lấy từ `V_RBAC_KTV_THONGTIN`. View đã lọc bằng `MANV = SESSION_USER`, nên kỹ thuật viên chỉ thấy chính mình.

### Bước 3: Cập nhật thông tin được phép

Bấm `Cập nhật thông tin`, sửa một trong các trường:

- `QUEQUAN`
- `SODT`
- `COSO`

Lưu lại và bấm `Tải lại`.

Kết quả mong đợi: cập nhật thành công.

Nói khi demo:

> Role kỹ thuật viên chỉ được `UPDATE` các cột liên hệ cơ bản. Các cột định danh như `MANV`, `HOTEN`, `VAITRO` không được cấp quyền sửa.

### Bước 4: Xem dịch vụ được phân công

Mở tab `DỊCH VỤ ĐƯỢC PHÂN CÔNG`.

Kết quả mong đợi:

- Chỉ thấy dịch vụ có `MAKTV` bằng user kỹ thuật viên đang đăng nhập.
- Không thấy dịch vụ của kỹ thuật viên khác.

Nói khi demo:

> Dữ liệu này lấy từ `V_RBAC_KTV_DICHVU`. View lọc bằng `MAKTV = SESSION_USER`, nên mỗi kỹ thuật viên chỉ nhìn thấy việc được giao cho mình.

### Bước 5: Ghi kết quả dịch vụ

Chọn một dòng dịch vụ, bấm `Ghi kết quả`, nhập kết quả và lưu.

Kết quả mong đợi: cập nhật thành công cột `KETQUA`.

Nói khi demo:

> Kỹ thuật viên chỉ được cập nhật cột `KETQUA`. Các cột như `MAHSBA`, `LOAIDV`, `NGAYDV`, `MAKTV` không được cấp quyền cập nhật, nên không thể tự đổi dịch vụ sang người khác hoặc sửa nội dung phân công.

## 5. Demo bằng giao diện: Bệnh nhân

### Bước 1: Đăng xuất và đăng nhập bệnh nhân

Bấm `Đăng xuất`, sau đó đăng nhập bằng tài khoản bệnh nhân:

```text
Username: BNxxxx
Password: 123456
```

Nếu database hiện user có tiền tố `C##`, đăng nhập theo đúng user đó:

```text
Username: C##BNxxxx
Password: 123456
```

Kết quả mong đợi: app tự mở màn hình `FormBenhNhan`.

Nói khi demo:

> App tiếp tục phân luồng theo role trong Oracle. User có `ROLE_BENHNHAN` sẽ vào form bệnh nhân.

### Bước 2: Xem hồ sơ cá nhân

Quan sát bảng thông tin bệnh nhân.

Kết quả mong đợi:

- Chỉ hiển thị một hồ sơ của chính bệnh nhân đang đăng nhập.
- Không thấy danh sách tất cả bệnh nhân.

Nói khi demo:

> Dữ liệu lấy từ `V_RBAC_BENHNHAN_THONGTIN`. View lọc bằng `MABN = SESSION_USER`, nên bệnh nhân chỉ thấy hồ sơ của chính mình.

### Bước 3: Cập nhật thông tin được phép

Bấm `Cập nhật thông tin`, sửa các trường được phép:

- `SONHA`
- `TENDUONG`
- `QUANHUYEN`
- `TINHTP`
- `TIENSUBENH`
- `TIENSUBENHGD`
- `DIUNGTHUOC`

Lưu lại và bấm `Tải lại`.

Kết quả mong đợi: cập nhật thành công.

Nói khi demo:

> Bệnh nhân chỉ được sửa các thông tin tự khai báo như địa chỉ, tiền sử bệnh và dị ứng thuốc. Các trường định danh như `MABN`, `TENBN`, `PHAI`, `NGAYSINH`, `CCCD` không được cấp quyền sửa.

## 6. Demo bổ sung bằng SQL

Nếu muốn chứng minh bảo mật nằm ở tầng database, chạy trực tiếp SQL bằng user thường.

### 6.1. Test kỹ thuật viên

```sql
CONNECT NVxxxx/"123456"@//localhost:1521/ORCLPDB1

SELECT USER FROM dual;

SELECT role
FROM session_roles
WHERE role = 'ROLE_KYTHUATVIEN';

SELECT *
FROM ADMIN_PHANHE1.V_RBAC_KTV_THONGTIN;

SELECT *
FROM ADMIN_PHANHE1.V_RBAC_KTV_DICHVU;
```

Cập nhật hợp lệ:

```sql
UPDATE ADMIN_PHANHE1.V_RBAC_KTV_DICHVU
SET KETQUA = N'Da thuc hien demo RBAC';
```

Lệnh phải bị chặn:

```sql
UPDATE ADMIN_PHANHE1.V_RBAC_KTV_DICHVU
SET MAKTV = 'NV0001';
```

Giải thích:

> Lệnh sửa `KETQUA` thành công vì role kỹ thuật viên được cấp `UPDATE(KETQUA)`. Lệnh sửa `MAKTV` bị chặn vì role không có quyền update cột này.

### 6.2. Test bệnh nhân

```sql
CONNECT BNxxxx/"123456"@//localhost:1521/ORCLPDB1

SELECT USER FROM dual;

SELECT role
FROM session_roles
WHERE role = 'ROLE_BENHNHAN';

SELECT *
FROM ADMIN_PHANHE1.V_RBAC_BENHNHAN_THONGTIN;
```

Cập nhật hợp lệ:

```sql
UPDATE ADMIN_PHANHE1.V_RBAC_BENHNHAN_THONGTIN
SET SONHA = N'999',
    TENDUONG = N'Duong demo RBAC',
    TIENSUBENH = N'Demo cap nhat tien su benh';
```

Lệnh phải bị chặn:

```sql
UPDATE ADMIN_PHANHE1.V_RBAC_BENHNHAN_THONGTIN
SET TENBN = N'Ten bi sua trai phep';
```

Giải thích:

> Bệnh nhân được cấp quyền update các cột thông tin tự khai báo, nhưng không được update `TENBN`, nên Oracle từ chối.

Có thể rollback sau khi test SQL:

```sql
ROLLBACK;
```

## 7. Câu nối kết khi demo

Có thể kết thúc phần demo bằng đoạn sau:

> Qua demo, có thể thấy RBAC không chỉ được xử lý ở giao diện. Giao diện chỉ giúp người dùng vào đúng màn hình theo role. Chính sách bảo mật nằm ở Oracle thông qua role, view lọc theo `SESSION_USER`, quyền cập nhật theo cột và `WITH CHECK OPTION`. Vì vậy, dù người dùng thao tác bằng WinForms hay kết nối trực tiếp bằng SQL, kỹ thuật viên và bệnh nhân vẫn chỉ thấy và chỉ sửa dữ liệu đúng với phạm vi được cấp.

## 8. Checklist demo nhanh

- Đăng nhập kỹ thuật viên thành công.
- Kỹ thuật viên chỉ thấy thông tin cá nhân của mình.
- Kỹ thuật viên chỉ thấy dịch vụ được phân công.
- Kỹ thuật viên cập nhật được `KETQUA`.
- Kỹ thuật viên không sửa được `MAKTV`.
- Đăng xuất và đăng nhập bệnh nhân thành công.
- Bệnh nhân chỉ thấy hồ sơ của mình.
- Bệnh nhân cập nhật được địa chỉ/tiền sử/dị ứng.
- Bệnh nhân không sửa được `TENBN`.
