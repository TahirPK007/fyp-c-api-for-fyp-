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
        virtualClinicEntities16 db = new virtualClinicEntities16();
        [HttpPost]
        public HttpResponseMessage Jrsignup(juniorDoctor jr)
        {
            try
            {
                var email = db.juniorDoctors.Where(j => j.email == jr.email).FirstOrDefault();
                if (email == null)
                {
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
                var d = DateTime.Now;
                var visits1 = db.visits.Where(v => v.status == 1 && v.jrdoc_id == id).FirstOrDefault();
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
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
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
                acp.time = DateTime.Now.Add(acceptedtime);
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
        [HttpPost]
        public HttpResponseMessage PrescriptionHistory(int patid, int jrdocid, history hs)
        {
            try
            {
                history h = new history();
                var d = DateTime.Now;
                h.patient_id = patid;
                h.jrdoc_id = jrdocid;
                h.prescription = hs.prescription;
                h.date = d.ToShortDateString();
                h.time = d.ToShortTimeString();
                db.histories.Add(h);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Data Added In History Table Successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }


    }
}
