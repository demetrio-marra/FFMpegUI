﻿using FFMpegUI.Models;

namespace FFMpegUI.Infrastructure.Support
{
    public class ConvertProcessItemTaskRunnerItem
    {
        public long QFileServerFileId { get; set; }
        public int ProcessItemId { get; set; }
        public FFMpegConvertParameters ConvertParameters { get; set; }
    }
}
