using AppChat.Controls;
using AppChatServer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppChat
{
    public partial class ChatServer : Form
    {
        public ChatServer()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            khoiTao();
        }
        Socket server;
        IPAddress ia;
        IPEndPoint ie;
        List<Socket> socketClients = new List<Socket>();
        List<Socket> socketSendClients = new List<Socket>();
        List<String> ipclients = new List<String>();
        int cb_location_y = 0;
        private void khoiTao()
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ia = IPAddress.Any;
            ie = new IPEndPoint(ia, 8888);
            server.Bind(ie);
            server.Listen(10);

            // Tạo một luồng mới để chấp nhận kết nối liên tục
            Thread acceptThread = new Thread(AcceptConnections);
            acceptThread.Start();
        }

        private void AcceptConnections()
        {
            while (true)
            {
                try
                {
                    Socket client = server.Accept();
                    socketClients.Add(client);
                    getData(client);
                    // Thực hiện xử lý cho clientSocket, bạn có thể tạo một luồng mới ở đây
                    // để xử lý dữ liệu từ clientSocket.
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi chấp nhận kết nối: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (txtSend.Text != "")
            {
                var dataObject = new
                {
                    type = "text",
                    data = txtSend.Text
                };

                string dataJson = JsonConvert.SerializeObject(dataObject);

                if (socketSendClients.Count > 0)
                {
                    socketSendClients.ForEach(socketClient =>
                    {
                        socketClient.Send(maHoa(dataJson));
                    });

                    String t = getTime();
                    SendMess sendmess = new SendMess(txtSend.Text, t);
                    FlowLayoutPanel rightSendMess = new FlowLayoutPanel();
                    rightSendMess.FlowDirection = FlowDirection.RightToLeft;

                    rightSendMess.Width = showChat.Width - 30;
                    rightSendMess.Height = sendmess.Height;

                    rightSendMess.Controls.Add(sendmess);
                    showChat.Controls.Add(rightSendMess);
                }
                else
                {
                    // message box
                }

                txtSend.Clear();
                txtSend.Focus();
            }
        }
        private void ProcessReceivedData(string data)
        {
            Mess mess = JsonConvert.DeserializeObject<Mess>(data);
            if (mess.type.ToLower() == "text")
            {
                string textData = mess.data;
                String t = getTime();
                GetMess getmess = new GetMess(textData, t);
                FlowLayoutPanel leftGetMess = new FlowLayoutPanel();
                leftGetMess.FlowDirection = FlowDirection.LeftToRight;

                leftGetMess.Width = showChat.Width - 30;
                leftGetMess.Height = getmess.Height;
                leftGetMess.Controls.Add(getmess);
                showChat.Controls.Add(leftGetMess);
            }
            else if (mess.type.ToLower() == "image")
            {
                string imageData = mess.data;
                // Hiển thị hình ảnh trong khung chat
                DisplayImageLeft(imageData, getTime());

            }
            else if (mess.type.ToLower() == "file")
            {
                byte[] fileData = Convert.FromBase64String(mess.data);
                string fileName = mess.filename;
                DisplayFileReceiveConfirmation(fileName, fileData, getTime());
            }
            else if (mess.type.ToLower() == "connected")
            {
                ipclients.Add(mess.data);
                showListClient();
            }
        }
        private void getData(Socket client)
        {
            Thread tNhan = new Thread(new ThreadStart(() => {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000]; // 5mb
                    client.Receive(data);
                    String s = giaiMa(data);
                    if (InvokeRequired)
                    {
                        Invoke((MethodInvoker)delegate {
                            ProcessReceivedData(s);
                        });
                    }
                    else
                    {
                        ProcessReceivedData(s);
                    }
                }
            }));
            tNhan.Start();
        }


        private byte[] maHoa(string s)
        {
            byte[] data = new byte[1024 * 5000]; // 5MB
            data = Encoding.Unicode.GetBytes(s);
            return data;
        }

        private String giaiMa(byte[] data)
        {
            String s = Encoding.Unicode.GetString(data);
            return s;
        }
        private String getTime()
        {
            // Lấy ngày và giờ hiện tại
            DateTime now = DateTime.Now;
            return now.ToString("hh:mm tt");
        }

        private void SendImage(string imagePath)
        {
            try
            {
                // Đọc dữ liệu hình ảnh từ tệp
                byte[] imageBytes;
                using (FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader binaryReader = new BinaryReader(fileStream))
                    {
                        imageBytes = binaryReader.ReadBytes((int)fileStream.Length);
                    }
                }
                
                // Chuyển hình ảnh thành chuỗi base64
                string imageBase64 = Convert.ToBase64String(imageBytes);

                var dataObject = new
                {
                    type = "image",
                    data = imageBase64
                };

                string dataJson = JsonConvert.SerializeObject(dataObject);

                // Gửi dữ liệu hình ảnh
                if (socketSendClients.Count > 0)
                {
                    socketSendClients.ForEach(socketClient =>
                    {
                        socketClient.Send(maHoa(dataJson));
                    });

                    // Hiển thị hình ảnh trong khung chat
                    DisplayImageRight(Image.FromFile(imagePath), getTime());
                }
                else
                {

                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi gửi hình ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImg_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif, *.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string imagePath = openFileDialog.FileName;
                SendImage(imagePath);
            }
        }

        private void DisplayImageRight(Image image, string time)
        {
            try
            {
                // Tạo một PictureBox và đọc ảnh từ tệp
                PictureBox picture = new PictureBox();
                picture.SizeMode = PictureBoxSizeMode.Zoom; // Giữ tỷ lệ khung hình
                picture.Image = image;

                // Tính toán kích thước mới để fit vào khung chat
                int maxImageWidth = showChat.Width - 30;
                int newWidth = Math.Min(picture.Image.Width, maxImageWidth);
                int newHeight = (int)((double)newWidth / picture.Image.Width * picture.Image.Height);

                // Đặt lại kích thước ảnh
                picture.Width = newWidth -100;
                picture.Height = newHeight -100;

                // Dịch chuyển ảnh về bên phải và thêm khoảng trống bên trái
                picture.Left = showChat.Width - picture.Width - 10; // Điều chỉnh giá trị để tạo khoảng trống
                picture.Margin = new Padding(10, 0, 0, 0); // Thêm khoảng trống bên trái

                // Tạo FlowLayoutPanel và thêm hình ảnh vào nó
                FlowLayoutPanel imagePanel = new FlowLayoutPanel();
                imagePanel.FlowDirection = FlowDirection.RightToLeft;
                imagePanel.Width = showChat.Width - 30;
                

                imagePanel.Controls.Add(picture);

                // Hiển thị thời gian
                Label timeLabel = new Label();
                timeLabel.Text = getTime();
                timeLabel.ForeColor = Color.White;
                timeLabel.Width = picture.Width - 40;
                timeLabel.Margin = new Padding(0, 10, 0, 0);

                imagePanel.Height = picture.Height + timeLabel.Height + 5;

                imagePanel.Controls.Add(timeLabel);

                // Thêm FlowLayoutPanel chứa hình ảnh vào showChat
                showChat.Controls.Add(imagePanel);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hiển thị hình ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnIcon_Click(object sender, EventArgs e)
        {
            if (pnIcon.Visible == false)
                pnIcon.Visible = true;
            else
                pnIcon.Visible = false; 
        }


        private void DisplayImageLeft(string imageData, string time)
        {
            try
            {
                // Tạo MemoryStream từ dữ liệu hình ảnh
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(imageData)))
                {
                    // Tạo một PictureBox và đọc ảnh từ MemoryStream
                    PictureBox picture = new PictureBox();
                    picture.SizeMode = PictureBoxSizeMode.Zoom; // Giữ tỷ lệ khung hình
                    picture.Image = Image.FromStream(ms);

                    // Tính toán kích thước mới để fit vào khung chat
                    int maxImageWidth = showChat.Width - 30;
                    int newWidth = Math.Min(picture.Image.Width, maxImageWidth);
                    int newHeight = (int)((double)newWidth / picture.Image.Width * picture.Image.Height);

                    // Đặt lại kích thước ảnh
                    picture.Width = newWidth - 100;
                    picture.Height = newHeight - 100;

                    // Dịch chuyển ảnh về bên phải và thêm khoảng trống bên trái
                    picture.Left = showChat.Width - picture.Width - 10; // Điều chỉnh giá trị để tạo khoảng trống
                    picture.Margin = new Padding(10, 0, 0, 0); // Thêm khoảng trống bên trái

                    // Tạo FlowLayoutPanel và thêm hình ảnh vào nó
                    FlowLayoutPanel imagePanel = new FlowLayoutPanel();
                    imagePanel.FlowDirection = FlowDirection.LeftToRight;
                    imagePanel.Width = showChat.Width - 30;


                    imagePanel.Controls.Add(picture);

                    // Hiển thị thời gian
                    Label timeLabel = new Label();
                    timeLabel.Text = getTime();
                    timeLabel.ForeColor = Color.White;
                    timeLabel.Width = picture.Width - 30;
                    timeLabel.Margin = new Padding(520, 10, 0, 0);

                    imagePanel.Height = picture.Height + timeLabel.Height + 5;

                    imagePanel.Controls.Add(timeLabel);

                    // Thêm FlowLayoutPanel chứa hình ảnh vào showChat
                    showChat.Controls.Add(imagePanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hiển thị hình ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void sendEmoji(object sender, EventArgs e)
        {
            try
            {
                // Lấy hình ảnh từ nút btnEmoji
                Guna.UI2.WinForms.Guna2CircleButton btnEmoji = (Guna.UI2.WinForms.Guna2CircleButton)sender;
                Image emojiImage = btnEmoji.Image;

                // Chuyển hình ảnh thành chuỗi base64
                string imageBase64 = ImageToBase64(emojiImage);

                var dataObject = new
                {
                    type = "image",
                    data = imageBase64
                };

                string dataJson = JsonConvert.SerializeObject(dataObject);

                // Gửi dữ liệu hình ảnh
                if (socketSendClients.Count > 0)
                {
                    socketSendClients.ForEach(socketClient =>
                    {
                        socketClient.Send(maHoa(dataJson));
                    });
                    // Hiển thị hình ảnh trong khung chat
                    DisplayImageRight(emojiImage, getTime());
                }
                else
                {

                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi gửi emoji: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string ImageToBase64(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                byte[] imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }



        private void btnFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                // Gửi dữ liệu tệp đến máy chủ
                SendFile(filePath, getTime());
            }
        }
        private void SendFile(string filePath, string time)
        {
            try
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                string fileName = Path.GetFileName(filePath);
                string fileBase64 = Convert.ToBase64String(fileData);

                var dataObject = new
                {
                    type = "file",
                    filename = fileName,
                    data = fileBase64
                };

                string dataJson = JsonConvert.SerializeObject(dataObject);
                if (socketSendClients.Count > 0)
                {
                    socketSendClients.ForEach(socketClient =>
                    {
                        socketClient.Send(maHoa(dataJson));
                    });

                    // Tạo liên kết và hiển thị trong khung chat
                    DisplayFileSentConfirmation(fileName, fileData, time);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi gửi tệp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        private void DisplayFileSentConfirmation(string fileName, byte[] fileBytes, string time)
        {
            try
            {
                // Tạo một Label để hiển thị thông điệp
                Label confirmationLabel = new Label();
                confirmationLabel.Text = $"Đã gửi tệp: {fileName}";
                confirmationLabel.ForeColor = Color.White;
                confirmationLabel.TextAlign = ContentAlignment.MiddleRight;
                confirmationLabel.Width = showChat.Width - 30;
                confirmationLabel.Margin = new Padding(0, 5, 0, 0);

                // Tạo một Button để tải tệp xuống
                Guna.UI2.WinForms.Guna2Button downloadButton = new Guna.UI2.WinForms.Guna2Button();
                downloadButton.Size = new Size(130,130);
                downloadButton.FillColor = Color.Transparent;
                downloadButton.ImageSize = new Size(120, 120);
                
                downloadButton.Click += (sender, e) => DownloadFile(fileBytes, fileName, "." + fileName.Split('.')[1]);

                // Đặt hình ảnh cho nút downloadButton
                downloadButton.Image = Properties.Resources.file; // Thay thế "file" bằng tên tệp hình ảnh thực tế
                

                // Tạo một FlowLayoutPanel và thêm Label và Button vào nó
                FlowLayoutPanel confirmationPanel = new FlowLayoutPanel();
                confirmationPanel.FlowDirection = FlowDirection.RightToLeft;
                confirmationPanel.Width = showChat.Width - 30;
                confirmationPanel.Controls.Add(confirmationLabel);
                confirmationPanel.Controls.Add(downloadButton);

                // Hiển thị thời gian
                Label timeLabel = new Label();
                timeLabel.TextAlign = ContentAlignment.TopRight;
                timeLabel.Text = time;
                timeLabel.ForeColor = Color.White;
                timeLabel.Width = confirmationLabel.Width - 40;
                timeLabel.Margin = new Padding(0, 5, 75, 0);

                confirmationPanel.Height = confirmationLabel.Height + downloadButton.Height + timeLabel.Height + 5;

                confirmationPanel.Controls.Add(timeLabel);

                // Thêm FlowLayoutPanel chứa thông điệp vào showChat
                showChat.Controls.Add(confirmationPanel);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hiển thị thông báo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DownloadFile(byte[] fileBytes, string name, string type)
        {
            try
            {
                // Yêu cầu người dùng chọn nơi lưu tệp
                string[] fileDetails = {
                    "Text Files (.txt)|.txt",
                    "PDF Files (.pdf)|.pdf",
                    "Image Files (.jpg;.png)|*.jpg;*.png",
                    "Excel Files (.xls;.xlsx)|*.xls;*.xlsx",
                    "Word Files (.doc;.docx)|*.doc;*.docx",
                    "PowerPoint Files (.ppt;.pptx)|*.ppt;*.pptx",
                    "Audio Files (.mp3;.wav)|*.mp3;*.wav",
                    "Video Files (.mp4;.avi)|*.mp4;*.avi",
                    "Compressed Files (.zip;.rar)|*.zip;*.rar"
                    // Thêm các cặp tên tệp và định dạng tệp tin tương ứng vào đây
                };

                string filterFile = fileDetails.FirstOrDefault(fileDetai => fileDetai.Contains(type));
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = filterFile + "|" + "All Files (*.*)|*.*";
                saveFileDialog.FileName = name;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string savePath = saveFileDialog.FileName;

                        try
                        {
                            // Ghi dữ liệu byte vào đường dẫn tệp tin đã chọn
                            File.WriteAllBytes(savePath, fileBytes);
                            MessageBox.Show("Lưu tệp tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("An error occurred: " + ex.Message);
                        }

                    MessageBox.Show("Tệp đã được tải xuống thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải xuống tệp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DisplayFileReceiveConfirmation(string fileName, byte[] fileBytes, string time)
        {
            try
            {
                // Tạo một Label để hiển thị thông điệp
                Label confirmationLabel = new Label();
                confirmationLabel.Text = $"Đã gửi tệp: {fileName}";
                confirmationLabel.ForeColor = Color.White;
                confirmationLabel.TextAlign = ContentAlignment.MiddleLeft;
                confirmationLabel.Width = showChat.Width - 30;
                confirmationLabel.Margin = new Padding(20, 5, 0, 0);

                // Tạo một Button để tải tệp xuống
                Guna.UI2.WinForms.Guna2Button downloadButton = new Guna.UI2.WinForms.Guna2Button();
                downloadButton.Size = new Size(130, 130);
                downloadButton.FillColor = Color.Transparent;
                downloadButton.ImageSize = new Size(120, 120);
                downloadButton.Margin = new Padding(20, 0, 0, 0);

                downloadButton.Click += (sender, e) => DownloadFile(fileBytes, fileName, "." + fileName.Split('.')[1]);

                // Đặt hình ảnh cho nút downloadButton
                downloadButton.Image = Properties.Resources.file; // Thay thế "file" bằng tên tệp hình ảnh thực tế


                // Tạo một FlowLayoutPanel và thêm Label và Button vào nó
                FlowLayoutPanel confirmationPanel = new FlowLayoutPanel();
                confirmationPanel.FlowDirection = FlowDirection.LeftToRight;
                confirmationPanel.Width = showChat.Width - 30;
                confirmationPanel.Controls.Add(confirmationLabel);
                confirmationPanel.Controls.Add(downloadButton);

                // Hiển thị thời gian
                Label timeLabel = new Label();
                /*timeLabel.TextAlign = ContentAlignment.TopRight;*/
                timeLabel.Text = time;
                timeLabel.ForeColor = Color.White;
                timeLabel.Width = confirmationLabel.Width - 40;
                timeLabel.Margin = new Padding(70, 5, 0, 0);

                confirmationPanel.Height = confirmationLabel.Height + downloadButton.Height + timeLabel.Height + 5;

                confirmationPanel.Controls.Add(timeLabel);

                // Thêm FlowLayoutPanel chứa thông điệp vào showChat
                showChat.Controls.Add(confirmationPanel);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hiển thị thông báo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void showListClient()
        {
            pnList.Controls.Clear();
            cb_location_y = 0;
            ipclients.ForEach(ip =>
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Width = 250;
                checkbox.Text = ip;
                checkbox.Font = new Font("Arial", 12);
                checkbox.ForeColor = Color.White;
                checkbox.Location = new Point(0, cb_location_y);
                checkbox.Tag = ip;
                checkbox.Click += Checkbox_Click;

                pnList.Controls.Add(checkbox);
                cb_location_y += checkbox.Height + 5;
            });
        }

        private void Checkbox_Click(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            int socketIndex = ipclients.FindIndex(ip => ip == checkBox.Tag.ToString());
            checkBox.Checked = checkBox.Checked == true;
            if (socketIndex != -1)
            {
                if (checkBox.Checked==true)
                {
                    socketSendClients.Add(socketClients[socketIndex]);
                }
                else
                {
                    socketSendClients.Remove(socketClients[socketIndex]);
                }
            }
        }
    }
}
