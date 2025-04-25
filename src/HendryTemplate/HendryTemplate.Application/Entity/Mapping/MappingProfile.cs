using AutoMapper;
using Common.Application.Extensions;
using HendryTemplate.Application.Commands.User.Models;
using HendryTemplate.Application.Responses.User;
using HendryTemplate.Domain.Entities;

namespace HendryTemplate.Application.Entity.Mapping
{

    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Entity to Entity (apply changes)
            CreateMap<User, User>().DisableCtorValidation();
            #endregion

            #region Command > Domain
            CreateMap<UserCommand, User>().OptionalValueRules();
            #endregion

            #region Domain > Response
            CreateMap<User, UserResponse>();
            #endregion
        }
    }
}
