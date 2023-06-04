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



        private void ThisTextEditor_TextChanged(object sender, EventArgs e)
        {
            if (Gval.Views.CurrentEditorkernel == null || Gval.Views.UcShower.Tag == null)
            {
                return;
            }
            (Gval.Views.CurrentEditorkernel.DataContext as Node).Summary = ThisTextEditor.Text;
            Gval.Views.CurrentEditorkernel.BtnSaveDoc.IsEnabled = true;
        }

        private void Command_Typesetting_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditorHelper.TypeSetting(ThisTextEditor);
        }

        private void ThisTextEditor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ThisTextEditor.TextArea.Document.Insert(ThisTextEditor.CaretOffset, "\n　　");
                ThisTextEditor.LineDown();
            }
            //逗号||句号的情况
            if (e.Key == Key.OemComma ||
                e.Key == Key.OemPeriod)
            {

            }
        }
    }
}
