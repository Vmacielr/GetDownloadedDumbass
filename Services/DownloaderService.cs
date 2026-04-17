using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GetDownloadedDumbass.Models;

namespace GetDownloadedDumbass.Services
{
    public class DownloaderService
    {
        private readonly string _ytDlpPath;
        private readonly string _ffmpegPath;

        public DownloaderService(string ytDlpPath, string ffmpegPath)
        {
            _ytDlpPath = ytDlpPath;
            _ffmpegPath = ffmpegPath;
        }

        public async Task DownloadAsync(DownloadItem item, Action<double> onProgress, Action<string> onError, CancellationToken ct = default)
        {
            try
            {
                item.Status = DownloadStatus.Downloading;

                string formatArg = item.Format switch
                {
                    "mp3"  => "-x --audio-format mp3",
                    "flac" => "-x --audio-format flac",
                    "wav"  => "-x --audio-format wav",
                    "m4a"  => "-x --audio-format m4a",
                    _      => $"-f \"bestvideo[height<={GetHeight(item.Quality)}]+bestaudio/best\" --merge-output-format {item.Format}"
                };

                string args = $"{formatArg} " +
                              $"--ffmpeg-location \"{_ffmpegPath}\" " +
                              $"-o \"{item.OutputFolder}/%(title)s.%(ext)s\" " +
                              $"--newline " +
                              $"\"{item.Url}\"";

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _ytDlpPath,
                        Arguments = args,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    },
                    EnableRaisingEvents = true
                };

                process.Start();

                // Kill le process si annulation
                ct.Register(() =>
                {
                    try { if (!process.HasExited) process.Kill(entireProcessTree: true); }
                    catch { }
                });

                var stderrTask = process.StandardError.ReadToEndAsync();

                string? line;
                while ((line = await process.StandardOutput.ReadLineAsync()) != null)
                {
                    if (ct.IsCancellationRequested) break;

                    if (line.Contains("[download]") && line.Contains("%"))
                    {
                        double progress = ParseProgress(line);
                        item.Progress = progress;
                        System.Windows.Application.Current.Dispatcher.Invoke(() => onProgress(progress));
                    }
                }

                await process.WaitForExitAsync();
                string stderr = await stderrTask;

                if (ct.IsCancellationRequested)
                {
                    item.Status = DownloadStatus.Failed;
                    item.ErrorMessage = "Cancelled";
                }
                else if (process.ExitCode == 0)
                {
                    item.Status = DownloadStatus.Completed;
                    item.CompletedAt = DateTime.Now;
                    item.Progress = 100;
                    System.Windows.Application.Current.Dispatcher.Invoke(() => onProgress(100));
                }
                else
                {
                    item.Status = DownloadStatus.Failed;
                    item.ErrorMessage = stderr;
                    System.Windows.Application.Current.Dispatcher.Invoke(() => onError(stderr));
                }
            }
            catch (OperationCanceledException)
            {
                item.Status = DownloadStatus.Failed;
                item.ErrorMessage = "Cancelled";
            }
            catch (Exception ex)
            {
                item.Status = DownloadStatus.Failed;
                item.ErrorMessage = ex.Message;
                System.Windows.Application.Current.Dispatcher.Invoke(() => onError(ex.Message));
            }
        }

        public async Task<(string title, string thumbnailUrl)> FetchMetadataAsync(string url)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _ytDlpPath,
                        Arguments = $"--get-title --get-thumbnail \"{url}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string title     = await process.StandardOutput.ReadLineAsync() ?? "Unknown Title";
                string thumbnail = await process.StandardOutput.ReadLineAsync() ?? string.Empty;
                await process.WaitForExitAsync();

                return (title, thumbnail);
            }
            catch
            {
                return ("Unknown Title", string.Empty);
            }
        }

        private static double ParseProgress(string line)
        {
            try
            {
                int start = line.IndexOf(' ') + 1;
                int end   = line.IndexOf('%');
                if (end > start)
                {
                    string percent = line.Substring(start, end - start).Trim();
                    return double.Parse(percent, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            catch { }
            return 0;
        }

        private static string GetHeight(string quality) => quality switch
        {
            "1080p" => "1080",
            "720p"  => "720",
            "480p"  => "480",
            "360p"  => "360",
            _       => "9999"
        };
    }
}