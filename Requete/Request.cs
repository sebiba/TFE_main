using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Requete
{
    public class Request
    {
        public static string Post(string Url, string ContentType, string input)
        {
            WebRequest post = WebRequest.Create(Url);  // set url to post request
            post.Method = "POST";
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

        public static string Post(string path, string token)
        {
            var webClient = new WebClient();
            string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
            webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
            webClient.Headers.Add("Authorization", "Bearer " + token);
            var fileData = webClient.Encoding.GetString(File.ReadAllBytes(path));
            var package = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, Path.GetFileName(new Uri(path).LocalPath), "pdf", fileData);

            var nfile = webClient.Encoding.GetBytes(package);

            byte[] resp = webClient.UploadData("http://localhost:51727/api/ApiApp", "POST", nfile);
            return string.Empty;
        }

        public static string GetToken(string username, string password)
        {
            string test = Post("http://localhost:51727/Token", "text/plain", "grant_type=password&username="+username+"&password="+password);
            Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(test);
            return data["access_token"];
        }
    }
}
