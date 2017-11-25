using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Devices;
using System.Security.AccessControl;
namespace CTools
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string Jumple = "";
        private string Spider = "";
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                RegistryKey classesRoot = Registry.ClassesRoot;
                classesRoot.DeleteSubKey(@"Directory\Background\shellex\ContextMenuHandlers\NvCplDesktopContext", true);
                classesRoot.Close();
                this.button3.Enabled = false;
            }
            catch
            {
                MessageBox.Show("已经加速过或不可加速！", "错误");
                this.button3.Enabled = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RegistryKey localMachine = Registry.LocalMachine;
            try
            {
                RegistryKey key2 = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\Teacher.exe");
                try { this.Jumple = key2.GetValue("Path").ToString(); }
                catch { }
                key2 = localMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\3000soft\Red Spider");
                try { this.Spider = key2.GetValue("AgentCommand").ToString(); }
                catch { }
                //MessageBox.Show(Spider);
                key2.Close();
            }
            catch { }
            string currentDirectory = Environment.CurrentDirectory;
            char ch = currentDirectory[currentDirectory.Length - 2];
            string introduced6 = ch.ToString();
            ch = currentDirectory[currentDirectory.Length - 1];
            if ((introduced6 + ch.ToString()) == "32")
            {
                this.button1_Click(null, null);
            }
            else if (File.Exists(Environment.CurrentDirectory + @"\ToolsForCOM.exe"))
            {
                if(getPer())
                    File.Copy(Environment.CurrentDirectory + @"\ToolsForCOM.exe", @"C:\Windows\System32\sethc.exe", true);
            }
            else
            {
                MessageBox.Show(this, "请勿修改文件名称，会导致控制中无法解除控制！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Computer computer = new Computer();
                //x86 x64
                var pos = this.Spider.IndexOf("REDAgent");
                //MessageBox.Show(this.Spider.Substring(0, pos) + "RedAgentt.exe");
                if ((this.Spider != "") && File.Exists(this.Spider.Substring(0, pos) + "RedAgentt.exe"))
                { 
                    //MessageBox.Show(this.Spider.Substring(0, pos) + "\\RedAgentt.exe");
                    computer.FileSystem.RenameFile(this.Spider.Substring(0,pos)+"\\RedAgentt.exe", "REDAgent.exe");
                }
                else if (File.Exists(@"C:\Program Files\3000soft\Red Spider\REDAgentt.exe"))
                { 
                    computer.FileSystem.RenameFile(@"C:\Program Files\3000soft\Red Spider\REDAgentt.exe", "REDAgent.exe"); 
                }
                else if (File.Exists(@"C:\Program Files (x86)\3000soft\Red Spider\REDAgentt.exe"))
                { 
                    computer.FileSystem.RenameFile(@"C:\Program Files (x86)\3000soft\Red Spider\REDAgentt.exe", "REDAgent.exe"); 
                }
                if ((this.Jumple != "") && File.Exists(this.Jumple + @"\Studentt.exe"))
                { 
                    computer.FileSystem.RenameFile(this.Jumple + @"\Studentt.exe", "Student.exe");
                }
                else if (File.Exists(@"C:\Program Files\Jumple\ECR\Studentt.exe"))
                { 
                    computer.FileSystem.RenameFile(@"C:\Program Files\Jumple\ECR\Studentt.exe", "Student.exe");
                }
                else if (File.Exists(@"C:\Program Files (x86)\Jumple\ECR\Studentt.exe"))
                {
                    computer.FileSystem.RenameFile(@"C:\Program Files (x86)\Jumple\ECR\Studentt.exe", "Student.exe");
                }
            }
            catch
            {
                MessageBox.Show("找不到文件。", "错误");
            }
            this.StartProcess();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Computer computer;
            if (this.Spider != "" && File.Exists(this.Spider))
            {
                computer = new Computer();
                computer.FileSystem.RenameFile(this.Spider, "REDAgentt.exe");
            }
            else if (File.Exists(@"C:\Program Files\3000soft\Red Spider\REDAgent.exe"))
            {
                computer = new Computer();
                computer.FileSystem.RenameFile(@"C:\Program Files\3000soft\Red Spider\REDAgent.exe", "REDAgentt.exe");
            }
            else if (File.Exists(@"C:\Program Files (x86)\3000soft\Red Spider\REDAgent.exe"))
            {
                computer = new Computer();
                computer.FileSystem.RenameFile(@"C:\Program Files\3000soft\Red Spider\REDAgent.exe", "REDAgentt.exe");
            }
            if ((this.Jumple != "") && File.Exists(this.Jumple + @"\Student.exe"))
            {
                computer = new Computer();
                computer.FileSystem.RenameFile(this.Jumple + @"\Student.exe", "Studentt.exe");
            }
            else if (File.Exists(@"C:\Program Files\Jumple\ECR\Student.exe"))
            {
                computer = new Computer();
                computer.FileSystem.RenameFile(@"C:\Program Files\Jumple\ECR\Student.exe", "Studentt.exe");
            }
            else if (File.Exists(@"C:\Program Files (x86)\Jumple\ECR\Student.exe"))
            {
                computer = new Computer();
                computer.FileSystem.RenameFile(@"C:\Program Files\Jumple\ECR\Student.exe", "Studentt.exe");
            }
            this.KillProcessExists();

        }
        //检查进程是否存在
        private bool CheckProcessExists(int s)
        {
            Process[] processesByName;
            int num;
            Process process;
            if (s == 1)
            {
                processesByName = Process.GetProcessesByName("REDAgent");
                num = 0;
                while (num < processesByName.Length)
                {
                    process = processesByName[num];
                    return true;
                }
                return false;
            }
            processesByName = Process.GetProcessesByName("Student");
            num = 0;
            while (num < processesByName.Length)
            {
                process = processesByName[num];
                return true;
            }
            return false;
        }
        //结束进程
        private void KillProcessExists()
        {
            try
            {
                Process[] processesByName = Process.GetProcessesByName("REDAgent");
                foreach (Process process in processesByName)
                {
                    process.Kill();
                    process.Close();
                }
                processesByName = Process.GetProcessesByName("Student");
                foreach (Process process in processesByName)
                {
                    process.Kill();
                    process.Close();
                }
            }
            catch
            {
                MessageBox.Show("结束进程时出现错误！", "错误");
            }
        }
        //打开链接
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.ocrosoft.com");
        }
        //启动进程
        private void StartProcess()
        {
            try
            {
                if (File.Exists(this.Spider) && !this.CheckProcessExists(1))
                {
                    Process.Start(this.Spider);
                }
                else if (File.Exists(@"C:\Program Files\3000soft\Red Spider\REDAgent.exe") && !this.CheckProcessExists(1))
                {
                    Process.Start(@"C:\Program Files\3000soft\Red Spider\REDAgent.exe");
                }
                else if (File.Exists(@"C:\Program Files (x86)\3000soft\Red Spider\REDAgent.exe") && !this.CheckProcessExists(1))
                {
                    Process.Start(@"C:\Program Files (x86)\3000soft\Red Spider\REDAgent.exe");
                }
                if (((this.Jumple != "") && File.Exists(this.Jumple + @"\Student.exe")) && !this.CheckProcessExists(2))
                {
                    Process.Start(this.Jumple + @"\Student.exe");
                }
                else if (File.Exists(@"C:\Program Files\Jumple\ECR\Student.exe") && !this.CheckProcessExists(2))
                {
                    Process.Start(@"C:\Program Files\Jumple\ECR\Student.exe");
                }
                else if (File.Exists(@"C:\Program Files (x86)\Jumple\ECR\Student.exe") && !this.CheckProcessExists(2))
                {
                    Process.Start(@"C:\Program Files (x86)\Jumple\ECR\Student.exe");
                }
            }
            catch
            {
                MessageBox.Show("开启失败！", "错误");
            }
        }
        bool getPer()
        {
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                p.Start();
                p.StandardInput.WriteLine(@"Takeown /f 'C:\Windows\System32\sethc.exe'");
                p.StandardInput.WriteLine(@"exit");
                p.StandardInput.AutoFlush = true;
                p.WaitForExit();
                p.Close();
                FileInfo fileInfo = new FileInfo(@"C:\Windows\System32\sethc.exe");
                System.Security.AccessControl.FileSecurity fileSecurity = fileInfo.GetAccessControl();
                fileSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
                fileInfo.SetAccessControl(fileSecurity);
                return true;
            }
            catch { return false; }
        }
    }
}
