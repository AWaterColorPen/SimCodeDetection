using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CoreWeb.Models
{
    public class DataDb
    {
        public int Id { get; set; }
        public string source { get; set; }
        public int vid { get; set; }
        public int user { get; set; }
        public int problem { get; set; }

        public string snippets { get; set; }

        public List<string> Snippets
        {
            get
            {
                return Tools.JsonTool.Json2ListString(snippets);
            }
            set
            {
                snippets = Tools.JsonTool.ListString2Json(value);
            }
        }
    }

    public class DataDBContext : DbContext
    {
        public DbSet<DataDb> Datas { get; set; }
    }
}