using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PlayWithFaceDetection.FaceRecognitionDotNet.HelenTraining
{
    [Serializable]
    [XmlRoot(ElementName = "dataset")]
    public class Dataset
    {

        [XmlElement("name")]
        public string Name;

        [XmlElement("comment")]
        public string Comment;

        [XmlArray("images")]
        [XmlArrayItem("image")]
        public Image[] Images;

    }
}
