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
            try
            {
                return MakeList(GetFiles());
            }
            catch {
                return "<p>Votre bibliotheque est probablement vide... Vous devez d'habord enregistrer un pdf depuis votre application avant de pouvoir le voir ici.</p>";
            }
        }

        public static string MakeList(List<string> value)
        {
            string output = "<ul class=\"list-group list-group-flush\">";
            value.ForEach(delegate (string text)
            {
                output += "<li class=\"list-group-item\" onclick=\"ViewFile(this)\" data-value=\"" + text+"\">" + string.Join("\\", text.Split(new string[] { "\\" }, StringSplitOptions.None).Skip(1)) + "</li>";
            });
            return output + "</ul>";
        }

        public static List<string> GetFiles(string id = null){
            if(id==null) id = HttpContext.Current.Session["user"].ToString();
            DirectoryInfo d = new DirectoryInfo(@"E:\TFE\WebApp\Data\");  //root folder for datas
            //DirectoryInfo d = new DirectoryInfo(@"D:\jsp\tablature");  //root folder for datas
            DirectoryInfo[] Ids = d.GetDirectories();
            if (!Ids.Select(x => x.Name).ToList().Contains(id))
            {
                throw new FileNotFoundException();
            }
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