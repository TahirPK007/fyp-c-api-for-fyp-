using fyp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using System.Web.UI;

namespace fyp.Controllers
{
    public class JobsController : ApiController
    {
        virtualClinicEntities27 db = new virtualClinicEntities27();
        [HttpGet]
        public HttpResponseMessage AssignPatientToDoctor()
        {
            // Check for the already recommended and still pending visits.
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

            // Get new visits
            var newVisits = db.visits.Where(d => d.status == 0).ToList();

            // Get all busy junior docs.
            var currentlyBusy = db.visits.Where(d => d.status == 1).ToList();

            // Get available doctors who have handled fewer than 5 patients and fewer than 5 patients in a day
            var availableDoctors = db.juniorDoctors
                .Where(x => x.status == 1 && x.assignedpatientcount < 5 && x.dailyassignedpatientcount < 5)
                .ToList();

            // Sort the list according to rating
            availableDoctors = availableDoctors.OrderByDescending(x => x.rating).ToList();

            // Now you have new visits and available docs.
            // Loop through each visit and assign a doctor one by one
            foreach (var visit in newVisits)
            {
                var doctor = availableDoctors.FirstOrDefault(d => d.jrdoc_id != visit.jrdoc_id);
                if (doctor != null)
                {
                    // Check if the patient has a follow-up visit with a junior doctor
                    var pat = db.patients.FirstOrDefault(p => p.patient_id == visit.patient_id);
                    if (pat != null && pat.jrdoc_id != null)
                    {
                        doctor = availableDoctors.FirstOrDefault(d => d.jrdoc_id == pat.jrdoc_id);
                    }
                }

                if (doctor != null)
                {
                    visit.jrdoc_id = doctor.jrdoc_id;
                    visit.status = 1; // Recommended to doctor.
                    visit.AssignedDatetime = DateTime.Now;
                    doctor.assignedpatientcount += 1;
                    doctor.dailyassignedpatientcount += 1;
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
        //function to check which doctor is eligible for senior doctor
        [HttpGet]
        public HttpResponseMessage EligibleforSrdoc()
        {
            try
            {
                var eligibledocs = db.juniorDoctors.Where(d => d.assignedpatientcount >= 100 && d.rating > 5).ToList();

                foreach (var data in eligibledocs)
                {
                    var email = data.email;
                    var password = data.password;
                    var name = data.full_name;
                    // Create new SeniorDoctor instance
                    seniorDoctor src = new seniorDoctor();
                    src.full_name= name;
                        src.email = email;
                        src.password = password;
                        src.role = "srdoc";

                    // Add the SeniorDoctor to the senior doctors table
                    db.seniorDoctors.Add(src);
                    // Update the corresponding JuniorDoctor entry
                    data.email = null;
                    data.password = null;
                    db.juniorDoctors.AddOrUpdate(data);
                }

                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "Successfully made the senior doctor");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

    }
}
