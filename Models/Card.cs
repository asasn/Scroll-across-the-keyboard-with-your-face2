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
            string[] ls = new string[] { "别称", "身份", "外观", "阶级", "所属", "物品", "能力", "经历" };
            foreach (string lineTitle in ls)
            {
                Line line = new Line();
                line.Title = lineTitle;
                Lines.Add(line);
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
