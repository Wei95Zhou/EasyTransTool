using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Renci.SshNet;
using System.Net;
using IPAddressManagement;
using FileManagement;
using RemoteManagement;
using UserManagement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Text.RegularExpressions;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.Reflection;

namespace ExtPkgUpdateTool
{
    public partial class Form1 : Form
    {
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem showMenuItem;
        private ToolStripMenuItem updateMenuItem;
        private ToolStripMenuItem exitMenuItem;
        private DateTime lastClosingTime;
        private string sRelVer = "1.2.6";

        IPAddressOp duIpOp = new IPAddressOp("DuIp", "./config/IpDataSet.cfg");
        IPAddressOp ruIpOp = new IPAddressOp("RuIp", "./config/IpDataSet.cfg");
        IPAddressOp ensfOp = new IPAddressOp("ensf", "./config/IpDataSet.cfg");
        IPAddressOp fsuIpOp = new IPAddressOp("FsuIp", "./config/IpDataSet.cfg");
        IPAddressOp serverIpOp = new IPAddressOp("ServerIp", "./config/IpDataSet.cfg");
        DevTypeOp devTypeOp = new DevTypeOp("./config/DevType.cfg");
        FilePathOp filePathOp = new FilePathOp("FilePath", "./config/Path.cfg");
        FilePathOp newVerPathOp = new FilePathOp("NewVerPath", "./config/Path.cfg");
        FilePathOp newVerChkPathOp = new FilePathOp("NewVerChkPath", "./config/Path.cfg");
        UserManager usrMng = new UserManager("./config/UserMng.cfg");
        FileOp logFile = new FileOp("./log/Script.log");
        //MainForm mainForm = new MainForm();
        public Form1()
        {
            InitializeComponent();
            InitializeSystemTray();
            lastClosingTime = DateTime.Now;
            this.Text = "EasyTransTool-V" + sRelVer + "(Developed by wei.zhou@FW)";
            newVerRelPath.Text = newVerPathOp.GetPath();

            filePath.Text = filePathOp.GetPath();

            TypeSelBox.Items.Add("vDU-ORU");
            TypeSelBox.Items.Add("CDU-RU");
            TypeSelBox.Items.Add("vDU-FSU-RU");
            TypeSelBox.SelectedIndex = devTypeOp.GetLinkType();

            ComboBox_Refresh(DuIpComboBox, duIpOp, duIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, duIpDelButton);
            ComboBox_Refresh(RuIpComboBox, ruIpOp, ruIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, ruIpDelButton);
            if (string.Equals("vDU-FSU-RU", TypeSelBox.Text))
            {
                ComboBox_Refresh(FsuIpComboBox, fsuIpOp, fsuIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, fsuIpDelButton);
                ComboBox_Disable(EnsfComboBox, ensfDelButton);
            }
            else if(string.Equals("CDU-RU", TypeSelBox.Text))
            {
                ComboBox_Disable(FsuIpComboBox, fsuIpDelButton);
                ComboBox_Disable(EnsfComboBox, ensfDelButton);
            }
            else if (string.Equals("vDU-ORU", TypeSelBox.Text))
            {
                ComboBox_Disable(FsuIpComboBox, fsuIpDelButton);
                Console.WriteLine("ensfCnt = " + ensfOp.GetIPAddressCount(TypeSelBox.Text));
                ComboBox_Refresh(EnsfComboBox, ensfOp, ensfOp.GetIPAddressCount(TypeSelBox.Text) - 1, ensfDelButton);
            }
            else
            {
                ComboBox_Refresh(FsuIpComboBox, fsuIpOp, fsuIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, fsuIpDelButton);
                ComboBox_Refresh(EnsfComboBox, ensfOp, ensfOp.GetIPAddressCount(TypeSelBox.Text) - 1, ensfDelButton);
            }
        }

        private void filePathSel_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "选择要上传或升级的文件";
            fileDialog.Filter = "所有文件|*.*|EXT Files(*.EXT)|*.EXT";
            fileDialog.InitialDirectory = filePathOp.GetPath();

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = fileDialog.FileName;
                if(filePathOp.FileNameValid(selectedFilePath))
                {
                    filePath.Text = selectedFilePath;
                    filePathOp.SavePath(selectedFilePath);
                }
                else
                {
                    MessageBox.Show("文件名存在不支持的字符，仅支持A~Za~z-_.:");
                }
            }
        }

        static string GetCommand(string line)
        {
            int startIndex = line.IndexOf('\'') + 1;
            int endIndex = line.LastIndexOf('\'');
            return line.Substring(startIndex, endIndex - startIndex);
        }

        static string GetExpectedString(string line)
        {
            int startIndex = line.IndexOf('\'') + 1;
            int endIndex = line.LastIndexOf('\'');
            return line.Substring(startIndex, endIndex - startIndex);
        }

        string scriptPath;

        private void uploadButton_Click(object sender, EventArgs e)
        {
            if (string.Equals("vDU-FSU-RU", TypeSelBox.Text))
            {
                scriptPath = "./script/vduFsuRuUpload.script";
            }
            else if (string.Equals("CDU-RU", TypeSelBox.Text))
            {
                scriptPath = "./script/cduRuUpload.script";
            }
            else if (string.Equals("vDU-ORU", TypeSelBox.Text))
            {
                scriptPath = "./script/vduOruUpload.script";
            }
            else
            {
                MessageBox.Show("无效的连接类型！");
                return;
            }
            upload_update_Core(sender, e);
            //string updateFile = "/tmp/" + filePathOp.getSelFileName();
            if (string.Equals("vDU-FSU-RU", TypeSelBox.Text))
            {
                Clipboard.SetText("SWM_PkgUpdate " + "\"/tmp/" + filePathOp.getSelFileName() + "\"");
            }
            else if (string.Equals("CDU-RU", TypeSelBox.Text))
            {
                Clipboard.SetText("SWM_PkgUpdate " + "\"/tmp/" + filePathOp.getSelFileName() + "\"");
            }
            else if (string.Equals("vDU-ORU", TypeSelBox.Text))
            {
                Clipboard.SetText("SWM_OranPkgUpdate " + "\"/tmp/" + filePathOp.getSelFileName() + "\"");
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("功能开发中！");
            return;
            if (string.Equals("vDU-FSU-RU", TypeSelBox.Text))
            {
                scriptPath = "./script/vduFsuRuUpdate.script";
            }
            else if (string.Equals("CDU-RU", TypeSelBox.Text))
            {
                scriptPath = "./script/cduRuUpdate.script";
            }
            else if (string.Equals("vDU-ORU", TypeSelBox.Text))
            {
                scriptPath = "./script/vduOruUpdate.script";
            }
            else
            {
                MessageBox.Show("无效的连接类型！");
                return;
            }
            upload_update_Core(sender, e);
        }

        private void upload_update_Core(object sender, EventArgs e)
        {
            // Save the IP address and refresh the ComboBox
            string duIpAddress = DuIpComboBox.Text;
            string ruIpAddress = RuIpComboBox.Text;
            string fsuIpAddress = FsuIpComboBox.Text;
            string ensfAddress = EnsfComboBox.Text;

            if (!File.Exists(filePathOp.GetPath()))
            {
                MessageBox.Show("本地文件不存在！");
                return;
            }
            if (!File.Exists(scriptPath))
            {
                MessageBox.Show("升级脚本不存在！");
                return;
            }
            if (!duIpOp.IsIPAddressValid(duIpAddress))
            {
                MessageBox.Show("DU IP 地址无效！");
                return;
            }
            else if (!ruIpOp.IsIPAddressValid(ruIpAddress))
            {
                MessageBox.Show("RU IP 地址无效！");
                return;
            }
            else if ((!string.Equals("", fsuIpAddress)) && (!fsuIpOp.IsIPAddressValid(fsuIpAddress)))
            {
                MessageBox.Show("FSU IP 地址无效！");
                return;
            }
            else
            {
                duIpOp.SaveIPAddressToFile(TypeSelBox.Text, duIpAddress);
                ruIpOp.SaveIPAddressToFile(TypeSelBox.Text, ruIpAddress);
                if (!string.Equals("", fsuIpAddress))
                {
                    fsuIpOp.SaveIPAddressToFile(TypeSelBox.Text, fsuIpAddress);
                    ComboBox_Refresh(FsuIpComboBox, fsuIpOp, fsuIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, fsuIpDelButton);
                }
                if (!string.Equals("", ensfAddress))
                {
                    ensfOp.SaveIPAddressToFile(TypeSelBox.Text, ensfAddress);
                    ComboBox_Refresh(EnsfComboBox, ensfOp, ensfOp.GetIPAddressCount(TypeSelBox.Text) - 1, ensfDelButton);
                }
                devTypeOp.SaveDevType(TypeSelBox.SelectedIndex);
                ComboBox_Refresh(DuIpComboBox, duIpOp, duIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, duIpDelButton);
                ComboBox_Refresh(RuIpComboBox, ruIpOp, ruIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, ruIpDelButton);
            }
            
            // Start update procedure
            // 1.Put file to 116.8 server
            var testUser = usrMng.GetUserByType("testUser");
            var user1168fw = usrMng.GetUserByType("1168fw");

            //SshOp serverSshOp = new SshOp(serverIpOp.GetLastIpAddress(TypeSelBox.Text), testUser.Username, testUser.Password);
            SshOp serverSshOp = new SshOp(serverIpOp.GetLastIpAddress(TypeSelBox.Text), user1168fw.Username, user1168fw.Password);

            //SftpOp serverSftpOp = new SftpOp(serverIpOp.GetLastIpAddress(TypeSelBox.Text), testUser.Username, testUser.Password);
            SftpOp serverSftpOp = new SftpOp(serverIpOp.GetLastIpAddress(TypeSelBox.Text), user1168fw.Username, user1168fw.Password);

            //string filePath = "/home/zw/" + Environment.UserName + "/";
            string filePath = "/home/" + user1168fw.Username + "/tmp/" + Environment.UserName + "/";
            string fileTempName = filePathOp.getSelFileName() + Regex.Replace(DateTime.Now.TimeOfDay.ToString(), @"[^\d]", "");

            //每一句命令都需要检查返回值
            if (true == serverSshOp.Connect())
            {
                serverSshOp.RunCommand("mkdir -p " + filePath);
                serverSshOp.Disconnect();
            }
            if (true == serverSftpOp.Connect())
            {
                serverSftpOp.UploadFile(filePathOp.GetPath(), filePath + fileTempName);
                serverSftpOp.Disconnect();
            }
            if (true == serverSshOp.Connect())
            {
                if (true == serverSshOp.StartShell())
                {
                    try
                    {
                        using (StreamReader reader = new StreamReader(scriptPath))
                        {
                            string line;
                            logFile.ClearFile();
                            while ((line = reader.ReadLine()) != null)
                            {
                                line = line.Replace("USER_PATH", Environment.UserName);
                                line = line.Replace("DU_IP_ADDR", duIpAddress);
                                line = line.Replace("ENS_F", ensfAddress);
                                line = line.Replace("FSU_IP_ADDR", fsuIpAddress);
                                line = line.Replace("RU_IP_ADDR", ruIpAddress);
                                line = line.Replace("FILE_TRANS_NAME", fileTempName);
                                line = line.Replace("FILE_NAME", filePathOp.getSelFileName());
                                CmdWindow.Text = line;
                                CmdWindow.Refresh();
                                Console.WriteLine(line);
                                string timestamp = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]");
                                logFile.AppendToFile(timestamp + line + "\n");
                                if (line.StartsWith("sendln "))
                                {
                                    string command = GetCommand(line);
                                    serverSshOp.ExecuteCommand(command);
                                }
                                else if (line.StartsWith("wait "))
                                {
                                    string expectedString = GetExpectedString(line);
                                    serverSshOp.WaitForOutput(expectedString);
                                }
                                else if (line.StartsWith("wait_timer "))
                                {
                                    string expectedString = GetExpectedString(line);
                                    if (false == serverSshOp.WaitForOutput_Timer(expectedString))
                                    {
                                        MessageBox.Show("上传失败，请检查IP配置！");
                                        CmdWindow.Text = "";
                                        break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid command: " + line);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to read file: " + ex.Message);
                    }
                }
                serverSshOp.RunCommand("rm " + filePath + fileTempName);
                serverSshOp.Disconnect();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.Equals("vDU-FSU-RU", TypeSelBox.Text))
            {
                ComboBox_Refresh(DuIpComboBox, duIpOp, duIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, duIpDelButton);
                ComboBox_Refresh(RuIpComboBox, ruIpOp, ruIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, ruIpDelButton);
                ComboBox_Refresh(FsuIpComboBox, fsuIpOp, fsuIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, fsuIpDelButton);
                ComboBox_Disable(EnsfComboBox, ensfDelButton);
            }
            else if (string.Equals("CDU-RU", TypeSelBox.Text))
            {
                ComboBox_Refresh(DuIpComboBox, duIpOp, duIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, duIpDelButton);
                ComboBox_Refresh(RuIpComboBox, ruIpOp, ruIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, ruIpDelButton);
                ComboBox_Disable(FsuIpComboBox, fsuIpDelButton);
                ComboBox_Disable(EnsfComboBox, ensfDelButton);
            }
            else if (string.Equals("vDU-ORU", TypeSelBox.Text))
            {
                ComboBox_Refresh(DuIpComboBox, duIpOp, duIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, duIpDelButton);
                ComboBox_Refresh(RuIpComboBox, ruIpOp, ruIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, ruIpDelButton);
                ComboBox_Refresh(EnsfComboBox, ensfOp, ensfOp.GetIPAddressCount(TypeSelBox.Text) - 1, ensfDelButton);
                ComboBox_Disable(FsuIpComboBox, fsuIpDelButton);
            }
            else
            {
                ComboBox_Refresh(DuIpComboBox, duIpOp, duIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, duIpDelButton);
                ComboBox_Refresh(RuIpComboBox, ruIpOp, ruIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, ruIpDelButton);
                ComboBox_Refresh(FsuIpComboBox, fsuIpOp, fsuIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, fsuIpDelButton);
                ComboBox_Refresh(EnsfComboBox, ensfOp, ensfOp.GetIPAddressCount(TypeSelBox.Text) - 1, ensfDelButton);
            }
        }

        private void ComboBox_Refresh(System.Windows.Forms.ComboBox comboBox, IPAddressOp ipAddrOp, int defaultIpIndex, System.Windows.Forms.Button button)
        {
            button.Enabled = true;
            comboBox.Enabled = true;
            comboBox.Items.Clear();
            if (ipAddrOp.GetIPAddressCount(TypeSelBox.Text) == 0)
            {
                comboBox.Items.Add(ipAddrOp.GetLastIpAddress(TypeSelBox.Text));
                comboBox.SelectedIndex = 0;
                return;
            }
            for (int index = 0; index < ipAddrOp.GetIPAddressCount(TypeSelBox.Text); index++)
            {
                comboBox.Items.Add(ipAddrOp.GetIPAddressAtIndex(TypeSelBox.Text, index));
            }
            if(defaultIpIndex >= ipAddrOp.GetIPAddressCount(TypeSelBox.Text))
                defaultIpIndex = ipAddrOp.GetIPAddressCount(TypeSelBox.Text) - 1;
            comboBox.SelectedItem = ipAddrOp.GetIPAddressAtIndex(TypeSelBox.Text, defaultIpIndex);
        }

        private void ComboBox_Disable(System.Windows.Forms.ComboBox comboBox, System.Windows.Forms.Button button)
        {
            comboBox.Items.Clear();
            comboBox.Text = string.Empty;
            comboBox.Enabled = false;
            button.Enabled = false;
        }

        private void duIpDelButton_Click(object sender, EventArgs e)
        {
            duIpOp.DeleteIPAddress(DuIpComboBox.Text);
            ComboBox_Refresh(DuIpComboBox, duIpOp, DuIpComboBox.SelectedIndex, duIpDelButton);
        }

        private void ruIpDelButton_Click(object sender, EventArgs e)
        {
            ruIpOp.DeleteIPAddress(RuIpComboBox.Text);
            ComboBox_Refresh(RuIpComboBox, ruIpOp, RuIpComboBox.SelectedIndex, ruIpDelButton);
        }

        private void ensfDelButton_Click(object sender, EventArgs e)
        {
            ensfOp.DeleteIPAddress(EnsfComboBox.Text);
            ComboBox_Refresh(EnsfComboBox, ensfOp, EnsfComboBox.SelectedIndex, ensfDelButton);
        }

        private void fsuIpDelButton_Click(object sender, EventArgs e)
        {
            fsuIpOp.DeleteIPAddress(FsuIpComboBox.Text);
            ComboBox_Refresh(FsuIpComboBox, fsuIpOp, FsuIpComboBox.SelectedIndex, fsuIpDelButton);
        }

        private void DuIpComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void RuIpComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void InitializeSystemTray()
        {
            // 创建 NotifyIcon
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new Icon("Icon.ico"); // 设置状态栏图标（自定义图标文件）
            notifyIcon.Text = "EasyTransTool"; // 设置状态栏提示文本

            // 创建 ContextMenuStrip
            contextMenu = new ContextMenuStrip();

            // 创建 Show 菜单项
            showMenuItem = new ToolStripMenuItem();
            showMenuItem.Text = "打开程序";
            showMenuItem.Click += ShowMenuItem_Click;

            // 创建 Show 菜单项
            updateMenuItem = new ToolStripMenuItem();
            updateMenuItem.Text = "检查更新";
            updateMenuItem.Click += UpdateCheckMenuItem_Click;

            // 创建 Exit 菜单项
            exitMenuItem = new ToolStripMenuItem();
            exitMenuItem.Text = "退出";
            exitMenuItem.Click += ExitMenuItem_Click;

            // 添加菜单项到 ContextMenuStrip
            contextMenu.Items.Add(showMenuItem);
            contextMenu.Items.Add(updateMenuItem);
            contextMenu.Items.Add(exitMenuItem);

            // 将 ContextMenuStrip 与 NotifyIcon 关联
            notifyIcon.ContextMenuStrip = contextMenu;

            // 将窗体的最小化操作转换为最小化到系统托盘
            this.FormClosing += MainForm_FormClosing;

            // 添加双击事件处理程序
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            // 显示 NotifyIcon
            notifyIcon.Visible = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 阻止窗体关闭，最小化到系统托盘
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;

                TimeSpan timeSinceLastClosing = DateTime.Now - lastClosingTime;
                // 如果时间差超过一天（大于等于24小时）
                if (timeSinceLastClosing.TotalHours >= 24)
                {
                    if (true == NewVerCheck())
                    {
                        MessageBox.Show("有新版本！请去主界面下方位置获取~");
                    }
                    // 更新上次调用时间为当前时间
                    lastClosingTime = DateTime.Now;
                }
            }
        }

        private void ShowMenuItem_Click(object sender, EventArgs e)
        {
            // 双击状态栏图标或点击 Show 菜单项时，还原窗体并显示
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Show();
        }

        private void UpdateCheckMenuItem_Click(object sender, EventArgs e)
        {
            /*this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Show();*/

            if (true == NewVerCheck())
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                this.Show();
                MessageBox.Show("有新版本！请去主界面下方位置获取~");
            }
            else
            {
                MessageBox.Show("当前版本已是最新！");
            }
        }

        private bool NewVerCheck()
        {
            bool bNewVerRel = false;
            var user1168fw = usrMng.GetUserByType("1168fw");
            var testUser = usrMng.GetUserByType("testUser");
            SshOp serverSshOp = new SshOp(serverIpOp.GetLastIpAddress(TypeSelBox.Text), user1168fw.Username, user1168fw.Password);
            //SshOp serverSshOp = new SshOp(serverIpOp.GetLastIpAddress(TypeSelBox.Text), testUser.Username, testUser.Password);
            if (true == serverSshOp.Connect())
            {
                string sLatestVer = serverSshOp.RunCommand("cat " + newVerChkPathOp.GetPath());
                if ((!string.IsNullOrEmpty(sLatestVer)) && !(0 == string.Compare(sRelVer, sLatestVer.Substring(0, sRelVer.Length))))
                {
                    bNewVerRel = true;
                }
                else
                {
                    bNewVerRel = false;
                }
                serverSshOp.Disconnect();
            }
            return bNewVerRel;
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            // 点击退出菜单项时，关闭应用程序
            notifyIcon.Visible = false;
            Application.Exit();
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            // 双击系统托盘图标时，显示窗口
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.TopMost = true;
            this.Show();
            this.Activate();
            this.TopMost = false;
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            // 监听窗体大小变化，最小化窗体时隐藏到系统托盘
            if (this.WindowState == FormWindowState.Minimized && !this.MinimizeBox)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }
    }
}

namespace IPAddressManagement
{
    //LinkType: 0:vDU-ORU 1:CDU-RU 2:vDU-FSU-RU
    //IpType: 0:DuIp 1:FsuIp 2:RuIp 3:ensf 4:ServerIp
    public class DevTypeOp
    {
        private string sDevTypePath;
        public DevTypeOp(string sDevTypeFilePath)
        {
            this.sDevTypePath = sDevTypeFilePath;
            return;
        }
        public void SaveDevType(int iDevType)
        {
            if (File.Exists(sDevTypePath))
            {
                File.WriteAllText(sDevTypePath, iDevType.ToString());
            }
            else
            {
                // Create a new file and write the number to it
                using (StreamWriter writer = File.CreateText(sDevTypePath))
                {
                    writer.WriteLine(iDevType);
                }
            }
        }
        public int GetLinkType()
        {
            if (!File.Exists(sDevTypePath))
            {
                return 0;
            }
            string[] lines = File.ReadAllLines(sDevTypePath);
            if (lines.Length > 0 && int.TryParse(lines[0], out int Type))
            {
                return Type;
            }

            return 0;
        }
    }

    public class IPAddressOp
    {
        private string sIpFilePath;
        private string sIpType;
        public IPAddressOp(string sIpType, string sIpFilePath)
        {
            if (!File.Exists(sIpFilePath))
            {
                //创建文件，并把文件写为默认值
                using (StreamWriter writer = File.CreateText(sIpFilePath))
                {
                    writer.WriteLine("vDU-ORU DuIp 0.0.0.0");
                }
            }
            this.sIpType = sIpType;
            this.sIpFilePath = sIpFilePath;
        }

        public bool IsIPAddressValid(string ipAddress)
        {
            IPAddress parsedIPAddress;
            // Try parse IP address
            if (IPAddress.TryParse(ipAddress, out parsedIPAddress))
            {
                // Check the validation of IPv4 or IPv6 address
                return parsedIPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                    || parsedIPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
            }
            return false;
        }

        public void SaveIPAddressToFile(string sLinkType, string sIpAddress)
        {
            string sIpInfo = sLinkType + " " + sIpType + " " + sIpAddress;
            if (!File.Exists(sIpFilePath))
            {
                File.WriteAllText(sIpFilePath, sIpInfo);
                return;
            }
            List<string> ipAddresses = LoadIPAddressesFromFile();
            if (ipAddresses.Contains(sIpInfo))
                ipAddresses.Remove(sIpInfo);
            ipAddresses.Add(sIpInfo);
            File.WriteAllLines(sIpFilePath, ipAddresses);
            Console.WriteLine("IP 地址已保存到文件");
        }

        public string GetLastIpAddress(string sLinkType)
        {
            bool bVldIpFlag = false;
            string lastIPAddress = null;
            if (File.Exists(sIpFilePath))
            {
                string[] lines = File.ReadAllLines(sIpFilePath);
                if (lines.Length > 0)
                {
                    // 逐行读取文件
                    foreach (string line in File.ReadLines(sIpFilePath))
                    {
                        // 使用 Split 方法分隔字符串
                        string[] parts = line.Split(' ');
                        // 检查前两个子字符串是否匹配给定条件
                        if ((parts.Length >= 2) && (parts[0] == sLinkType) && (parts[1] == sIpType))
                        {
                            // 记录当前行的第三个子字符串
                            lastIPAddress = parts.Length >= 3 ? parts[2] : null;
                            bVldIpFlag = true;
                        }
                    }
                }
            }
            if (sIpType == "ensf")
            {
                if (bVldIpFlag == true)
                    return lastIPAddress;
                else
                    return "ens0f0";
            }
            else
            {
                if (!IsIPAddressValid(lastIPAddress))
                    return "0.0.0.0";
                return lastIPAddress;
            }
        }

        public int GetIPAddressCount(string sLinkType)
        {
            if (!File.Exists(sIpFilePath))
            {
                return 0;
            }
            int count = 0;
            foreach (string line in File.ReadLines(sIpFilePath))
            {
                string[] parts = line.Split(' ');

                if ((parts.Length >= 2) && (parts[0] == sLinkType) && (parts[1] == sIpType))
                {
                    if(sIpType == "ensf")
                        count++;
                    else if(IsIPAddressValid(parts.Length >= 3 ? parts[2] : null))
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public string GetIPAddressAtIndex(string sLinkType, int index)
        {
            if (!File.Exists(sIpFilePath))
            {
                return "0.0.0.0";
            }
            string[] lines = File.ReadAllLines(sIpFilePath);
            int count = 0;
            foreach (string line in lines)
            {
                string[] parts = line.Split(' ');

                if (parts.Length >= 3 && parts[0] == sLinkType && parts[1] == sIpType)
                {
                    if (count == index)
                    {
                        return parts[2];
                    }
                    count++;
                }
            }
            return "0.0.0.0"; // or you can throw an exception to indicate an invalid index
        }

        public void DeleteIPAddress(string ipAddress)
        {
            // 读取文件的所有行
            string[] ipAddresses = File.ReadAllLines(sIpFilePath);

            // 使用 LINQ 过滤出不匹配给定条件的行
            var remainingIpAdds = ipAddresses.Where(line =>
            {
                string[] parts = line.Split(' ');
                return !(parts.Length >= 2 && parts[1] == sIpType && parts[2] == ipAddress);
            }).ToArray();

            // 覆盖文件，将过滤后的行写回文件
            File.WriteAllLines(sIpFilePath, remainingIpAdds);
        }

        private List<string> LoadIPAddressesFromFile()
        {
            if (!File.Exists(sIpFilePath))
            {
                return new List<string>();
            }
            return new List<string>(File.ReadAllLines(sIpFilePath));
        }
    }
}

namespace FileManagement
{
    public class FilePathOp
    {
        private string sPathType;
        private string sFilePath;
        public FilePathOp(string sPathType, string sFilePath)
        {
            this.sPathType = sPathType;
            this.sFilePath = sFilePath;
            if (!File.Exists(sFilePath))
            {
                using (StreamWriter writer = File.CreateText(sFilePath))
                {
                    writer.WriteLine("");
                }
            }
        }
        public string GetPath()
        {
            string lastSelectedPath = string.Empty;
            if (!File.Exists(sFilePath))
            {
                return lastSelectedPath;
            }
            string[] lines = File.ReadAllLines(sFilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(' ');
                if (parts.Length >= 2 && parts[0] == sPathType)
                {
                    lastSelectedPath = line.Substring(line.IndexOf(' ') + 1);
                }
            }
            return lastSelectedPath; // or you can throw an exception to indicate an invalid index
        }
        public void SavePath(string selectedFilePath)
        {
            string sPathInfo = sPathType + ' ' + selectedFilePath;
            if (!File.Exists(sFilePath))
            {
                File.WriteAllText(sFilePath, sPathInfo);
                return;
            }
            string[] lines = File.ReadAllLines(sFilePath);
            using (StreamWriter writer = new StreamWriter(sFilePath))
            {
                foreach (string line in lines)
                {
                    if (!line.StartsWith(sPathType))
                    {
                        writer.WriteLine(line);
                    }
                }
                writer.WriteLine(sPathInfo);
            }
        }
        public string getSelFileName()
        {
            string configFilePath = GetPath();
            int lastIndex = configFilePath.LastIndexOf('\\');
            if (lastIndex >= 0)
            {
                return configFilePath.Substring(lastIndex + 1);
            }
            else
            {
                return null;
            }
        }
        public bool FilePathValid(string selectedFilePath)
        {
            string pattern = @"^[a-zA-Z0-9._:/\\-]+$";
            return Regex.IsMatch(selectedFilePath, pattern);
        }
        public bool FileNameValid(string selectedFilePath)
        {
            int lastIndex = selectedFilePath.LastIndexOf('\\');
            if (lastIndex >= 0)
            {
                string pattern = @"^[a-zA-Z0-9._:-]+$";
                return Regex.IsMatch(selectedFilePath.Substring(lastIndex + 1), pattern);
            }
            else
            {
                return false;
            }
        }
    }

    public class FileOp
    {
        private string sFilePath;
        public FileOp(string sFilePath)
        {
            this.sFilePath = sFilePath;
            CreateFile();
        }
        // 读取文件内容
        public string ReadFile()
        {
            try
            {
                if (File.Exists(sFilePath))
                {
                    return File.ReadAllText(sFilePath);
                }
                else
                {
                    return "File does not exist!";
                }
            }
            catch (Exception ex)
            {
                return "Error while reading the file: " + ex.Message;
            }
        }

        // 写入内容到文件
        public bool WriteToFile(string filePath, string content)
        {
            try
            {
                File.WriteAllText(filePath, content);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while writing to the file: " + ex.Message);
                return false;
            }
        }

        // 追加内容到文件
        public bool AppendToFile(string content)
        {
            try
            {
                File.AppendAllText(sFilePath, content);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while appending to the file: " + ex.Message);
                return false;
            }
        }

        // 创建文件
        public bool CreateFile()
        {
            try
            {
                if (!File.Exists(sFilePath))
                {
                    File.Create(sFilePath).Close();
                    return true;
                }
                else
                {
                    Console.WriteLine("File already exists!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while creating the file: " + ex.Message);
                return false;
            }
        }

        // 删除文件
        public bool DeleteFile()
        {
            try
            {
                if (File.Exists(sFilePath))
                {
                    File.Delete(sFilePath);
                    return true;
                }
                else
                {
                    Console.WriteLine("File does not exist!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while deleting the file: " + ex.Message);
                return false;
            }
        }

        public bool ClearFile()
        {
            try
            {
                if (File.Exists(sFilePath))
                {
                    File.WriteAllText(sFilePath, string.Empty);
                    return true;
                }
                else
                {
                    Console.WriteLine("File does not exist!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while clearing the file: " + ex.Message);
                return false;
            }
        }
    }
}

namespace RemoteManagement
{
    public class SshOp
    {
        private readonly string host;
        private readonly string username;
        private readonly string password;
        private SshClient sshClient;
        private ShellStream shellStream;

        public SshOp(string host, string username, string password)
        {
            this.host = host;
            this.username = username;
            this.password = password;
        }

        public bool Connect()
        {
            sshClient = new SshClient(host, username, password);

            try
            {
                sshClient.Connect();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to SSH server: {ex.Message}");
                return false;
            }
        }

        public bool StartShell()
        {
            if (sshClient == null || !sshClient.IsConnected)
            {
                Console.WriteLine("SSH client is not connected.");
                return false;
            }

            try
            {
                shellStream = sshClient.CreateShellStream("xterm", 80, 24, 800, 600, 1024);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start SSH shell: {ex.Message}");
                return false;
            }
        }

        public void ExecuteCommand(string command)
        {
            if (shellStream == null || !shellStream.CanWrite)
            {
                Console.WriteLine("SSH shell is not available.");
                return;
            }

            try
            {
                shellStream.WriteLine(command);
                shellStream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to execute SSH command: {ex.Message}");
            }
        }

        public string RunCommand(string command)
        {
            if (sshClient == null || !sshClient.IsConnected)
            {
                Console.WriteLine("SSH client is not connected.");
                return null;
            }

            try
            {
                var sshCommand = sshClient.RunCommand(command);
                var result = sshCommand.Result;
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to run SSH command: {ex.Message}");
                return null;
            }
        }

        public bool WaitForOutput(string expectedOutput)
        {
            if (shellStream == null || !shellStream.CanRead)
            {
                Console.WriteLine("SSH shell is not available for reading.");
                return false;
            }

            try
            {
                var outputBuffer = new StringBuilder();
                var buffer = new byte[1024];

                while (true)
                {
                    var bytesRead = shellStream.Read(buffer, 0, buffer.Length);
                    /*if (bytesRead <= 0)
                    {
                        Console.WriteLine("bytesRead <= 0");
                        break;
                    }*/

                    // 字节流转换为字符串
                    var output = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    outputBuffer.Append(output);

                    // 检查是否达到预期的输出
                    if (outputBuffer.ToString().Contains(expectedOutput))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to wait for SSH output: {ex.Message}");
                return false;
            }
        }

        public bool WaitForOutput_Timer(string expectedOutput)
        {
            if (shellStream == null || !shellStream.CanRead)
            {
                Console.WriteLine("SSH shell is not available for reading.");
                return false;
            }

            try
            {
                var outputBuffer = new StringBuilder();
                var buffer = new byte[1024];

                DateTime startTime = DateTime.Now; // 记录起始时间

                while ((DateTime.Now - startTime).TotalSeconds <= 5) // 检查经过的时间是否超过5秒
                {
                    var bytesRead = shellStream.Read(buffer, 0, buffer.Length);
                    /*if (bytesRead <= 0)
                    {
                        Console.WriteLine("bytesRead <= 0");
                        break;
                    }*/

                    // 字节流转换为字符串
                    var output = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    outputBuffer.Append(output);

                    // 检查是否达到预期的输出
                    if (outputBuffer.ToString().Contains(expectedOutput))
                    {
                        return true;
                    }
                }
                Console.WriteLine("Timeout: no expected output found within 5 seconds."); // 超时信息
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to wait for SSH output: {ex.Message}");
                return false;
            }
        }

        public void Disconnect()
        {
            if (sshClient != null && sshClient.IsConnected)
            {
                sshClient.Disconnect();
            }
        }
    }
    public class SftpOp
    {
        private readonly string host;
        private readonly string username;
        private readonly string password;
        private SftpClient sftpClient;

        public SftpOp(string host, string username, string password)
        {
            this.host = host;
            this.username = username;
            this.password = password;
        }

        public bool Connect()
        {
            sftpClient = new SftpClient(host, username, password);

            try
            {
                sftpClient.Connect();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to SFTP server: {ex.Message}");
                return false;
            }
        }

        public bool DownloadFile(string remoteFilePath, string localFilePath)
        {
            if (sftpClient == null || !sftpClient.IsConnected)
            {
                Console.WriteLine("SFTP client is not connected.");
                return false;
            }

            try
            {
                using (var fileStream = new FileStream(localFilePath, FileMode.Create))
                {
                    sftpClient.DownloadFile(remoteFilePath, fileStream);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to download file from SFTP server: {ex.Message}");
                return false;
            }
        }

        public bool UploadFile(string localFilePath, string remoteFilePath)
        {
            if (sftpClient == null || !sftpClient.IsConnected)
            {
                Console.WriteLine("SFTP client is not connected.");
                return false;
            }

            try
            {
                using (var fileStream = new FileStream(localFilePath, FileMode.Open))
                {
                    sftpClient.UploadFile(fileStream, remoteFilePath);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to upload file to SFTP server: {ex.Message}" + remoteFilePath);
                return false;
            }
        }

        public void RenameFile(string oldRemoteFilePath, string newRemoteFilePath)
        {
            sftpClient.RenameFile(oldRemoteFilePath, newRemoteFilePath);
        }

        public void Disconnect()
        {
            if (sftpClient != null && sftpClient.IsConnected)
            {
                sftpClient.Disconnect();
            }
        }
    }
}

namespace UserManagement
{
    public class User
    {
        public string Type { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public User(string type, string username, string password)
        {
            Type = type;
            Username = username;
            Password = password;
        }

        public override string ToString()
        {
            return $"{Type}:{Username}:{Password}";
        }
    }

    public class UserManager
    {
        private string filePath;
        private Dictionary<string, User> users;

        public UserManager(string filePath)
        {
            this.filePath = filePath;
            users = new Dictionary<string, User>();
            LoadUsersFromFile();
        }

        public void AddUser(string type, string username, string password)
        {
            var user = new User(type, username, password);
            string userString = user.ToString();

            users[username] = user;

            // Write all users to the file
            File.WriteAllLines(filePath, GetUsersAsStringList());
        }

        public void RemoveUser(string username)
        {
            if (users.ContainsKey(username))
            {
                users.Remove(username);

                // Write all users to the file
                File.WriteAllLines(filePath, GetUsersAsStringList());
            }
        }

        public User GetUserByType(string type)
        {
            foreach (var user in users.Values)
            {
                if (user.Type == type)
                {
                    return user;
                }
            }

            return null;
        }

        private List<string> GetUsersAsStringList()
        {
            var userList = new List<string>();

            foreach (var user in users.Values)
            {
                userList.Add(user.ToString());
            }

            return userList;
        }

        private void LoadUsersFromFile()
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 3)
                    {
                        string type = parts[0];
                        string username = parts[1];
                        string password = parts[2];
                        var user = new User(type, username, password);
                        users[username] = user;
                    }
                }
            }
        }
    }
}
