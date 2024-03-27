using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using System.Windows.Forms;

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
                var items = sftpClient.ListDirectory(remoteFilePath);
                foreach (var item in items)
                {
                    if(item.IsDirectory && !item.Name.StartsWith("."))
                    {
                        string remoteSubDir = remoteFilePath + "/" + item.Name;
                        string localSubDir = Path.Combine(localFilePath, item.Name);
                        Directory.CreateDirectory(localSubDir);
                        DownloadFile(remoteSubDir, localSubDir);
                    }
                    else if (!item.IsDirectory)
                    {
                        string remoteFile = remoteFilePath + "/" + item.Name;
                        string localFileName = CleanFileName(item.Name);
                        if (localFileName != item.Name)
                        {
                            MessageBox.Show($"文件名包含非法字符：{item.Name}，下载的文件名已将该字符删除！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        string localFile = Path.Combine(localFilePath, localFileName);

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
                MessageBox.Show($"下载文件失败: {ex.Message}");
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
                MessageBox.Show($"上传文件失败: {ex.Message}" + remoteFilePath);
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

        private string CleanFileName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
        }
    }
}
