using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO;
using Microsoft.Win32;


namespace RebuildSql
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // Chọn ẩn
            this.Hide();
            //Sử dụng notifycon1 ẩn form - run appication background tray
            this.ShowInTaskbar = false;
            //Ẩn form
            WindowState = FormWindowState.Minimized;
            notifyIcon1.Visible = true;
            
            

         
        }
       

        private void button1_Click(object sender, EventArgs e)
        {
            // Cho hiện notifyIcon
            notifyIcon1.Visible = true;
            // Hiện BaloonTip hoặc không
            //notifyIcon1.ShowBalloonTip(10);

            // Chọn ẩn
            this.Hide();
            // Hoặc
            this.ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            // Hoặc cả 2 để ẩn form
            
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                // Ẩn notifyIcon đi
                notifyIcon1.Visible = false;
                // Cách này
                WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                // Hoặc
                this.Show();
                // Hoặc cả 2 miễn là phải tương ứng với lúc ẩn
            }
        }
        private async void rebuild()
        {
            try
            {
                // show data from Database ( Mydata) to DataGridView
                // Connect to server
                
                string connection = "server=hnibuv,1433;database=TESTDB;user=sa;password=root";
                SqlConnection con = new SqlConnection(connection);
                con.Open();
                //
                string qr1 = "ALTER INDEX [tbTest_index1] ON [dbo].[tbTest] REBUILD";
                SqlCommand command = new SqlCommand(qr1, con);
                command.CommandText = qr1;
                Task t = Task.Run(() => command.ExecuteNonQuery());
                t.Wait();
                //
                string qr2 = "ALTER INDEX [tbTest_index2] ON [dbo].[tbTest] REBUILD";
                SqlCommand cmd2 = new SqlCommand(qr2, con);
                cmd2.CommandText = qr2;
                Task y = Task.Run(() => cmd2.ExecuteNonQuery());
                y.Wait();
                
                // tự động shrink database
                string qr3 = "use MT_Xm; alter database TESTDB set recovery simple";
                SqlCommand cmd3 = new SqlCommand(qr3, con);
                cmd3.CommandText = qr3;
                Task z = Task.Run(() => cmd3.ExecuteNonQuery());
                z.Wait();
                //
                string qr4 = "alter database TESTDB set recovery simple";
                SqlCommand cmd4 = new SqlCommand(qr4, con);
                cmd4.CommandText = qr4;
                Task j = Task.Run(() => cmd4.ExecuteNonQuery());
                j.Wait();
                //
                string qr5 = "alter database TESTDB set recovery full";
                SqlCommand cmd5 = new SqlCommand(qr5, con);
                cmd4.CommandText = qr5;
                Task k = Task.Run(() => cmd5.ExecuteNonQuery());
                k.Wait();

                con.Dispose();
                command.Dispose();
                cmd2.Dispose();
                cmd3.Dispose();
                cmd4.Dispose();
       
                con.Close();
                //MessageBox.Show("Done");
                //use MT_Xm 
                //alter database  set recovery simple
                //dbcc shrinkfile(log,0)
                //alter database  set recovery full

            }
            catch (Exception ex)
            {
                MessageBox.Show("x");
            }
            return;
            
        }


        //Đăng ký chương trình khởi động cùng window
        private async void button2_Click(object sender, EventArgs e)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            registryKey.SetValue("Rebuildsql", Directory.GetCurrentDirectory() + "\\Rebuildsql.exe");
        }


        //hẹn giờ thực hiện
        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = DateTime.Now.ToString("mm/dd/yy hh:mm:ss tt");
            
            //Đang lỗi phần định dạng ngày chưa đúng nên chưa so sánh được - chuyển về timestap
            if (label1.Text == "01/12/22 01:05:00 AM")
            {
                timer1.Stop();
                rebuild();  
            }

        }

        
    }
}
