using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RootNS.Helper
{
    internal class WebdavHelper
    {
        public static byte[] DownloadWebDavFile(string remoteFile, string userName, string passWord)
        {
            System.Net.WebProxy clsProxy = new System.Net.WebProxy();
            clsProxy.BypassProxyOnLocal = true;
            string strPassUrl = remoteFile.Substring(0, remoteFile.IndexOf(@"\"));
            clsProxy.BypassList = new string[] { strPassUrl };
            Uri clsUri = new Uri(remoteFile);
            System.Net.WebRequest req = System.Net.WebRequest.Create(clsUri);
            req.Proxy = clsProxy;
            req.Method = "GET";
            req.Credentials = new System.Net.NetworkCredential(userName, passWord);
            System.Net.WebResponse res = null;
            try
            {
                res = req.GetResponse();
            }
            catch (Exception ex)
            {
                return null;
            }
            System.IO.Stream inStream = res.GetResponseStream();
            BinaryReader reader = new BinaryReader(inStream);
            byte[] btyChunk = new byte[4096];
            byte[] buffer = new byte[(int)res.ContentLength];
            try
            {
                int count = 0;
                int i = 0;
                while ((count = reader.Read(btyChunk, 0, btyChunk.Length)) > 0)
                {
                    Array.Copy(btyChunk, 0, buffer, i, count);
                    i = i + count;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return buffer;
        }

        public static int UploadWebDavFile(string _WebFileUrl, string _LocalFile, string _UserName, string _Password)
        {
            try
            {
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)WebRequest.Create(_WebFileUrl);
                req.Credentials = new NetworkCredential(_UserName, _Password);
                req.PreAuthenticate = true;
                req.Method = "PUT";
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
                return 0;
            }
            return 1;
        }

        public static int DeleteWebDavFile(string _WebFileUrl, string _UserName, string _Password)
        {
            try
            {
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)WebRequest.Create(_WebFileUrl);
                req.Credentials = new NetworkCredential(_UserName, _Password);
                req.PreAuthenticate = true;
                req.Method = "DELETE";
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
