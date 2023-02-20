using RootNS.Models;
using RootNS.MyControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
                return new NodeItem();
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
                return new NodeItem();
            }
            catch
            {
                return new NodeItem();
            }
        }

        //这里只有在TwoWay的时候才有用
        public object ConvertBack(object value, Type targetType, object parameter,
         System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }



    /// <summary>
    /// TypeName和是否文件夹决定统计单位
    /// </summary>
    public class TypeName2ShowText : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
            {
                return null;
            }
            try
            {
                if (values[0].ToString() == Book.TypeNameEnum.信息卡.ToString())
                {
                    return null;
                }

                //是目录的时候，显示为“章”，否则显示为“字”
                if ((bool)values[1] == true)
                {
                    return "章";
                }
                else
                {
                    return "字";
                }
            }
            catch
            {
                return null;
            }
        }

        //这里只有在TwoWay的时候才有用
        public object[] ConvertBack(object value, Type[] targetType, object parameter,
         System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// TypeName决定是否显示统计
    /// </summary>
    public class TypeName2CountVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Visible;
            }
            try
            {
                if (value.ToString() == Book.TypeNameEnum.大事记.ToString() ||
                    value.ToString() == Book.TypeNameEnum.故事大纲.ToString() ||
                    value.ToString() == Book.TypeNameEnum.情节设计.ToString() ||
                    value.ToString() == Book.TypeNameEnum.信息卡.ToString() ||
                    value.ToString() == Book.TypeNameEnum.全局题材管理.ToString() ||
                    value.ToString() == Book.TypeNameEnum.全局情节设计.ToString() ||
                    value.ToString() == Book.TypeNameEnum.全局信息卡.ToString()
                    )
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
            catch
            {
                return Visibility.Visible;
            }
        }

        //这里只有在TwoWay的时候才有用
        public object ConvertBack(object value, Type targetType, object parameter,
         System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// TypeName和是否文件夹决定CheckBox是否显现
    /// </summary>
    public class TypeName2CheckBoxVisibility : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
            {
                return Visibility.Collapsed;
            }
            try
            {
                if (values[0].ToString() == Book.TypeNameEnum.故事大纲.ToString() && (bool)values[1] == false)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
            catch
            {
                return Visibility.Collapsed;
            }
        }

        //这里只有在TwoWay的时候才有用
        public object[] ConvertBack(object value, Type[] targetType, object parameter,
         System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
