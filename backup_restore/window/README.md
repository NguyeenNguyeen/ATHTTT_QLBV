# Copy 2 file này để cùng thư mục với file chạy ADMIN.exe của đồ án.
(hoặc để trong thư mục thích hợp và chỉnh sửa lại đường dẫn đến files ở dưới)

# Ở nút [Sao lưu RMAN], viết code C\#:
System.Diagnostics.Process.Start("run_rman_backup.bat");

# Ở nút [Khôi phục thảm họa], viết code C\#:
System.Diagnostics.Process.Start("run_rman_restore.bat");
