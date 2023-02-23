using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RootNS.Helper
{
    public class FileIO
    {
        /// <summary>
        /// 获取图像对象
        /// </summary>
        /// <param name="imgPath"></param>
        /// <returns></returns>
        public static BitmapImage GetImgObject(string imgPath)
        {
            if (false == IsFileExists(imgPath))
            {
                return null;
            }
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(imgPath);
            bitmap.EndInit();
            return bitmap.Clone();
        }


        /// <summary>
        /// 创建新文件夹
        /// </summary>
        /// <param name="srcFolderPath"></param>
        public static void TryToCreateFolder(string srcFolderPath)
        {
            string path = System.IO.Path.GetDirectoryName(srcFolderPath);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            if (false == IsFolderExists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 创建新文档
        /// </summary>
        /// <param name="newDocFullName"></param>
        public static void CreateNewDoc(string newDocFullName)
        {
            WriteToTxt(newDocFullName, "");
        }

        /// <summary>
        /// 判断文件夹路径是否存在
        /// </summary>
        /// <param name="srcFolderPath"></param>
        /// <returns></returns>
        public static bool IsFolderExists(string srcFolderPath)
        {
            return Directory.Exists(srcFolderPath);
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public static bool IsFileExists(string fullFileName)
        {
            return File.Exists(fullFileName);
        }

        /// <summary>
        /// 文件夹非法字符集合
        /// </summary>
        public static char[] invalidCharsInFolderName = Path.GetInvalidPathChars();

        /// <summary>
        /// 文件名非法字符集合
        /// </summary>
        public static char[] invalidCharsInFileName = Path.GetInvalidFileNameChars();

        /// <summary>
        /// 方法：替换文件名非法字符
        /// </summary>
        /// <param name="name">填入的名称</param>
        /// <returns>合法字符</returns>
        public static string ReplaceFileName(string name)
        {
            name = name.Replace("/", "／");
            name = name.Replace("\\", "＼");
            name = name.Replace(":", "：");
            name = name.Replace("*", "※");
            name = name.Replace("?", "？");
            name = name.Replace("\"", "“");
            name = name.Replace("<", "＜");
            name = name.Replace(">", "＞");
            name = name.Replace("|", "│");
            name = name.Trim(new char[] { '.' }); //过滤首尾字符'.'
            return name;
        }

        /// <summary>
        /// 方法：删除文档（可设置是否至回收站）
        /// </summary>
        /// <param name="fullFileName">完整的文件名</param>
        public static void DeleteFile(string fullFileName)
        {
            if (File.Exists(fullFileName))
            {
                FileSystem.DeleteFile(fullFileName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
        }

        /// <summary>
        /// 方法：删除文件夹（可设置是否至回收站）
        /// </summary>
        /// <param name="fullFolderName">完整的文件夹名</param>
        public static void DeleteDirectory(string fullFolderName)
        {
            if (System.IO.Directory.Exists(fullFolderName))
            {
                FileSystem.DeleteDirectory(fullFolderName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
        }

        /// <summary>
        /// 方法：文件夹重命名
        /// </summary>
        /// <param name="srcFolderPath">原文件夹</param>
        /// <param name="destFolderPath">新文件夹</param>
        public static void RenameDir(string srcFolderPath, string destFolderPath)
        {
            //来源文件夹存在，目标文件夹不存在，且来源和目标文件夹不相同
            if (System.IO.Directory.Exists(srcFolderPath) && !System.IO.Directory.Exists(destFolderPath) && srcFolderPath != destFolderPath)
            {
                System.IO.Directory.Move(srcFolderPath, destFolderPath);
            }
        }

        /// <summary>
        /// 方法：文件重命名
        /// </summary>
        /// <param name="fOld"></param>
        /// <param name="fNew"></param>
        public static void RenameFile(string fOld, string fNew)
        {
            //原文件存在，且改名后的新文件不存在
            if (File.Exists(fOld) && false == File.Exists(fNew))
            {
                File.Move(fOld, fNew);
            }
        }

        /// <summary>
        /// 方法：写入TXT文件
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <param name="content"></param>
        public static void WriteToTxt(string fullFileName, string content)
        {
            if (string.IsNullOrEmpty(fullFileName))
                return;
            FileStream fs = new FileStream(fullFileName, FileMode.OpenOrCreate, FileAccess.Write);

            if (File.Exists(fullFileName))
            {
                fs.SetLength(0); //先清空文件
            }
            StreamWriter sw = new StreamWriter(fs, new UTF8Encoding(true));
            sw.Write(content);   //写入字符串
            sw.Close();
            Console.WriteLine("保存至：" + fullFileName);
        }

        /// <summary>
        /// 方法：读取TXT文件
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public static string ReadFromTxt(string fullFileName)
        {
            if (false == File.Exists(fullFileName))
                return string.Empty;
            StreamReader sr = new StreamReader(fullFileName, GetEncoding(fullFileName));
            string text = sr.ReadToEnd();
            sr.Close();
            return text;
        }


        /// <summary>
        /// 获取文件编码
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Encoding GetEncoding(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            Encoding r = GetType(fs);
            fs.Close();
            return r;
        }



        /// <summary>
        /// 通过给定的文件流，判断文件的编码类型
        /// </summary>
        /// <param name=“fs“>文件流</param>
        /// <returns>文件的编码类型</returns>
        private static Encoding GetType(Stream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }

        /// <summary>
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// </summary>
        /// <param name=“data“></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }

    }
}
