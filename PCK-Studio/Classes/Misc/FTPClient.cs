using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace PckStudio.Classes.Misc
{
    public class FTPClient : IDisposable
    {
        private const int bufferSize = 2048;

        private Uri hostUri;
        private NetworkCredential credentials;

        private FtpWebRequest request = null;
        private FtpWebResponse response = null;
        private Stream _stream = null;

        public FTPClient(string host, string username)
            : this(new Uri(host), username, string.Empty)
        {
        }

        public FTPClient(Uri uri, string username)
            : this(uri, username, string.Empty)
        {
        }

        public FTPClient(string host, string username, string password)
            : this(new Uri(host), username, password)
        {
        }

        public FTPClient(Uri uri, string username, string password)
        {
            hostUri = uri;
            credentials = new NetworkCredential(username, password);

            if (hostUri.Scheme != Uri.UriSchemeFtp)
            {
                throw new InvalidOperationException("Not a valid FTP Scheme");
            }

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

        // TODO: let it accept a destination Stream ?
        public void DownloadFile(string remoteFilepath, string localFilepath)
        {
            try
            {
                request = CreateFTPWebRequest(new Uri(hostUri, remoteFilepath), credentials, WebRequestMethods.Ftp.DownloadFile);
                //request = (FtpWebRequest)WebRequest.Create(host + "/" + remoteFile);
                //request.Credentials = credentials;
                //request.Method = WebRequestMethods.Ftp.DownloadFile;
                
                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = true;

                response = (FtpWebResponse)request.GetResponse();
                _stream = response.GetResponseStream();
                byte[] buffer = new byte[Convert.ToInt32(GetFileSize(remoteFilepath))];
                int num = _stream.Read(buffer, 0, Convert.ToInt32(GetFileSize(remoteFilepath)));

                using (FileStream fileStream = new FileStream(localFilepath, FileMode.OpenOrCreate))
                {
                    try
                    {
                        while (num > 0)
                        {
                            fileStream.Write(buffer, 0, num);
                            num = _stream.Read(buffer, 0, Convert.ToInt32(GetFileSize(remoteFilepath)));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                _stream.Close();
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
                request = CreateFTPWebRequest(new Uri(hostUri, directory), credentials, WebRequestMethods.Ftp.ListDirectory);
                //request = (FtpWebRequest)WebRequest.Create(host + "/" + directory);
                //request.Credentials = credentials;
                //request.Method = WebRequestMethods.Ftp.ListDirectory;

                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = true;

                response = (FtpWebResponse)request.GetResponse();
                _stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(_stream);
                string text = string.Empty;
                try
                {
                    while (streamReader.Peek() != -1)
                    {
                        text += streamReader.ReadLine() + "|";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                streamReader.Close();
                _stream.Close();
                response.Close();
                request = null;

                try
                {
                    return text.Split("|".ToCharArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Array.Empty<string>();
        }

        public void UploadFile(string localFile, string remoteFile)
        {
            try
            {
                request = CreateFTPWebRequest(new Uri(hostUri, remoteFile), credentials, WebRequestMethods.Ftp.UploadFile);
                //request = (FtpWebRequest)WebRequest.Create(host + "/" + remoteFile);
                //request.Credentials = credentials;
                //request.Method = WebRequestMethods.Ftp.UploadFile;

                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = true;
                
                _stream = request.GetRequestStream();
                FileStream fileStream = new FileStream(localFile, FileMode.Open);
                byte[] buffer = new byte[fileStream.Length];
                int num = fileStream.Read(buffer, 0, (int)fileStream.Length);
                try
                {
                    while (num != 0)
                    {
                        _stream.Write(buffer, 0, num);
                        num = fileStream.Read(buffer, 0, (int)fileStream.Length);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                fileStream.Close();
                _stream.Close();
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
                request = CreateFTPWebRequest(new Uri(hostUri, filename), credentials, WebRequestMethods.Ftp.DeleteFile);
                //request = (FtpWebRequest)WebRequest.Create(host + "/" + filename);
                //request.Credentials = credentials;
                //request.Method = WebRequestMethods.Ftp.DeleteFile;

                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = true;

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
                request = CreateFTPWebRequest(new Uri(hostUri, name), credentials, WebRequestMethods.Ftp.Rename);
                //request = (FtpWebRequest)WebRequest.Create(host + "/" + name);
                //request.Credentials = credentials;
                //request.Method = WebRequestMethods.Ftp.Rename;

                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = true;
                
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
                request = CreateFTPWebRequest(new Uri(hostUri, serverFilepath), credentials, WebRequestMethods.Ftp.AppendFile);
                //request = (FtpWebRequest)WebRequest.Create(host + "/" + name);
                //request.Credentials = credentials;
                //request.Method = WebRequestMethods.Ftp.MakeDirectory;
                
                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = true;

                request.ContentLength = data.Length;

                // This example assumes the FTP site uses anonymous logon.
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
                request = CreateFTPWebRequest(new Uri(hostUri, name), credentials, WebRequestMethods.Ftp.MakeDirectory);
                //request = (FtpWebRequest)WebRequest.Create(host + "/" + name);
                //request.Credentials = credentials;
                //request.Method = WebRequestMethods.Ftp.MakeDirectory;

                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = true;

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
            FtpWebRequest ftpWebRequest = CreateFTPWebRequest(new Uri(hostUri, filepath), credentials, WebRequestMethods.Ftp.GetFileSize);
            //FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(host + "/" + fileName);
            //ftpWebRequest.Credentials = credentials;
            //ftpWebRequest.Method = WebRequestMethods.Ftp.GetFileSize;
            
            ftpWebRequest.UseBinary = true;

            FtpWebResponse response = (FtpWebResponse)ftpWebRequest.GetResponse();
            long contentLength = response.ContentLength;
            response.Close();
            return contentLength;
        }

        public void Dispose()
        {
            _stream.Dispose();
            response.Dispose();
            request = null;
            response = null;
            _stream = null;
        }
    }
}