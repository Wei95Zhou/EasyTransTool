using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
