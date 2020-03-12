using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Utils
{
    public class ProcessImage
    {
        #region Fields, Properties and Enumns

        /// <summary>
        /// Max image size in bytes allowed from import service.
        /// </summary>
        public const int MaxFileSize = 1024 * 1024 * 3;

        /// <summary>
        /// Max upload image size in bytes. 5MB - now is passed like argument for method CheckUploadedFile
        /// </summary>
        //public const int MaxUploadFileSize = 1024 * 1024 * 5;

        /// <summary>
        /// Allowed file formats and extensions.
        /// </summary>
        public static Dictionary<string, ImageFormat> allowedImageFiles
            = new Dictionary<string, ImageFormat> { 
                                                { ".jpeg", ImageFormat.Jpeg},
                                                { ".jpg", ImageFormat.Jpeg},
                                                { ".png", ImageFormat.Png},
                                                { ".bmp", ImageFormat.Bmp },
                                            /*    { ".tiff", ImageFormat.Tiff },
                                                { ".tif", ImageFormat.Tiff },*/
                                                { ".gif", ImageFormat.Gif },
                                                { ".JPEG",ImageFormat.Jpeg },
                                                { ".JPG", ImageFormat.Jpeg},
                                                { ".PNG", ImageFormat.Png},
                                                { ".BMP" ,ImageFormat.Bmp },
                                              /*  { ".TIFF", ImageFormat.Tiff },
                                                { ".TIF", ImageFormat.Tiff },*/
                                                { ".GIF", ImageFormat.Gif }};

        /// <summary>
        /// Process image errors.
        /// </summary>
        public enum Errors
        {
            FileNoError = 0,
            FileExistsError,
            FileExtensionError,
            FileMaxSizeError,
            FileMinResolutionError,
            FileUnknowError
        }

        /// <summary>
        /// Image file type.
        /// </summary>
        public enum ImageFileKind
        {
            Original = 0,
            ImportToObjectGallery = 1,
            ContractorLogo = 2,
            Thumbnail = 3,
            UploadToObjectGallery = 4,
            ObjectGallery = 5,
            Profile = 6,
            ManufacturerLogo = 7,
            PersonLogo = 8,
            Temp = 9
        }

        public enum ImageSize
        {
            Big,
            Small,
            Originals
        }

        /// <summary>
        /// Image file size dimension class.
        /// </summary>
        public class ImageFileDimension
        {
            /// <summary>
            /// Image file kind.
            /// </summary>
            public ImageFileKind ImageKind;

            /// <summary>
            /// Resize to Width.
            /// </summary>
            public Nullable<int> Width;

            /// <summary>
            /// Resize to Height.
            /// </summary>
            public Nullable<int> Height;

            /// <summary>
            /// Directory where file will be saved.
            /// </summary>
            public string DirectoryToSave;

            public Color BackGround = Color.White;

            public bool ResizeToDimensions = false;

            public bool IsCropped = false;
        }

        private static List<ImageFileDimension> listImageFileDimensions = new List<ImageFileDimension>()
        {
            new ImageFileDimension { ImageKind = ImageFileKind.Original, DirectoryToSave = "Originals", ResizeToDimensions = true}, 
            new ImageFileDimension { ImageKind = ImageFileKind.Profile, DirectoryToSave = "Big", Width = 250 , Height = 250, IsCropped = true }, 
            new ImageFileDimension { ImageKind = ImageFileKind.Profile, DirectoryToSave = "Small", Width = 32 , Height = 32, IsCropped = true },
            //new ImageFileDimension { ImageKind = ImageFileKind.Thumbnail, DirectoryToSave = "thumbnails_list", Width = 190 , Height = 143, IsCropped = true }, 
            //new ImageFileDimension { ImageKind = ImageFileKind.Thumbnail, DirectoryToSave = "references_list", Width = 270 , Height = 160, IsCropped = true },
            //new ImageFileDimension { ImageKind = ImageFileKind.Thumbnail, DirectoryToSave = "thumb_home_list", Width = 230 , Height = 160, IsCropped = true }, 
            
            ////from import object images
            
            //new ImageFileDimension { ImageKind = ImageFileKind.ImportToObjectGallery, DirectoryToSave = "gallery", Height = 300 },
            //new ImageFileDimension { ImageKind = ImageFileKind.ImportToObjectGallery, DirectoryToSave = "lightbox_gallery", Width = 590, Height=590 }, 
            //new ImageFileDimension { ImageKind = ImageFileKind.ImportToObjectGallery, DirectoryToSave = "thumb_lightbox_gallery" ,Height = 83, Width=110, IsCropped = true  },
            
            
            //// contractor logo upload
            //new ImageFileDimension { ImageKind = ImageFileKind.ContractorLogo, DirectoryToSave = "logos", Width = 110 , Height = 110 },
            //new ImageFileDimension { ImageKind = ImageFileKind.ContractorLogo, DirectoryToSave = "thumbnails_list", Width = 190 , Height = 143 }, 
            
            ////first step upload object
            //new ImageFileDimension { ImageKind = ImageFileKind.UploadToObjectGallery, DirectoryToSave ="thumb_lightbox_gallery" ,Height = 83, Width=110, IsCropped = true },
            
            ////second step 
            //new ImageFileDimension { ImageKind = ImageFileKind.ObjectGallery, DirectoryToSave = "gallery", Height = 300  },
            //new ImageFileDimension { ImageKind = ImageFileKind.ObjectGallery, DirectoryToSave = "lightbox_gallery", Width = 590, Height=590  },
            
            //new ImageFileDimension { ImageKind = ImageFileKind.Profile, DirectoryToSave = "profile", Width = 980, Height=440 } ,
            
            // // manufacturer logo upload
            //new ImageFileDimension { ImageKind = ImageFileKind.ManufacturerLogo, DirectoryToSave = "logos", Width = 110 , Height = 110 },
            //new ImageFileDimension { ImageKind = ImageFileKind.ManufacturerLogo, DirectoryToSave = "thumbnails_list", Width = 190 , Height = 143}, 
            //new ImageFileDimension { ImageKind = ImageFileKind.ManufacturerLogo, DirectoryToSave = "logos_small", Width = 100 , Height = 60},

            //// Person logo
            //new ImageFileDimension { ImageKind = ImageFileKind.PersonLogo, DirectoryToSave = "logos", Width = 100 , Height = 100 },
            
            ////Temp files
            //new ImageFileDimension { ImageKind = ImageFileKind.Temp, DirectoryToSave = "temp"}
        };


        /// <summary>
        /// Resize image parameters and directory to store.
        /// </summary>
        public static Dictionary<string, KeyValuePair<int, int>> resizedFiles
            = new Dictionary<string, KeyValuePair<int, int>> { { "home_web", new KeyValuePair<int, int>(118, 118) } };

        private static string tbnPrefix = "tbn_";

        private static int newWidth = 200;
        private static int newHeight = 200;

        #endregion

        [Obsolete]
        public static KeyValuePair<Errors, string> ProcessPostedImage(string filepath, HttpPostedFile userPostedFile, string id, string subid)
        {
            string error = "Error";

            try
            {
                error = userPostedFile.FileName;

                if (!CheckFileExtension(userPostedFile.FileName))
                {
                    return (new KeyValuePair<Errors, string>(Errors.FileExtensionError, error));
                }

                if (!CheckFileSize(userPostedFile.ContentLength, MaxFileSize))
                {
                    return (new KeyValuePair<Errors, string>(Errors.FileMaxSizeError, error));
                }

                string newFileName = SaveFileOnDisk(filepath, userPostedFile, "image_", string.Format("{0}_{1}", id, subid));

                foreach (string p in resizedFiles.Keys)
                {
                    SaveFile(filepath, userPostedFile, p, newFileName);
                }

                return (new KeyValuePair<Errors, string>(Errors.FileNoError, newFileName + System.IO.Path.GetExtension(userPostedFile.FileName)));
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return (new KeyValuePair<Errors, string>(Errors.FileUnknowError, error));
        }

        /// <summary>
        /// Check image file size.
        /// </summary>
        /// <param name="hpf"></param>
        /// <returns></returns>
        private static bool CheckFileSize(int fileSize, int maxSize)
        {
            if (fileSize < maxSize)
            {
                return true;
            }
            return false;
        }

        private static bool CheckFileExtension(string fileName)
        {
            string fileExt = System.IO.Path.GetExtension(fileName);
            if (allowedImageFiles.ContainsKey(fileExt))
            {
                return true;
            }

            return false;
        }

        private static bool CheckImageResolution(HttpPostedFile hpf, int minWidht, int minHeight)
        {
            using (Bitmap src = Bitmap.FromStream(hpf.InputStream) as Bitmap)
            {
                if (src.Width < minWidht || src.Height < minHeight)
                {
                    return false;
                }
                return true;
            }
        }

        private static void SaveFile(string filepath, HttpPostedFile hpf, string path, string newFileName)
        {
            string fileExt = System.IO.Path.GetExtension(hpf.FileName);

            // MakeDirectoryIfExists(filepath + "\\" + path);

            string fileName = filepath + "\\" + path + "\\" + newFileName + fileExt;

            Bitmap src = Bitmap.FromStream(hpf.InputStream) as Bitmap;

            // Resize the bitmap data
            Bitmap result = ProportionallyResizeBitmap(src, resizedFiles[path].Key, resizedFiles[path].Value);
            result.Save(fileName, allowedImageFiles[fileExt]);
        }

        private static void SaveCustomFile(string filepath, HttpPostedFile hpf, string path, int w, int h, string newFileName)
        {
            string fileExt = System.IO.Path.GetExtension(hpf.FileName);
            //  MakeDirectoryIfExists(filepath + "\\" + path);

            string fileName = filepath + "\\" + path + "\\" + newFileName + fileExt;

            // Resize the bitmap data
            Bitmap src = Bitmap.FromStream(hpf.InputStream) as Bitmap;
            Bitmap result = ProportionallyResizeBitmap(src, w, h);
            result.Save(fileName, allowedImageFiles[fileExt]);
        }

        private static string SaveFileOnDisk(string filepath, HttpPostedFile hpf, string prefix, string suffix)
        {
            string fileExt = System.IO.Path.GetExtension(hpf.FileName);

            string newFileName = filepath + "\\" + prefix + suffix + fileExt;

            //hpf.SaveAs(newFileName);

            return (prefix + suffix);
        }

        public static string SaveFileOnDisk(string filepath, HttpPostedFileBase hpf, string path, string newFileName)
        {
            string fileExt = System.IO.Path.GetExtension(hpf.FileName);

            string FilePath = filepath + "\\" + path + "\\"; 

            Bitmap src = Bitmap.FromStream(hpf.InputStream) as Bitmap;

            foreach (var value in listImageFileDimensions)
            {
                string newFilePath = FilePath + value.DirectoryToSave + "\\" + newFileName + fileExt;
                if (value.IsCropped)
                {
                    Bitmap result = CropAndResizeBitmap(src, 0, 0, src.Width, src.Height, value.Width.Value, value.Height.Value ); //ProportionallyResizeBitmap(src, value.Width.Value, value.Height.Value);
                    result.Save(newFilePath);
                }
                else 
                {
                    hpf.SaveAs(newFilePath);
                }
                
            }

            return (newFileName + fileExt);
        }

        private static string SaveFileThumb(string filepath, HttpPostedFile hpf)
        {
            string fileExt = System.IO.Path.GetExtension(hpf.FileName);

            string tbnFileName = filepath + "\\" + tbnPrefix + System.IO.Path.GetFileName(hpf.FileName);

            Bitmap src = Bitmap.FromStream(hpf.InputStream) as Bitmap;

            // Resize the bitmap data
            Bitmap result = ProportionallyResizeBitmap(src, newWidth, newHeight);
            result.Save(tbnFileName, allowedImageFiles[fileExt]);
            return tbnFileName;
        }

        /// <exception cref="System.ArgumentNullException">Thrown when originalImgPath or format is null.</exception>
        /// <exception cref="System.Runtime.InteropServices.ExternalException">Thrown when the image was saved with the wrong image format. Or when the image was saved to the same file it was created from.</exception>
        public static void SaveCroppedImageByKind(string fileName, Rectangle cropData, ImageFileKind imageKind)
        {
            string fileExt = System.IO.Path.GetExtension(fileName);
            string tempImgPath = Path.Combine(Files.UploadDirectoryPath, listImageFileDimensions.Where(ifd => ifd.ImageKind == ImageFileKind.Temp).SingleOrDefault().DirectoryToSave, fileName);
            string originalImgPath = Path.Combine(Files.UploadDirectoryPath, listImageFileDimensions.Where(ifd => ifd.ImageKind == ImageFileKind.Original).SingleOrDefault().DirectoryToSave, fileName);

            using (Bitmap src = new Bitmap(tempImgPath))
            {
                if (src.Width < (cropData.Width + cropData.Left) || src.Height < (cropData.Height + cropData.Top))
                {
                    cropData = GetDefaultCrop(cropData, src);
                }
                // Resize the bitmap data
                using (Bitmap result = CropBitmap(src, cropData))
                {
                    result.Save(originalImgPath, allowedImageFiles[fileExt]);
                    ResizeImageFilesByKind(fileName, fileName, imageKind);
                }
            }
            DeleteFile(tempImgPath);
        }

        private static Rectangle GetDefaultCrop(Rectangle cropData, Bitmap src)
        {
            double aspecRatio = cropData.Height / cropData.Width;
            double cropPart = 1.02;
            double cropStep = 0.02;
            double cropSizeX;
            double cropSizeY;

            do
            {
                cropPart -= cropStep;
                cropSizeX = src.Width * cropPart;
                cropSizeY = cropSizeX * aspecRatio;
            } while (cropSizeX > src.Width || cropSizeY > src.Height);

            int cropX = Convert.ToInt32((src.Width - cropSizeX) / 2);
            int cropY = Convert.ToInt32((src.Height - cropSizeY) / 2);

            return new Rectangle(cropX, cropY, Convert.ToInt32(cropSizeX), Convert.ToInt32(cropSizeY));
        }

        private static Bitmap ProportionallyResizeBitmap(Bitmap src, int maxWidth, int maxHeight)
        {
            // original dimensions
            int w = src.Width;
            int h = src.Height;

            float factorW = ((float)maxWidth) / w;
            float factorH = ((float)maxHeight) / h;

            float ratio = (factorW < factorH ? factorW : factorH);// (widthRatio)MIN(maxWidth / Width, maxHeight / Height);
            double newWidth = ratio * w;
            double newHeight = ratio * h;

            // Create new Bitmap at new dimensions
            Bitmap result = new Bitmap((int)newWidth, (int)newHeight);
            using (Graphics g = Graphics.FromImage((System.Drawing.Image)result))
                g.DrawImage(src, 0, 0, (int)newWidth, (int)newHeight);
            return result;
        }

        private static Bitmap CropAndResizeBitmap(Bitmap src, int x, int y, int cropWidth, int cropHeight, int newWidth, int newHeight)
        {
            // original dimensions
            int w = src.Width;
            int h = src.Height;

            if (cropWidth + x > w || cropHeight + y > h)
            {
                return null;
            }

            Bitmap result = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage((System.Drawing.Image)result))
            {
                g.DrawImage(src, new Rectangle(0, 0, newWidth, newHeight), x, y, cropWidth, cropHeight, GraphicsUnit.Pixel);
            }
            return result;
        }

        private static Bitmap CropBitmap(Bitmap src, Rectangle cropData)
        {
            // original dimensions
            int w = src.Width;
            int h = src.Height;

            if (cropData.Width + cropData.X > w || cropData.Height + cropData.Y > h)
            {
                return null;
            }

            Bitmap result = new Bitmap(cropData.Width, cropData.Height);
            using (Graphics g = Graphics.FromImage((System.Drawing.Image)result))
            {
                g.DrawImage(src, new Rectangle(0, 0, cropData.Width, cropData.Height), cropData, GraphicsUnit.Pixel);
            }
            return result;
        }

        private static Bitmap ResizeBitmap(Bitmap src, int newWidth, int newHeight)
        {
            Bitmap result = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage((System.Drawing.Image)result))
            {
                g.DrawImage(src, 0, 0, newWidth, newHeight);
            }
            return result;
        }

        public static void DeleteAllFiles(string filepath, Dictionary<string, int> dFiles)
        {
            foreach (string f in dFiles.Keys)
            {
                DeleteSingleFile(filepath, f);
            }
        }

        public static void DeleteSingleFile(string filepath, string file)
        {
            foreach (string p in resizedFiles.Keys)
            {

                string fileName = filepath + "\\" + p + "\\" + file;
                if (File.Exists(fileName)) File.Delete(fileName);
            }
        }

        private static void DeleteCustomFile(string filepath, string path, string file)
        {
            File.Delete(filepath + "\\" + file);
            string fileName = filepath + "\\" + path + "\\" + file;
            File.Delete(fileName);
        }

        /// <summary>
        /// Download image file.
        /// </summary>
        /// <param name="filePath">Image url.</param>
        public static bool DownloadImageFile(string filePath, string fileName)
        {
            try
            {
                WebClient l_WebClient = new WebClient();
                byte[] imageBytes = l_WebClient.DownloadData(filePath);

                KeyValuePair<Errors, string> downloadedFileStatus = CheckDownloadedFile(fileName, imageBytes.Length);

                if (downloadedFileStatus.Key == Errors.FileNoError)
                {
                    string fullFileName = Path.Combine(Files.UploadDirectoryPath, listImageFileDimensions.Where(ifd => ifd.ImageKind == ImageFileKind.Original).SingleOrDefault().DirectoryToSave);
                    Files.MakeDirectoryIfNotExists(fullFileName);
                    SaveFileOnDisk(Path.Combine(fullFileName, fileName), imageBytes);
                    return true;
                }
                //else LogManager.LogInFile("Error:" + downloadedFileStatus.Value);

            }
            catch (Exception ex)
            {
                //LogManager.LogInFile("Error:" + ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Check file extension and size.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <param name="fileSize">File size in bytes.</param>
        /// <returns></returns>
        private static KeyValuePair<Errors, string> CheckDownloadedFile(string filePath, int fileSize)
        {
            if (!CheckFileExtension(filePath))
            {
                return (new KeyValuePair<Errors, string>(Errors.FileExtensionError, String.Format("File {0} not supported.", filePath)));
            }

            if (!CheckFileSize(fileSize, MaxFileSize))
            {
                return (new KeyValuePair<Errors, string>(Errors.FileMaxSizeError, String.Format("File {0} with size: {1} over limit.", filePath, fileSize)));
            }

            return (new KeyValuePair<Errors, string>(Errors.FileNoError, String.Format("File {0} downloaded successfully.", filePath)));
        }

        /// <summary>
        /// Save file on disk.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="fileBytes">Bytes of file.</param>
        private static void SaveFileOnDisk(string fileName, byte[] fileBytes)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(fileBytes);
                }
            }
        }

        /// <summary>
        /// Resize file to all defined dimensions.
        /// </summary>
        /// <param name="origFileName">Original file name.</param>
        /// <param name="newFileName">New unique file name.</param>
        /// <param name="imageFileKind">ImageFileKind .</param>
        public static void ResizeImageFilesByKind(string origFileName, string newFileName, ImageFileKind imageFileKind)
        {
            string origFullFileName = Path.Combine(Files.UploadDirectoryPath, listImageFileDimensions.Where(ifd => ifd.ImageKind == ImageFileKind.Original).SingleOrDefault().DirectoryToSave, origFileName);

            using (Bitmap bitmap = (Bitmap)Bitmap.FromFile(origFullFileName))
            {
                foreach (ImageFileDimension ifd in listImageFileDimensions)
                {
                    if (imageFileKind == ifd.ImageKind)
                    {
                        Files.MakeDirectoryIfNotExists(Path.Combine(Files.UploadDirectoryPath, ifd.DirectoryToSave));
                        if (ifd.IsCropped)
                        {
                            ResizeImageFile(bitmap, newFileName, ifd);
                        }
                        else
                        {
                            ResizeImageFileWithDimens(bitmap, newFileName, ifd);
                        }
                    }
                }
            }

            //rename original file name to new file name.
            if (origFileName != newFileName)
            {
                string newFullFileName = Path.Combine(Files.UploadDirectoryPath, listImageFileDimensions.Where(ifd => ifd.ImageKind == ImageFileKind.Original).SingleOrDefault().DirectoryToSave, newFileName);
                FileInfo TheFile = new FileInfo(newFullFileName);
                if (TheFile.Exists)
                {
                    File.Delete(origFullFileName);
                }
                else
                {
                    File.Move(origFullFileName, newFullFileName);
                }
            }
        }

        /// <summary>
        /// Resize image file and save it with new unique file name.
        /// </summary>
        /// <param name="bitmap">Bitmap instance.</param>
        /// <param name="newFileName">Unique file name.</param>
        /// <param name="imageFileDimension">ImageFileDimension instance.</param>     
        public static void ResizeImageFileWithDimens(Bitmap bitmap, string newFileName, ImageFileDimension imageFileDimension)
        {
            int origHeight = bitmap.Height;
            int origWidth = bitmap.Width;

            int newWidth = origWidth;
            int newHeight = origHeight;

            string fullFileName = Path.Combine(Files.UploadDirectoryPath, imageFileDimension.DirectoryToSave, newFileName);
            ImageFormat imageFormat = allowedImageFiles[Path.GetExtension(newFileName)];

            if (imageFileDimension.Width == null && imageFileDimension.Height == null)
            {
                bitmap.Save(fullFileName, imageFormat);
            }
            else
            {
                if (imageFileDimension.Width == null)
                {
                    newHeight = imageFileDimension.Height.Value;

                    if (origHeight > newHeight)
                    {
                        float heightRatio = (float)newHeight / origHeight;
                        newWidth = (int)(origWidth * heightRatio);
                    }
                    else
                        newHeight = origHeight;
                }
                else if (imageFileDimension.Height == null)
                {
                    newWidth = imageFileDimension.Width.Value;

                    if (origWidth > newWidth)
                    {
                        float widthRatio = (float)newWidth / origWidth;
                        newHeight = (int)(origHeight * widthRatio);
                        //SaveResizedImageFile(bitmap, fullFileName, newWidth, newHeight, imageFormat);
                    }
                    else
                        newWidth = origWidth;
                }
                else
                {
                    newHeight = imageFileDimension.Height.Value;
                    newWidth = imageFileDimension.Width.Value;

                    float widthRatio = ((float)newWidth) / origWidth;
                    float heightRatio = ((float)newHeight) / origHeight;

                    float ratio = (widthRatio < heightRatio ? widthRatio : heightRatio);// (widthRatio)MIN(maxWidth / Width, maxHeight / Height);

                    if (ratio < 1.0f)
                    {
                        newWidth = (int)(origWidth * ratio);
                        newHeight = (int)(origHeight * ratio);
                    }
                    else
                    {
                        newWidth = origWidth;
                        newHeight = origHeight;
                    }
                }

                SaveResizedImageFile(bitmap, fullFileName, newWidth, newHeight, imageFormat, null);
            }
        }

        /// <summary>
        /// Resize image file and save it with new unique file name.
        /// </summary>
        /// <param name="bitmap">Bitmap instance.</param>
        /// <param name="newFileName">Unique file name.</param>
        /// <param name="imageFileDimension">ImageFileDimension instance.</param>
        public static void ResizeImageFile(Bitmap bitmap, string newFileName, ImageFileDimension imageFileDimension)
        {
            int origHeight = bitmap.Height;
            int origWidth = bitmap.Width;

            int newWidth = origWidth;
            int newHeight = origHeight;

            int? targetHeight = imageFileDimension.Height;
            int? targetWidth = imageFileDimension.Width;

            string fullFileName = Path.Combine(Files.UploadDirectoryPath, imageFileDimension.DirectoryToSave, newFileName);
            ImageFormat imageFormat = allowedImageFiles[Path.GetExtension(newFileName)];

            if (imageFileDimension.Width == null && imageFileDimension.Height == null)
            {
                bitmap.Save(fullFileName, imageFormat);
            }
            else
            {
                if (imageFileDimension.Width == null)
                {
                    newHeight = imageFileDimension.Height.Value;

                    if (origHeight > newHeight)
                    {
                        float heightRatio = (float)newHeight / origHeight;
                        newWidth = (int)(origWidth * heightRatio);
                    }
                    else
                        newHeight = origHeight;

                    targetWidth = newWidth;
                    targetHeight = newHeight;
                }
                else if (imageFileDimension.Height == null)
                {
                    newWidth = imageFileDimension.Width.Value;

                    if (origWidth > newWidth)
                    {
                        float widthRatio = (float)newWidth / origWidth;
                        newHeight = (int)(origHeight * widthRatio);
                    }
                    else
                        newWidth = origWidth;

                    targetWidth = newWidth;
                    targetHeight = newHeight;
                }
                else
                {
                    newHeight = imageFileDimension.Height.Value;
                    newWidth = imageFileDimension.Width.Value;

                    float widthRatio = ((float)newWidth) / origWidth;
                    float heightRatio = ((float)newHeight) / origHeight;

                    float ratio = (widthRatio > heightRatio ? widthRatio : heightRatio);// (widthRatio)MIN(maxWidth / Width, maxHeight / Height);

                    if (!imageFileDimension.IsCropped)
                    {
                        if (ratio < 1.0f)
                        {
                            newWidth = (int)(origWidth * ratio);
                            newHeight = (int)(origHeight * ratio);
                        }
                        else
                        {
                            targetWidth = newWidth = origWidth;
                            targetHeight = newHeight = origHeight;
                        }
                    }
                    else
                    {
                        newWidth = (int)(origWidth * ratio);
                        newHeight = (int)(origHeight * ratio);
                    }
                }

                Rectangle? rectF = null;
                if (imageFileDimension.IsCropped)
                    rectF = new Rectangle(0, 0, targetWidth.Value, targetHeight.Value);

                SaveResizedImageFile(bitmap, fullFileName, newWidth, newHeight, imageFormat, rectF);
            }
        }

        private static void SaveResizedImageFile(Bitmap bitmap, string fullFileName, int newWidth, int newHeight, ImageFormat imageFormat, Rectangle? cropArea)
        {
            int origHeight = bitmap.Height;
            int origWidth = bitmap.Width;

            using (Bitmap resultBitmap = new Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                resultBitmap.SetResolution(72, 72);
                using (Graphics g = Graphics.FromImage(resultBitmap))
                {
                    SetGraphicsModes(g);

                    using (var attribute = new ImageAttributes())
                    {
                        attribute.SetWrapMode(WrapMode.Tile);
                        g.DrawImage(bitmap, new Rectangle(0, 0, newWidth, newHeight), 0, 0, (origWidth), (origHeight), GraphicsUnit.Pixel, attribute);
                    }
                }

                if (cropArea != null)
                {
                    try
                    {
                        Rectangle r = new Rectangle(cropArea.Value.X, cropArea.Value.Y, cropArea.Value.Width, cropArea.Value.Height);
                        if (cropArea.Value.Height > newHeight)
                            r.Height = newHeight;

                        if (cropArea.Value.Width > newWidth)
                            r.Width = newWidth;

                        Bitmap resultBitmap1 = resultBitmap.Clone(r, resultBitmap.PixelFormat);
                        {
                            if (imageFormat == ImageFormat.Jpeg)
                            {
                                resultBitmap1.Save(fullFileName, GetEncoderInfo("image/jpeg"), GetJpegEncoderParams());
                            }
                            else
                                resultBitmap1.Save(fullFileName, imageFormat);
                        }
                        resultBitmap1.Dispose();
                    }
                    catch //(Exception ex)
                    {

                    }
                }
                else
                {
                    //resultBitmap.MakeTransparent();
                    if (imageFormat == ImageFormat.Jpeg)
                    {
                        resultBitmap.Save(fullFileName, GetEncoderInfo("image/jpeg"), GetJpegEncoderParams());
                    }
                    else
                        resultBitmap.Save(fullFileName, imageFormat);
                }
            }
        }

        private static void SetGraphicsModes(Graphics graphics)
        {
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
        }

        private static EncoderParameters GetJpegEncoderParams()
        {
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L); //You can use 100L to increase or decrease the quality of the image.
            myEncoderParameters.Param[0] = myEncoderParameter;
            return myEncoderParameters;
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

            for (int j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                {
                    return encoders[j];
                }
            }

            return null;
        }

        /// <summary>
        /// Get Format By File Extension
        /// </summary>
        /// <param name="filename"> File name.</param>
        /// <returns>Image format.</returns>
        public ImageFormat GetFileFormatByExtension(string filename)
        {
            ImageFormat iformat = allowedImageFiles[Path.GetExtension(filename)];
            if (iformat == ImageFormat.Tiff)
            {
                iformat = ImageFormat.Jpeg;
            }
            return iformat;
        }

        /// <summary>
        /// Check uploaded file extension and size.
        /// </summary>
        /// <param name="uploadedFile">File upload content.</param>
        /// <returns></returns>
        public static KeyValuePair<Errors, string> CheckUploadedFile(HttpPostedFile uploadedFile, int maxUploadFileSizeInMB)
        {
            return CheckUploadedFile(uploadedFile, maxUploadFileSizeInMB, 0, 0);
        }

        /// <summary>
        /// Check uploaded file extension and size.
        /// </summary>
        /// <param name="uploadedFile">File upload content.</param>
        /// <returns></returns>
        public static KeyValuePair<Errors, string> CheckUploadedFile(HttpPostedFile uploadedFile, int maxUploadFileSizeInMB, int minUploadFileWidth, int minUploadFileHeight)
        {
            if (!CheckFileExtension(uploadedFile.FileName))
            {
                return (new KeyValuePair<Errors, string>(Errors.FileExtensionError, null));
            }

            int maxSizeInBytes = 1024 * 1024 * maxUploadFileSizeInMB;
            if (!CheckFileSize(uploadedFile.ContentLength, maxSizeInBytes))
            {
                return (new KeyValuePair<Errors, string>(Errors.FileMaxSizeError, null));
            }

            if ((minUploadFileWidth != 0 || minUploadFileHeight != 0) && !CheckImageResolution(uploadedFile, minUploadFileWidth, minUploadFileHeight))
            {
                return (new KeyValuePair<Errors, string>(Errors.FileMinResolutionError, null));
            }

            return (new KeyValuePair<Errors, string>(Errors.FileNoError, null));
        }

        /// <summary>
        /// Resize uploaded file to all defined dimensions.
        /// </summary>
        /// <param name="origFileName">File upload content.</param>
        /// <param name="newFileName">New unique file name.</param>
        /// <param name="imageFileKind">ImageFileKind .</param>
        public static void ResizeImageFilesByKindOnUpload(HttpPostedFile uploadedFile, string newFileName, ImageFileKind imageFileKind, bool isThumbListItem = false)
        {
            UploadImageFile(uploadedFile, newFileName, ImageFileKind.Original);

            using (Bitmap bitmap = (Bitmap)Bitmap.FromStream(uploadedFile.InputStream))
            {
                foreach (ImageFileDimension ifd in listImageFileDimensions)
                {
                    if (imageFileKind == ifd.ImageKind)
                    {
                        Files.MakeDirectoryIfNotExists(Path.Combine(Files.UploadDirectoryPath, ifd.DirectoryToSave));
                        if (ifd.IsCropped)
                            ResizeImageFile(bitmap, newFileName, ifd);
                        else
                            ResizeImageFileWithDimens(bitmap, newFileName, ifd);
                    }
                }
            }
        }

        public static void UploadImageFile(HttpPostedFile uploadedFile, string newFileName, ImageFileKind fileKind)
        {
            Files.MakeDirectoryIfNotExists(Path.Combine(Files.UploadDirectoryPath, listImageFileDimensions.Where(ifd => ifd.ImageKind == fileKind).SingleOrDefault().DirectoryToSave));
            uploadedFile.SaveAs(Path.Combine(Files.UploadDirectoryPath, listImageFileDimensions.Where(ifd => ifd.ImageKind == fileKind).SingleOrDefault().DirectoryToSave, newFileName));
        }

        /// <summary>
        /// Delete all file by selected kind.
        /// </summary>
        /// <param name="filename">File name.</param>
        /// <param name="imageFileKind">Image file kind.</param>
        /// <param name="deleteOriginal">Delete original file</param>
        public static void DeleteImageFileByKind(string filename, ImageFileKind imageFileKind, bool deleteOriginal = true)
        {
            // delete from original 
            if (deleteOriginal)
            {
                DeleteFile(Path.Combine(Files.UploadDirectoryPath, listImageFileDimensions.Where(ifd => ifd.ImageKind == ImageFileKind.Original).SingleOrDefault().DirectoryToSave, filename));
            }

            //delete from linked directory 
            foreach (ImageFileDimension ifd in listImageFileDimensions)
            {
                if (imageFileKind == ifd.ImageKind)
                {
                    DeleteFile(Path.Combine(Files.UploadDirectoryPath, ifd.DirectoryToSave, filename));
                }
            }
        }

        /// <summary>
        /// Delete file if exists.
        /// </summary>
        /// <param name="fullPathName">Full path name.</param>
        private static void DeleteFile(string fullPathName)
        {
            if (File.Exists(fullPathName))
            {
                File.Delete(fullPathName);
            }
        }

        public static void DeleteTempFiles(List<string> fileNames)
        {
            foreach (string file in fileNames)
            {
                DeleteImageFileByKind(file, ImageFileKind.Temp, false);
            }
        }

        /// <summary>
        /// Get directory path by image kind.
        /// </summary>
        /// <param name="imageFileKind"></param>
        /// <param name="includeFullPath"></param>
        /// <returns></returns>
        public static string GetDirectoryToSaveWebPath(string directory)
        {
            ImageFileDimension dimensions = listImageFileDimensions.Where(ifd => ifd.DirectoryToSave == directory).FirstOrDefault();
            if (dimensions != null)
            {
                return Path.Combine(Files.uploadFileDirectory, dimensions.DirectoryToSave);
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Get imageFileDimension by save directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static ImageFileDimension GetImageFileDimensionBySaveDirectory(string directory)
        {
            return listImageFileDimensions.Where(ifd => ifd.DirectoryToSave == directory).FirstOrDefault();
        }

        /// <summary>
        /// Send image thumbnail as stream.
        /// </summary>
        /// <param name="stream">Response stream.</param>
        /// <param name="filename">File name.</param>
        /// <param name="imageFileDimension">Image dimensions.</param>
        [Obsolete]
        public static void SendImageThumbnailAsStream(Stream stream, string filename, ImageFileKind imageFileKind)
        {
            string origFullFileName = Path.Combine(Files.UploadDirectoryPath, listImageFileDimensions.Where(ifd => ifd.ImageKind == ImageFileKind.Original).SingleOrDefault().DirectoryToSave, filename);

            ImageFileDimension imageFileDimension = listImageFileDimensions.Where(ifd => ifd.ImageKind == imageFileKind).FirstOrDefault();

            using (Image bitmap = Bitmap.FromFile(origFullFileName))
            {
                int origHeight = bitmap.Height;
                int origWidth = bitmap.Width;

                int newHeight = imageFileDimension.Height.Value;
                int newWidth = imageFileDimension.Width.Value;

                float widthRatio = ((float)newWidth) / origWidth;
                float heightRatio = ((float)newHeight) / origHeight;

                float ratio = (widthRatio < heightRatio ? widthRatio : heightRatio);// (widthRatio)MIN(maxWidth / Width, maxHeight / Height);

                if (ratio >= 1.0f)
                {
                    bitmap.Save(stream, allowedImageFiles[Path.GetExtension(origFullFileName)]);
                }
                else
                {
                    using (Image resultBitmap = bitmap.GetThumbnailImage((int)(origWidth * ratio), (int)(origHeight * ratio), null, IntPtr.Zero))
                    {
                        resultBitmap.Save(stream, allowedImageFiles[Path.GetExtension(origFullFileName)]);
                    }
                }
            }

        }

        public static string ByteArrayToString(byte[] input)
        {
            UTF8Encoding enc = new UTF8Encoding();
            string str = enc.GetString(input);
            return str;
        }

        /*private static void ResizeImageFile1(string fileName, ImageFileDimension imageFile)
          {
              string fullFileName = Files.UploadDirectoryPath + "\\" + fileName;

              using (Bitmap originalBitmap = Bitmap.FromFile(fullFileName, true) as Bitmap, newbmp = new Bitmap(imageFile.Width, imageFile.Height))
              {
                  double WidthVsHeightRatio = (float)originalBitmap.Width / (float)originalBitmap.Height;

                  using (Graphics newg = Graphics.FromImage(newbmp))
                  {
                      newg.Clear(Color.Transparent);

                      newg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                      if (WidthVsHeightRatio == 1d)
                      {
                          newg.DrawImage(originalBitmap, 0, 0, imageFile.Width, imageFile.Height);
                          newg.Save();
                      }
                      else if (WidthVsHeightRatio < 1d) //Image is taller than wider
                      {
                          newg.DrawImage(originalBitmap, new RectangleF(new PointF((float)(imageFile.Width / 2 - ((imageFile.Height * WidthVsHeightRatio) / 2)), 0), new SizeF((float)(imageFile.Width * WidthVsHeightRatio), (float)imageFile.Height)));
                          newg.Save();
                      }
                      else //Image is wider than taller
                      {
                          double inverse = Math.Pow(WidthVsHeightRatio, -1);
                          RectangleF rectF = new RectangleF(new PointF(0, (float)(imageFile.Height / 2 - ((imageFile.Width * inverse) / 2))), new SizeF((float)imageFile.Width, (float)(imageFile.Height * inverse)));
                        
                          newg.DrawImage(originalBitmap, rectF);
                          newg.Save();
                      }
                  }

                  newbmp.Save(Files.UploadDirectoryPath + "\\" + "1_1_1.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
              }
          }
         
              string fullFileName = Files.UploadDirectoryPath + "\\" + fileName;

                  Int32 newWidth = imageFileDimension.Width;
                  Int32 newHeight = imageFileDimension.Height;

                  //Use the uploaded filename for saving without the '.' extension 

                  //String upName = FileUpload1.FileName.Substring(0, FileUpload1.FileName.IndexOf("."));

                  //Set the save path of the resized image, you will need this directory already created in your web site 

                  //string filePath = "~/Upload/" + upName + ".jpg";

                  //Create a new Bitmap using the uploaded picture as a Stream

                  //Set the new bitmap resolution to 72 pixels per inch 

                  Bitmap upBmp = (Bitmap)Bitmap.FromFile(fullFileName) ;

                  Bitmap newBmp = new Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                  newBmp.SetResolution(72, 72);

                  //Get the uploaded image Width and Height 
                  Double upWidth = upBmp.Width;
                  Double upHeight = upBmp.Height;
                  int newX = 0;

                  //Set the new top left drawing position on the image canvas 
                  int newY = 0;
                  Double reDuce;

                  //Keep the aspect ratio of image the same if not 4:3 and work out the newX and newY positions

                  //to ensure the image is always in the centre of the canvas vertically and horizontally 
                  if (upWidth > upHeight)
                  {
                      //Landscape picture 
                      reDuce = newWidth / upWidth;

                      //calculate the Width percentage reduction as decimal 
                      newHeight = ((Int32)(upHeight * reDuce));

                      //reduce the uploaded image Height by the reduce amount 
                      newY = ((Int32)((imageFileDimension.Height - newHeight) / 2));

                      //Position the image centrally down the canvas 
                      newX = 0;
                      //Picture will be full Width 
                  }
                  else if (upWidth < upHeight)
                  {
                      //Portrait picture 
                      reDuce = newHeight / upHeight;

                      //calculate the Height percentage reduction as decimal 
                      newWidth = ((Int32)(upWidth * reDuce));

                      //reduce the uploaded image Height by the reduce amount 
                      newX = ((Int32)((imageFileDimension.Width - newWidth) / 2));

                      //Position the image centrally across the canvas 
                      newY = 0;

                      //Picture will be full hieght 
                  }
                  else if (upWidth == upHeight)
                  {
                      //square picture 
                      reDuce = newHeight / upHeight;

                      //calculate the Height percentage reduction as decimal 
                      newWidth = ((Int32)(upWidth * reDuce));


                      //reduce the uploaded image Height by the reduce amount 
                      newX = ((Int32)((imageFileDimension.Width - newWidth) / 2));
                      //Position the image centrally across the canvas 

                      newY = ((Int32)((imageFileDimension.Height - newHeight) / 2));
                      //Position the image centrally down the canvas 
                  }

                  //Create a new image from the uploaded picture using the Graphics class

                  //Clear the graphic and set the background colour to white

                  //Use Antialias and High Quality Bicubic to maintain a good quality picture

                  //Save the new bitmap image using 'Png' picture format and the calculated canvas positioning 
                  Graphics newGraphic = Graphics.FromImage(newBmp);

                  try
                  {
                      newGraphic.Clear(Color.White);

                      newGraphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                      newGraphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                      newGraphic.DrawImage(upBmp, newX, newY, newWidth, newHeight);

                      newBmp.Save(Files.UploadDirectoryPath + "\\" + "1_1_1.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);                    
                  }
                  catch (Exception ex)
                  {
                      string newError = ex.Message;
                     // lblError.Text = newError;
                  }
                  finally
                  {
                      upBmp.Dispose();
                      newBmp.Dispose();
                      newGraphic.Dispose();
                  }
              }
        */
    }
}

