using fyp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace fyp.Controllers
{
    public class JobsController : ApiController
    {
        virtualClinicEntities5 db = new virtualClinicEntities5();

        [HttpGet]
        public HttpResponseMessage AssignPatientToDoctor()
        {

            //get new vistis
            var newVisits = db.visits.Where(d => d.status == 0);

            //get all busy jr docs.
            var currentlyBusy = db.visits.Where(d => d.status == 1);

            var availableDoctors = new List<juniorDoctor>();

            //now check each doctor 1-by-1 that is available and add to list
            foreach (var doc in db.juniorDoctors.Where(x => x.status == 1))
            {
                var result = currentlyBusy.FirstOrDefault(v =>v.jrdoc_id == doc.jrdoc_id);
                if (result == null)
                    availableDoctors.Add(doc);
            }

            //now you have new vists and available docs.
            //loop each visit and assign the doc one by one
            foreach (var visit in newVisits)
            {
                var doctor = availableDoctors.FirstOrDefault();
                if (doctor != null)
                {
                    visit.jrdoc_id = doctor.jrdoc_id;
                    visit.status = 1;
                    availableDoctors.Remove(doctor);
                }
            }
            db.SaveChanges();
         return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
