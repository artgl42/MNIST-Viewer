using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using MNIST_Viewer.Model;
using MNIST_Viewer.Toolbox;

namespace MNIST_Viewer.ViewModel
{
    public class MNISTViewModel : NotifyPropertyChanged
    {
        public MNISTViewModel()
        {
            OpenImagesFileCommand = new DelegateCommand(param => ExecuteOpenImagesFileCommand());
            OpenLabelsFileCommand = new DelegateCommand(param => ExecuteOpenLabelsFileCommand());
            ResetSettingCommand = new DelegateCommand(param => ExecuteResetSettingCommand());
            LoadFilesCommand = new DelegateCommand(param => ExecuteLoadFilesCommand(), param => CanExecuteLoadFilesCommand());
            NextPageCommand = new DelegateCommand(param => ExecuteNextPageCommand(param), param => CanExecuteNextPageCommand());
            PrevPageCommand = new DelegateCommand(param => ExecutePrevPageCommand(param), param => CanExecutePrevPageCommand());
            FilteredCommand = new DelegateCommand(param => ExecuteFilteredCommand(param));
            ShowItemCommand = new DelegateCommand(param => ExecuteShowItemCommand());

            ImagesFile = "train-images.idx3-ubyte";
            LabelsFile = "train-labels.idx1-ubyte";
            StartImageForLoad = 0;
            EndImageForLoad = 1000;
            PageSize = 100;
            ImageCountForLoad = (ushort)(EndImageForLoad - StartImageForLoad);
            _MNISTLoader = new MNISTLoader();
            _imagePages = new Dictionary<int, List<ImageData>>();
            NumberCounter = new ObservableCollection<int>(Enumerable.Repeat(0, 10).ToList());
            VisibleCollection = new List<ImageData>();
            _imageFilter = new bool[10];

            PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                switch (e.PropertyName)
                {
                    case "StartImageForLoad":
                        ImageCountForLoad = (ushort)(EndImageForLoad - StartImageForLoad);
                        break;
                    case "EndImageForLoad":
                        ImageCountForLoad = (ushort)(EndImageForLoad - StartImageForLoad);
                        break;
                    case "CurrentPage":
                        UpdateVisibleCollection();
                        break;
                    case "VisibleCollection":
                        ScrollViewerReset = true;
                        break;
                }
            };
        }

        private IMNISTLoader _MNISTLoader;
        private Dictionary<int, List<ImageData>> _imagePages;
        private bool[] _imageFilter;

        public DelegateCommand OpenImagesFileCommand { get; private set; }
        public DelegateCommand OpenLabelsFileCommand { get; private set; }
        public DelegateCommand LoadFilesCommand { get; private set; }
        public DelegateCommand NextPageCommand { get; private set; }
        public DelegateCommand PrevPageCommand { get; private set; }
        public DelegateCommand ResetSettingCommand { get; private set; }
        public DelegateCommand FilteredCommand { get; private set; }
        public DelegateCommand ShowItemCommand { get; private set; }

        private string _imagesFile;
        public string ImagesFile
        {
            get { return _imagesFile; }
            set
            {
                _imagesFile = value;
                OnPropertyChanged();
            }
        }

        private string _labelsFile;
        public string LabelsFile
        {
            get { return _labelsFile; }
            set
            {
                _labelsFile = value;
                OnPropertyChanged();
            }
        }

        private ushort _startImageForLoad;
        public ushort StartImageForLoad
        {
            get { return _startImageForLoad; }
            set
            {
                _startImageForLoad = value;
                OnPropertyChanged();
            }
        }

        private ushort _endImageForLoad;
        public ushort EndImageForLoad
        {
            get { return _endImageForLoad; }
            set
            {
                _endImageForLoad = value;
                OnPropertyChanged();
            }
        }

        private ushort _imageCountForLoad;
        public ushort ImageCountForLoad
        {
            get { return _imageCountForLoad; }
            set
            {
                _imageCountForLoad = value;
                OnPropertyChanged();
            }
        }

        private ushort _pageSize;
        public ushort PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                OnPropertyChanged();
            }
        }

        private ushort _currentPage;
        public ushort CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        private List<int> _pageNumbers;
        public List<int> PageNumbers
        {
            get { return _pageNumbers; }
            set
            {
                _pageNumbers = value;
                OnPropertyChanged();
            }
        }

        private List<ImageData> _visibleCollection;
        public List<ImageData> VisibleCollection
        {
            get { return _visibleCollection; }
            set
            {
                _visibleCollection = value;
                OnPropertyChanged();
            }
        }

        private ImageData _currentImage;
        public ImageData CurrentImage
        {
            get { return _currentImage; }
            set
            {
                _currentImage = value;
                OnPropertyChanged();
            }
        }

        private bool _scrollViewerReset;
        public bool ScrollViewerReset
        {
            get { return _scrollViewerReset; }
            set
            {
                _scrollViewerReset = value;
                OnPropertyChanged();
            }
        }

        private bool _openFlyout;
        public bool OpenFlyout
        {
            get { return _openFlyout; }
            set
            {
                _openFlyout = value;
                OnPropertyChanged();
            }
        }

        private string _stringPixels;
        public string StringPixels
        {
            get { return _stringPixels; }
            set
            {
                _stringPixels = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<int> NumberCounter { get; set; }

        private void LoadData()
        {
            _imagePages = new Dictionary<int, List<ImageData>>();
            var _pagesCount = (ushort)(ImageCountForLoad / PageSize + (ImageCountForLoad % PageSize == 0 ? 0 : 1));
            for (int key = 1; key <= _pagesCount; key++)
            {
                ushort startLoad = (ushort)(StartImageForLoad + PageSize * (key - 1));
                ushort endLoad = (ushort)(startLoad + PageSize <= EndImageForLoad ? startLoad + PageSize : EndImageForLoad);
                var images = _MNISTLoader.LoadImages(ImagesFile, LabelsFile, startLoad, endLoad).ToList();
                _imagePages.Add(key, images);
            }
            PageNumbers = _imagePages.Keys.ToList();
            CurrentPage = 0;
        }

        private void UpdateVisibleCollection()
        {
            VisibleCollection = !_imageFilter.Any(f => f) ? 
                _imagePages[CurrentPage + 1].ToList() : 
                _imagePages[CurrentPage + 1].Where(image => !_imageFilter[image.Label]).ToList();

            for (int i = 0; i < NumberCounter.Count; i++) NumberCounter[i] = 0;
            foreach (var image in VisibleCollection) NumberCounter[image.Label]++;
        }

        public void ExecuteOpenImagesFileCommand()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Data\\",
                FileName = "train-images.idx3-ubyte",
                Filter = "train-images.idx3-ubyte | *.idx3-ubyte"
            };
            if (openFileDialog.ShowDialog() == true) ImagesFile = openFileDialog.FileName;
        }

        public void ExecuteOpenLabelsFileCommand()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Data\\",
                FileName = "train-labels.idx1-ubyte",
                Filter = "train-labels.idx1-ubyte | *.idx1-ubyte"
            };
            if (openFileDialog.ShowDialog() == true) LabelsFile = openFileDialog.FileName;
        }

        public bool CanExecuteLoadFilesCommand()
        {
            return File.Exists(ImagesFile) && File.Exists(LabelsFile) ? true : false;
        }

        public void ExecuteLoadFilesCommand()
        {
            LoadData();
            UpdateVisibleCollection();
        }

        public void ExecuteResetSettingCommand()
        {
            StartImageForLoad = 0;
            EndImageForLoad = 1000;
            PageSize = 100;
        }

        public bool CanExecuteNextPageCommand()
        {
            return _imagePages.Count > 0 && CurrentPage + 1 < ImageCountForLoad / PageSize ? true : false;
        }

        public void ExecuteNextPageCommand(object param)
        {
            CurrentPage++;
        }

        public bool CanExecutePrevPageCommand()
        {
            return _imagePages.Count > 0 && CurrentPage > 0 ? true : false;
        }

        public void ExecutePrevPageCommand(object param)
        {
            CurrentPage--;
        }

        public void ExecuteFilteredCommand(object param)
        {
            var index = int.Parse(param.ToString());
            _imageFilter[index] = _imageFilter[index] ? false : true;
            UpdateVisibleCollection();
        }

        public void ExecuteShowItemCommand()
        {
            string _stringPixels = null;
            for (int i = 0; i < CurrentImage.Pixels.Length; i++)
            {
                _stringPixels += CurrentImage.Pixels[i].ToString("X2") + ((i + 1) % 28 != 0 ? " " : Environment.NewLine);  
            }

            StringPixels = _stringPixels;
            OpenFlyout = true;
        }
    }
}
