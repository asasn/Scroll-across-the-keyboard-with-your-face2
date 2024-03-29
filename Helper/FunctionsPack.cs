﻿using RootNS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Management;
using System.Text.RegularExpressions;
using System.Web;

namespace RootNS.Helper
{
    /// <summary>
    /// 函数包
    /// </summary>
    public class FunctionsPack
    {

        /// <summary>
        /// 根据名称查找命令
        /// </summary>
        /// <param name="commandBindings"></param>
        /// <param name="commandName"></param>
        /// <returns></returns>
        public static ICommand FindCommandByName(CommandBindingCollection commandBindings, string commandName)
        {
            foreach (CommandBinding cb in commandBindings)
            {
                RoutedCommand rc = cb.Command as RoutedCommand;
                if (commandName == rc.Name)
                {
                    return cb.Command;
                }
            }
            return null;
        }
        public static void ShowMessageBox(string text)
        {
            MessageBox.Show(text, "提示", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
        }

        /// <summary>
        /// 判断当前是否处于设计模式
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static bool IsInDesignMode(System.Windows.Controls.Control control)
        {
            return System.ComponentModel.DesignerProperties.GetIsInDesignMode(control);
        }

        /// <summary>
        /// 获取文件MD5值
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <returns>MD5值</returns>
        public static string GetMD5HashFromFile(string filePath)
        {
            try
            {
                FileStream file = new FileStream(filePath, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 弹窗位置X坐标
        /// </summary>
        /// <param name="thisWin"></param>
        /// <param name="uc"></param>
        public static void ForViewPointX(Window thisWin, UIElement uc, double offset = 0)
        {
            thisWin.Left = uc.TranslatePoint(new Point(), Gval.Views.MainWindow).X + offset;
        }

        /// <summary>
        /// 弹窗位置Y坐标
        /// </summary>
        /// <param name="thisWin"></param>
        /// <param name="uc"></param>
        /// <param name="offset"></param>
        public static void ForViewPointY(Window thisWin, UIElement uc, double offset = 0)
        {
            thisWin.Top = uc.TranslatePoint(new Point(), Gval.Views.MainWindow).Y + offset;
        }

        /// <summary>
        /// 版本检查
        /// </summary>
        public static void CheckVersion()
        {
            DateTime now = DateTime.Now;
            string lastCheckHour = now.ToString("yyyy-MM-dd HH");
            object record = Settings.Get(Gval.MaterialBook, Gval.SettingsKeys.LastCheckHour);
            if (record != null)
            {
                Gval.LastCheckHour = Convert.ToString(record);
            }
            if (Gval.LastCheckHour == lastCheckHour)
            {
                return;
            }
            else
            {
                Gval.LastCheckHour = lastCheckHour;
                Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.LastCheckHour, lastCheckHour);
            }

            StreamReader reader = WebHelper.GetHtmlReaderObject(Gval.Url.Latest);
            if (reader == null)
            {
                Gval.LatestVersion = "网络错误！";
                return;
            }
            else
            {
                string text = reader.ReadToEnd();
                Dictionary<string, object> latestInfo = JsonHelper.Jto<Dictionary<string, object>>(text);
                string versionName = latestInfo["name"].ToString();
                Match match = Regex.Match(text, "\\d+\\.\\d+\\.\\d+\\.\\d+");
                if (match.Success)
                {
                    Gval.LatestVersion = HttpUtility.UrlDecode(match.Value);
                }
            }
            if (Gval.CurrentVersion == Gval.LatestVersion)
            {

            }
            else if (Gval.LatestVersion != "网络错误！" && Gval.LatestVersion != "未检查")
            {
                Gval.HasNewVersion = true;
            }
        }



    }
}
