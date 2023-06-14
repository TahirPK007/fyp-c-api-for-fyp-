using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.OleDb;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using fyp.Models;

namespace fyp.Controllers
{
    public class PatientController : ApiController
    {
        virtualClinicEntities28 db = new virtualClinicEntities28();
        //adding new patient details
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
        //checking if the patient is still exist
        [HttpGet]
        public HttpResponseMessage Checkingcnic(string cnic)
        {
            try
            {
                var data = db.patients.Where(p => p.cnic == cnic).FirstOrDefault();
                if(data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, data);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "no record");
                }
                
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        //populating the visit table when the vitals of the current patient's get added
        [HttpPost]
        public HttpResponseMessage Visits(int patient_id, int status,int nurseid)
        {
            try
            {
                visit v = new visit();
                var d = DateTime.Now;
                v.patient_id = patient_id;
                v.status = status;
                v.nurseID = nurseid;
                v.date = d.ToShortDateString();
                v.time = d.ToLongTimeString();
                db.visits.Add(v);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "New visit added");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        //updating the existing patients details
        [HttpPost]
        public HttpResponseMessage Updatepatdetails(patient patt, int patient_id, string newcnic)
        {
            try
            {
                var patienttoupdate = db.patients.Where(p => p.patient_id == patient_id).FirstOrDefault();
                var d = DateTime.Now;
                patienttoupdate.cnic = newcnic;
                patienttoupdate.full_name = patt.full_name;
                patienttoupdate.relation = patt.relation;
                patienttoupdate.relative_name = patt.relative_name;
                patienttoupdate.dob = patt.dob;
                patienttoupdate.gender = patt.gender;
                patienttoupdate.date = d.ToShortDateString();
                patienttoupdate.time = d.ToShortTimeString();
                db.patients.AddOrUpdate(patienttoupdate);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Details updated successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        //this function will get all the patient history
        [HttpGet]
        public HttpResponseMessage AllHistory(int patid)
        {
            try
            {
                var details = (from x in db.appointments
                               join p in db.prescriptions on x.appointment_id equals p.appointment_id
                               join cmnts in db.commentsTests on x.appointment_id equals cmnts.appointment_id
                               join v in db.vitals on x.appointment_id equals v.appointment_id
                               where x.patient_id == patid
                               where p.appointment_id == x.appointment_id
                               where cmnts.appointment_id==x.appointment_id
                               where v.appointment_id == x.appointment_id
                               select new { x, p,cmnts,v }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, details);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
