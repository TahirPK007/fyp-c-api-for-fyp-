﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using fyp.Models;
using static Antlr.Runtime.Tree.TreeWizard;

namespace fyp.Controllers
{

    public class UserController : ApiController
    {
        virtualClinicEntities25 db = new virtualClinicEntities25();

        [HttpGet]
        public HttpResponseMessage GetAllPrescriptions(double cnic)
        {
            try
            {
                var data = db.patients.Where(p => p.cnic == cnic).FirstOrDefault();
                int patid = data.patient_id;
                var details = (from x in db.appointments
                               join p in db.prescriptions on x.appointment_id equals p.appointment_id
                               where x.patient_id == patid
                               where p.appointment_id == x.appointment_id
                               select new { x, p }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, details);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

    }
}
