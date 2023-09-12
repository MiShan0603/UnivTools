using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnivTools.UI.UserControls;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace UnivTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        // 左边导航栏
        private List<LvItem> _lvItems = new List<LvItem>();

        private IntPtr _selfHwnd  = IntPtr.Zero;
        private HwndSource _windowSource = null;
        private IntPtr _clipboardViewerNext = IntPtr.Zero;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _selfHwnd = new WindowInteropHelper(this).Handle;
            _windowSource = HwndSource.FromHwnd(_selfHwnd);

            var style = Win32.User32.GetWindowLong(_selfHwnd, Win32.User32.GWL_EXSTYLE);
            Win32.User32.SetWindowLong(_selfHwnd, Win32.User32.GWL_EXSTYLE, style | UnivTools.Wrapper.Win32.WS_EX_NOACTIVATE);
            Win32.User32.SetWindowPos(_selfHwnd, UnivTools.Wrapper.Win32.HWND_TOPMOST, 0, 0, 0, 0, Win32.User32.SWP_NOSIZE | Win32.User32.SWP_NOMOVE | Win32.User32.SWP_NOACTIVATE);

            _clipboardViewerNext = (IntPtr)Win32.User32.SetClipboardViewer(_selfHwnd);
            _windowSource.AddHook(WndProc);


            InitListView();
        }

        private void lvPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gridContent.Children.Clear();

            if (lvPanel.SelectedItem == null)
                return;

            var item = lvPanel.SelectedItem as LvItem;
            if (item != null)
            {
                gridContent.Children.Clear();
                gridContent.Children.Add(item.panel);
            }
        }

        private void InitListView()
        {
            lvPanel.Items.Clear();

            // pdf 
            {
                LvItem lvItemPdf = new LvItem();
                lvItemPdf.img = new BitmapImage(new Uri("pack://application:,,,/Assets/image/pdf.png"));
                lvItemPdf.name = "PDF文档";
                lvItemPdf.panel = new UserControl_Pdf(this);

                (lvItemPdf.panel as UserControlInterfaces).HideControls(false);

                lvPanel.Items.Add(lvItemPdf);
            }

            // clipboard 
            {
                LvItem lvItemClipboard = new LvItem();
                lvItemClipboard.img = new BitmapImage(new Uri("pack://application:,,,/Assets/image/clipboard.png"));
                lvItemClipboard.name = "黏贴板";
                lvItemClipboard.panel = new UserControl_Clipboard();

                (lvItemClipboard.panel as UserControlInterfaces).HideControls(false);

                lvPanel.Items.Add(lvItemClipboard);
            }


            // setting
            {
                LvItem lvItemSetting = new LvItem();
                lvItemSetting.img = new BitmapImage(new Uri("pack://application:,,,/Assets/image/setting.png"));
                lvItemSetting.name = "设置";
                lvItemSetting.panel = new UserControl_ABout();

                (lvItemSetting.panel as UserControlInterfaces).HideControls(false);

                lvPanel.Items.Add(lvItemSetting);
            }

            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Win32.User32.ChangeClipboardChain(_selfHwnd, _clipboardViewerNext);
        }

        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) 
        {
            switch (msg)
            {
                case Win32.User32.WM_DRAWCLIPBOARD:
                    {

                    }
                    break;
                default:
                    break;
            }

            return IntPtr.Zero;
        }
    }
}
