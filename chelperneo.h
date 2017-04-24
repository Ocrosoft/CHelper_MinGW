#ifndef CHELPERNEO_H
#define CHELPERNEO_H
//#pragma execution_character_set("utf-8")

#include <QtWidgets/QMainWindow>
#include "ui_chelperneo.h"

class CHelperNeo : public QMainWindow
{
	Q_OBJECT

public:
	CHelperNeo(QWidget *parent = 0);
	~CHelperNeo();
private slots:
	int rs_rectl();//红蜘蛛启用控制
	int jp_rectl();//江波ECR启用控制
	void menu_rs();//点击打开红蜘蛛菜单
	void menu_jp();//点击打开江波ECR菜单
	void menu_rs_back();//关闭红蜘蛛菜单
	void menu_jp_back();//关闭江波ECR菜单
	void rightButton_optimise();//右键加速
    QString subString(QString s, int begin, int length);//取子串
	bool fileExists(QString path);//文件是否存在
	bool dirExists(QString path);//文件夹是否存在
	bool processExists(QString processName);//进程是否存在
	bool killProcess(QString processName);//结束进程
    void helpDialog();//帮助
    void aboutDialog();//关于

    void auto_select();//自动选择操作
	int rs_dectl();//红蜘蛛解除控制
	int jp_dectl();//江波ECR解除控制
    int rs_window();//红蜘蛛窗口化
    int rs_copy();//复制红蜘蛛补丁文件
    int rs_full();//红蜘蛛恢复全屏
    int jp_patch();//江波ECR安装补丁
    int jp_copy();//复制江波ECR补丁文件
	void rectl();//启用控制

    void patch_sethc();//补丁
private:
	Ui::CHelperNeoClass ui;
	QString Spider_Process_Name = "REDAgent.exe";//红蜘蛛文件/进程名
	QString Spider_Changed_Name = "REDAgentt.exe";//红蜘蛛解除控制更改成的文件名
	QString Spider_Full_Name = "REDAgentFull.exe";//红蜘蛛窗口化原文件更改成的文件名
	QString Jumple_Process_Name = "Student.exe";//江波ECR文件/进程名
	QString Jumple_Changed_Name = "Studentt.exe";//江波ECR解除控制更改成的文件名
    QString Jumple_Origen_Name = "StudentOrigen.exe";//江波ECR安装补丁源文件更改成的文件名
	QString Spider = "";//最终得出的红蜘蛛路径
	QString Spider_x64 = "C:\\Program Files (x86)\\3000soft\\Red Spider\\";//64位默认路径
	QString Spider_x86 = "C:\\Program Files\\3000soft\\Red Spider\\";//32位默认路径
	QString Jumple = "";//最终得出的江波ECR路径
	QString Jumple_x64 = "C:\\Program Files (x86)\\Jumple\\ECR\\";//64位默认路径
	QString Jumple_X86 = "C:\\Program Files\\Jumple\\ECR\\";//32位默认路径
	QString Spider_reg[2] = {//红蜘蛛注册表
		"HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\3000soft\\Red Spider",//64位
		"HKEY_LOCAL_MACHINE\\SOFTWARE\\3000soft\\Red Spider" };//32位
	QString Jumple_reg = //江波ECR注册表
		"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\Teacher.exe";//32位，64位
    QString help_String = "<ol class=' list-paddingleft-2'>	<li>		<p>			关于自动选择[解除/启用]控制操作		</p>		<p>			1.1.安装了红蜘蛛的情况下，尝试窗口化，如果失败则选择解除控制。		</p>		<p>			1.2.安装了江波ECR的情况下，尝试安装补丁，如果失败则解除控制。		</p>		<p>			1.3.若已经窗口化/解除控制，则回复全屏/开启控制。		</p>	</li>	<li>		<p>			关于红蜘蛛的窗口化功能		</p>		<p>			2.1.窗口化使用红蜘蛛自身的窗口化功能，推荐使用。		</p>		<p>			2.2.窗口化之后，可以操作鼠标键盘，其它功能见红蜘蛛说明书。		</p>		<p>			2.3.窗口化之后，<span style='color:#FF0000;'>不会</span>影响教师端监视等功能。		</p>	</li>	<li>		<p>			关于江波ECR的补丁功能		</p>		<p>			3.1.删除了被控制时的窗口置顶和鼠标锁定功能，可自由进行操作。		</p>		<p>			3.2.补丁安装后无任何显示效果，故无需删除（无删除补丁功能）。		</p>	</li>	<li>		<p>			关于解除控制功能		</p>		<p>			4.1.解除控制原理为关闭控制软件并阻止其重新启动。		</p>		<p>			4.2.解除控制后教师端界面上显示为<span style='color:#FF0000;'>未开机</span>状态。		</p>		<p>			4.3.教师端所有针对此计算机的所有功能<span style='color:#FF0000;'>失效</span>。		</p>	</li>	<li>		<p>			关于右键加速功能		</p>		<p>			5.1.桌面右键卡顿问题在于安装了Nvidia的显示器驱动。		</p>		<p>			5.2.加速原理为删除其右键菜单项。		</p>	</li>	<li>		<p>			关于控制中解控		</p>		<p>			6.1.该功能暂时无法正常使用。		</p>	</li></ol>";
    QString about_String = "<p style='text-align:center;'>	<strong>Computer Helper<br /></strong>Version: 1.1<br />免责声明<br />请认真阅读帮助<br /><span>造成损失概不负责<br /></span><span>&copy;2016-2017 oc</span><span>rosoft<br /></span><a href='https://www.ocrosoft.com/'><span>https://www.ocrosoft.com/</span></a></p>";
};
#endif // CHELPERNEO_H
