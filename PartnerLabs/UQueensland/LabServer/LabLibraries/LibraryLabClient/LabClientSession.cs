using System;
using System.Xml;

namespace Library.LabClient
{
    public class LabClientSession
    {
        public bool multiSubmit;
        public string bannerTitle;
        public string statusVersion;
        public string navmenuPhotoUrl;
        public string mailtoUrl;
        public string labinfoText;
        public string labinfoUrl;
        public XmlNode xmlNodeLabConfiguration;
        public XmlNode xmlNodeConfiguration;
        public XmlNode xmlNodeValidation;
        public XmlNode xmlNodeSpecification;
        public LabClientToSbAPI labClientToSbAPI;
    }
}
