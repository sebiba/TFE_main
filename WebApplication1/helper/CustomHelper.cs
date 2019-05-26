using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplication1.helper
{
    public class CustomHelper
    {
        public static string Bibli()
        {
            return MakeList(GetFiles());
        }

        public static string MakeList(List<string> value)
        {
            string output = "<ul>";
            value.ForEach(delegate (string text)
            {
                output += "<li onclick=\"ViewFile(this)\" data-value=\""+text+"\">" + string.Join("\\", text.Split(new string[] { "\\" }, StringSplitOptions.None).Skip(1)) + "</li>";
            });
            return output + "</ul>";
        }

        public static List<string> GetFiles(string id = null){
            if(id==null) id = HttpContext.Current.Session["user"].ToString();
            DirectoryInfo d = new DirectoryInfo(@"D:\programmation\c#\TFE\WebApplication1\Data");  //root folder for datas
            DirectoryInfo[] Ids = d.GetDirectories();
            FileInfo[] Files = Ids.Where(x => x.Name == id.ToString()).First().GetFiles("*.pdf");  // Getting pdf files
            List<string> str = new List<string>();
            foreach (FileInfo file in Files)
            {
                str.Add(id + @"\" + file.Name);
            }
            return str;
        }
    }
}