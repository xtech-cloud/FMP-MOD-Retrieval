
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
            public Filter[] filters { get; set; } = new Filter[0];
        }

        public class ResetTimer
        {
            [XmlAttribute("timeout")]
            public float timeout { get; set; } = 20;
        }

        public class ResultPage
        {
            [XmlAttribute("capacity")]
            public int capacity { get; set; } = 10;
            [XmlAttribute("button")]
            public int button { get; set; } = 10;
        }

        public class RecordSideMenu
        {
            [XmlAttribute("selectedColor")]
            public string selectedColor { get; set; } = "";
            [XmlAttribute("titleImage")]
            public string titleImage { get; set; } = "";
        }

        public class Backgound
        {
            [XmlAttribute("visible")]
            public bool visible { get; set; } = false;
            [XmlAttribute("image")]
            public string image { get; set; } = "";
        }

        public class Mask
        {
            [XmlAttribute("visible")]
            public bool visible { get; set; } = false;
            [XmlAttribute("color")]
            public string color { get; set; } = "#FFFFFFFF";
        }

        public class Keyboard
        {
            [XmlAttribute("image")]
            public string image { get; set; } = "";
            [XmlAttribute("keyImage")]
            public string keyImage { get; set; } = "";
        }

        public class Style
        {
            [XmlAttribute("name")]
            public string name { get; set; } = "";
            [XmlAttribute("primaryColor")]
            public string primaryColor { get; set; } = "";
            [XmlElement("PageHomeMask")]
            public Mask pageHomeMask { get; set; } = new Mask();
            [XmlElement("PageHomeBackground")]
            public Backgound pageHomeBackground { get; set; } = new Backgound();
            [XmlElement("PageRecordBackground")]
            public Backgound pageRecordBackground { get; set; } = new Backgound();
            [XmlElement("ResultPage")]
            public ResultPage resultPage { get; set; } = new ResultPage();
            [XmlElement("RecordSideMenu")]
            public RecordSideMenu recordSideMenu { get; set; } = new RecordSideMenu();
            [XmlElement("Processor")]
            public Processor processor { get; set; } = new Processor();
            [XmlElement("ResetTimer")]
            public ResetTimer resetTimer { get; set; } = new ResetTimer();
            [XmlElement("Keyboard")]
            public Keyboard keyboard { get; set; } = new Keyboard();
        }

        [XmlArray("Styles"), XmlArrayItem("Style")]
        public Style[] styles { get; set; } = new Style[0];
    }
}

