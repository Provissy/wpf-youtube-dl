using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        }

        private void btn_StartDownload_Click(object sender, RoutedEventArgs e)
        {
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
            MessageBox.Show(" -f bestvideo[vcodec^=av01]+bestaudio ");
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
                        return ".\\youtube-dl.exe --proxy " + tb_Proxy.Text + " -f bestvideo[vcodec^=av01]+bestaudio " + tb_Link.Text;
                    }
                    else
                    {
                        return ".\\youtube-dl.exe -f bestvideo[vcodec^=av01]+bestaudio " + tb_Link.Text;
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(tb_Proxy.Text))
                    {
                        return ".\\youtube-dl.exe --proxy " + tb_Proxy.Text + " -f bestvideo+bestaudio " + tb_Link.Text;
                    }
                    else
                    {
                        return ".\\youtube-dl.exe -f bestvideo+bestaudio " + tb_Link.Text;
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
            // do something when an error is written to the error stream
            tb_Out.Dispatcher.Invoke(() =>
            {
                tb_Out.Text = "ERROR!";
            });
        }

        void outputCollection_DataAdded(object sender, DataAddedEventArgs e)
        {
            // do something when an object is written to the output stream

            tb_Out.Dispatcher.Invoke(() =>
            {
                Collection<PSObject> curr = outputCollection.ReadAll();
                foreach(PSObject r in curr)
                {
                    tb_Out.Text += "\n" + r.ToString();
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
    }
}
