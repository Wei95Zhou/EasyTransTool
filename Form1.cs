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
using SwiftCopyButtonManagement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Text.RegularExpressions;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.Reflection;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics;

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
        private string sRelVer = "2.4.3";

        IPAddressOp duIpOp = new IPAddressOp("DuIp", "./config/IpDataSet.cfg");
        IPAddressOp ruIpOp = new IPAddressOp("RuIp", "./config/IpDataSet.cfg");
        IPAddressOp ensfOp = new IPAddressOp("ensf", "./config/IpDataSet.cfg");
        IPAddressOp fsuIpOp = new IPAddressOp("FsuIp", "./config/IpDataSet.cfg");
        IPAddressOp serverIpOp = new IPAddressOp("ServerIp", "./config/IpDataSet.cfg");
        TypeOp transModeTypeOp = new TypeOp("TransModeType", "PC->RU", "./config/Type.cfg");
        TypeOp devTypeOp = new TypeOp("LinkType", "CDU-RU", "./config/Type.cfg");
        FilePathOp uploadFilePathOp = new FilePathOp("UploadFilePath", "C:", "./config/Path.cfg");
        FilePathOp dlFileSavePathOp = new FilePathOp("DownloadFileSavePath", "C:", "./config/Path.cfg");
        FilePathOp dlFilPathInRuOp = new FilePathOp("FilePathInRu", "/tmp/", "./config/Path.cfg");
        FilePathOp newVerPathOp = new FilePathOp("NewVerPath", "C:", "./config/Path.cfg");
        FilePathOp newVerChkPathOp = new FilePathOp("NewVerChkPath", "C:", "./config/Path.cfg");
        UserManager usrTest = new UserManager("./config/UserMng.cfg", "testUser");
        UserManager usr1168fw = new UserManager("./config/UserMng.cfg", "1168fw");
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

            initAllObject();
        }

        private void initAllObject()
        {
            TransModeSelBox.Enabled = true;
            transModeSwitchButton.Enabled = true;
            filePathSel.Enabled = true;
            TypeSelBox.Enabled = true;
            duIpDelButton.Enabled = true;
            ruIpDelButton.Enabled = true;
            uploadButton.Enabled = true;
            pw123qweCheckBox.Enabled = true;

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
                fileDialog.InitialDirectory = uploadFilePathOp.GetPath();

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = fileDialog.FileName;
                    if (uploadFilePathOp.FileNameValid(selectedFilePath))
                    {
                        filePath.Text = selectedFilePath;
                        uploadFilePathOp.SavePath(selectedFilePath);
                    }
                    else
                    {
                        MessageBox.Show("文件名存在不支持的字符，仅支持A~Za~z-_.:");
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
                        dlFileSavePathOp.SavePath(folderBrowserDialog.SelectedPath);
                    }
                }
            }
        }

        string scriptPath;

        private void uploadButton_Click(object sender, EventArgs e)
        {
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
            string duIpAddress = (string)receiver[0];
            string ruIpAddress = (string)receiver[1];
            string fsuIpAddress = (string)receiver[2];
            string ensfAddress = (string)receiver[3];
            fileTransBGWorker.ReportProgress(8);
            if (string.Equals("PC->RU", transModeTypeOp.GetType()))
            {
                upload_update_Core(duIpAddress, ruIpAddress, fsuIpAddress, ensfAddress);
            }
            else if (string.Equals("RU->PC", transModeTypeOp.GetType()))
            {
                download_update_Core(duIpAddress, ruIpAddress, fsuIpAddress, ensfAddress);
            }
            initAllObject();
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
            uploadButton.Enabled = false;
            pw123qweCheckBox.Enabled = false;
            return;
        }

        private string fileTransScriptPreCheck()
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
                    dlFilPathInRuOp.SavePath(dlFileName.Text);
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
                if (!File.Exists(uploadFilePathOp.GetPath()))
                {
                    MessageBox.Show("本地文件不存在！");
                    return false;
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

        private void upload_update_Core(string duIpAddress, string ruIpAddress, string fsuIpAddress, string ensfAddress)
        {
            // Start update procedure
            // 1.Put file to 116.8 server
            /*SshOp serverSshOp = new SshOp(serverIpOp.GetLastIpAddress(devTypeOp.GetType()), usrTest.GetName(), usrTest.GetPW());
            SftpOp serverSftpOp = new SftpOp(serverIpOp.GetLastIpAddress(devTypeOp.GetType()), usrTest.GetName(), usrTest.GetPW());
            string filePathInServer = "/home/zw/" + Environment.UserName + "/";*/
            SshOp serverSshOp = new SshOp(serverIpOp.GetLastIpAddress(devTypeOp.GetType()), usr1168fw.GetName(), usr1168fw.GetPW());
            SftpOp serverSftpOp = new SftpOp(serverIpOp.GetLastIpAddress(devTypeOp.GetType()), usr1168fw.GetName(), usr1168fw.GetPW());
            string filePathInServer = "/home/" + usr1168fw.GetName() + "/tmp/" + Environment.UserName + "/";

            string timeStamp = Regex.Replace(DateTime.Now.TimeOfDay.ToString(), @"[^\d]", "");
            string fileUploadFileName = uploadFilePathOp.getSelFileName();

            string tempFilePathInServer = filePathInServer + timeStamp;

            //每一句命令都需要检查返回值
            fileTransBGWorker.ReportProgress(10);
            if (true == serverSshOp.Connect())
            {
                serverSshOp.RunCommand("mkdir -p " + tempFilePathInServer);
                serverSshOp.Disconnect();
            }
            fileTransBGWorker.ReportProgress(15);
            if (true == serverSftpOp.Connect())
            {
                serverSftpOp.UploadFile(uploadFilePathOp.GetPath(), tempFilePathInServer + "/" + fileUploadFileName);
                serverSftpOp.Disconnect();
            }
            fileTransBGWorker.ReportProgress(40);
            if (true == serverSshOp.Connect())
            {
                if(false == script_execute_core(serverSshOp, duIpAddress, ruIpAddress, fsuIpAddress, ensfAddress, timeStamp, 40))
                {
                    serverSshOp.RunCommand("rm -rf " + tempFilePathInServer);
                    serverSshOp.Disconnect();
                    return;
                }
                serverSshOp.RunCommand("rm -rf " + tempFilePathInServer);
                serverSshOp.Disconnect();
            }
            fileTransBGWorker.ReportProgress(100);
        }

        private void download_update_Core(string duIpAddress, string ruIpAddress, string fsuIpAddress, string ensfAddress)
        {
            // Start update procedure
            // 1.Put file to 116.8 server
            /*SshOp serverSshOp = new SshOp(serverIpOp.GetLastIpAddress(devTypeOp.GetType()), usrTest.GetName(), usrTest.GetPW());
            SftpOp serverSftpOp = new SftpOp(serverIpOp.GetLastIpAddress(devTypeOp.GetType()), usrTest.GetName(), usrTest.GetPW());
            string filePathInServer = "/tmp/";*/
            SshOp serverSshOp = new SshOp(serverIpOp.GetLastIpAddress(devTypeOp.GetType()), usr1168fw.GetName(), usr1168fw.GetPW());
            SftpOp serverSftpOp = new SftpOp(serverIpOp.GetLastIpAddress(devTypeOp.GetType()), usr1168fw.GetName(), usr1168fw.GetPW());
            string filePathInServer = "/home/" + usr1168fw.GetName() + "/tmp/" + Environment.UserName + "/";

            string timeStamp = Regex.Replace(DateTime.Now.TimeOfDay.ToString(), @"[^\d]", "");
            string tempFilePathInServer = filePathInServer + timeStamp;


            //每一句命令都需要检查返回值
            fileTransBGWorker.ReportProgress(10);
            if (true == serverSshOp.Connect())
            {
                serverSshOp.RunCommand("mkdir -p " + tempFilePathInServer);
                if(false == script_execute_core(serverSshOp, duIpAddress, ruIpAddress, fsuIpAddress, ensfAddress, timeStamp, 10))
                {
                    serverSshOp.RunCommand("rm -rf " + tempFilePathInServer);
                    serverSshOp.Disconnect();
                    return;
                }
                serverSshOp.Disconnect();
            }
            fileTransBGWorker.ReportProgress(70);
            if (true == serverSftpOp.Connect())
            {
                //考虑在脚本里实现文件下载过程的调用，这样可以更好进行进度条的控制
                serverSftpOp.DownloadFile(tempFilePathInServer, dlFileSavePathOp.GetPath());
                serverSftpOp.Disconnect();
            }
            fileTransBGWorker.ReportProgress(95);
            if (true == serverSshOp.Connect())
            {
                serverSshOp.RunCommand("rm -rf " + tempFilePathInServer);
                serverSshOp.Disconnect();
            }
            fileTransBGWorker.ReportProgress(100);
        }

        private bool script_execute_core(SshOp serverSshOp, string duIpAddress, string ruIpAddress, string fsuIpAddress, string ensfAddress, string timeStamp, int transBasePercent)
        {
            double scriptLineCnt = File.ReadLines(scriptPath).Count();
            double lineCounter = 0;
            if (true == serverSshOp.StartShell())
            {
                try
                {
                    using (StreamReader reader = new StreamReader(scriptPath))
                    {
                        string line;
                        logFile.ClearFile();
                        logFile.AppendToFile("running " + scriptPath + "\n");
                        while ((line = reader.ReadLine()) != null)
                        {
                            line = scriptUpdater(line, duIpAddress, ruIpAddress, fsuIpAddress, ensfAddress, timeStamp);
                            if((line == string.Empty) || (false == scriptExecuter(line, serverSshOp)))
                            {
                                return false;
                            }
                            lineCounter++;
                            fileTransBGWorker.ReportProgress(transBasePercent + (int)((lineCounter / scriptLineCnt) * 60));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to read file: " + ex.Message);
                }
            }
            return true;
        }

        private string scriptUpdater(string line, string duIpAddress, string ruIpAddress, string fsuIpAddress, string ensfAddress, string timeStamp)
        {
            string sUpdatedScript = line;
            string userInputFilePathInRU = dlFilPathInRuOp.GetPath();
            string filePathInRU;
            string fileNameInRU;

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
        private bool scriptExecuter(string script, SshOp serverSshOp)
        {
            Console.WriteLine(script);
            logFile.AppendToFile(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]") + script + "\n");
            if (script.StartsWith("#") || (script == string.Empty))
            {
                return true;
            }
            else if (script.StartsWith("sendln "))
            {
                string command = GetCommand(script);
                serverSshOp.ExecuteCommand(command);
            }
            else if (script.StartsWith("wait "))
            {
                string expectedString = GetExpectedString(script);
                serverSshOp.WaitForOutput(expectedString);
            }
            else if (script.StartsWith("wait_timer "))
            {
                string expectedString = GetExpectedString(script);
                if (false == serverSshOp.WaitForOutput_Timer(expectedString))
                {
                    MessageBox.Show("传输失败，请检查IP配置及连接情况！");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Invalid command: " + script);
            }
            return true;
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
            SshOp serverSshOp = new SshOp(serverIpOp.GetLastIpAddress(TypeSelBox.Text), usr1168fw.GetName(), usr1168fw.GetPW());
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
        }

        private void TransModeSwitch()
        {
            if (string.Equals("PC->RU", TransModeSelBox.Text))
            {
                fielPathLabel.Text = "文件路径";
                uploadButton.Text = "上传文件";
                filePathSel.Text = "选择文件";
                filePath.Text = uploadFilePathOp.GetPath();
                dlFileName.Enabled = false;
                dlFileName.Text = String.Empty;
                fileDlLabel.Enabled = false;
                dlHintButton.Enabled = false;
            }
            else if (string.Equals("RU->PC", TransModeSelBox.Text))
            {
                fielPathLabel.Text = "保存路径";
                uploadButton.Text = "下载文件";
                filePathSel.Text = "选择路径";
                filePath.Text = dlFileSavePathOp.GetPath();
                dlFileName.Enabled = true;
                dlFileName.Text = dlFilPathInRuOp.GetPath();
                fileDlLabel.Enabled = true;
                dlHintButton.Enabled = true;
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
    }
}

namespace IPAddressManagement
{
    //LinkType: 0:vDU-ORU 1:CDU-RU 2:vDU-FSU-RU
    //IpType: 0:DuIp 1:FsuIp 2:RuIp 3:ensf 4:ServerIp
    public class TypeOp
    {
        private string sType;
        private string sFilePath;
        private string sDefaultValue;
        public TypeOp(string sType, string sDefaultValue, string sFilePath)
        {
            this.sType = sType;
            this.sFilePath = sFilePath;
            this.sDefaultValue = sDefaultValue;
            if (!File.Exists(sFilePath))
            {
                using (StreamWriter writer = File.CreateText(sFilePath))
                {
                    writer.WriteLine(sType + " " + sDefaultValue);
                }
            }
        }
        public void SaveType(string sSaveType)
        {
            string sTypeInfo = sType + " " + sSaveType;
            if (!File.Exists(sFilePath))
            {
                File.WriteAllText(sFilePath, sTypeInfo);
                return;
            }

            string[] lines = File.ReadAllLines(sFilePath);
            using (StreamWriter writer = new StreamWriter(sFilePath))
            {
                foreach (string line in lines)
                {
                    if (!line.StartsWith(sType))
                    {
                        writer.WriteLine(line);
                    }
                }
                writer.WriteLine(sTypeInfo);
            }
        }
        public string GetType()
        {
            string lastSelectedType = sDefaultValue;
            if (!File.Exists(sFilePath))
            {
                return lastSelectedType;
            }
            string[] lines = File.ReadAllLines(sFilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(' ');
                if (parts.Length >= 2 && parts[0] == sType)
                {
                    lastSelectedType = line.Substring(line.IndexOf(' ') + 1);
                }
            }
            //这里需要根据类型返回有效值，避免其他bug
            return lastSelectedType;
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
        private string sDefaultPath;
        public FilePathOp(string sPathType, string sDefaultPath, string sFilePath)
        {
            this.sPathType = sPathType;
            this.sFilePath = sFilePath;
            this.sDefaultPath = sDefaultPath;
            if (!File.Exists(sFilePath))
            {
                using (StreamWriter writer = File.CreateText(sFilePath))
                {
                    writer.WriteLine(sPathType + " " + sDefaultPath);
                }
            }
        }
        public string GetPath()
        {
            string lastSelectedPath = sDefaultPath;
            if (!File.Exists(sFilePath))
            {
                lastSelectedPath = sDefaultPath;
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
                var files = sftpClient.ListDirectory(remoteFilePath);
                foreach (var file in files)
                {
                    if (!file.IsDirectory)
                    {
                        string remoteFile = remoteFilePath + "/" + file.Name;
                        string localFile = Path.Combine(localFilePath, file.Name);

                        using (Stream fileStream = File.Create(localFile))
                        {
                            sftpClient.DownloadFile(remoteFile, fileStream);
                        }
                    }
                }
                Console.WriteLine("所有文件已下载完成。");
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
    public class UserManager
    {
        private string sFilePath;
        private string sUserType;

        public UserManager(string sFilePath, string sUserType)
        {
            this.sFilePath = sFilePath;
            this.sUserType = sUserType;
            if (!File.Exists(sFilePath))
            {
                MessageBox.Show("用户名及密码配置缺失，将无法传输文件！");
                using (StreamWriter writer = File.CreateText(sFilePath))
                {
                    writer.WriteLine("");
                }
            }
        }

        public string GetName()
        {
            string sName = string.Empty;
            if (!File.Exists(sFilePath))
            {
                sName = string.Empty;
                return sName;
            }
            string[] lines = File.ReadAllLines(sFilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length >= 2 && parts[0] == sUserType)
                {
                    sName = parts[1];
                }
            }
            return sName;
        }
        public string GetPW()
        {
            string sPassword = string.Empty;
            if (!File.Exists(sFilePath))
            {
                sPassword = string.Empty;
                return sPassword;
            }
            string[] lines = File.ReadAllLines(sFilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length >= 2 && parts[0] == sUserType)
                {
                    sPassword = parts[2];
                }
            }
            return sPassword;
        }
    }
}

namespace SwiftCopyButtonManagement
{
    public class SwiftCpButtonManager
    {
        private string sFilePath;

        public SwiftCpButtonManager(string sFilePath)
        {
            this.sFilePath = sFilePath;
            if (!File.Exists(sFilePath))
            {
                MessageBox.Show("快捷复制配置文件缺失，无法使用此功能！");
                using (StreamWriter writer = File.CreateText(sFilePath))
                {
                    writer.WriteLine("");
                }
            }
        }
        public string GetNameByIndex(int index)
        {
            int lineCount = 0;
            string sName = string.Empty;
            if (!File.Exists(sFilePath))
            {
                sName = string.Empty;
                return sName;
            }
            string[] lines = File.ReadAllLines(sFilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if ((parts.Length >= 2) && (lineCount == index))
                {
                    sName = parts[0];
                    return sName;
                }
                lineCount++;
            }
            return sName;
        }
        public string GetCommandByIndex(int index)
        {
            int lineCount = 0;
            string sCommand = string.Empty;
            if (!File.Exists(sFilePath))
            {
                sCommand = string.Empty;
                return sCommand;
            }
            string[] lines = File.ReadAllLines(sFilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if ((parts.Length >= 2) && (lineCount == index))
                {
                    sCommand = parts[1];
                    return sCommand;
                }
                lineCount++;
                
            }
            return sCommand;
        }
    }
}
