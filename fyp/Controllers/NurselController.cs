using System;
using System.Collections.Generic;
using System.IO;
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
        virtualClinicEntities26 db = new virtualClinicEntities26();

        [HttpPost]
        public HttpResponseMessage Nurselogin(string email, string password)
        {
            nurse nr = new nurse();
            var user2 = db.nurses.Where(u => u.email == email && u.password == password).FirstOrDefault();
            if (user2 != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, user2);
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
                    string filename = image.FileName + "." + extension;
                    image.SaveAs(HttpContext.Current.Server.MapPath("~/Content/Uploads/" + filename));
                    vital vit = new vital();
                    vit.patient_id = int.Parse(request["patient_id"]);
                    vit.systolic = (request["systolic"]);
                    vit.diastolic = (request["diastolic"]);
                    vit.sugar = (request["sugar"]);
                    vit.temperature = (request["temperature"]);
                    vit.symptoms = (request["symptoms"].ToString());
                    vit.image = "http://192.168.0.106/fyp/Content/Uploads/" + filename;
                    vit.status = 0;
                    vit.rated = 0;
                    db.vitals.Add(vit);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "current patient's vital added");
                }
                else
                {
                    vital vit = new vital();
                    vit.patient_id = int.Parse(request["patient_id"]);
                    vit.systolic = (request["systolic"]);
                    vit.diastolic = (request["diastolic"]);
                    vit.sugar = (request["sugar"]);
                    vit.temperature = (request["temperature"]);
                    vit.symptoms = (request["symptoms"].ToString());
                    vit.image = null;
                    vit.status = 0;
                    vit.rated = 0;
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

    }
}
