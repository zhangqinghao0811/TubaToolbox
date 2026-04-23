using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TubaToolbox
{
    public partial class ToolsPage : UserControl
    {
        private string basePath;

        public ToolsPage(List<ToolItem> tools, string basePath)
        {
            this.basePath = basePath;
            InitializeComponent();
            BuildTools(tools);
        }

        private void BuildTools(List<ToolItem> tools)
        {
            foreach (var tool in tools)
            {
                var card = new Button
                {
                    Style = FindResource("ToolCard") as Style,
                };

                var stackPanel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };

                // 图标
                var iconImage = new Image
                {
                    Width = 28,
                    Height = 28,
                    Margin = new Thickness(0, 0, 0, 4),
                    VerticalAlignment = VerticalAlignment.Top
                };
                
                if (!string.IsNullOrEmpty(tool.RelativePath))
                {
                    string fullPath = Path.Combine(basePath, tool.RelativePath);
                    if (File.Exists(fullPath) && !tool.IsImage)
                    {
                        try
                        {
                            iconImage.Source = ExtractIcon(fullPath);
                        }
                        catch { iconImage.Source = CreateDefaultIcon(); }
                    }
                    else if (tool.IsImage && File.Exists(fullPath))
                    {
                        try { iconImage.Source = new BitmapImage(new Uri(fullPath)); } catch { }
                    }
                    else
                    {
                        iconImage.Source = CreateDefaultIcon();
                    }
                }
                else
                {
                    iconImage.Source = CreateDefaultIcon();
                }

                stackPanel.Children.Add(iconImage);

                // 名称
                var nameText = new TextBlock
                {
                    Text = tool.Name,
                    FontSize = 11,
                    FontFamily = new FontFamily("Microsoft YaHei UI"),
                    Foreground = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33)),
                    TextWrapping = TextWrapping.Wrap,
                    MaxWidth = 125,
                    TextAlignment = TextAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                stackPanel.Children.Add(nameText);

                card.Content = stackPanel;

                if (tool.IsInfoOnly)
                {
                    card.IsEnabled = false;
                    card.Opacity = 0.7;
                }
                else
                {
                    card.Tag = tool;
                    card.MouseDoubleClick += ToolCard_DoubleClick;
                }

                toolsPanel.Children.Add(card);
            }
        }

        private void ToolCard_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ToolItem tool)
            {
                LaunchTool(tool);
            }
        }

        private void LaunchTool(ToolItem tool)
        {
            try
            {
                if (string.IsNullOrEmpty(tool.RelativePath)) return;

                string fullPath = Path.Combine(basePath, tool.RelativePath);

                if (!File.Exists(fullPath))
                {
                    MessageBox.Show($"文件不存在：{fullPath}", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = fullPath,
                    WorkingDirectory = Path.GetDirectoryName(fullPath),
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ImageSource ExtractIcon(string filePath)
        {
            try
            {
                using (var icon = System.Drawing.Icon.ExtractAssociatedIcon(filePath))
                {
                    if (icon != null)
                    {
                        using (var bmp = icon.ToBitmap())
                        using (var ms = new MemoryStream())
                        {
                            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            ms.Position = 0;
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.StreamSource = ms;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                            bitmap.Freeze();
                            return bitmap;
                        }
                    }
                }
            }
            catch { }
            return CreateDefaultIcon();
        }

        private ImageSource CreateDefaultIcon()
        {
            var drawing = new GeometryDrawing
            {
                Brush = new SolidColorBrush(Color.FromRgb(0x34, 0x98, 0xDB)),
                Pen = null,
                Geometry = new EllipseGeometry(new Point(14, 14), 12, 12)
            };

            var drawingImage = new DrawingImage(drawing);
            drawingImage.Freeze();
            return drawingImage;
        }
    }
}
