using System;

namespace DATAFILTER
{
    partial class MainForm
    {
        #region GIẢI PHÓNG TÀI NGUYÊN
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                filterWorker?.Dispose();
                textChangedTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region PHẦN THIẾT KẾ UI ỨNG DỤNG - MODERN VERSION
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainContainer = new System.Windows.Forms.SplitContainer();
            this.inputPanel = new System.Windows.Forms.Panel();
            this.inputTextBox = new System.Windows.Forms.RichTextBox();
            this.inputHeaderPanel = new System.Windows.Forms.Panel();
            this.inputCountLabel = new System.Windows.Forms.Label();
            this.inputTitleLabel = new System.Windows.Forms.Label();
            this.resultPanel = new System.Windows.Forms.Panel();
            this.resultTextBox = new System.Windows.Forms.RichTextBox();
            this.resultHeaderPanel = new System.Windows.Forms.Panel();
            this.resultCountLabel = new System.Windows.Forms.Label();
            this.resultTitleLabel = new System.Windows.Forms.Label();
            this.topToolbarPanel = new System.Windows.Forms.Panel();
            this.lineCountPanel = new System.Windows.Forms.Panel();
            this.lineCountComboBox = new System.Windows.Forms.ComboBox();
            this.lineCountLabel = new System.Windows.Forms.Label();
            this.bottomToolbarPanel = new System.Windows.Forms.Panel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.exportButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.filterButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.mainContainer)).BeginInit();
            this.mainContainer.Panel1.SuspendLayout();
            this.mainContainer.Panel2.SuspendLayout();
            this.mainContainer.SuspendLayout();
            this.inputPanel.SuspendLayout();
            this.inputHeaderPanel.SuspendLayout();
            this.resultPanel.SuspendLayout();
            this.resultHeaderPanel.SuspendLayout();
            this.topToolbarPanel.SuspendLayout();
            this.lineCountPanel.SuspendLayout();
            this.bottomToolbarPanel.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainContainer
            // 
            this.mainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContainer.Location = new System.Drawing.Point(0, 45);
            this.mainContainer.Name = "mainContainer";
            this.mainContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainContainer.Panel1
            // 
            this.mainContainer.Panel1.Controls.Add(this.inputPanel);
            this.mainContainer.Panel1MinSize = 200;
            // 
            // mainContainer.Panel2
            // 
            this.mainContainer.Panel2.Controls.Add(this.resultPanel);
            this.mainContainer.Panel2MinSize = 200;
            this.mainContainer.Size = new System.Drawing.Size(484, 785);
            this.mainContainer.SplitterDistance = 386;
            this.mainContainer.SplitterWidth = 8;
            this.mainContainer.TabIndex = 0;
            // 
            // inputPanel
            // 
            this.inputPanel.BackColor = System.Drawing.Color.White;
            this.inputPanel.Controls.Add(this.inputTextBox);
            this.inputPanel.Controls.Add(this.inputHeaderPanel);
            this.inputPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputPanel.Location = new System.Drawing.Point(0, 0);
            this.inputPanel.Name = "inputPanel";
            this.inputPanel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 8);
            this.inputPanel.Size = new System.Drawing.Size(484, 386);
            this.inputPanel.TabIndex = 0;
            // 
            // inputTextBox
            // 
            this.inputTextBox.BackColor = System.Drawing.Color.White;
            this.inputTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputTextBox.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputTextBox.Location = new System.Drawing.Point(8, 34);
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(468, 344);
            this.inputTextBox.TabIndex = 1;
            this.inputTextBox.Text = "";
            this.inputTextBox.WordWrap = false;
            // 
            // inputHeaderPanel
            // 
            this.inputHeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(240)))), ((int)(((byte)(255)))));
            this.inputHeaderPanel.Controls.Add(this.inputCountLabel);
            this.inputHeaderPanel.Controls.Add(this.inputTitleLabel);
            this.inputHeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.inputHeaderPanel.Location = new System.Drawing.Point(8, 0);
            this.inputHeaderPanel.Name = "inputHeaderPanel";
            this.inputHeaderPanel.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.inputHeaderPanel.Size = new System.Drawing.Size(468, 34);
            this.inputHeaderPanel.TabIndex = 0;
            // 
            // inputCountLabel
            // 
            this.inputCountLabel.AutoSize = true;
            this.inputCountLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.inputCountLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputCountLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
            this.inputCountLabel.Location = new System.Drawing.Point(343, 5);
            this.inputCountLabel.Name = "inputCountLabel";
            this.inputCountLabel.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.inputCountLabel.Size = new System.Drawing.Size(115, 23);
            this.inputCountLabel.TabIndex = 1;
            this.inputCountLabel.Text = "📊 Số lượng: 0 dòng";
            this.inputCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // inputTitleLabel
            // 
            this.inputTitleLabel.AutoSize = true;
            this.inputTitleLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.inputTitleLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(90)))), ((int)(((byte)(140)))));
            this.inputTitleLabel.Location = new System.Drawing.Point(10, 5);
            this.inputTitleLabel.Name = "inputTitleLabel";
            this.inputTitleLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.inputTitleLabel.Size = new System.Drawing.Size(145, 22);
            this.inputTitleLabel.TabIndex = 0;
            this.inputTitleLabel.Text = "📥 DỮ LIỆU ĐẦU VÀO";
            // 
            // resultPanel
            // 
            this.resultPanel.BackColor = System.Drawing.Color.White;
            this.resultPanel.Controls.Add(this.resultTextBox);
            this.resultPanel.Controls.Add(this.resultHeaderPanel);
            this.resultPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultPanel.Location = new System.Drawing.Point(0, 0);
            this.resultPanel.Name = "resultPanel";
            this.resultPanel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 8);
            this.resultPanel.Size = new System.Drawing.Size(484, 391);
            this.resultPanel.TabIndex = 0;
            // 
            // resultTextBox
            // 
            this.resultTextBox.BackColor = System.Drawing.Color.White;
            this.resultTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.resultTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultTextBox.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultTextBox.Location = new System.Drawing.Point(8, 36);
            this.resultTextBox.Name = "resultTextBox";
            this.resultTextBox.ReadOnly = true;
            this.resultTextBox.Size = new System.Drawing.Size(468, 347);
            this.resultTextBox.TabIndex = 1;
            this.resultTextBox.Text = "";
            this.resultTextBox.WordWrap = false;
            // 
            // resultHeaderPanel
            // 
            this.resultHeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(255)))), ((int)(((byte)(240)))));
            this.resultHeaderPanel.Controls.Add(this.resultCountLabel);
            this.resultHeaderPanel.Controls.Add(this.resultTitleLabel);
            this.resultHeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.resultHeaderPanel.Location = new System.Drawing.Point(8, 0);
            this.resultHeaderPanel.Name = "resultHeaderPanel";
            this.resultHeaderPanel.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.resultHeaderPanel.Size = new System.Drawing.Size(468, 36);
            this.resultHeaderPanel.TabIndex = 0;
            // 
            // resultCountLabel
            // 
            this.resultCountLabel.AutoSize = true;
            this.resultCountLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.resultCountLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultCountLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(150)))), ((int)(((byte)(80)))));
            this.resultCountLabel.Location = new System.Drawing.Point(351, 5);
            this.resultCountLabel.Name = "resultCountLabel";
            this.resultCountLabel.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.resultCountLabel.Size = new System.Drawing.Size(107, 23);
            this.resultCountLabel.TabIndex = 1;
            this.resultCountLabel.Text = "✅ Kết quả: 0 dòng";
            this.resultCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // resultTitleLabel
            // 
            this.resultTitleLabel.AutoSize = true;
            this.resultTitleLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.resultTitleLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(120)))), ((int)(((byte)(60)))));
            this.resultTitleLabel.Location = new System.Drawing.Point(10, 5);
            this.resultTitleLabel.Name = "resultTitleLabel";
            this.resultTitleLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.resultTitleLabel.Size = new System.Drawing.Size(170, 22);
            this.resultTitleLabel.TabIndex = 0;
            this.resultTitleLabel.Text = "📤 KẾT QUẢ LỌC DỮ LIỆU";
            // 
            // topToolbarPanel
            // 
            this.topToolbarPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.topToolbarPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.topToolbarPanel.Controls.Add(this.lineCountPanel);
            this.topToolbarPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topToolbarPanel.Location = new System.Drawing.Point(0, 0);
            this.topToolbarPanel.Name = "topToolbarPanel";
            this.topToolbarPanel.Size = new System.Drawing.Size(484, 45);
            this.topToolbarPanel.TabIndex = 1;
            // 
            // lineCountPanel
            // 
            this.lineCountPanel.Controls.Add(this.lineCountComboBox);
            this.lineCountPanel.Controls.Add(this.lineCountLabel);
            this.lineCountPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lineCountPanel.Location = new System.Drawing.Point(0, 0);
            this.lineCountPanel.Name = "lineCountPanel";
            this.lineCountPanel.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.lineCountPanel.Size = new System.Drawing.Size(482, 43);
            this.lineCountPanel.TabIndex = 0;
            // 
            // lineCountComboBox
            // 
            this.lineCountComboBox.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.lineCountComboBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.lineCountComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lineCountComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lineCountComboBox.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lineCountComboBox.FormattingEnabled = true;
            this.lineCountComboBox.Items.AddRange(new object[] {
            "🔹 Lấy 1 kết quả (Mặc định)",
            "🔸 Lấy 2 kết quả",
            "🔶 Lấy 3 kết quả"});
            this.lineCountComboBox.Location = new System.Drawing.Point(250, 8);
            this.lineCountComboBox.Name = "lineCountComboBox";
            this.lineCountComboBox.Size = new System.Drawing.Size(220, 25);
            this.lineCountComboBox.TabIndex = 1;
            // 
            // lineCountLabel
            // 
            this.lineCountLabel.AutoSize = true;
            this.lineCountLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.lineCountLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lineCountLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lineCountLabel.Location = new System.Drawing.Point(12, 8);
            this.lineCountLabel.Name = "lineCountLabel";
            this.lineCountLabel.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lineCountLabel.Size = new System.Drawing.Size(204, 20);
            this.lineCountLabel.TabIndex = 0;
            this.lineCountLabel.Text = "⚙️ Số dòng lọc (nếu trùng key):";
            // 
            // bottomToolbarPanel
            // 
            this.bottomToolbarPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.bottomToolbarPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bottomToolbarPanel.Controls.Add(this.statusStrip);
            this.bottomToolbarPanel.Controls.Add(this.exportButton);
            this.bottomToolbarPanel.Controls.Add(this.clearButton);
            this.bottomToolbarPanel.Controls.Add(this.filterButton);
            this.bottomToolbarPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomToolbarPanel.Location = new System.Drawing.Point(0, 830);
            this.bottomToolbarPanel.Name = "bottomToolbarPanel";
            this.bottomToolbarPanel.Size = new System.Drawing.Size(484, 88);
            this.bottomToolbarPanel.TabIndex = 2;
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.Transparent;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.progressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 64);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(482, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.ForeColor = System.Drawing.Color.ForestGreen;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(467, 17);
            this.statusLabel.Spring = true;
            this.statusLabel.Text = "✔️ Sẵn sàng";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(200, 16);
            this.progressBar.Visible = false;
            // 
            // exportButton
            // 
            this.exportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.exportButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.exportButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.exportButton.FlatAppearance.BorderSize = 0;
            this.exportButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exportButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportButton.ForeColor = System.Drawing.Color.White;
            this.exportButton.Location = new System.Drawing.Point(330, 17);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(140, 38);
            this.exportButton.TabIndex = 2;
            this.exportButton.Text = "💾 XUẤT FILE TXT";
            this.exportButton.UseVisualStyleBackColor = false;
            this.exportButton.Click += new System.EventHandler(this.ExportButton_Click);
            this.exportButton.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.exportButton.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(67)))), ((int)(((byte)(54)))));
            this.clearButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.clearButton.FlatAppearance.BorderSize = 0;
            this.clearButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearButton.ForeColor = System.Drawing.Color.White;
            this.clearButton.Location = new System.Drawing.Point(169, 17);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(140, 38);
            this.clearButton.TabIndex = 1;
            this.clearButton.Text = "🗑️ XÓA TOÀN BỘ";
            this.clearButton.UseVisualStyleBackColor = false;
            this.clearButton.Click += new System.EventHandler(this.ClearButton_Click);
            this.clearButton.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.clearButton.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // filterButton
            // 
            this.filterButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.filterButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.filterButton.FlatAppearance.BorderSize = 0;
            this.filterButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.filterButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.filterButton.ForeColor = System.Drawing.Color.White;
            this.filterButton.Location = new System.Drawing.Point(13, 17);
            this.filterButton.Name = "filterButton";
            this.filterButton.Size = new System.Drawing.Size(136, 38);
            this.filterButton.TabIndex = 0;
            this.filterButton.Text = "🔍 LỌC DỮ LIỆU";
            this.filterButton.UseVisualStyleBackColor = false;
            this.filterButton.Click += new System.EventHandler(this.FilterButton_Click);
            this.filterButton.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.filterButton.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(484, 918);
            this.Controls.Add(this.mainContainer);
            this.Controls.Add(this.bottomToolbarPanel);
            this.Controls.Add(this.topToolbarPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "🔧 LỌC TỌA ĐỘ TRÙNG LẶP - Nông Văn Phấn";
            this.mainContainer.Panel1.ResumeLayout(false);
            this.mainContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainContainer)).EndInit();
            this.mainContainer.ResumeLayout(false);
            this.inputPanel.ResumeLayout(false);
            this.inputHeaderPanel.ResumeLayout(false);
            this.inputHeaderPanel.PerformLayout();
            this.resultPanel.ResumeLayout(false);
            this.resultHeaderPanel.ResumeLayout(false);
            this.resultHeaderPanel.PerformLayout();
            this.topToolbarPanel.ResumeLayout(false);
            this.lineCountPanel.ResumeLayout(false);
            this.lineCountPanel.PerformLayout();
            this.bottomToolbarPanel.ResumeLayout(false);
            this.bottomToolbarPanel.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        #region KHAI BÁO BIẾN THÀNH PHẦN GIAO DIỆN

        // Main components
        private System.Windows.Forms.SplitContainer mainContainer;
        private System.Windows.Forms.Panel inputPanel;
        private System.Windows.Forms.Panel resultPanel;
        private System.Windows.Forms.RichTextBox inputTextBox;
        private System.Windows.Forms.RichTextBox resultTextBox;

        // Header panels
        private System.Windows.Forms.Panel inputHeaderPanel;
        private System.Windows.Forms.Panel resultHeaderPanel;
        private System.Windows.Forms.Label inputTitleLabel;
        private System.Windows.Forms.Label resultTitleLabel;
        private System.Windows.Forms.Label inputCountLabel;
        private System.Windows.Forms.Label resultCountLabel;

        // Toolbar panels
        private System.Windows.Forms.Panel topToolbarPanel;
        private System.Windows.Forms.Panel bottomToolbarPanel;
        private System.Windows.Forms.Panel lineCountPanel;

        // Controls
        private System.Windows.Forms.ComboBox lineCountComboBox;
        private System.Windows.Forms.Label lineCountLabel;
        private System.Windows.Forms.Button filterButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button exportButton;

        // Status bar
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripProgressBar progressBar;

        // Thêm event handlers cho hover effects
        private void Button_MouseEnter(object sender, System.EventArgs e)
        {
            var btn = sender as System.Windows.Forms.Button;
            if (btn != null)
            {
                btn.BackColor = System.Drawing.Color.FromArgb(
                    Math.Min(255, btn.BackColor.R + 20),
                    Math.Min(255, btn.BackColor.G + 20),
                    Math.Min(255, btn.BackColor.B + 20)
                );
            }
        }

        private void Button_MouseLeave(object sender, System.EventArgs e)
        {
            if (sender == filterButton)
                filterButton.BackColor = System.Drawing.Color.FromArgb(76, 175, 80);
            else if (sender == clearButton)
                clearButton.BackColor = System.Drawing.Color.FromArgb(244, 67, 54);
            else if (sender == exportButton)
                exportButton.BackColor = System.Drawing.Color.FromArgb(0, 150, 136);
        }
        #endregion
    }
}