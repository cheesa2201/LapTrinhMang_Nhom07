// Form đầu tiên hiện ra khi mở app Client: cho người dùng nhập địa chỉ IP và Port của Host
// để thực hiện kết nối, trước khi vào MainForm chính để xem màn hình và chat.

using System;
using System.Windows.Forms;

namespace ScreenShare.Client.Forms
{
    public partial class ConnectForm : Form
    {
        // MainForm đọc 2 giá trị này sau khi ConnectForm đóng lại thành công
        public string HostIp { get; private set; } = "";
        public int HostPort { get; private set; }

        public ConnectForm()
        {
            InitializeComponent();
            btnConnect.Click += BtnConnect_Click;
        }

        private void BtnConnect_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIpAddress.Text))
            {
                lblStatus.Text = "Vui lòng nhập địa chỉ IP Host.";
                return;
            }

            if (!int.TryParse(txtPort.Text, out int port))
            {
                lblStatus.Text = "Port không hợp lệ.";
                return;
            }

            HostIp = txtIpAddress.Text.Trim();
            HostPort = port;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private TextBox txtIpAddress;
        private Label label1;
        private TextBox txtPort;
        private Button btnConnect;
        private Label lblStatus;
        private Label txtIp;

        private void InitializeComponent()
        {
            txtIpAddress = new TextBox();
            txtIp = new Label();
            label1 = new Label();
            txtPort = new TextBox();
            btnConnect = new Button();
            lblStatus = new Label();
            SuspendLayout();
            // 
            // txtIpAddress
            // 
            txtIpAddress.Location = new Point(90, 61);
            txtIpAddress.Name = "txtIpAddress";
            txtIpAddress.Size = new Size(442, 31);
            txtIpAddress.TabIndex = 0;
            txtIpAddress.Text = "127.0.0.1";
            // 
            // txtIp
            // 
            txtIp.AutoSize = true;
            txtIp.Location = new Point(90, 33);
            txtIp.Name = "txtIp";
            txtIp.Size = new Size(132, 25);
            txtIp.TabIndex = 1;
            txtIp.Text = "Địa chỉ IP Host:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(90, 125);
            label1.Name = "label1";
            label1.Size = new Size(48, 25);
            label1.TabIndex = 2;
            label1.Text = "Port:";
            // 
            // txtPort
            // 
            txtPort.Location = new Point(90, 153);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(442, 31);
            txtPort.TabIndex = 3;
            txtPort.Text = "5000";
            // 
            // btnConnect
            // 
            btnConnect.BackColor = Color.CornflowerBlue;
            btnConnect.Font = new Font("Segoe UI", 9F);
            btnConnect.ForeColor = SystemColors.ButtonHighlight;
            btnConnect.Location = new Point(90, 224);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(442, 34);
            btnConnect.TabIndex = 4;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = false;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.Brown;
            lblStatus.Location = new Point(90, 312);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(155, 25);
            lblStatus.TabIndex = 5;
            lblStatus.Text = "Port không hợp lệ";
            // 
            // ConnectForm
            // 
            ClientSize = new Size(634, 427);
            Controls.Add(lblStatus);
            Controls.Add(btnConnect);
            Controls.Add(txtPort);
            Controls.Add(label1);
            Controls.Add(txtIp);
            Controls.Add(txtIpAddress);
            Name = "ConnectForm";
            ResumeLayout(false);
            PerformLayout();

        }
    }
}