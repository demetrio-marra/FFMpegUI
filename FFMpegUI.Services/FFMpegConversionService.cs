using FFMpegUI.Models;
using FFMpegUI.Services.Middlewares;
using System.Diagnostics;

namespace FFMpegUI.Services
{
    public class FFMpegConversionService : IFFMpegConvertingService
    {
        private readonly IQFileServerApiService fileServerApiService;

        public FFMpegConversionService(IQFileServerApiService fileServerApiService)
        {
            this.fileServerApiService = fileServerApiService;
        }


        async Task<FFMpegConvertedFileDTO> IFFMpegConvertingService.Convert(long qFileServerFileId, FFMpegConvertParameters parameters)
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
            ConvertFile(sourceFilePath, targetFilePath, parameters);

            // carichiamo su qfileserver            
            long retId = 0;
            using (var upStream = new FileStream(targetFilePath, FileMode.Open, FileAccess.Read))
            {
                var dto = await fileServerApiService.UploadFile(upStream, targetFileName);
                retId = dto.Id;
            }

            var ret = new FFMpegConvertedFileDTO
            {
                Filename = targetFileName,
                QFileServerId = retId
            };

            Directory.Delete(tempDir, true);

            return ret;
        }


        private void ConvertFile(string sourceFilePath, string destFilePath, FFMpegConvertParameters parameters)
        {
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

                // Event handlers to capture the process output
                process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
                process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

                process.Start();

                // Begin asynchronous read operations on the process output
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();
            }
        }
    }
}
