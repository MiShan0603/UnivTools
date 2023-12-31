﻿using PdfiumViewer;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UnivTools.UI
{
    public enum Pdf2ImageSaveMode
    {
        /// <summary>
        /// 当前页面
        /// </summary>
        Pdf2Image_CurPage, 

        /// <summary>
        /// 范围保存
        /// </summary>
        Pdf2Image_Pages, 

        /// <summary>
        /// 全部保存
        /// </summary>
        Pdf2Image_AllPages
    }

    public class Pdf2ImageInfo 
    {
        public Pdf2ImageSaveMode SaveMode { get; set; }
        public int TotlePages { get; set; }
        public int CurPage { get; set; }
        public String ImgDir { get; set; }

        /// <summary>
        /// 范围保存，开始页
        /// </summary>
        public int FromPage { get; set; }

        /// <summary>
        /// 范围保存，结束页
        /// </summary>
        public int ToPage { get; set; }
    }

    /// <summary>
    /// PDF2Image.xaml 的交互逻辑
    /// </summary>
    public partial class PDF2Image : Window
    {
        private Pdf2ImageInfo mPdf2ImageInfo = null;
        public PDF2Image()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hWnd = new WindowInteropHelper(this).Handle;
            var style = Win32.User32.GetWindowLong(hWnd, Win32.User32.GWL_EXSTYLE);
            Win32.User32.SetWindowLong(hWnd, Win32.User32.GWL_EXSTYLE, style | UnivTools.Wrapper.Win32.WS_EX_NOACTIVATE);
            Win32.User32.SetWindowPos(hWnd, UnivTools.Wrapper.Win32.HWND_TOPMOST, 0, 0, 0, 0, Win32.User32.SWP_NOSIZE | Win32.User32.SWP_NOMOVE | Win32.User32.SWP_NOACTIVATE);

            if (mPdf2ImageInfo == null)
            {
                mPdf2ImageInfo = new Pdf2ImageInfo();
                mPdf2ImageInfo.FromPage = 0;
                mPdf2ImageInfo.ToPage = 0;
                mPdf2ImageInfo.CurPage = 0;
                mPdf2ImageInfo.TotlePages = 0;
                mPdf2ImageInfo.SaveMode = Pdf2ImageSaveMode.Pdf2Image_AllPages;
            }

            txtCurPage.Text = mPdf2ImageInfo.CurPage.ToString();
            txtPagesFrom.Text = mPdf2ImageInfo.FromPage.ToString(); 
            txtPagesTo.Text = mPdf2ImageInfo.ToPage.ToString();
            txtImageDir.Text = mPdf2ImageInfo.ImgDir;

            switch (mPdf2ImageInfo.SaveMode) 
            {
                case Pdf2ImageSaveMode.Pdf2Image_CurPage:
                    btnSelectCurPage.IsChecked = true;
                    btnSelectPages.IsChecked = false;
                    btnSelectAll.IsChecked = false;
                    break;
                case Pdf2ImageSaveMode.Pdf2Image_Pages:
                    btnSelectCurPage.IsChecked = false;
                    btnSelectPages.IsChecked = true;
                    btnSelectAll.IsChecked = false;
                    break;
                case Pdf2ImageSaveMode.Pdf2Image_AllPages:
                    btnSelectCurPage.IsChecked = false;
                    btnSelectPages.IsChecked = false;
                    btnSelectAll.IsChecked = true;
                    break;
            }
        }

        public void SetPdf2ImageInfo(Pdf2ImageInfo info)
        {
            mPdf2ImageInfo = info;

            try
            {
                txtCurPage.Text = mPdf2ImageInfo.CurPage.ToString();
                txtPagesFrom.Text = mPdf2ImageInfo.FromPage.ToString();
                txtPagesTo.Text = mPdf2ImageInfo.ToPage.ToString();
                txtImageDir.Text = mPdf2ImageInfo.ImgDir;

                switch (mPdf2ImageInfo.SaveMode)
                {
                    case Pdf2ImageSaveMode.Pdf2Image_CurPage:
                        btnSelectCurPage.IsChecked = true;
                        btnSelectPages.IsChecked = false;
                        btnSelectAll.IsChecked = false;
                        break;
                    case Pdf2ImageSaveMode.Pdf2Image_Pages:
                        btnSelectCurPage.IsChecked = false;
                        btnSelectPages.IsChecked = true;
                        btnSelectAll.IsChecked = false;
                        break;
                    case Pdf2ImageSaveMode.Pdf2Image_AllPages:
                        btnSelectCurPage.IsChecked = false;
                        btnSelectPages.IsChecked = false;
                        btnSelectAll.IsChecked = true;
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public Pdf2ImageInfo GetPdf2ImageInfo()
        {
            try
            {
                mPdf2ImageInfo.CurPage = int.Parse(txtCurPage.ToString().Trim());
                mPdf2ImageInfo.FromPage = int.Parse(txtPagesFrom.ToString().Trim());
                mPdf2ImageInfo.ToPage = int.Parse(txtPagesTo.ToString().Trim());
            }
            catch (Exception ex)
            {

            }

            return mPdf2ImageInfo;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }



        private void txtCurPage_TextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Utils.IsDigit(e.Text);
        }

        private void txtPagesFrom_TextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Utils.IsDigit(e.Text);
        }

        private void txtPagesTo_TextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Utils.IsDigit(e.Text);
        }

        private void btnSelectCurPage_Click(object sender, RoutedEventArgs e)
        {
            mPdf2ImageInfo.SaveMode = Pdf2ImageSaveMode.Pdf2Image_CurPage;
        }

        private void btnSelectPages_Click(object sender, RoutedEventArgs e)
        {
            mPdf2ImageInfo.SaveMode = Pdf2ImageSaveMode.Pdf2Image_Pages;
        }

        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            mPdf2ImageInfo.SaveMode = Pdf2ImageSaveMode.Pdf2Image_AllPages;
        }

        private void btnImageDir_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = mPdf2ImageInfo.ImgDir;    //设置初始目录
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
            {
                mPdf2ImageInfo.ImgDir = folderBrowserDialog.SelectedPath;
                txtImageDir.Text = folderBrowserDialog.SelectedPath;
                txtImageDir.Focus();
            }
            
        }
    }
}
