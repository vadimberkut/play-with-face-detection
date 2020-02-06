using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using OpenCvSharp;

namespace PlayWithFaceDetection
{
    /// <summary>
    /// Create Computer Vision or Face resource on portal - https://docs.microsoft.com/en-us/azure/cognitive-services/cognitive-services-apis-create-account?tabs=singleservice%2Cwindows
    /// F0 is free 20 calls per minutes, 30000 per month
    /// </summary>
    public class MicrosoftFaceApiWrapper
    {
        private readonly string FACE_ENDPOINT;
        private readonly string FACE_SUBSCRIPTION_KEY;

        private IFaceClient _client = null;
        private IFaceClient client { 
            get
            {
                return _client ?? GetClient(FACE_ENDPOINT, FACE_SUBSCRIPTION_KEY);
            } 
        }

        public MicrosoftFaceApiWrapper(string FACE_ENDPOINT, string FACE_SUBSCRIPTION_KEY)
        {
            this.FACE_ENDPOINT = FACE_ENDPOINT;
            this.FACE_SUBSCRIPTION_KEY = FACE_SUBSCRIPTION_KEY;
        }

        private IFaceClient GetClient(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }

        public void DetectFacesOnImage(string sourceImagePath, string destImagePath)
        {
            // Used in the Detect Faces and Verify examples.
            // Recognition model 2 is used for feature extraction, use 1 to simply recognize/detect a face. 
            // However, the API calls to Detection that are used with Verify, Find Similar, or Identify must share the same recognition model.
            const string RECOGNITION_MODEL1 = RecognitionModel.Recognition01;
            const string RECOGNITION_MODEL2 = RecognitionModel.Recognition02;

            // Detect - get features from faces.
            DetectFaceExtract(client, RECOGNITION_MODEL1, sourceImagePath, destImagePath).Wait();
        }
       

        /* 
         * DETECT FACES
         * Detects features from faces and IDs them.
         */
        public async Task DetectFaceExtract(IFaceClient client, string recognitionModel, string sourceImagePath, string destImagePath)
        {
            Console.WriteLine("========DETECT FACES========");
            Console.WriteLine();

            IList<DetectedFace> detectedFaces;

            //// Detect faces with all attributes from image url.
            //detectedFaces = await client.Face.DetectWithUrlAsync(
            //    url: $"{url}{imageFileName}",
            //    returnFaceAttributes: new List<FaceAttributeType> {
            //        FaceAttributeType.Accessories,
            //        FaceAttributeType.Age,
            //        FaceAttributeType.Blur,
            //        FaceAttributeType.Emotion,
            //        FaceAttributeType.Exposure,
            //        FaceAttributeType.FacialHair,
            //        FaceAttributeType.Gender,
            //        FaceAttributeType.Glasses,
            //        FaceAttributeType.Hair,
            //        FaceAttributeType.HeadPose,
            //        FaceAttributeType.Makeup,
            //        FaceAttributeType.Noise,
            //        FaceAttributeType.Occlusion,
            //        FaceAttributeType.Smile
            //    },
            //    recognitionModel: recognitionModel
            //);

            // Detect faces with all attributes from image file.

            try
            {
                using(Stream imageFileStream = File.OpenRead(sourceImagePath))
                {
                    detectedFaces = await client.Face.DetectWithStreamAsync(
                        image: imageFileStream,
                        returnFaceId: true,
                        returnFaceLandmarks: false,
                        returnFaceAttributes: new List<FaceAttributeType> {
                            FaceAttributeType.Accessories,
                            FaceAttributeType.Age,
                            FaceAttributeType.Blur,
                            FaceAttributeType.Emotion,
                            FaceAttributeType.Exposure,
                            FaceAttributeType.FacialHair,
                            FaceAttributeType.Gender,
                            FaceAttributeType.Glasses,
                            FaceAttributeType.Hair,
                            FaceAttributeType.HeadPose,
                            FaceAttributeType.Makeup,
                            FaceAttributeType.Noise,
                            FaceAttributeType.Occlusion,
                            FaceAttributeType.Smile
                        },
                        recognitionModel: recognitionModel
                    );
                }
            }
            // Catch and display Face API errors.
            catch (APIErrorException ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
            // Catch and display all other errors.
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

            Console.WriteLine($"{detectedFaces.Count} face(s) detected from image `{sourceImagePath}`.");

            // Draw bounding box of the faces (using OpenCvSharp)
            var srcImage = new Mat(sourceImagePath);
            var rnd = new Random();

            foreach (var face in detectedFaces)
            {
                Console.WriteLine($"Face attributes for {sourceImagePath}:");

                // Get bounding box of the faces
                Console.WriteLine($"Rectangle(Left/Top/Width/Height) : {face.FaceRectangle.Left} {face.FaceRectangle.Top} {face.FaceRectangle.Width} {face.FaceRectangle.Height}");

                // Draw rectangle arounf face (using OpenCvSharp)
                var color = Scalar.FromRgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                OpenCvSharp.Rect rect = new Rect(face.FaceRectangle.Left, face.FaceRectangle.Top, face.FaceRectangle.Width, face.FaceRectangle.Height);
                Cv2.Rectangle(srcImage, rect, color, 2);
            }
            
            // Parse and print all attributes of each detected face.
            foreach (var face in detectedFaces)
            {
                Console.WriteLine($"Face attributes for {sourceImagePath}:");

                //// Get accessories of the faces
                //List<Accessory> accessoriesList = (List<Accessory>)face.FaceAttributes.Accessories;
                //int count = face.FaceAttributes.Accessories.Count;
                //string accessory; string[] accessoryArray = new string[count];
                //if (count == 0) { accessory = "NoAccessories"; }
                //else
                //{
                //    for (int i = 0; i < count; ++i) { accessoryArray[i] = accessoriesList[i].Type.ToString(); }
                //    accessory = string.Join(",", accessoryArray);
                //}
                //Console.WriteLine($"Accessories : {accessory}");

                //// Get face other attributes
                //Console.WriteLine($"Age : {face.FaceAttributes.Age}");
                //Console.WriteLine($"Blur : {face.FaceAttributes.Blur.BlurLevel}");

                //// Get emotion on the face
                //string emotionType = string.Empty;
                //double emotionValue = 0.0;
                //Emotion emotion = face.FaceAttributes.Emotion;
                //if (emotion.Anger > emotionValue) { emotionValue = emotion.Anger; emotionType = "Anger"; }
                //if (emotion.Contempt > emotionValue) { emotionValue = emotion.Contempt; emotionType = "Contempt"; }
                //if (emotion.Disgust > emotionValue) { emotionValue = emotion.Disgust; emotionType = "Disgust"; }
                //if (emotion.Fear > emotionValue) { emotionValue = emotion.Fear; emotionType = "Fear"; }
                //if (emotion.Happiness > emotionValue) { emotionValue = emotion.Happiness; emotionType = "Happiness"; }
                //if (emotion.Neutral > emotionValue) { emotionValue = emotion.Neutral; emotionType = "Neutral"; }
                //if (emotion.Sadness > emotionValue) { emotionValue = emotion.Sadness; emotionType = "Sadness"; }
                //if (emotion.Surprise > emotionValue) { emotionType = "Surprise"; }
                //Console.WriteLine($"Emotion : {emotionType}");

                //// Get more face attributes
                //Console.WriteLine($"Exposure : {face.FaceAttributes.Exposure.ExposureLevel}");
                //Console.WriteLine($"FacialHair : {string.Format("{0}", face.FaceAttributes.FacialHair.Moustache + face.FaceAttributes.FacialHair.Beard + face.FaceAttributes.FacialHair.Sideburns > 0 ? "Yes" : "No")}");
                //Console.WriteLine($"Gender : {face.FaceAttributes.Gender}");
                //Console.WriteLine($"Glasses : {face.FaceAttributes.Glasses}");

                //// Get hair color
                //Hair hair = face.FaceAttributes.Hair;
                //string color = null;
                //if (hair.HairColor.Count == 0) { if (hair.Invisible) { color = "Invisible"; } else { color = "Bald"; } }
                //HairColorType returnColor = HairColorType.Unknown;
                //double maxConfidence = 0.0f;
                //foreach (HairColor hairColor in hair.HairColor)
                //{
                //    if (hairColor.Confidence <= maxConfidence) { continue; }
                //    maxConfidence = hairColor.Confidence; returnColor = hairColor.Color; color = returnColor.ToString();
                //}
                //Console.WriteLine($"Hair : {color}");

                //// Get more attributes
                //Console.WriteLine($"HeadPose : {string.Format("Pitch: {0}, Roll: {1}, Yaw: {2}", Math.Round(face.FaceAttributes.HeadPose.Pitch, 2), Math.Round(face.FaceAttributes.HeadPose.Roll, 2), Math.Round(face.FaceAttributes.HeadPose.Yaw, 2))}");
                //Console.WriteLine($"Makeup : {string.Format("{0}", (face.FaceAttributes.Makeup.EyeMakeup || face.FaceAttributes.Makeup.LipMakeup) ? "Yes" : "No")}");
                //Console.WriteLine($"Noise : {face.FaceAttributes.Noise.NoiseLevel}");
                //Console.WriteLine($"Occlusion : {string.Format("EyeOccluded: {0}", face.FaceAttributes.Occlusion.EyeOccluded ? "Yes" : "No")} " +
                //    $" {string.Format("ForeheadOccluded: {0}", face.FaceAttributes.Occlusion.ForeheadOccluded ? "Yes" : "No")}   {string.Format("MouthOccluded: {0}", face.FaceAttributes.Occlusion.MouthOccluded ? "Yes" : "No")}");
                //Console.WriteLine($"Smile : {face.FaceAttributes.Smile}");
                //Console.WriteLine();
            }

            // Save result image (using OpenCvSharp)
            srcImage.SaveImage(destImagePath);
        }
    }
}
