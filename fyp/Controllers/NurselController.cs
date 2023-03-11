using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using fyp.Models;


namespace fyp.Controllers
{
    public class NurselController : ApiController
    {
        virtualClinicEntities5 db = new virtualClinicEntities5();

        [HttpPost]
        public HttpResponseMessage Nurselogin(string email, string password)
        {
            nurse nr = new nurse();
            var user2 = db.nurses.Where(u => u.email == email && u.password == password).FirstOrDefault();
            if (user2!=null)
            {
                return Request.CreateResponse(HttpStatusCode.OK,user2);
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
                if (image != null)
                {
                    string extension = image.FileName.Split('.')[1];
                string filename = image.FileName;
                image.SaveAs(HttpContext.Current.Server.MapPath("~/Content/Uploads/" + filename));
                vital vit = new vital();
                
                    vit.patient_id = int.Parse(request["patient_id"]);
                vit.blood_pressure = (request["blood_pressure"]);
                vit.sugar = (request["sugar"]);
                vit.temperature = (request["temperature"]);
                vit.symptoms = (request["symptoms"]);
                vit.image = filename;
                db.vitals.Add(vit);
                db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "current patient's vital added");
                }
            else
                {
                    vital vit = new vital();
                    vit.patient_id = int.Parse(request["patient_id"]);
                    vit.blood_pressure = (request["blood_pressure"]);
                    vit.sugar = (request["sugar"]);
                    vit.temperature = (request["temperature"]);
                    vit.symptoms = (request["symptoms"]);
                    vit.image = "";
                    db.vitals.Add(vit);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "current patient's vital added");
                }
                
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
               var fetchedpatient= (from p in db.patients join v in db.vitals on p.patient_id equals v.patient_id select new { p.full_name, p.dob, p.gender, v }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, fetchedpatient);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
