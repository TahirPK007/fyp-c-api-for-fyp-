using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
        virtualClinicEntities27 db = new virtualClinicEntities27();

        //nurse login method
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
        //adding the vitals for the current patient
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

        [HttpGet]
        public HttpResponseMessage Gettingappointments(int nurseid)
        {
            try
            {
                var data = db.appointments.Where(a => a.shown == 0 && a.nurseID==nurseid);
                var appts = (from a in db.appointments
                             join pat in db.patients on a.patient_id equals pat.patient_id
                             where a.shown == 0 && a.nurseID==nurseid
                             where pat.patient_id==a.patient_id
                             select new {a,pat}).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, appts);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpGet]
        public HttpResponseMessage GettingDoneaptdetails(int aptid)
        {
            try
            {
                var data = db.appointments.Where(a=>a.appointment_id==aptid).FirstOrDefault();
                int apointmentid = data.appointment_id;
                var details = (from x in db.appointments
                               join p in db.prescriptions on x.appointment_id equals p.appointment_id
                               where x.appointment_id == apointmentid
                               where p.appointment_id == x.appointment_id
                               select new { x, p }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, details);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage FinishDoneAppointment(int aptid)
        {
            try
            {
                var data = db.appointments.Where(a => a.appointment_id == aptid).FirstOrDefault();
                data.shown = 1;
                db.appointments.AddOrUpdate(data);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "success");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

    }
}
