

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using LibMVCS = XTC.FMP.LIB.MVCS;
using XTC.FMP.MOD.Retrieval.LIB.Proto;
using XTC.FMP.MOD.Retrieval.LIB.MVCS;
using System.Collections;
using XTC.FMP.LIB.MVCS;
using System.IO;
using System.Linq;

namespace XTC.FMP.MOD.Retrieval.LIB.Unity
{
    /// <summary>
    /// 实例类
    /// </summary>
    public class MyInstance : MyInstanceBase
    {
        public class UiReference
        {
            public class PageRecord
            {
                public GameObject root;
                public ScrollRect srList;
                public Button btnShowMore;
                public Text txtTag;
                public RectTransform listItemTemplate;
            }

            public InputField input;
            public RectTransform recordTemplate;
            public RectTransform pageTemplate;
            public Button btnPrevPage;
            public Button btnNextPage;
            public RectTransform tabs;
            public Button tabAll;
            public Button tabImage;
            public Button tabVideo;
            public Button tabAudio;
            public Button tabText;
            public Button tabDocument;
            public GameObject tipNotFound;
            public PageRecord pageRecord = new PageRecord();
        }

        private UiReference uiReference_ = new UiReference();
        private float resetTimer_ = 0f;
        private Coroutine coroutineResetTimerTick_;

        public MyInstance(string _uid, string _style, MyConfig _config, MyCatalog _catalog, LibMVCS.Logger _logger, Dictionary<string, LibMVCS.Any> _settings, MyEntryBase _entry, MonoBehaviour _mono, GameObject _rootAttachments)
            : base(_uid, _style, _config, _catalog, _logger, _settings, _entry, _mono, _rootAttachments)
        {
        }

        /// <summary>
        /// 当被创建时
        /// </summary>
        /// <remarks>
        /// 可用于加载主题目录的数据
        /// </remarks>
        public void HandleCreated()
        {
            if (style_.processor.source == "file://")
            {
                parseDisk(style_.processor.uri, style_);
            }

            uiReference_.input = rootUI.transform.Find("InputField").GetComponent<InputField>();
            var btnInputClickArea = rootUI.transform.Find("inputClickArea").GetComponent<Button>();
            btnInputClickArea.onClick.AddListener(() =>
            {
                rewindResetTimer();
                rootUI.GetComponent<Animator>().SetTrigger("on");
            });
            bindKeyEvents("keyboard/line1");
            bindKeyEvents("keyboard/line2");
            bindKeyEvents("keyboard/line3");
            uiReference_.recordTemplate = rootUI.transform.Find("panel-result/records/record").GetComponent<RectTransform>();
            uiReference_.recordTemplate.gameObject.SetActive(false);
            uiReference_.pageTemplate = rootUI.transform.Find("panel-result/page/indexs/index").GetComponent<RectTransform>();
            uiReference_.pageTemplate.gameObject.SetActive(false);
            uiReference_.btnPrevPage = rootUI.transform.Find("panel-result/page/btnBack").GetComponent<Button>();
            uiReference_.btnPrevPage.onClick.AddListener(() =>
            {
                rewindResetTimer();
                onBackPageClick();
            });
            uiReference_.btnPrevPage.gameObject.SetActive(false);
            uiReference_.btnNextPage = rootUI.transform.Find("panel-result/page/btnNext").GetComponent<Button>();
            uiReference_.btnNextPage.onClick.AddListener(() =>
            {
                rewindResetTimer();
                onNextPageClick();
            });
            uiReference_.btnNextPage.gameObject.SetActive(false);
            uiReference_.tabs = rootUI.transform.Find("panel-result/tabs").GetComponent<RectTransform>();
            uiReference_.tabs.gameObject.SetActive(false);
            uiReference_.tabAll = rootUI.transform.Find("panel-result/tabs/tab-all").GetComponent<Button>();
            uiReference_.tabAll.onClick.AddListener(() =>
            {
                rewindResetTimer();
                activateTab(uiReference_.tabAll);
                onTabClick(DummyModel.Record.Format.All);
            });
            uiReference_.tabImage = rootUI.transform.Find("panel-result/tabs/tab-image").GetComponent<Button>();
            uiReference_.tabImage.onClick.AddListener(() =>
            {
                rewindResetTimer();
                activateTab(uiReference_.tabImage);
                onTabClick(DummyModel.Record.Format.Image);
            });
            uiReference_.tabVideo = rootUI.transform.Find("panel-result/tabs/tab-video").GetComponent<Button>();
            uiReference_.tabVideo.onClick.AddListener(() =>
            {
                rewindResetTimer();
                activateTab(uiReference_.tabVideo);
                onTabClick(DummyModel.Record.Format.Video);
            });
            uiReference_.tabAudio = rootUI.transform.Find("panel-result/tabs/tab-audio").GetComponent<Button>();
            uiReference_.tabAudio.onClick.AddListener(() =>
            {
                rewindResetTimer();
                activateTab(uiReference_.tabAudio);
                onTabClick(DummyModel.Record.Format.Audio);
            });
            uiReference_.tabText = rootUI.transform.Find("panel-result/tabs/tab-text").GetComponent<Button>();
            uiReference_.tabText.onClick.AddListener(() =>
            {
                rewindResetTimer();
                activateTab(uiReference_.tabText);
                onTabClick(DummyModel.Record.Format.Text);
            });
            uiReference_.tabDocument = rootUI.transform.Find("panel-result/tabs/tab-document").GetComponent<Button>();
            uiReference_.tabDocument.onClick.AddListener(() =>
            {
                rewindResetTimer();
                activateTab(uiReference_.tabDocument);
                onTabClick(DummyModel.Record.Format.Document);
            });
            uiReference_.tipNotFound = rootUI.transform.Find("panel-result/tipNotFound").gameObject;
            uiReference_.tipNotFound.SetActive(false);

            uiReference_.pageRecord.root = rootUI.transform.Find("pageRecord").gameObject;
            uiReference_.pageRecord.srList = rootUI.transform.Find("pageRecord/menu/srList").GetComponent<ScrollRect>();
            uiReference_.pageRecord.btnShowMore = rootUI.transform.Find("pageRecord/menu/btnShowMore").GetComponent<Button>();
            uiReference_.pageRecord.txtTag = rootUI.transform.Find("pageRecord/tag/Text").GetComponent<Text>();
            uiReference_.pageRecord.listItemTemplate = rootUI.transform.Find("pageRecord/menu/srList/Viewport/Content/record").GetComponent<RectTransform>();
            uiReference_.pageRecord.listItemTemplate.gameObject.SetActive(false);
            rootUI.transform.Find("pageRecord/header/btnBack").GetComponent<Button>().onClick.AddListener(() =>
            {
                switchToSearchPage();
            });
            uiReference_.pageRecord.btnShowMore.gameObject.SetActive(false);
            rootUI.transform.Find("title").GetComponent<Button>().onClick.AddListener(() =>
            {
                pauseResetTimer();
                reset();
            });
        }

        /// <summary>
        /// 当被删除时
        /// </summary>
        public void HandleDeleted()
        {
        }

        /// <summary>
        /// 当被打开时
        /// </summary>
        /// <remarks>
        /// 可用于加载内容目录的数据
        /// </remarks>
        public void HandleOpened(string _source, string _uri)
        {
            rootUI.gameObject.SetActive(true);
        }

        /// <summary>
        /// 当被关闭时
        /// </summary>
        public void HandleClosed()
        {
            rootUI.gameObject.SetActive(false);
        }

        /// <summary>
        /// 暂停重置钩子
        /// </summary>
        private void pauseResetTimer()
        {
            if (null != coroutineResetTimerTick_)
            {
                mono_.StopCoroutine(coroutineResetTimerTick_);
                coroutineResetTimerTick_ = null;
            }
        }

        /// <summary>
        /// 重启重置钩子
        /// </summary>
        private void rewindResetTimer()
        {
            resetTimer_ = 0;
            if (null != coroutineResetTimerTick_)
                return;
            coroutineResetTimerTick_ = mono_.StartCoroutine(tickResetTimer());
        }


        private void bindKeyEvents(string _container)
        {
            foreach (Transform child in rootUI.transform.Find(_container))
            {
                var btnKey = child.gameObject.GetComponent<Button>();
                btnKey.onClick.AddListener(() =>
                {
                    rewindResetTimer();
                    if (btnKey.gameObject.name.Equals("back"))
                    {
                        uiReference_.input.text = uiReference_.input.text.Length > 0 ? uiReference_.input.text.Substring(0, uiReference_.input.text.Length - 1) : "";
                    }
                    else if (btnKey.gameObject.name.Equals("clear"))
                    {
                        uiReference_.input.text = "";
                    }
                    else if (uiReference_.input.text.Length < 24)
                    {
                        uiReference_.input.text += btnKey.gameObject.name;
                    }
                    else
                    {
                        return;
                    }
                    searchFromLocalFiles(uiReference_.input.text);
                });
            }
        }

        private void activateTab(Button _tab)
        {
            System.Action<Button> hide = (_button) =>
            {
                _button.transform.Find("Checkmark").gameObject.SetActive(false);
            };
            hide(uiReference_.tabAll);
            hide(uiReference_.tabImage);
            hide(uiReference_.tabVideo);
            hide(uiReference_.tabAudio);
            hide(uiReference_.tabText);
            hide(uiReference_.tabDocument);
            _tab.transform.Find("Checkmark").gameObject.SetActive(true);
        }

        private IEnumerator tickResetTimer()
        {
            while (true)
            {
                yield return new UnityEngine.WaitForSeconds(1);
                resetTimer_ += 1;
                if (resetTimer_ > style_.resetTimer.timeout)
                {
                    resetTimer_ = 0;
                    reset();
                    break;
                }
            }
            coroutineResetTimerTick_ = null;
        }

        private void reset()
        {
            rootUI.GetComponent<Animator>().SetTrigger("off");
            uiReference_.input.text = "";
            uiReference_.tabs.gameObject.SetActive(false);
            uiReference_.tipNotFound.SetActive(false);
            clearResults();
            clearPages();
        }

        private void clearResults()
        {
            List<GameObject> records = new List<GameObject>();
            // 跳过模板
            for (int i = 1; i < uiReference_.recordTemplate.parent.childCount; ++i)
            {
                records.Add(uiReference_.recordTemplate.parent.GetChild(i).gameObject);
            }
            foreach (var go in records)
            {
                UnityEngine.GameObject.Destroy(go);
            }
        }

        private void clearPages()
        {
            uiReference_.btnNextPage.gameObject.SetActive(false);
            uiReference_.btnPrevPage.gameObject.SetActive(false);
            List<GameObject> pages = new List<GameObject>();
            // 跳过模板
            for (int i = 1; i < uiReference_.pageTemplate.parent.childCount; ++i)
            {
                pages.Add(uiReference_.pageTemplate.parent.GetChild(i).gameObject);
            }
            foreach (var go in pages)
            {
                UnityEngine.GameObject.DestroyImmediate(go);
            }
        }

        private void onBackPageClick()
        {
            bool isRightHasOther = true;
            bool isLeftHasOther = true;
            int startPageIndex = searchReply_.pageIndex - style_.resultPage.button + 1;
            if (startPageIndex <= 0)
            {
                startPageIndex = 0;
                isLeftHasOther = false;
            }
            int endPageIndex = startPageIndex + style_.resultPage.button - 1;
            if (endPageIndex >= searchReply_.pageCount - 1)
            {
                endPageIndex = searchReply_.pageCount - 1;
                isRightHasOther = false;
            }

            searchReply_.pageIndex = startPageIndex;
            switchPage(startPageIndex, endPageIndex, isLeftHasOther, isRightHasOther);
        }

        private void onNextPageClick()
        {
            bool isRightHasOther = true;
            bool isLeftHasOther = true;
            int endPageIndex = searchReply_.pageIndex + 2 * style_.resultPage.button - 1;
            if (endPageIndex >= searchReply_.pageCount - 1)
            {
                endPageIndex = searchReply_.pageCount - 1;
                isRightHasOther = false;
            }
            int startPageIndex = endPageIndex - style_.resultPage.button + 1;
            if (startPageIndex <= 0)
            {
                startPageIndex = 0;
                isLeftHasOther = false;
            }

            searchReply_.pageIndex = startPageIndex;
            switchPage(startPageIndex, endPageIndex, isLeftHasOther, isRightHasOther);
        }

        private void onTabClick(DummyModel.Record.Format _format)
        {
            searchReply_.filterRecords.Clear();
            if (DummyModel.Record.Format.All == _format)
            {
                searchReply_.filterRecords.AddRange(searchReply_.allRecords);
            }
            else
            {
                IEnumerable<DummyModel.Record> matchedRecords = from file in searchReply_.allRecords where file.format == _format select file;
                searchReply_.filterRecords.AddRange(matchedRecords.ToList());
            }
            searchReply_.pageCount = (searchReply_.filterRecords.Count / style_.resultPage.capacity) + (searchReply_.filterRecords.Count % style_.resultPage.capacity == 0 ? 0 : 1);
            clearResults();
            refreshPageBar(searchReply_.pageCount);
            activatePage(0);
        }

        private void switchToRecordPage()
        {
            uiReference_.pageRecord.root.GetComponent<UnityEngine.Animator>().SetTrigger("on");
        }

        private void switchToSearchPage()
        {
            uiReference_.pageRecord.root.GetComponent<UnityEngine.Animator>().SetTrigger("off");
            /*
            //关闭所有播放器
            foreach (var e in config.processor.filters)
            {
                var opt = new Dictionary<string, object>();
                opt["uid"] = e.playerId;
                string strAction = string.Format("/{0}/Stop", e.playerModule);
                model.Broadcast(strAction, opt);
            }
            */
        }

        /// <summary>
        /// 刷新显示全部标签
        /// </summary>
        /// <param name="_visible"></param>
        private void refreshActiveTabAll(bool _visible)
        {
            activateTab(uiReference_.tabAll);
            uiReference_.tabs.gameObject.SetActive(_visible);
        }

        private void refreshPageBar(int _pageCount)
        {
            clearPages();
            for (int i = 0; i < _pageCount; i++)
            {
                var clone = UnityEngine.GameObject.Instantiate(uiReference_.pageTemplate.gameObject, uiReference_.pageTemplate.parent);
                clone.name = i.ToString();
                clone.transform.Find("Label").GetComponent<UnityEngine.UI.Text>().text = (i + 1).ToString();
                clone.gameObject.SetActive(i < style_.resultPage.button);
                var button = clone.GetComponent<UnityEngine.UI.Button>();
                button.onClick.AddListener(() =>
                {
                    rewindResetTimer();
                    int index;
                    int.TryParse(button.gameObject.name, out index);
                    activatePage(index);
                });
            }
            uiReference_.btnNextPage.gameObject.SetActive(_pageCount > style_.resultPage.button);
            uiReference_.btnPrevPage.gameObject.SetActive(_pageCount > style_.resultPage.button);
            uiReference_.btnPrevPage.interactable = false;
            uiReference_.btnNextPage.interactable = _pageCount > style_.resultPage.button;
            mono_.StartCoroutine(relayoutPage());
        }

        private IEnumerator relayoutPage()
        {
            //禁用后开启触发重新布局
            yield return new WaitForEndOfFrame();
            uiReference_.pageTemplate.parent.GetComponent<UnityEngine.UI.ContentSizeFitter>().enabled = false;
            yield return new WaitForEndOfFrame();
            uiReference_.pageTemplate.parent.GetComponent<UnityEngine.UI.ContentSizeFitter>().enabled = true;
            yield return new WaitForEndOfFrame();
        }

        /// <summary>
        /// 刷新激活的页码
        /// </summary>
        /// <param name="_page"></param>
        private void refreshActivePage(int _page)
        {
            for (int i = 1; i < uiReference_.pageTemplate.parent.childCount; ++i)
            {
                UnityEngine.Transform child = uiReference_.pageTemplate.parent.GetChild(i);
                child.Find("Background/Checkmark").gameObject.SetActive(_page == i - 1);
            }
        }
        private void refreshSearchResult(List<DummyModel.Record> _records)
        {
            clearResults();
            foreach (var record in _records)
            {
                var clone = UnityEngine.GameObject.Instantiate(uiReference_.recordTemplate.gameObject, uiReference_.recordTemplate.parent);
                clone.name = record.uri;
                clone.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = record.name;
                clone.transform.Find("icon-text").gameObject.SetActive(DummyModel.Record.Format.Text == record.format);
                clone.transform.Find("icon-image").gameObject.SetActive(DummyModel.Record.Format.Image == record.format);
                clone.transform.Find("icon-video").gameObject.SetActive(DummyModel.Record.Format.Video == record.format);
                clone.transform.Find("icon-audio").gameObject.SetActive(DummyModel.Record.Format.Audio == record.format);
                clone.transform.Find("icon-document").gameObject.SetActive(DummyModel.Record.Format.Document == record.format);
                clone.transform.Find("icon-thumb").gameObject.SetActive(false);
                clone.SetActive(true);
                clone.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
                {
                    rewindResetTimer();
                    activateRecord(clone.name);
                });
            }
            //facade.ui.tipNotFound.gameObject.SetActive(_records.Count == 0);
        }

        private IEnumerator refreshRecordMenu(string _tag, DummyModel.Record _record, List<DummyModel.Record> _records, System.Action _onFinish)
        {
            uiReference_.pageRecord.txtTag.text = _tag;
            // 在删除前调用，否则删除掉isOn的toggle后，会自动设置一个toggle的isOn为true
            uiReference_.pageRecord.listItemTemplate.parent.GetComponent<UnityEngine.UI.ToggleGroup>().allowSwitchOff = true;
            yield return new UnityEngine.WaitForEndOfFrame();
            // 删除旧数据
            List<UnityEngine.GameObject> items = new List<UnityEngine.GameObject>();
            for (int i = 1; i < uiReference_.pageRecord.listItemTemplate.parent.childCount; ++i)
            {
                items.Add(uiReference_.pageRecord.listItemTemplate.parent.GetChild(i).gameObject);
            }
            foreach (var item in items)
            {
                UnityEngine.GameObject.DestroyImmediate(item);
            }
            yield return new UnityEngine.WaitForEndOfFrame();
            List<UnityEngine.UI.Toggle> toggles = new List<UnityEngine.UI.Toggle>();
            yield return new UnityEngine.WaitForEndOfFrame();
            //添加新数据
            UnityEngine.UI.Toggle targetToggle = null;
            foreach (var record in _records)
            {
                var clone = UnityEngine.GameObject.Instantiate(uiReference_.pageRecord.listItemTemplate.gameObject, uiReference_.pageRecord.listItemTemplate.parent);
                clone.name = record.uri;
                clone.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = record.name;
                clone.transform.Find("icon-text").gameObject.SetActive(DummyModel.Record.Format.Text == record.format);
                clone.transform.Find("icon-image").gameObject.SetActive(DummyModel.Record.Format.Image == record.format);
                clone.transform.Find("icon-video").gameObject.SetActive(DummyModel.Record.Format.Video == record.format);
                clone.transform.Find("icon-audio").gameObject.SetActive(DummyModel.Record.Format.Audio == record.format);
                clone.transform.Find("icon-document").gameObject.SetActive(DummyModel.Record.Format.Document == record.format);
                clone.transform.Find("icon-thumb").gameObject.SetActive(false);
                clone.SetActive(true);
                var toggle = clone.GetComponent<UnityEngine.UI.Toggle>();
                toggles.Add(toggle);
                if (record == _record)
                {
                    targetToggle = toggle;
                }
                yield return new UnityEngine.WaitForEndOfFrame();
            }
            yield return new UnityEngine.WaitForEndOfFrame();
            uiReference_.pageRecord.listItemTemplate.parent.GetComponent<UnityEngine.UI.ToggleGroup>().allowSwitchOff = false;
            yield return new UnityEngine.WaitForEndOfFrame();
            foreach (var toggle in toggles)
            {
                toggle.onValueChanged.AddListener((_toggled) =>
                {
                    if (!_toggled)
                        return;
                    string uri = toggle.name;
                    DummyModel.Record foundRecord = null;
                    foreach (var record in searchReply_.filterRecords)
                    {
                        if (record.uri.Equals(uri))
                        {
                            foundRecord = record;
                            break;
                        }
                    }

                    if (null == foundRecord)
                    {
                        logger_.Error("{0} not found", uri);
                        return;
                    }

                    float delay = searchReply_.recordActivated ? 0.02f : 1f;
                    mono_.StartCoroutine(playRecord(foundRecord, delay));
                });
            }
            if (null != targetToggle)
            {
                targetToggle.transform.SetAsFirstSibling();
                uiReference_.pageRecord.listItemTemplate.SetAsFirstSibling();
                targetToggle.isOn = true;
            }
            yield return new UnityEngine.WaitForEndOfFrame();
            _onFinish();
        }

        private void activatePage(int _page)
        {
            searchReply_.pageIndex = _page;
            int recordCount = searchReply_.filterRecords.Count - _page * style_.resultPage.capacity > style_.resultPage.capacity ? style_.resultPage.capacity : searchReply_.filterRecords.Count - _page * style_.resultPage.capacity;
            List<DummyModel.Record> records = new List<DummyModel.Record>();
            records.AddRange(searchReply_.filterRecords.GetRange(_page * style_.resultPage.capacity, recordCount));
            refreshActivePage(_page);
            refreshSearchResult(records);
        }

        /// <summary>
        /// 激活搜索结果
        /// </summary>
        /// <param name="_status"></param>
        /// <param name="_uri"></param>
        private void activateRecord(string _uri)
        {
            DummyModel.Record foundRecord = null;
            foreach (var record in searchReply_.filterRecords)
            {
                if (record.uri.Equals(_uri))
                {
                    foundRecord = record;
                    break;
                }
            }

            if (null == foundRecord)
            {
                logger_.Error("{0} not found", _uri);
                return;
            }

            searchReply_.recordActivated = false;
            switchToRecordPage();
            List<DummyModel.Record> records = new List<DummyModel.Record>();
            // 只加载当前页
            int startIndex = searchReply_.pageIndex * style_.resultPage.capacity;
            if (startIndex >= searchReply_.filterRecords.Count - 1)
                startIndex = searchReply_.filterRecords.Count - 1;
            int endIndex = (searchReply_.pageIndex + 1) * style_.resultPage.capacity - 1;
            if (endIndex >= searchReply_.filterRecords.Count - 1)
                endIndex = searchReply_.filterRecords.Count - 1;
            records.AddRange(searchReply_.filterRecords.GetRange(startIndex, endIndex - startIndex + 1));

            mono_.StartCoroutine(refreshRecordMenu(searchReply_.searchKey, foundRecord, records, () =>
            {
                searchReply_.recordActivated = true;
            }));
        }

        private void switchPage(int _startIndex, int _endIndex, bool _leftHasOther, bool _rightHasOther)
        {
            // 跳过模板
            for (int i = 1; i < uiReference_.pageTemplate.parent.childCount; ++i)
            {
                var page = uiReference_.pageTemplate.parent.GetChild(i);
                int pageIndex = i - 1;
                page.gameObject.SetActive(pageIndex >= _startIndex && pageIndex <= _endIndex);
            }
            uiReference_.btnNextPage.interactable = _rightHasOther;
            uiReference_.btnPrevPage.interactable = _leftHasOther;
            activatePage(_startIndex);
            mono_.StartCoroutine(relayoutPage());
        }

        private IEnumerator playRecord(DummyModel.Record _record, float _delay)
        {
            Dictionary<string, object> variableS = new Dictionary<string, object>();
            //关闭所有播放器
            foreach (var e in style_.processor.filters)
            {
                publishSubjects(e.stopSubjects, variableS);
            }
            // 播放器的关闭会延迟一帧处理
            yield return new UnityEngine.WaitForEndOfFrame();
            yield return new UnityEngine.WaitForEndOfFrame();

            rewindResetTimer();
            var filter = findFilter(_record);
            if (null == filter)
            {
                logger_.Error("filter not found, record is format:{0}", _record.format);
                yield break;
            }
            variableS["{{uri}}"] = _record.uri;
            publishSubjects(filter.playSubjects, variableS);
        }

        private MyConfig.Filter findFilter(DummyModel.Record _record)
        {
            foreach (var filter in style_.processor.filters)
            {
                if (_record.format.ToString().Equals(filter.format))
                    return filter;
            }
            return null;
        }

        #region Records
        //TODO move to mvcs.dll
        public class SearchReply
        {
            public List<DummyModel.Record> filterRecords = new List<DummyModel.Record>();
            public List<DummyModel.Record> allRecords = new List<DummyModel.Record>();
            /// <summary>
            /// 搜索结果是否被激活
            /// </summary>
            public bool recordActivated = false;
            /// <summary>
            /// 当前的页码
            /// </summary>
            public int pageIndex = 0;
            /// <summary>
            /// 页的数量
            /// </summary>
            public int pageCount = 0;
            /// <summary>
            /// 搜索的关键字
            /// </summary>
            public string searchKey;
        }


        private List<DummyModel.Record> records_ = new List<DummyModel.Record>();
        private SearchReply searchReply_ = new SearchReply();

        private void parseDisk(string _uri, MyConfig.Style _style)
        {

            System.Func<FileInfo, bool> isMatch = (_file) =>
            {
                bool matched = false;
                foreach (var filter in _style.processor.filters)
                {
                    if (filter.extension.Contains(_file.Extension))
                        matched = true;
                }
                return matched;
            };
            records_ = new List<DummyModel.Record>();
            DirectoryInfo dir = new DirectoryInfo(_uri);
            IEnumerable<FileInfo> fileList = dir.GetFiles("*.*", SearchOption.AllDirectories);
            IEnumerable<FileInfo> fileQuery = from file in fileList where isMatch(file) orderby file.Name select file;
            foreach (FileInfo fi in fileQuery)
            {
                DummyModel.Record record = new DummyModel.Record();
                record.uri = fi.FullName;
                record.name = Path.GetFileNameWithoutExtension(fi.FullName);
                record.format = takeFileFormat(Path.GetExtension(fi.FullName), _style);
                record.initials = NPinyin.Pinyin.GetInitials(Path.GetFileNameWithoutExtension(fi.Name));
                records_.Add(record);
            }
        }
        private DummyModel.Record.Format takeFileFormat(string _extension, MyConfig.Style _style)
        {
            foreach (var filter in _style.processor.filters)
            {
                if (filter.extension.Contains(_extension))
                {
                    if (filter.format.Equals(DummyModel.Record.Format.Text.ToString()))
                        return DummyModel.Record.Format.Text;
                    else if (filter.format.Equals(DummyModel.Record.Format.Image.ToString()))
                        return DummyModel.Record.Format.Image;
                    else if (filter.format.Equals(DummyModel.Record.Format.Video.ToString()))
                        return DummyModel.Record.Format.Video;
                    else if (filter.format.Equals(DummyModel.Record.Format.Audio.ToString()))
                        return DummyModel.Record.Format.Audio;
                    else if (filter.format.Equals(DummyModel.Record.Format.Document.ToString()))
                        return DummyModel.Record.Format.Document;
                }
            }
            return DummyModel.Record.Format.Unknown;
        }

        private void searchFromLocalFiles(string _key)
        {
            searchReply_.searchKey = _key;
            searchReply_.filterRecords.Clear();
            searchReply_.allRecords.Clear();
            clearResults();

            if (string.IsNullOrWhiteSpace(_key))
            {
                return;
            }
            IEnumerable<DummyModel.Record> records = from file in records_ where file.initials.ToLower().Contains(_key.ToLower()) select file;
            searchReply_.allRecords.AddRange(records.ToList());
            searchReply_.filterRecords.AddRange(searchReply_.allRecords);
            searchReply_.pageCount = (searchReply_.filterRecords.Count / style_.resultPage.capacity) + (searchReply_.filterRecords.Count % style_.resultPage.capacity == 0 ? 0 : 1);
            refreshActiveTabAll(searchReply_.pageCount > 0);
            refreshPageBar(searchReply_.pageCount);
            activatePage(0);
        }

        #endregion
    }
}
