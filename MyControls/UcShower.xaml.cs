using ICSharpCode.AvalonEdit;
using JiebaNet.Segmenter;
using RootNS.Helper;
using RootNS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RootNS.MyControls
{
    /// <summary>
    /// UcShower.xaml 的交互逻辑
    /// </summary>
    public partial class UcShower : UserControl
    {
        public UcShower()
        {
            InitializeComponent();
        }

        private void Command_Typesetting_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ThisTextEditor_TextChanged(object sender, EventArgs e)
        {
            if (this.DataContext == null)
            {
                return;
            }
            TextEditor summaryTextEditor = ((Gval.Views.EditorTabControl.SelectedItem as HandyControl.Controls.TabItem).Content as Editorkernel).SummaryTextEditor;
            summaryTextEditor.Text = ThisTextEditor.Text;
        }
    }
}
