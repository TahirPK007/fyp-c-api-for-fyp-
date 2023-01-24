using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using fyp.Models;


namespace fyp.Controllers
{
    public class NurselController : ApiController
    {
        virtualClinicEntities1 db = new virtualClinicEntities1();

        [HttpGet]
        public HttpResponseMessage Nurselogin(string username, string password)
        {

            var user = db.nurses.Where(u => u.username == username && u.password == password);
            if (user.Count() > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "successfully logged in");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "user doesnt exist");
            }
        }   
        [HttpPost]
        public HttpResponseMessage Addvitals()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;
                var image = request.Files["image"];
        string extension = image.FileName.Split('.')[1];
        String filename = image.FileName;
        image.SaveAs(HttpContext.Current.Server.MapPath("~/Content/Uploads/" + filename));
                vital vit = new vital();
                vit.nurseID = int.Parse(request["nurseID"]);
                vit.patient_id = int.Parse(request["patient_id"]);
                vit.bp = (request["bp"]);
                vit.sugar = int.Parse(request["sugar"]);
                vit.temper = float.Parse(request["temper"]);
                vit.symptoms= (request["symptoms"]);
                vit.image = filename;
                db.vitals.Add(vit);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "current patient's vital added");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpGet]
        public HttpResponseMessage Fetchpatvit()
        {
            try
            {
               var fetchedpatient= (from p in db.patients join v in db.vitals on p.patient_id equals v.patient_id select new { p.fullname, p.dob, p.gender, v }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, fetchedpatient);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
