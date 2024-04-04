using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using IPAddressManagement;
using FileManagement;
using RemoteManagement;
using UserManagement;
using SwiftCopyButtonManagement;
using System.Text.RegularExpressions;
using System.Threading;

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
        private string sRelVer = "3.1.1";
        private bool globalTransFlag = false;

        IPAddressOp duIpOp = new IPAddressOp("DuIp", "./config/IpDataSet.cfg");
        IPAddressOp ruIpOp = new IPAddressOp("RuIp", "./config/IpDataSet.cfg");
        IPAddressOp ensfOp = new IPAddressOp("ensf", "./config/IpDataSet.cfg");
        IPAddressOp fsuIpOp = new IPAddressOp("FsuIp", "./config/IpDataSet.cfg");
        IPAddressOp serverIpOp = new IPAddressOp("BaseServerIp", "./config/IpDataSet.cfg");
        TypeOp transModeTypeOp = new TypeOp("TransModeType", "PC->RU", "./config/Type.cfg");
        TypeOp devTypeOp = new TypeOp("LinkType", "CDU-RU", "./config/Type.cfg");
        TypeOp pw123qweTypeOp = new TypeOp("SimplePwType", "FALSE", "./config/Type.cfg");/*wether the password is 123qwe*/
        FilePathOp uploadFilePathOp = new FilePathOp("UploadFilePath", "C:", "./config/Path.cfg");
        FilePathOp dlFileSavePathOp = new FilePathOp("DownloadFileSavePath", "C:", "./config/Path.cfg");
        FilePathOp dlFilPathInRuOp = new FilePathOp("FilePathInRu", "/tmp/", "./config/Path.cfg");
        FilePathOp newVerPathOp = new FilePathOp("NewVerPath", "C:", "./config/Path.cfg");
        FilePathOp newVerChkPathOp = new FilePathOp("NewVerChkPath", "C:", "./config/Path.cfg");
        UserManager usrTest = new UserManager("./config/UserMng.cfg", "testUser");
        UserManager usrBaseServer = new UserManager("./config/UserMng.cfg", "usrBaseServer");
        UserManager usrRuUser = new UserManager("./config/UserMng.cfg", "ruUser");
        UserManager usrRuRoot = new UserManager("./config/UserMng.cfg", "ruRoot");
        UserManager usrRuUserOld = new UserManager("./config/UserMng.cfg", "ruUserOld");
        UserManager usrRuRootOld = new UserManager("./config/UserMng.cfg", "ruRootOld");
        UserManager usrCduUser = new UserManager("./config/UserMng.cfg", "cduUser");
        UserManager usrCduRoot = new UserManager("./config/UserMng.cfg", "cduRoot");
        UserManager usrVduUser = new UserManager("./config/UserMng.cfg", "vduUser");
        UserManager usrVduRoot = new UserManager("./config/UserMng.cfg", "vduRoot");
        UserManager usrFsuUser = new UserManager("./config/UserMng.cfg", "fsuUser");
        UserManager usrFsuRoot = new UserManager("./config/UserMng.cfg", "fsuRoot");
        SwiftCpButtonManager swiftCpButtionOp = new SwiftCpButtonManager("./config/SwiftCopyButton.cfg");
        FileOp logFile = new FileOp("./log/Script.log");
        //MainForm mainForm = new MainForm();
        public Form1()
        {
            InitializeComponent();
            InitializeSystemTray();
            //Title Init
            lastClosingTime = DateTime.Now;
            this.Text = "EasyTransTool-V" + sRelVer + "(Developed by wei.zhou@FW)";
            //Show new version release path in form
            newVerRelPath.Text = newVerPathOp.GetPath();

            initAllObject(false);
        }

        private void initAllObject(bool transEndFlag)
        {
            TransModeSelBox.Enabled = true;
            transModeSwitchButton.Enabled = true;
            filePathSel.Enabled = true;
            TypeSelBox.Enabled = true;
            duIpDelButton.Enabled = true;
            ruIpDelButton.Enabled = true;
            pw123qweCheckBox.Enabled = true;
            TransSplitButton.Enabled = true;

            //Check whether need to config Base Server
            if (string.Equals("USER_NAME", usrBaseServer.GetName()) || string.Equals("PASSWORD", usrBaseServer.GetPW()))
            {
                MessageBox.Show("请在config/UserMng.cfg文件中配置服务器的用户名和密码！");
            }

            //Init trans type select box
            //Init file path label
            TransModeSelBox.Items.Clear();
            TransModeSelBox.Items.Add("PC->RU");
            TransModeSelBox.Items.Add("RU->PC");
            TransModeSelBox.SelectedItem = transModeTypeOp.GetType();
            TransModeSwitch();

            //Init link type select box
            TypeSelBox.Items.Clear();
            TypeSelBox.Items.Add("vDU-ORU");
            TypeSelBox.Items.Add("CDU-RU");
            TypeSelBox.Items.Add("vDU-FSU-RU");
            TypeSelBox.SelectedItem = devTypeOp.GetType();

            //Init CheckBox
            if (string.Equals("TRUE", pw123qweTypeOp.GetType()))
                pw123qweCheckBox.Checked = true;

            //Init TransModeSplitButton
            TransButtonRefresh();
            

            ComboBox_Refresh(DuIpComboBox, duIpOp, duIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, duIpDelButton);
            ComboBox_Refresh(RuIpComboBox, ruIpOp, ruIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, ruIpDelButton);
            if (string.Equals("vDU-FSU-RU", TypeSelBox.Text))
            {
                ComboBox_Refresh(FsuIpComboBox, fsuIpOp, fsuIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, fsuIpDelButton);
                ComboBox_Disable(EnsfComboBox, ensfDelButton);
            }
            else if (string.Equals("CDU-RU", TypeSelBox.Text))
            {
                ComboBox_Disable(FsuIpComboBox, fsuIpDelButton);
                ComboBox_Disable(EnsfComboBox, ensfDelButton);
            }
            else if (string.Equals("vDU-ORU", TypeSelBox.Text))
            {
                ComboBox_Disable(FsuIpComboBox, fsuIpDelButton);
                ComboBox_Refresh(EnsfComboBox, ensfOp, ensfOp.GetIPAddressCount(TypeSelBox.Text) - 1, ensfDelButton);
            }
            else
            {
                ComboBox_Refresh(FsuIpComboBox, fsuIpOp, fsuIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, fsuIpDelButton);
                ComboBox_Refresh(EnsfComboBox, ensfOp, ensfOp.GetIPAddressCount(TypeSelBox.Text) - 1, ensfDelButton);
            }

            string[] swiftCpButtonName = new string[4];
            for (int index = 0; index < 4; index++)
            {
                swiftCpButtonName[index] = swiftCpButtionOp.GetNameByIndex(index);
            }
            if (string.Empty != swiftCpButtonName[0]) swiftCpButton0.Text = swiftCpButtonName[0];
            else swiftCpButton0.Enabled = false;
            if (string.Empty != swiftCpButtonName[1]) swiftCpButton1.Text = swiftCpButtonName[1];
            else swiftCpButton1.Enabled = false;
            if (string.Empty != swiftCpButtonName[2]) swiftCpButton2.Text = swiftCpButtonName[2];
            else swiftCpButton2.Enabled = false;
            if (string.Empty != swiftCpButtonName[3]) swiftCpButton3.Text = swiftCpButtonName[3];
            else swiftCpButton3.Enabled = false;

            return;
        }

        private void filePathSel_Click(object sender, EventArgs e)
        {
            if (string.Equals("PC->RU", TransModeSelBox.Text))
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Title = "选择要上传或升级的文件";
                fileDialog.Filter = "所有文件|*.*";
                fileDialog.Multiselect = true;
                fileDialog.InitialDirectory = uploadFilePathOp.GetPath();

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    string[] selectedFilePaths = fileDialog.FileNames;
                    bool bPathDeleteFlag = false;
                    foreach (string filePath in selectedFilePaths)
                    {
                        if (uploadFilePathOp.FileNameValid(filePath))
                        {
                            if (bPathDeleteFlag == false)
                            {
                                uploadFilePathOp.DeletePath();
                                bPathDeleteFlag = true;
                            }
                            uploadFilePathOp.SavePath(filePath, false);
                        }
                        else
                        {
                            MessageBox.Show("文件名存在不支持的字符，仅支持A~Za~z-_.:");
                        }
                    }
                    for (int i = 0; i < uploadFilePathOp.PathCount(); i++)
                    {
                        if (i == 0)
                        {
                            filePath.Text = uploadFilePathOp.GetPathByIndex(i);
                        }
                        else
                        {
                            Console.WriteLine(uploadFilePathOp.getSelFileNameByIndex(i));
                            filePath.Text += "|" + uploadFilePathOp.getSelFileNameByIndex(i);
                        }
                    }
                }
            }
            else if (string.Equals("RU->PC", TransModeSelBox.Text))
            {
                using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
                {
                    folderBrowserDialog.Description = "选择保存的路径";
                    folderBrowserDialog.SelectedPath = dlFileSavePathOp.GetPath();

                    DialogResult result = folderBrowserDialog.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        filePath.Text = folderBrowserDialog.SelectedPath;
                        dlFileSavePathOp.SavePath(folderBrowserDialog.SelectedPath, true);
                    }
                }
            }
            TransButtonRefresh();
        }

        string scriptPath;

        private void TransSplitButton_Click(object sender, EventArgs e)
        {
            globalTransFlag = true;
            fileTransProgressBar.Value = 0;

            // Save the IP address and refresh the ComboBox
            string duIpAddress = DuIpComboBox.Text;
            string ruIpAddress = RuIpComboBox.Text;
            string fsuIpAddress = FsuIpComboBox.Text;
            string ensfAddress = EnsfComboBox.Text;
            var ipAddress = new object[4];
            ipAddress[0] = duIpAddress;
            ipAddress[1] = ruIpAddress;
            ipAddress[2] = fsuIpAddress;
            ipAddress[3] = ensfAddress;


            scriptPath = fileTransScriptPreCheck();

            if (scriptPath == String.Empty) return;
            fileTransProgressBar.Value = 3;

            if (!fileTransIpPreCheck(duIpAddress, ruIpAddress, fsuIpAddress, ensfAddress)) return;
            fileTransProgressBar.Value = 5;

            if (!fileTransPathPreCheck()) return;

            disableAllObject();

            fileTransBGWorker.RunWorkerAsync(ipAddress);
        }

        private void fileTransBGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var receiver = e.Argument as object[];
            string[] ipSet = new string[receiver.Length];
            for (int i = 0; i < receiver.Length; i++)
                ipSet[i] = receiver[i].ToString();

            fileTransBGWorker.ReportProgress(8);
            if (string.Equals("PC->RU", transModeTypeOp.GetType()))
            {
                upload_Core(ipSet);
            }
            else if (string.Equals("RU->PC", transModeTypeOp.GetType()))
            {
                download_Core(ipSet);
            }
            initAllObject(true);
            globalTransFlag = false;
            return;
        }

        private void fileTransBGWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine("Progress Value: " + e.ProgressPercentage.ToString());
            this.fileTransProgressBar.Value = e.ProgressPercentage;
        }

        private void fileTransBGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (100 != fileTransProgressBar.Value)
            {
                fileTransProgressBar.Value = 0;
            }
        }

        private void disableAllObject()
        {
            TransModeSelBox.Enabled = false;
            transModeSwitchButton.Enabled = false;
            filePathSel.Enabled = false;
            TypeSelBox.Enabled= false;
            DuIpComboBox.Enabled= false;
            duIpDelButton.Enabled= false;
            FsuIpComboBox.Enabled = false;
            fsuIpDelButton.Enabled = false;
            RuIpComboBox.Enabled = false;
            ruIpDelButton.Enabled = false;
            EnsfComboBox.Enabled = false;
            ensfDelButton.Enabled = false;
            dlFileName.Enabled = false;
            pw123qweCheckBox.Enabled = false;
            TransSplitButton.Enabled = false;
            return;
        }

        private string fileTransScriptPreCheck()
        {
            if (string.Equals("上传文件", TransSplitButton.Text) || string.Equals("下载文件", TransSplitButton.Text))
            {
                if (string.Equals("PC->RU", TransModeSelBox.Text))
                {
                    if (string.Equals("vDU-FSU-RU", TypeSelBox.Text)) scriptPath = "./script/vduFsuRuUpload.script";
                    else if (string.Equals("CDU-RU", TypeSelBox.Text)) scriptPath = "./script/cduRuUpload.script";
                    else if (string.Equals("vDU-ORU", TypeSelBox.Text)) scriptPath = "./script/vduOruUpload.script";
                    else
                    {
                        MessageBox.Show("无效的连接类型！");
                        return String.Empty;
                    }
                }
                else if (string.Equals("RU->PC", TransModeSelBox.Text))
                {
                    if (string.Equals("vDU-FSU-RU", TypeSelBox.Text)) scriptPath = "./script/vduFsuRuDownload.script";
                    else if (string.Equals("CDU-RU", TypeSelBox.Text)) scriptPath = "./script/cduRuDownload.script";
                    else if (string.Equals("vDU-ORU", TypeSelBox.Text)) scriptPath = "./script/vduOruDownload.script";
                    else
                    {
                        MessageBox.Show("无效的连接类型！");
                        return String.Empty;
                    }
                }
                else
                {
                    MessageBox.Show("无效的传输类型！");
                    return String.Empty;
                }
            }
            else
            {
                if (string.Equals("PC->RU", TransModeSelBox.Text))
                {
                    if (string.Equals("vDU-FSU-RU", TypeSelBox.Text))
                    {
                        if ((2 == uploadFilePathOp.PathCount())
                        && (((string.Equals("RUSW_APP.out", uploadFilePathOp.getSelFileNameByIndex(0))) && (string.Equals("SymTbl.txt", uploadFilePathOp.getSelFileNameByIndex(1))))
                        || ((string.Equals("RUSW_APP.out", uploadFilePathOp.getSelFileNameByIndex(1))) && (string.Equals("SymTbl.txt", uploadFilePathOp.getSelFileNameByIndex(0))))))
                        {
                            scriptPath = "./script/vduFsuRuRuswAppUpdate.script";
                        }
                        else
                        {
                            MessageBox.Show("更新RUSW_APP.out需要同时选择RUSW_APP.out和SymTbl.txt为上传文件！");
                            return String.Empty;
                        }
                    }
                    else if (string.Equals("CDU-RU", TypeSelBox.Text))
                    {
                        if ((2 == uploadFilePathOp.PathCount())
                        && (((string.Equals("RUSW_APP.out", uploadFilePathOp.getSelFileNameByIndex(0))) && (string.Equals("SymTbl.txt", uploadFilePathOp.getSelFileNameByIndex(1))))
                        || ((string.Equals("RUSW_APP.out", uploadFilePathOp.getSelFileNameByIndex(1))) && (string.Equals("SymTbl.txt", uploadFilePathOp.getSelFileNameByIndex(0))))))
                        {
                            scriptPath = "./script/cduRuRuswAppUpdate.script";
                        }
                        else
                        {
                            MessageBox.Show("更新RUSW_APP.out需要同时选择RUSW_APP.out和SymTbl.txt为上传文件！");
                            return String.Empty;
                        }
                    }
                    else if (string.Equals("vDU-ORU", TypeSelBox.Text))
                    {
                        if ((1 == uploadFilePathOp.PathCount()) && (uploadFilePathOp.getSelFileNameByIndex(0).Contains("rcm.")))
                        {
                            scriptPath = "./script/vduOruRcmUpdate.script";
                        }
                        else
                        {
                            MessageBox.Show("更新rcm需要选择rcm为上传文件！");
                            return String.Empty;
                        }
                    }
                    else
                    {
                        MessageBox.Show("无效的连接类型！");
                        return String.Empty;
                    }
                }
                else
                {
                    MessageBox.Show("RU->PC无法" + TransSplitButton.Text + "！");
                    return String.Empty;
                }
            }
            return scriptPath;
        }

        private bool fileTransIpPreCheck(string duIpAddress, string ruIpAddress, string fsuIpAddress, string ensfAddress)
        {
            if (!File.Exists(scriptPath))
            {
                MessageBox.Show("升级脚本不存在！");
                return false;
            }
            if (!duIpOp.IsIPAddressValid(duIpAddress))
            {
                MessageBox.Show("DU IP 地址无效！");
                return false;
            }
            else if (!ruIpOp.IsIPAddressValid(ruIpAddress))
            {
                MessageBox.Show("RU IP 地址无效！");
                return false;
            }
            else if ((!string.Equals("", fsuIpAddress)) && (!fsuIpOp.IsIPAddressValid(fsuIpAddress)))
            {
                MessageBox.Show("FSU IP 地址无效！");
                return false;
            }
            else
            {
                devTypeOp.SaveType(TypeSelBox.SelectedItem.ToString());
                transModeTypeOp.SaveType(TransModeSelBox.SelectedItem.ToString());
                if (string.Empty != dlFileName.Text)
                {
                    dlFilPathInRuOp.SavePath(dlFileName.Text, true);
                }
                duIpOp.SaveIPAddressToFile(TypeSelBox.Text, duIpAddress);
                ruIpOp.SaveIPAddressToFile(TypeSelBox.Text, ruIpAddress);
                if (!string.Equals("", fsuIpAddress))
                {
                    fsuIpOp.SaveIPAddressToFile(TypeSelBox.Text, fsuIpAddress);
                    //ComboBox_Refresh(FsuIpComboBox, fsuIpOp, fsuIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, fsuIpDelButton);
                }
                if (!string.Equals("", ensfAddress))
                {
                    ensfOp.SaveIPAddressToFile(TypeSelBox.Text, ensfAddress);
                    //ComboBox_Refresh(EnsfComboBox, ensfOp, ensfOp.GetIPAddressCount(TypeSelBox.Text) - 1, ensfDelButton);
                }
                //ComboBox_Refresh(DuIpComboBox, duIpOp, duIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, duIpDelButton);
                //ComboBox_Refresh(RuIpComboBox, ruIpOp, ruIpOp.GetIPAddressCount(TypeSelBox.Text) - 1, ruIpDelButton);
                return true;
            }
        }

        private bool fileTransPathPreCheck()
        {
            if (string.Equals("PC->RU", transModeTypeOp.GetType()))
            {
                for (int i = 0; i < uploadFilePathOp.PathCount(); i++)
                {
                    if (!File.Exists(uploadFilePathOp.GetPathByIndex(i)))
                    {
                        MessageBox.Show("本地文件不存在！");
                        return false;
                    }
                }
                return true;
            }
            else if (string.Equals("RU->PC", transModeTypeOp.GetType()))
            {
                if (!Directory.Exists(dlFileSavePathOp.GetPath()))
                {
                    MessageBox.Show("保存路径不存在！");
                    return false;
                }
                return true;
            }
            return false;
        }

        private void upload_Core(string[] ipSet)
        {
            // Start update procedure
            // 1.Put file to 116.8 server
            /*SshOp serverSshOp = new SshOp(serverIpOp.GetLastIpAddress(devTypeOp.GetType()), usrTest.GetName(), usrTest.GetPW());
            SftpOp serverSftpOp = new SftpOp(serverIpOp.GetLastIpAddress(devTypeOp.GetType()), usrTest.GetName(), usrTest.GetPW());
            string filePathInServer = "/home/zw/" + Environment.UserName + "/";*/
            SshOp serverSshOp = new SshOp(serverIpOp.GetLastIpAddress(devTypeOp.GetType()), usrBaseServer.GetName(), usrBaseServer.GetPW());
            SftpOp serverSftpOp = new SftpOp(serverIpOp.GetLastIpAddress(devTypeOp.GetType()), usrBaseServer.GetName(), usrBaseServer.GetPW());
            string filePathInServer = "/home/" + usrBaseServer.GetName() + "/EasyTransTool/" + Environment.UserName + "/";

            string timeStamp = Regex.Replace(DateTime.Now.TimeOfDay.ToString(), @"[^\d]", "");
            string tempFilePathInServer = filePathInServer + timeStamp;
            bool failFlag = false;

            //每一句命令都需要检查返回值
            fileTransBGWorker.ReportProgress(10);
            if (true == serverSshOp.Connect())
            {
                serverSshOp.RunCommand("mkdir -p " + tempFilePathInServer);
                serverSshOp.Disconnect();
            }
            else
            {
                MessageBox.Show("连接服务器失败！请检查config目录下IpDataSet.cfg和UserMng.cfg文件中服务器IP、用户名和密码配置是否正确！");
                return;
            }
            fileTransBGWorker.ReportProgress(15);
            if (true == serverSftpOp.Connect())
            {
                for (int i = 0; i < uploadFilePathOp.PathCount(); i++)
                {
                    if (false == serverSftpOp.UploadFile(uploadFilePathOp.GetPathByIndex(i), tempFilePathInServer + "/" + uploadFilePathOp.getSelFileNameByIndex(i)))
                    {
                        failFlag = true;
                        break;
                    }
                }
                if(string.Equals("./script/vduFsuRuRuswAppUpdate.script", scriptPath) || string.Equals("./script/cduRuRuswAppUpdate.script", scriptPath))
                {
                    if (false == serverSftpOp.UploadFile("./script/RuswAppUpdate.sh", tempFilePathInServer + "/RuswAppUpdate.sh"))
                        failFlag = true;
                }
                else if (string.Equals("./script/vduOruRcmUpdate.script", scriptPath))
                {
                    if (false == serverSftpOp.UploadFile("./script/RcmUpdate.sh", tempFilePathInServer + "/RcmUpdate.sh"))
                        failFlag = true;
                }
                serverSftpOp.Disconnect();
            }
            fileTransBGWorker.ReportProgress(40);
            if (true == serverSshOp.Connect())
            {
                logFile.ClearFile();
                if ((true == failFlag) || (true != serverSshOp.StartShell()) || (false == script_execute_core(scriptPath, serverSshOp, ipSet, timeStamp, 40)))
                {
                    serverSshOp.RunCommand("rm -rf " + tempFilePathInServer);
                    serverSshOp.RunCommand("echo " + "$(date +\"%Y-%m-%d %H:%M:%S\") " + Environment.UserName + " " + sRelVer + " UploadFail >> " + filePathInServer + "TransResult.log");
                    serverSshOp.Disconnect();
                    return;
                }
                serverSshOp.RunCommand("rm -rf " + tempFilePathInServer);
                serverSshOp.RunCommand("echo " + "$(date +\"%Y-%m-%d %H:%M:%S\") " + Environment.UserName + " " + sRelVer + " UploadSuccuss >> " + filePathInServer + "TransResult.log");
                serverSshOp.Disconnect();
            }
            fileTransBGWorker.ReportProgress(100);
        }

        private void download_Core(string[] ipSet)
        {
            // Start update procedure
            // 1.Put file to 116.8 server
            /*SshOp serverSshOp = new SshOp(serverIpOp.GetLastIpAddress(devTypeOp.GetType()), usrTest.GetName(), usrTest.GetPW());
            SftpOp serverSftpOp = new SftpOp(serverIpOp.GetLastIpAddress(devTypeOp.GetType()), usrTest.GetName(), usrTest.GetPW());
            string filePathInServer = "/tmp/";*/
            SshOp serverSshOp = new SshOp(serverIpOp.GetLastIpAddress(devTypeOp.GetType()), usrBaseServer.GetName(), usrBaseServer.GetPW());
            SftpOp serverSftpOp = new SftpOp(serverIpOp.GetLastIpAddress(devTypeOp.GetType()), usrBaseServer.GetName(), usrBaseServer.GetPW());
            string filePathInServer = "/home/" + usrBaseServer.GetName() + "/EasyTransTool/" + Environment.UserName + "/";

            string timeStamp = Regex.Replace(DateTime.Now.TimeOfDay.ToString(), @"[^\d]", "");
            string tempFilePathInServer = filePathInServer + timeStamp;
            bool failFlag = false;

            //每一句命令都需要检查返回值
            fileTransBGWorker.ReportProgress(10);
            if (true == serverSshOp.Connect())
            {
                serverSshOp.RunCommand("mkdir -p " + tempFilePathInServer);

                logFile.ClearFile();
                if ((true != serverSshOp.StartShell()) || (false == script_execute_core(scriptPath, serverSshOp, ipSet, timeStamp, 10)))
                {
                    serverSshOp.RunCommand("rm -rf " + tempFilePathInServer);
                    serverSshOp.RunCommand("echo " + "$(date +\"%Y-%m-%d %H:%M:%S\") " + Environment.UserName + " " + sRelVer + " DownloadFail >> " + filePathInServer + "TransResult.log");
                    serverSshOp.Disconnect();
                    return;
                }
                serverSshOp.Disconnect();
            }
            else
            {
                MessageBox.Show("连接服务器失败！请检查config目录下IpDataSet.cfg和UserMng.cfg文件中服务器IP、用户名和密码配置是否正确！");
                return;
            }
            fileTransBGWorker.ReportProgress(70);
            if (true == serverSftpOp.Connect())
            {
                //考虑在脚本里实现文件下载过程的调用，这样可以更好进行进度条的控制
                if (false == serverSftpOp.DownloadFile(tempFilePathInServer, dlFileSavePathOp.GetPath()))
                {
                    failFlag = true;
                }
                serverSftpOp.Disconnect();
            }
            fileTransBGWorker.ReportProgress(95);
            if (true == serverSshOp.Connect())
            {
                if (true == failFlag)
                {
                    serverSshOp.RunCommand("rm -rf " + tempFilePathInServer);
                    serverSshOp.RunCommand("echo " + "$(date +\"%Y-%m-%d %H:%M:%S\") " + Environment.UserName + " " + sRelVer + " DownloadFail >> " + filePathInServer + "TransResult.log");
                    serverSshOp.Disconnect();
                    return;
                }
                serverSshOp.RunCommand("rm -rf " + tempFilePathInServer);
                serverSshOp.RunCommand("echo " + "$(date +\"%Y-%m-%d %H:%M:%S\") " + Environment.UserName + " " + sRelVer + " DownloadSuccess >> " + filePathInServer + "TransResult.log");
                serverSshOp.Disconnect();
            }
            fileTransBGWorker.ReportProgress(100);
        }

        int timeoutThreshold = 5;
        int waitResult = 0;//0:didn't get expeced output, 1:get first expected output, 2:get second expected output...
        private bool script_execute_core(string scriptFile, SshOp serverSshOp, string[] ipSet, string timeStamp, int transBasePercent)
        {
            try
            {
                double scriptLineCnt = File.ReadLines(scriptFile).Count();
                double lineCounter = 0;
                logFile.AppendToFile("===============Start Running " + scriptFile + "===============\n");
                string[] fileContents = File.ReadAllLines(scriptFile);

                for (int lineId = 1; lineId <= fileContents.Length; lineId++)
                {
                    string line = scriptUpdater(fileContents[lineId - 1], ipSet, timeStamp);

                    logFile.AppendToFile(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]") + line + "\n");

                    int execRst = scriptExecuter(line, scriptFile, serverSshOp, ipSet, timeStamp);
                    if (-1 == execRst) return false;
                    else if (0 < execRst) lineId = execRst - 1; //-1 because lineId++ every for loop

                    lineCounter++;
                    if (0 < transBasePercent)
                    {
                        fileTransBGWorker.ReportProgress(transBasePercent + (int)((lineCounter / scriptLineCnt) * 60));
                    }
                }
                logFile.AppendToFile("===============Finish Running " + scriptFile + "===============\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to read file: " + ex.Message);
                return false;
            }
            return true;
        }

        private string scriptUpdater(string line, string[] ipSet, string timeStamp)
        {
            string sUpdatedScript = line;
            string userInputFilePathInRU = dlFilPathInRuOp.GetPath();
            string filePathInRU;
            string fileNameInRU;
            string duIpAddress = ipSet[0];
            string ruIpAddress = ipSet[1];
            string fsuIpAddress = ipSet[2];
            string ensfAddress = ipSet[3];

            if (dlFilPathInRuOp.GetPath().StartsWith("/"))
            {
                int lastIndex = userInputFilePathInRU.LastIndexOf('/');
                if (lastIndex >= 0)
                {
                    filePathInRU = userInputFilePathInRU.Substring(0, lastIndex);
                    fileNameInRU = userInputFilePathInRU.Substring(lastIndex + 1);
                }
                else
                {
                    MessageBox.Show("下载失败，请检查要获取的文件路径是否正确！");
                    return string.Empty;
                }
            }
            else
            {
                filePathInRU = "/tmp";
                fileNameInRU = userInputFilePathInRU;
            }

            sUpdatedScript = sUpdatedScript.Replace("USER_PATH", Environment.UserName);
            sUpdatedScript = sUpdatedScript.Replace("DU_IP_ADDR", duIpAddress);
            sUpdatedScript = sUpdatedScript.Replace("ENS_F", ensfAddress);
            sUpdatedScript = sUpdatedScript.Replace("FSU_IP_ADDR", fsuIpAddress);
            sUpdatedScript = sUpdatedScript.Replace("RU_IP_ADDR", ruIpAddress);
            sUpdatedScript = sUpdatedScript.Replace("BASE_SERVER_NAME", usrBaseServer.GetName());
            sUpdatedScript = sUpdatedScript.Replace("BASE_SERVER_PW", usrBaseServer.GetPW());
            sUpdatedScript = sUpdatedScript.Replace("RU_USER_NAME", usrRuUser.GetName());
            sUpdatedScript = sUpdatedScript.Replace("RU_ROOT_NAME", usrRuRoot.GetName());
            if (pw123qweCheckBox.Checked == true)
            {
                sUpdatedScript = sUpdatedScript.Replace("RU_USER_PW", usrRuUserOld.GetPW());
                sUpdatedScript = sUpdatedScript.Replace("RU_ROOT_PW", usrRuRootOld.GetPW());
            }
            else
            {
                sUpdatedScript = sUpdatedScript.Replace("RU_USER_PW", usrRuUser.GetPW());
                sUpdatedScript = sUpdatedScript.Replace("RU_ROOT_PW", usrRuRoot.GetPW());
            }
            sUpdatedScript = sUpdatedScript.Replace("CDU_USER_NAME", usrCduUser.GetName());
            sUpdatedScript = sUpdatedScript.Replace("CDU_USER_PW", usrCduUser.GetPW());
            sUpdatedScript = sUpdatedScript.Replace("CDU_ROOT_NAME", usrCduRoot.GetName());
            sUpdatedScript = sUpdatedScript.Replace("CDU_ROOT_PW", usrCduRoot.GetPW());
            sUpdatedScript = sUpdatedScript.Replace("VDU_USER_NAME", usrVduUser.GetName());
            sUpdatedScript = sUpdatedScript.Replace("VDU_USER_PW", usrVduUser.GetPW());
            sUpdatedScript = sUpdatedScript.Replace("VDU_ROOT_NAME", usrVduRoot.GetName());
            sUpdatedScript = sUpdatedScript.Replace("VDU_ROOT_PW", usrVduRoot.GetPW());
            sUpdatedScript = sUpdatedScript.Replace("FSU_USER_NAME", usrFsuUser.GetName());
            sUpdatedScript = sUpdatedScript.Replace("FSU_USER_PW", usrFsuUser.GetPW());
            sUpdatedScript = sUpdatedScript.Replace("FSU_ROOT_NAME", usrFsuRoot.GetName());
            sUpdatedScript = sUpdatedScript.Replace("FSU_ROOT_PW", usrFsuRoot.GetPW());
            sUpdatedScript = sUpdatedScript.Replace("UPLOAD_FILE_NAME", uploadFilePathOp.getSelFileName());
            sUpdatedScript = sUpdatedScript.Replace("DOWNLOAD_FILE_PATH_IN_RU", filePathInRU);
            sUpdatedScript = sUpdatedScript.Replace("DOWNLOAD_FILE_NAME_IN_RU", fileNameInRU);
            sUpdatedScript = sUpdatedScript.Replace("TIME_STAMP", timeStamp);
            return sUpdatedScript;
        }

        private int scriptExecuter(string script, string scriptFile, SshOp serverSshOp, string[] ipSet, string timeStamp)
        {
            if (script == string.Empty)
                return 0;
            string[] subScript = Regex.Matches(script, @"('[^']*')|([^ ]+)")
                                                    .Cast<Match>()
                                                    .Select(m => m.Value.Trim('\''))
                                                    .ToArray();

            if ((subScript[0].StartsWith("#")) || (subScript[0].EndsWith(":")))
                return 0;
            else if (string.Equals("send", subScript[0]))
                serverSshOp.ExecuteCommand(subScript[1]);
            else if (string.Equals("wait", subScript[0]))
            {
                string[] expOutput = subScript.Skip(1).ToArray();
                waitResult = serverSshOp.WaitForOutput(expOutput, timeoutThreshold);
                if (waitResult < 0)
                {
                    MessageBox.Show("传输失败，请检查IP配置及连接情况！");
                    return -1;
                }
            }
            else if (string.Equals("max_wait_time", subScript[0]))
            {
                if (int.TryParse(subScript[1], out int maxWaitTime))
                {
                    if (maxWaitTime < 0)
                        timeoutThreshold = -1;
                    else
                        timeoutThreshold = maxWaitTime;
                }
                else
                {
                    MessageBox.Show(script + "\n设置最大等待时间失败！");
                    return -1;
                }
            }
            else if (string.Equals("if", subScript[0]))
            {
                if (int.TryParse(subScript[1], out int wantedOutput))
                {
                    if (wantedOutput == waitResult)
                    {
                        string remainCmd = string.Join(" ", subScript.Skip(2));
                        return scriptExecuter(remainCmd, scriptFile, serverSshOp, ipSet, timeStamp);
                    }
                }
                else
                {
                    MessageBox.Show(script + "\nif命令使用方法错误！");
                    return -1;
                }
            }
            else if (string.Equals("goto", subScript[0]))
            {
                int lineId = FindLabel(scriptFile, subScript[1]);
                if (lineId == -1) { return -1; }
                return lineId;
            }
            else if (string.Equals("runscript", subScript[0]))
            {
                string scriptFileName = subScript[1];
                if (!File.Exists(scriptFileName))
                {
                    MessageBox.Show(scriptFileName + "脚本不存在！");
                }
                if (false == script_execute_core(scriptFileName, serverSshOp, ipSet, timeStamp, -1)) 
                    return -1;
            }
            else if (string.Equals("msleep", subScript[0]))
            {
                if (int.TryParse(subScript[1], out int msleepTime))
                {
                    Thread.Sleep(msleepTime);
                }
                else
                {
                    MessageBox.Show(script + "\nusleep命令使用方法错误！");
                    return -1;
                }
            }
            else
            {
                MessageBox.Show("非法命令: " + script);
                return -1;
            }
            return 0;
        }

        private int FindLabel(string filePath, string labelName)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i] == labelName + ":")
                    {
                        return i + 1; // 行号从1开始，数组索引从0开始，所以要加1
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(filePath + "中找不到标签: " + labelName);
            }
            return -1;
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
            TransButtonRefresh();
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
            SshOp serverSshOp = new SshOp(serverIpOp.GetLastIpAddress(TypeSelBox.Text), usrBaseServer.GetName(), usrBaseServer.GetPW());

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

        private void TransModeSelBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            TransModeSwitch();
            TransButtonRefresh();
        }

        private void transModeSwitchButton_Click(object sender, EventArgs e)
        {
            if (string.Equals("PC->RU", TransModeSelBox.SelectedItem))
            {
                TransModeSelBox.SelectedItem = "RU->PC";
            }
            else if (string.Equals("RU->PC", TransModeSelBox.SelectedItem))
            {
                TransModeSelBox.SelectedItem = "PC->RU";
            }
            TransModeSwitch();
            TransButtonRefresh();
        }

        private void TransModeSwitch()
        {
            if (string.Equals("PC->RU", TransModeSelBox.Text))
            {
                fielPathLabel.Text = "文件路径:";
                filePathSel.Text = "选择文件";
                for (int i = 0; i < uploadFilePathOp.PathCount(); i++)
                {
                    if (i == 0)
                    {
                        filePath.Text = uploadFilePathOp.GetPathByIndex(i);
                    }
                    else
                    {
                        Console.WriteLine(uploadFilePathOp.getSelFileNameByIndex(i));
                        filePath.Text += "|" + uploadFilePathOp.getSelFileNameByIndex(i);
                    }
                }
                filePath.TextAlign = ContentAlignment.MiddleLeft;
                dlFileName.Enabled = false;
                dlFileName.Text = String.Empty;
                fileDlLabel.Enabled = false;
                dlHintButton.Enabled = false;
            }
            else if (string.Equals("RU->PC", TransModeSelBox.Text))
            {
                fielPathLabel.Text = "保存路径:";
                filePathSel.Text = "选择路径";
                filePath.Text = dlFileSavePathOp.GetPath();
                filePath.TextAlign = ContentAlignment.MiddleLeft;
                dlFileName.Enabled = true;
                dlFileName.Text = dlFilPathInRuOp.GetPath();
                fileDlLabel.Enabled = true;
                dlHintButton.Enabled = true;
            }
        }

        private void TransButtonRefresh()
        {
            if (globalTransFlag)
                return;

            TransModeContexMenuStrip.Items.Clear();

            if (string.Equals("PC->RU", TransModeSelBox.Text))
                TransModeContexMenuStrip.Items.Add("上传文件", null, TransModeContextMenuItem_Click);
            else
                TransModeContexMenuStrip.Items.Add("下载文件", null, TransModeContextMenuItem_Click);

            TransModeContexMenuStrip.Items.Add("更新RUSW_APP", null, TransModeContextMenuItem_Font9_Click);
            TransModeContexMenuStrip.Items.Add("更新rcm", null, TransModeContextMenuItem_Click);

            TransSplitButton.AutoSize = false;
            TransSplitButton.Text = TransModeContexMenuStrip.Items[0].Text;
            TransSplitButton.Font = new Font("宋体", 12, FontStyle.Regular);

            if (string.Equals("PC->RU", TransModeSelBox.Text))
            {
                if (string.Equals("vDU-FSU-RU", TypeSelBox.Text) || string.Equals("CDU-RU", TypeSelBox.Text))
                {
                    TransModeContexMenuStrip.Items[2].Enabled = false;
                    if (string.Equals("RUSW_APP.out", uploadFilePathOp.getSelFileNameByIndex(0)) || string.Equals("RUSW_APP.out", uploadFilePathOp.getSelFileNameByIndex(1)))
                    {
                        TransSplitButton.Text = TransModeContexMenuStrip.Items[1].Text;
                        TransSplitButton.Font = new Font("宋体", 9, FontStyle.Regular);
                    }
                }
                else if (string.Equals("vDU-ORU", TypeSelBox.Text))
                {
                    TransModeContexMenuStrip.Items[1].Enabled = false;
                    if (uploadFilePathOp.getSelFileNameByIndex(0).Contains("rcm."))
                    {
                        TransSplitButton.Text = TransModeContexMenuStrip.Items[2].Text;
                    }
                }
                else
                {
                    TransModeContexMenuStrip.Items[1].Enabled = false;
                    TransModeContexMenuStrip.Items[2].Enabled = false;
                }
            }
            else
            {
                TransModeContexMenuStrip.Items[1].Enabled = false;
                TransModeContexMenuStrip.Items[2].Enabled = false;
            }
        }

        private void dlHintButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("默认获取路径为/tmp/，如输入abc*，将获取/tmp/路径下abc开头的所有文件\n" +
                "如果输入/home/user/abc，将获取/home/user/路径下的名为abc的文件");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("如果是密码为123qwe的机型，请勾选此框");
        }

        private void swiftCpLabel_Click(object sender, EventArgs e)
        {
            //this.Size = new Size(this.Size.Width, this.Size.Height + 50);
        }

        private void swiftCpButton0_Click(object sender, EventArgs e)
        {
            string command = swiftCpButtionOp.GetCommandByIndex(0);
            command = command.Replace("UPLOAD_FILE_NAME", uploadFilePathOp.getSelFileName());
            Clipboard.SetText(command);
        }

        private void swiftCpButton1_Click(object sender, EventArgs e)
        {
            string command = swiftCpButtionOp.GetCommandByIndex(1);
            command = command.Replace("UPLOAD_FILE_NAME", uploadFilePathOp.getSelFileName());
            Clipboard.SetText(command);
        }

        private void swiftCpButton2_Click(object sender, EventArgs e)
        {
            string command = swiftCpButtionOp.GetCommandByIndex(2);
            command = command.Replace("UPLOAD_FILE_NAME", uploadFilePathOp.getSelFileName());
            Clipboard.SetText(command);
        }

        private void swiftCpButton3_Click(object sender, EventArgs e)
        {
            string command = swiftCpButtionOp.GetCommandByIndex(3);
            command = command.Replace("UPLOAD_FILE_NAME", uploadFilePathOp.getSelFileName());
            Clipboard.SetText(command);
        }

        private void SwiftCopyHelpButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("点击按钮将复制对应的命令到剪贴板！\n" +
                "=====================================================\n" +
                "《自定义快捷复制按钮教程》\n" +
                "通过修改config\\SwiftCopyButton.cfg文件自定义下方快捷复制按钮。\n" +
                "以PkgUpdate:SWM_PkgUpdate \"/tmp/updateFileName\"为例，\n" +
                "英文冒号（:）前为按钮名，英文冒号（:）后为点击按钮后被复制的命令，\n" +
                "本例中按钮名为SWM_PkgUpdate，点击后复制的命令为SWM_PkgUpdate \"/tmp/updateFileName\"\n" + 
                "=====================================================\n" +
                "《自定义快捷复制按钮进阶》\n" +
                "如果使用\"UPLOAD_FILE_NAME\"在自定义命令中，例如：\nPkgUpdate:SWM_PkgUpdate \"/tmp/UPLOAD_FILE_NAME\"\n" +
                "则按钮名为SWM_PkgUpdate\nUPLOAD_FILE_NAME将自动替换为传输到RU的文件名");
        }

        private void pw123qweCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (pw123qweCheckBox.Checked == true)
                pw123qweTypeOp.SaveType("TRUE");
            else
                pw123qweTypeOp.SaveType("FALSE");
        }

        private void TransModeContextMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            TransSplitButton.Text = item.Text;
            TransSplitButton.AutoSize = false;
            TransSplitButton.Font = new Font("宋体", 12, FontStyle.Regular);
        }
        private void TransModeContextMenuItem_Font9_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            TransSplitButton.Text = item.Text;
            TransSplitButton.AutoSize = false;
            TransSplitButton.Font = new Font("宋体", 9, FontStyle.Regular);
        }

    }
}
