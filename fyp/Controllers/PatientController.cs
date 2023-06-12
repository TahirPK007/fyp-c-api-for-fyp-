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
        virtualClinicEntities27 db = new virtualClinicEntities27();
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
        [HttpPost]
        public HttpResponseMessage AllHistory(int patid)
        {
            try
            {
                var data = (from p in db.patients
                            join v in db.vitals on p.patient_id equals v.patient_id
                            join vv in db.visits on v.vital_id equals vv.visit_id
                            join a in db.appointments on vv.visit_id equals a.visit_id
                            join pres in db.prescriptions on a.appointment_id equals pres.appointment_id
                            where p.patient_id == patid
                            where v.patient_id == patid
                            where vv.patient_id== patid
                            where a.appointment_id==patid
                            where pres.appointment_id== a.appointment_id
                            select new {p,v,vv,a,pres}
                            ).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, "Details updated successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
