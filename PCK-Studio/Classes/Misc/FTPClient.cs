using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace PckStudio.Classes.Misc
{
    public class FTPClient : IDisposable
    {
        private Uri hostUri;
        private ICredentials clientCredentials;

        private FtpWebRequest request = null;
        private FtpWebResponse response = null;
        private int _timeout = 1_000; // 1 sec

        public FTPClient(string host, string username)
            : this(new Uri(host), username, string.Empty) { }

        public FTPClient(Uri uri, string username)
            : this(uri, username, string.Empty) { }

        public FTPClient(string host, string username, string password)
            : this(new Uri(host), username, password) { }

        public FTPClient(Uri uri, string username, string password)
            : this(uri, new NetworkCredential(username, password)) { }

        public FTPClient(string host, ICredentials credentials)
            : this(new Uri(host), credentials) { }
        
        public FTPClient(Uri uri, ICredentials credentials)
            {
            if (uri.Scheme != Uri.UriSchemeFtp)
            {
                throw new InvalidOperationException("Not a valid FTP Scheme");
            }
            hostUri = uri;
            clientCredentials = credentials;
        }

        /// <summary>
        /// Creates a new FTP Request
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="credentials"></param>
        /// <param name="method">See <see cref="WebRequestMethods.Ftp"/></param>
        /// <returns><see cref="FtpWebRequest"/></returns>
        public static FtpWebRequest CreateFTPWebRequest(Uri uri, ICredentials credentials, string method)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Credentials = credentials;
            request.Method = method;
            return request;
        }

        public void DownloadFile(string remoteFilepath, string localFilepath)
        {
            using (var fs = File.OpenWrite(localFilepath))
            {
                DownloadFile(fs, remoteFilepath);
            }
        }

        public void DownloadFile(Stream destination, string remoteFilepath)
        {
            try
            {
                request = CreateFTPWebRequest(new Uri(hostUri, remoteFilepath), clientCredentials, WebRequestMethods.Ftp.DownloadFile);
                
                SetRequestTimeout();

                response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();

                long destinationOrigin = destination.Position;
                responseStream.CopyTo(destination);
                destination.Position = destinationOrigin;

                responseStream.Close();
                response.Close();
                request = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public string[] ListDirectory(string directory)
        {
            try
            {
                request = CreateFTPWebRequest(new Uri(hostUri, directory), clientCredentials, WebRequestMethods.Ftp.ListDirectory);

                SetRequestTimeout();

                response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                
                IList<string> text = new List<string>();

                    while (streamReader.Peek() != -1)
                    {
                    text.Add(streamReader.ReadLine());
                    }
                streamReader.Close();
                responseStream.Close();

                response.Close();
                request = null;
                return text.ToArray();
                }
                catch (Exception ex)
                {
                Console.WriteLine(ex.ToString());
            }
            return Array.Empty<string>();
        }

        public void UploadFile(string localFile, string remoteFile)
        {
            using (var fs = File.OpenRead(localFile))
            {
                UploadFile(fs, remoteFile);
            }
        }

        public void UploadFile(Stream source, string remoteFile)
        {
            try
            {
                request = CreateFTPWebRequest(new Uri(hostUri, remoteFile), clientCredentials, WebRequestMethods.Ftp.UploadFile);

                SetRequestTimeout();

                Stream requestStream = request.GetRequestStream();
                source.CopyTo(requestStream);
                requestStream.Close();

                response = (FtpWebResponse)request.GetResponse();
                response.Close();

                request = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void DeleteFile(string filename)
        {
            try
            {
                request = CreateFTPWebRequest(new Uri(hostUri, filename), clientCredentials, WebRequestMethods.Ftp.DeleteFile);

                SetRequestTimeout();

                response = (FtpWebResponse)request.GetResponse();
                response.Close();
                
                request = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void Rename(string name, string newName)
        {
            try
            {
                request = CreateFTPWebRequest(new Uri(hostUri, name), clientCredentials, WebRequestMethods.Ftp.Rename);

                SetRequestTimeout();

                request.RenameTo = newName;
                response = (FtpWebResponse)request.GetResponse();
                response.Close();
                request = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void AppendFile(string serverFilepath, byte[] data)
        {
            try
            {
                request = CreateFTPWebRequest(new Uri(hostUri, serverFilepath), clientCredentials, WebRequestMethods.Ftp.AppendFile);
                
                SetRequestTimeout();

                request.ContentLength = data.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Console.WriteLine("Append status: {0}", response.StatusDescription);

                response.Close();
                request = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void CreateDirectory(string name)
        {
            try
            {
                request = CreateFTPWebRequest(new Uri(hostUri, name), clientCredentials, WebRequestMethods.Ftp.MakeDirectory);

                SetRequestTimeout();

                response = (FtpWebResponse)request.GetResponse();
                response.Close();
                
                request = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public long GetFileSize(string filepath)
        {
            request = CreateFTPWebRequest(new Uri(hostUri, filepath), clientCredentials, WebRequestMethods.Ftp.GetFileSize);
            
            SetRequestTimeout();
            
            response = (FtpWebResponse)request.GetResponse();
            long contentLength = response.ContentLength;
            response.Close();

            request = null;
            return contentLength;
        }

        public void SetTimeoutLimit(TimeSpan delay)
        {
            _timeout = (int)delay.TotalMilliseconds;
        }

        private void SetRequestTimeout()
        {
            if (request != null)
                request.Timeout = _timeout;
        }

        public void Dispose()
        {
            response?.Dispose();
            request = null;
            response = null;
        }
    }
}