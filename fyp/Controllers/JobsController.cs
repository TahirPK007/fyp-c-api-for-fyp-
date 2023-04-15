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

        virtualClinicEntities21 db = new virtualClinicEntities21();
        [HttpGet]
        public HttpResponseMessage AssignPatientToDoctor()
        {
            //check for the already recommended and still pending visits.
            var pending = db.visits.Where(v => v.status == 1);
            foreach (var visit in pending)
            {
                var timeDiff = DateTime.Now.Subtract(visit.AssignedDatetime.Value);
                if (timeDiff.TotalSeconds >= 10)
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

    }
}
