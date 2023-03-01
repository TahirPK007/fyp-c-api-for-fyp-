using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using fyp.Models;

namespace fyp.Controllers
{
    public class PatientController : ApiController
    {
        virtualClinicEntities2 db = new virtualClinicEntities2();
        [HttpPost]
        public HttpResponseMessage Addpat(patient pat)
        {
            try
            {
                var checkcnic = db.patients.Where(dr => dr.cnic == pat.cnic).FirstOrDefault();
                if (checkcnic == null)
                {
                    patient p = new patient();
                    var d = DateTime.Now;
                    p.cnic = pat.cnic;
                    p.full_name = pat.full_name;
                    p.relation = pat.relation;
                    p.relative_name = pat.relative_name;
                    p.dob = pat.dob;
                    p.gender = pat.gender;
                    p.date = d.ToShortDateString();
                    p.time = d.ToShortTimeString();
                    db.patients.Add(p);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "new patient added");
                }
                else {
                    return Request.CreateResponse(HttpStatusCode.Found, "Patient Already Exist");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
