using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        public string GetPathByIndex(int iPathIndex)
        {
            int iPathId = 0;
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
                    if (iPathId == iPathIndex)
                    {
                        lastSelectedPath = line.Substring(line.IndexOf(' ') + 1);
                        break;
                    }
                    iPathId++;
                }
            }
            return lastSelectedPath; // or you can throw an exception to indicate an invalid index
        }
        public void SavePath(string selectedFilePath, bool bReplaceFlag)
        {
            string sPathInfo = sPathType + ' ' + selectedFilePath;
            if (!File.Exists(sFilePath))
            {
                File.WriteAllText(sFilePath, sPathInfo);
                return;
            }
            if (bReplaceFlag == true)
            {
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
            else
            {
                sPathInfo += Environment.NewLine;
                File.AppendAllText(sFilePath, sPathInfo);
            }
        }
        public void DeletePath()
        {
            if (!File.Exists(sFilePath))
            {
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
            }
        }
        public int PathCount()
        {
            int PathCnt = 0;
            if (!File.Exists(sFilePath))
            {
                return 0;
            }
            string[] lines = File.ReadAllLines(sFilePath);
            foreach (string line in lines)
            {
                if (line.StartsWith(sPathType))
                {
                    PathCnt++;
                }
            }
            return PathCnt;
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
        public string getSelFileNameByIndex(int iPathIndex)
        {
            string configFilePath = GetPathByIndex(iPathIndex);
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
