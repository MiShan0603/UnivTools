using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace UnivTools
{
    internal class LvItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 当前的图标信息
        /// </summary>
        public ImageSource img { get; set; }

        /// <summary>
        /// 当前名字
        /// </summary>
        public String name { get; set; }

        /// <summary>
        /// 对应的自定义控件
        /// </summary>
        public UserControl panel { get; set; }
    }
}
