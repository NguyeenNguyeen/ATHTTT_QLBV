import os

designer_path = r"c:\Users\Acer\Desktop\ATHTTT_QLBV\ADMIN\AddUserForm.Designer.cs"
cs_path = r"c:\Users\Acer\Desktop\ATHTTT_QLBV\ADMIN\AddUserForm.cs"

# Define mapping
mapping = {
    "Lo?i tài kho?n:": "Loại tài khoản:",
    "M?t kh?u:": "Mật khẩu:",
    "H? tên:": "Họ tên:",
    "Gi?i tính (Phái):": "Giới tính (Phái):",
    "Ngày sinh:": "Ngày sinh:",
    "CMND/CCCD:": "CMND/CCCD:",
    "Quê quán:": "Quê quán:",
    "S? ÐT:": "Số ĐT:",
    "Vai trò:": "Vai trò:",
    "Chuyên khoa:": "Chuyên khoa:",
    "Co s?:": "Cơ sở:",
    "S? nhà:": "Số nhà:",
    "Tên du?ng:": "Tên đường:",
    "Qu?n/huy?n:": "Quận/huyện:",
    "T?nh/TP:": "Tỉnh/TP:",
    "Ti?n s? b?nh:": "Tiền sử bệnh:",
    "TS b?nh gia dình:": "TS bệnh gia đình:",
    "D? ?ng thu?c:": "Dị ứng thuốc:",
    "Luu": "Lưu",
    "H?y": "Hủy",
    "Thêm Ngu?i Dùng": "Thêm Người Dùng",
    "B?nh nhân": "Bệnh nhân"
}

def fix_file(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    for bad, good in mapping.items():
        content = content.replace(bad, good)

    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)

fix_file(designer_path)
fix_file(cs_path)

print("Done fixing encoding issues!")
