using AutoMapper;
using Investigator.Models.DTOs;

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
                config.CreateMap<QuestionOptionDto, QuestionOption>().ReverseMap();
                config.CreateMap<JiraTicketDto, JiraTicket>().ReverseMap();

            });
            return mappingconfig;
        }
    }
}
