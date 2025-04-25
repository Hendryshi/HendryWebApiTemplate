using AutoMapper;
using Common.Domain.Common;

namespace Common.Application.Mappings
{
    public class CoreModelMapper : Profile
    {
        public CoreModelMapper()
        {
            #region domain > domain
            CreateMap<BaseEntity, BaseEntity>()
                .ForMember(x => x.Id, o => { o.Ignore(); o.UseDestinationValue(); })
                .ForAllMembers(opt =>
                    {
                        // ignore null values
                        opt.Condition((src, dest, srcMember) => srcMember != null);
                    }
                )
            ;

            CreateMap<BaseAuditableEntity, BaseAuditableEntity>()
               .ForMember(x => x.SysCreated, o => { o.Ignore(); o.UseDestinationValue(); })
               .ForMember(x => x.SysLastModified, o => { o.Ignore(); o.UseDestinationValue(); })
           ;
            #endregion

            #region basic mappings
            CreateMap<DateTime?, string>().ConvertUsing(s => s.HasValue ? s.Value.ToString("yyyy-MM-dd HH:mm:ss.ffffff") : string.Empty);
            CreateMap<DateTime, string>().ConvertUsing(s => s.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
            CreateMap<string, Guid>().ConvertUsing(s => string.IsNullOrWhiteSpace(s) ? Guid.Empty : Guid.Parse(s));
            CreateMap<string, Guid?>().ConvertUsing(s => string.IsNullOrWhiteSpace(s) ? null : Guid.Parse(s));
            #endregion
        }
    }
}