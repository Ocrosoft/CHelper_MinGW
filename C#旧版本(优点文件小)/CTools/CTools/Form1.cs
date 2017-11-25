using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualBasic.Devices;
using System.Security.AccessControl;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CTools
{
    public partial class Form1 : Form
    {
        class HotKey
        {
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers,Keys vk);
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool UnregisterHotKey(IntPtr hWnd,int id );
            [Flags()]
            public enum KeyModifiers
            {
                None = 0,
                Alt = 1,
                Ctrl = 2,
                Shift = 4,
                WindowsKey = 8
            }
        }
        public Form1()
        {
            InitializeComponent();
        }
        private string Jumple = "";//注册表江波ECR的路径
        private string Spider = "";//注册表红蜘蛛的路径
        private string Jumple2 = @"C:\Program Files (x86)\Jumple\ECR\";//默认x86江波路径
        private string Spider2 = @"C:\Program Files (x86)\3000soft\Red Spider\";//默认x86红蜘蛛路径
        /// <summary>
        /// 右键加速(删除右键N卡的控制面板)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                RegistryKey classesRoot = Registry.ClassesRoot;
                classesRoot.DeleteSubKey(@"Directory\Background\shellex\ContextMenuHandlers\NvCplDesktopContext", true);
                classesRoot.Close();
            }
            catch
            {
                label2.Text = "已经加速过或不可加速！";
                timer2.Enabled = true;
            }
        }
        /// <summary>
        /// 尝试从注册表获取红蜘蛛和江波ECR的路径，将自己复制到system32下，以支持shift解控
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            RegistryKey localMachine = Registry.LocalMachine;
            try
            {
                RegistryKey key2 = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\Teacher.exe");
                try { Jumple = key2.GetValue("Path").ToString(); }//寻找江波ECR
                catch { }
                key2 = localMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\3000soft\Red Spider");
                if (key2 == null) key2 = localMachine.OpenSubKey(@"SOFTWARE\3000soft\Red Spider");
                try { Spider = key2.GetValue("AgentCommand").ToString(); }//寻找红蜘蛛
                catch { }
                try { if (key2 != null) key2.Close(); }
                catch { }
                if (Spider != "") Spider = Spider.Substring(0, Spider.IndexOf("REDAgent"));
                if (Jumple != "") Jumple = Jumple + "\\";
                if (!File.Exists(Jumple2)) Jumple2 = @"C:\Program Files\Jumple\ECR\";
                if (!File.Exists(Spider2)) Spider2 = @"C:\Program Files\3000soft\Red Spider\";
            }
            catch { }
            string strFullPath = Application.ExecutablePath;
            if (strFullPath.ToLower().IndexOf("system32") != -1)//判断是不是从system32文件夹启动的
            {
                button1_Click(null, null);//自动解除控制
                button6.Enabled = false;//补丁文件无法找到，禁用
                timer2.Enabled = true;
            }
            else
            {
                getPer();//获取sethc权限
                try
                {
                    //复制自身
                    File.Copy(strFullPath, @"C:\Windows\System32\sethc.exe", true);
                    label2.Text = "补丁创建成功\n控制中连续按5下以上shift键\n可以解除控制";
                    timer2.Enabled = true;
                }
                catch
                {
                    label2.Text = "补丁创建失败\n控制中解除功能将无法使用";
                    timer2.Enabled = true;
                }
            }
        }
        /// <summary>
        /// 启用控制，文件名改回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Computer computer = new Computer();
                //红蜘蛛
                if ((Spider != "") && File.Exists(Spider + "RedAgentt.exe"))
                    computer.FileSystem.RenameFile(Spider + "RedAgentt.exe", "REDAgent.exe");
                else if (File.Exists(Spider + "REDAgentt.exe"))
                    computer.FileSystem.RenameFile(Spider + "REDAgentt.exe", "REDAgent.exe");
                //江波ECR
                if ((Jumple != "") && File.Exists(Jumple + "Studentt.exe"))
                    computer.FileSystem.RenameFile(Jumple + "Studentt.exe", "Student.exe");
                else if (File.Exists(Jumple2 + "Studentt.exe"))
                    computer.FileSystem.RenameFile(Jumple2 + "Studentt.exe", "Student.exe");
            }
            catch
            {
                label2.Text = "找不到文件，解除控制失败";
                timer2.Enabled = true;
            }
            StartProcess();
            label2.Text = "操作成功，结果请稍候...";
            timer2.Enabled = true;
        }
        /// <summary>
        /// 解除控制，改变文件名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Computer computer = new Computer();
                //红蜘蛛
                if (Spider != "" && File.Exists(Spider + "REDAgent.exe"))
                    computer.FileSystem.RenameFile(Spider + "REDAgent.exe", "REDAgentt.exe");
                else if (File.Exists(Spider2 + "REDAgent.exe"))
                    computer.FileSystem.RenameFile(Spider2 + "REDAgent.exe", "REDAgentt.exe");
                //江波ECR
                if ((Jumple != "") && File.Exists(Jumple + "Student.exe"))
                    computer.FileSystem.RenameFile(Jumple + "Student.exe", "Studentt.exe");
                else if (File.Exists(Jumple2 + "Student.exe"))
                    computer.FileSystem.RenameFile(Jumple2 + "Student.exe", "Studentt.exe");

                KillProcessExists();//结束进程
            }
            catch
            {
                label2.Text = "无法找到文件，解除控制失败";
            }

            label2.Text = "操作成功，结果请稍候...";
            timer2.Enabled = true;
        }
        /// <summary>
        /// 检查进程是否存在
        /// </summary>
        /// <param name="s">1表示红蜘蛛，2表示江波ECR</param>
        /// <returns></returns>
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
        /// <summary>
        /// 结束进程
        /// </summary>
        private bool KillProcessExists()
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
                return true;
            }
            catch
            {
                label2.Text = "无法结束进程!解除控制失败";
                timer2.Enabled = true;
                return false;
            }
        }
        /// <summary>
        /// 打开主页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.ocrosoft.com");
        }
        /// <summary>
        /// 启动进程
        /// </summary>
        private void StartProcess()
        {
            try
            {
                if (Spider != "" && File.Exists(Spider + "REDAgent.exe") && !CheckProcessExists(1))
                    Process.Start(Spider + "REDAgent.exe");
                else if (File.Exists(Spider2 + "REDAgent.exe") && !CheckProcessExists(1))
                    Process.Start(Spider2 + "REDAgent.exe");

                if (Jumple != "" && File.Exists(Jumple + "Student.exe") && !CheckProcessExists(2))
                    Process.Start(Jumple + "Student.exe");
                else if (File.Exists(Jumple2 + "Student.exe") && !CheckProcessExists(2))
                    Process.Start(Jumple2 + "Student.exe");
            }
            catch
            {
                label2.Text = "无法创建进程!开启控制失败";
                timer2.Enabled = true;
            }
        }
        /// <summary>
        /// 获取sethc的权限
        /// </summary>
        /// <returns></returns>
        bool getPer()
        {
            try
            {
                string file = @"D:\run.bat";
                //如果D:\run.bat不存在，则创建
                if (!File.Exists(file))
                {
                    FileStream myFs = new FileStream(file, FileMode.Create);
                    StreamWriter mySw = new StreamWriter(myFs);
                    mySw.Write(@"Takeown /f ""C:\Windows\System32\sethc.exe""");
                    mySw.Close();
                    myFs.Close();
                }
                //隐藏窗口启动run.bat
                ProcessStartInfo myStartInfo = new ProcessStartInfo();
                myStartInfo.FileName = file;
                Process myProcess = new Process();
                myProcess.StartInfo = myStartInfo;
                myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                myProcess.Start();
                myProcess.WaitForExit();
                FileInfo fileInfo = new FileInfo(@"C:\Windows\System32\sethc.exe");
                FileSecurity fileSecurity = fileInfo.GetAccessControl();
                fileSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
                fileInfo.SetAccessControl(fileSecurity);
                return true;
            }
            catch
            {
                label2.Text = "无法获得权限\n控制中解控功能将无法使用\n请尝试以管理员身份运行";
                timer2.Enabled = true;
                return false;
            }
        }
        /// <summary>
        /// 移除提示，继续显示状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            string tmp = "当前状态:\n";
            if (File.Exists(Spider == "" ? Spider2 : Spider + "REDAgentFull.exe")) tmp += "红蜘蛛:窗口补丁";
            else if (CheckProcessExists(1)) tmp += "红蜘蛛:未解除";
            else if (File.Exists(Spider == "" ? Spider2 : Spider + "REDAgentt.exe")) tmp += "红蜘蛛:已解除";
            else if (Spider == "" && !File.Exists(Spider2 + "REDAgent.exe")) tmp += "红蜘蛛:未安装";
            else tmp += "红蜘蛛:未知(或被删除)";

            tmp += '\n';

            if (CheckProcessExists(2)) tmp += "江波ECR:未解除";
            else if (File.Exists(Jumple == "" ? Jumple2 : Jumple + "Studentt.exe")) tmp += "江波ECR:已解除";
            else if (Jumple == "" && !File.Exists(Jumple2 + "Student.exe")) tmp += "江波ECR:未安装";
            else tmp += "江波ECR:未知(或被删除)";

            label2.Text = tmp;
            timer2.Enabled = false;
        }
        /// <summary>
        /// 收起/展开说明
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            //561, 332
            if (button5.Text == "<")
            {
                Size = new Size(287, 322);
                button5.Text = ">";
            }
            else
            {
                Size = new Size(551, 322);
                button5.Text = "<";
            }
        }
        /// <summary>
        /// 红蜘蛛窗口化功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Spider == "" ? Spider2 : Spider + "REDAgent.exe"))
            {
                label2.Text = "仅适用于红蜘蛛\n江波ECR无法使用\n请使用解除控制功能";
                timer2.Enabled = true;
                return;
            }
            if (File.Exists(Spider == "" ? Spider2 : Spider + "REDAgentFull.exe"))
            {
                label2.Text = "已经安装过补丁\n无法再次安装\n请删除补丁后重试\n";
                timer2.Enabled = true;
                return;
            }
            string strFullPath = Application.ExecutablePath;
            try
            {
                Computer computer = new Computer();
                string path = "";
                if (Spider != "" && File.Exists(Spider + "REDAgent.exe"))
                {
                    computer.FileSystem.RenameFile(Spider + "REDAgent.exe", "REDAgentFull.exe");
                    path = Spider;
                }
                else if (File.Exists(Spider2 + "REDAgent.exe"))
                {
                    computer.FileSystem.RenameFile(Spider2 + "REDAgent.exe", "REDAgentFull.exe");
                    path = Spider2;
                }
                string name = Path.GetFileName(strFullPath);
                File.Copy(strFullPath.Substring(0, strFullPath.Length - name.Length) + "REDAgent.exe", path + "REDAgent.exe", true);
                KillProcessExists();
                Process.Start(path + "REDAgent.exe");
                label2.Text = "操作成功,结果请稍候...";
                timer2.Enabled = true;
            }
            catch
            {
                label2.Text = "补丁安装失败";
                timer2.Enabled = true;
            }
        }
        /// <summary>
        /// 删除窗口化补丁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            string path;
            if (Spider != "") path = Spider;
            else path = Spider2;
            if (!File.Exists(path + "REDAgentFull.exe"))
            {
                label2.Text = "未安装补丁,无法删除.";
                timer2.Enabled = true;
                return;
            }
            try
            {
                File.Delete(path + "tmp.exe");
                Computer computer = new Computer();
                computer.FileSystem.RenameFile(path + "REDAgent.exe", "tmp.exe");
                KillProcessExists();
                computer.FileSystem.RenameFile(path + "REDAgentFull.exe", "REDAgent.exe");
                if (!CheckProcessExists(1)) Process.Start(path + "REDAgent.exe");
                label2.Text = "操作成功,结果请稍候...";
                timer2.Enabled = true;
            }
            catch
            {
                label2.Text = "删除补丁失败";
                timer2.Enabled = true;
            }
        }
    }
}
