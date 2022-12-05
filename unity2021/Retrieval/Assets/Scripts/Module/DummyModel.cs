
namespace XTC.FMP.MOD.Retrieval.LIB.Unity
{
    /// <summary>
    /// 虚拟数据
    /// </summary>
    public class DummyModel : DummyModelBase
    {
        public class Record
        {
            public enum Format
            {
                Unknown,
                All,
                Text,
                Document,
                Image,
                Video,
                Audio
            }
            public Format format;
            public string uri;
            public string name;
            public string initials;
        }

        public class DummyStatus : DummyStatusBase
        {
        }

        public DummyModel(string _uid) : base(_uid)
        {
        }

    }
}

