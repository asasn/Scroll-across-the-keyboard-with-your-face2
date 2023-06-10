using RootNS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RootNS.Helper
{
    internal class WebdavHelper
    {
        static string GetFileMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hashBytes = md5.ComputeHash(stream);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }
            }
        }

        public static string GetEtag(string remoteFile, string userName, string passWord)
        {

            System.Net.WebResponse res;
            string saveFilePath;
            try
            {
                System.Net.WebProxy clsProxy = new System.Net.WebProxy();
                clsProxy.BypassProxyOnLocal = true;
                string strPassUrl = remoteFile.Substring(0, remoteFile.IndexOf(@"\"));
                saveFilePath = Gval.Path.DataDirectory + "\\" + remoteFile.Substring(strPassUrl.Length, remoteFile.Length - strPassUrl.Length);
                clsProxy.BypassList = new string[] { strPassUrl };
                Uri clsUri = new Uri(remoteFile);
                System.Net.WebRequest req = System.Net.WebRequest.Create(clsUri);
                req.Proxy = clsProxy;
                req.Method = "GET";
                req.Timeout = 15000;
                req.Credentials = new System.Net.NetworkCredential(userName, passWord);
                res = req.GetResponse();
                string eTag = res.Headers["ETag"];
                return eTag;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static byte[] DownloadWebDavFile(string remoteFile, string userName, string passWord, string localFilePath)
        {
            System.Net.WebResponse res;
            System.IO.Stream inStream;
            string saveFilePath;
            try
            {
                System.Net.WebProxy clsProxy = new System.Net.WebProxy();
                clsProxy.BypassProxyOnLocal = true;
                string strPassUrl = remoteFile.Substring(0, remoteFile.IndexOf(@"\"));
                saveFilePath = Gval.Path.DataDirectory + "\\" + remoteFile.Substring(strPassUrl.Length, remoteFile.Length - strPassUrl.Length);
                clsProxy.BypassList = new string[] { strPassUrl };
                Uri clsUri = new Uri(remoteFile);
                System.Net.WebRequest req = System.Net.WebRequest.Create(clsUri);
                req.Proxy = clsProxy;
                req.Method = "GET";
                req.Timeout = 15000;
                req.Credentials = new System.Net.NetworkCredential(userName, passWord);
                res = req.GetResponse();
                inStream = res.GetResponseStream();
                BinaryReader reader = new BinaryReader(inStream);
                byte[] btyChunk = new byte[4096];
                byte[] buffer = new byte[(int)res.ContentLength];
                int count = 0;
                int i = 0;
                while ((count = reader.Read(btyChunk, 0, btyChunk.Length)) > 0)
                {
                    Array.Copy(btyChunk, 0, buffer, i, count);
                    i = i + count;
                }
                if (string.IsNullOrEmpty(saveFilePath) == false)
                {
                    File.WriteAllBytes(saveFilePath, buffer);
                }
                return buffer;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string UploadWebDavFile(string _WebFileUrl, string _LocalFile, string _UserName, string _Password, string localFilePath)
        {
            try
            {
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)WebRequest.Create(_WebFileUrl);
                req.Credentials = new NetworkCredential(_UserName, _Password);
                req.PreAuthenticate = true;
                req.Method = "PUT";
                req.Timeout = 15000;
                req.AllowWriteStreamBuffering = true;
                Stream reqStream = req.GetRequestStream();
                FileStream rdm = new FileStream(_LocalFile, FileMode.Open);
                byte[] inData = new byte[4096];
                int byteRead = rdm.Read(inData, 0, inData.Length);
                while (byteRead > 0)
                {
                    reqStream.Write(inData, 0, byteRead);
                    byteRead = rdm.Read(inData, 0, inData.Length);
                }
                rdm.Close();
                reqStream.Close();
                req.GetResponse();
            }
            catch (Exception e)
            {
                return null;
            }
            string eTag = GetEtag(_WebFileUrl, _UserName, _Password);
            return eTag;
        }

        public static int DeleteWebDavFile(string _WebFileUrl, string _UserName, string _Password)
        {
            try
            {
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)WebRequest.Create(_WebFileUrl);
                req.Credentials = new NetworkCredential(_UserName, _Password);
                req.PreAuthenticate = true;
                req.Method = "DELETE";
                req.Timeout = 15000;
                req.AllowWriteStreamBuffering = true;
                req.GetResponse();
            }
            catch (Exception e)
            {
                return 0;
            }
            return 1;
        }


    }
}
