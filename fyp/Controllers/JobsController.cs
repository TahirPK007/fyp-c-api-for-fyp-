using fyp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;

namespace fyp.Controllers
{
    public class JobsController : ApiController
    {
        virtualClinicEntities9 db = new virtualClinicEntities9();

        [HttpGet]
        public HttpResponseMessage AssignPatientToDoctor()
        {
            //check for the already recommended and still pending visits.
            var pending = db.visits.Where(v => v.status == 1);
            foreach (var visit in pending)
            {
                var timeDiff = DateTime.Now.Subtract(visit.AssignedDatetime.Value);
                if (timeDiff.TotalSeconds >= 120)
                {
                    visit.status = 0;
                    //visit.jrdoc_id = null;
                }
            }

            //get new vistis
            var newVisits = db.visits.Where(d => d.status == 0);

            //get all busy jr docs.
            var currentlyBusy = db.visits.Where(d => d.status == 1);

            var availableDoctors = new List<juniorDoctor>();

            //now check each doctor 1-by-1 that is available and add to list
            foreach (var doc in db.juniorDoctors.Where(x => x.status == 1))
            {
                var result = currentlyBusy.FirstOrDefault(v => v.jrdoc_id == doc.jrdoc_id);
                if (result == null)
                    availableDoctors.Add(doc);
            }

            //now you have new vists and available docs.
            //loop each visit and assign the doc one by one
            foreach (var visit in newVisits)
            {
                var doctor = availableDoctors.FirstOrDefault();
                if (visit.jrdoc_id != null)
                {
                    doctor = availableDoctors.FirstOrDefault(d => d.jrdoc_id != visit.jrdoc_id);
                }

                if (doctor != null)
                {
                    visit.jrdoc_id = doctor.jrdoc_id;
                    visit.status = 1;//recommended to doctor.
                    visit.AssignedDatetime = DateTime.Now;
                    availableDoctors.Remove(doctor);
                }
            }
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        //[HttpGet]
        //public HttpResponseMessage FetchPatentWithVitals()
        //{
        //    try
        //    {
        //        visit vit = new visit();
        //        juniorDoctor dr = new juniorDoctor();
        //        patient pr = new patient();
        //        var newvits = db.visits.Where(v => v.jrdoc_id == dr.jrdoc_id).FirstOrDefault();
        //        var record = (from v in db.visits join p in db.patients on v.patient_id equals p.patient_id join vv in db.vitals on p.patient_id equals vv.patient_id where v.patient_id == newvits.patient_id select new { p, v }).ToList();
        //        return Request.CreateResponse(HttpStatusCode.OK, record);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
        //    }
        //}
    }
}
