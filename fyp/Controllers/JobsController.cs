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
        virtualClinicEntities28 db = new virtualClinicEntities28();
        [HttpGet]
        public HttpResponseMessage AssignPatientToDoctor()
        {
            //getting threshold value
            var dataaa = db.admins.Where(a => a.adminid == 1).FirstOrDefault();
            var threshold = dataaa.threshold;

            //get new visits
            var newVisits = db.visits.Where(d => d.status == 0).ToList();
            var availableDoctors = new List<juniorDoctor>();

            //now check each doctor one by one that is available and add to the list
            foreach (var doc in db.juniorDoctors.Where(x => x.status == 1&&x.count<threshold))
            {
                    availableDoctors.Add(doc);
            }
            //sort the list according to rating
            availableDoctors.OrderByDescending(xx=>xx.rating).ToList();

            //now you have new visits and available docs.
            //loop each visit and assign the doc one by one
            foreach (var visit in newVisits)
            {
                var doctor = availableDoctors.FirstOrDefault(d => d.status==1&&d.count<threshold);
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
                    var jrdoc=db.juniorDoctors.Where(d=>d.jrdoc_id==doctor.jrdoc_id).FirstOrDefault();
                    var checkingcount = jrdoc.count;
                    if(checkingcount == threshold)
                    {
                        availableDoctors.Remove(doctor);
                    }
                    else
                    {
                        jrdoc.count += 1;
                        db.juniorDoctors.AddOrUpdate(jrdoc);
                    }
                    
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
        //updating jrdoc count
        [HttpPost]
        public HttpResponseMessage Updatingjrdoccount(int jrdocid)
        {

            try
            {
                var data = db.juniorDoctors.Where(d => d.jrdoc_id == jrdocid).FirstOrDefault();
                data.count -= 1;
                db.juniorDoctors.AddOrUpdate(data);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "successfuly updated count");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        //setting globally patient threshold
        [HttpPost]
        public HttpResponseMessage SettingThreshold(int thresh)
        {

            try
            {
                var data = db.admins.Where(a => a.adminid == 1).FirstOrDefault();
                data.threshold = thresh;
                db.admins.AddOrUpdate(data);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "successfuly set threshold");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }


    }
}
