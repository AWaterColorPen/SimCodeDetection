using SimCodeDetectionWeb.SimCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimCodeDetectionWeb.Controllers
{
    public class BetaController : Controller
    {
        // GET: Beta
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string source1, string source2)
        {
            
            ViewBag.simsnippets = BetaWorker(source1, source2);
            return View();
        }

        private IEnumerable<SimSnippet> BetaWorker(string source1, string source2)
        {
            var snippets1 = CodeParse.Slicer.Slicing(source1);
            var snippets2 = CodeParse.Slicer.Slicing(source2);

            List<SimSnippet> simsnippets = new List<SimSnippet>();
            for (var i = 0; i < snippets1.Count; i++)
            {
                for (var j = 0; j < snippets2.Count; j++)
                {
                    SimSnippet simsnippet = new SimSnippet(snippets1[i], snippets2[j]);
                    System.Diagnostics.Debug.WriteLine("({0} , {1}) = {2}", i, j, simsnippet.similar);
                    if (simsnippet.similar > 0.1)
                        simsnippets.Add(simsnippet);
                }
            }
            return simsnippets;
        }
    }
}