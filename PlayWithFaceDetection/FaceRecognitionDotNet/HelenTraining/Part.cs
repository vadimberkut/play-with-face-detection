using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PlayWithFaceDetection.FaceRecognitionDotNet.HelenTraining
{
    [Serializable]
    public sealed class Part
    {

        [XmlAttribute("x")]
        public float X;

        [XmlAttribute("y")]
        public float Y;

        [XmlAttribute("name")]
        public string Name;

    }
}
