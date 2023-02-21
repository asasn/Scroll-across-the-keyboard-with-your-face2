using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace RootNS.Helper
{
    internal class JsonHelper
    {
        /// <summary>
        /// 把Obj对象转换成Json字符串
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static string Otj<T>(T obj)
        {
            JavaScriptSerializer jserializer = new JavaScriptSerializer();
            return jserializer.Serialize(obj);
        }
        /// <summary>
        /// 从Json字符串中获取对象属性以载入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static T Jto<T>(string strJson)
        {
            JavaScriptSerializer jserializer = new JavaScriptSerializer();
            try
            {
                return jserializer.Deserialize<T>(strJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("解析错误 - {0}！", ex));
                return default;
            }
        }
    }
}
