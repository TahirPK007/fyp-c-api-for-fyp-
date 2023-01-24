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
        virtualClinicEntities1 db = new virtualClinicEntities1();
        [HttpPost]
        public HttpResponseMessage Addpatient(patient pat,int nurseid)
        {
            try
            {
                var checkcnic = db.patients.Where(dr => dr.cnic == pat.cnic).FirstOrDefault();
                if (checkcnic == null)
                {
                    patient p = new patient();
                    var d = DateTime.Now;
                    p.cnic = pat.cnic;
                    p.fullname = pat.fullname;
                    p.relation = pat.relation;
                    p.relativename = pat.relativename;
                    p.dob = pat.dob;
                    p.gender = pat.gender;
                    p.date = d.ToShortDateString();
                    p.time = d.ToShortTimeString();
                    db.patients.Add(p);
                    db.SaveChanges();
                    var exa = db.patients.Where(dr => dr.cnic == pat.cnic).FirstOrDefault();
                    examine e = new examine();
                    e.patient_id = exa.patient_id;
                    e.nurseID = nurseid;
                    db.examines.Add(e);
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
