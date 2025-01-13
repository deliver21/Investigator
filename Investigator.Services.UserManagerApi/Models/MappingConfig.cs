using AutoMapper;

namespace Investigator.Services.UserManagerAPI.Models
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingconfig = new MapperConfiguration(config =>
            {

            });
            return mappingconfig;
        }
    }
}
