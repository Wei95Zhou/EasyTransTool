namespace ExtPkgUpdateTool
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.filePathSel = new System.Windows.Forms.Button();
            this.filePath = new System.Windows.Forms.Label();
            this.fielPathLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TypeSelBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.DuIpComboBox = new System.Windows.Forms.ComboBox();
            this.RuIpComboBox = new System.Windows.Forms.ComboBox();
            this.duIpDelButton = new System.Windows.Forms.Button();
            this.ruIpDelButton = new System.Windows.Forms.Button();
            this.ensfDelButton = new System.Windows.Forms.Button();
            this.EnsfComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.fsuIpDelButton = new System.Windows.Forms.Button();
            this.FsuIpComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.newVerRelPath = new System.Windows.Forms.TextBox();
            this.hintLabel = new System.Windows.Forms.Label();
            this.updateChkTimer = new System.Windows.Forms.Timer(this.components);
            this.TransModeSelBox = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.transModeSwitchButton = new System.Windows.Forms.Button();
            this.dlFileName = new System.Windows.Forms.TextBox();
            this.fileDlLabel = new System.Windows.Forms.Label();
            this.dlHintButton = new System.Windows.Forms.Button();
            this.pw123qweCheckBox = new System.Windows.Forms.CheckBox();
            this.pw123qweButton = new System.Windows.Forms.Button();
            this.fileTransProgressBar = new System.Windows.Forms.ProgressBar();
            this.fileTransBGWorker = new System.ComponentModel.BackgroundWorker();
            this.swiftCpButton0 = new System.Windows.Forms.Button();
            this.swiftCpButton1 = new System.Windows.Forms.Button();
            this.swiftCpButton2 = new System.Windows.Forms.Button();
            this.progressLabel = new System.Windows.Forms.Label();
            this.swiftCpLabel = new System.Windows.Forms.Label();
            this.swiftCpButton3 = new System.Windows.Forms.Button();
            this.SwiftCopyHelpButton = new System.Windows.Forms.Button();
            this.TransSplitButton = new wyDay.Controls.SplitButton();
            this.TransModeContexMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // filePathSel
            // 
            this.filePathSel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.filePathSel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.filePathSel.Location = new System.Drawing.Point(554, 56);
            this.filePathSel.Name = "filePathSel";
            this.filePathSel.Size = new System.Drawing.Size(81, 23);
            this.filePathSel.TabIndex = 3;
            this.filePathSel.Text = "选择文件";
            this.filePathSel.UseVisualStyleBackColor = true;
            this.filePathSel.Click += new System.EventHandler(this.filePathSel_Click);
            // 
            // filePath
            // 
            this.filePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.filePath.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.filePath.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.filePath.Location = new System.Drawing.Point(148, 56);
            this.filePath.Name = "filePath";
            this.filePath.Size = new System.Drawing.Size(400, 23);
            this.filePath.TabIndex = 0;
            this.filePath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fielPathLabel
            // 
            this.fielPathLabel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fielPathLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fielPathLabel.Location = new System.Drawing.Point(62, 57);
            this.fielPathLabel.Name = "fielPathLabel";
            this.fielPathLabel.Size = new System.Drawing.Size(80, 23);
            this.fielPathLabel.TabIndex = 2;
            this.fielPathLabel.Text = "文件路径:";
            this.fielPathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label2.Location = new System.Drawing.Point(62, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 23);
            this.label2.TabIndex = 4;
            this.label2.Text = "连接类型:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label4.Location = new System.Drawing.Point(62, 131);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 23);
            this.label4.TabIndex = 7;
            this.label4.Text = "DU IP:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TypeSelBox
            // 
            this.TypeSelBox.AllowDrop = true;
            this.TypeSelBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TypeSelBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TypeSelBox.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TypeSelBox.FormattingEnabled = true;
            this.TypeSelBox.Location = new System.Drawing.Point(148, 92);
            this.TypeSelBox.Name = "TypeSelBox";
            this.TypeSelBox.Size = new System.Drawing.Size(400, 24);
            this.TypeSelBox.TabIndex = 4;
            this.TypeSelBox.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.Location = new System.Drawing.Point(62, 205);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 23);
            this.label3.TabIndex = 10;
            this.label3.Text = "RU IP:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DuIpComboBox
            // 
            this.DuIpComboBox.AllowDrop = true;
            this.DuIpComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DuIpComboBox.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DuIpComboBox.FormattingEnabled = true;
            this.DuIpComboBox.Location = new System.Drawing.Point(148, 129);
            this.DuIpComboBox.Name = "DuIpComboBox";
            this.DuIpComboBox.Size = new System.Drawing.Size(400, 24);
            this.DuIpComboBox.TabIndex = 5;
            // 
            // RuIpComboBox
            // 
            this.RuIpComboBox.AllowDrop = true;
            this.RuIpComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RuIpComboBox.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.RuIpComboBox.FormattingEnabled = true;
            this.RuIpComboBox.Location = new System.Drawing.Point(148, 203);
            this.RuIpComboBox.Name = "RuIpComboBox";
            this.RuIpComboBox.Size = new System.Drawing.Size(400, 24);
            this.RuIpComboBox.TabIndex = 7;
            // 
            // duIpDelButton
            // 
            this.duIpDelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.duIpDelButton.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.duIpDelButton.Location = new System.Drawing.Point(554, 130);
            this.duIpDelButton.Name = "duIpDelButton";
            this.duIpDelButton.Size = new System.Drawing.Size(81, 23);
            this.duIpDelButton.TabIndex = 13;
            this.duIpDelButton.TabStop = false;
            this.duIpDelButton.Text = "删除";
            this.duIpDelButton.UseVisualStyleBackColor = true;
            this.duIpDelButton.Click += new System.EventHandler(this.duIpDelButton_Click);
            // 
            // ruIpDelButton
            // 
            this.ruIpDelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ruIpDelButton.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ruIpDelButton.Location = new System.Drawing.Point(554, 204);
            this.ruIpDelButton.Name = "ruIpDelButton";
            this.ruIpDelButton.Size = new System.Drawing.Size(81, 23);
            this.ruIpDelButton.TabIndex = 11;
            this.ruIpDelButton.TabStop = false;
            this.ruIpDelButton.Text = "删除";
            this.ruIpDelButton.UseVisualStyleBackColor = true;
            this.ruIpDelButton.Click += new System.EventHandler(this.ruIpDelButton_Click);
            // 
            // ensfDelButton
            // 
            this.ensfDelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ensfDelButton.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ensfDelButton.Location = new System.Drawing.Point(554, 241);
            this.ensfDelButton.Name = "ensfDelButton";
            this.ensfDelButton.Size = new System.Drawing.Size(81, 23);
            this.ensfDelButton.TabIndex = 10;
            this.ensfDelButton.TabStop = false;
            this.ensfDelButton.Text = "删除";
            this.ensfDelButton.UseVisualStyleBackColor = true;
            this.ensfDelButton.Click += new System.EventHandler(this.ensfDelButton_Click);
            // 
            // EnsfComboBox
            // 
            this.EnsfComboBox.AllowDrop = true;
            this.EnsfComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EnsfComboBox.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnsfComboBox.FormattingEnabled = true;
            this.EnsfComboBox.Location = new System.Drawing.Point(148, 240);
            this.EnsfComboBox.Name = "EnsfComboBox";
            this.EnsfComboBox.Size = new System.Drawing.Size(400, 24);
            this.EnsfComboBox.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label5.Location = new System.Drawing.Point(62, 242);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 23);
            this.label5.TabIndex = 13;
            this.label5.Text = "网络设备:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fsuIpDelButton
            // 
            this.fsuIpDelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fsuIpDelButton.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fsuIpDelButton.Location = new System.Drawing.Point(554, 167);
            this.fsuIpDelButton.Name = "fsuIpDelButton";
            this.fsuIpDelButton.Size = new System.Drawing.Size(81, 23);
            this.fsuIpDelButton.TabIndex = 12;
            this.fsuIpDelButton.TabStop = false;
            this.fsuIpDelButton.Text = "删除";
            this.fsuIpDelButton.UseVisualStyleBackColor = true;
            this.fsuIpDelButton.Click += new System.EventHandler(this.fsuIpDelButton_Click);
            // 
            // FsuIpComboBox
            // 
            this.FsuIpComboBox.AllowDrop = true;
            this.FsuIpComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FsuIpComboBox.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FsuIpComboBox.FormattingEnabled = true;
            this.FsuIpComboBox.Location = new System.Drawing.Point(148, 166);
            this.FsuIpComboBox.Name = "FsuIpComboBox";
            this.FsuIpComboBox.Size = new System.Drawing.Size(400, 24);
            this.FsuIpComboBox.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label6.Location = new System.Drawing.Point(62, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 23);
            this.label6.TabIndex = 16;
            this.label6.Text = "FSU IP:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // newVerRelPath
            // 
            this.newVerRelPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.newVerRelPath.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.newVerRelPath.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.newVerRelPath.Location = new System.Drawing.Point(194, 444);
            this.newVerRelPath.Name = "newVerRelPath";
            this.newVerRelPath.ReadOnly = true;
            this.newVerRelPath.Size = new System.Drawing.Size(495, 14);
            this.newVerRelPath.TabIndex = 11;
            this.newVerRelPath.TabStop = false;
            this.newVerRelPath.Text = "-";
            this.newVerRelPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // hintLabel
            // 
            this.hintLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.hintLabel.AutoSize = true;
            this.hintLabel.Location = new System.Drawing.Point(465, 428);
            this.hintLabel.Name = "hintLabel";
            this.hintLabel.Size = new System.Drawing.Size(227, 12);
            this.hintLabel.TabIndex = 0;
            this.hintLabel.Text = "发现Bug请告知FW周唯~ 新版本发布位置：";
            // 
            // updateChkTimer
            // 
            this.updateChkTimer.Interval = 5000;
            this.updateChkTimer.Tick += new System.EventHandler(this.UpdateCheckMenuItem_Click);
            // 
            // TransModeSelBox
            // 
            this.TransModeSelBox.AllowDrop = true;
            this.TransModeSelBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TransModeSelBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TransModeSelBox.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TransModeSelBox.FormattingEnabled = true;
            this.TransModeSelBox.Location = new System.Drawing.Point(148, 19);
            this.TransModeSelBox.Name = "TransModeSelBox";
            this.TransModeSelBox.Size = new System.Drawing.Size(400, 24);
            this.TransModeSelBox.TabIndex = 1;
            this.TransModeSelBox.SelectedIndexChanged += new System.EventHandler(this.TransModeSelBox_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label7.Location = new System.Drawing.Point(42, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 23);
            this.label7.TabIndex = 10;
            this.label7.Text = "传输方向:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // transModeSwitchButton
            // 
            this.transModeSwitchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.transModeSwitchButton.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.transModeSwitchButton.Location = new System.Drawing.Point(554, 19);
            this.transModeSwitchButton.Name = "transModeSwitchButton";
            this.transModeSwitchButton.Size = new System.Drawing.Size(81, 23);
            this.transModeSwitchButton.TabIndex = 2;
            this.transModeSwitchButton.Text = "切换";
            this.transModeSwitchButton.UseVisualStyleBackColor = true;
            this.transModeSwitchButton.Click += new System.EventHandler(this.transModeSwitchButton_Click);
            // 
            // dlFileName
            // 
            this.dlFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dlFileName.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dlFileName.Location = new System.Drawing.Point(148, 277);
            this.dlFileName.Name = "dlFileName";
            this.dlFileName.Size = new System.Drawing.Size(376, 24);
            this.dlFileName.TabIndex = 9;
            // 
            // fileDlLabel
            // 
            this.fileDlLabel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fileDlLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.fileDlLabel.Location = new System.Drawing.Point(62, 279);
            this.fileDlLabel.Name = "fileDlLabel";
            this.fileDlLabel.Size = new System.Drawing.Size(80, 23);
            this.fileDlLabel.TabIndex = 24;
            this.fileDlLabel.Text = "获取路径:";
            this.fileDlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dlHintButton
            // 
            this.dlHintButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dlHintButton.Location = new System.Drawing.Point(524, 277);
            this.dlHintButton.Name = "dlHintButton";
            this.dlHintButton.Size = new System.Drawing.Size(24, 24);
            this.dlHintButton.TabIndex = 0;
            this.dlHintButton.TabStop = false;
            this.dlHintButton.Text = "?";
            this.dlHintButton.UseVisualStyleBackColor = true;
            this.dlHintButton.Click += new System.EventHandler(this.dlHintButton_Click);
            // 
            // pw123qweCheckBox
            // 
            this.pw123qweCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pw123qweCheckBox.AutoSize = true;
            this.pw123qweCheckBox.Location = new System.Drawing.Point(4, 444);
            this.pw123qweCheckBox.Name = "pw123qweCheckBox";
            this.pw123qweCheckBox.Size = new System.Drawing.Size(84, 16);
            this.pw123qweCheckBox.TabIndex = 25;
            this.pw123qweCheckBox.Text = "老密码机型";
            this.pw123qweCheckBox.UseVisualStyleBackColor = true;
            this.pw123qweCheckBox.CheckedChanged += new System.EventHandler(this.pw123qweCheckBox_CheckedChanged);
            // 
            // pw123qweButton
            // 
            this.pw123qweButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pw123qweButton.Location = new System.Drawing.Point(85, 439);
            this.pw123qweButton.Name = "pw123qweButton";
            this.pw123qweButton.Size = new System.Drawing.Size(24, 24);
            this.pw123qweButton.TabIndex = 10;
            this.pw123qweButton.Text = "?";
            this.pw123qweButton.UseVisualStyleBackColor = true;
            this.pw123qweButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // fileTransProgressBar
            // 
            this.fileTransProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileTransProgressBar.Location = new System.Drawing.Point(148, 316);
            this.fileTransProgressBar.Name = "fileTransProgressBar";
            this.fileTransProgressBar.Size = new System.Drawing.Size(400, 23);
            this.fileTransProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.fileTransProgressBar.TabIndex = 26;
            // 
            // fileTransBGWorker
            // 
            this.fileTransBGWorker.WorkerReportsProgress = true;
            this.fileTransBGWorker.WorkerSupportsCancellation = true;
            this.fileTransBGWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.fileTransBGWorker_DoWork);
            this.fileTransBGWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.fileTransBGWorker_ProgressChanged);
            this.fileTransBGWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.fileTransBGWorker_RunWorkerCompleted);
            // 
            // swiftCpButton0
            // 
            this.swiftCpButton0.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.swiftCpButton0.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.swiftCpButton0.Location = new System.Drawing.Point(65, 384);
            this.swiftCpButton0.Name = "swiftCpButton0";
            this.swiftCpButton0.Size = new System.Drawing.Size(124, 31);
            this.swiftCpButton0.TabIndex = 27;
            this.swiftCpButton0.UseVisualStyleBackColor = true;
            this.swiftCpButton0.Click += new System.EventHandler(this.swiftCpButton0_Click);
            // 
            // swiftCpButton1
            // 
            this.swiftCpButton1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.swiftCpButton1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.swiftCpButton1.Location = new System.Drawing.Point(213, 384);
            this.swiftCpButton1.Name = "swiftCpButton1";
            this.swiftCpButton1.Size = new System.Drawing.Size(124, 31);
            this.swiftCpButton1.TabIndex = 28;
            this.swiftCpButton1.UseVisualStyleBackColor = true;
            this.swiftCpButton1.Click += new System.EventHandler(this.swiftCpButton1_Click);
            // 
            // swiftCpButton2
            // 
            this.swiftCpButton2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.swiftCpButton2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.swiftCpButton2.Location = new System.Drawing.Point(361, 384);
            this.swiftCpButton2.Name = "swiftCpButton2";
            this.swiftCpButton2.Size = new System.Drawing.Size(124, 31);
            this.swiftCpButton2.TabIndex = 29;
            this.swiftCpButton2.UseVisualStyleBackColor = true;
            this.swiftCpButton2.Click += new System.EventHandler(this.swiftCpButton2_Click);
            // 
            // progressLabel
            // 
            this.progressLabel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.progressLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.progressLabel.Location = new System.Drawing.Point(62, 316);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(80, 23);
            this.progressLabel.TabIndex = 30;
            this.progressLabel.Text = "传输进度:";
            this.progressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // swiftCpLabel
            // 
            this.swiftCpLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.swiftCpLabel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.swiftCpLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.swiftCpLabel.Location = new System.Drawing.Point(27, 354);
            this.swiftCpLabel.Name = "swiftCpLabel";
            this.swiftCpLabel.Size = new System.Drawing.Size(649, 23);
            this.swiftCpLabel.TabIndex = 31;
            this.swiftCpLabel.Text = "=============================👇快捷命令复制👇=============================";
            this.swiftCpLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.swiftCpLabel.Click += new System.EventHandler(this.swiftCpLabel_Click);
            // 
            // swiftCpButton3
            // 
            this.swiftCpButton3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.swiftCpButton3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.swiftCpButton3.Location = new System.Drawing.Point(509, 384);
            this.swiftCpButton3.Name = "swiftCpButton3";
            this.swiftCpButton3.Size = new System.Drawing.Size(124, 31);
            this.swiftCpButton3.TabIndex = 32;
            this.swiftCpButton3.UseVisualStyleBackColor = true;
            this.swiftCpButton3.Click += new System.EventHandler(this.swiftCpButton3_Click);
            // 
            // SwiftCopyHelpButton
            // 
            this.SwiftCopyHelpButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.SwiftCopyHelpButton.Location = new System.Drawing.Point(649, 353);
            this.SwiftCopyHelpButton.Name = "SwiftCopyHelpButton";
            this.SwiftCopyHelpButton.Size = new System.Drawing.Size(24, 24);
            this.SwiftCopyHelpButton.TabIndex = 33;
            this.SwiftCopyHelpButton.TabStop = false;
            this.SwiftCopyHelpButton.Text = "?";
            this.SwiftCopyHelpButton.UseVisualStyleBackColor = true;
            this.SwiftCopyHelpButton.Click += new System.EventHandler(this.SwiftCopyHelpButton_Click);
            // 
            // TransSplitButton
            // 
            this.TransSplitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TransSplitButton.AutoSize = true;
            this.TransSplitButton.ContextMenuStrip = this.TransModeContexMenuStrip;
            this.TransSplitButton.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TransSplitButton.Location = new System.Drawing.Point(554, 278);
            this.TransSplitButton.Name = "TransSplitButton";
            this.TransSplitButton.Size = new System.Drawing.Size(98, 23);
            this.TransSplitButton.SplitMenuStrip = this.TransModeContexMenuStrip;
            this.TransSplitButton.TabIndex = 35;
            this.TransSplitButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.TransSplitButton.UseVisualStyleBackColor = true;
            this.TransSplitButton.Click += new System.EventHandler(this.TransSplitButton_Click);
            // 
            // TransModeContexMenuStrip
            // 
            this.TransModeContexMenuStrip.Name = "TransModeContexMenuStrip";
            this.TransModeContexMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 461);
            this.Controls.Add(this.TransSplitButton);
            this.Controls.Add(this.SwiftCopyHelpButton);
            this.Controls.Add(this.swiftCpButton3);
            this.Controls.Add(this.swiftCpLabel);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.swiftCpButton2);
            this.Controls.Add(this.swiftCpButton1);
            this.Controls.Add(this.swiftCpButton0);
            this.Controls.Add(this.fileTransProgressBar);
            this.Controls.Add(this.pw123qweButton);
            this.Controls.Add(this.pw123qweCheckBox);
            this.Controls.Add(this.dlHintButton);
            this.Controls.Add(this.fileDlLabel);
            this.Controls.Add(this.dlFileName);
            this.Controls.Add(this.transModeSwitchButton);
            this.Controls.Add(this.TransModeSelBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.hintLabel);
            this.Controls.Add(this.newVerRelPath);
            this.Controls.Add(this.fsuIpDelButton);
            this.Controls.Add(this.FsuIpComboBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ensfDelButton);
            this.Controls.Add(this.EnsfComboBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ruIpDelButton);
            this.Controls.Add(this.duIpDelButton);
            this.Controls.Add(this.RuIpComboBox);
            this.Controls.Add(this.DuIpComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TypeSelBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.fielPathLabel);
            this.Controls.Add(this.filePath);
            this.Controls.Add(this.filePathSel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(720, 500);
            this.Name = "Form1";
            this.Text = "EasyTransTool(Developed by wei.zhou@FW)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button filePathSel;
        private System.Windows.Forms.Label filePath;
        private System.Windows.Forms.Label fielPathLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox TypeSelBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox DuIpComboBox;
        private System.Windows.Forms.ComboBox RuIpComboBox;
        private System.Windows.Forms.Button duIpDelButton;
        private System.Windows.Forms.Button ruIpDelButton;
        private System.Windows.Forms.Button ensfDelButton;
        private System.Windows.Forms.ComboBox EnsfComboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button fsuIpDelButton;
        private System.Windows.Forms.ComboBox FsuIpComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox newVerRelPath;
        private System.Windows.Forms.Label hintLabel;
        private System.Windows.Forms.Timer updateChkTimer;
        private System.Windows.Forms.ComboBox TransModeSelBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button transModeSwitchButton;
        private System.Windows.Forms.TextBox dlFileName;
        private System.Windows.Forms.Label fileDlLabel;
        private System.Windows.Forms.Button dlHintButton;
        private System.Windows.Forms.CheckBox pw123qweCheckBox;
        private System.Windows.Forms.Button pw123qweButton;
        private System.Windows.Forms.ProgressBar fileTransProgressBar;
        private System.ComponentModel.BackgroundWorker fileTransBGWorker;
        private System.Windows.Forms.Button swiftCpButton0;
        private System.Windows.Forms.Button swiftCpButton1;
        private System.Windows.Forms.Button swiftCpButton2;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.Label swiftCpLabel;
        private System.Windows.Forms.Button swiftCpButton3;
        private System.Windows.Forms.Button SwiftCopyHelpButton;
        private wyDay.Controls.SplitButton TransSplitButton;
        private System.Windows.Forms.ContextMenuStrip TransModeContexMenuStrip;
    }
}

