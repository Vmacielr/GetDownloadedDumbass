using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GetDownloadedDumbass.Models;
using GetDownloadedDumbass.Services;

namespace GetDownloadedDumbass.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // ── Services ──────────────────────────────────────────────
        private readonly DownloaderService _downloader;
        private readonly HistoryService _history;
        private readonly SettingsService _settingsService;

        // ── Cancellation ──────────────────────────────────────────
        private CancellationTokenSource? _cts;

        // ── State ─────────────────────────────────────────────────
        public AppSettings Settings { get; private set; }
        public ObservableCollection<DownloadItem> Queue { get; } = new();
        public ObservableCollection<DownloadItem> History { get; } = new();

        // ── ComboBox sources ──────────────────────────────────────
        public List<string> Formats { get; } = new() { "mp4", "mkv", "mp3", "flac", "wav", "m4a" };
        public List<string> Qualities { get; } = new() { "best", "1080p", "720p", "480p", "360p" };

        // ── Bound properties ──────────────────────────────────────
        private string _url = string.Empty;
        public string Url
        {
            get => _url;
            set { _url = value; OnPropertyChanged(); }
        }

        private string _selectedFormat = "mp4";
        public string SelectedFormat
        {
            get => _selectedFormat;
            set { _selectedFormat = value; OnPropertyChanged(); }
        }

        private string _selectedQuality = "best";
        public string SelectedQuality
        {
            get => _selectedQuality;
            set { _selectedQuality = value; OnPropertyChanged(); }
        }

        private string _statusMessage = "Ready";
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        // ── Commands ──────────────────────────────────────────────
        public ICommand AddToQueueCommand { get; }
        public ICommand StartDownloadCommand { get; }
        public ICommand StopDownloadCommand { get; }
        public ICommand ClearHistoryCommand { get; }
        public ICommand BrowseFolderCommand { get; }

        // ── Constructor ───────────────────────────────────────────
        public MainViewModel()
        {
            _settingsService = new SettingsService();
            Settings = _settingsService.Load();

            _downloader = new DownloaderService(Settings.YtDlpPath, Settings.FfmpegPath);
            _history = new HistoryService();

            foreach (var item in _history.Load())
                History.Add(item);

            AddToQueueCommand    = new RelayCommand(_ => AddToQueue(), _ => !string.IsNullOrWhiteSpace(Url));
            StartDownloadCommand = new RelayCommand(async _ => await StartDownloadAsync(), _ => Queue.Count > 0 && !IsBusy);
            StopDownloadCommand  = new RelayCommand(_ => StopDownload(), _ => IsBusy);
            ClearHistoryCommand  = new RelayCommand(_ => ClearHistory());
            BrowseFolderCommand  = new RelayCommand(_ => BrowseFolder());
        }

        // ── Methods ───────────────────────────────────────────────
        private void AddToQueue()
        {
            var item = new DownloadItem
            {
                Url = Url,
                Format = SelectedFormat,
                Quality = SelectedQuality,
                OutputFolder = Settings.DefaultOutputFolder
            };

            Queue.Add(item);
            Url = string.Empty;
            StatusMessage = $"{Queue.Count} item(s) in queue";
        }

        private async Task StartDownloadAsync()
        {
            _cts = new CancellationTokenSource();
            IsBusy = true;

            while (Queue.Count > 0 && !_cts.Token.IsCancellationRequested)
            {
                var item = Queue[0];
                StatusMessage = $"Downloading: {item.Url}";

                await _downloader.DownloadAsync(
                    item,
                    progress => StatusMessage = $"Progress: {progress:F0}%",
                    error => StatusMessage = $"Error: {error}",
                    _cts.Token
                );

                Queue.RemoveAt(0);
                _history.Add(item);
                History.Insert(0, item);
            }

            StatusMessage = _cts.Token.IsCancellationRequested ? "Download stopped." : "All downloads completed";
            IsBusy = false;
            _cts.Dispose();
            _cts = null;
        }

        private void StopDownload()
        {
            _cts?.Cancel();
            StatusMessage = "Stopping...";
        }

        private void ClearHistory()
        {
            History.Clear();
            _history.Save(new List<DownloadItem>());
        }

        private void BrowseFolder()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Settings.DefaultOutputFolder = dialog.SelectedPath;
                _settingsService.Save(Settings);
                OnPropertyChanged(nameof(Settings));
            }
        }

        // ── INotifyPropertyChanged ────────────────────────────────
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}