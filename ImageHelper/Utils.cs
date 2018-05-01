using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Image = System.Drawing.Image;
using RichTextBox = System.Windows.Controls.RichTextBox;

namespace ImageHelper
{
    class Utils
    {
        private RichTextBox Log;
        public Utils(RichTextBox logBox)
        {
            Log = logBox;
        }

        public int fontSize = 4;

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
            Log.AppendText("Openning image " + file + "\n");

            Image imgPhoto = Image.FromFile(file);

            Log.AppendText("Image are opened.\n");

            float nPercent = ((float)Percent / 100);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;

            int destX = 0;
            int destY = 0;
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight,
                                     PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                                    imgPhoto.VerticalResolution);

            using (Graphics grPhoto = Graphics.FromImage(bmPhoto))
            {
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(imgPhoto,
                    new Rectangle(destX, destY, destWidth, destHeight),
                    new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                    GraphicsUnit.Pixel);

                if (placeName)
                {
                    PlaceNameOnImage(grPhoto, file, destWidth, destHeight, nPercent);
                }
            }
            Log.AppendText("Image processed.\n");


            return bmPhoto;
        }

        public Graphics PlaceNameOnImage(Graphics grPhoto, string file, int destWidth, int destHeight, float percent)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            
            using (Font arialFont = new Font("Arial", (destWidth / fontSize) / fileName.Length, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                grPhoto.DrawString(fileName, arialFont, Brushes.Black, new PointF(5f, 5f));
                
            }
            return grPhoto;
        }

    }
}
