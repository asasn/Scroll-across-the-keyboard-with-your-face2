﻿using RootNS.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNS.Models
{
    public partial class Gval : NotificationObject
    {

        private static bool _flagLoadingCompleted;
        /// <summary>
        /// 完成标志：载入
        /// </summary>
        public static bool FlagLoadingCompleted
        {
            get { return _flagLoadingCompleted; }
            set
            {
                _flagLoadingCompleted = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(FlagLoadingCompleted)));
            }
        }

        private static Book _currentBook = new Book();//不指定Guid
        /// <summary>
        /// 当前书籍
        /// </summary>
        public static Book CurrentBook
        {
            get { return _currentBook; }
            set
            {
                _currentBook = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(CurrentBook)));
            }
        }

        //生成资料库时，指定一个固定的值作为Guid；
        private static Book _materialBook = new Book() { Guid = new Guid("00000000-1111-1111-1111-000000000000"), Title = "资料库" };
        /// <summary>
        /// 资料库
        /// </summary>
        public static Book MaterialBook
        {
            get { return _materialBook; }
            set
            {
                _materialBook = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(MaterialBook)));
            }
        }


        private static ObservableCollection<Book> _booksBank = new ObservableCollection<Book>();
        /// <summary>
        /// 书库集合
        /// </summary>
        public static ObservableCollection<Book> BooksBank
        {
            get { return _booksBank; }
            set
            {
                _booksBank = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(BooksBank)));
            }
        }

        private static ObservableCollection<Node> _openingDocList = new ObservableCollection<Node>();
        /// <summary>
        /// 打开文档的集合
        /// </summary>
        public static ObservableCollection<Node> OpeningDocList
        {
            get { return _openingDocList; }
            set
            {
                _openingDocList = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(OpeningDocList)));
            }
        }

        private static string _previousText = string.Empty;
        /// <summary>
        /// 搜索查找的文字（上一个）
        /// </summary>
        public static string PreviousText
        {
            get { return _previousText; }
            set
            {
                _previousText = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(PreviousText)));
            }
        }
    }
}
