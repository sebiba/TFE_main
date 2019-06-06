using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

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

        [HttpGet]
        [Route("api/ApiApp/GetFile")]
        public HttpResponseMessage GetFile(string fileName)
        {
            //Create HTTP Response.
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            //Set the File Path.
            string filePath = HttpContext.Current.Server.MapPath("~/Data/"+ User.Identity.GetUserId()+"/") + fileName;

            //Check whether File exists.
            if (!File.Exists(filePath))
            {
                //Throw 404 (Not Found) exception if File not found.
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = string.Format("File not found: {0} .", fileName);
                throw new HttpResponseException(response);
            }

            //Read the File into a Byte Array.
            byte[] bytes = File.ReadAllBytes(filePath);

            //Set the Response Content.
            response.Content = new ByteArrayContent(bytes);

            //Set the Response Content Length.
            response.Content.Headers.ContentLength = bytes.LongLength;

            //Set the Content Disposition Header Value and FileName.
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileName;

            //Set the File Content Type.
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));
            return response;
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
            string root = HttpContext.Current.Server.MapPath("~/Data/"+ IdUser +"/");  // path on server
            MultipartFormDataStreamProvider provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (MultipartFileData fileData in provider.FileData)
                {
                    if (string.IsNullOrEmpty(fileData.Headers.ContentDisposition.FileName))
                    {
                        return Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted");
                    }
                    string fileName = fileData.Headers.ContentDisposition.FileName;
                    if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                    {
                        fileName = fileName.Trim('"');
                    }
                    if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                    {
                        fileName = Path.GetFileName(fileName);
                    }
                    if (File.Exists(Path.Combine(root, fileName))) File.Delete(Path.Combine(root, fileName));
                    File.Move(fileData.LocalFileName, Path.Combine(root, fileName));  // move transmitted file to good path
                }

                if(provider.FileData.Count == 0)  return Request.CreateResponse(HttpStatusCode.BadRequest);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }


        [HttpPost]
        [Route("api/ApiApp/GetFiles")]
        public List<string> GetFiles()
        {
            var id = User.Identity.GetUserId();
            return helper.CustomHelper.GetFiles(User.Identity.GetUserId()).Select(delegate(string x) { return string.Join("\\",x.Split(new string[] { "\\" }, StringSplitOptions.None).Skip(1)); }).ToList();
        }

        [HttpGet]
        [Route("api/ApiApp/Share")]
        public string Share(string toShare, string dest)
        {
            
            string id = User.Identity.GetUserId();
            return new AccountController().Share(id,toShare, dest);
        }

        [HttpPost]
        [Route("api/ApiApp/Register")]
        public bool RegisterUser()
        {
            return false;
        }
    }
}
