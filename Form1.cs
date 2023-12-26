using Dotmim.Sync;
using Dotmim.Sync.SqlServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace doan_phantan
{
    public partial class Form1 : Form
    {
        DataTable dt_DS_emp = new DataTable();
        public Form1()
        {
            InitializeComponent();
        }
        private SqlConnection conn_publisher = new SqlConnection();
        private int ketNoi_CSDLGOC()
        {

            if (conn_publisher != null && conn_publisher.State == ConnectionState.Open) conn_publisher.Close();
            try
            {
                conn_publisher.ConnectionString = Program.connstr_publisher;
                conn_publisher.Close();
                return 1;
            }

            catch (Exception e)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu.\nBạn xem lại user name và password.\n " + e.Message, "", MessageBoxButtons.OK);
                return 0;
            }

        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (ketNoi_CSDLGOC() == 0) return;
            else MessageBox.Show("Thanh cong\n ");


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'heThongPhanTanDataSet.employee' table. You can move, or remove it, as needed.
            this.employeeTableAdapter.Fill(this.heThongPhanTanDataSet.employee);
            Program.KetNoi();


        }




        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("NAME", typeof(string));
            dt.Columns.Add("BIRTHDAY", typeof(DateTime));
            dt.Columns.Add("GENDER", typeof(bool));
            dt.Columns.Add("EMAIL", typeof(string));


            if (bdsNhanVien.DataSource is doan_phantan.HeThongPhanTanDataSet dataSet)
            {
                // Kiểm tra xem DataSet có chứa DataTable 'employee' không
                if (dataSet.Tables.Contains("employee"))
                {
                    dt_DS_emp = dataSet.Tables["employee"];

                    for (int i = 0; i < dt_DS_emp.Rows.Count; i++)
                    {
                        // Kiểm tra giá trị null trước khi thêm vào DataTable
                        bool genderValue = (dt_DS_emp.Rows[i]["GENDER"] != DBNull.Value) ? Convert.ToBoolean(dt_DS_emp.Rows[i]["GENDER"]) : false;

                        dt.Rows.Add(
                            dt_DS_emp.Rows[i]["ID"],
                            dt_DS_emp.Rows[i]["NAME"],
                            dt_DS_emp.Rows[i]["BIRTHDAY"],
                            genderValue,
                            dt_DS_emp.Rows[i]["EMAIL"]
                        );

                    }
                }
                else
                {
                    MessageBox.Show("DataSet không chứa DataTable 'employee'.");
                }
            }
            else
            {
                MessageBox.Show("DataSource không phải là một DataSet.");
            }
            try
            {
                SqlParameter para = new SqlParameter();
                para.SqlDbType = SqlDbType.Structured;
                para.TypeName = "dbo.TYPE_EMPLOYEE";
                para.ParameterName = "@EMPLOYEE";
                para.Value = dt;

                Program.KetNoi();
                SqlCommand Sqlcmd = new SqlCommand("SP_UPDATE_EMPLOYEE", Program.conn);
                Sqlcmd.Parameters.Clear();
                Sqlcmd.CommandType = CommandType.StoredProcedure;
                Sqlcmd.Parameters.Add(para);
                Sqlcmd.ExecuteNonQuery();
                MessageBox.Show("Cập nhật thành công!!!");
                // Reload lại dữ liệu trong DataGridView
                this.employeeTableAdapter.Fill(this.heThongPhanTanDataSet.employee);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
                Console.WriteLine($"Lỗi: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

       

        private void btnXoa_Click_1(object sender, EventArgs e)
        {
            if (dgvEmployee.SelectedRows.Count > 0)
            {
                int rowIndex = dgvEmployee.SelectedRows[0].Index;

                if (rowIndex >= 0 && rowIndex < dgvEmployee.Rows.Count)
                {
                    // Lấy giá trị của cột đầu tiên từ dòng được chọn
                    object firstColumnValue = dgvEmployee.Rows[rowIndex].Cells[0].Value;
                    object ColumnValue2 = dgvEmployee.Rows[rowIndex].Cells[1].Value;

                    // Hiển thị hộp thoại xác nhận
                    DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa Nhân viên : {ColumnValue2} không?",
                                                          "Xác nhận xóa",
                                                          MessageBoxButtons.YesNo,
                                                          MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Nếu người dùng chọn "Yes", thì tiến hành xóa dòng
                        try
                        {
                            String strlenh = "EXEC SP_XOA_EMPLOYEE '" + firstColumnValue + "'";

                            //MessageBox.Show(strlenh);

                            dt_DS_emp = Program.ExecSqlDataTable(strlenh);
                            dgvEmployee.Rows.RemoveAt(rowIndex);
                            MessageBox.Show("Xóa thành công!!!");
                            // Reload lại dữ liệu trong DataGridView
                            this.employeeTableAdapter.Fill(this.heThongPhanTanDataSet.employee);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.StackTrace);
                            Console.WriteLine($"Lỗi: {ex.Message}");
                            Console.WriteLine($"StackTrace: {ex.StackTrace}");
                        }
                    }
                    // Nếu người dùng chọn "No", không thực hiện xóa.
                }
                else
                {
                    MessageBox.Show("Vị trí hàng không hợp lệ.");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để xóa.");
            }
        }

        private async void btnBackup_Click(object sender, EventArgs e)
        {
            SqlSyncProvider serverProvider = new SqlSyncProvider(Program.connstr_publisher);
            SqlSyncProvider clientProvider = new SqlSyncProvider(Program.connstr_client);
            var setup = new SyncSetup("employee");
            SyncAgent agent = new SyncAgent(clientProvider, serverProvider);
            var result = await agent.SynchronizeAsync(setup);
            Console.WriteLine(result);
            MessageBox.Show("Đồng bộ dữ liệu thành công", "Thông báo", MessageBoxButtons.OK);

        }

        private void dgvEmployee_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
