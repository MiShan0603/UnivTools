﻿using PdfiumViewer;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace UnivTools.UI.UserControls
{
    /// <summary>
    /// UserControl_Pdf.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl_Pdf : System.Windows.Controls.UserControl, UserControlInterfaces
    {
        private System.Windows.Window _window;
        private String mPdfPath;
        public UserControl_Pdf(System.Windows.Window ownerWindow)
        {
            InitializeComponent();

            _window = ownerWindow;

            pdfViewer.Renderer.DisplayRectangleChanged += Renderer_DisplayRectangleChanged;
            pdfViewer.Renderer.ZoomChanged += Renderer_ZoomChanged;

            pdfViewer.Renderer.MouseMove += Renderer_MouseMove;
            pdfViewer.Renderer.MouseLeave += Renderer_MouseLeave;
            ShowPdfLocation(PdfPoint.Empty);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            //if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            //{
            //    e.Effects = System.Windows.DragDropEffects.Link;
            //    System.Windows.MessageBox.Show(((System.Array)e.Data.GetData(System.Windows.DataFormats.FileDrop)).GetValue(0).ToString());
            //}
            //else
            //{
            //    e.Effects = System.Windows.DragDropEffects.None;
            //}
        }

        private void UserControl_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            bool dropEnabled = true;
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop, true))
            {
                string[] filenames = e.Data.GetData(System.Windows.DataFormats.FileDrop, true) as string[];

                foreach (string filename in filenames)
                {
                    if (System.IO.Path.GetExtension(filename).ToUpperInvariant() != ".PDF")
                    {
                        dropEnabled = false;
                        break;
                    }
                }
            }
            else
            {
                dropEnabled = false;
            }

            if (!dropEnabled)
            {
                e.Effects = System.Windows.DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void UserControl_Drop(object sender, System.Windows.DragEventArgs e)
        {
            string[] droppedFilenames = e.Data.GetData(System.Windows.DataFormats.FileDrop, true) as string[];

            // System.Windows.MessageBox.Show(droppedFilenames.ToString());
            if (droppedFilenames.Count() > 0)
            {
                OpenPdf(droppedFilenames[0]);
            }
        }

        


        #region ToolBar

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*";
                dialog.Title = "Open PDF File";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    OpenPdf(dialog.FileName);
                    
                }
            }
        }

        private void txtCurPage_TextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Utils.IsDigit(e.Text);
        }

        private void txtCurPage_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                int page;
                if (int.TryParse(txtCurPage.Text, out page))
                    pdfViewer.Renderer.Page = page - 1;
            }
        }

        private void btnPre_Click(object sender, RoutedEventArgs e)
        {
            pdfViewer.Renderer.Page--;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            pdfViewer.Renderer.Page++;
        }

        private void txtCurZoom_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                float zoom;
                if (float.TryParse(txtCurZoom.Text, out zoom))
                    pdfViewer.Renderer.Zoom = zoom;
            }
        }

        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            pdfViewer.Renderer.ZoomIn();
        }

        private void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            pdfViewer.Renderer.ZoomOut();
        }

        private void btnFitWidth_Click(object sender, RoutedEventArgs e)
        {
            FitPage(PdfViewerZoomMode.FitWidth);
        }

        private void btnFitHeight_Click(object sender, RoutedEventArgs e)
        {
            FitPage(PdfViewerZoomMode.FitHeight);
        }

        private void btnFitBest_Click(object sender, RoutedEventArgs e)
        {
            FitPage(PdfViewerZoomMode.FitBest);
        }

        private void FitPage(PdfViewerZoomMode zoomMode)
        {
            int page = pdfViewer.Renderer.Page;
            pdfViewer.ZoomMode = zoomMode;
            pdfViewer.Renderer.Zoom = 1;
            pdfViewer.Renderer.Page = page;
        }

        private void btnRotateLeft_Click(object sender, RoutedEventArgs e)
        {
            pdfViewer.Renderer.RotateLeft();
        }

        private void btnRotateRight_Click(object sender, RoutedEventArgs e)
        {
            pdfViewer.Renderer.RotateRight();
        }

        private void btnGetText_Click(object sender, RoutedEventArgs e)
        {
            int page = pdfViewer.Renderer.Page;
            string text = pdfViewer.Document.GetPdfText(page);
            string caption = string.Format("Page {0} contains {1} character(s):", page + 1, text.Length);

            if (text.Length > 128) text = text.Substring(0, 125) + "...\n\n\n\n..." + text.Substring(text.Length - 125);
            System.Windows.MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnToImage_Click(object sender, RoutedEventArgs e)
        {
            Pdf2ImageInfo info = new Pdf2ImageInfo();
            info.CurPage = pdfViewer.Renderer.Page + 1;
            info.FromPage = 1;
            info.ToPage = pdfViewer.Document.PageCount;
            info.SaveMode = Pdf2ImageSaveMode.Pdf2Image_CurPage;

            System.IO.FileInfo fi = new System.IO.FileInfo(mPdfPath);
            info.ImgDir = fi.DirectoryName;

            PDF2Image pDF2Image = new PDF2Image();
            pDF2Image.Owner = _window;
            pDF2Image.SetPdf2ImageInfo(info);
            if (pDF2Image.ShowDialog() == true)
            {
                info = pDF2Image.GetPdf2ImageInfo();
                if (info.ImgDir.LastIndexOf('\\') != info.ImgDir.Length - 1)
                {
                    info.ImgDir += "\\";
                }

                if (info.FromPage <= 0)
                {
                    info.FromPage = 1;
                }

                if (info.ToPage <= 0 || info.ToPage > pdfViewer.Document.PageCount)
                {
                    info.ToPage = pdfViewer.Document.PageCount;
                }

                if (info.CurPage <= 0)
                {
                    info.CurPage = 1;
                }

                if (info.CurPage <= 0 || info.CurPage > pdfViewer.Document.PageCount)
                {
                    info.CurPage = pdfViewer.Renderer.Page;
                }

                switch (info.SaveMode) 
                {
                    case Pdf2ImageSaveMode.Pdf2Image_CurPage:
                        info.FromPage = info.CurPage;
                        info.ToPage = info.CurPage;
                        break;
                    case Pdf2ImageSaveMode.Pdf2Image_Pages:
                        break;
                    case Pdf2ImageSaveMode.Pdf2Image_AllPages:
                        info.FromPage = 1;
                        info.ToPage = pdfViewer.Document.PageCount;
                        break;
                    default:
                        return;
                        break;
                }

                string imageFormat = ".jpg";
                for (int i = info.FromPage; i <= info.ToPage; i++)
                {
                    int dpi = 300;
                    using (var image = pdfViewer.Document.Render(i, dpi, dpi, PdfRenderFlags.CorrectFromDpi))
                    {
                        var encoder = ImageCodecInfo.GetImageEncoders()
                            .First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                        var encParams = new EncoderParameters(1);
                        encParams.Param[0] = new EncoderParameter(
                            System.Drawing.Imaging.Encoder.Quality, 100L);

                        string imgFile = $"{info.ImgDir}{fi.Name}-{i}{imageFormat}";
                        image.Save(imgFile, encoder, encParams);
                    }
                }
            }
        }

#endregion ToolBar


        public void HideControls(bool hide)
        {
            pdfForm.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
        }

        

        #region PDF Event
        private void Renderer_MouseLeave(object sender, EventArgs e)
        {
            ShowPdfLocation(PdfPoint.Empty);
        }

        private void Renderer_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Drawing.Point pt = new System.Drawing.Point();
            pt.X = e.X;
            pt.Y = e.Y;
            ShowPdfLocation(pdfViewer.Renderer.PointToPdf(pt));
        }

        private void Renderer_ZoomChanged(object sender, EventArgs e)
        {
            txtCurZoom.Text = pdfViewer.Renderer.Zoom.ToString();
        }

        private void Renderer_DisplayRectangleChanged(object sender, EventArgs e)
        {
            txtCurPage.Text = (pdfViewer.Renderer.Page + 1).ToString();
        }

        private void ShowPdfLocation(PdfPoint point)
        {
        }




        #endregion PDF Event


        private void OpenPdf(string pdfPath)
        {
            try
            {
                pdfViewer.Document = PdfDocument.Load(pdfPath);
                mPdfPath = pdfPath;
                txtPdfBG.Text = mPdfPath;
                txtTotlePages.Text = @"/" + pdfViewer.Document.PageCount.ToString();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "ERROR",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
}
