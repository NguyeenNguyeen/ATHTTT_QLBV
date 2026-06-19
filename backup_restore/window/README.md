# RMAN backup & restore (OS-based): 2 nút

## Copy 2 file này để cùng thư mục với file chạy ADMIN.exe của đồ án.
(hoặc để trong thư mục thích hợp và chỉnh sửa lại đường dẫn đến files ở dưới)

## Ở nút [Sao lưu RMAN], viết code C\#:
System.Diagnostics.Process.Start("run_rman_backup.bat");

## Ở nút [Khôi phục thảm họa], viết code C\#:
System.Diagnostics.Process.Start("run_rman_restore.bat");



# Data pump backup & restore (script): 2 nút
Các file .bat cho loại này chỉ để tham khảo thôi, ti đã chuyển sang script sql ở trong file chung rồi.
- dùng procedure ADMIN_PHANHE1.SP_BACKUP_DATAPUMP: không tham số (nó sẽ chạy khoảng > 45s tuỳ thuộc vào độ lớn dữ liệu)
- dùng procedure ADMIN_PHANHE1.SP_RESTORE_DATAPUMP(p_table_name: VARCHAR2 default NULL): 
    + nếu không truyền tham số: -> mặc định restore toàn bộ dữ liệu.
    + nếu truyền tham số thì chỉ restore 1 bảng đó thôi: không khuyến nghị dùng lắm vì các bảng sẽ có các constraints các thứ, cần restore nhiều bảng để restore cả các ràng buộc đó.


# Flashback backup & restore (script): 1 nút - restore dựa trên audit log
Script nằm trong file chung.
- Cách tui dùng ở đây: dựa vào audit log để biết được thời điểm dữ liệu bị sửa sai để mình restore lại, dùng flashbask archive của oracle để lưu lại dữ liệu cũ trước khi bị sửa thay vì tự tạo 1 bảng để lưu dữ liệu cũ đó.
- dùng procedure ADMIN_PHANHE1.SP_RESTORE_FLASHBACK(p_table_name: VARCHAR2, p_safe_time: VARCHAR2-format: YYYY-MM-DD HH24:MI:SS) để restore lại dữ liệu của bảng p_table_name quay lại thời điểm p_safe_time.
