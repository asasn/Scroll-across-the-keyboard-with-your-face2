using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RootNS.Helper
{
    /// <summary>
    /// 通用帮助类，包含各种功能
    /// </summary>
    public class CommonHelper
    {
        public class Count
        {
            /// <summary>
            /// 统计汉字字数
            /// </summary>
            /// <param name="StrContert"></param>
            /// <returns></returns>
            public static int GetChineseLeng(string StrContert)
            {
                if (string.IsNullOrEmpty(StrContert))
                    return 0;
                //此处进行的是一些转义符的替换,以免照成统计错误.
                StrContert = StrContert.Replace("\n", "").Replace("\r", "").Replace("\t", "");
                //替换掉内容中的数字,英文,空格.方便进行字数统计.
                int iChinese = getByteLengthByChinese(Regex.Matches(StrContert, @"[A-Za-z0-9][A-Za-z0-9'\-.]*").ToString().Replace("　", "").Replace(" ", ""));
                return Regex.Matches(StrContert, @"[0-9][0-9'\-.]*").Count + iChinese;//中文中的字数统计需要加上数字统计.
            }
            /// <summary>
            /// 返回字节数
            /// </summary>
            /// <param name="s"></param>
            /// <returns></returns>
            private static int getByteLengthByChinese(string s)
            {
                int l = 0;
                char[] q = s.ToCharArray();
                for (int i = 0; i < q.Length; i++)
                {
                    if ((int)q[i] >= 0x4E00 && (int)q[i] <= 0x9FA5) // 汉字
                    {
                        l += 1;
                    }
                }
                return l;
            }

            /// <summary>
            /// 起点中文网的统计
            /// </summary>
            /// <param name="content"></param>
            /// <returns></returns>
            public static int QiDianCount(string input)
            {
                if (string.IsNullOrEmpty(input))
                {
                    return 0;
                }
                // 将特定符号替换为一个■
                input = Regex.Replace(input, @"(——)|[…]", "■", RegexOptions.Multiline);

                // 将连续的英文单词替换为一个■
                input = Regex.Replace(input, @"([a-zA-Z\p{P}][a-zA-Z\p{P}]+)", "■", RegexOptions.Multiline);

                // 将连续的数字替换为一个■
                input = Regex.Replace(input, @"([0-9\p{P}][0-9\p{P}]+)", "■", RegexOptions.Multiline);

                // 去除不可见符号和全角/半角空格
                input = Regex.Replace(input, @"[\s]", "", RegexOptions.Multiline);

                // 返回字符串中空格的数量加1
                return input.Length;
            }

            /// <summary>
            /// 自定义的非空字符统计
            /// </summary>
            /// <param name="content"></param>
            /// <returns></returns>
            public static int CountWords(string content)
            {
                if (string.IsNullOrEmpty(content))
                {
                    return 0;
                }
                int total = 0;
                char[] q = content.ToCharArray();
                for (int i = 0; i < q.Length; i++)
                {
                    if (q[i] > 32 && q[i] != 0xA0 && q[i] != 0x3000) // 非空字符，Unicode编码 0x4E0 - 0x9FA5 为汉字，0x3000为全角空格
                    {
                        total += 1;
                    }
                }
                return total;
            }
        }
    }
}
