using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using fyp.Models;

namespace fyp.Controllers
{
    public class JrdocController : ApiController
    {
        virtualClinicEntities27 db = new virtualClinicEntities27();

        public object SqlMethods { get; private set; }

        [HttpPost]
        public HttpResponseMessage Jrsignup(juniorDoctor jr)
        {
            try
            {
                var email = db.juniorDoctors.Where(j => j.email == jr.email).FirstOrDefault();
                if (email == null)
                {
                    jr.rating = 0;
                    db.juniorDoctors.Add(jr);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "true");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "email alread exist");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage MyNewCases(int id)
        {
            try
            {
                var visits1 = db.visits.Where(v => v.status == 1 && v.jrdoc_id == id).FirstOrDefault();
                if (visits1 != null)
                {
                    var jrdocid = visits1.jrdoc_id;
                    var patid = visits1.patient_id;
                    var record = (from x in db.visits
                                  join p in db.patients on x.patient_id equals p.patient_id
                                  join v in db.vitals on p.patient_id equals v.patient_id
                                  where x.status == 1 && x.jrdoc_id == id
                                  where p.patient_id == patid
                                  where v.patient_id == patid && v.status == 0
                                  select new { p, v, x }).ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, record);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "no new cases");
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage Jrlogin(string email, string password)
        {
            try
            {
                var user1 = db.juniorDoctors.Where(u => u.email == email && u.password == password).FirstOrDefault();
                if (user1 != null)
                {
                    user1.status = 1;
                    db.juniorDoctors.AddOrUpdate(user1);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, user1);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "user doesnt exist");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage Jrlogout(int jrdocid)
        {
            var logout = db.juniorDoctors.Where(u => u.jrdoc_id == jrdocid).FirstOrDefault();
            if (logout != null)
            {
                logout.status = 0;
                db.juniorDoctors.AddOrUpdate(logout);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "logged_out");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "error occured while logging out");
            }
        }
        //when the doctors accept the case
        [HttpPost]
        public HttpResponseMessage AcceptedCase(int jrdocid, int patid, int visitid)
        {
            try
            {
                acceptcase acp = new acceptcase();
                var visittoupdate = db.visits.Where(v => v.status == 1 && v.jrdoc_id == jrdocid).FirstOrDefault();
                TimeSpan acceptedtime = DateTime.Now.Subtract(visittoupdate.AssignedDatetime.Value);
                acp.patient_id = patid;
                acp.jrdoc_id = jrdocid;
                acp.visit_id = visitid;
                acp.time = $"{acceptedtime.Hours} hour {acceptedtime.Minutes} minute {acceptedtime.Seconds} second";
                visittoupdate.status = 2;
                db.acceptcases.Add(acp);
                db.visits.AddOrUpdate(visittoupdate);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "data added in accept table");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        //adding data into the appointment table
        [HttpPost]
        public HttpResponseMessage Appointment(int patid, int jrdocid, int visitid, int nurseid)
        {
            try
            {
                appointment apt = new appointment();
                var d = DateTime.Now;
                apt.patient_id = patid;
                apt.jrdoc_id = jrdocid;
                apt.date = d.ToShortDateString();
                apt.time = d.ToShortTimeString();
                apt.visit_id = visitid;
                apt.status = 0;
                apt.nurseID = nurseid;
                apt.shown = 0;
                db.appointments.Add(apt);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "new appointment added");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }


        }
        //in this function getting the last patient's appointment id
        [HttpGet]
        public HttpResponseMessage Gettingappointmentid(int patid)
        {
            try
            {
                var data = db.appointments.Where(a => a.patient_id == patid && a.status == 0).FirstOrDefault();
                int aptid = data.appointment_id;
                return Request.CreateResponse(HttpStatusCode.OK, aptid);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }


        }
        [HttpPost]
        public HttpResponseMessage Addingprescription([FromBody] List<prescription> prescriptions)
        {
            try
            {
                foreach (var data in prescriptions)
                {
                    db.prescriptions.Add(data);
                }
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "prescription added for current patient");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        //updating vitals status to prevent from re-fetching the same patient's vitals
        [HttpPost]
        public HttpResponseMessage Updatingvitalstatus(int vitalid)
        {
            try
            {
                var data = db.vitals.Where(v => v.vital_id == vitalid).FirstOrDefault();
                data.status = 1;
                db.vitals.AddOrUpdate(data);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Successfully Updated");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage CommentsTest(int aptid, string comments)
        {
            try
            {
                commentsTest cts = new commentsTest();
                cts.appointment_id = aptid;
                cts.comments = comments;
                db.commentsTests.Add(cts);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Successfully added");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage AddFollowUp(int patid, int jrdocid)
        {
            try
            {
                var data = db.patients.Where(p => p.patient_id == patid).FirstOrDefault();
                data.jrdoc_id = jrdocid;
                db.patients.AddOrUpdate(data);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "FollowedUp Success");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage RemoveFollowUp(int patid)
        {
            try
            {
                var data = db.patients.Where(p => p.patient_id == patid).FirstOrDefault();
                data.jrdoc_id = null;
                db.patients.AddOrUpdate(data);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Successfully removed followed up patient");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }




    }
}
