using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Requete
{
    public class Request
    {
        public static string PostFile(string token, string path)
        {
            var webClient = new WebClient();
            string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
            webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
            webClient.Headers.Add("Authorization", "Bearer " + token);
            var fileData = webClient.Encoding.GetString(File.ReadAllBytes(path));
            var package = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, Path.GetFileName(new Uri(path).LocalPath), "pdf", fileData);

            var nfile = webClient.Encoding.GetBytes(package);

            byte[] resp = webClient.UploadData("http://tfe.moovego.be/api/ApiApp", "POST", nfile);
            return string.Empty;
        }

        public static string Post(string token, string Url, string ContentType=null, string input=null)
        {
            WebRequest post = WebRequest.Create(Url);  // set url to post request
            post.Method = "POST";
            if(token!=null) post.Headers.Add("Authorization", "Bearer " + token);
            post.ContentType = ContentType;

            // send data
            StreamWriter dataStream = new StreamWriter(post.GetRequestStream());
            dataStream.Write(input);
            dataStream.Close();

            // Get the response.  
            WebResponse response = post.GetResponse();
            if (response == null) return null;
            StreamReader responseFromServer = new StreamReader(response.GetResponseStream());
            return responseFromServer.ReadToEnd().Trim();
        }

        public static async Task<bool> DownloadFile(string token, string file, string destination)
        {
            var uri = new Uri("http://tfe.moovego.be/api/ApiApp/GetFile?fileName=" + file);
            var request = WebRequest.CreateHttp(uri);
            request.Headers.Add("Authorization", "Bearer " + token);
            var response = await request.GetResponseAsync();

            ContentDispositionHeaderValue contentDisposition;
            var fileName = ContentDispositionHeaderValue.TryParse(response.Headers["Content-Disposition"], out contentDisposition)
                ? contentDisposition.FileName
                : "noname.dat";
            using (var fs = new FileStream(destination + @"\" + fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await response.GetResponseStream().CopyToAsync(fs);
            }

            return true;
        }

        public static string GetToken(string username, string password)
        {
            string test = Post(null, "http://tfe.moovego.be/Token", "text/plain", "grant_type=password&username="+username+"&password="+password);
            Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(test);
            return data["access_token"];
        }

        public static string Get(string token, string url,Dictionary<string, string> param)
        {
            string requete = url +"?";
            for(int i=0;i<param.Values.Count;i++)
            {
                requete += param.Keys.ToList().ElementAt(i)+"="+ param.Values.ToList().ElementAt(i)+"&";
            }
            requete = requete.TrimEnd('&');
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requete);
            request.Method = "GET";
            if (token != null) request.Headers.Add("Authorization", "Bearer " + token);

            var webResponse = request.GetResponse();
            var webStream = webResponse.GetResponseStream();
            var responseReader = new StreamReader(webStream);
            var response = responseReader.ReadToEnd();
            responseReader.Close();
            return response;
        }
    }
}
