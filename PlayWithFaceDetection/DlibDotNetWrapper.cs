using System;
using System.Collections.Generic;
using System.Text;
using DlibDotNet;
using DlibDotNet.Extensions;
using Dlib = DlibDotNet.Dlib;

namespace PlayWithFaceDetection
{
    /// <summary>
    /// https://github.com/takuya-takeuchi/DlibDotNet/
    /// </summary>
    public static class DlibDotNetWrapper
    {
        public static void DetectFacesOnImage(string sourceImagePath, string destImagePath)
        {
            // set up Dlib facedetector
            using (var fd = Dlib.GetFrontalFaceDetector())
            {
                // load input image
                var image = Dlib.LoadImage<RgbPixel>(sourceImagePath);

                DetectFacesOnImage(image);

                // export the modified image
                Dlib.SaveJpeg(image, destImagePath);
            }
        }

        private static void DetectFacesOnImage(Array2D<RgbPixel> image)
        {
            // set up Dlib facedetector
            using (var fd = Dlib.GetFrontalFaceDetector())
            {
                // find all faces in the image
                var faces = fd.Operator(image);
                foreach (Rectangle face in faces)
                {
                    // draw a rectangle for each face
                    Dlib.DrawRectangle(image, face, color: new RgbPixel(0, 255, 255), thickness: 4);
                }
            }
        }
    }
}
