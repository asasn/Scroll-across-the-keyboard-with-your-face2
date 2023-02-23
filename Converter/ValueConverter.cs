using RootNS.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RootNS.Converter
{
    /// <summary>
    /// 动态设置编辑器外壳TabControl的最大宽度
    /// </summary>
    public class SetEditorMaxWidth : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return Math.Abs(Math.Min((Double)values[0] - ((Double)values[1] * 2) - 280 - 120 - 50, 900));
            }
            catch (Exception)
            {
                return 700;
            }
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// 动态设置目录树外壳GroupBox的最大宽度
    /// </summary>
    public class SetTreeMaxWidth : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return Math.Abs(Math.Min(((Double)values[0] - (Double)values[1] - 280 - 120 - 50) / 2, 380));
            }
            catch (Exception)
            {
                return 264;
            }
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    ///// <summary>
    ///// TypeName决定按钮是否显现
    ///// </summary>
    //public class TypeName2CountVisibility : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        if (value == null)
    //        {
    //            return Visibility.Collapsed;
    //        }
    //        try
    //        {
    //            if (value.ToString() == Book.TypeNameEnum.本书作品相关.ToString())
    //            {
    //                return Visibility.Collapsed;
    //            }
    //            if (value.ToString() == Book.TypeNameEnum.本书已发布.ToString())
    //            {
    //                return Visibility.Collapsed;
    //            }
    //            return Visibility.Visible;
    //        }
    //        catch
    //        {
    //            return Visibility.Collapsed;
    //        }
    //    }

    //    //这里只有在TwoWay的时候才有用
    //    public object ConvertBack(object value, Type targetType, object parameter,
    //     System.Globalization.CultureInfo culture)
    //    {
    //        return null;
    //    }
    //}

    ///// <summary>
    ///// TypeName决定按钮是否可用
    ///// </summary>
    //public class TypeName2IsEnabled : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        if (value == null)
    //        {
    //            return true;
    //        }
    //        try
    //        {
    //            if (value.ToString() == Book.TypeNameEnum.草稿.ToString())
    //            {
    //                return false;
    //            }

    //            return true;
    //        }
    //        catch
    //        {
    //            return true;
    //        }
    //    }

    //    //这里只有在TwoWay的时候才有用
    //    public object ConvertBack(object value, Type targetType, object parameter,
    //     System.Globalization.CultureInfo culture)
    //    {
    //        return null;
    //    }
    //}
}
