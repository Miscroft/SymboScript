namespace SymboScript
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button_Exe = new System.Windows.Forms.Button();
            this.CodeBox = new System.Windows.Forms.RichTextBox();
            this.button_Finish = new System.Windows.Forms.Button();
            this.ErrBox = new System.Windows.Forms.RichTextBox();
            this.button_Com = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.檔案ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.開啟ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.存檔ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.建置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.編譯ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.執行ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.停止ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Exe
            // 
            this.button_Exe.Location = new System.Drawing.Point(286, 1);
            this.button_Exe.Name = "button_Exe";
            this.button_Exe.Size = new System.Drawing.Size(75, 23);
            this.button_Exe.TabIndex = 0;
            this.button_Exe.Text = "Execute";
            this.button_Exe.UseVisualStyleBackColor = true;
            this.button_Exe.Click += new System.EventHandler(this.compile);
            // 
            // CodeBox
            // 
            this.CodeBox.DetectUrls = false;
            this.CodeBox.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.CodeBox.Location = new System.Drawing.Point(5, 22);
            this.CodeBox.Name = "CodeBox";
            this.CodeBox.Size = new System.Drawing.Size(440, 330);
            this.CodeBox.TabIndex = 1;
            this.CodeBox.Text = "";
            // 
            // button_Finish
            // 
            this.button_Finish.Location = new System.Drawing.Point(367, 1);
            this.button_Finish.Name = "button_Finish";
            this.button_Finish.Size = new System.Drawing.Size(75, 23);
            this.button_Finish.TabIndex = 2;
            this.button_Finish.Text = "Finish";
            this.button_Finish.UseVisualStyleBackColor = true;
            this.button_Finish.Click += new System.EventHandler(this.finish_compile);
            // 
            // ErrBox
            // 
            this.ErrBox.Location = new System.Drawing.Point(5, 359);
            this.ErrBox.Name = "ErrBox";
            this.ErrBox.Size = new System.Drawing.Size(440, 150);
            this.ErrBox.TabIndex = 3;
            this.ErrBox.Text = "";
            // 
            // button_Com
            // 
            this.button_Com.Location = new System.Drawing.Point(205, 1);
            this.button_Com.Name = "button_Com";
            this.button_Com.Size = new System.Drawing.Size(75, 23);
            this.button_Com.TabIndex = 4;
            this.button_Com.Text = "Compile";
            this.button_Com.UseVisualStyleBackColor = true;
            this.button_Com.Click += new System.EventHandler(this.execute);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.檔案ToolStripMenuItem,
            this.建置ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(454, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 檔案ToolStripMenuItem
            // 
            this.檔案ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.開啟ToolStripMenuItem,
            this.存檔ToolStripMenuItem});
            this.檔案ToolStripMenuItem.Name = "檔案ToolStripMenuItem";
            this.檔案ToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.檔案ToolStripMenuItem.Text = "檔案";
            // 
            // 開啟ToolStripMenuItem
            // 
            this.開啟ToolStripMenuItem.Name = "開啟ToolStripMenuItem";
            this.開啟ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.開啟ToolStripMenuItem.Text = "開啟";
            this.開啟ToolStripMenuItem.Click += new System.EventHandler(this.open_file);
            // 
            // 存檔ToolStripMenuItem
            // 
            this.存檔ToolStripMenuItem.Name = "存檔ToolStripMenuItem";
            this.存檔ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.存檔ToolStripMenuItem.Text = "存檔";
            this.存檔ToolStripMenuItem.Click += new System.EventHandler(this.save_file);
            // 
            // 建置ToolStripMenuItem
            // 
            this.建置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.編譯ToolStripMenuItem,
            this.執行ToolStripMenuItem,
            this.停止ToolStripMenuItem});
            this.建置ToolStripMenuItem.Name = "建置ToolStripMenuItem";
            this.建置ToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.建置ToolStripMenuItem.Text = "建置";
            // 
            // 編譯ToolStripMenuItem
            // 
            this.編譯ToolStripMenuItem.Name = "編譯ToolStripMenuItem";
            this.編譯ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.編譯ToolStripMenuItem.Text = "編譯";
            this.編譯ToolStripMenuItem.Click += new System.EventHandler(this.execute);
            // 
            // 執行ToolStripMenuItem
            // 
            this.執行ToolStripMenuItem.Name = "執行ToolStripMenuItem";
            this.執行ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.執行ToolStripMenuItem.Text = "執行";
            this.執行ToolStripMenuItem.Click += new System.EventHandler(this.compile);
            // 
            // 停止ToolStripMenuItem
            // 
            this.停止ToolStripMenuItem.Name = "停止ToolStripMenuItem";
            this.停止ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.停止ToolStripMenuItem.Text = "停止";
            this.停止ToolStripMenuItem.Click += new System.EventHandler(this.finish_compile);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 522);
            this.Controls.Add(this.button_Com);
            this.Controls.Add(this.ErrBox);
            this.Controls.Add(this.button_Finish);
            this.Controls.Add(this.CodeBox);
            this.Controls.Add(this.button_Exe);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "SymboScript";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Exe;
        private System.Windows.Forms.RichTextBox CodeBox;
        private System.Windows.Forms.Button button_Finish;
        private System.Windows.Forms.RichTextBox ErrBox;
        private System.Windows.Forms.Button button_Com;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 檔案ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 開啟ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 存檔ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 建置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 編譯ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 執行ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 停止ToolStripMenuItem;
    }
}

