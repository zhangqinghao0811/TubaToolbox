using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TubaToolbox
{
    public partial class MainWindow : Window
    {
        private string toolsBasePath;
        private Dictionary<string, UserControl> categoryPages = new();
        private Button? lastClickedButton;

        public MainWindow()
        {
            toolsBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tools");
            InitializeComponent();
            LoadWindowIcon();
            LoadCategoryPages();
            ShowCategory("硬件信息");
            CheckToolsPath();
        }

        private void LoadWindowIcon()
        {
            try
            {
                string iconPath = Path.Combine(AppContext.BaseDirectory, "app.ico");
                if (File.Exists(iconPath))
                {
                    Icon = BitmapFrame.Create(new Uri(iconPath));
                }
            }
            catch { }
        }

        private void CheckToolsPath()
        {
            if (!Directory.Exists(toolsBasePath))
            {
                MessageBox.Show(
                    $"未找到工具目录：{toolsBasePath}\n\n请将 TubaToolbox.exe 复制到图吧工具箱的安装目录下运行（与 tools 文件夹同级），否则工具列表中的程序将无法启动。",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void LoadCategoryPages()
        {
            try
            {
                categoryPages["硬件信息"] = new HardwareInfoPage();
                categoryPages["CPU工具"] = new ToolsPage(GetCpuTools(), toolsBasePath);
                categoryPages["主板工具"] = new ToolsPage(GetMotherboardTools(), toolsBasePath);
                categoryPages["内存工具"] = new ToolsPage(GetMemoryTools(), toolsBasePath);
                categoryPages["显卡工具"] = new ToolsPage(GetGpuTools(), toolsBasePath);
                categoryPages["磁盘工具"] = new ToolsPage(GetDiskTools(), toolsBasePath);
                categoryPages["屏幕工具"] = new ToolsPage(GetScreenTools(), toolsBasePath);
                categoryPages["综合工具"] = new ToolsPage(GetComprehensiveTools(), toolsBasePath);
                categoryPages["外设工具"] = new ToolsPage(GetPeripheralTools(), toolsBasePath);
                categoryPages["烤鸡工具"] = new ToolsPage(GetStressTestTools(), toolsBasePath);
                categoryPages["游戏工具"] = new ToolsPage(GetGameTools(), toolsBasePath);
                categoryPages["其他工具"] = new ToolsPage(GetOtherTools(), toolsBasePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载工具列表时出错：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Category_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                // 重置之前按钮的样式
                if (lastClickedButton != null)
                {
                    lastClickedButton.Background = Brushes.Transparent;
                    lastClickedButton.Foreground = new SolidColorBrush(Color.FromRgb(0xDC, 0xDC, 0xDC));
                }
                
                // 高亮当前按钮
                btn.Background = new SolidColorBrush(Color.FromRgb(0x29, 0x80, 0xB9));
                btn.Foreground = Brushes.White;
                lastClickedButton = btn;

                ShowCategory(tag);
            }
        }

        private void ShowCategory(string category)
        {
            if (!string.IsNullOrEmpty(category) && categoryPages.ContainsKey(category))
            {
                contentArea.Content = categoryPages[category];
            }
        }

        // ==================== 工具列表定义 ====================

        private List<ToolItem> GetCpuTools() => new()
        {
            new("CPU-Z (64位)", "处理器工具\\CPUZ\\cpuz_x64.exe"),
            new("CPU-Z (32位)", "处理器工具\\CPUZ\\cpuz_x32.exe"),
            new("LinX", "处理器工具\\LinX\\LinX.exe"),
            new("Prime95", "处理器工具\\Prime95\\prime95.exe"),
            new("P95一键烤机", "处理器工具\\Prime95\\start.bat"),
            new("SuperPI", "处理器工具\\superpi\\Superpi.exe"),
            new("wPrime", "处理器工具\\wPrime\\wPrime.exe"),
            new("Fritz Chess", "处理器工具\\XIANGQI\\xiangqi.exe"),
            new("CPU天梯图", "其他工具\\天梯图\\CPU天梯图.jpg", true),
            new("cpu burner", "烤鸡工具\\FurMark\\cpuburner.exe"),
            new("Core Temp 64", "处理器工具\\CoreTemp\\Core Temp x64.exe"),
            new("Core Temp 32", "处理器工具\\CoreTemp\\Core Temp x86.exe"),
            new("AIDA64", "综合检测\\AIDA64\\aida64.exe"),
            new("ThrottleStop", "处理器工具\\ThrottleStop\\ThrottleStop.exe"),
            new("核间延迟测试", "处理器工具\\C2CLatency\\C2CLatency.exe"),
            new("流量卡售后", "常用工具\\免费领流量卡\\Start.bat"),
            new("新手指引", null, false, true),
        };

        private List<ToolItem> GetMotherboardTools() => new() { new("AIDA64", "综合检测\\AIDA64\\aida64.exe") };
        
        private List<ToolItem> GetMemoryTools() => new()
        {
            new("MemTest", "内存工具\\memtest\\memtest.exe"),
            new("MemTest64", "内存工具\\memtest64\\MemTest64.exe"),
            new("魔方内存盘", "内存工具\\魔方内存盘\\ramdisk.exe"),
            new("Thaiphoon", "内存工具\\Thaiphoon\\Thaiphoon.exe"),
            new("MemTest pro", "内存工具\\memtestpro\\memtestpro.exe"),
            new("TM5", "内存工具\\tm5\\TM5.exe"),
            new("ZenTimings", null),
        };

        private List<ToolItem> GetGpuTools() => new()
        {
            new("GPU-Z", "显卡工具\\GPUZ\\GPU-Z.exe"),
            new("一键烤显卡", "烤鸡工具\\FurMark\\start.bat"),
            new("一键烤存", "烤鸡工具\\FurMark\\一键烤显存.bat"),
            new("FurMark", "烤鸡工具\\FurMark\\FurMark.exe"),
            new("FurMark2", "烤鸡工具\\FurMark_win64\\FurMark_GUI.exe"),
            new("显存测试", "烤鸡工具\\FurMark_win64\\gpushark.bat"),
            new("GpuTest", "显卡工具\\GpuTest_Windows x64\\GpuTest_GUI.exe"),
            new("显卡天梯图", "其他工具\\天梯图\\显卡天梯图.jpg", true),
            new("DXVAChecker", "显卡工具\\dxvachecker\\DXVAChecker.exe"),
            new("nvidiaInspector", "显卡工具\\nvidiaInspector\\nvidiaInspector.exe"),
            new("Profile Insp.", "显卡工具\\nvidiaProfileInspector\\nvidiaProfileInspector.exe"),
            new("DDU", "显卡工具\\DDU\\Display Driver Uninstaller.exe"),
            new("gpushark", "烤鸡工具\\FurMark\\gpushark.exe"),
            new("gpushark2.0", "烤鸡工具\\FurMark_win64\\_fm2-gui.exe"),
            new("游戏加加", "其他工具\\游戏加加\\start.bat"),
            new("Nvidia驱动", "显卡工具\\Nvidia显卡驱动下载\\Start.bat"),
            new("AMD驱动", "显卡工具\\AMD显卡驱动下载\\Start.bat"),
        };

        private List<ToolItem> GetDiskTools() => new()
        {
            new("AS SSD", "硬盘工具\\ASSSDBenchmark\\ASSSDBenchmark.exe"),
            new("DiskInfo 64", "硬盘工具\\CrystalDiskInfo\\DiskInfo64S.exe"),
            new("DiskInfo 32", "硬盘工具\\CrystalDiskInfo\\DiskInfo32S.exe"),
            new("DiskMark 64", "硬盘工具\\CrystalDiskMark\\DiskMark64S.exe"),
            new("DiskMark 32", "硬盘工具\\CrystalDiskMark\\DiskMark64.exe"),
            new("HDTune", "硬盘工具\\HDTune\\HDTune.exe"),
            new("ATTO", "硬盘工具\\ATTODISKBENCHMARK\\ATTO 磁盘基准测试.exe"),
            new("TxBENCH", "硬盘工具\\TxBENCH\\TxBENCH.exe"),
            new("Defraggler", "硬盘工具\\Defraggler\\Defraggler.exe"),
            new("DiskGenius", "硬盘工具\\DiskGenius\\DiskGenius.exe"),
            new("MyDiskTest", "硬盘工具\\mydisktest\\MyDiskTest_v298.exe"),
            new("魔方数据恢复", "硬盘工具\\魔方数据恢复\\魔方数据恢复.exe"),
            new("FINALDATA", "硬盘工具\\finaldata\\FINALDATA.exe"),
            new("FdWizard", "硬盘工具\\finaldata\\FdWizard.exe"),
            new("LLFTOOL", "硬盘工具\\LLFTOOL\\LLFTOOL.exe"),
            new("h2testw", "硬盘工具\\H2testw\\h2testw_1.4.exe"),
            new("urwtest", "硬盘工具\\URWTEST\\urwtest_v18.exe"),
            new("windirstat", "硬盘工具\\windirstat\\windirstat.exe"),
            new("SpaceSniffer", "硬盘工具\\SpaceSniffer\\SpaceSniffer.exe"),
            new("WizTree", "硬盘工具\\WizTree\\WizTree.exe"),
            new("SSDZ", "硬盘工具\\SSDZ\\SSDZ.exe"),
            new("Flash ID", null),
            new("FlashMaster", null),
        };

        private List<ToolItem> GetScreenTools() => new()
        {
            new("坏点漏光测试", "显示器工具\\UFO测试\\Start.bat"),
            new("色域检测", "显示器工具\\色域检测\\monitorinfo.exe"),
            new("在线屏测", "显示器工具\\在线屏幕测试\\在线屏幕测试.bat"),
            new("UFO测试", "显示器工具\\UFO测试\\Start.bat"),
        };

        private List<ToolItem> GetComprehensiveTools() => new()
        {
            new("AIDA64", "综合检测\\AIDA64\\aida64.exe"),
            new("HWiNFO 64", "综合检测\\hwinfo\\HWiNFO64.exe"),
            new("HWiNFO 32", "综合检测\\hwinfo\\HWiNFO32.exe"),
            new("Speccy 64", "综合检测\\speccy\\Speccy64.exe"),
            new("Speccy 32", "综合检测\\speccy\\Speccy.exe"),
            new("RWEverything", "综合检测\\RWEverything\\Rw.exe"),
            new("HWMonitor 64", "综合检测\\HWMonitor\\HWMonitor_x64.exe"),
            new("HWMonitor 32", "综合检测\\HWMonitor\\HWMonitor_x32.exe"),
        };

        private List<ToolItem> GetPeripheralTools() => new()
        {
            new("外设测试中心", "外设工具\\在线外设测试中心\\在线外设测试中心.bat"),
            new("Keyboard Test", "外设工具\\Keyboard Test Utility\\Keyboard Test Utility.exe"),
            new("鼠标双击测试", "外设工具\\鼠标单机变双击测试器\\鼠标单击变双击测试器V2.0.exe"),
            new("MOUSERATE", "外设工具\\MOUSERATE\\MOUSERATE.EXE"),
            new("AresonMouse", "外设工具\\AresonMouseTest\\鼠标测试软件AresonMouseTestProgram.exe"),
            new("MouseTester", "外设工具\\MouseTester\\MouseTester.exe"),
            new("KeyTweak", "外设工具\\KeyTweak\\KeyTweak.exe"),
        };

        private List<ToolItem> GetStressTestTools() => new()
        {
            new("FurMark", "烤鸡工具\\FurMark\\FurMark.exe"),
            new("GpuTest", "显卡工具\\GpuTest_Windows x64\\GpuTest_GUI.exe"),
            new("Prime95", "处理器工具\\Prime95\\prime95.exe"),
            new("LinX", "处理器工具\\LinX\\LinX.exe"),
            new("AIDA64", "综合检测\\AIDA64\\aida64.exe"),
            new("cpu burner", "烤鸡工具\\FurMark\\cpuburner.exe"),
            new("一键烤鸡", "烤鸡工具\\FurMark\\start.bat"),
            new("FurMark2", "烤鸡工具\\FurMark_win64\\FurMark_GUI.exe"),
        };

        private List<ToolItem> GetGameTools() => new()
        {
            new("Steam下载", "游戏工具\\Steam\\下载Steam.bat"),
            new("EPC下载", "游戏工具\\epic\\Start.bat"),
            new("EA APP下载", "游戏工具\\eaapp\\Start.bat"),
            new("战网下载", "游戏工具\\battle\\Start.bat"),
            new("帧数温度", "其他工具\\游戏加加\\start.bat"),
            new("存档安装", "游戏工具\\GameBuff\\Start.bat"),
            new("GameBuff", "游戏工具\\GameBuff\\Start.bat"),
            new("风灵月影", "游戏工具\\风灵月影\\Start.bat"),
            new("迅雷加速器", "游戏工具\\迅雷加速器\\Start.bat"),
            new("雷神加速器", "游戏工具\\雷神加速器\\Start.bat"),
            new("迅游加速器", "游戏工具\\迅游加速器\\Start.bat"),
            new("斧牛加速器", "游戏工具\\斧牛加速器\\Start.bat"),
            new("玩家动力", "游戏工具\\玩家动力\\Start.bat"),
            new("游牛加速器", null),
            new("DirectX Repair", "其他工具\\DirectX_Repair\\DirectX Repair.exe"),
        };

        private List<ToolItem> GetOtherTools() => new()
        {
            new("BatteryInfo", "其他工具\\BatteryInfoView\\BatteryInfoView.exe"),
            new("ULTRAISO", "其他工具\\ULTRAISO\\ULTRAISO.exe"),
            new("Geek Uninst.", "其他工具\\Geek Uninstaller\\Geek Uninstaller.exe"),
            new("Dism++ x86", "其他工具\\Dism++\\Dism++x86.exe"),
            new("Dism++ x64", "其他工具\\Dism++\\Dism++x64.exe"),
            new("Dism++ ARM64", "其他工具\\Dism++\\Dism++ARM64.exe"),
            new("Everything", "其他工具\\Everything\\everything.exe"),
            new("GifCam", "其他工具\\gifcam\\GifCam.exe"),
            new("微星小飞机", null),
            new("检查更新", "常用工具\\检查更新\\检查更新.bat"),
            new("BSView 64", "其他工具\\bluescreenview\\BlueScreenViewx64.exe"),
            new("BSView 32", "其他工具\\bluescreenview\\BlueScreenViewx86.exe"),
            new("游戏加速", null),
            new("DesktopOK", "其他工具\\DesktopOK\\DesktopOK.exe"),
            new("procexp 64", "其他工具\\procexp\\procexp64.exe"),
            new("procexp 32", "其他工具\\procexp\\procexp.exe"),
            new("rufus", "其他工具\\rufus\\rufus.exe"),
            new("Ventoy", "其他工具\\ventoy\\Ventoy2Disk.exe"),
            new("NEXT ITY", "其他工具\\next_itellyou\\Start.bat"),
        };
    }
}
