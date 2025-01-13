using AutoMapper;
using Investigator.Services.UserManagerAPI.Data;
using Investigator.Services.UserManagerAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Investigator.Services.UserManagerAPI.Controllers
{
    [Route("api/userManager]")]
    [ApiController]
    public class UserManagerAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _responseDto;
        private IMapper _mapper;
        public UserManagerAPIController(AppDbContext db, IMapper mapper) 
        {
            _db = db;
            _responseDto = new();
            _mapper = mapper;
        }
        [HttpGet]
        public ResponseDto Get()
        {
            try
            {

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }
            return _responseDto;
        }
        [HttpPost]
        public async Task<IActionResult> Login()
        {
            var client = new HttpClient();
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", "YOUR_CLIENT_ID"),
                new KeyValuePair<string, string>("client_secret", "YOUR_CLIENT_SECRET"),
                new KeyValuePair<string, string>("username", "delivrewelo@gmail.com"),
                new KeyValuePair<string, string>("password", "DelivreMateCode1.")
            });
            var response = await client.PostAsync("https://login.salesforce.com/services/oauth2/token", data);
            var content = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<dynamic>(content).access_token;

            var account = new
            {
                Name = "User Name"
            };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var accountResponse = await client.PostAsJsonAsync("https://your-instanc.salesforce.com/services/data/vXX.X/sobjects/Account/", account);
            //matecode - dev - ed.develop.my.salesforce.com
            return Ok();
        }
    }
}
