﻿namespace FFMpegUI.Models
{
    public class FFMpegProcessItem
    {
        public int Id { get; set; }
        public int? ProcessId { get; set; }
        public long? SourceFileId { get; set; }
        public string? SourceFileName { get; set; }
        public long? ConvertedFileId { get; set; }
        public string? ConvertedFileName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? Successfull { get; set; }
        public string? StatusMessage { get; set; }
        public long? SourceFileSize { get; set; }
        public long? ConvertedFileSize { get; set; }
    }
}
