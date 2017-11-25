using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualBasic.Devices;
using System.Security.AccessControl;
using System.Runtime.InteropServices;
using CSharpWin;
using System.Drawing;

namespace CTools
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string Jumple = "";//注册表江波ECR的路径
        private string Spider = "";//注册表红蜘蛛的路径
        private string Jumple2 = @"C:\Program Files (x86)\Jumple\ECR\";//默认x86江波路径
        private string Spider2 = @"C:\Program Files (x86)\3000soft\Red Spider\";//默认x86红蜘蛛路径
        private bool virtu = VirtualDesktop.Current._name.IndexOf("Default") == -1 ? true : false;//true说明是虚拟桌面
        private bool newMode = VirtualDesktop.Current._name.IndexOf("Default") == -1 ? true : false;//新的解除控制方案
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
                button3.Enabled = false;
            }
            catch
            {
                label2.Text = "已经加速过或不可加速！";
                button3.Enabled = false;
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
            if (!virtu)//是默认桌面
            {
                RegisterHotKey(Handle, 0x10000, MODKEY.MOD_CONTROL, Keys.D1);//注册Ctrl+1
                RegisterHotKey(Handle, 0x10002, MODKEY.MOD_CONTROL, Keys.NumPad1);//注册Ctrl+Num1
                label3.Text = label3.Text + "1";//修改说明
                label4.Text = "解除控制2(自由桌面):Ctrl+1";
            }
            else
            {
                RegisterHotKey(Handle, 0x10001, MODKEY.MOD_CONTROL, Keys.D2);//注册Ctrl+2
                RegisterHotKey(Handle, 0x10002, MODKEY.MOD_CONTROL, Keys.NumPad2);
                label3.Text = label3.Text + "2";
                label4.Text = "启用控制2(控制桌面):Ctrl+2";
                button1.Enabled = false;//虚拟桌面打开的，禁用所有按钮
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                label1.Visible = false;
            }
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
                if (key2 != null) key2.Close();
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
            }
            else if (!virtu && getPer())//如果不是在虚拟桌面启动的，获得sethc权限
            {
                try
                {
                    File.Copy(strFullPath, @"C:\Windows\System32\sethc.exe", true);
                    string name = Path.GetFileName(strFullPath);
                    File.Copy(strFullPath.Substring(0, strFullPath.Length - name.Length) + "DesktopManager.dll", @"C:\Windows\System32\DesktopManager.dll", true);
                    label2.Text = "补丁创建成功\n控制中连续按5下以上shift键\n可以解除控制";
                    timer2.Enabled = true;
                }
                catch
                {
                    label2.Text = "补丁创建失败\n控制中解除功能将无法使用";
                    timer2.Enabled = true;
                }
            }
            timer2.Enabled = true;//调用防止空白
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

                if ((Spider != "") && File.Exists(Spider + "RedAgentt.exe"))
                    computer.FileSystem.RenameFile(Spider + "RedAgentt.exe", "REDAgent.exe");
                else if (File.Exists(Spider + "REDAgentt.exe"))
                    computer.FileSystem.RenameFile(Spider + "REDAgentt.exe", "REDAgent.exe");

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
            Computer computer = new Computer();
            if (Spider != "" && File.Exists(Spider + "REDAgent.exe"))
                computer.FileSystem.RenameFile(Spider + "REDAgent.exe", "REDAgentt.exe");
            else if (File.Exists(Spider2 + "REDAgent.exe"))
                computer.FileSystem.RenameFile(Spider2 + "REDAgent.exe", "REDAgentt.exe");

            if ((Jumple != "") && File.Exists(Jumple + "Student.exe"))
                computer.FileSystem.RenameFile(Jumple + "Student.exe", "Studentt.exe");
            else if (File.Exists(Jumple2 + "Student.exe"))
                computer.FileSystem.RenameFile(Jumple2 + "Student.exe", "Studentt.exe");

            KillProcessExists();//结束进程
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
                label2.Text = "无法结束进程!解除控制失败";
                timer2.Enabled = true;
            }
        }
        /// <summary>
        /// 打开主页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.ocrosoft.com");
        }
        /// <summary>
        /// 启动进程
        /// </summary>
        private void StartProcess()
        {
            try
            {
                if (Spider != "" && File.Exists(Spider + "REDAgent") && !CheckProcessExists(1))
                    Process.Start(Spider + "REDAgent");
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
                label2.Text = "无法获得权限，\n控制中解控功能将无法使用.\n尝试以管理员身份运行";
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
            if (CheckProcessExists(1) && newMode) tmp = tmp + "红蜘蛛:已解除2";
            else if (File.Exists(Spider == "" ? Spider2 : Spider + "REDAgentFull.exe")) tmp = tmp + "红蜘蛛:窗口补丁";
            else if (CheckProcessExists(1)) tmp = tmp + "红蜘蛛:未解除";
            else if (Spider == "" && !File.Exists(Spider2 + "REDAgent.exe")) tmp = tmp + "红蜘蛛:未安装";
            else if (File.Exists(Spider == "" ? Spider2 : Spider + "REDAgentt.exe")) tmp = tmp + "红蜘蛛:已解除1";
            else tmp = tmp + "红蜘蛛:未知";
            tmp += '\n';
            if (CheckProcessExists(2) && newMode) tmp = tmp + "江波ECR:已解除2";
            else if (CheckProcessExists(2)) tmp = tmp + "江波ECR:未解除";
            else if (Jumple == "" && !File.Exists(Jumple2 + "Student.exe")) tmp = tmp + "江波ECR:未安装";
            else if (File.Exists(Jumple == "" ? Jumple2 : Jumple + "Student.exe")) tmp = tmp + "江波ECR:已解除1";
            else tmp += "江波ECR:未知";
            label2.Text = tmp;
            timer2.Enabled = false;
        }
        private VirtualDesktop vd;
        /// <summary>
        /// 创建虚拟桌面并切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            label2.Text = "注意:不要关闭任何一个桌面的本程序,\n关闭后将无法切换桌面(开启/解除控制)";
            if (MessageBox.Show("请关闭所有打开的程序后解除控制。\n确认关闭了所有的程序?", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            newMode = true;
            timer2.Enabled = true;
            if (vd == null && VirtualDesktop.Current._name.IndexOf("Default") != -1)
            {
                vd = new VirtualDesktop("VDesktop");//创建虚拟桌面
                vd.CreateProcess("explorer.exe");//启动桌面浏览器
                vd.CreateProcess("ctfmon.exe");//启动输入法
                vd.CreateProcess(Application.ExecutablePath);
                vd.Show();
            }
            button2_Click(null, null);//启动进程
            button4.Enabled = false;
        }
        public enum MODKEY
        {
            MOD_ALT = 0x0001,
            MOD_CONTROL = 0x0002,
            MOD_SHIFT = 0x0004,
            MOD_WIN = 0x0008,
        }
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(
            IntPtr wnd, int id, MODKEY mode, Keys vk);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr wnd, int id);
        /// <summary>
        /// 重写WndProc，按下快捷键切换桌面
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0x0312)
            {
                switch (m.WParam.ToInt32())
                {
                    case 0x10000:
                        if (vd == null) break;
                        vd.Show();//切换到虚拟桌面
                        break;
                    case 0x10001:
                        VirtualDesktop.Default.Show();//切换到默认桌面
                        break;
                    case 0x10002:
                        if (Visible) Visible = false;
                        else Visible = true;
                        break;
                }
            }
        }
        /// <summary>
        /// 反注册热键，关闭虚拟桌面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(Handle, 0x10000);
            UnregisterHotKey(Handle, 0x10001);
            UnregisterHotKey(Handle, 0x10002);
            if (vd != null) { vd.Close(); }
        }
        /// <summary>
        /// 收起/展开说明
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (button5.Text == "<")
            {
                Size = new Size(285, 322);
                button5.Text = ">";
            }
            else
            {
                Size = new Size(566, 322);
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
            if (File.Exists(Spider == "" ? Spider2 : Spider + "REDAgentFull.exe"))
            {
                label2.Text = "已经安装过补丁,\n无法再次安装.\n可删除补丁后重试.\n";
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
                KillProcessExists();//结束进程
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
