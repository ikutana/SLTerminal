using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SLTerminal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Process TheTerminal;
        public TextBox LastBlock;
        public String OutputRequest = "";
        public String InputRequest = "";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GridSplitter_LayoutUpdated(object sender, EventArgs e)
        {
             
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheTerminal = new Process();
            TheTerminal.StartInfo.CreateNoWindow = true;
            TheTerminal.StartInfo.RedirectStandardOutput = true;
            TheTerminal.StartInfo.RedirectStandardError = true;
            TheTerminal.StartInfo.RedirectStandardInput = true;
            TheTerminal.StartInfo.FileName = "cmd.exe";
            TheTerminal.OutputDataReceived += TheTerminal_OutputDataReceived;
            TheTerminal.ErrorDataReceived += TheTerminal_OutputDataReceived;
            TheTerminal.Start();
            TheTerminal.BeginOutputReadLine();
            TheTerminal.BeginErrorReadLine();
        }

        private void TheTerminal_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            var received = e.Data;
            Dispatcher.Invoke(() =>
            {
                if (LastBlock == null || (string)LastBlock.Tag != "Output")
                {
                    LastBlock = new TextBox()
                    {
                        Background = System.Windows.Media.Brushes.White,
                        Foreground = System.Windows.Media.Brushes.Black,
                        Tag = "Output",
                        TextWrapping = TextWrapping.Wrap,
                        AcceptsReturn = true
                        
                    };
                    LastBlock.Margin =  new Thickness(0,0,80,0);
                    var DP = new DockPanel()
                    {
                        LastChildFill = false
                    }
                    ;
                    DockPanel.SetDock(LastBlock, Dock.Left);
                    DP.Children.Add(LastBlock);
                    OutputArea.Children.Add(DP);
                    var tb = new TextBlock() { Background = System.Windows.Media.Brushes.White };
                    DockPanel.SetDock(tb, Dock.Right);
                    OutputArea.Children.Add(tb);

                }
                LastBlock.Text += received+"\n";
                LastBlock.Width = LastBlock.Text.Split('\n').Max(p => p.Length) * 6;
                SV.ScrollToEnd();
            });
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            var Key = e.Key;
            if (e.Key == Key.Return||e.Key == Key.Enter)
            {
                Dispatcher.Invoke(() =>
                {
                    if (LastBlock == null || (string)LastBlock.Tag != "Input")
                    {
                        LastBlock = new TextBox();
                        LastBlock.Background = System.Windows.Media.Brushes.LightGreen;
                        LastBlock.Foreground = System.Windows.Media.Brushes.Black;
                        LastBlock.Tag = "Input";
                        LastBlock.TextAlignment = TextAlignment.Right;
                        LastBlock.Margin = new Thickness(160, 0, 0, 0);

                        var DP = new DockPanel()
                        {
                            Width = OutputArea.Width,
                            LastChildFill = true
                        };
                        DockPanel.SetDock(LastBlock, Dock.Right);
                        OutputArea.Children.Add(LastBlock);
                        var tb = new TextBlock() { Background = System.Windows.Media.Brushes.White };
                        DockPanel.SetDock(tb, Dock.Left);
                        OutputArea.Children.Add(tb);
                    }
                    var cmdline = InputArea.Text.Trim('\n');
                    LastBlock.Text += cmdline;
                    InputArea.Text = "";
                    TheTerminal.StandardInput.WriteLine(cmdline);
                    SV.ScrollToEnd();
                    LastBlock.Width = cmdline.Length * 6;
                }
                );
            }
        }
    }
}
