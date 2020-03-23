using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
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

namespace wpf_youtube_dl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private PSDataCollection<PSObject> outputCollection = new PSDataCollection<PSObject>();
        
        private PowerShell ps;
        private IAsyncResult iasresult;
        public MainWindow()
        {
            InitializeComponent();
            tb_Path.Text = Directory.GetCurrentDirectory();

            // For my own test only.

            //tb_Proxy.Text = "127.0.0.1:30997";
            //tb_Path.Text = "R:\\! Youtube Video\\";
        }

        private void btn_StartDownload_Click(object sender, RoutedEventArgs e)
        {
            // Check path and try to fix first
            if (tb_Path.Text.Substring(tb_Path.Text.Length - 1) != "\\")
                tb_Path.Text += "\\";

            ps = null;
            iasresult = null;
            string cmdScript = getCommand("down");
            ps = PowerShell.Create();
            ps.AddScript(cmdScript);
            ps.Streams.Error.DataAdded += Error_DataAdded;
            outputCollection.DataAdded += outputCollection_DataAdded;
            iasresult = ps.BeginInvoke<PSObject, PSObject>(null, outputCollection);
        }

        private void btn_CheckQuality_Click(object sender, RoutedEventArgs e)
        {
            ps = null;
            iasresult = null;
            string cmdScript = getCommand("check");
            ps = PowerShell.Create();
            ps.AddScript(cmdScript);
            ps.Streams.Error.DataAdded += Error_DataAdded;
            outputCollection.DataAdded += outputCollection_DataAdded;
            
            iasresult = ps.BeginInvoke<PSObject, PSObject>(null, outputCollection);
            
        }

        private string getCommand(string type)
        {
            if(type == "down")
            {
                if(cb_AV1.IsChecked== true)
                {
                    if (!String.IsNullOrEmpty(tb_Proxy.Text))
                    {
                        return ".\\youtube-dl.exe --proxy " + tb_Proxy.Text + " -f bestvideo[vcodec^=av01]+bestaudio[acodec^=opus] -o \"" + tb_Path.Text + "%(title)s-%(id)s.%(vcodec)s.%(ext)s\" " + tb_Link.Text;
                    }
                    else
                    {
                        return ".\\youtube-dl.exe -f bestvideo[vcodec^=av01]+bestaudio[acodec^=opus] -o \"" + tb_Path.Text + "%(title)s-%(id)s.%(vcodec)s.%(ext)s\" " + tb_Link.Text;
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(tb_Proxy.Text))
                    {
                        return ".\\youtube-dl.exe --proxy " + tb_Proxy.Text + " -f bestvideo[vcodec^=vp9]+bestaudio[acodec^=opus] -o \"" + tb_Path.Text + "%(title)s-%(id)s.%(vcodec)s.%(ext)s\" " + tb_Link.Text;
                    }
                    else
                    {
                        return ".\\youtube-dl.exe -f bestvideo[vcodec^=vp9]+bestaudio[acodec^=opus] -o \"" + tb_Path.Text + "%(title)s-%(id)s.%(vcodec)s.%(ext)s\" " + tb_Link.Text;
                    }
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(tb_Proxy.Text))
                {
                    return ".\\youtube-dl.exe --proxy " + tb_Proxy.Text + " -F " + tb_Link.Text;
                }
                else
                {
                    return ".\\youtube-dl.exe -F " + tb_Link.Text;
                }
            }
            
        }

        void Error_DataAdded(object sender, DataAddedEventArgs e)
        {
            tb_Out.Dispatcher.Invoke(() =>
            {
                foreach(ErrorRecord erc in ps.Streams.Error.ReadAll())
                {
                    tb_Out.Text += "\n !!!STD_ERR:  " + erc.ToString(); 
                }
            });
        }

        void outputCollection_DataAdded(object sender, DataAddedEventArgs e)
        {

            tb_Out.Dispatcher.Invoke(() =>
            {
                Collection<PSObject> curr = outputCollection.ReadAll();
                foreach(PSObject r in curr)
                {

                    tb_Out.Text += "\n" + r.ToString();
                    if (r.ToString().Contains("av01"))
                    {
                        cb_AV1.IsChecked = true;
                        tb_Out.Text += "\n" + "↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑~AV1~";
                    }

                    tb_Out.ScrollToEnd();
                }
            }
            );
        }

        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            ps.Stop();
            ps = null;
            iasresult = null;
        }

        private void btn_OpenDest_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe" , @tb_Path.Text);
        }
    }
}
