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
        virtualClinicEntities3 db = new virtualClinicEntities3();
        [HttpPost]
        public HttpResponseMessage Addpat(patient pat)
        {
            try
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
                    patient get_id = db.patients.OrderByDescending(s => s.patient_id).FirstOrDefault();
                    int id = get_id.patient_id;
                    return Request.CreateResponse(HttpStatusCode.OK, id);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage Checkcnic(int cnic)
        {
            var exist = db.patients.Where(p => p.cnic == cnic).FirstOrDefault();
            if (exist != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, exist);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "patient does'nt exist");
            }
        }
    }
}
