using RootNS.Models;
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
    /// 多路绑定：根据是否目录与是否展开的布尔值判断图标
    /// </summary>
    public class Bool2IconString : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
            {
                return null;
            }
            try
            {
                if ((bool)values[0] == true)
                {
                    if ((bool)values[1] == true)
                    {
                        return "\ue96c";
                    }
                    else
                    {
                        return "\ue96c";
                    }
                }
                else
                {
                    return "\ue968";
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }


    /// <summary>
    /// 删除属性决定透明度
    /// </summary>
    public class IsDelConvertOpacity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return 1;
            }
            try
            {
                //删除为真的时候，进行设置
                if ((bool)value == true)
                {
                    return 0.5;
                }
                else
                {
                    return 1;
                }
            }
            catch
            {
                return 1;
            }
        }

        //这里只有在TwoWay的时候才有用
        public object ConvertBack(object value, Type targetType, object parameter,
         System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }


    /// <summary>
    /// 布尔值决定是否Collapsed（卡片缩起）
    /// </summary>
    public class BoolConvertToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            try
            {
                if ((bool)value == true)
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
        public object ConvertBack(object value, Type targetType, object parameter,
         System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// 布尔值决定是否Collapsed（卡片缩起）
    /// </summary>
    public class BoolConvertToReVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Visible;
            }
            try
            {
                if ((bool)value == true)
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
}
