#include "chelperneo.h"
#include <QMessageBox>
#include <QSettings>
#include <QFile>
#include <QFileInfo>
#include <QProcess>
#include <QTextCodec>
#include <QSysInfo>

CHelperNeo::CHelperNeo(QWidget *parent)
	: QMainWindow(parent)
{
    bool xp=false;
    if(QSysInfo::windowsVersion()==0x0030) xp=true;

	ui.setupUi(this);
	ui.pushButton_rs_window->setVisible(false);
	ui.pushButton_rs_full->setVisible(false);
	ui.pushButton_rs_back->setVisible(false);
	ui.pushButton_rs_dectl->setVisible(false);

	ui.pushButton_jp_dectl->setVisible(false);
	ui.pushButton_jp_back->setVisible(false);
    ui.pushButton_jp_patch->setVisible(false);

    resize(this->width(), 297);
    //setMinimumSize(340, 297);
	this->setWindowTitle("机房小工具2");

	//按钮
	connect(ui.pushButton_auto, SIGNAL(clicked()), this, SLOT(auto_select()));
	connect(ui.pushButton_rs, SIGNAL(clicked()), this, SLOT(menu_rs()));
	connect(ui.pushButton_jp, SIGNAL(clicked()), this, SLOT(menu_jp()));
	connect(ui.pushButton_rectl, SIGNAL(clicked()), this, SLOT(rectl()));
	connect(ui.pushButton_rs_window, SIGNAL(clicked()), this, SLOT(rs_window()));
	connect(ui.pushButton_rs_full, SIGNAL(clicked()), this, SLOT(rs_full()));
	connect(ui.pushButton_rs_dectl, SIGNAL(clicked()), this, SLOT(rs_dectl()));
	connect(ui.pushButton_rs_back, SIGNAL(clicked()), this, SLOT(menu_rs_back()));
	connect(ui.pushButton_jp_dectl, SIGNAL(clicked()), this, SLOT(jp_dectl()));
	connect(ui.pushButton_jp_back, SIGNAL(clicked()), this, SLOT(menu_jp_back()));
    connect(ui.pushButton_patch,SIGNAL(clicked()),this,SLOT(patch_sethc()));
    connect(ui.pushButton_jp_patch,SIGNAL(clicked()),this,SLOT(jp_patch()));

	//菜单
	connect(ui.action_rs_dectl, SIGNAL(triggered()), this, SLOT(rs_dectl()));
    connect(ui.action_rs_rectl, SIGNAL(triggered()), this, SLOT(rs_rectl()));
	connect(ui.action_rs_window, SIGNAL(triggered()), this, SLOT(rs_window()));
	connect(ui.action_rs_full, SIGNAL(triggered()), this, SLOT(rs_full()));
	connect(ui.action_jp_dectl, SIGNAL(triggered()), this, SLOT(jp_dectl()));
	connect(ui.action_jp_rectl, SIGNAL(triggered()), this, SLOT(jp_rectl()));
	connect(ui.action_rop, SIGNAL(triggered()), this, SLOT(rightButton_optimise()));
	connect(ui.action_quit, SIGNAL(triggered()), this, SLOT(close()));
	connect(ui.action_help, SIGNAL(triggered()), this, SLOT(helpDialog()));
	connect(ui.action_about, SIGNAL(triggered()), this, SLOT(aboutDialog()));
    connect(ui.action_jp_patch,SIGNAL(triggered()),this,SLOT(jp_patch()));

	//红蜘蛛
	auto reg = new QSettings(Spider_reg[0], QSettings::NativeFormat);
	auto value = reg->value("AgentCommand", QVariant()).toString();
	if (value == "")reg = new QSettings(Spider_reg[1], QSettings::NativeFormat);
	value = reg->value("AgentCommand", QVariant()).toString();
	if (value != "")Spider = subString(value, 0, value.indexOf("REDAgent"));
	if (!dirExists(Spider))//注册表读取的路径不正确
	{
		if (!dirExists(Spider_x64))//x64路径不正确
			if (!dirExists(Spider_x86))//x86路径不正确
				Spider = "";//不存在红蜘蛛
			else Spider = Spider_x86;
		else Spider = Spider_x64;
	}
	//江波ECR
	reg = new QSettings(Jumple_reg, QSettings::NativeFormat);
	value = reg->value("Path", QVariant()).toString();
	if (value != "")Jumple = value + "\\";
	if (!dirExists(Jumple))//注册表读取的路径不正确
	{
		if (!dirExists(Jumple_x64))//x64路径不正确
			if (!dirExists(Jumple_X86))//x86路径不正确
				Jumple = "";//不存在江波ECR
			else Jumple = Jumple_X86;
		else Jumple = Jumple_x64;
	}
	//UI 调整
	if (Spider == "")
	{
		ui.pushButton_rs->setEnabled(false);
		ui.action_rs_dectl->setEnabled(false);
		ui.action_rs_full->setEnabled(false);
		ui.action_rs_rectl->setEnabled(false);
		ui.action_rs_window->setEnabled(false);
		ui.label_stat->setText("红蜘蛛：未安装 | ");
	}
	else
	{
		if (fileExists(Spider + Spider_Full_Name))
			ui.label_stat->setText("红蜘蛛：窗口化 | ");
		else if (fileExists(Spider + Spider_Changed_Name))
			ui.label_stat->setText("红蜘蛛：已解除 | ");
		else if (fileExists(Spider + Spider_Process_Name))
			ui.label_stat->setText("红蜘蛛：未解除 | ");
		else ui.label_stat->setText("红蜘蛛：损坏 | ");
	}

	if (Jumple == "")
	{
        ui.pushButton_jp->setEnabled(false);
        ui.action_jp_dectl->setEnabled(false);
        ui.action_jp_rectl->setEnabled(false);
        ui.action_jp_patch->setEnabled(false);
        ui.label_stat->setText(ui.label_stat->text() + "江波ECR：未安装");
	}
	else
	{
		if (fileExists(Jumple + Jumple_Process_Name))
			ui.label_stat->setText(ui.label_stat->text() + "江波ECR：未解除");
		else if (fileExists(Jumple + Jumple_Changed_Name))
			ui.label_stat->setText(ui.label_stat->text() + "江波ECR：已解除");
		else
			ui.label_stat->setText(ui.label_stat->text() + "江波ECR：损坏");
	}
	if (Spider == ""&&Jumple == "")
	{
		ui.pushButton_auto->setEnabled(false);
		ui.pushButton_rectl->setEnabled(false);
    }
    if(Jumple != "" && xp)
    {
        QMessageBox::information(this,"警告","检测到XP系统和江波ECR，小工具不支持此环境！");
    }    

    auto file = new QFile("C:\\Windows\\System32\\sethc.exe.bak");
    if(file->exists())ui.label_patch->setText("补丁状态：已安装");
    else ui.label_patch->setText("补丁状态：未安装");
}

CHelperNeo::~CHelperNeo()
{

}

void CHelperNeo::menu_rs()
{
	this->ui.pushButton_rs->setVisible(false);
	this->ui.pushButton_rs_window->setVisible(true);
	this->ui.pushButton_rs_full->setVisible(true);
	this->ui.pushButton_rs_back->setVisible(true);
	this->ui.pushButton_rs_dectl->setVisible(true);
}

void CHelperNeo::menu_jp()
{
	this->ui.pushButton_jp->setVisible(false);
	this->ui.pushButton_jp_dectl->setVisible(true);
	this->ui.pushButton_jp_back->setVisible(true);
    this->ui.pushButton_jp_patch->setVisible(true);
}

void CHelperNeo::menu_rs_back()
{
	this->ui.pushButton_rs_window->setVisible(false);
	this->ui.pushButton_rs_full->setVisible(false);
	this->ui.pushButton_rs_back->setVisible(false);
	this->ui.pushButton_rs_dectl->setVisible(false);
	this->ui.pushButton_rs->setVisible(true);
}

void CHelperNeo::menu_jp_back()
{
	this->ui.pushButton_jp_dectl->setVisible(false);
	this->ui.pushButton_jp_back->setVisible(false);
    this->ui.pushButton_jp_patch->setVisible(false);
	this->ui.pushButton_jp->setVisible(true);
}

void CHelperNeo::rightButton_optimise()
{
	auto reg = new
		QSettings("HKEY_CLASSES_ROOT\\Directory\\Background\\shellex\\ContextMenuHandlers",
			QSettings::NativeFormat);
	auto list = reg->allKeys();
	bool isExist = false;
	for (int i = 0; i < list.size(); i++)
		if (list[i] == "NvCplDesktopContext/.") { isExist = true; break; }
	if (!isExist)
	{
		QMessageBox::information(this, "提示", "当前无需加速~");
		return;
	}
	reg->remove("NvCplDesktopContext");
	list = reg->allKeys();
	isExist = false;
	for (int i = 0; i < list.size(); i++)
		if (list[i] == "NvCplDesktopContext/.") { isExist = true; break; }
	if (!isExist)
		QMessageBox::information(this, "提示", "加速成功，快去试试桌面右键还卡不卡~");
	else
		QMessageBox::critical(this, "错误", "加速失败，请检查是否权限不足...");
}

QString CHelperNeo::subString(QString s, int begin, int length)
{
	QString ret = "";
	for (int i = begin; i < begin + length&&i < s.length(); i++)
		ret += s[i];
	return ret;
}

bool CHelperNeo::fileExists(QString path)
{
	auto file = new QFile(path);
	return file->exists();
}

bool CHelperNeo::dirExists(QString path)
{
	auto  fileInfo = new QFileInfo(path);
	return fileInfo->exists();
}

bool CHelperNeo::processExists(QString processName)
{
	QProcess process;
	process.start("tasklist -fi \"imagename eq " + processName + "\"");
	QString result;
	QTextCodec* gbkCodec = QTextCodec::codecForName("GBK");
	if (process.waitForFinished())
		result = gbkCodec->toUnicode(process.readAll());
	if (result.indexOf("没有") != -1)return false;
	else return true;
}

bool CHelperNeo::killProcess(QString processName)
{
	QProcess process;
	process.start("taskkill -f -fi \"imagename eq " + processName + "\"");
	QString result;
	QTextCodec* gbkCodec = QTextCodec::codecForName("GBK");
	if (process.waitForFinished())
		result = gbkCodec->toUnicode(process.readAll());
	if (result.indexOf("已终止") != -1)return true;
	else return false;
}

int CHelperNeo::rs_dectl()
{
	int code = 0;
	{
		if (Spider == "") { code = 1; goto error; }//未安装红蜘蛛
		auto file = new QFile(Spider + Spider_Process_Name);
		if (!file->exists()) //找不到文件
		{
			file = new QFile(Spider + Spider_Changed_Name);
			if (file->exists())code = -1;
			else code = 2;
			goto error;
		}
		if (!file->rename(Spider + Spider_Changed_Name)) { code = 3; goto error; }//重命名失败
		if (!processExists(Spider_Process_Name)) { code = 4; goto error; }//进程不存在
		if (!killProcess(Spider_Process_Name)) { code = 5; goto error; }//结束进程失败
		auto s = ui.label_stat->text();
		ui.label_stat->setText("红蜘蛛：已解除 " + subString(s, s.indexOf("|"), s.length()));
		QMessageBox::information(this, "提示", "已经成功解除控制啦~");
		return code;
	}
error:
	switch (code)
	{
	case 1:
		QMessageBox::critical(this, "错误", "喂喂喂，你压根就没安装红蜘蛛好吧~");
		break;
	case 2:
		QMessageBox::critical(this, "错误", "额，找不到文件？！是不是损坏了啊？");
		break;
	case 3:
		QMessageBox::critical(this, "错误", "诶？权限不足？！用管理员身份运行试试！");
		break;
	case 4:
		QMessageBox::critical(this, "错误", "额，找不到进程？！红蜘蛛可能被某种神秘力量关闭了~");
		break;
	case 5:
		QMessageBox::critical(this, "错误", "诶？无法关闭红蜘蛛？！用管理员身份运行试试！");
		break;
	case -1:
		QMessageBox::critical(this, "错误", "醒醒，好像已经解除控制了啊~");
		break;
	}
	if (code >= 4)//已改名，未结束进程，则需要改回
	{
		auto file = new QFile(Spider + Spider_Changed_Name);
		file->rename(Spider + Spider_Process_Name);
	}
	return code;
}

int CHelperNeo::jp_dectl()
{
	int code = 0;
	{
		if (Jumple == "") { code = 1; goto error; }//未安装江波ECR
		auto file = new QFile(Jumple + Jumple_Process_Name);
		if (!file->exists()) //找不到文件
		{
			file = new QFile(Jumple + Jumple_Changed_Name);
			if (file->exists())code = -1;
			else code = 2;
			goto error;
		}
		if (!file->rename(Jumple + Jumple_Changed_Name)) { code = 3; goto error; }//重命名失败
		if (!processExists(Jumple_Process_Name)) { code = 4; goto error; }//进程不存在
		if (!killProcess(Jumple_Process_Name)) { code = 5; goto error; }//结束进程失败
		auto s = ui.label_stat->text();
		ui.label_stat->setText(subString(s, 0, s.indexOf("|")) + "| 江波ECR：已解除");
		QMessageBox::information(this, "提示", "已经成功解除控制啦~");
		return code;
	}
error:
	switch (code)
	{
	case 1:
		QMessageBox::critical(this, "错误", "喂喂喂，你压根就没安装江波ECR好吧~");
		break;
	case 2:
		QMessageBox::critical(this, "错误", "额，找不到文件？！是不是损坏了啊？");
		break;
	case 3:
		QMessageBox::critical(this, "错误", "诶？权限不足？！用管理员身份运行试试！");
		break;
	case 4:
		QMessageBox::critical(this, "错误", "额，找不到进程？！江波ECR可能被某种神秘力量关闭了~");
		break;
	case 5:
		QMessageBox::critical(this, "错误", "诶？无法关闭江波ECR？！用管理员身份运行试试！");
		break;
	case -1:
		QMessageBox::critical(this, "错误", "醒醒，好像已经解除控制了啊~");
		break;
	}
	if (code >= 4)//已改名，未结束进程，则需要改回
	{
		auto file = new QFile(Jumple + Jumple_Changed_Name);
		file->rename(Jumple + Jumple_Process_Name);
	}
	return code;
}

int CHelperNeo::rs_rectl()
{
	int code = 0;
	{
		if (Spider == "") { code = 1; goto error; }
		auto file = new QFile(Spider + Spider_Changed_Name);
		if (!file->exists()) 
		{ 
			file = new QFile(Spider + Spider_Process_Name);
			if (file->exists())code = -1;
			else code = 2; 
			goto error; 
		}
		if (!file->rename(Spider + Spider_Process_Name)) { code = 3; goto error; }
        if (!processExists(Spider_Process_Name) &&
                !QProcess::startDetached(Spider + Spider_Process_Name, QStringList() << (Spider + Spider_Process_Name)))
            { code = 4; goto error; } // 进程不存在时才去启动
		auto s = ui.label_stat->text();
		ui.label_stat->setText("红蜘蛛：未解除 " + subString(s, s.indexOf("|"), s.length()));
		QMessageBox::information(this, "提示", "已经成功恢复控制啦~");
		return code;
	}
error:
	switch (code)
	{
	case 1:
		QMessageBox::critical(this, "错误", "喂喂喂，你压根就没安装红蜘蛛好吧~");
		break;
	case 2:
		QMessageBox::critical(this, "错误", "额，找不到文件？！是不是损坏了啊？");
		break;
	case 3:
		QMessageBox::critical(this, "错误", "诶？权限不足？！用管理员身份运行试试！");
		break;
	case 4:
		QMessageBox::critical(this, "错误", "额，红蜘蛛启动失败了...");
		break;
	case -1:
		QMessageBox::critical(this, "错误", "喂喂喂，你还没解除控制呢~");
		break;
	}
	return code;
}

int CHelperNeo::jp_rectl()
{
	int code = 0;
	{
		if (Jumple == "") { code = 1; goto error; }
		auto file = new QFile(Jumple + Jumple_Changed_Name);
		if (!file->exists()) 
		{ 
			file = new QFile(Jumple + Jumple_Process_Name);
			if (file->exists())code = -1;
			else code = 2; 
			goto error; 
		}
        if (!processExists(Spider_Process_Name) &&
                !file->rename(Jumple + Jumple_Process_Name))
            { code = 3; goto error; }
		if (!QProcess::startDetached(Jumple + Jumple_Process_Name, QStringList() << (Spider + Spider_Process_Name))) { code = 4; goto error; }
		auto s = ui.label_stat->text();
		ui.label_stat->setText(subString(s, 0, s.indexOf("|")) + "| 江波ECR：未解除");
		QMessageBox::information(this, "提示", "已经成功恢复控制啦~");
		return code;
	}
error:
	switch (code)
	{
	case 1:
		QMessageBox::critical(this, "错误", "喂喂喂，你压根就没安装江波ECR好吧~");
		break;
	case 2:
		QMessageBox::critical(this, "错误", "额，找不到文件？！是不是损坏了啊？");
		break;
	case 3:
		QMessageBox::critical(this, "错误", "诶？权限不足？！用管理员身份运行试试！");
		break;
	case 4:
		QMessageBox::critical(this, "错误", "额，江波ECR启动失败了...");
		break;
	case -1:
		QMessageBox::critical(this, "错误", "喂喂喂，你还没解除控制呢~");
		break;
	}
	return code;
}

void CHelperNeo::helpDialog()
{
	QMessageBox::about(this, "帮助", help_String);
}

void CHelperNeo::aboutDialog()
{
	QMessageBox::about(this, "关于", about_String);
}

void CHelperNeo::rectl()
{
	if (Spider == ""&&Jumple == "")
	{
		QMessageBox::critical(this, "错误", "喂喂喂，啥都没装你想让我开启啥？");
		return;
	}
	if (Spider != "")rs_rectl();
	if (Jumple != "")jp_rectl();
}

void CHelperNeo::auto_select()
{
	if (Spider == ""&&Jumple == "")
	{
		QMessageBox::critical(this, "错误", "喂喂喂，啥都没装你想让我干啥？");
		return;
	}
	if (Spider != "")
	{
		auto file = new QFile(Spider + Spider_Full_Name);
		if (file->exists())rs_full();//已窗口化
		else
		{
			file = new QFile(Spider + Spider_Process_Name);
			if (file->exists())//未解控
			{
				if (rs_window()) 
				{
					QMessageBox::information(this, "提示", "QAQ，窗口化失败了...尝试解除控制。");
					rs_dectl(); 
				}
			}
			file = new QFile(Spider + Spider_Changed_Name);
			if (file->exists())rs_rectl();
		}
	}
	if (Jumple != "")
	{
		auto file = new QFile(Jumple + Jumple_Process_Name);
		if (file->exists())
        {
            if(jp_patch())
            {
                QMessageBox::information(this, "提示", "QAQ，安装补丁失败了...尝试解除控制。");
                jp_dectl();
            }
        }
        else
        {
            file = new QFile(Jumple + Jumple_Changed_Name);
            if (file->exists())jp_rectl();
        }
	}
}

int CHelperNeo::rs_window()
{
	int code = 0;
    auto file = new QFile(Spider + Spider_Full_Name);
	{
		if (Spider == "") { code = 1; goto error; }
		if (file->exists()) { code = 2; goto error; }//已经窗口化
		file = new QFile(Spider + Spider_Changed_Name);
		if (file->exists()) { code = 3; goto error; }//已经解除控制
	back3:
		file = new QFile(Spider + Spider_Process_Name);
		if (!file->exists()) { code = 4; goto error; }//文件不存在
		if (!file->rename(Spider + Spider_Full_Name)) { code = 5; goto error; }//改名失败
		if (!processExists(Spider_Process_Name)) { code = 6; goto error; }//进程不存在
		if (!killProcess(Spider_Process_Name)) { code = 7; goto error; }//结束进程失败
	back6:
		if (rs_copy()) { code = 9; goto error; }
        if (!processExists(Spider_Process_Name))
            if (!QProcess::startDetached(Spider + Spider_Process_Name, QStringList() << (Spider + Spider_Process_Name))) { code = 8; goto error; }
		auto s = ui.label_stat->text();
		ui.label_stat->setText("红蜘蛛：窗口化 " + subString(s, s.indexOf("|"), s.length()));
		QMessageBox::information(this, "提示", "已经成功窗口化啦~");
		return code;
	}
error:
	switch (code)
    {
	case 1:
		QMessageBox::critical(this, "错误", "喂喂喂，你压根就没安装红蜘蛛好吧~");
		break;
	case 4:
		QMessageBox::critical(this, "错误", "额，找不到文件？！是不是损坏了啊？");
		break;
	case 3:
		if (QMessageBox::information(this, "提示", "已经解除控制，是否恢复控制并窗口化？", QMessageBox::Yes, QMessageBox::No) == QMessageBox::No)
			break;//NO
		else
		{
            auto file = new QFile(Spider + Spider_Changed_Name);
			if (!file->rename(Spider + Spider_Process_Name)) { QMessageBox::critical(this, "错误", "诶？权限不足？！用管理员身份运行试试！"); break; }
			else
			{
				code = 0;
				goto back3;
			}
		}
	case 2:
        QMessageBox::critical(this, "错误", "醒醒，好像已经窗口化了~");
		break;
	case 5:
		QMessageBox::critical(this, "错误", "诶？权限不足？！用管理员身份运行试试！");
		break;
	case 6:
		if(QMessageBox::critical(this, "提示", "额，找不到进程？！红蜘蛛可能被某种神秘力量关闭了~\n是否尝试开启红蜘蛛并窗口化？", QMessageBox::Yes, QMessageBox::No)==QMessageBox::No)
			break;
		else
		{
			code = 0;
			goto back6;
			return 0;
		}
	case 7:
		QMessageBox::critical(this, "错误", "诶？无法关闭红蜘蛛？！用管理员身份运行试试！");
		break;
	case 8:
		QMessageBox::critical(this, "错误", "额，红蜘蛛启动失败了...");
		break;
	}
	if (code >= 6)
	{
		auto file = new QFile(Spider + Spider_Full_Name);
		file->rename(Spider + Spider_Process_Name);
	}
	return code;
}

int CHelperNeo::rs_copy()
{
	int code = 0;
	{
		auto file = new QFile(Spider_Process_Name);
		if (!file->exists()) { code = 1; goto error; }
		if (!file->copy(Spider + Spider_Process_Name)) { code = 2; goto error; }
		return 0;
	}
error:
	switch (code)
	{
	case 1:
		QMessageBox::critical(this, "错误", "QAQ，找不到补丁文件了...");
		break;
	case 2:
		QMessageBox::critical(this, "错误", "诶？权限不足？！用管理员身份运行试试！");
		break;
	}
	return code;
}

int CHelperNeo::rs_full()
{
	int code = 0;
	{
		if (Spider == "") { code = 1; goto error; }//未安装红蜘蛛
		auto file = new QFile(Spider + Spider_Full_Name);
		if (!file->exists()) { code = 6; goto error; }//未窗口化
		file = new QFile(Spider + Spider_Process_Name);
		if (!file->exists()) { code = 2; goto error; }//文件不存在
		if (!file->rename(Spider + Spider_Changed_Name)) { code = 3; goto error; }//改名失败
		if (processExists(Spider_Process_Name)&&//进程不存在时不结束进程，不报错
			!killProcess(Spider_Process_Name)) { code = 4; goto error; }//结束进程失败
		file = new QFile(Spider + Spider_Full_Name);
		if (!file->exists()) { code = 2; goto error; }//文件不存在
		if (!file->rename(Spider + Spider_Process_Name)) { code = 3; goto error; }
		if (!QProcess::startDetached(Spider + Spider_Process_Name, QStringList() << (Spider + Spider_Process_Name))) { code = 5; goto error; }
		file = new QFile(Spider + Spider_Changed_Name);
		if (!file->remove()) { code = 3; goto error; }//删除文件失败
		auto s = ui.label_stat->text();
		ui.label_stat->setText("红蜘蛛：未解除 " + subString(s, s.indexOf("|"), s.length()));
		QMessageBox::information(this, "提示", "成功恢复全屏控制啦~");
		return code;
	}
error:
	switch (code)
	{
	case 1:
		QMessageBox::critical(this, "错误", "喂喂喂，你压根就没安装红蜘蛛好吧~");
		break;
	case 2:
		QMessageBox::critical(this, "错误", "额，找不到文件？！是不是损坏了啊？");
		break;
	case 3:
		QMessageBox::critical(this, "错误", "诶？权限不足？！用管理员身份运行试试！");
		break;
	case 4:
		QMessageBox::critical(this, "错误", "诶？无法关闭红蜘蛛？！用管理员身份运行试试！");
		break;
	case 5:
		QMessageBox::critical(this, "错误", "额，红蜘蛛启动失败了...");
		break;
	case 6:
		QMessageBox::critical(this, "错误", "别骗我，你一定还没有窗口化~");
		break;
	}
	if (code <= 3 || code == 6)return code;//这里判断没有改名的情况，直接返回，避免解除控制后再点击恢复全屏导致恢复控制错误
	auto file = new QFile(Spider + Spider_Changed_Name);
	if (file->exists())//已经改过名(第一次)，！也有可能是解除了控制！
	{
		file = new QFile(Spider + Spider_Process_Name);
		if (file->exists())//原文件也存在，属于 删除失败或结束、启动进程失败
		{
			file->rename(Spider + Spider_Full_Name);
			file = new QFile(Spider + Spider_Changed_Name);
			file->rename(Spider + Spider_Process_Name);
			if (!processExists(Spider_Process_Name))
				QProcess::startDetached(Spider + Spider_Process_Name, QStringList() << (Spider + Spider_Process_Name));
		}
		else//原文件不存在，属于 第二次改名失败或文件不存在，一般不会出现这种情况
		{
			file = new QFile(Spider + Spider_Changed_Name);
			file->rename(Spider + Spider_Process_Name);
			if (!processExists(Spider_Process_Name))
				QProcess::startDetached(Spider + Spider_Process_Name, QStringList() << (Spider + Spider_Process_Name));
		}
	}
	return code;
}

void CHelperNeo::patch_sethc()
{
    QString error_str="";
    if(ui.label_patch->text().indexOf("已")==-1)
    {
        auto p= new QProcess();
        p->start("Takeown",QStringList()<<"/f"<<"C:\\Windows\\System32\\sethc.exe");
        p->waitForStarted();
        if(p->waitForFinished())
        {
            if(QFile("debug.o").exists())
            {
                QMessageBox::information(this,"测试输出",QString::fromLocal8Bit(p->readAllStandardError()));
                QMessageBox::information(this,"测试输出",QString::fromLocal8Bit(p->readAllStandardOutput()));
            }
            auto file=new QFile("C:\\Windows\\SysNative\\sethc.exe");
            p = new QProcess();
            p->start("setPermission.exe");
            p->waitForStarted();
            if(p->waitForFinished() && file->rename("C:\\Windows\\SysNative\\sethc.exe.bak"))
            {
                file=new QFile("chelper_sethc.exe");
                if(file->exists())
                {
                    if(file->copy("C:\\Windows\\SysNative\\sethc.exe"))
                    {
                        ui.label_patch->setText("补丁状态：已安装");
                        ui.pushButton_patch->setText("删除增强补丁");
                        return;
                    }
                    else { error_str="安装补丁失败！";  goto error; }
                }
                else { error_str="补丁文件不存在！";  goto error; }
            }
            else { error_str="备份原文件出错！尝试以管理员身份运行！";  goto error; }
        }
        else { error_str="获取权限失败！尝试以管理员身份运行！";  goto error; }
    }
    else
    {
        auto file=new QFile("C:\\Windows\\SysNative\\sethc.exe");
        if(file->exists())
        {
           if(file->remove())
           {
               file=new QFile("C:\\Windows\\SysNative\\sethc.exe.bak");
               if(file->exists())
               {
                   if(file->rename("C:\\Windows\\SysNative\\sethc.exe"))
                   {
                       ui.pushButton_patch->setText("安装增强补丁");
                       ui.label_patch->setText("补丁状态：未安装");
                       return;
                   }
                   else { error_str="恢复备份文件失败！";  goto error2; }
               }
               else { error_str="备份文件不存在！";  goto error2; }
           }
           else { error_str="删除补丁失败！尝试以管理员身份运行！";  goto error2; }
        }
        else { error_str="已安装补丁不存在！";  goto error2; }
    }
    error:
        QMessageBox::critical(this,"错误","安装补丁失败，"+error_str);
        return;
    error2:
        QMessageBox::critical(this,"错误","删除补丁失败，"+error_str);
        return;
}

int CHelperNeo::jp_patch()
{
    int code = 0;
    auto file = new QFile(Jumple + Jumple_Origen_Name);
    {
        if (Jumple == "") { code = 1; goto error; }
        if (file->exists()) { code = 2; goto error; }//已经补丁
        file = new QFile(Jumple + Jumple_Changed_Name);
        if (file->exists()) { code = 3; goto error; }//已经解除控制
    back3:
        file = new QFile(Jumple + Jumple_Process_Name);
        if (!file->exists()) { code = 4; goto error; }//文件不存在
        if (!file->rename(Jumple + Jumple_Origen_Name)) { code = 5; goto error; }//改名失败
        if (!processExists(Jumple_Process_Name)) { code = 6; goto error; }//进程不存在
        if (!killProcess(Jumple_Process_Name)) { code = 7; goto error; }//结束进程失败
    back6:
        if (jp_copy()) { code = 9; goto error; }
        if (!processExists(Jumple_Process_Name))
            if (!QProcess::startDetached(Jumple + Jumple_Process_Name, QStringList() << (Jumple + Jumple_Process_Name))) { code = 8; goto error; }
        auto s = ui.label_stat->text();
        ui.label_stat->setText(subString(s, 0, s.indexOf("|")) + "| 江波ECR：补丁");
        QMessageBox::information(this, "提示", "补丁安装成功啦~");
        return code;
    }
error:
    switch (code)
    {
    case 1:
        QMessageBox::critical(this, "错误", "喂喂喂，你压根就没安装江波ECR好吧~");
        break;
    case 4:
        QMessageBox::critical(this, "错误", "额，找不到文件？！是不是损坏了啊？");
        break;
    case 3:
        if (QMessageBox::information(this, "提示", "已经解除控制，是否恢复控制并安装补丁？", QMessageBox::Yes, QMessageBox::No) == QMessageBox::No)
            break;//NO
        else
        {
            auto file = new QFile(Jumple + Jumple_Changed_Name);
            if (!file->rename(Jumple + Jumple_Process_Name)) { QMessageBox::critical(this, "错误", "诶？权限不足？！用管理员身份运行试试！"); break; }
            else
            {
                code = 0;
                goto back3;
            }
        }
    case 2:
        QMessageBox::critical(this, "错误", "醒醒，好像已经安装了补丁誒~");
        break;
    case 5:
        QMessageBox::critical(this, "错误", "诶？权限不足？！用管理员身份运行试试！");
        break;
    case 6:
        if(QMessageBox::critical(this, "提示", "额，找不到进程？！江波ECR可能被某种神秘力量关闭了~\n是否尝试开启江波ECR并安装补丁？", QMessageBox::Yes, QMessageBox::No)==QMessageBox::No)
            break;
        else
        {
            code = 0;
            goto back6;
            return 0;
        }
    case 7:
        QMessageBox::critical(this, "错误", "诶？无法关闭江波ECR？！用管理员身份运行试试！");
        break;
    case 8:
        QMessageBox::critical(this, "错误", "额，江波ECR启动失败了...");
        break;
    }
    if (code >= 6)
    {
        auto file = new QFile(Jumple + Jumple_Origen_Name);
        file->rename(Jumple + Jumple_Process_Name);
    }
    return code;
}

int CHelperNeo::jp_copy()
{
    int code = 0;
    {
        auto file = new QFile(Jumple_Process_Name);
        if (!file->exists()) { code = 1; goto error; }
        if (!file->copy(Jumple + Jumple_Process_Name)) { code = 2; goto error; }
        return 0;
    }
error:
    switch (code)
    {
    case 1:
        QMessageBox::critical(this, "错误", "QAQ，找不到补丁文件了...");
        break;
    case 2:
        QMessageBox::critical(this, "错误", "诶？权限不足？！用管理员身份运行试试！");
        break;
    }
    return code;
}
