using System;

namespace GetDownloadedDumbass.Models
{
    public enum DownloadStatus
    {
        Queued,
        Downloading,
        Completed,
        Failed
    }

    public class DownloadItem
    {
        public string Url { get; set; } = string.Empty;
        public string Title { get; set; } = "Unknown Title";
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string Format { get; set; } = "mp4";
        public string Quality { get; set; } = "best";
        public string OutputFolder { get; set; } = string.Empty;
        public double Progress { get; set; } = 0;
        public DownloadStatus Status { get; set; } = DownloadStatus.Queued;
        public DateTime AddedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; } = null;
        public string ErrorMessage { get; set; } = string.Empty;
        public bool IsPlaylist { get; set; } = false;
    }
}