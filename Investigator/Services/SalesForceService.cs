using AutoMapper;
using Investigator.Models;
using Investigator.Repository.IRepository;
using Investigator.Services.IServices;
using Salesforce.Chatter.Models;
using Salesforce.Common;
using Salesforce.Force;

namespace Investigator.Services
{
    public class SalesForceService:ISalesForceService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unit;
        private AuthenticationClient _auth;
        public SalesForceService(IConfiguration config, IMapper mapper, IUnitOfWork unit) 
        {
            _mapper = mapper;
            _unit = unit;
            _auth = new AuthenticationClient();
            _auth.UsernamePasswordAsync(
                 config.GetSection("SalesForceCredentials").GetValue<string>("ConsumerKey"),
                 config.GetSection("SalesForceCredentials").GetValue<string>("ConsumerSecret"),
                 config.GetSection("SalesForceCredentials").GetValue<string>("Username"),
                 config.GetSection("SalesForceCredentials").GetValue<string>("Password")
             ).GetAwaiter().GetResult();
        }
        public async Task<bool> CreateUserToSalesForce(ApplicationUser user)
        {
            if(user == null) return false;
            try
            {
                var instanceUrl = _auth.InstanceUrl;
                var accessToken = _auth.AccessToken;
                var apiVersion = _auth.ApiVersion;
                var client = new ForceClient(instanceUrl, accessToken, apiVersion);
                var account = new Account()
                {
                    Name = user.UserName,
                    Description = "Investigator"
                };
                var id = await client.CreateAsync("Account", account);
                if(id.Success)
                {
                    user.SalesForceUserId = id.Id.ToString();
                    _unit.ApplicationUser.Update(user);
                    _unit.Save();
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}
