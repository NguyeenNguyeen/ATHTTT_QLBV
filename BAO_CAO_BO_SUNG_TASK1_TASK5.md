# Bổ sung báo cáo Task 1 và Task 5

Tài liệu này được viết để bổ sung vào file báo cáo hiện tại `CQ16-Report2026.pdf`. Nội dung bên dưới có thể được chèn trực tiếp vào phần Phân hệ 2, mục 1 và phần cuối mục 5 của báo cáo.

---

## 1. Yêu cầu 1: giải pháp cấp quyền truy cập, cài đặt giao diện - RBAC

### 1.1. Bài toán và giải pháp

**Bài toán.**  
Trong phân hệ quản lý dữ liệu y tế, hai vai trò Kỹ thuật viên và Bệnh nhân có phạm vi truy cập hẹp, ổn định và có thể mô hình hóa bằng quyền tĩnh:

- Kỹ thuật viên chỉ được xem thông tin cá nhân của chính mình, chỉ được cập nhật một số thông tin liên hệ cơ bản và chỉ được xem các dịch vụ cận lâm sàng được phân công cho mình.
- Kỹ thuật viên chỉ được cập nhật kết quả thực hiện dịch vụ, không được thay đổi mã hồ sơ bệnh án, loại dịch vụ, ngày thực hiện hoặc kỹ thuật viên phụ trách.
- Bệnh nhân chỉ được xem hồ sơ cá nhân của chính mình.
- Bệnh nhân chỉ được cập nhật các thông tin được phép tự khai báo hoặc tự hiệu chỉnh như địa chỉ, tiền sử bệnh, tiền sử bệnh gia đình và dị ứng thuốc; không được sửa các thông tin định danh như mã bệnh nhân, họ tên, giới tính, ngày sinh hoặc CCCD.

Vì các quyền này phụ thuộc chủ yếu vào vai trò của người dùng và không cần chính sách động phức tạp theo từng nghiệp vụ điều phối, nhóm lựa chọn cơ chế **Role-Based Access Control (RBAC)** kết hợp với **view có điều kiện lọc theo tài khoản đăng nhập**. RBAC chịu trách nhiệm cấp quyền ở mức đối tượng và mức cột, còn view chịu trách nhiệm giới hạn dữ liệu về đúng chủ thể đang đăng nhập.

**Cơ sở lý thuyết.**  
RBAC là mô hình kiểm soát truy cập trong đó quyền không được cấp trực tiếp rời rạc cho từng người dùng, mà được gom vào các role. Người dùng được gán role phù hợp với chức năng công việc. Khi cần thay đổi chính sách phân quyền, quản trị viên chỉ cần điều chỉnh quyền của role hoặc thay đổi role được gán cho người dùng, giúp hệ thống dễ quản lý hơn so với việc cấp quyền trực tiếp cho từng tài khoản.

Trong Oracle, RBAC được triển khai thông qua các đối tượng `ROLE`, lệnh `GRANT`/`REVOKE`, quyền trên bảng/view/procedure và quyền cập nhật theo cột. Đối với các bài toán "chỉ xem dòng của chính mình", nhóm không cấp quyền trực tiếp lên bảng gốc mà tạo các view lọc dữ liệu bằng `SYS_CONTEXT('USERENV', 'SESSION_USER')`. Nhờ đó, dù câu lệnh SQL ở tầng ứng dụng không ghi điều kiện `WHERE`, Oracle vẫn chỉ trả về dữ liệu thuộc về user đang đăng nhập.

**Giải pháp triển khai.**  
Nhóm tạo hai role chính cho Task 1:

- `ROLE_KYTHUATVIEN`: dành cho kỹ thuật viên.
- `ROLE_BENHNHAN`: dành cho bệnh nhân.

Đối với kỹ thuật viên, hệ thống tạo hai view:

- `V_RBAC_KTV_THONGTIN`: lấy thông tin nhân viên từ bảng `NHANVIEN`, với điều kiện `MANV = SESSION_USER`. View này chỉ cho phép cập nhật các cột `QUEQUAN`, `SODT`, `COSO`.
- `V_RBAC_KTV_DICHVU`: lấy danh sách dịch vụ từ bảng `HSBA_DV`, với điều kiện `MAKTV = SESSION_USER`. View này chỉ cho phép cập nhật cột `KETQUA`.

Đối với bệnh nhân, hệ thống tạo view:

- `V_RBAC_BENHNHAN_THONGTIN`: lấy thông tin từ bảng `BENHNHAN`, với điều kiện `MABN = SESSION_USER`. View này chỉ cho phép cập nhật các cột `SONHA`, `TENDUONG`, `QUANHUYEN`, `TINHTP`, `TIENSUBENH`, `TIENSUBENHGD`, `DIUNGTHUOC`.

Các view đều sử dụng `WITH CHECK OPTION` để ngăn người dùng cập nhật dữ liệu làm dòng dữ liệu thoát khỏi phạm vi được phép nhìn thấy. Đây là lớp bảo vệ bổ sung quan trọng, đặc biệt đối với các view cho phép cập nhật.

Ngoài ra, nhóm xây dựng thủ tục `SP_SYNC_TASK1_RBAC_USERS` để đồng bộ tài khoản Oracle thật từ dữ liệu nghiệp vụ:

- Tạo user Oracle tương ứng với `MANV` của nhân viên và `MABN` của bệnh nhân nếu chưa tồn tại.
- Cấp `CREATE SESSION` để người dùng có thể đăng nhập vào hệ thống.
- Gán `ROLE_KYTHUATVIEN` cho nhân viên có vai trò kỹ thuật viên.
- Gán `ROLE_BENHNHAN` cho các tài khoản bệnh nhân.
- Thu hồi các role không còn phù hợp khi dữ liệu vai trò trong bảng `NHANVIEN` thay đổi.

Cách làm này giúp dữ liệu nghiệp vụ và tài khoản đăng nhập không bị lệch nhau sau khi thêm, sửa vai trò nhân viên hoặc thêm bệnh nhân mới.

### 1.2. Thuyết minh kết quả đạt được

**Cài đặt ở tầng cơ sở dữ liệu.**  
Script chính `CQ2026-CQ16-PH1-Database.sql` đã tạo đầy đủ các role, view và quyền cần thiết cho RBAC. Người dùng cuối không được cấp quyền trực tiếp lên bảng gốc `NHANVIEN`, `BENHNHAN`, `HSBA_DV`, mà thao tác thông qua các view trung gian đã lọc dữ liệu. Điều này đảm bảo nếu người dùng kết nối trực tiếp bằng SQL*Plus hoặc công cụ khác, phạm vi dữ liệu vẫn bị giới hạn ở tầng CSDL.

Bảng phân quyền chính:

| Vai trò | Đối tượng được truy cập | Quyền |
|---|---|---|
| `ROLE_KYTHUATVIEN` | `V_RBAC_KTV_THONGTIN` | `SELECT`, `UPDATE(QUEQUAN, SODT, COSO)` |
| `ROLE_KYTHUATVIEN` | `V_RBAC_KTV_DICHVU` | `SELECT`, `UPDATE(KETQUA)` |
| `ROLE_BENHNHAN` | `V_RBAC_BENHNHAN_THONGTIN` | `SELECT`, `UPDATE(SONHA, TENDUONG, QUANHUYEN, TINHTP, TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC)` |

**Giao diện kỹ thuật viên.**  
Nhóm xây dựng form `FormKyThuatVien` cho người dùng thuộc role kỹ thuật viên. Sau khi đăng nhập, hệ thống truyền connection của chính user Oracle đang đăng nhập vào form, không dùng tài khoản admin để truy vấn thay. Giao diện gồm hai nhóm chức năng:

- Tab thông tin cá nhân: hiển thị thông tin từ `V_RBAC_KTV_THONGTIN`, cho phép tải lại và cập nhật các trường liên hệ được phép.
- Tab dịch vụ được phân công: hiển thị dữ liệu từ `V_RBAC_KTV_DICHVU`, cho phép kỹ thuật viên ghi kết quả thực hiện dịch vụ vào cột `KETQUA`.

Khi kỹ thuật viên cập nhật kết quả, câu lệnh ứng dụng chỉ cập nhật view `V_RBAC_KTV_DICHVU`. Nếu kỹ thuật viên cố sửa các cột ngoài phạm vi như `MAKTV`, `MAHSBA`, `LOAIDV`, Oracle sẽ từ chối do không có quyền trên cột đó.

**Giao diện bệnh nhân.**  
Nhóm xây dựng form `FormBenhNhan` cho người dùng thuộc role bệnh nhân. Form này hiển thị hồ sơ từ `V_RBAC_BENHNHAN_THONGTIN`, cho phép bệnh nhân tải lại và cập nhật các trường được phép như địa chỉ, tiền sử bệnh, tiền sử bệnh gia đình, dị ứng thuốc. Những trường định danh như `MABN`, `TENBN`, `PHAI`, `NGAYSINH`, `CCCD` chỉ được hiển thị, không cấp quyền cập nhật.

**Luồng đăng nhập và phân luồng giao diện.**  
`LoginForm` đăng nhập bằng tài khoản Oracle thật của từng người dùng. Sau khi kết nối thành công, ứng dụng kiểm tra vai trò hiện hành trong session. Nếu tài khoản có `ROLE_KYTHUATVIEN`, hệ thống mở `FormKyThuatVien`; nếu có `ROLE_BENHNHAN`, hệ thống mở `FormBenhNhan`. Như vậy, giao diện được phân tuyến theo role và dữ liệu tiếp tục được bảo vệ bởi quyền Oracle ở tầng dưới.

**Kiểm thử.**  
Nhóm xây dựng kịch bản `oracle/test_task1_rbac.sql` để kiểm chứng độc lập bằng SQL. Kịch bản kiểm thử gồm các nhóm tình huống:

- Kiểm tra các role, view và quyền trên view đã được tạo đúng.
- Tạo dữ liệu tạm cho kỹ thuật viên `NV7777` và một dịch vụ thuộc `HSBA_DV` được phân công cho kỹ thuật viên này.
- Đăng nhập bằng tài khoản kỹ thuật viên, xác nhận chỉ thấy thông tin cá nhân và dịch vụ của chính mình.
- Kiểm tra kỹ thuật viên cập nhật được `KETQUA` và cập nhật được thông tin cá nhân được phép.
- Kiểm tra các thao tác bị cấm như cập nhật `MAKTV` bị Oracle chặn.
- Đăng nhập bằng tài khoản bệnh nhân, xác nhận chỉ thấy hồ sơ của chính mình.
- Kiểm tra bệnh nhân cập nhật được địa chỉ, tiền sử bệnh, dị ứng thuốc.
- Kiểm tra thao tác bị cấm như cập nhật `TENBN` bị Oracle chặn.

Kết quả kiểm thử cho thấy việc kết hợp role, view lọc theo `SESSION_USER`, quyền cập nhật theo cột và `WITH CHECK OPTION` đáp ứng đúng yêu cầu bảo mật của Task 1.

### 1.3. Nhận xét, đánh giá và bài học kinh nghiệm

**Ưu điểm.**

- Chính sách phân quyền rõ ràng, dễ kiểm chứng vì mỗi vai trò chỉ được cấp quyền trên các view chuyên biệt.
- Dữ liệu được bảo vệ ở tầng CSDL, không phụ thuộc hoàn toàn vào kiểm tra ở giao diện.
- Quyền cập nhật theo cột giúp giới hạn chính xác các trường mà người dùng được sửa.
- Việc sử dụng user Oracle thật giúp audit, VPD, FGA và các cơ chế bảo mật khác ghi nhận đúng người thực hiện hành động.
- Thủ tục đồng bộ user/role giúp hạn chế sai lệch giữa dữ liệu nghiệp vụ và tài khoản Oracle.

**Hạn chế.**

- RBAC phù hợp với quyền tương đối tĩnh, nhưng chưa đủ linh hoạt cho các vai trò cần phân quyền theo từng dòng dữ liệu nghiệp vụ phức tạp như bác sĩ hoặc điều phối viên; các trường hợp đó cần VPD.
- Việc ánh xạ tài khoản Oracle theo mã nhân viên/mã bệnh nhân đòi hỏi dữ liệu mã phải chuẩn hóa và không trùng lặp.
- Khi thay đổi vai trò nhân viên, cần chạy lại thủ tục đồng bộ để gán/thu hồi role đúng.

**Bài học kinh nghiệm.**

- Không nên cấp quyền trực tiếp lên bảng gốc cho người dùng cuối nếu chỉ cần cho phép họ truy cập một phần dữ liệu.
- View kết hợp `SYS_CONTEXT('USERENV', 'SESSION_USER')` là cách đơn giản và hiệu quả để hiện thực hóa yêu cầu "chỉ thấy dữ liệu của chính mình".
- Khi view cho phép cập nhật, cần dùng `WITH CHECK OPTION` để tránh cập nhật làm sai phạm vi dữ liệu.
- Cần kiểm thử cả trường hợp hợp lệ và không hợp lệ. Việc chỉ kiểm thử thao tác được phép là chưa đủ để chứng minh chính sách bảo mật hoạt động đúng.

---

## Nội dung bổ sung đúng các chỗ trống ở Yêu cầu 4 và Yêu cầu 5

### Chèn vào 4.2 Thuyết minh kết quả đạt được

**a. Standard auditing:**

Nhóm đã cấu hình Standard Auditing ở mức CSDL bằng tham số `audit_trail = DB, EXTENDED`. Với cấu hình này, Oracle ghi nhận thông tin audit vào bảng hệ thống thay vì chỉ ghi ra hệ điều hành, đồng thời lưu thêm thông tin mở rộng như câu lệnh SQL và biến bind khi có thể. Sau khi bật tham số, database cần được khởi động lại để cấu hình có hiệu lực.

Trong script CSDL, nhóm tạo view `ADMIN_PHANHE1.V_ALL_AUDIT_LOG` để tổng hợp log Standard Auditing với các thông tin cần thiết cho việc theo dõi: loại audit, người dùng thực hiện, thời gian, hành động, đối tượng bị tác động, câu lệnh SQL và trạng thái thực thi. View này giúp phần ứng dụng không cần truy vấn trực tiếp nhiều bảng hệ thống phức tạp, đồng thời tạo một nguồn dữ liệu thống nhất để hiển thị log.

Ở giao diện WinForms, tab `Audit` trong mục `Audit + Backup/Recover` tự động tải log khi người quản trị mở tab lần đầu. Nút `Refresh` cho phép tải lại log mới nhất sau khi thực hiện các thao tác thử nghiệm trên hệ thống. Kết quả đạt được là người quản trị có thể theo dõi các thao tác quan trọng trực tiếp trong ứng dụng, thay vì phải mở SQL Developer để truy vấn thủ công.

### Chèn vào 4.3 Nhận xét, đánh giá và bài học kinh nghiệm

**a. Standard auditing:**

Standard Auditing là cơ chế phù hợp để ghi nhận các thao tác ở mức hệ thống và mức đối tượng, đặc biệt trong các tình huống cần biết ai đã đăng nhập, ai đã truy cập hoặc thay đổi dữ liệu, thao tác xảy ra vào thời điểm nào và tác động lên đối tượng nào. Việc bật `DB, EXTENDED` giúp log có nhiều thông tin hơn, hỗ trợ tốt hơn cho quá trình kiểm tra và truy vết sự cố.

Ưu điểm của cách triển khai là log được lưu tập trung trong CSDL, dễ truy vấn, dễ tổng hợp và có thể đưa lên giao diện quản trị. Khi kết hợp với view `V_ALL_AUDIT_LOG`, người dùng quản trị có một màn hình thống nhất để xem log mà không cần nhớ cấu trúc các bảng audit nội bộ của Oracle.

Hạn chế là Standard Auditing có thể làm tăng dung lượng lưu trữ và phát sinh thêm chi phí ghi log nếu cấu hình quá rộng. Vì vậy, khi triển khai thực tế cần xác định rõ những thao tác nào cần audit, tránh bật quá nhiều chính sách không cần thiết. Ngoài ra, audit chỉ thật sự có giá trị khi hệ thống có quy trình xem log định kỳ, lọc các hành vi bất thường và bảo vệ log khỏi việc chỉnh sửa trái phép.

Bài học rút ra là audit nên được thiết kế ngay từ đầu cùng với cơ chế phân quyền. Phân quyền giúp ngăn chặn truy cập trái phép, còn audit giúp phát hiện và truy vết khi có hành vi đáng ngờ hoặc khi cần chứng minh trách nhiệm của người dùng.

### Chèn vào phần Cơ sở lý thuyết của Yêu cầu 5

**a. RMAN:**

RMAN, viết tắt của Recovery Manager, là công cụ sao lưu và phục hồi vật lý chính thức của Oracle. Khác với Data Pump là phương pháp sao lưu logic theo schema, bảng hoặc dữ liệu xuất nhập, RMAN làm việc ở mức vật lý của database như datafile, control file, SPFILE và archived redo log. Vì vậy, RMAN phù hợp cho các tình huống cần khôi phục toàn bộ database hoặc khôi phục sau sự cố mất file dữ liệu.

RMAN hỗ trợ nhiều dạng sao lưu như full backup, incremental backup, backup archived redo log và backup control file. Khi database chạy ở chế độ `ARCHIVELOG`, RMAN có thể thực hiện backup nóng, tức là sao lưu khi database vẫn đang mở và phục vụ người dùng. Đây là điểm quan trọng đối với các hệ thống cần giảm thời gian dừng dịch vụ.

Trong phục hồi, RMAN sử dụng các backup piece đã tạo cùng với archived redo log để đưa database về trạng thái nhất quán. RMAN cũng cung cấp lệnh `RESTORE DATABASE VALIDATE` để kiểm tra khả năng đọc và sử dụng backup mà chưa cần phục hồi thật, giúp giảm rủi ro trước khi thực hiện restore trên môi trường chính.

### Chèn vào phần Giải pháp của Yêu cầu 5

**a. RMAN:**

Nhóm triển khai RMAN bằng hai file batch đặt trực tiếp trong thư mục `ADMIN`: `run_rman_backup.bat` và `run_rman_restore.bat`. Hai file này được cấu hình trong project WinForms để tự động copy sang thư mục build, nhờ đó khi chạy ứng dụng bằng Visual Studio hoặc `dotnet run`, chương trình có thể gọi script RMAN ngay trong thư mục chạy của ứng dụng.

File `run_rman_backup.bat` tạo một file lệnh RMAN tạm thời, sau đó thực hiện backup toàn bộ database dưới dạng compressed backupset. Script cũng backup archived redo log và current control file, lưu các backup piece vào thư mục `C:\Backup_Oracle`. Mỗi lần chạy, script sinh file log riêng theo timestamp, ví dụ `rman_backup_YYYYMMDD_HHMMSS.log`, giúp người quản trị dễ kiểm tra kết quả.

File `run_rman_restore.bat` dùng để phục hồi database từ các backup piece đã có trong `C:\Backup_Oracle`. Script kiểm tra sự tồn tại của file `.bkp`, catalog các backup piece, đưa database về trạng thái phù hợp, sau đó chạy `RESTORE DATABASE` và `RECOVER DATABASE`. Vì restore là thao tác rủi ro cao, giao diện WinForms hiển thị hộp thoại xác nhận trước khi gọi script restore.

Trên giao diện, nhóm bổ sung hai nút `Backup RMAN` và `Restore RMAN` trong tab `Backup/Restore`. Khi nhấn `Backup RMAN`, ứng dụng gọi `run_rman_backup.bat`. Khi nhấn `Restore RMAN`, ứng dụng yêu cầu xác nhận rồi mới gọi `run_rman_restore.bat`.

### Chèn vào 5.2 Thuyết minh kết quả đạt được

**a. RMAN:**

Chức năng RMAN đã được tích hợp vào ứng dụng WinForms thông qua tab `Audit + Backup/Recover`, mục `Backup/Restore`. Người quản trị có thể chạy backup vật lý bằng nút `Backup RMAN` mà không cần tự tìm và mở script bên ngoài. Script RMAN tạo backup piece trong thư mục `C:\Backup_Oracle`, đồng thời tạo file log để kiểm tra chi tiết quá trình chạy.

Khi kiểm thử, RMAN đã tạo được các file backup cho database, archived redo log và control file. Việc này đáp ứng yêu cầu sao lưu vật lý ở mức toàn hệ thống, bổ sung cho các phương pháp backup logic như Data Pump. Ngoài ra, nhóm đã kiểm tra khả năng sử dụng backup bằng lệnh validate của RMAN, qua đó xác nhận các backup piece có thể đọc được và có khả năng dùng cho phục hồi.

Đối với restore, nhóm đã chuẩn bị script phục hồi và tích hợp nút gọi từ giao diện. Tuy nhiên, do restore thật sẽ can thiệp trực tiếp vào trạng thái database, có thể shutdown database và ghi đè dữ liệu hiện tại, thao tác này được đặt sau bước xác nhận rõ ràng. Cách làm này giúp hạn chế việc người dùng vô tình chạy restore trong lúc hệ thống đang hoạt động bình thường.

### Chèn vào 5.3 Nhận xét, đánh giá và bài học kinh nghiệm

**a. RMAN:**

RMAN là phương pháp phù hợp nhất cho mục tiêu sao lưu và phục hồi vật lý toàn database. So với Data Pump, RMAN không chỉ lưu dữ liệu ở mức schema mà còn bảo vệ các thành phần quan trọng của database như datafile, archived redo log và control file. Vì vậy, RMAN đặc biệt cần thiết trong các tình huống sự cố nghiêm trọng như mất datafile, hỏng database hoặc cần khôi phục toàn hệ thống.

Ưu điểm của giải pháp là script có thể chạy độc lập từ command line hoặc được gọi trực tiếp từ giao diện WinForms. Backup được lưu vào một thư mục cố định, có log theo từng lần chạy, giúp việc kiểm tra và demo dễ hơn. Việc tích hợp nút RMAN vào giao diện cũng giúp người quản trị thao tác nhanh hơn, không cần nhớ chính xác câu lệnh RMAN.

Hạn chế của RMAN là yêu cầu môi trường Oracle phải được chuẩn bị đúng, đặc biệt database nên bật `ARCHIVELOG` nếu muốn backup nóng và phục hồi nhất quán. Ngoài ra, restore RMAN là thao tác nhạy cảm vì có thể làm database tạm dừng hoặc ghi đè trạng thái hiện tại, nên không nên cho chạy một cách tự động mà không có xác nhận.

Bài học kinh nghiệm là backup và restore cần được kiểm thử định kỳ, không chỉ dừng ở việc tạo file backup. Một bản backup chỉ thật sự có giá trị khi đã được kiểm tra khả năng phục hồi. Bên cạnh đó, cần lưu cả control file và archived redo log để tăng khả năng khôi phục sau sự cố.
