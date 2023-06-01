using fyp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;

namespace fyp.Controllers
{
    public class JobsController : ApiController
    {

        virtualClinicEntities27 db = new virtualClinicEntities27();
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

                    // Decrement the rating of the doctor
                    var doctor = db.juniorDoctors.FirstOrDefault(d => d.jrdoc_id == visit.jrdoc_id);
                    if (doctor != null)
                    {
                        doctor.rating -= 1;
                    }
                }
            }

            //get new visits
            var newVisits = db.visits.Where(d => d.status == 0);

            //get all busy junior docs.
            var currentlyBusy = db.visits.Where(d => d.status == 1);

            var availableDoctors = new List<juniorDoctor>();

            //now check each doctor one by one that is available and add to the list
            foreach (var doc in db.juniorDoctors.Where(x => x.status == 1))
            {
                
                var result = currentlyBusy.FirstOrDefault(v => v.jrdoc_id == doc.jrdoc_id);
                if (result == null)
                    availableDoctors.Add(doc);
            }
            //sort the list according to rating
            availableDoctors.OrderByDescending(x => x.rating).ToList();

            //now you have new visits and available docs.
            //loop each visit and assign the doc one by one
            foreach (var visit in newVisits)
            {
                var doctor = availableDoctors.FirstOrDefault(d => d.jrdoc_id != visit.jrdoc_id);
                if (doctor != null)
                {
                    //checkin if the patient has follow up visit with the junior doctor
                    var pat = db.patients.Where(p => p.patient_id == visit.patient_id).FirstOrDefault();
                    if (pat != null && pat.jrdoc_id != null)
                    {
                        doctor = availableDoctors.FirstOrDefault(d => d.jrdoc_id == pat.jrdoc_id);
                    }
                }

                if (doctor != null)
                {
                    visit.jrdoc_id = doctor.jrdoc_id;
                    visit.status = 1; //recommended to doctor.
                    visit.AssignedDatetime = DateTime.Now;
                    availableDoctors.Remove(doctor);
                }
            }
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK);
        }



        //this function will get the rating from apt table and calculate the avg and assign to jrdoc in his table
        [HttpPost]
        public HttpResponseMessage CalculateRatingAndAssingToJrdoc(int jrdocid)
        {

            try
            {
                var jr = db.juniorDoctors.Where(d => d.jrdoc_id == jrdocid).FirstOrDefault();
                var apt = db.appointments.Where(a => a.jrdoc_id == jrdocid).ToList();
                //var data = db.appointments.Where(a => a.time.Contains("%4%")).ToList();
                float avgrating = (float)apt.Average(b => b.rating);
                jr.rating = avgrating;
                db.juniorDoctors.AddOrUpdate(jr);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "successfuly calculated and assigned rating");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

    }
}
