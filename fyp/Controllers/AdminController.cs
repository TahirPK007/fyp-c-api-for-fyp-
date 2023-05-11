using fyp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace fyp.Controllers
{
    public class AdminController : ApiController
    {
        virtualClinicEntities26 db = new virtualClinicEntities26();
        [HttpPost]
        public HttpResponseMessage Addnewnurse(nurse nur)
        {
            try
            {
                nurse x= new nurse();
                x.full_name = nur.full_name;
                x.email=nur.email;
                x.password = nur.password;
                x.role = "nurse";
                db.nurses.Add(x);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Nurse Added Successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage Addnewsrdoc(seniorDoctor sr)
        {
            try
            {
                seniorDoctor x = new seniorDoctor();
                x.full_name = sr.full_name;
                x.email = sr.email;
                x.password = sr.password;
                x.role = "srdoc";
                x.status = 0;
                db.seniorDoctors.Add(x);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "SeniorDoctor Added Successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
