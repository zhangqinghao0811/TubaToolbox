# Tuba Toolbox 硬件工具箱

> 一款现代化的硬件检测与工具集合启动器，基于 WPF + .NET 8 开发，完美解决原版图吧工具箱的中文乱码问题。

## 项目简介

Tuba Toolbox 是对经典「图吧工具箱」的现代化重写版本。原版软件在 Windows 开启 **"Beta: 使用 Unicode UTF-8 提供全球语言支持"** 后会出现严重的中文乱码问题。本项目使用 WPF 框架从零重写，彻底解决编码问题的同时提供更美观的界面和更好的用户体验。

## 功能特性

### 12 大分类，100+ 硬件工具

| 分类     | 包含工具                                                       |
| ------ | ---------------------------------------------------------- |
| 硬件信息   | 自动检测并展示电脑完整硬件配置（CPU、主板、内存、显卡、磁盘、声卡、网卡等）                    |
| CPU 工具 | CPU-Z、LinX、Prime95、Core Temp、ThrottleStop、SuperPI、wPrime 等 |
| 主板工具   | AIDA64 综合检测                                                |
| 内存工具   | MemTest、MemTest64、Thaiphoon Burner、TM5、魔方内存盘 等             |
| 显卡工具   | GPU-Z、FurMark、GpuTest、DDU、nvidiaInspector 等                |
| 磁盘工具   | AS SSD、CrystalDiskInfo/Mark、HDTune、ATTO、DiskGenius 等       |
| 屏幕工具   | 坏点漏光测试、色域检测、UFO 测试                                         |
| 综合工具   | AIDA64、HWiNFO、Speccy、RWEverything、HWMonitor                |
| 外设工具   | 键盘测试、鼠标测试、外设测试中心、KeyTweak                                  |
| 烤鸡工具   | FurMark、Prime95、LinX、cpu burner 一键烤机                       |
| 游戏工具   | Steam/Epic/EA/战网下载、游戏加速器合集、存档工具                            |
| 其他工具   | BatteryInfoView、Dism++、Everything、Ventoy、rufus 等           |

### 核心优势

- ✅ **零乱码** — 原生 UTF-8 支持，任何系统区域设置下均正常显示中文
- ✅ **单文件发布** — 发布为独立 .exe，无需安装运行时（需 .NET 8 Runtime）
- ✅ **零配置** — 将 exe 放入 `tools` 文件夹同级目录即可运行
- ✅ **现代 UI** — WPF 矢量渲染，支持高 DPI 缩放，流畅动画
- ✅ **自动图标** — 自动提取各工具 exe 的图标显示
- ✅ **双击启动** — 双击工具卡片即可启动对应程序

## 技术栈

- **框架**: WPF (Windows Presentation Foundation)
- **运行时**: .NET 8.0 (Windows Desktop)
- **语言**: C# 12 / XAML
- **架构**: MVVM-like UserControl 组合模式
- **硬件检测**: System.Management (WMI) + Microsoft.Win32.Registry

## 项目结构

```
TubaToolbox/
├── src/                          # 源代码目录
│   ├── App.xaml                 # 应用入口 & 全局样式资源
│   ├── App.xaml.cs
│   ├── MainWindow.xaml          # 主窗口（Grid 两栏布局）
│   ├── MainWindow.xaml.cs       # 分类切换逻辑 & 工具列表定义
│   ├── HardwareInfoPage.xaml    # 硬件信息页面
│   ├── HardwareInfoPage.xaml.cs # WMI 硬件数据采集
│   ├── ToolsPage.xaml           # 工具列表面板
│   ├── ToolsPage.xaml.cs        # 工具卡片渲染 & 启动逻辑
│   ├── ToolItem.cs              # 工具项数据模型
│   ├── IconGenerator.cs         # 应用图标自动生成
│   └── TubaToolbox.csproj       # 项目配置文件
├── .gitignore
└── README.md
```

## 快速开始

### 环境要求

- Windows 10/11 x64
- [.NET 8.0 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)（如未安装可改为 SelfContained 发布）

### 从源码构建

```bash
# 克隆项目
git clone https://github.com/yourname/TubaToolbox.git
cd TubaToolbox

# 还原依赖
dotnet restore src/TubaToolbox.csproj

# Release 编译
dotnet build src/TubaToolbox.csproj -c Release

# 发布为单文件 exe
dotnet publish src/TubaToolbox.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

### 运行

1. 确保 `tools` 文件夹与 `TubaToolbox.exe` 在同一目录。（放到图吧工具箱安装路径即可。）
2. 双击 `TubaToolbox.exe` 即可运行

## 与原版对比

| 特性       | 原版图吧工具箱           | Tuba Toolbox |
| -------- | ----------------- | ------------ |
| UTF-8 乱码 | ❌ 严重乱码            | ✅ 完美支持       |
| 框架       | Delphi / WinForms | WPF + .NET 8 |
| 高 DPI 支持 | ❌ 模糊              | ✅ 原生矢量       |
| 单文件发布    | 需打包               | ✅ 原生支持       |
| 开源       | ❌ 闭源              | ✅ 开源         |

## 开发说明

### 添加新工具

编辑 [MainWindow.xaml.cs](src/MainWindow.xaml.cs) 中对应的 `GetXxxTools()` 方法：

```csharp
private List<ToolItem> GetCpuTools() => new()
{
    new("工具名称", "相对路径\\to\\tool.exe"),
    new("图片查看", "路径\\image.jpg", isImage: true),  // 图片类型
    new("仅展示", null, isInfoOnly: true),               // 仅展示不可点击
};
```

### 自定义样式

全局样式定义在 [App.xaml](src/App.xaml) 的 `<Application.Resources>` 中：

- `CategoryButton` — 左侧分类按钮样式
- `ToolCard` — 句柄卡片样式（悬停效果）

## License

MIT License

## 致谢

- 感谢「图吧工具箱」原版作者提供的优秀工具集合
- 感谢所有开源硬件检测工具的开发者

