using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace DIBZ.Common
{
    public class FileSaveHelper
    {

        #region Static Members
        public static System.Drawing.Image MakeGrayscale(System.Drawing.Image src)
        {
            //make an empty bitmap the same size as original
            Bitmap original = new Bitmap(src);
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    //get the pixel from the original image
                    Color originalColor = original.GetPixel(i, j);

                    //create the grayscale version of the pixel
                    int grayScale = (int)((originalColor.R * .3) + (originalColor.G * .59)
                        + (originalColor.B * .11));

                    //create the color object
                    Color newColor = Color.FromArgb(grayScale, grayScale, grayScale);

                    //set the new image's pixel to the grayscale version
                    newBitmap.SetPixel(i, j, newColor);
                }
            }
            MemoryStream ms = new MemoryStream();
            newBitmap.Save(ms, ImageFormat.Jpeg);

            return System.Drawing.Image.FromStream(ms);
        }

        public static byte[] ReadBytesFromFile(string filePath)
        {
            if (File.Exists(filePath))
                return File.ReadAllBytes(filePath);

            return new byte[0];
        }

        public static void ResizeAndSaveFromStream(string filePath, int MaxWidth, int MaxHeight, System.IO.Stream Buffer)
        {
            ResizeAndSaveFromStream(filePath, MaxWidth, MaxHeight, Buffer, false);
        }

        public static void ResizeAndSaveFromStream(string filePath, int MaxWidth, int MaxHeight, System.IO.Stream Buffer, bool makeGrayed)
        {
            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(Buffer);
                /*var fileExtension = System.IO.Path.GetExtension(filePath);
                var imageFormat = GetImageFormatFromExtension(fileExtension);*/

                SaveBytesToFile(ResizeImage(image, MaxWidth, MaxHeight, makeGrayed, ImageFormat.Png), filePath);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(string.Format("Error While Saving File To Disk [FileSaveHelper.ResizeFromStream]  {0}", filePath), ex);
                throw ex;
            }
        }

        public static void ResizeAndSaveFromImagePath(string imagePath, int width, int height,
            string pathToSaveFile)
        {
            var buffer = ResizeImageFromImagePath(imagePath, width, height);
            SaveBytesToFile(buffer, pathToSaveFile);
        }

        public static byte[] ResizeImageFromImagePath(string path, int width, int height)
        {
            var image = System.Drawing.Image.FromFile(path);
            return ResizeImage(image, width, height);
        }

        public static byte[] ResizeImage(System.Drawing.Image FullsizeImage, int NewWidth, int MaxHeight)
        {
            return ResizeImage(FullsizeImage, NewWidth, MaxHeight, ImageFormat.Bmp);
        }

        public static byte[] ResizeImage(System.Drawing.Image FullsizeImage, int NewWidth, int MaxHeight, ImageFormat imageFormat)
        {
            bool OnlyResizeIfWider = true;

            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            if (OnlyResizeIfWider)
            {
                if (FullsizeImage.Width <= NewWidth)
                {
                    NewWidth = FullsizeImage.Width;
                }
            }

            int NewHeight = FullsizeImage.Height * NewWidth / FullsizeImage.Width;
            if (NewHeight > MaxHeight)
            {
                // Resize with height instead
                NewWidth = FullsizeImage.Width * MaxHeight / FullsizeImage.Height;
                NewHeight = MaxHeight;
            }

            //System.Drawing.Image NewImage = FullsizeImage.GetThumbnailImage(NewWidth, NewHeight, null, IntPtr.Zero);
            Size newSize = new Size(NewWidth, NewHeight);
            using (Bitmap newImage = new Bitmap(newSize.Width, newSize.Height))
            {
                using (Graphics canvas = Graphics.FromImage(newImage))
                {

                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    canvas.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    canvas.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    canvas.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;


                    /*canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;*/
                    canvas.DrawImage(FullsizeImage, new Rectangle(new Point(0, 0), newSize));
                    MemoryStream m = new MemoryStream();
                    newImage.Save(m, imageFormat);
                    return m.GetBuffer();
                    //return newImage;
                }
            }
        }

        public static void SaveImage(System.Drawing.Image FullsizeImage, string serverPath)
        {
            SaveImage(FullsizeImage, serverPath, false);
        }

        public static void SaveImage(System.Drawing.Image FullsizeImage, string serverPath, bool forceImageFormat)
        {
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            //System.Drawing.Image NewImage = FullsizeImage.GetThumbnailImage(NewWidth, NewHeight, null, IntPtr.Zero);
            Size newSize = new Size(FullsizeImage.Width, FullsizeImage.Height);
            byte[] buffer;
            using (Bitmap newImage = new Bitmap(FullsizeImage.Width, FullsizeImage.Height))
            {
                using (Graphics canvas = Graphics.FromImage(newImage))
                {
                    canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    canvas.DrawImage(FullsizeImage, new Rectangle(new Point(0, 0), newSize));
                    MemoryStream m = new MemoryStream();
                    var extension = Path.GetExtension(serverPath);
                    ImageFormat imageFormat;
                    if (forceImageFormat)
                    {
                        imageFormat = ImageFormat.Png;
                    }
                    else
                    {
                        imageFormat = GetImageFormatFromExtension(extension);
                    }
                    newImage.Save(m, imageFormat);
                    buffer = m.GetBuffer();
                }
            }

            SaveBytesToFile(buffer, serverPath);
        }

        public static byte[] ResizeImageWithMinimumDimension(System.Drawing.Image FullsizeImage, int minWidth, int minHeight)
        {
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            var newWidth = 0;
            // min width is checked and updated to what it should be
            if (FullsizeImage.Width < minWidth)
            {
                minWidth = FullsizeImage.Width;
            }
            newWidth = minWidth;

            // Calculating the new height with respect to the change in the width. The new height
            // should be greater than the min height. If not then the width will be adjusted
            var newHeight = FullsizeImage.Height * minWidth / FullsizeImage.Width;
            if (newHeight < minHeight)
            {
                newWidth = FullsizeImage.Width * minHeight / FullsizeImage.Height;
                newHeight = minHeight;
            }

            //System.Drawing.Image NewImage = FullsizeImage.GetThumbnailImage(NewWidth, NewHeight, null, IntPtr.Zero);
            Size newSize = new Size(newWidth, newHeight);
            using (Bitmap newImage = new Bitmap(newSize.Width, newSize.Height))
            {
                using (Graphics canvas = Graphics.FromImage(newImage))
                {
                    //canvas.SmoothingMode = SmoothingMode.AntiAlias;
                    canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //canvas.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    canvas.DrawImage(FullsizeImage, new Rectangle(new Point(0, 0), newSize));
                    MemoryStream m = new MemoryStream();
                    newImage.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return m.GetBuffer();
                    //return newImage;
                }
            }
        }

        public static byte[] ResizeImage(System.Drawing.Image FullsizeImage, int NewWidth, int MaxHeight, bool makeGrayed, ImageFormat imageFormat)
        {
            if (makeGrayed)
                FullsizeImage = MakeGrayscale(FullsizeImage);
            return ResizeImage(FullsizeImage, NewWidth, MaxHeight, imageFormat);
        }

        private static byte[] ResizeImageFromBytesArray(byte[] OriginalFile, int NewWidth, int MaxHeight)
        {
            return ResizeImage(GetImageFromBytes(OriginalFile), NewWidth, MaxHeight);
        }

        public static void ResizeAndSaveBitmap(Bitmap bitmap, int width, int height, string pathToSave)
        {
            MemoryStream ms = new MemoryStream();
            var fileExtension = System.IO.Path.GetExtension(pathToSave);
            /*var imageFormat = GetImageFormatFromExtension(fileExtension);
            bitmap.Save(ms, imageFormat);*/
            bitmap.Save(ms, ImageFormat.Png);
            //byte[] bitmapData = ms.ToArray();
            ResizeAndSaveFromStream(pathToSave, width, height, ms);
        }


        private static byte[] ResizeImageAsThumbnail(byte[] OriginalFile, int _thumbnailSize)
        {
            System.Drawing.Image photo = System.Drawing.Image.FromStream(new MemoryStream(OriginalFile));

            int width, height;
            if (photo.Width > photo.Height)
            {
                width = _thumbnailSize * photo.Width / photo.Height;
                height = _thumbnailSize;
            }
            else
            {
                width = _thumbnailSize;
                height = _thumbnailSize * photo.Height / photo.Width;
            }
            Bitmap target = new Bitmap(_thumbnailSize, _thumbnailSize);
            using (Graphics graphics = Graphics.FromImage(target))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(photo, (_thumbnailSize - width) / 2, (_thumbnailSize - height) / 2, width, height);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    target.Save(memoryStream, ImageFormat.Png);
                    return memoryStream.GetBuffer();
                }
            }
        }

        public static void SaveBitmapToDisk(Page pg, string filePath, Bitmap bmp)
        {
            try
            {
                bmp.Save(filePath);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(string.Format("Error While Saving File To Disk [FileSaveHelper.SaveBitmapToDisk]  {0}", filePath), ex);
                throw ex;
            }
        }

        public static void SaveBytesToFile(byte[] buffer, string filePath)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(filePath, FileMode.CreateNew));
            bw.Write(buffer);
            bw.Flush();
            bw.Close();
        }

        public static void SaveBytesToStream(byte[] buffer, Stream output)
        {
            BinaryWriter bw = new BinaryWriter(output);
            bw.Write(buffer);
        }

        private static byte[] GetBytesFromImage(System.Drawing.Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, img.RawFormat);

            img.Dispose();
            return ms.GetBuffer();
        }

        private static System.Drawing.Image GetImageFromBytes(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }



        #endregion

        #region Crop Image ---------------------------------------------------------------------------------
        public static string CropImage(string originalFilePath, int w, int h, int X, int Y, string serverPathToSave,
            string croppedFileName)
        {
            return CropImage(originalFilePath, w, h, X, Y, serverPathToSave, croppedFileName, false, 0, 0);
        }

        /// <summary>
        /// Crops 
        /// </summary>
        /// <param name="originalFile"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public static string CropImage(string originalFilePath, int w, int h, int X, int Y, string serverPathToSave,
            string croppedFileName, bool resizeImage, int newWidth, int newHeight)
        {
            using (System.Drawing.Image img = System.Drawing.Image.FromFile(originalFilePath))
            {
                using (System.Drawing.Bitmap _bitmap = new System.Drawing.Bitmap(w, h))
                {
                    _bitmap.SetResolution(img.HorizontalResolution, img.VerticalResolution);
                    using (Graphics _graphic = Graphics.FromImage(_bitmap))
                    {
                        _graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        _graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        _graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        _graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        //_graphic.drDrawImage(img, 0, 0, w, h);
                        _graphic.DrawImage(img, new Rectangle(0, 0, w, h), X, Y, w, h, GraphicsUnit.Pixel);

                        string extension = Path.GetExtension(originalFilePath);
                        // If the image is a gif file, change it into png
                        if (extension.EndsWith("gif", StringComparison.OrdinalIgnoreCase))
                        {
                            extension = ".png";
                        }

                        var widthVariant = w - newWidth;
                        var heightVariant = h - newHeight;
                        if (widthVariant < 0)
                            widthVariant = widthVariant * -1;
                        if (heightVariant < 0)
                            heightVariant = heightVariant = heightVariant * -1;

                        if (widthVariant <= 2 && heightVariant <= 2)
                            resizeImage = false;

                        string newFullPathName = string.Concat(serverPathToSave, croppedFileName, extension);

                        using (EncoderParameters encoderParameters = new EncoderParameters(1))
                        {
                            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                            if (resizeImage)
                            {
                                ResizeAndSaveBitmap(_bitmap, newWidth, newHeight, newFullPathName);
                            }
                            else
                            {
                                _bitmap.Save(newFullPathName, GetImageCodec(extension), encoderParameters);
                            }
                        }

                        //// Saving the resized thumbnail of the original cropped image
                        //var thumbPath = ConversionHelper.GetSafeServerMapPath(Path.Combine(SystemSettings.PostImageThumbnailPath,
                        //    croppedFileName + extension));
                        //FileSaveHelper.ResizeAndSaveFromImagePath(newFullPathName, 99, 51, thumbPath);

                        return croppedFileName + extension;
                    }
                }
            }
        }

        /// <summary>
        /// Find the right codec
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static ImageCodecInfo GetImageCodec(string extension)
        {
            extension = extension.ToUpperInvariant();
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FilenameExtension.Contains(extension))
                {
                    return codec;
                }
            }
            return codecs[1];
        }
        #endregion

        HttpContext context;

        public FileSaveHelper()
            : this(HttpContext.Current)
        {
        }

        public FileSaveHelper(HttpContext _context)
        {
            context = _context;
        }

        private string GetNewFilePath(string uploadFolder, string originalName, string fileName, int count)
        {
            originalName = originalName.Replace("'", string.Empty);
            originalName = originalName.Replace("\"", string.Empty);

            fileName = fileName.Replace("'", string.Empty);
            fileName = fileName.Replace("\"", string.Empty);

            string fullPath = ConversionHelper.GetSafeServerMapPath(Path.Combine(uploadFolder, fileName));
            if (File.Exists(fullPath))
            {
                string fName = string.Format("{0}{1}{2}",
                    Path.GetFileNameWithoutExtension(originalName),
                    ++count,
                    Path.GetExtension(fileName));
                fullPath = GetNewFilePath(uploadFolder, originalName, fName, count);
            }
            return fullPath;
        }

        private string GetNewFilePath(string uploadFolder, string fileName)
        {
            return GetNewFilePath(uploadFolder, Path.GetFileName(fileName), fileName, 0);
        }
        public string SaveBytesToFile(byte[] buffer, string uploadFolder, string fileName)
        {
            return SaveBytesToFile(buffer, uploadFolder, fileName, false);
        }
        public string SaveBytesToFile(byte[] buffer, string uploadFolder, string fileName, bool returnFullPath)
        {
            string newPath = GetNewFilePath(uploadFolder, fileName);
            SaveBytesToFile(buffer, newPath);
            if (returnFullPath)
                return newPath;

            return Path.GetFileName(newPath);
        }
        public static void DeleteFile(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
                File.Delete(path);
        }

        public static ImageFormat GetImageFormatFromExtension(string fileExtension)
        {
            fileExtension = fileExtension.ToLower();
            switch (fileExtension)
            {
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".png":
                    return ImageFormat.Png;
                case ".tif":
                case ".tiff":
                    return ImageFormat.Tiff;
                default:
                    return ImageFormat.Jpeg;
            }
        }

        /// <summary>
        /// Crops 
        /// </summary>
        /// <param name="originalFile"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public static string BasCropImage(string originalFilePath, int w, int h, int X, int Y, string serverPathToSave,
            string croppedFileName, bool resizeImage, int newWidth, int newHeight)
        {
            using (System.Drawing.Image img = System.Drawing.Image.FromFile(originalFilePath))
            {
                using (System.Drawing.Bitmap _bitmap = new System.Drawing.Bitmap(w, h))
                {
                    _bitmap.SetResolution(img.HorizontalResolution, img.VerticalResolution);
                    using (Graphics _graphic = Graphics.FromImage(_bitmap))
                    {
                        _graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        _graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        _graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        _graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        //_graphic.drDrawImage(img, 0, 0, w, h);
                        _graphic.DrawImage(img, new Rectangle(0, 0, w, h), X, Y, w, h, GraphicsUnit.Pixel);

                        string extension = Path.GetExtension(originalFilePath);
                        // If the image is a gif file, change it into png
                        if (extension.EndsWith("gif", StringComparison.OrdinalIgnoreCase))
                        {
                            extension = ".png";
                        }

                        string newFullPathName = string.Concat(serverPathToSave, croppedFileName, extension);

                        using (EncoderParameters encoderParameters = new EncoderParameters(1))
                        {
                            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                            if (resizeImage)
                            {
                                ResizeAndSaveBitmap(_bitmap, newWidth, newHeight, newFullPathName);
                            }
                            else
                            {
                                _bitmap.Save(newFullPathName, GetImageCodec(extension), encoderParameters);
                            }
                        }

                        //// Saving the resized thumbnail of the original cropped image
                        //var thumbPath = ConversionHelper.GetSafeServerMapPath(Path.Combine(SystemSettings.PostImageThumbnailPath,
                        //    croppedFileName + extension));
                        //FileSaveHelper.ResizeAndSaveFromImagePath(newFullPathName, 99, 51, thumbPath);

                        return croppedFileName + extension;
                    }
                }
            }
        }

        public static void ResizeTo(string originalFilePath, int width, int height, string pathToSave)
        {
            using (System.Drawing.Image src = System.Drawing.Image.FromFile(originalFilePath))
            {
                using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height))
                {
                    var rect = new Rectangle(0, 0, width, height);
                    if (src.Height > src.Width)
                    {
                        rect.Width = (int)((double)rect.Width * ((double)src.Width / (double)src.Height));
                        rect.X += (width - rect.Width) / 2;
                    }
                    else
                    {
                        rect.Height = (int)((double)rect.Height * ((double)src.Height / (double)src.Width));
                        rect.Y += (height - rect.Height) / 2;
                    }
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        g.DrawImage(src, rect, 0, 0, src.Width, src.Height, GraphicsUnit.Pixel);
                    }

                    using (EncoderParameters encoderParameters = new EncoderParameters(1))
                    {
                        encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                        bmp.Save(pathToSave, GetImageCodec(Path.GetExtension(pathToSave)), encoderParameters);
                    }
                }
            }
        }
    }
}
