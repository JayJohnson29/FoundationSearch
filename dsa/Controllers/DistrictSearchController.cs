using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dsa.Controllers
{
    public class DistrictSearchController : Controller
    {

        private List<SelectListItem> StatusList
        {
            get
            {
                return new List<SelectListItem>
                    {
                        new SelectListItem { Text = "N/A", Value = "0", Selected = true },
                        new SelectListItem { Text = "Member", Value = "1" },
                        new SelectListItem { Text = "Accredited", Value = "2" },
                        new SelectListItem { Text = "Member and Accredited", Value = "3" },
                        new SelectListItem { Text = "Member or Accredited", Value = "4" }
                    };
            }
        }

        // GET: DistrictSearch
        public ActionResult Index()
        {
            var model = new Models.CouncilFoundationModel
            {
                State = string.Empty,
                District = string.Empty,
                CouncilStatus = string.Empty,
            };


            var svc = new Services.FoundationService();
            // var foundations = svc.GetFoundations(model);
            var foundations = new List<Models.FoundationModel>();
            var states = svc.GetAllStates();
            var allStates = states.Select(s => new SelectListItem { Text = s.Value, Value = s.Key }).ToList();
            return View(new Models.CouncilFoundationModel
            {
                AllStates = allStates,
                AllStatus = StatusList,
                Foundations = foundations,
            });
        }


        [HttpPost]
        public ActionResult Foundations(Models.CouncilFoundationModel model)
        {
            var svc = new Services.FoundationService();
            var foundations = svc.GetFoundations(model);
            var states = svc.GetAllStates();
            var allStates = states.Select(s => new SelectListItem { Text = s.Value, Value = s.Key }).ToList();
            return View("index", new Models.CouncilFoundationModel
            {
                AllStates = allStates,
                AllStatus = StatusList,
                Foundations = foundations,
            });
        }

    }
}
