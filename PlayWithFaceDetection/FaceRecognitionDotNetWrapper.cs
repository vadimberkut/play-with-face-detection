using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FaceRecognitionDotNet;
using DlibDotNet;
using System.IO.Compression;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing.Imaging;
using PlayWithFaceDetection.FaceRecognitionDotNet.HelenTraining;
using System.Drawing;

namespace PlayWithFaceDetection
{
    /// <summary>
    /// You must install these packages:
    /// - FaceRecognitionDotNet 
    /// Models must be trained manually (Helen Dataset)
    /// https://github.com/takuya-takeuchi/FaceRecognitionDotNet
    /// </summary>
    public static class FaceRecognitionDotNetWrapper
    {
        /// <summary>
        /// The first step of model training: read Helen dtaset and generate XML file
        /// </summary>
        /// <param name="helenTrainingDatasetDirPath"></param>
        /// <param name="modelDirPath"></param>
        /// <param name="detectedFaceAreaPadding"></param>
        public static int GenerateXmlFile(string helenTrainingDatasetDirPath, string modelDirPath, int detectedFaceAreaPadding = 50)
        {
            // reassign params to avoid renaming
            string directory = modelDirPath; // where put result model
            int padding = detectedFaceAreaPadding;
            string extractPath = helenTrainingDatasetDirPath; // where Helen dataset is located

            Directory.CreateDirectory(directory);
            Directory.CreateDirectory(extractPath);

            // TODO: it loads model files, but they aren't henerated yet. Strange.
            FaceRecognition _FaceRecognition = FaceRecognition.Create(directory);

            // const string extractPath = "helen";
            var zips = new[]
            {
                new{ Zip = "annotation.zip", IsImage = false, Directory = "annotation" },
                new{ Zip = "helen_1.zip",    IsImage = true,  Directory = "helen_1" },
                new{ Zip = "helen_2.zip",    IsImage = true,  Directory = "helen_2" },
                new{ Zip = "helen_3.zip",    IsImage = true,  Directory = "helen_3" },
                new{ Zip = "helen_4.zip",    IsImage = true,  Directory = "helen_4" },
                new{ Zip = "helen_5.zip",    IsImage = true,  Directory = "helen_5" }
            };

            Directory.CreateDirectory(extractPath);

            foreach (var zip in zips)
            {
                if (!Directory.Exists(Path.Combine(extractPath, zip.Directory)))
                    ZipFile.ExtractToDirectory(zip.Zip, extractPath);
            }

            var annotation = zips.FirstOrDefault(arg => !arg.IsImage);
            var imageZips = zips.Where(arg => arg.IsImage).ToArray();
            if (annotation == null)
                return -1;

            var images = new List<PlayWithFaceDetection.FaceRecognitionDotNet.HelenTraining.Image>();
            foreach (var file in Directory.EnumerateFiles(Path.Combine(extractPath, annotation.Directory)))
            {
                Console.WriteLine($"Process: '{file}'");

                var txt = File.ReadAllLines(file);
                var filename = txt[0];
                var jpg = $"{filename}.jpg";
                foreach (var imageZip in imageZips)
                {
                    var found = false;
                    var path = Path.Combine(Path.Combine(extractPath, imageZip.Directory, jpg));
                    if (File.Exists(path))
                    {
                        found = true;
                        using (var fi = FaceRecognition.LoadImageFile(path))
                        {
                            var locations = _FaceRecognition.FaceLocations(fi, 1, Model.Hog).ToArray();
                            if (locations.Length != 1)
                            {
                                Console.WriteLine($"\t'{path}' has {locations.Length} faces.");
                            }
                            else
                            {
                                var location = locations.First();
                                var parts = new List<Part>();
                                for (var i = 1; i < txt.Length; i++)
                                {
                                    var tmp = txt[i].Split(',').Select(s => s.Trim()).Select(float.Parse).Select(s => (int)s).ToArray();
                                    parts.Add(new Part { X = tmp[0], Y = tmp[1], Name = $"{i - 1}" });
                                }

                                var image = new PlayWithFaceDetection.FaceRecognitionDotNet.HelenTraining.Image
                                {
                                    File = Path.Combine(imageZip.Directory, jpg),
                                    Box = new Box
                                    {
                                        Left = location.Left - padding,
                                        Top = location.Top - padding,
                                        Width = location.Right - location.Left + 1 + padding * 2,
                                        Height = location.Bottom - location.Top + 1 + padding * 2,
                                        Part = parts.ToArray()
                                    }
                                };

                                using (var bitmap = System.Drawing.Image.FromFile(path))
                                {
                                    var b = image.Box;
                                    using (var g = Graphics.FromImage(bitmap))
                                    {
                                        using (var p = new Pen(Color.Red, bitmap.Width / 400f))
                                            g.DrawRectangle(p, b.Left, b.Top, b.Width, b.Height);

                                        foreach (var part in b.Part)
                                            g.FillEllipse(Brushes.GreenYellow, part.X, part.Y, 5, 5);
                                    }

                                    var result = Path.Combine(extractPath, "Result");
                                    Directory.CreateDirectory(result);

                                    bitmap.Save(Path.Combine(result, jpg), ImageFormat.Jpeg);
                                }

                                images.Add(image);
                            }
                        }
                    }

                    if (found)
                        break;
                }
            }

            var dataset = new Dataset
            {
                Name = "helen dataset",
                Comment = "Created by Takuya Takeuchi.",
                Images = images.ToArray()
            };

            var settings = new XmlWriterSettings();
            using (var sw = new StreamWriter(Path.Combine(extractPath, "helen-dataset.xml"), false, new System.Text.UTF8Encoding(false)))
            using (var writer = XmlWriter.Create(sw, settings))
            {
                writer.WriteProcessingInstruction("xml-stylesheet", @"type=""text/xsl"" href=""image_metadata_stylesheet.xsl""");
                var serializer = new XmlSerializer(typeof(Dataset));
                serializer.Serialize(writer, dataset);
            }

            return 0;
        }

        public static void DetectFacesOnImage(string sourceImagePath, string destImagePath)
        {
            //string modelsDirPath = "";
            //FaceRecognition.Create(directory);
        }
    }
}
