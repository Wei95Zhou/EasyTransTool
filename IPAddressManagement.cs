using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
                    if (sIpType == "ensf")
                        count++;
                    else if (IsIPAddressValid(parts.Length >= 3 ? parts[2] : null))
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
