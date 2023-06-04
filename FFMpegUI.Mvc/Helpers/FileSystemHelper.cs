namespace FFMpegUI.Mvc.Helpers
{
    public class FileSystemHelper
    {
        public static string FormatFileSize(long fileSizeInBytes)
        {
            string[] sizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            const int byteConversion = 1024;

            if (fileSizeInBytes == 0)
            {
                return "0" + sizeSuffixes[0];
            }

            var bytes = Math.Abs(fileSizeInBytes);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, byteConversion)));
            var size = Math.Round(bytes / Math.Pow(byteConversion, place), 1);
            var suffix = sizeSuffixes[place];

            return (Math.Sign(fileSizeInBytes) * size).ToString() + suffix;
        }
    }
}
