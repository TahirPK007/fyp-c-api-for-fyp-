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
        virtualClinicEntities19 db = new virtualClinicEntities19();
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
            //if more than srdocs are online
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

        [HttpGet]
        public HttpResponseMessage MyNewAppointments(int id)
        {
            try
            {
                var appointments = db.appointments.Where(x=>x.srdoc_id== id).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, appointments);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }

    }
}
