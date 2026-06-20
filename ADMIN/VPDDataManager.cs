using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace ADMIN
{
    /// <summary>
    /// Quản lý dữ liệu VPD - thực hiện các truy vấn không WHERE
    /// để để VPD tự động filter dữ liệu dựa trên bác sĩ đang đăng nhập
    /// </summary>
    public class VPDDataManager
    {
        private OracleConnection _doctorConnection;
        private string _currentUsername;

        public VPDDataManager(OracleConnection connection, string username)
        {
            _doctorConnection = connection;
            _currentUsername = username;
        }

        /// <summary>
        /// Lấy danh sách HSBA (VPD sẽ tự filter: MABS = username)
        /// </summary>
        public DataTable GetHSBA()
        {
            DataTable dt = new DataTable();
            try
            {
                string query = "SELECT MAHSBA, MABN, NGAY, CHANDOAN, DIEÜTR, KETLUAN, MAKHOA FROM ADMIN_PHANHE1.HSBA";
                using (OracleCommand cmd = new OracleCommand(query, _doctorConnection))
                {
                    cmd.CommandTimeout = 30;
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy HSBA: {ex.Message}");
            }
            return dt;
        }

        /// <summary>
        /// Lấy danh sách bệnh nhân (VPD filter: MABN IN (SELECT MABN FROM HSBA WHERE MABS = username))
        /// </summary>
        public DataTable GetBenhNhan()
        {
            DataTable dt = new DataTable();
            try
            {
                string query = "SELECT MABN, TENBN, PHAI, NGAYSINH, TIENSÜBENH, TIENSÜBENHGD, DIÜUNGTHUOC FROM ADMIN_PHANHE1.BENHNHAN";
                using (OracleCommand cmd = new OracleCommand(query, _doctorConnection))
                {
                    cmd.CommandTimeout = 30;
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy bệnh nhân: {ex.Message}");
            }
            return dt;
        }

        /// <summary>
        /// Lấy danh sách đơn thuốc (VPD filter: MAHSBA IN (SELECT MAHSBA FROM HSBA WHERE MABS = username))
        /// </summary>
        public DataTable GetDonThuoc()
        {
            DataTable dt = new DataTable();
            try
            {
                string query = "SELECT MAHSBA, NGAYDT, TENTHUOC, LIEUDUNG FROM ADMIN_PHANHE1.DONTHUOC";
                using (OracleCommand cmd = new OracleCommand(query, _doctorConnection))
                {
                    cmd.CommandTimeout = 30;
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy đơn thuốc: {ex.Message}");
            }
            return dt;
        }

        /// <summary>
        /// Lấy danh sách dịch vụ chẩn đoán (VPD filter: MAHSBA IN (SELECT MAHSBA FROM HSBA WHERE MABS = username))
        /// </summary>
        public DataTable GetHSBA_DV()
        {
            DataTable dt = new DataTable();
            try
            {
                string query = "SELECT MAHSBA, LOAIDV, NGAYDV, MAKTV, KETQUA FROM ADMIN_PHANHE1.HSBA_DV";
                using (OracleCommand cmd = new OracleCommand(query, _doctorConnection))
                {
                    cmd.CommandTimeout = 30;
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy dịch vụ: {ex.Message}");
            }
            return dt;
        }

        /// <summary>
        /// Cập nhật HSBA (chỉ 3 trường cho phép: CHANDOAN, DIEÜTR, KETLUAN)
        /// </summary>
        public void UpdateHSBA(string mahsba, string chanDoan, string dieuTri, string ketLuan)
        {
            try
            {
                string query = @"UPDATE ADMIN_PHANHE1.HSBA 
                                SET CHANDOAN = :chanDoan, DIEÜTR = :dieuTri, KETLUAN = :ketLuan 
                                WHERE MAHSBA = :mahsba";
                using (OracleCommand cmd = new OracleCommand(query, _doctorConnection))
                {
                    cmd.Parameters.Add(":mahsba", mahsba);
                    cmd.Parameters.Add(":chanDoan", chanDoan);
                    cmd.Parameters.Add(":dieuTri", dieuTri);
                    cmd.Parameters.Add(":ketLuan", ketLuan);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi cập nhật HSBA: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật bệnh nhân (3 trường: TIENSÜBENH, TIENSÜBENHGD, DIÜUNGTHUOC)
        /// </summary>
        public void UpdateBenhNhan(string mabn, string tienSuBenh, string tienSuBenhGD, string diUngThuoc)
        {
            try
            {
                string query = @"UPDATE ADMIN_PHANHE1.BENHNHAN 
                                SET TIENSÜBENH = :tiensübenh, TIENSÜBENHGD = :tiensübenhgd, DIÜUNGTHUOC = :diüungthuoc 
                                WHERE MABN = :mabn";
                using (OracleCommand cmd = new OracleCommand(query, _doctorConnection))
                {
                    cmd.Parameters.Add(":mabn", mabn);
                    cmd.Parameters.Add(":tiensübenh", tienSuBenh);
                    cmd.Parameters.Add(":tiensübenhgd", tienSuBenhGD);
                    cmd.Parameters.Add(":diüungthuoc", diUngThuoc);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi cập nhật bệnh nhân: {ex.Message}");
            }
        }

        /// <summary>
        /// Thêm đơn thuốc (validate MAHSBA thuộc bác sĩ hiện tại)
        /// </summary>
        public void InsertDonThuoc(string mahsba, string ngaydt, string tenThuoc, string lieuDung)
        {
            try
            {
                // Validate MAHSBA thuộc bác sĩ
                if (!IsHSBABelongsToDoctor(mahsba))
                {
                    throw new Exception("HSBA không thuộc bác sĩ hiện tại!");
                }

                string query = @"INSERT INTO ADMIN_PHANHE1.DONTHUOC (MAHSBA, NGAYDT, TENTHUOC, LIEUDUNG) 
                                VALUES (:mahsba, :ngaydt, :tenthuoc, :lieudung)";
                using (OracleCommand cmd = new OracleCommand(query, _doctorConnection))
                {
                    cmd.Parameters.Add(":mahsba", mahsba);
                    cmd.Parameters.Add(":ngaydt", ngaydt);
                    cmd.Parameters.Add(":tenthuoc", tenThuoc);
                    cmd.Parameters.Add(":lieudung", lieuDung);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi thêm đơn thuốc: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa đơn thuốc
        /// </summary>
        public void DeleteDonThuoc(string mahsba, string ngaydt)
        {
            try
            {
                string query = @"DELETE FROM ADMIN_PHANHE1.DONTHUOC WHERE MAHSBA = :mahsba AND NGAYDT = :ngaydt";
                using (OracleCommand cmd = new OracleCommand(query, _doctorConnection))
                {
                    cmd.Parameters.Add(":mahsba", mahsba);
                    cmd.Parameters.Add(":ngaydt", ngaydt);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi xóa đơn thuốc: {ex.Message}");
            }
        }

        /// <summary>
        /// Thêm dịch vụ chẩn đoán (validate MAHSBA)
        /// </summary>
        public void InsertHSBA_DV(string mahsba, string loaidv, string ngaydv, string maktv, string ketqua)
        {
            try
            {
                if (!IsHSBABelongsToDoctor(mahsba))
                {
                    throw new Exception("HSBA không thuộc bác sĩ hiện tại!");
                }

                string query = @"INSERT INTO ADMIN_PHANHE1.HSBA_DV (MAHSBA, LOAIDV, NGAYDV, MAKTV, KETQUA) 
                                VALUES (:mahsba, :loaidv, :ngaydv, :maktv, :ketqua)";
                using (OracleCommand cmd = new OracleCommand(query, _doctorConnection))
                {
                    cmd.Parameters.Add(":mahsba", mahsba);
                    cmd.Parameters.Add(":loaidv", loaidv);
                    cmd.Parameters.Add(":ngaydv", ngaydv);
                    cmd.Parameters.Add(":maktv", maktv);
                    cmd.Parameters.Add(":ketqua", ketqua);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi thêm dịch vụ: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa dịch vụ chẩn đoán
        /// </summary>
        public void DeleteHSBA_DV(string mahsba, string loaidv, string ngaydv)
        {
            try
            {
                string query = @"DELETE FROM ADMIN_PHANHE1.HSBA_DV WHERE MAHSBA = :mahsba AND LOAIDV = :loaidv AND NGAYDV = :ngaydv";
                using (OracleCommand cmd = new OracleCommand(query, _doctorConnection))
                {
                    cmd.Parameters.Add(":mahsba", mahsba);
                    cmd.Parameters.Add(":loaidv", loaidv);
                    cmd.Parameters.Add(":ngaydv", ngaydv);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi xóa dịch vụ: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra HSBA có thuộc bác sĩ hiện tại không (dùng VPD để validate)
        /// </summary>
        private bool IsHSBABelongsToDoctor(string mahsba)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM ADMIN_PHANHE1.HSBA WHERE MAHSBA = :mahsba";
                using (OracleCommand cmd = new OracleCommand(query, _doctorConnection))
                {
                    cmd.Parameters.Add(":mahsba", mahsba);
                    object result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int count))
                        return count > 0;
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy danh sách MAHSBA cho dropdown (dùng khi thêm dữ liệu)
        /// </summary>
        public List<string> GetHSBAList()
        {
            List<string> list = new List<string>();
            try
            {
                string query = "SELECT MAHSBA FROM ADMIN_PHANHE1.HSBA ORDER BY MAHSBA";
                using (OracleCommand cmd = new OracleCommand(query, _doctorConnection))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var value = reader[0]?.ToString();
                            if (!string.IsNullOrEmpty(value))
                                list.Add(value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy danh sách HSBA: {ex.Message}");
            }
            return list;
        }
    }
}
