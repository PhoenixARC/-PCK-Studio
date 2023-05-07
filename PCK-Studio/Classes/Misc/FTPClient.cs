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
        private Uri _host;
        private ICredentials _credentials;

        private FtpWebRequest _request = null;
        private FtpWebResponse _response = null;
        private int _timeout = 1_000; // 1 sec

        public FTPClient(string host, string username)
            : this(new Uri(host), username, string.Empty) { }

        public FTPClient(Uri host, string username)
            : this(host, username, string.Empty) { }

        public FTPClient(string host, string username, string password)
            : this(new Uri(host), username, password) { }

        public FTPClient(Uri uri, string username, string password)
            : this(uri, new NetworkCredential(username, password)) { }

        public FTPClient(string host, ICredentials credentials)
            : this(new Uri(host), credentials) { }
        
        public FTPClient(Uri host, ICredentials credentials)
        {
            if (host.Scheme != Uri.UriSchemeFtp)
            {
                throw new InvalidOperationException("Not a valid FTP Scheme");
            }
            this._host = host;
            _credentials = credentials;
        }

        /// <summary>
        /// Creates a new FTP Request
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="credentials"></param>
        /// <param name="method">See <see cref="WebRequestMethods.Ftp"/></param>
        /// <returns><see cref="FtpWebRequest"/></returns>
        public static FtpWebRequest CreateRequest(Uri uri, ICredentials credentials, string method)
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
                DownloadFile(remoteFilepath, fs);
            }
        }

        public void DownloadFile(string remoteFilepath, Stream destination)
        {
            try
            {
                _request = CreateRequest(new Uri(_host, remoteFilepath), _credentials, WebRequestMethods.Ftp.DownloadFile);
                SetRequestTimeout();

                _response = (FtpWebResponse)_request.GetResponse();
                Stream responseStream = _response.GetResponseStream();

                long destinationOrigin = destination.Position;
                responseStream.CopyTo(destination);
                destination.Position = destinationOrigin;

                responseStream.Close();
                _response.Close();
                _request = null;
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
                _request = CreateRequest(new Uri(_host, directory), _credentials, WebRequestMethods.Ftp.ListDirectory);

                SetRequestTimeout();

                _response = (FtpWebResponse)_request.GetResponse();

                Stream responseStream = _response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);

                IList<string> text = new List<string>();

                while (streamReader.Peek() != -1)
                {
                    text.Add(streamReader.ReadLine());
                }
                streamReader.Close();
                responseStream.Close();

                _response.Close();
                _request = null;
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
                _request = CreateRequest(new Uri(_host, remoteFile), _credentials, WebRequestMethods.Ftp.UploadFile);

                SetRequestTimeout();

                Stream requestStream = _request.GetRequestStream();
                source.CopyTo(requestStream);
                requestStream.Close();

                _response = (FtpWebResponse)_request.GetResponse();
                _response.Close();

                _request = null;
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
                _request = CreateRequest(new Uri(_host, filename), _credentials, WebRequestMethods.Ftp.DeleteFile);

                SetRequestTimeout();

                _response = (FtpWebResponse)_request.GetResponse();
                _response.Close();
                
                _request = null;
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
                _request = CreateRequest(new Uri(_host, name), _credentials, WebRequestMethods.Ftp.Rename);

                SetRequestTimeout();

                _request.RenameTo = newName;
                _response = (FtpWebResponse)_request.GetResponse();
                _response.Close();
                _request = null;
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
                _request = CreateRequest(new Uri(_host, serverFilepath), _credentials, WebRequestMethods.Ftp.AppendFile);
                
                SetRequestTimeout();

                _request.ContentLength = data.Length;

                Stream requestStream = _request.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                FtpWebResponse response = (FtpWebResponse)_request.GetResponse();

                Console.WriteLine("Append status: {0}", response.StatusDescription);

                response.Close();
                _request = null;
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
                _request = CreateRequest(new Uri(_host, name), _credentials, WebRequestMethods.Ftp.MakeDirectory);

                SetRequestTimeout();

                _response = (FtpWebResponse)_request.GetResponse();
                _response.Close();
                
                _request = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public long GetFileSize(string filepath)
        {
            _request = CreateRequest(new Uri(_host, filepath), _credentials, WebRequestMethods.Ftp.GetFileSize);
            
            SetRequestTimeout();
            
            _response = (FtpWebResponse)_request.GetResponse();
            long contentLength = _response.ContentLength;
            _response.Close();

            _request = null;
            return contentLength;
        }

        public void SetTimeoutLimit(TimeSpan delay)
        {
            _timeout = (int)delay.TotalMilliseconds;
        }

        private void SetRequestTimeout()
        {
            if (_request != null)
                _request.Timeout = _timeout;
        }

        public void Dispose()
        {
            _response?.Dispose();
            _request = null;
            _response = null;
        }
    }
}