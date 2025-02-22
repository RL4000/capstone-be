﻿using Capstone_API.DBContexts;
using Capstone_API.Models;
using Capstone_API.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Capstone_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // handle login proccess
    public class LoginController : ControllerBase
    {
        private readonly IAccessRepository _repo;

        public LoginController(IAccessRepository repo)
        {
            _repo = repo;
        }

        // GET api/<LoginController>/5
        [HttpPost]
        public async Task<Respond<string>> Login([FromForm] string phone, [FromForm] string password)
        {
            var checkPhone = _repo.CheckPhoneNumberExistAsync(phone);
            var encrypt = _repo.EncryptAsync(password);
            if (await checkPhone == false)
            return new Respond<string>()
            {
                StatusCode = HttpStatusCode.NotFound,
                Error = "Login fail!",
                Message = "This phone number is not registed!",
                Data = null
            };
            var account = _repo.GetAccountAsync(phone,await encrypt);
            if (await account == null)
            return new Respond<string>()
            {
                StatusCode = HttpStatusCode.NotFound,
                Error = "Login fail!",
                Message = "This password is wrong!",
                Data = null
            };
            var jwt = _repo.JWTGenerateAsync(phone, await encrypt);
            return new Respond<string>()
            {
                StatusCode = HttpStatusCode.Accepted,
                Error = "",
                Message = "Login success!",
                Data = "JWT Token: " + await jwt
            };

        }
    }
}
