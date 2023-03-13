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
        virtualClinicEntities5 db = new virtualClinicEntities5();
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
                var visits = db.visits.Where(v => v.status == 1 && v.jrdoc_id == id);

                return Request.CreateResponse(HttpStatusCode.OK, visits);

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
            juniorDoctor jr = new juniorDoctor();
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
        public HttpResponseMessage Acceptcase(acceptCase acp)
        {
            try
            {
                db.acceptCases.Add(acp);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "data added in accept table");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage History(history his)
        {
            try
            {
                history h = new history();
                var d = DateTime.Now;
                h.patient_id = his.patient_id;
                h.jrdoc_id = his.jrdoc_id;
                h.prescription = his.prescription;
                h.date = d.ToShortDateString();
                h.time = d.ToShortTimeString();
                db.histories.Add(h);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "data added in accept table");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

    }
}
