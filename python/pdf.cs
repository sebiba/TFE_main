using Newtonsoft.Json;
using Requete;
using System;
using System.Collections.Generic;

namespace python
{
    public class Pdf
    {
        public string _path;

        public Pdf(string pathParam){
            _path = pathParam;
        }

        /// <summary>
        /// get list of all pdf of the account
        /// </summary>
        /// <param name="pseudo">pseudo to connect on the webapp</param>
        /// <param name="password">password to connect on the webapp</param>
        /// <returns></returns>
        public List<string> GetPdf(string pseudo, string password)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<string>>(Request.Post(Request.GetToken(pseudo, password), "http://tfe.moovego.be/api/ApiApp/GetFiles"));
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// upload the pdf to the server
        /// </summary>
        /// <param name="pseudo">pseudo to connect on the webapp</param>
        /// <param name="password">password to connect on the webapp</param>
        public void UploadPdf(string pseudo, string password)
        {
            if(_path != "about:blank") {
                try { 
                Request.PostFile(Request.GetToken(pseudo, password), _path);
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        /// <summary>
        /// download on the local computer a pdf from the server
        /// </summary>
        /// <param name="pseudo">pseudo to connect on the webapp</param>
        /// <param name="password">password to connect on the webapp</param>
        /// <param name="selected">safeName of the file to download</param>
        /// <param name="destFolder">where to save in local the pdf to download</param>
        public void DownloadPdf(string pseudo, string password, string selected, string destFolder)
        {
            try { 
            _ = Request.DownloadFile(Request.GetToken(pseudo, password), selected, destFolder);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// share a pdf whith an other account on the server
        /// </summary>
        /// <param name="pseudo">pseudo to connect on the webapp</param>
        /// <param name="password">password to connect on the webapp</param>
        /// <param name="selected">safeName of the pdf to share</param>
        /// <param name="ShareTo">email of the account to share with</param>
        /// <returns></returns>
        public bool SharePdf(string pseudo, string password, string selected, string ShareTo)
        {
            try {
                if ("\"True\"" == Request.Get(Request.GetToken(pseudo, password), "http://tfe.moovego.be/api/ApiApp/Share", new Dictionary<string, string> { { "toShare", selected }, { "dest", ShareTo } }))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                throw;
            }
        }
    }
}
