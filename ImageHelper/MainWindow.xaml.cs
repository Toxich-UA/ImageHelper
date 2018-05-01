using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.IO.Compression;
using System.IO.Packaging;
using MessageBox = System.Windows.MessageBox;


namespace ImageHelper
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			utils = new Utils(Log);
		}

		private Utils utils;
	    private string[] _fileList = null;
	    private string _filePath = "";
	    private string _destDirPath = "";
	    public bool _isLogOnened = false;

		private void BtnPickDirectory_OnClick(object sender, RoutedEventArgs e)
		{
			
			_filePath = utils.GetFolderPath();
            if (_filePath != null)
            {
                _fileList = utils.GetAllFileNames(_filePath);
                DirectoryPath.Text = _filePath;
                FileCounter.Content = _fileList.Length;
            }
			
		}

       

        private void CompresionPersent_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (!char.IsDigit(e.Text, e.Text.Length - 1))
				e.Handled = true;
			
		}

		private void BtnStartImageResize_Click(object sender, RoutedEventArgs e)
		{
			
			int maxFileCount = (int) FileCounter.Content;
			if (maxFileCount == 0)
			{
				MessageBox.Show(@"Destination folder doesn't contain no one image", @"Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

		    int fileCounter = 0;
			ProgressBar.Value = fileCounter;
			ProgressBar.Maximum = maxFileCount;



			if (_fileList != null)
			{
				Log.AppendText("=====================\n");
				Log.AppendText("Start new processing.\n");
				Log.AppendText("File to Process "+ maxFileCount+"\n");
				foreach (string file in _fileList)
				{
					
					string fileName = file.Substring(file.LastIndexOf("\\", StringComparison.Ordinal));
					
					if (CompresionPersent.Text.Equals(String.Empty))
					{
						CompresionPersent.BorderBrush = System.Windows.Media.Brushes.Red;
						return;
					}

				    Image rezImgPhoto;
				    try
					{
						rezImgPhoto = utils.ScaleByPercent(file, Int32.Parse(CompresionPersent.Text), CheNameOnImages.IsChecked.Value);
					}
					catch (FileNotFoundException)
					{
						Log.AppendText("File "+fileName+" is not found; \n");
						Log.AppendText("File "+fileName+" are skiped; \n");
						fileCounter += 1;
						continue;
					}
					
					rezImgPhoto.Save(_destDirPath + fileName, ImageFormat.Jpeg);
					Log.AppendText("Image "+ _destDirPath + fileName + " saved\n");
					rezImgPhoto.Dispose();
					fileCounter += 1;
					ProgressBar.Value = fileCounter;

				}
				if (ChkMakeArchive.IsChecked.Value )
					if (!ArchiveName.Text.Equals(String.Empty))
					{
						string archiveName = ArchiveName.Text;
						string archivePath = (archiveName.EndsWith(".zip") ? archiveName : archiveName + ".zip");
						
						ZipFile.CreateFromDirectory(_destDirPath,  archivePath);
						
						File.Move(archivePath, _destDirPath+archivePath);
						MessageBox.Show(@"Archive created!", @"Congratulation", MessageBoxButton.OK, MessageBoxImage.Information);
					}
				MessageBox.Show(@"Work is done!", @"Congratulation", MessageBoxButton.OK, MessageBoxImage.Information);
				Log.AppendText("Work done.\n ");
				Log.AppendText("=====================\n ");

			}
		}

		private void BtnPickDestDirectory_OnClick(object sender, RoutedEventArgs e)
		{
			_destDirPath = utils.GetFolderPath();
            if (_destDirPath != null)
            {
                string[] destFile = utils.GetAllFileNames(_destDirPath);
                DestDirectoryPath.Text = _destDirPath;
                if (destFile.Length != 0)
                    MessageBox.Show(@"Destination folder has some files", @"Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
		}

		private void CompresionPersent_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			CompresionPersent.BorderBrush = System.Windows.Media.Brushes.Gray;
		}

		private void BtnOpenLog_Click(object sender, RoutedEventArgs e)
		{
			//MAX = 550 MIN = 368
			if (_isLogOnened)
			{
				this.Height = 368;
				_isLogOnened = !_isLogOnened;
			}
			else
			{
				this.Height = 550;
				_isLogOnened = !_isLogOnened;
			}
		}

		
	}
}
