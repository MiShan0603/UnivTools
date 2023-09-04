using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnivTools.UI.UserControls;
using static System.Net.Mime.MediaTypeNames;

namespace UnivTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<LvItem> lvItems = new List<LvItem>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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

    }
}
