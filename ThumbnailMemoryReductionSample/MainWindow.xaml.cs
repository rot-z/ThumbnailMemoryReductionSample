using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ThumbnailMemoryReductionSample
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ボタンのクリックイベントハンドラ
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // フォルダを選択
            var fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowNewFolderButton = false;
            if (fbd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            // ラジオボタンの選択に応じて、ViewModelを生成する
            var type = ImageViewModel.ScaleType.TransformedBitmap;
            if (radWrapWithWritableBitmap.IsChecked == true)
            {
                type = ImageViewModel.ScaleType.WrappedWithWritableBitmap;
            }

            // 指定されたフォルダ内のファイル一覧を取得
            string[] files = System.IO.Directory.GetFiles(fbd.SelectedPath);

            // フォルダ内のファイルでViewModelを生成
            List<ImageViewModel> lst = new List<ImageViewModel>();
            foreach (string f in files)
            {
                lst.Add(new ImageViewModel(f, type));
            }
            lstThumbnails.ItemsSource = lst;
        }
    }
}
