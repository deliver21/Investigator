using AutoMapper;
using Investigator.Models.DTO;

namespace Investigator.Models
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingconfig = new MapperConfiguration(config =>
            {
                config.CreateMap<QuestionDto, Question>().ReverseMap();
                config.CreateMap<FormDto, Form>().ReverseMap();

            });
            return mappingconfig;
        }
    }
}
