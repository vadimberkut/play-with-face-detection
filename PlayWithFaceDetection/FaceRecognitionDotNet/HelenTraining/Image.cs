using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PlayWithFaceDetection.FaceRecognitionDotNet.HelenTraining
{
    [Serializable]
    public class Image
    {

        [XmlAttribute("file")]
        public string File;

        [XmlElement("box")]
        public Box Box;

    }
}
