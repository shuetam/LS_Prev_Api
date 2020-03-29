using Live.Repositories;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Live.Mapper;
using System.Diagnostics;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;
using Live.Core;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace Live.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    [Route("api/[controller]")]
    public class BestsellersController : LiveController
    {
        private readonly IBestsellersRepository _bookRepository;

        public BestsellersController(IBestsellersRepository bookRepository)
        {
            this._bookRepository = bookRepository;
        }

        [HttpGet("getbestsellers")]
        public async Task<IActionResult> GetBestsellers()
        {
            var books = await _bookRepository.GetActuallBestsellersAsync();
            Console.WriteLine("------------getting books---------------");
            Console.WriteLine(books[0].title);
            return Json(books);
        }

        [HttpPost("update")]
        public async Task UpdateBestsellser()
        {
           
            await _bookRepository.UpdateAsync();
        }


    }
}
