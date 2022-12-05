
using UnityEngine;
using LibMVCS = XTC.FMP.LIB.MVCS;
using XTC.FMP.MOD.Retrieval.LIB.MVCS;

namespace XTC.FMP.MOD.Retrieval.LIB.Unity
{
    /// <summary>
    /// 模块入口
    /// </summary>
    public class MyEntry : MyEntryBase
    {
        public DummyModel getDummyModel()
        {
            return modelDummy_;
        }
    }
}

