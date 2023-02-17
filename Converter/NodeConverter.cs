using RootNS.Models;
using RootNS.MyControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RootNS.Converter
{
    /// <summary>
    /// TabName决定节点模板
    /// </summary>
    public class NodeConvertToNodeTemplate : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return new NodeDoc();
            }
            Node node = (Node)value;
            try
            {
                //if (node.TabName == Material.MaterialTabName.题材.ToString() && node.IsDir == false)
                //{
                //    return new NodeItemForTopic();
                //}
                //if ((node.TabName == Book.NoteTabName.模板.ToString() ||
                //    node.TabName == Material.MaterialTabName.范文.ToString() ||
                //    node.TabName == Material.MaterialTabName.资料.ToString() ||
                //    node.TabName == Material.MaterialTabName.灵感.ToString()
                //    ) && node.IsDir == false)
                //{
                //    return new NodeItemForMaterial();
                //}
                return new NodeDoc();
            }
            catch
            {
                return new NodeDoc();
            }
        }

        //这里只有在TwoWay的时候才有用
        public object ConvertBack(object value, Type targetType, object parameter,
         System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
