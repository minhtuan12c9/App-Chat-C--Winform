using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppChat.Controls
{
    public partial class SendMess : UserControl
    {
        public SendMess(String s, String t)
        {
            InitializeComponent();
            mess2.Text = s;
            timeMess2.Text = t;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Hiển thị hộp thoại xác nhận
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xoá không?", "Xác nhận xoá", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            // Kiểm tra kết quả của hộp thoại
            if (result == DialogResult.OK)
            {
                PerformDeleteOperation();
            }
            else
            {
                // Người dùng đã chọn "Cancel" hoặc đóng hộp thoại, không làm gì cả
            }
        }
        private void PerformDeleteOperation()
        {
            messBox2.FillColor = Color.DimGray;
            messBox2.FillColor2 = Color.DimGray;
            mess2.Text = "Bạn đã xoá tin nhắn này";
            mess2.ForeColor = Color.White;
            mess2.Padding = new Padding(0, 0, 0, 10);
            mess2.TextAlign = ContentAlignment.MiddleCenter;
        }
    }
}
