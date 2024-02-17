using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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