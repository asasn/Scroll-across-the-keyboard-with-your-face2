﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace RootNS.Properties {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RootNS.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性，对
        ///   使用此强类型资源类的所有资源查找执行重写。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 不问苍生问鬼神 的本地化字符串。
        /// </summary>
        public static string AppAuthor {
            get {
                return ResourceManager.GetString("AppAuthor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 https://www.lkong.com/user/584564 的本地化字符串。
        /// </summary>
        public static string ContactWay {
            get {
                return ResourceManager.GetString("ContactWay", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 https://github.com/asasn/Scroll-across-the-keyboard-with-your-face2 的本地化字符串。
        /// </summary>
        public static string Homepage {
            get {
                return ResourceManager.GetString("Homepage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 获取本书的章节大纲
        ///编辑时可自动对齐，但章节标题必须为第xx章：这样的格式，然后换新行写章纲 的本地化字符串。
        /// </summary>
        public static string TipBtnOutlines {
            get {
                return ResourceManager.GetString("TipBtnOutlines", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 获取本书的卡片名称和别名（用于高亮匹配或者语音输入） 的本地化字符串。
        /// </summary>
        public static string TipBtnPackage {
            get {
                return ResourceManager.GetString("TipBtnPackage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 信息卡管理（本书）
        ///目录标题的匹配关键词：
        ///角色、龙套、势力、部门、地区、场景、道具、其他
        ///目录标题中包含以上关键词都能自动匹配，不建议同一标题包含多个关键词，比如龙套角色。 的本地化字符串。
        /// </summary>
        public static string TipRoleCard {
            get {
                return ResourceManager.GetString("TipRoleCard", resourceCulture);
            }
        }
    }
}
