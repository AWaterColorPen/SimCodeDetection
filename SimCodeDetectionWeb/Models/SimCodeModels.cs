using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SimCodeDetectionWeb.Models
{
    public class Problem
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string input { get; set; }
        public string output { get; set; }
        public string simpleInput { get; set; }
        public string simpleOutput { get; set; }
        public DateTime endTime { get; set; }

        public int users { get; set; }
        public int judgeType { get; set; }	

        public virtual OJUser OUser { get; set; }
        public virtual ICollection<Submission> submissions { get; set; }
       
        public void UpdateUsers()
        {
            var userlist = submissions.Select(m => new { User = m.OUser }).Distinct();
            users = userlist.Count();
        }
        
    }

    public class Submission
    {
        public int Id { get; set; }
        public string source { get; set; }
        public string status { get; set; }
        public DateTime subTime { get; set; }
        public string language { get; set; }

        public string snippets { get; set; }
        public string results { get; set; }

        public virtual OJUser OUser { get; set; }
        public virtual Problem problem { get; set; }
        
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

    public class OJUser
    {
        public int Id { get; set; }
        public string userName { get; set; }
        public string userLevel { get; set; }
        public string studentId { get; set; }

        public virtual ICollection<Submission> submissions { get; set; }
    }

    public class SimCodeDBContext : DbContext
    {
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<OJUser> OJUsers { get; set; }
    }
}