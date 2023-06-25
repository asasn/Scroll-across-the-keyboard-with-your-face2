using RootNS.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RootNS.Models
{
    public class Name : NotificationObject
    {
        public Name()
        {
            LoadDicts();
            this.PropertyChanged += Name_PropertyChanged;
        }

        private string _oneName = "←左右按钮取名→";

        public string OneName
        {
            get { return _oneName; }
            set
            {
                _oneName = value;
                RaisePropertyChanged(nameof(OneName));
            }
        }


        public enum StyleValue
        {
            常规,
            称号,
            地名,
        }

        private string _surnameText;
        /// <summary>
        /// 指定姓氏
        /// </summary>
        public string SurnameText
        {
            get { return _surnameText; }
            set
            {
                _surnameText = value;
                RaisePropertyChanged(nameof(SurnameText));
            }
        }

        private ObservableCollection<string> _nText = new ObservableCollection<string>() { "", "", "", "" };
        /// <summary>
        /// 指定名字（集合）
        /// </summary>
        public ObservableCollection<string> NText
        {
            get { return _nText; }
            set
            {
                _nText = value;
                RaisePropertyChanged(nameof(NText));
            }
        }

        private string _sText;
        /// <summary>
        /// 指定称号
        /// </summary>
        public string SText
        {
            get { return _sText; }
            set
            {
                _sText = value;
                RaisePropertyChanged(nameof(SText));
            }
        }


        private void Name_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Style))
            {
                if (this.Style == StyleValue.常规.ToString())
                {
                    CurrentPrefixBank = SurnameBank;
                    CurrentSuffixBank = NameBank;
                }
                if (this.Style == StyleValue.称号.ToString())
                {
                    CurrentPrefixBank = PrefixBank;
                    CurrentSuffixBank = SuffixBank;
                }
                if (this.Style == StyleValue.地名.ToString())
                {

                    CurrentPrefixBank = PlacePrefixBank;
                    CurrentSuffixBank = PlaceSuffixBank;
                }
                RefreshCurrentDict();
            }
        }

        private string _style = StyleValue.常规.ToString();

        public string Style
        {
            get { return _style; }
            set
            {
                _style = value;
                RaisePropertyChanged(nameof(Style));
            }
        }

        private int _surnameLength = 1;
        /// <summary>
        /// 姓氏长度
        /// </summary>
        public int SurnameLength
        {
            get { return _surnameLength; }
            set
            {
                _surnameLength = value;
                RaisePropertyChanged(nameof(SurnameLength));
            }
        }


        private int _nameLength = 1;
        /// <summary>
        /// 名字长度
        /// </summary>
        public int NameQuantity
        {
            get { return _nameLength; }
            set
            {
                _nameLength = value;
                RaisePropertyChanged(nameof(NameQuantity));
            }
        }

        private int _suffixLength = 1;
        /// <summary>
        /// 后缀长度
        /// </summary>
        public int SuffixLength
        {
            get { return _suffixLength; }
            set
            {
                _suffixLength = value;
                RaisePropertyChanged(nameof(SuffixLength));
            }
        }


        private ObservableCollection<string> _results = new ObservableCollection<string>();
        /// <summary>
        /// 结果集合
        /// </summary>
        public ObservableCollection<string> Results
        {
            get { return _results; }
            set
            {
                _results = value;
                RaisePropertyChanged(nameof(Results));
            }
        }


        /// <summary>
        /// 生成结果集合
        /// </summary>
        public void Generate(int length = 80)
        {
            Results.Clear();
            for (int i = 0; i < length; i++)
            {
                string prefixNormal = string.Empty;
                string suffixNormal = string.Empty;

                if (this.Style == StyleValue.常规.ToString())
                {
                    if (CurrentPrefixDict.Count > 0)
                    {
                        if (string.IsNullOrWhiteSpace(SurnameText))
                        {
                            do
                            {
                                prefixNormal = GetStringFromList(CurrentPrefixDict);
                            } while (prefixNormal.Length != SurnameLength);
                        }
                        else
                        {
                            prefixNormal = SurnameText;
                        }

                    }

                    if (CurrentSuffixDict.Count > 0)
                    {
                        for (int n = 0; n < NameQuantity; n++)
                        {
                            if (string.IsNullOrWhiteSpace(NText[n]))
                            {
                                string sTemp = GetStringFromList(CurrentSuffixDict);
                                if (suffixNormal.Contains(sTemp))
                                {
                                    n--;
                                }
                                else
                                {
                                    suffixNormal += sTemp;
                                }
                            }
                            else
                            {
                                suffixNormal += NText[n];
                            }
                        }
                    }
                }
                if (this.Style == StyleValue.称号.ToString() || this.Style == StyleValue.地名.ToString())
                {
                    if (CurrentPrefixDict.Count > 0)
                    {
                        for (int n = 0; n < NameQuantity; n++)
                        {
                            if (string.IsNullOrWhiteSpace(NText[n]))
                            {
                                string sTemp = GetStringFromList(CurrentPrefixDict);
                                if (prefixNormal.Contains(sTemp))
                                {
                                    n--;
                                }
                                else
                                {
                                    prefixNormal += sTemp;
                                }
                            }
                            else
                            {
                                prefixNormal += NText[n];
                            }
                        }
                    }
                    if (CurrentSuffixDict.Count > 0)
                    {
                        if (string.IsNullOrWhiteSpace(SText))
                        {
                            do
                            {
                                suffixNormal = GetStringFromList(CurrentSuffixDict);
                            } while (suffixNormal.Length != SuffixLength);
                        }
                        else
                        {
                            suffixNormal = SText;
                        }
                    }
                }
                Results.Add(prefixNormal + suffixNormal);
            }

            //在这里赋值就不用额外写集合变动事件的代码了，正常来说应该在事件里面写的
            //反正只有点击按钮，生成结果的时候，才要从结果集合中取值，干脆写在这个函数里
            if (Results.Count > 0)
            {
                OneName = Results[0];
            }
        }


        private List<string> _currentPrefixDict = new List<string>();
        /// <summary>
        /// 合并之后的字符串字典（前缀）
        /// </summary>
        public List<string> CurrentPrefixDict
        {
            get { return _currentPrefixDict; }
            set
            {
                _currentPrefixDict = value;
                RaisePropertyChanged(nameof(CurrentPrefixDict));
            }
        }

        private List<string> _currentSuffixDict = new List<string>();
        /// <summary>
        /// 合并之后的字符串字典（后缀）
        /// </summary>
        public List<string> CurrentSuffixDict
        {
            get { return _currentSuffixDict; }
            set
            {
                _currentSuffixDict = value;
                RaisePropertyChanged(nameof(CurrentSuffixDict));
            }
        }

        private string _currentPrefixDictCount = "0";

        public string CurrentPrefixDictCount
        {
            get { return _currentPrefixDictCount; }
            set
            {
                _currentPrefixDictCount = value;
                RaisePropertyChanged(nameof(CurrentPrefixDictCount));
            }
        }

        private string _currentSuffixDictCount = "0";

        public string CurrentSuffixDictCount
        {
            get { return _currentSuffixDictCount; }
            set
            {
                _currentSuffixDictCount = value;
                RaisePropertyChanged(nameof(CurrentSuffixDictCount));
            }
        }

        private ObservableCollection<TextItem> _currentPrefixBank;
        /// <summary>
        /// 当前使用的字典库（前缀）
        /// </summary>
        public ObservableCollection<TextItem> CurrentPrefixBank
        {
            get { return _currentPrefixBank; }
            set
            {
                _currentPrefixBank = value;
                RaisePropertyChanged(nameof(CurrentPrefixBank));
            }
        }

        private ObservableCollection<TextItem> _currentSuffixBank;
        /// <summary>
        /// 当前使用的字典库（后缀）
        /// </summary>
        public ObservableCollection<TextItem> CurrentSuffixBank
        {
            get { return _currentSuffixBank; }
            set
            {
                _currentSuffixBank = value;
                RaisePropertyChanged(nameof(CurrentSuffixBank));
            }
        }

        /// <summary>
        /// 字库（对应TXT文件）
        /// </summary>
        public class TextItem : NotificationObject
        {
            private string _title;

            public string Title
            {
                get { return _title; }
                set
                {
                    _title = value;
                    RaisePropertyChanged(nameof(Title));
                }
            }

            private bool _use;

            public bool Use
            {
                get { return _use; }
                set
                {
                    _use = value;
                    RaisePropertyChanged(nameof(Use));
                }
            }

            private List<string> _dict;

            public List<string> Dict
            {
                get { return _dict; }
                set
                {
                    _dict = value;
                    RaisePropertyChanged(nameof(Dict));
                }
            }
        }

        private ObservableCollection<TextItem> _allBank = new ObservableCollection<TextItem>();

        public ObservableCollection<TextItem> AllBank
        {
            get { return _allBank; }
            set
            {
                _allBank = value;
                RaisePropertyChanged(nameof(AllBank));
            }
        }

        private ObservableCollection<TextItem> _surnameBank = new ObservableCollection<TextItem>();

        public ObservableCollection<TextItem> SurnameBank
        {
            get { return _surnameBank; }
            set
            {
                _surnameBank = value;
                RaisePropertyChanged(nameof(SurnameBank));
            }
        }

        private ObservableCollection<TextItem> _nameBank = new ObservableCollection<TextItem>();

        public ObservableCollection<TextItem> NameBank
        {
            get { return _nameBank; }
            set
            {
                _nameBank = value;
                RaisePropertyChanged(nameof(NameBank));
            }
        }

        private ObservableCollection<TextItem> _prefixBank = new ObservableCollection<TextItem>();

        public ObservableCollection<TextItem> PrefixBank
        {
            get { return _prefixBank; }
            set
            {
                _prefixBank = value;
                RaisePropertyChanged(nameof(PrefixBank));
            }
        }

        private ObservableCollection<TextItem> _suffixBank = new ObservableCollection<TextItem>();

        public ObservableCollection<TextItem> SuffixBank
        {
            get { return _suffixBank; }
            set
            {
                _suffixBank = value;
                RaisePropertyChanged(nameof(SuffixBank));
            }
        }

        private ObservableCollection<TextItem> _placePrefixBank = new ObservableCollection<TextItem>();

        public ObservableCollection<TextItem> PlacePrefixBank
        {
            get { return _placePrefixBank; }
            set
            {
                _placePrefixBank = value;
                RaisePropertyChanged(nameof(PlacePrefixBank));
            }
        }

        private ObservableCollection<TextItem> _placeSuffixBank = new ObservableCollection<TextItem>();

        public ObservableCollection<TextItem> PlaceSuffixBank
        {
            get { return _placeSuffixBank; }
            set
            {
                _placeSuffixBank = value;
                RaisePropertyChanged(nameof(PlaceSuffixBank));
            }
        }


        void LoadDicts()
        {
            LoadBank(new string[] {
                "常见百家姓",
                "常用字2500字",
                "称号后缀",
                "太上洞玄灵宝三十二天天尊应号经",
                "称号前缀",
                "楚辞字库",
                "次常用字1000字",
                "地名后缀",
                "女性化字库",
                "千字文字库",
                "全唐诗字库",
                "诗经字库",
                "通用字表7000字",
                "完整百家姓",
                "玄幻百家姓",
                "周易字库",
            }, AllBank, new string[] { });

            LoadBank(new string[] {
                "常见百家姓",
                "完整百家姓",
                "玄幻百家姓",
            }, SurnameBank, new string[] { "常见百家姓" });

            LoadBank(new string[] {
                "楚辞字库",
                "诗经字库",
                "周易字库",
                "千字文字库",
                "女性化字库",
                "全唐诗字库",
                "常用字2500字",
                "次常用字1000字",
                "通用字表7000字"
            }, NameBank, new string[] { "楚辞字库", "诗经字库" });

            LoadBank(new string[] {
                "周易字库",
                "称号前缀",
                "太上洞玄灵宝三十二天天尊应号经",
            }, PrefixBank, new string[] { "称号前缀" });

            LoadBank(new string[] {
                "称号后缀"
            }, SuffixBank, new string[] { "称号后缀" });

            LoadBank(new string[] {
                "千字文字库", "周易字库",
            }, PlacePrefixBank, new string[] { "周易字库" });

            LoadBank(new string[] {
                "地名后缀"
            }, PlaceSuffixBank, new string[] { "地名后缀" });


            CurrentPrefixBank = SurnameBank;
            CurrentSuffixBank = NameBank;
            RefreshCurrentDict();
        }

        private void LoadBank(string[] banks, ObservableCollection<TextItem> bankName, string[] checks)
        {
            foreach (string fileName in banks)
            {
                TextItem textItem = new TextItem() { Title = fileName, Dict = GetListFormTXT("Resources/语料/取名工具/" + fileName + ".txt") };
                bankName.Add(textItem);
                if (checks.Contains(textItem.Title))
                {
                    textItem.Use = true;
                };
                textItem.PropertyChanged += TextItem_PropertyChanged;
            }
        }

        private void TextItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TextItem.Use))
            {
                RefreshCurrentDict();
            }
        }

        /// <summary>
        /// 刷新当前使用的两个字典（切换字库或者命名风格时使用）
        /// </summary>
        private void RefreshCurrentDict()
        {
            CurrentPrefixDict.Clear();
            CurrentSuffixDict.Clear();
            foreach (TextItem textItem in CurrentPrefixBank)
            {
                if (textItem.Use == true)
                {
                    CurrentPrefixDict = CurrentPrefixDict.Union(textItem.Dict).ToList<string>();
                }
            }
            foreach (TextItem textItem in CurrentSuffixBank)
            {
                if (textItem.Use == true)
                {
                    CurrentSuffixDict = CurrentSuffixDict.Union(textItem.Dict).ToList<string>();
                }
            }
            CurrentPrefixDictCount = CurrentPrefixDict.Count.ToString();
            CurrentSuffixDictCount = CurrentSuffixDict.Count.ToString();
        }


        /// <summary>
        /// 从字符串列表中随机取出一个字符串
        /// </summary>
        /// <param name="stringList"></param>
        /// <returns></returns>
        string GetStringFromList(List<string> stringList)
        {
            if (stringList.Count == 0)
            {
                return string.Empty;
            }
            int index = new Random(Guid.NewGuid().GetHashCode()).Next(0, stringList.Count());
            return stringList[index];
        }

        /// <summary>
        /// 合并多个字符串列表
        /// </summary>
        /// <param name="listArray"></param>
        /// <returns></returns>
        List<string> GetUnionList(List<List<string>> listArray)
        {
            List<string> listRet = new List<string>();
            foreach (List<string> listString in listArray)
            {
                listRet = listRet.Union(listString).ToList<string>();
            }
            return listRet;
        }

        List<string> GetListFormTXT(string filePath)
        {
            List<string> myList = new List<string>();
            Uri uri = new Uri(filePath, UriKind.RelativeOrAbsolute);
            try
            {
                Stream txtStream = Application.GetResourceStream(uri).Stream;
                StreamReader reader = new StreamReader(txtStream, UnicodeEncoding.GetEncoding("utf-8"));
                //按行读取
                string strLine;
                while ((strLine = reader.ReadLine()) != null)
                {
                    strLine = strLine.Trim().ToString();
                    myList.Add(strLine);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return myList;
        }

        /// <summary>
        /// 从字典读取拼音
        /// </summary>
        /// <param name="hanzi"></param>
        /// <returns></returns>
        public string ReadFromPinyinDict(string hanzi)
        {
            List<string> myList = GetListFormTXT("Resources/语料/拼音字典/PinyinDict.txt");
            foreach (string line in myList)
            {
                if (line.Contains(hanzi))
                {
                    return line;
                }
            }
            return null;
        }
    }
}
