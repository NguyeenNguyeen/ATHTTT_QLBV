
-- ========================================================
-- PHẦN 3: CHÈN DỮ LIỆU MẪU (DATA MOCKING)
-- ========================================================

-- KHOA
INSERT INTO KHOA VALUES ('K01', N'Khoa Tim mạch');
INSERT INTO KHOA VALUES ('K02', N'Khoa Thần kinh');
INSERT INTO KHOA VALUES ('K03', N'Khoa Tiêu hóa');

-- NHANVIEN
-- Giám đốc & Lãnh đạo
INSERT INTO NHANVIEN VALUES ('NV001', N'Nguyễn Văn Nam', N'Nam', TO_DATE('01-01-70', 'DD-MM-YY'), '079170000001', N'TP.HCM', '0901000001', N'Giám đốc', N'Quản lý chung', 'HCM');
INSERT INTO NHANVIEN VALUES ('NV002', N'Trần Phúc Mạnh', N'Nam', TO_DATE('15-05-75', 'DD-MM-YY'), '079175000002', N'TP.HCM', '0901000002', N'Lãnh đạo khoa', N'Tim mạch', 'HCM');
INSERT INTO NHANVIEN VALUES ('NV003', N'Lê Phương Thanh', N'Nữ', TO_DATE('20-08-80', 'DD-MM-YY'), '079180000003', N'Hà Nội', '0901000003', N'Lãnh đạo khoa', N'Thần kinh', 'HN');

-- Bác sĩ/Y tá
INSERT INTO NHANVIEN VALUES ('NV004', N'Phạm Nhật Minh', N'Nam', TO_DATE('10-10-85', 'DD-MM-YY'), '079185000004', N'Đà Nẵng', '0901000004', N'Bác sĩ/Y sĩ', N'Tim mạch', 'HCM');
INSERT INTO NHANVIEN VALUES ('NV005', N'Hoàng Thị Tuyết', N'Nữ', TO_DATE('05-12-88', 'DD-MM-YY'), '079188000005', N'Cần Thơ', '0901000005', N'Bác sĩ/Y sĩ', N'Thần kinh', 'HCM');
INSERT INTO NHANVIEN VALUES ('NV006', N'Bùi Quốc Huy', N'Nam', TO_DATE('14-02-87', 'DD-MM-YY'), '079187000006', N'Hải Phòng', '0901000006', N'Bác sĩ/Y sĩ', N'Tiêu hóa', 'HP');

-- Kỹ thuật viên & Điều phối viên
INSERT INTO NHANVIEN VALUES ('NV007', N'Đặng Trọng Trân', N'Nam', TO_DATE('25-03-90', 'DD-MM-YY'), '079190000007', N'TP.HCM', '0901000007', N'Kỹ thuật viên', N'Chẩn đoán hình ảnh', 'HCM');
INSERT INTO NHANVIEN VALUES ('NV008', N'Vũ Như Nguyệt', N'Nữ', TO_DATE('30-07-92', 'DD-MM-YY'), '079192000008', N'Hà Nội', '0901000008', N'Điều phối viên', N'Hành chính', 'HCM');

-- BENHNHAN
INSERT INTO BENHNHAN VALUES ('BN000001', N'Lý Văn Trường', N'Nam', TO_DATE('12-04-95', 'DD-MM-YY'), '079195000001', N'123', N'Nguyễn Trãi', N'Quận 5', N'TP.HCM', N'Không', N'Không', N'Penicillin');
INSERT INTO BENHNHAN VALUES ('BN000002', N'Trương Ngọc Bình', N'Nữ', TO_DATE('08-09-82', 'DD-MM-YY'), '079182000002', N'456', N'Lê Lợi', N'Quận 1', N'TP.HCM', N'Cao huyết áp', N'Tiểu đường', N'Không');
INSERT INTO BENHNHAN VALUES ('BN000003', N'Ngô Văn Thanh', N'Nam', TO_DATE('22-11-00', 'DD-MM-YY'), '079200000003', N'789', N'Trần Phú', N'Hà Đông', N'Hà Nội', N'Đau dạ dày', N'Không', N'Hải sản');

-- HSBA
INSERT INTO HSBA VALUES ('HS000001', 'BN000001', TO_DATE('10-04-26', 'DD-MM-YY'), N'Rối loạn nhịp tim', N'Theo dõi đồ thị tim, cấp thuốc', 'NV004', 'K01', N'Ổn định, cần tái khám sau 1 tuần');
INSERT INTO HSBA VALUES ('HS000002', 'BN000002', TO_DATE('11-04-26', 'DD-MM-YY'), N'Đau nửa đầu', N'Chụp MRI, cấp thuốc giảm đau', 'NV005', 'K02', N'Đau đầu căng cơ, không có tổn thương não');
INSERT INTO HSBA VALUES ('HS000003', 'BN000003', TO_DATE('12-04-26', 'DD-MM-YY'), N'Viêm dạ dày cấp', N'Nội soi dạ dày, cấp thuốc', 'NV006', 'K03', N'Viêm niêm mạc dạ dày vùng hang vị');

-- HSBA_DV
-- Dịch vụ do Kỹ thuật viên (NV007) thực hiện
INSERT INTO HSBA_DV VALUES ('HS000001', N'Đo điện tâm đồ (ECG)', TO_DATE('10-04-26', 'DD-MM-YY'), 'NV007', N'Nhịp tim hơi nhanh, xoang bình thường');
INSERT INTO HSBA_DV VALUES ('HS000002', N'Chụp MRI Sọ não', TO_DATE('11-04-26', 'DD-MM-YY'), 'NV007', N'Không phát hiện khối u hay tổn thương thực thể');
INSERT INTO HSBA_DV VALUES ('HS000003', N'Nội soi dạ dày', TO_DATE('12-04-26', 'DD-MM-YY'), 'NV007', N'Sung huyết hang vị mức độ nhẹ');

-- DONTHUOC
INSERT INTO DONTHUOC VALUES ('HS000001', N'Concor 5mg', TO_DATE('10-04-26', 'DD-MM-YY'), N'Ngày 1 viên, uống buổi sáng sau ăn');
INSERT INTO DONTHUOC VALUES ('HS000002', N'Paracetamol 500mg', TO_DATE('11-04-26', 'DD-MM-YY'), N'Ngày 2 viên, sáng/tối khi đau đầu');
INSERT INTO DONTHUOC VALUES ('HS000003', N'Omeprazole 20mg', TO_DATE('12-04-26', 'DD-MM-YY'), N'Ngày 1 viên, uống trước khi ăn sáng 30 phút');

-- THONG_BAO (Dành cho OLS - Hiện tại để trống nhãn OLS_LABEL chờ Phân hệ 2)
INSERT INTO THONG_BAO VALUES ('TB001', N'Họp giao ban toàn viện tháng 4', TO_TIMESTAMP('20-04-26 08:00:00', 'DD-MM-YY HH24:MI:SS'), N'Hội trường A', NULL);
INSERT INTO THONG_BAO VALUES ('TB002', N'Họp khẩn Ban Lãnh đạo Khoa Tim mạch', TO_TIMESTAMP('21-04-26 14:00:00', 'DD-MM-YY HH24:MI:SS'), N'Phòng họp 1 - Cơ sở HCM', NULL);

COMMIT;

