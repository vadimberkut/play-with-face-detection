using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace PlayWithFaceDetection
{
    class Program
    {
        static void Main(string[] args)
        {
            DotNetEnv.Env.Load("./.env");

            const string inputDirPath = "./input-images";
            const string outputDirPath = "./output";

            Directory.CreateDirectory(inputDirPath);
            Directory.CreateDirectory(outputDirPath);

            // load input image
            var imageNames = Directory.GetFiles(inputDirPath).ToList();

            // Microsoft Face API
            // TODO: move to env if publish to GitHub
            string FACE_ENDPOINT = Environment.GetEnvironmentVariable("FACE_ENDPOINT");
            string FACE_SUBSCRIPTION_KEY = Environment.GetEnvironmentVariable("FACE_SUBSCRIPTION_KEY");
            MicrosoftFaceApiWrapper microsoftFaceApiWrapper = new MicrosoftFaceApiWrapper(FACE_ENDPOINT, FACE_SUBSCRIPTION_KEY);

            for (int i = 0; i < imageNames.Count; i++)
            {
                //// DlibDotNet
                //DlibDotNetWrapper.DetectFacesOnImage(imageNames[i], Path.Combine(outputDirPath, $"output_{i + 1}_1dlibdotnet.jpg"));

                //// OpenCvSharp4
                //OpenCvSharpWrapper.DetectFacesOnImage(imageNames[i], Path.Combine(outputDirPath, $"output_{i + 1}_2opencvsharp-haar.jpg"));

                // FaceRecognitionDotNet
                // 1. Generate XML file
                // 2. Train model on Helen dataset
                int result1 = FaceRecognitionDotNetWrapper.GenerateXmlFile("./FaceRecognitionDotNet/HelenTraining", "./FaceRecognitionDotNet/HelenTraining/Models");
                if(result1 != 0)
                {
                    throw new Exception("Error generating XML file.");
                }

                //// Microsoft Face API
                //microsoftFaceApiWrapper.DetectFacesOnImage(imageNames[i], Path.Combine(outputDirPath, $"output_{i + 1}_3microsoftfaceapi.jpg"));
                //Thread.Sleep(500); // throttle
            }
        }
    }
}
