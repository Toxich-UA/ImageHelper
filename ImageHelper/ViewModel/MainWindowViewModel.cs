using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Image = System.Drawing.Image;


namespace ImageHelper.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {

        #region Private Member

        private MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
        private Utils _utils = new Utils();

        private string[] _fileList = null;

        private string _sourceDirectoryPath;
        private int _fileCount;
        private string _progressbar;
        private bool _startButtonEnabled = true;
        private string _destinationDirectoryPath;

        #endregion

        #region View Components
        /// <summary>
        /// Describe all components
        /// </summary>
        public string percent { get; set; } = String.Empty;
        public bool placeName { get; set; } = true;
        public string SourceDirectoryPath
        {
            get => _sourceDirectoryPath;
            set
            {
                if (_sourceDirectoryPath != value)
                {
                    _sourceDirectoryPath = value;
                }
            }
        }
        public int FileCounter
        {
            get => _fileCount;
            set
            {
                if (_fileCount != value)
                {
                    _fileCount = value;
                }
            }
        }
        public string DestDirectoryPath
        {
            get => _destinationDirectoryPath;
            set
            {
                if (_destinationDirectoryPath != value)
                {
                    _destinationDirectoryPath = value;
                }
            }
        }
        public string ProgressBar
        {
            get => _progressbar;
            set
            {
                if (_progressbar != value)
                {
                    _progressbar = value;
                }
            }
        }
        public bool StartButtonIsEnabled
        {
            get => _startButtonEnabled;
            set
            {
                if (_startButtonEnabled != value)
                {
                    _startButtonEnabled = value;
                }
            }
        }
        

        #endregion

        #region Helper Methods

        public void PickSourceDirectory()
        {
            SourceDirectoryPath = _utils.GetFolderPath();
            if (SourceDirectoryPath != null)
            {
                _fileList = _utils.GetAllFileNames(SourceDirectoryPath);
                FileCounter = _fileList.Length;
            }
        }

        public void PickDestDirectory()
        {
            DestDirectoryPath = _utils.GetFolderPath();
            if (DestDirectoryPath != null)
            {
                string[] destFile = _utils.GetAllFileNames(DestDirectoryPath);
                DestDirectoryPath = DestDirectoryPath;
                if (destFile.Length != 0)
                    MessageBox.Show(@"Destination folder has some files", @"Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        #endregion

        #region Public Commands
        /// <summary>
        /// Creating command for all usable components
        /// </summary>
        public ICommand PickSourceDirectoryCommand { get; set; }
        public ICommand PickDestinationDirectoryCommand { get; set; }
        public ICommand ShowLogCommand { get; set; }
        public ICommand StartResizeCommand { get; set; }
        public ICommand TextSettings { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializint created command
        /// </summary>
        public MainWindowViewModel()
        {
            this.PickSourceDirectoryCommand = new RelayCommand(PickSourceDirectory);
            this.PickDestinationDirectoryCommand = new RelayCommand(PickDestDirectory);
            this.StartResizeCommand = new RelayCommand(StartImageResize);
            this.TextSettings = new RelayCommand(OpenTextSettingsWindow);

        }
        #endregion

        /// <summary>
        /// Allow only numbers to input
        /// </summary>
        private void GetCompresionPersent(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;

        }

        private void StartImageResize()
        {
            ProgressBar = 0.ToString();
            int maxFileCount = FileCounter;
            bool nameOnImage = placeName;
            if (maxFileCount == 0)
            {
                MessageBox.Show(@"Source folder doesn't contain no one image", @"Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string compresPercent = percent;

            if (_fileList != null)
            {
                if (compresPercent.Equals(String.Empty))
                {
                    mainWindow.CompresionPersent.BorderBrush = System.Windows.Media.Brushes.Red;
                    return;
                }

                //log.AppendText("=====================\n");
                //log.AppendText($"Start new processing at {DateTime.Now.ToShortTimeString()}\n");
                //log.AppendText($"File to Process {maxFileCount}\n");

                Thread process = new Thread(() =>
                {
                    StartButtonIsEnabled = false;

                    Image resizedImage;
                    int progressDelta = 100 / _fileCount;
                    foreach (var image in _fileList)
                    {
                        try
                        {
                            resizedImage = _utils.ScaleByPercent(image, Convert.ToInt32(percent), placeName);
                        }
                        catch (Exception)
                        {
                            //log.AppendText($"File {fileName} is not found; \n");
                            //log.AppendText($"File {fileName} was skiped; \n");
                            Console.WriteLine("DEBUG: Exception was rized");
                            continue;
                        }
                        resizedImage.Save(Path.Combine(DestDirectoryPath, Path.GetFileName(image)), ImageFormat.Jpeg);
                        resizedImage.Dispose();
                        ProgressBar = (Convert.ToInt32(ProgressBar) + progressDelta).ToString();
                    }
                    ProgressBar = 100.ToString();
                    StartButtonIsEnabled = true;
                    MessageBox.Show(@"Work is done!", @"Congratulation", MessageBoxButton.OK, MessageBoxImage.Information);
                });

                process.Start();


                
                //log.AppendText("Work done.\n ");
                //log.AppendText("=====================\n ");
            }
            return;

        }
        public void OpenTextSettingsWindow()
        {
            //Test test = new Test();
            //test.Show();
        }
        
        





    }
}

