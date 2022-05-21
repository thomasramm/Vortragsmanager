using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.Module
{
    internal static class Photo
    {
        private static void SaveToFile(BitmapSource bitmapSource, string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || bitmapSource == null) 
                return;

            //creating frame and putting it to Frames collection of selected encoder
            var frame = BitmapFrame.Create(bitmapSource);
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(frame);
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    encoder.Save(fs);
                }
            }
            catch
            {
                Log.Error($"Fehler beim export des Foto {fileName}");
            }
        }

        public static void SaveToFile(string folder)
        {
            foreach (var speaker in DataContainer.Redner.Where(x => x.Foto != null))
            {
                var file = $"{speaker.Name} ({speaker.Versammlung.Name}).jpg";
                file = string.Concat(file.Split(Path.GetInvalidFileNameChars()));
                var fullName = Path.Combine(folder, file);
                SaveToFile(speaker.Foto, fullName);
            }
        }

        private static BitmapSource LoadFromFile(string path)
        {
            var rotation = Rotation.Rotate0;
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var bitmapFrame = BitmapFrame.Create(fileStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);

                const string orientationQuery = "System.Photo.Orientation";
                if ((bitmapFrame.Metadata is BitmapMetadata bitmapMetadata) && (bitmapMetadata.ContainsQuery(orientationQuery)))
                {
                    var o = bitmapMetadata.GetQuery(orientationQuery);

                    if (o != null)
                    {
                        switch ((ushort)o)
                        {
                            case 6:
                            {
                                rotation = Rotation.Rotate90;
                            }
                                break;
                            case 3:
                            {
                                rotation = Rotation.Rotate180;
                            }
                                break;
                            case 8:
                            {
                                rotation = Rotation.Rotate270;
                            }
                                break;
                        }
                    }
                }
            }

            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(path);
            image.Rotation = rotation;
            image.EndInit();
            image.Freeze();

            return image;
        }

        public static BitmapSource LoadFromFile()
        {
            var op = new OpenFileDialog();
            op.Title = "Wähle ein Foto aus";
            op.Filter = "Foto|*.jpg;*.jpeg;*.png|" +
                        "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                        "Portable Network Graphic (*.png)|*.png|"+
                        "Alle Dateien (*.*)|*.*";

            if (op.ShowDialog() == DialogResult.OK)
            {
                var img = LoadFromFile(op.FileName);
                var scale = CalculateFactor(img.PixelWidth, img.PixelHeight);
                var targetBitmap = new TransformedBitmap(img, new ScaleTransform(scale, scale));
                return targetBitmap;
            }
            return null;
        }

        private static double CalculateFactor(int width, int height)
        {
            var size = Math.Max(width, height);
            var faktor = 500d / size;
            return Math.Min(faktor, 1);
        }
        
        public static byte[] ConvertBitmapToByteArray<T>(BitmapSource bitmapSource) where T : BitmapEncoder, new()
        {
            if (bitmapSource == null)
                return null;

            //creating frame and putting it to Frames collection of selected encoder
            var frame = BitmapFrame.Create(bitmapSource);
            var encoder = new T();
            encoder.Frames.Add(frame);
            try
            {
                using (var fs = new MemoryStream())
                {
                    encoder.Save(fs);
                    var imageBytes = fs.ToArray();
                    return imageBytes;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static BitmapSource ConvertByteArrayToBitmap(byte[] imageBytes)
        {
            if (imageBytes == null)
                return null;

            //MemoryStream stream = new MemoryStream(imageBytes);
            Stream imageStremSource = new MemoryStream(imageBytes, false);
            JpegBitmapDecoder decoder = new JpegBitmapDecoder(imageStremSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            BitmapSource bitmapsources = decoder.Frames[0];
            imageStremSource.Close();
            return bitmapsources;
 
            //// Convert byte[] to Image
            //MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            //ms.Write(imageBytes, 0, imageBytes.Length);


            //var image = new Bitmap(ms);
            //return image;


            ////creating frame and putting it to Frames collection of selected encoder
            //var frame = BitmapFrame.Create(bitmapSource);
            //var encoder = new T();
            //encoder.Frames.Add(frame);
            //try
            //{
            //    using (var fs = new MemoryStream())
            //    {
            //        encoder.Save(fs);
            //        var imageBytes = fs.ToArray();
            //        return imageBytes;
            //    }
            //}
            //catch (Exception e)
            //{
            //    return null;
            //}
        }
    }
}
