using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using fyp.Models;

namespace fyp.Controllers
{
    public class SrdocController : ApiController
    {
        virtualClinicEntities23 db = new virtualClinicEntities23();

        [HttpGet]
        public HttpResponseMessage AssignAppointmentsToSrDoctor()
        {
            //get appointments that are not rated
            var appointments = db.appointments.Where(d => d.status == 0).ToList();

            var availableDoctors = new List<seniorDoctor>();

            //now check each senior doctor 1-by-1 that is online and add to list
            foreach (var doc in db.seniorDoctors.Where(x => x.status == 1))
            {
                availableDoctors.Add(doc);
            }
            //if only 1 srdoc is online then all the cases will be assigned to that particual sr doctor
            if (availableDoctors.Count == 1)
            {
                foreach (var x in appointments)
                {
                    foreach (var y in availableDoctors)
                    {
                        x.srdoc_id = y.srdoc_id;
                    }
                }

            }
            //if more than one srdocs are online
            if (availableDoctors.Count > 1)
            {
                int appointmentsPerDoctor = appointments.Count() / availableDoctors.Count();
                int appointmentsassigned = 0;
                foreach (var doctor in availableDoctors)
                {
                    for (int i = 0; i < appointmentsPerDoctor && appointmentsassigned < appointments.Count(); i++)
                    {
                        appointments[appointmentsassigned].srdoc_id = doctor.srdoc_id;
                        appointmentsassigned++;
                    }
                }
                //if there are any remaining appointments then they will be assigned to first sr doc
                while (appointmentsassigned < appointments.Count())
                {
                    appointments[appointmentsassigned].srdoc_id = availableDoctors[0].srdoc_id;
                    appointmentsassigned++;
                }
            }
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        public HttpResponseMessage Srdoclogin(string email, string password)
        {
            try
            {
                var srdoc = db.seniorDoctors.Where(u => u.email == email && u.password == password).FirstOrDefault();
                if (srdoc != null)
                {
                    srdoc.status = 1;
                    db.seniorDoctors.AddOrUpdate(srdoc);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, srdoc);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "user doesnt exist");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        //it will get the no.of appointments assigned to a sr doc
        [HttpGet]
        public HttpResponseMessage MyNewAppointments(int id)
        {
            try
            {
                var appointments = db.appointments.Where(x => x.srdoc_id == id && x.status==0).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, appointments);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }
        //it will get the details about the specific appointment
        [HttpGet]
        public HttpResponseMessage AppointmentDetails(int visitid)
        {
            try
            {
                var data = db.visits.Where(v => v.visit_id == visitid).FirstOrDefault();
                var data1 = db.appointments.Where(a => a.visit_id == visitid).FirstOrDefault();
                var aptid = data1.appointment_id;
                var jrdocid = data.jrdoc_id;
                var patid = data.patient_id;
                var details = (from x in db.visits
                               join p in db.patients on x.patient_id equals p.patient_id
                               join v in db.vitals on p.patient_id equals v.patient_id
                               join jr in db.juniorDoctors on x.jrdoc_id equals jr.jrdoc_id
                               join ac in db.acceptcases on x.visit_id equals ac.visit_id
                               join apt in db.appointments on x.visit_id equals apt.visit_id

                               where x.visit_id == visitid
                               where p.patient_id == patid
                               where v.status == 1 && v.rated == 0 && v.patient_id == patid
                               where jr.jrdoc_id == jrdocid
                               where ac.visit_id == visitid
                               where apt.visit_id == visitid

                               select new { x, p, v, jr, ac, apt }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, details);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }

        //it will get the prescription details according to current appointment
        [HttpGet]
        public HttpResponseMessage Getpresdetails(int aptid)
        {
            try
            {
                var data = db.prescriptions.Where(p => p.appointment_id == aptid).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, data);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }

        //it will finish the current appointment with rating
        [HttpPost]
        public HttpResponseMessage DoneAppointment(int aptid, float rating,int patid)
        {
            try
            {
                var data = db.appointments.Where(a => a.appointment_id == aptid).FirstOrDefault();
                var data1 = db.vitals.Where(v => v.patient_id == patid && v.status == 1 && v.rated == 0).FirstOrDefault();
                data.rating = rating;
                data.status = 1;
                db.appointments.AddOrUpdate(data);
                data1.rated = 1;
                db.vitals.AddOrUpdate(data1);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Appointment Done");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

    }
}
