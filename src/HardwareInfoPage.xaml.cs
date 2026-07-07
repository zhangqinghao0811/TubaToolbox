using System;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace TubaToolbox
{
    public partial class HardwareInfoPage : UserControl
    {
        public HardwareInfoPage()
        {
            InitializeComponent();
            Loaded += (s, e) => BuildUI();
        }

        private void BuildUI()
        {
            rootPanel.Children.Clear();

            // 标题
            var title = new TextBlock
            {
                Text = "硬件信息",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                FontFamily = new FontFamily("Microsoft YaHei UI"),
                Margin = new Thickness(0, 0, 0, 15)
            };
            rootPanel.Children.Add(title);

            // 信息卡片
            AddInfoCard("型号信息", GetComputerModel());
            AddInfoCard("系统信息", GetSystemInfo());
            AddInfoCard("运行时间", GetUptime());

            // 详细信息标题
            var detailTitle = new TextBlock
            {
                Text = "详细信息",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(0xDC, 0xFF, 0xFF)),
                FontFamily = new FontFamily("Microsoft YaHei UI"),
                Margin = new Thickness(0, 15, 0, 10)
            };
            rootPanel.Children.Add(detailTitle);

            // 详细信息列表
            string[] details = {
                $"处理器：{GetProcessorInfo()}",
                $"主板：{GetMotherboardInfo()}",
                $"内存：{GetMemoryInfo()}",
                $"显卡：{GetGraphicsInfo()}",
                $"显示器：{GetMonitorInfo()}",
                $"磁盘：{GetDiskInfo()}",
                $"声卡：{GetSoundCardInfo()}",
                $"网卡：{GetNetworkAdapterInfo()}"
            };

            foreach (var detail in details)
            {
                var label = new TextBlock
                {
                    Text = detail,
                    FontSize = 13,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Microsoft YaHei UI"),
                    Margin = new Thickness(0, 4, 0, 2),
                    TextWrapping = TextWrapping.Wrap
                };
                rootPanel.Children.Add(label);
            }
        }

        private void AddInfoCard(string labelText, string valueText)
        {
            var cardBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(45, 255, 255, 255)),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(12, 10, 12, 10),
                Margin = new Thickness(0, 0, 0, 8)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var label = new TextBlock
            {
                Text = labelText + "：",
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(130, 200, 255)),
                FontFamily = new FontFamily("Microsoft YaHei UI"),
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(label, 0);
            grid.Children.Add(label);

            var value = new TextBlock
            {
                Text = valueText,
                FontSize = 13,
                Foreground = Brushes.White,
                FontFamily = new FontFamily("Microsoft YaHei UI"),
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5, 0, 0, 0)
            };
            Grid.SetColumn(value, 1);
            grid.Children.Add(value);

            cardBorder.Child = grid;
            rootPanel.Children.Add(cardBorder);
        }

        private string GetComputerModel()
        {
            try
            {
                using (var mc = new ManagementClass("Win32_ComputerSystem"))
                foreach (ManagementObject mo in mc.GetInstances())
                    return $"{mo["Manufacturer"]} {mo["Model"]}";
            }
            catch { }
            return "无法获取";
        }

        private string GetSystemInfo()
        {
            try
            {
                string arch = Environment.Is64BitOperatingSystem ? "64位" : "32位";
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    if (key != null)
                    {
                        string productName = key.GetValue("ProductName", "Windows")?.ToString() ?? "Windows";
                        string displayVersion = key.GetValue("DisplayVersion", "")?.ToString() ?? "";

                        // 读取 Build Number，优先 CurrentBuildNumber，其次 CurrentBuild
                        int buildNumber = 0;
                        object buildObj = key.GetValue("CurrentBuildNumber") ?? key.GetValue("CurrentBuild");
                        if (buildObj != null)
                            int.TryParse(buildObj.ToString(), out buildNumber);

                        // 判断主版本：Win11 的 BuildNumber >= 22000
                        // 注意：Win11 的 ProductName 注册表项仍为 "Windows 10"，需根据 BuildNumber 修正
                        string majorName = productName;
                        if (buildNumber >= 22000)
                        {
                            if (majorName.Contains("Windows 10"))
                                majorName = majorName.Replace("Windows 10", "Windows 11");
                            else if (!majorName.Contains("Windows 11"))
                                majorName = "Windows 11 " + majorName;
                        }

                        // 将英文版名翻译为中文，与 Windows 中文版显示一致
                        majorName = TranslateEditionToChinese(majorName);

                        string version = !string.IsNullOrEmpty(displayVersion) ? $" {displayVersion}" : "";
                        return $"{majorName}{version} {arch}";
                    }
                }
                return $"Windows {arch}";
            }
            catch { }
            return "无法获取";
        }

        /// <summary>
        /// 将 ProductName 中的英文版名翻译为中文，与 Windows 中文版显示一致。
        /// 例如 "Windows 11 Pro" → "Windows 11 专业版"
        /// </summary>
        private static string TranslateEditionToChinese(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            // 顺序敏感：先替换长串，避免 "Pro" 匹配到 "Pro for Workstations" 中的部分
            return name
                .Replace("Pro for Workstations", "工作站版")
                .Replace("Professional", "专业版")
                .Replace(" Pro", " 专业版")
                .Replace("Home Single Language", "家庭单语言版")
                .Replace("Home", "家庭版")
                .Replace("Education", "教育版")
                .Replace("Enterprise", "企业版");
        }

        private string GetUptime()
        {
            try
            {
                int sec = Environment.TickCount / 1000;
                return $"{sec / 86400}天{(sec % 86400) / 3600}小时{(sec % 3600) / 60}分钟";
            }
            catch { }
            return "无法获取";
        }

        private string GetProcessorInfo()
        {
            try
            {
                using (var mc = new ManagementClass("Win32_Processor"))
                foreach (ManagementObject mo in mc.GetInstances())
                    return $"{mo["Name"]} ({mo["NumberOfCores"]}核{mo["NumberOfLogicalProcessors"]}线程)";
            }
            catch { }
            return "无法获取";
        }

        private string GetMotherboardInfo()
        {
            try
            {
                using (var mc = new ManagementClass("Win32_BaseBoard"))
                foreach (ManagementObject mo in mc.GetInstances())
                    return $"{mo["Manufacturer"]} {mo["Product"]}".Trim();
            }
            catch { }
            return "无法获取";
        }

        private string GetMemoryInfo()
        {
            try
            {
                long totalMB = 0;
                var modules = new System.Collections.Generic.List<string>();
                using (var mc = new ManagementClass("Win32_PhysicalMemory"))
                foreach (ManagementObject mo in mc.GetInstances())
                {
                    totalMB += Convert.ToInt64(mo["Capacity"]) / (1024 * 1024);
                    double gb = Convert.ToInt64(mo["Capacity"]) / (1024.0 * 1024.0 * 1024.0);
                    modules.Add($"{gb:F0}GB");
                }
                double totalGB = totalMB / 1024.0;
                return modules.Count > 0 ? $"{totalGB:F1}GB DDR ({string.Join(" + ", modules)})" : $"{totalGB:F1}GB DDR";
            }
            catch { }
            return "无法获取";
        }

        private string GetGraphicsInfo()
        {
            try
            {
                var cards = new System.Collections.Generic.List<string>();
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
                foreach (ManagementObject mo in searcher.Get())
                {
                    string name = mo["Name"]?.ToString();
                    if (!string.IsNullOrEmpty(name)) cards.Add(name);
                }
                return cards.Count > 0 ? string.Join(" / ", cards.ToArray(), 0, Math.Min(cards.Count, 2)) : "无法获取";
            }
            catch { }
            return "无法获取";
        }

        private string GetMonitorInfo()
        {
            try
            {
                var monitors = new System.Collections.Generic.List<string>();

                // 首选：Win32_PnPEntity 获取显示器设备名（格式如 "Generic Monitor (P27H2V)"）
                // 提取括号内的真实型号，与 Windows 设置>系统>屏幕 高级显示器设置 一致
                try
                {
                    using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE PNPClass='Monitor'"))
                    {
                        foreach (ManagementObject mo in searcher.Get())
                        {
                            string name = mo["Name"]?.ToString();
                            if (string.IsNullOrEmpty(name)) continue;

                            string realName = ExtractMonitorModelName(name);
                            if (!string.IsNullOrEmpty(realName) && !monitors.Contains(realName))
                                monitors.Add(realName);
                        }
                    }
                }
                catch { }

                // 次选：WmiMonitorID 从 EDID 读取 UserFriendlyName
                if (monitors.Count == 0)
                {
                    try
                    {
                        using (var searcher = new ManagementObjectSearcher(@"root\wmi", "SELECT * FROM WmiMonitorID WHERE Active = TRUE"))
                        {
                            foreach (ManagementObject mo in searcher.Get())
                            {
                                string name = ReadWmiMonitorName(mo);
                                if (!string.IsNullOrEmpty(name) && !monitors.Contains(name))
                                    monitors.Add(name);
                            }
                        }
                    }
                    catch { }
                }

                // 最后回退：Win32_DesktopMonitor
                if (monitors.Count == 0)
                {
                    using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DesktopMonitor"))
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        string name = mo["Name"]?.ToString();
                        if (!string.IsNullOrEmpty(name) && !monitors.Contains(name)) monitors.Add(name);
                    }
                }

                return monitors.Count > 0 ? string.Join(" / ", monitors) : "无法获取";
            }
            catch { }
            return "无法获取";
        }

        /// <summary>
        /// 从 "Generic Monitor (P27H2V)" 这种格式中提取括号内的真实型号。
        /// 若无括号，则返回原始名称。
        /// </summary>
        private static string ExtractMonitorModelName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            int start = name.IndexOf('(');
            int end = name.LastIndexOf(')');
            if (start >= 0 && end > start + 1)
            {
                return name.Substring(start + 1, end - start - 1).Trim();
            }
            return name;
        }

        /// <summary>
        /// 从 WmiMonitorID 实例中读取 UserFriendlyName 字段（UInt16 数组）并转换为字符串。
        /// </summary>
        private static string ReadWmiMonitorName(ManagementObject mo)
        {
            try
            {
                object rawObj = mo["UserFriendlyName"];
                if (rawObj == null) return null;

                ushort[] raw;
                if (rawObj is ushort[] direct)
                {
                    raw = direct;
                }
                else if (rawObj is Array arr)
                {
                    raw = new ushort[arr.Length];
                    for (int i = 0; i < arr.Length; i++)
                        raw[i] = Convert.ToUInt16(arr.GetValue(i));
                }
                else return null;

                if (raw.Length == 0) return null;

                var chars = new char[raw.Length];
                for (int i = 0; i < raw.Length; i++)
                {
                    if (raw[i] == 0) { Array.Resize(ref chars, i); break; }
                    chars[i] = (char)raw[i];
                }
                var name = new string(chars).TrimEnd('\0', ' ');
                return string.IsNullOrEmpty(name) ? null : name;
            }
            catch { return null; }
        }

        private string GetDiskInfo()
        {
            try
            {
                var disks = new System.Collections.Generic.List<string>();
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive"))
                foreach (ManagementObject mo in searcher.Get())
                {
                    string model = mo["Model"]?.ToString();
                    if (!string.IsNullOrEmpty(model))
                    {
                        long sizeGB = Convert.ToInt64(mo["Size"]) / (1024L * 1024L * 1024L);
                        disks.Add($"{model} ({sizeGB:F0}GB)");
                    }
                }
                return disks.Count > 0 ? string.Join("\n              ", disks.ToArray(), 0, Math.Min(disks.Count, 2)) : "无法获取";
            }
            catch { }
            return "无法获取";
        }

        private string GetSoundCardInfo()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_SoundDevice"))
                foreach (ManagementObject mo in searcher.Get())
                {
                    string name = mo["Name"]?.ToString();
                    if (!string.IsNullOrEmpty(name)) return name;
                }
            }
            catch { }
            return "无法获取";
        }

        private string GetNetworkAdapterInfo()
        {
            try
            {
                var adapters = new System.Collections.Generic.List<string>();
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionID IS NOT NULL"))
                foreach (ManagementObject mo in searcher.Get())
                {
                    string name = mo["Name"]?.ToString();
                    if (!string.IsNullOrEmpty(name) && !adapters.Contains(name)) adapters.Add(name);
                }
                return adapters.Count > 0 ? string.Join(" / ", adapters) : "无法获取";
            }
            catch { }
            return "无法获取";
        }
    }
}
