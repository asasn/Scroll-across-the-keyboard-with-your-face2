﻿using RootNS.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNS.Models
{
    public partial class Gval : NotificationObject
    {
        /// <summary>
        /// 设置的键名
        /// </summary>
        public struct SettingsKeys
        {
            public static string CurrentBookGuid { get { return "CurrentBookGuid"; } }
            public static string CurrentThemeColor { get { return "CurrentThemeColor"; } }
            public static string TomatoTimeSetTotalMinutes { get { return "TomatoTimeSetTotalMinutes"; } }
            public static string Scroll2End { get { return "Scroll2End"; } }
            public static string FontSize { get { return "FontSize"; } }
            public static string FontFamily { get { return "FontFamily"; } }
            public static string TextAreaMargin { get { return "TextAreaMargin"; } }
            public static string IsShowLineNumbers { get { return "ShowLineNumbers"; } }
            public static string WebdavUrl { get { return "WebdavUrl"; } }
            public static string WebdavUserName { get { return "WebdavUserName"; } }
            public static string WebdavPassWord { get { return "WebdavPassWord"; } }
            public static string CursorToEnd { get { return "CursorToEnd"; } }
            public static string ThisBookTotalNodesCount { get { return "ThisBookTotalNodesCount"; } }
            public static string EditorColorTags { get { return "EditorColorTags"; } }
            public static string HasBook { get { return "HasBook"; } }
            public static string IsWarnAgain { get { return "IsWarnAgain"; } }
            public static string LastCheckHour { get { return "LastCheckHour"; } }
            public static string VerifyCode { get { return "VerifyCode"; } }
            public static string ShowNoVerify { get { return "ShowNoVerify"; } }
        }
        public static string VerifyCode { get; set; }

        private static bool _showNoVerify = true;

        public static bool ShowNoVerify
        {
            get { return _showNoVerify; }
            set
            {
                _showNoVerify = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(ShowNoVerify)));
            }
        }


        public static string LastCheckHour { get; set; }


        private static bool _isWarnAgain = true;

        public static bool IsWarnAgain
        {
            get { return _isWarnAgain; }
            set
            {
                _isWarnAgain = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(IsWarnAgain)));
            }
        }

        private static bool _hasBook = false;

        public static bool HasBook
        {
            get { return _hasBook; }
            set
            {
                _hasBook = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(HasBook)));
            }
        }


        private static Dictionary<string, object> _editorColorTags;

        public static Dictionary<string, object> EditorColorTags
        {
            get { return _editorColorTags; }
            set
            {
                _editorColorTags = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(EditorColorTags)));
            }
        }


        public struct EditorSettings
        {
            public static bool CursorToEnd { get; set; } = true;
        }

        /// <summary>
        /// Webdav相关
        /// </summary>

        public struct Webdav
        {
            public static string Url { get; set; }
            public static string UserName { get; set; }
            public static string PassWord { get; set; }
        }
    }
}
