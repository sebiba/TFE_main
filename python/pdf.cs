using Newtonsoft.Json;
using Requete;
using System.Collections.Generic;

namespace python
{
    public class Pdf
    {
        private string _path;

        public Pdf(string pathParam){
            _path = pathParam;
        }
        public List<string> GetPdf(string pseudo, string password)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<string>>(Request.Post(Request.GetToken(pseudo, password), "http://tfe.moovego.be/api/ApiApp/GetFiles"));
            }
            catch
            {
                return new List<string>();
            }
        }

        public void UploadPdf(string pseudo, string password)
        {
            Request.PostFile(Request.GetToken(pseudo, password), _path);
        }

        public void DownloadPdf(string pseudo, string password, string selected, string destFolder)
        {
            _ = Request.DownloadFile(Request.GetToken(pseudo, password), selected, destFolder);
        }

        public bool SharePdf(string pseudo, string password, string selected, string ShareTo)
        {
            if ("\"True\"" == Request.Get(Request.GetToken(pseudo, password), "http://tfe.moovego.be/api/ApiApp/Share", new Dictionary<string, string> { { "toShare", selected }, { "dest", ShareTo } }))
            {
                return true;
            }
            return false;
        }
    }
}
