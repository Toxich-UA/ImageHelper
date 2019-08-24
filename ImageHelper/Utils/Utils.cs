using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using DialogResult = System.Windows.Forms.DialogResult;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using FontStyle = System.Drawing.FontStyle;
using Image = System.Drawing.Image;
using Point = System.Drawing.Point;

namespace ImageHelper.ViewModel
{
    class Utils
    {
        #region Private members



        #endregion

        public Image DrawText(string text, Image image, int size = 50)
        {
            Font font = new Font("Arial", size);
            Point point = new Point(10, 10);
            Color textColor = Color.White;

            Graphics drawing = Graphics.FromImage(image);

            Brush textBrush = new SolidBrush(textColor);

            GraphicsPath Pen = new GraphicsPath();
            Pen.AddString(text, new FontFamily("Arial"), (int)FontStyle.Regular, drawing.DpiY * size / 72, point, new StringFormat());

            drawing.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            drawing.SmoothingMode = SmoothingMode.AntiAlias;
            drawing.InterpolationMode = InterpolationMode.HighQualityBicubic;
            drawing.PixelOffsetMode = PixelOffsetMode.HighQuality;

            drawing.DrawPath(new Pen(Color.Black, 5), Pen);
            drawing.DrawString(text, font, textBrush, point);

            drawing.Flush();
            drawing.Save();
            textBrush.Dispose();
            drawing.Dispose();

            return image;

        }

        public BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public string GetFolderPath()
        {
            string directoryPath = null;
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    directoryPath = dialog.SelectedPath;
                }
            }
            return directoryPath;
        }

        public string[] GetAllFileNames(string filePath)
        {
            return Directory.GetFiles(filePath, "*.jpg");
        }

        public Image ScaleByPercent(string file, int Percent, bool placeName)
        {
            Image imgPhoto = Image.FromFile(file);

            float nPercent = ((float)Percent / 100);
            int destWidth = (int)(imgPhoto.Width * nPercent);
            int destHeight = (int)(imgPhoto.Height * nPercent);

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution * nPercent, imgPhoto.VerticalResolution * nPercent);

            using (Graphics grPhoto = Graphics.FromImage(bmPhoto))
            {
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                grPhoto.DrawImage(imgPhoto, 0, 0);

                if (placeName)
                {
                    DrawText(Path.GetFileNameWithoutExtension(file), bmPhoto);
                }
            }

            return bmPhoto;
        }
    }
}
