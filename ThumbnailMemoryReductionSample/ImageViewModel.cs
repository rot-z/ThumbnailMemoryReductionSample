using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ThumbnailMemoryReductionSample
{
    /// <summary>
    /// イメージをプロパティに抱えるBindableなViewModel
    /// </summary>
    public class ImageViewModel : INotifyPropertyChanged
    {
        #region プロパティ変更通知イベント関連

        /// <summary> 
        /// プロパティの変更通知を行うイベント
        /// </summary> 
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary> 
        /// プロパティ変更通知を行う
        /// </summary> 
        /// <param name="propertyName">プロパティ名</param> 
        public virtual void RaisePropertyChanged(string propertyName)
        {
            var h = this.PropertyChanged;
            if (h != null)
            {
                h(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        /// <summary>
        /// サムネイルの生成方法
        /// </summary>
        public enum ScaleType
        {
            TransformedBitmap,
            WrappedWithWritableBitmap,
        }

        /// <summary>生成するサムネイルの横幅</summary>
        private const int THUMBNAIL_WIDTH = 30;
        /// <summary>生成するサムネイルの高さ</summary>
        private const int THUMBNAIL_HEIGHT = 30;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="filePath">サムネイル生成する元画像</param>
        /// <param name="type">サムネイルの生成方法</param>
        public ImageViewModel(string filePath, ScaleType type)
        {
            // サムネイル生成
            Thumbnail = CreateImage(filePath, THUMBNAIL_WIDTH, THUMBNAIL_HEIGHT, type);
            FilePath = filePath;
            FileName = System.IO.Path.GetFileName(filePath);
        }

        /// <summary>
        /// サムネイルとして表示するBitmapSource
        /// </summary>
        public BitmapSource Thumbnail { get; set; }

        /// <summary>
        /// 表示する画像のファイル名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 表示する画像のファイルパス
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 指定されたファイルパスの画像を読み込んで、サムネイルを作成する
        /// </summary>
        /// <param name="filePath">画像ファイルのパス</param>
        /// <param name="scaledWidth">拡大/縮小後の幅（Pixel単位）</param>
        /// <param name="scaledHeight">拡大/縮小後の高さ（Pixel単位）</param>
        /// <param name="type">サムネイルの生成方法</param>
        /// <returns>生成したBitmapSource</returns>
        private BitmapSource CreateImage(string filePath, int scaledWidth, int scaledHeight, ScaleType type)
        {
            try
            {
                using (System.IO.Stream stream = new System.IO.FileStream(
                    filePath,
                    System.IO.FileMode.Open,
                    System.IO.FileAccess.Read,
                    System.IO.FileShare.ReadWrite | System.IO.FileShare.Delete
                ))
                {
                    // 画像をデコード
                    BitmapDecoder decoder = BitmapDecoder.Create(
                        stream,
                        BitmapCreateOptions.PreservePixelFormat,
                        BitmapCacheOption.OnLoad
                    );

                    BitmapSource bmp = null;
                    if ((scaledWidth > 0) && (scaledHeight > 0))
                    {
                        // 拡大/縮小したイメージを生成する
                        double scaleX = (double)scaledWidth / decoder.Frames[0].PixelWidth;
                        double scaleY = (double)scaledHeight / decoder.Frames[0].PixelHeight;
                        double scale = Math.Min(scaleX, scaleY);

                        if (type == ScaleType.TransformedBitmap)
                        {
                            // TransformedBitmapをそのまま保持する
                            bmp = new TransformedBitmap(decoder.Frames[0], new ScaleTransform(scale, scale));
                        }
                        else
                        {
                            // 生成したTransformedBitmapから再度WritableBitmapを生成する
                            bmp = new WriteableBitmap(new TransformedBitmap(decoder.Frames[0], new ScaleTransform(scale, scale)));
                        }
                    }
                    else
                    {
                        // 原寸でイメージを生成する
                        bmp = new WriteableBitmap(decoder.Frames[0]);
                    }
                    bmp.Freeze();

                    return bmp;
                }
            }
            catch (Exception exc)
            {
                System.Diagnostics.StackFrame sf = new System.Diagnostics.StackFrame(1);
                System.Diagnostics.Debug.WriteLine(string.Format("[{0}]エラー：{1}", sf.GetMethod(), exc));
            }

            return null;
        }
    }
}
