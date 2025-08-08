using PhotoSauce.MagicScaler;

namespace Core.Helpers
{
	public static class ImageHelper
    {
        public static bool IsValidImage(string filename)
        {
            var ext = Path.GetExtension(filename).ToLower();
            var validExt = new List<string> { ".jpg", ".jpeg", ".png", ".gif" };

            return validExt.Contains(ext);
        }

        public static bool IsValidFile(string filename)
        {
            var ext = Path.GetExtension(filename).ToLower();
            var validExt = new List<string> { ".pdf"};

            return validExt.Contains(ext);
        }

        public static bool IsCompetenceFile(string filename)
        {
            var ext = Path.GetExtension(filename).ToLower();
            var validExt = new List<string> { ".jpg", ".jpeg", ".png", ".gif" , ".pdf"};
            return validExt.Contains(ext);
        }

        public static bool IsVideoFile(string filename)
        {
            var ext = Path.GetExtension(filename).ToLower();
            var validExt = new List<string> { ".mp4" };
            return validExt.Contains(ext);
        }

        public static string SaveImage(Stream inFileStream, string inFile, string outFolderPath, int width, int height)
        {
            var ext = Path.GetExtension(inFile);

            // generate a unique filename
            var fileName = $"{Guid.NewGuid().ToString()}{ext}";
            var outPath = Path.Combine(outFolderPath, fileName);

            //   var fileName = Path.GetFileNameWithoutExtension(inFile).Replace(" ", "");
            if (fileName.Length > 90)
                fileName = fileName.Substring(0, 80);

            var settings = new ProcessImageSettings
            {
                //SaveFormat = FileFormat.Jpeg,
                Width = width,
                Height = height,

                //JpegQuality = 80,

            };

            if (ext == ".png")
                settings.TrySetEncoderFormat(ImageMimeTypes.Png);
            else
            {
                settings.TrySetEncoderFormat(ImageMimeTypes.Jpeg);
                settings.DpiX = settings.DpiY = 80;
                ext = ".jpg";
            }

                //while (File.Exists(Path.Combine(outFolderPath, fileName + ext)))
                //{
                //    var rnd = new Random().Next(1, 99999);
                //    fileName = $"{fileName}_{rnd}";
                //}


                // var outPath = Path.Combine(outFolderPath, fileName + ext);

                using (var outStream = new FileStream(outPath, FileMode.Create))
                {
                    MagicImageProcessor.ProcessImage(inFileStream, outStream, settings);
                }

            return fileName;
        }

        public static string SaveImage(Stream inFileStream, string inFile, string outFolderPath)
        {
            var ext = Path.GetExtension(inFile);

            // generate a unique filename
            var fileName = $"{Guid.NewGuid().ToString()}{ext}";
            var outPath = Path.Combine(outFolderPath, fileName);


            if (fileName.Length > 90)
                fileName = fileName.Substring(0, 80);

            var settings = new ProcessImageSettings();


            if (ext == ".png")
                settings.TrySetEncoderFormat(ImageMimeTypes.Png);
            else
            {
                settings.TrySetEncoderFormat(ImageMimeTypes.Jpeg);
                settings.DpiX = settings.DpiY = 80;
                ext = ".jpg";
            }

            using (var outStream = new FileStream(outPath, FileMode.Create))
            {
                MagicImageProcessor.ProcessImage(inFileStream, outStream, settings);
            }

            return fileName;
        }

        public static string SaveFile(Stream inFileStream, string inFile, string outFolderPath)
        {
            var ext = ".pdf";
            var fileName = Path.GetFileNameWithoutExtension(inFile).Replace(" ", "");
            if (fileName.Length > 90)
                fileName = fileName.Substring(0, 80);




            while (File.Exists(Path.Combine(outFolderPath, fileName + ext)))
            {
                var rnd = new Random().Next(1, 99999);
                fileName = $"{fileName}_{rnd}";
            }


            var outPath = Path.Combine(outFolderPath, fileName + ext);

            using (var outStream = new FileStream(outPath, FileMode.Create))
            {

                inFileStream.CopyTo(outStream);
                // MagicImageProcessor.ProcessImage(inFileStream, outStream, settings);
            }

            return fileName + ext;
        }

        public static string SaveVideo(Stream inFileStream, string inFile, string outFolderPath)
        {
            // Uzantıyı alıp küçük harfe çeviriyoruz
            var ext = Path.GetExtension(inFile).ToLower();

            // Sadece .mp4 kabul edelim
            if (ext != ".mp4")
                throw new InvalidOperationException("Geçersiz video formatı. Sadece .mp4 dosyaları kabul edilir.");

            // Orijinal dosya adından baseName oluştur
            var baseName = Path.GetFileNameWithoutExtension(inFile)
                               .Replace(" ", "_");

            // Çok uzun ad olmasın
            if (baseName.Length > 80)
                baseName = baseName.Substring(0, 80);

            // Son dosya adı
            var fileName = $"{baseName}{ext}";
            var outPath = Path.Combine(outFolderPath, fileName);

            // Stream'i diske yaz (aynı isim varsa üzerine yazar)
            using (var outStream = new FileStream(outPath, FileMode.Create))
            {
                inFileStream.CopyTo(outStream);
            }

            return fileName;
        }

        public static bool RemoveFile(string fileName)
        {

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
                return true;
            }


            return true;




        }

    }
}
