using FFMpegUI.Messages;
using FFMpegUI.Models;
using FFMpegUI.Services.Middlewares;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FFMpegUI.Services
{
    public class FFMpegConversionService : IFFMpegConvertingService
    {
        private static readonly Regex ffmpegTimeRegex = new Regex(@"time=(\d{2}:\d{2}:\d{2}\.\d{2})", RegexOptions.Compiled);

        private readonly IQFileServerApiService fileServerApiService;
        private readonly IProgressMessagesDispatcher progressMessagesDispatcher;


        public FFMpegConversionService(IQFileServerApiService fileServerApiService,
            IProgressMessagesDispatcher progressMessagesDispatcher)
        {
            this.fileServerApiService = fileServerApiService;
            this.progressMessagesDispatcher = progressMessagesDispatcher;
        }


        async Task<FFMpegConvertedFileDTO> IFFMpegConvertingService.ConvertEmittingMessages(long qFileServerFileId, int processItemId, int processId, FFMpegConvertParameters parameters)
        {
            var ret = await Convert(qFileServerFileId, processItemId, processId, parameters, true);
            return ret;
        }


        private async Task<FFMpegConvertedFileDTO> Convert(long qFileServerFileId, int processItemId, int processId, FFMpegConvertParameters parameters, bool emitMessages)
        {
            var qSourceFile = await fileServerApiService.DownloadFile(qFileServerFileId);

            var tempDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())).FullName;

            var sourceFilePath = Path.Combine(tempDir, qSourceFile.FileName);
            using (var fs = new FileStream(sourceFilePath, FileMode.Create, FileAccess.Write))
            {
                await qSourceFile.FileStream.CopyToAsync(fs);
            }

            var targetFileName = qSourceFile.FileName + ".mp4";
            var targetFilePath = Path.Combine(tempDir, targetFileName);

            var videoDuration = GetVideoDuration(sourceFilePath);

            ConvertFile(sourceFilePath, targetFilePath, parameters, outputLine =>
            {
                if (outputLine == null)
                {
                    return;
                }

                Match match = ffmpegTimeRegex.Match(outputLine);
                if (match.Success)
                {
                    string currentTimeStr = match.Groups[1].Value;
                    TimeSpan currentTime = TimeSpan.Parse(currentTimeStr);

                    double percentange = (currentTime.TotalSeconds / videoDuration.TotalSeconds) * 100;

                    var progressMessage = new FFMpegProcessItemMessage
                    {
                        ProcessItemId = processItemId,
                        ProgressMessage = $"{percentange:0.##}% ({currentTime.ToString(@"hh\:mm\:ss")} on {videoDuration.ToString(@"hh\:mm\:ss")})",
                        ProcessId = processId
                    };

                    if (percentange > 100.0)
                    {
                        progressMessage.ProgressMessage = "completing...";
                    }

                    if (emitMessages)
                    {
                        progressMessagesDispatcher.DispatchProcessItemProgress(progressMessage);
                    }
                }
            });

            // carichiamo su qfileserver            
            long retId = 0;
            long fileSize = 0;
            using (var upStream = new FileStream(targetFilePath, FileMode.Open, FileAccess.Read))
            {
                var dto = await fileServerApiService.UploadFile(upStream, targetFileName);
                fileSize = dto.Size;
                retId = dto.Id;
            }

            var ret = new FFMpegConvertedFileDTO
            {
                Filename = targetFileName,
                QFileServerId = retId,
                Filesize = fileSize
            };

            Directory.Delete(tempDir, true);

            return ret;
        }
       

        private void ConvertFile(string sourceFilePath, string destFilePath, FFMpegConvertParameters parameters, Action<string> stdoutCallback)
        {
            var videoDuration = GetVideoDuration(sourceFilePath);

            string ffmpegPath = "ffmpeg"; // Assuming FFmpeg is in the system's PATH environment variable

            var vcodec = string.Empty;
            var acodec = string.Empty;

            if (parameters.VideoCodec == FFMpegVideoCodecType.H264)
            {
                vcodec = "libx264";
            }

            if (parameters.AudioCodec == FFMpegAudioCodecType.AAC)
            {
                acodec = "aac";
            }

            string arguments = $"-i \"{sourceFilePath}\" -vf \"scale={parameters.RescaleHorizontalWidth}:-1\" -vcodec {vcodec} -crf {parameters.OverallConversionQuality} -acodec {acodec} \"{destFilePath}\"";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;

                if (stdoutCallback != null)
                {
                    process.ErrorDataReceived += (sender, e) => stdoutCallback(e.Data);
                }

                process.Start();

                // Begin asynchronous read operations on the process output
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
        }


        private TimeSpan GetVideoDuration(string fileFullPath)
        {
            var ffmpeg = new Process();

            ffmpeg.StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i {fileFullPath}",
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            ffmpeg.Start();

            string output = ffmpeg.StandardError.ReadToEnd();
            var regex = new Regex(@"Duration: (\d{2}):(\d{2}):(\d{2})");
            var match = regex.Match(output);

            if (match.Success)
            {
                var hours = int.Parse(match.Groups[1].Value);
                var minutes = int.Parse(match.Groups[2].Value);
                var seconds = int.Parse(match.Groups[3].Value);

                var duration = new TimeSpan(hours, minutes, seconds);

                return duration;
            }
            else
            {
                throw new Exception("Could not determine duration");
            }
        }
    }
}
