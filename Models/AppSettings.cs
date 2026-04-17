namespace GetDownloadedDumbass.Models
{
    public class AppSettings
    {
        public string DefaultOutputFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        public string DefaultFormat { get; set; } = "mp4";
        public string DefaultQuality { get; set; } = "best";
        public string YtDlpPath { get; set; } = "Assets/yt-dlp.exe";
        public string FfmpegPath { get; set; } = "Assets/ffmpeg.exe";
    }
}