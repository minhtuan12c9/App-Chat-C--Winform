
namespace AppChatClient
{
    partial class GetMess
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.timeMess = new System.Windows.Forms.Label();
            this.messBox = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.guna2Panel6 = new Guna.UI2.WinForms.Guna2Panel();
            this.mess = new System.Windows.Forms.Label();
            this.avtMess = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.messBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.avtMess)).BeginInit();
            this.SuspendLayout();
            // 
            // timeMess
            // 
            this.timeMess.AutoSize = true;
            this.timeMess.BackColor = System.Drawing.Color.Transparent;
            this.timeMess.Font = new System.Drawing.Font("Franklin Gothic Medium", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeMess.ForeColor = System.Drawing.Color.Silver;
            this.timeMess.Location = new System.Drawing.Point(243, 113);
            this.timeMess.Name = "timeMess";
            this.timeMess.Size = new System.Drawing.Size(57, 16);
            this.timeMess.TabIndex = 13;
            this.timeMess.Text = "11:15 pm";
            // 
            // messBox
            // 
            this.messBox.BorderRadius = 15;
            this.messBox.Controls.Add(this.guna2Panel6);
            this.messBox.Controls.Add(this.mess);
            this.messBox.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(93)))));
            this.messBox.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(61)))), ((int)(((byte)(87)))));
            this.messBox.Location = new System.Drawing.Point(16, 28);
            this.messBox.Name = "messBox";
            this.messBox.Size = new System.Drawing.Size(293, 79);
            this.messBox.TabIndex = 14;
            // 
            // guna2Panel6
            // 
            this.guna2Panel6.BackColor = System.Drawing.Color.Transparent;
            this.guna2Panel6.BorderRadius = 25;
            this.guna2Panel6.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(47)))));
            this.guna2Panel6.Location = new System.Drawing.Point(-38, -31);
            this.guna2Panel6.Name = "guna2Panel6";
            this.guna2Panel6.Size = new System.Drawing.Size(84, 67);
            this.guna2Panel6.TabIndex = 6;
            // 
            // mess
            // 
            this.mess.BackColor = System.Drawing.Color.Transparent;
            this.mess.Font = new System.Drawing.Font("Franklin Gothic Medium", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mess.ForeColor = System.Drawing.Color.White;
            this.mess.Location = new System.Drawing.Point(49, 10);
            this.mess.Name = "mess";
            this.mess.Size = new System.Drawing.Size(241, 69);
            this.mess.TabIndex = 1;
            // 
            // avtMess
            // 
            this.avtMess.Image = global::AppChatClient.Properties.Resources.avatar_vo_tri_thumbnail1;
            this.avtMess.ImageRotate = 0F;
            this.avtMess.Location = new System.Drawing.Point(3, 3);
            this.avtMess.Name = "avtMess";
            this.avtMess.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.avtMess.Size = new System.Drawing.Size(48, 48);
            this.avtMess.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.avtMess.TabIndex = 12;
            this.avtMess.TabStop = false;
            // 
            // GetMess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.avtMess);
            this.Controls.Add(this.timeMess);
            this.Controls.Add(this.messBox);
            this.Name = "GetMess";
            this.Size = new System.Drawing.Size(314, 132);
            this.messBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.avtMess)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2CirclePictureBox avtMess;
        private System.Windows.Forms.Label timeMess;
        private Guna.UI2.WinForms.Guna2GradientPanel messBox;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel6;
        private System.Windows.Forms.Label mess;
    }
}
