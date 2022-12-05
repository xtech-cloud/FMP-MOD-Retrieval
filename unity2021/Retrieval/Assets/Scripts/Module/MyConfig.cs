
using System.Xml.Serialization;

namespace XTC.FMP.MOD.Retrieval.LIB.Unity
{
    /// <summary>
    /// 配置类
    /// </summary>
    public class MyConfig : MyConfigBase
    {
        public class Filter
        {
            [XmlAttribute("format")]
            public string format { get; set; }
            [XmlAttribute("extension")]
            public string extension { get; set; }
            [XmlArray("PlaySubjects"), XmlArrayItem("Subject")]
            public Subject[] playSubjects { get; set; } = new Subject[0];
            [XmlArray("StopSubjects"), XmlArrayItem("Subject")]
            public Subject[] stopSubjects { get; set; } = new Subject[0];
        }

        public class Processor
        {
            [XmlAttribute("source")]
            public string source { get; set; }
            [XmlAttribute("uri")]
            public string uri { get; set; }
            [XmlArray("Filters"), XmlArrayItem("Filter")]
            public Filter[] filters { get; set; }
        }

        public class ResetTimer
        {
            [XmlAttribute("timeout")]
            public float timeout { get; set; }
        }

        public class ResultPage
        {
            [XmlAttribute("capacity")]
            public int capacity { get; set; }
            [XmlAttribute("button")]
            public int button { get; set; }
        }

        public class Style
        {
            [XmlAttribute("name")]
            public string name { get; set; } = "";
            [XmlElement("ResultPage")]
            public ResultPage resultPage { get; set; }
            [XmlElement("Processor")]
            public Processor processor { get; set; }
            [XmlElement("ResetTimer")]
            public ResetTimer resetTimer { get; set; }

        }


        [XmlArray("Styles"), XmlArrayItem("Style")]
        public Style[] styles { get; set; } = new Style[0];
    }
}

