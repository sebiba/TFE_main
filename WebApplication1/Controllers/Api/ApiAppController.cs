using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebApplication1.Providers;

namespace WebApplication1.Controllers.Api
{
    [Authorize]
    public class ApiAppController : ApiController
    {
        // GET: api/ApiApp
        [AllowAnonymous]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ApiApp/5
        [AllowAnonymous]
        [HttpGet]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ApiApp
        [HttpPost]
        public async System.Threading.Tasks.Task<HttpResponseMessage> PostAsync()
        {
            var IdUser = User.Identity.GetUserId();
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            string root = HttpContext.Current.Server.MapPath("~/Data/"+ IdUser +"/");
            //MultipartFormDataStreamProvider provider = new MultipartFormDataStreamProvider(root);
            var provider = new CustomMultipartFormDataStreamProvider(root);
            await Request.Content.ReadAsMultipartAsync(provider);
            provider.ExtractValues();

            try
            {
                //await Request.Content.ReadAsMultipartAsync(provider);
                // Show all the key-value pairs.
                foreach (var key in provider.FormData.AllKeys)
                {
                    foreach (var val in provider.FormData.GetValues(key))
                    {
                        Trace.WriteLine(string.Format("{0}: {1}", key, val));
                    }
                }
                foreach (var file in provider.FileData)
                {
                    FileInfo fileInfo = new FileInfo(file.LocalFileName);
                    Trace.WriteLine(string.Format("Uploaded file: {0} ({1} bytes)\n", fileInfo.Name, fileInfo.Length));
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
