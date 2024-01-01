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

    public partial class frmConnect : Form
    {
        public frmConnect()
        {
            InitializeComponent();
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtIPv4.Text) && !string.IsNullOrEmpty(txtPort.Text) && !string.IsNullOrEmpty(txtLogin.Text) && !string.IsNullOrEmpty(txtPass.Text))
            {
                Program.connstr_client = "Data Source=" + txtIPv4.Text + "," + txtPort.Text + ";Initial Catalog=HeThongPhanTan;User ID=" + txtLogin.Text + ";password=" + txtPass.Text + ";TrustServerCertificate=true";

                SqlSyncProvider serverProvider = new SqlSyncProvider(Program.connstr_publisher);
                SqlSyncProvider clientProvider = new SqlSyncProvider(Program.connstr_client);
                var setup = new SyncSetup("employee");
                SyncAgent agent = new SyncAgent(clientProvider, serverProvider);
                var result = await agent.SynchronizeAsync(setup);
                Console.WriteLine(result);
                MessageBox.Show("Đồng bộ dữ liệu thành công", "Thông báo", MessageBoxButtons.OK);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
