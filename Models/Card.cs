using RootNS.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RootNS.Models.Card.Line;

namespace RootNS.Models
{
    public class Card : NotificationObject
    {
        public Card()
        {
            //ToDo：可能需要优化一下卡片模型的设计，把散乱的属性整合打包
            string[] ls = new string[] { "别称", "身份", "外观", "阶级", "所属", "物品", "能力", "经历" };
            foreach (string lineTitle in ls)
            {
                Line line = new Line
                {
                    Title = lineTitle
                };
                Lines.Add(line);
            }
        }

        private bool _hasChange;
        /// <summary>
        /// 运行时变量：供信息卡使用
        /// </summary>
        public bool HasChange
        {
            get { return _hasChange; }
            set
            {
                _hasChange = value;
                RaisePropertyChanged(nameof(HasChange));
            }
        }

        private string _tag = "角色";
        /// <summary>
        /// 卡片的类型标记
        /// </summary>
        public string Tag
        {
            get { return _tag; }
            set
            {
                _tag = value;
                RaisePropertyChanged(nameof(Tag));
            }
        }

        private ObservableCollection<Line> _lines = new ObservableCollection<Line>();

        public ObservableCollection<Line> Lines
        {
            get { return _lines; }
            set
            {
                _lines = value;
                RaisePropertyChanged(nameof(Lines));
            }
        }


        /// <summary>
        /// 清除没有内容的Tip元素
        /// </summary>
        public void ClaerNullTips()
        {
            foreach (Card.Line line in this.Lines)
            {
                //当移除完元素之后，数组大小发生了变更，会抛出异常，所以在这里使用逆序遍历来进行
                for (int i = line.Tips.Count - 1; i >= 0; i--)
                {
                    if (string.IsNullOrEmpty(line.Tips[i].Content))
                    {
                        line.Tips.Remove(line.Tips[i]);
                    }
                }
            }
        }


        /// <summary>
        /// 隐藏一部分没有元素的行
        /// </summary>
        public void HiddenNullLines()
        {
            foreach (Card.Line line in this.Lines)
            {
                bool allNull = true;//假设全部为空，只要有一个反例就可以跳出循环
                foreach (Card.Line.Tip tip in line.Tips)
                {
                    if (string.IsNullOrEmpty(tip.Content) == false)
                    {
                        allNull = false;
                        break;
                    }
                }
                if (allNull)
                {
                    line.Visibility = false;
                }
                else
                {
                    line.Visibility = true;
                }
            }
            //if (BtnSeeMore.Visibility == Visibility.Visible)
            //{
            //    //隐藏模式时，进行一些处理
            //    bool allLineNull = true;
            //    foreach (Card.Line line in ThisCard.Lines)
            //    {
            //        if (line.Visibility)
            //        {
            //            allLineNull = false;
            //            break;
            //        }
            //        if (allLineNull)
            //        {
            //            GMainLines.Visibility = Visibility.Collapsed;
            //        }
            //    }
            //}
        }



        public class Line : NotificationObject
        {
            private bool _visibility;

            public bool Visibility
            {
                get { return _visibility; }
                set
                {
                    _visibility = value;
                    RaisePropertyChanged(nameof(Visibility));
                }
            }


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

            public class Tip : NotificationObject
            {
                private string _tipContent;

                public string Content
                {
                    get { return _tipContent; }
                    set
                    {
                        _tipContent = value;
                        RaisePropertyChanged(nameof(Content));
                    }
                }

            }

            private ObservableCollection<Tip> _tips = new ObservableCollection<Tip>();

            public ObservableCollection<Tip> Tips
            {
                get { return _tips; }
                set
                {
                    _tips = value;
                    RaisePropertyChanged(nameof(Tips));
                }
            }


        }



    }
}
