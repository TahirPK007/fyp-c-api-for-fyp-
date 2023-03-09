using System;
using System.Collections.Generic;
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
                if(email==null)
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
            juniorDoctor jr = new juniorDoctor();
            var user = db.juniorDoctors.Where(u => u.email == email && u.password == password).FirstOrDefault();
            user.status= 1;
            if (user!=null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "jrdoc");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "user doesnt exist");
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
