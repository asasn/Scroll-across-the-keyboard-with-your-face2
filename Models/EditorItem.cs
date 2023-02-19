using RootNS.Helper;
using RootNS.MyControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RootNS.Models
{
    public class EditorItem: NotificationObject
    {
        private Editorkernel _tabContent;

        public Editorkernel TabContent
        {
            get { return _tabContent; }
            set
            {
                _tabContent = value;
                RaisePropertyChanged(nameof(TabContent));
            }
        }



        private Node _node;

        public Node Node
        {
            get { return _node; }
            set
            {
                _node = value;
                RaisePropertyChanged(nameof(Node));
            }
        }


        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged(nameof(IsSelected));
            }
        }

    }
}
