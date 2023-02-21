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
        virtualClinicEntities2 db=new virtualClinicEntities2();
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
        [HttpPost]
        public HttpResponseMessage Jrlogin(string email, string password)
        {

            var user = db.juniorDoctors.Where(u => u.email == email && u.password == password);
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
