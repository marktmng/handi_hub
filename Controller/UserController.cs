using Microsoft.AspNetCore.Mvc;
using System;

namespace DotnetAPI.Controllers
{


    public class UserController : ControllerBase
    {

        [HttpGet("TestConnection")]
        public DateTime TestConnection()
        {
            return DateTime.Now;
        }
    }
}