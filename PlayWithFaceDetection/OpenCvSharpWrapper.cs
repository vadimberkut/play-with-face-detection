﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenCvSharp;

namespace PlayWithFaceDetection
{
    /// <summary>
    /// You must install these packages:
    /// - OpenCvSharp4
    /// - OpenCvSharp4.runtime.win (or OpenCvSharp4.runtime.ubuntu.18.04-x64)
    /// https://github.com/shimat/opencvsharp/
    /// https://github.hsmw.workers.dev/VahidN/OpenCVSharp-Samples
    /// </summary>
    public static class OpenCvSharpWrapper
    {
        public static void DetectFacesOnImage(string sourceImagePath, string destImagePath)
        {
            var srcImage = new Mat(sourceImagePath);
            // Cv2.ImShow("Source", srcImage);
            Cv2.WaitKey(1); // do events

            var grayImage = new Mat();
            Cv2.CvtColor(srcImage, grayImage, ColorConversionCodes.BGRA2GRAY);
            Cv2.EqualizeHist(grayImage, grayImage);

            var cascade = new CascadeClassifier(@"./OpenCvSharp/haarcascade_frontalface_alt.xml");
            var nestedCascade = new CascadeClassifier(@"./OpenCvSharp/haarcascade_eye_tree_eyeglasses.xml");

            var faces = cascade.DetectMultiScale(
                image: grayImage,
                scaleFactor: 1.1,
                minNeighbors: 2,
                flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                minSize: new Size(30, 30)
            );

            Console.WriteLine("Detected faces: {0}", faces.Length);

            // Get accessories of the faces
            var rnd = new Random();
            var count = 1;
            foreach (var faceRect in faces)
            {
                var detectedFaceImage = new Mat(srcImage, faceRect);
                // Cv2.ImShow(string.Format("Face {0}", count), detectedFaceImage);
                Cv2.WaitKey(1); // do events

                // Draw rectangle arounf face
                var color = Scalar.FromRgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                Cv2.Rectangle(srcImage, faceRect, color, 2);

                // Get gray image
                var detectedFaceGrayImage = new Mat();
                Cv2.CvtColor(detectedFaceImage, detectedFaceGrayImage, ColorConversionCodes.BGRA2GRAY);
                
                var nestedObjects = nestedCascade.DetectMultiScale(
                    image: detectedFaceGrayImage,
                    scaleFactor: 1.1,
                    minNeighbors: 2,
                    flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                    minSize: new Size(30, 30)
                );

                Console.WriteLine("Nested Objects[{0}]: {1}", count, nestedObjects.Length);

                // Draw circle around accessory
                //foreach (var nestedObject in nestedObjects)
                //{
                //    var center = new Point
                //    {
                //        X = (int)(Math.Round(nestedObject.X + nestedObject.Width * 0.5, MidpointRounding.ToEven) + faceRect.Left),
                //        Y = (int)(Math.Round(nestedObject.Y + nestedObject.Height * 0.5, MidpointRounding.ToEven) + faceRect.Top)
                //    };
                //    var radius = Math.Round((nestedObject.Width + nestedObject.Height) * 0.25, MidpointRounding.ToEven);
                //    Cv2.Circle(srcImage, center, (int)radius, color, thickness: 3);
                //}

                count++;
            }

            // Cv2.ImShow("Haar Detection", srcImage);
            Cv2.WaitKey(1); // do events

            // Save result image
            srcImage.SaveImage(destImagePath);

            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
            srcImage.Dispose();
        }
    }
}
