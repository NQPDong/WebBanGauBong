using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebBanGauBong.Models;

namespace WebBanGauBong.Areas.Admin.Services
{
    public class AccountService
    {
        //public bool Connect(string username, string password, string role)
        //{
        //    string connStr = $"Data Source=localhost;Initial Catalog=QL_THU_BONG;User ID={username};Password={password};";
        //    string query = $"SELECT IS_ROLEMEMBER('{role}');";
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connStr))
        //        {
        //            conn.Open();

        //            using (SqlCommand cmd = new SqlCommand(query, conn))
        //            {

        //                // Thực thi truy vấn và nhận kết quả (luôn là một số nguyên)
        //                int result = (int)cmd.ExecuteScalar();

        //                // Trả về true nếu User là thành viên của Role (result == 1)
        //                return result == 1;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Xử lý lỗi kết nối, đăng nhập sai, hoặc lỗi thực thi query
        //        Console.WriteLine($"Error during role check: {ex.Message}");
        //        return false;
        //    }
        //}


        public bool Connect(string username, string password, string role)
        {
            //string connStr = $"workstation id = QL_THU_BONG.mssql.somee.com; packet size = 4096; user id = {username}; pwd = {password}; data source = QL_THU_BONG.mssql.somee.com; persist security info = False; initial catalog = QL_THU_BONG; TrustServerCertificate = True";
            string connStr = $"Server=db35576.databaseasp.net; Database=db35576; User Id={username}; Password={password}; Encrypt=False; MultipleActiveResultSets=True;  ";
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần
                Console.WriteLine($"Error during connection check: {ex.Message}");
                return false;
            }
        }


    }
}