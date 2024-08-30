using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EmployeeAPI.Controllers
{
    public class EmployeeController : ApiController
    {
        // POST api/<controller>
        public object Post([FromBody] object value)
        {
            Customer_SendResponse result = new Customer_SendResponse();
            result.Name = "John";
            result.City = "Taipei";
            result.Phone = "0912345678";
            result.Type = "student";
            result.Result = true;
            result.ErrMsg = "";

            return result;
        }

        public class Customer_SendResponse
        {
            public string Name { get; set; }

            public string City { get; set; }

            public string Phone { get; set; }

            public string Type { get; set; }

            public bool Result { get; set; }

            public string ErrMsg { get; set; }
        }
    }
}