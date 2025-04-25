//using Core.Common.Application.Extensions;
//using Core.Ptolemy.Common.Domain.Common.Definition;
//using Core.Ptolemy.Common.Journal.Domain;
//using Core.Ptolemy.Design.Application.Interfaces;
//using Core.Ptolemy.Design.Domain.Entities.DSProcedure;
//using Core.Ptolemy.Grpc.Interfaces;
//using FluentValidation;
//using Microsoft.EntityFrameworkCore;

//namespace HendryTemplate.Application.Entity.Rules
//{
//    public static class RulesExtensions
//    {
//        public static IRuleBuilderOptions<T, string> IsValidFormatValue<T>(this IRuleBuilder<T, string> ruleBuilder, IDefinitionTableService def, string format) where T : class
//        {
//            return (IRuleBuilderOptions<T, string>)ruleBuilder.Custom((Type, context) =>
//            {
//                ArgumentNullException.ThrowIfNull(def);
//                if (!string.IsNullOrWhiteSpace(format))
//                {
//                    var isValid = def.CheckValueByFormat(format, Type);
//                    if (!isValid)
//                    {
//                        context.AddFailure(format, "API-ERROR.DESIGN.ENUM-VALUE-INVALID");
//                    }
//                }
//            });
//        }
//        public static IRuleBuilderOptions<T, string> IsValidCountry<T>(this IRuleBuilder<T, string> ruleBuilder, ISearchQueryService service) where T : class
//        {
//            return (IRuleBuilderOptions<T, string>)ruleBuilder.CustomAsync(async (country, context, cancellationToken) =>
//            {
//                ArgumentNullException.ThrowIfNull(service);
//                var getCountries = await service.DEFCountryGet();
//                if (getCountries.IsFailed) context.AddFailure("Country", "API-ERROR.DESIGN.GET-VALUE-FAIL");
//                else
//                {
//                    if (!getCountries.Value.Select(c => c.Value).Contains(country))
//                    {
//                        context.AddFailure("Country", "API-ERROR.DESIGN.ENUM-VALUE-INVALID");
//                    }
//                }
//            });
//        }
//        public static IRuleBuilderOptions<T, string> IsValidPartyRole<T>(this IRuleBuilder<T, string> ruleBuilder, ISearchQueryService service) where T : class
//        {
//            return (IRuleBuilderOptions<T, string>)ruleBuilder.CustomAsync(async (role, context, cancellationToken) =>
//            {
//                ArgumentNullException.ThrowIfNull(service);
//                var getPartyRoles = await service.GetDEFIPRoles();
//                if (getPartyRoles.IsFailed) context.AddFailure("PartyRoles", "API-ERROR.DESIGN.GET-VALUE-FAIL");
//                else
//                {
//                    if (!getPartyRoles.Value.Select(c => c.Value).Contains(role))
//                    {
//                        context.AddFailure("PartyRoles", "API-ERROR.DESIGN.ENUM-VALUE-INVALID");
//                    }
//                }
//            });
//        }
//        public static IRuleBuilderOptions<T, string> IsValidPtoRequestStatus<T>(this IRuleBuilder<T, string> ruleBuilder, ISearchQueryService service) where T : class
//        {
//            return (IRuleBuilderOptions<T, string>)ruleBuilder.CustomAsync(async (status, context, cancellationToken) =>
//            {
//                ArgumentNullException.ThrowIfNull(service);
//                var getPtoRequestStatus = await service.GetDEFPtoRequestStatus(cancellationToken: cancellationToken);
//                if (getPtoRequestStatus.IsFailed) context.AddFailure("PtoRequestStatus", "API-ERROR.DESIGN.GET-VALUE-FAIL");
//                else
//                {
//                    if (!getPtoRequestStatus.Value.Select(c => c.Value).Contains(status))
//                    {
//                        context.AddFailure("PtoRequestStatus", "API-ERROR.DESIGN.ENUM-VALUE-INVALID");
//                    }
//                }
//            });
//        }
//        public static IRuleBuilderOptions<T, string> IsValidPtoRequestType<T>(this IRuleBuilder<T, string> ruleBuilder, ISearchQueryService service) where T : class
//        {
//            return (IRuleBuilderOptions<T, string>)ruleBuilder.CustomAsync(async (type, context, cancellationToken) =>
//            {
//                ArgumentNullException.ThrowIfNull(service);
//                var getPtoRequestType = await service.GetDEFPtoRequestType(cancellationToken: cancellationToken);
//                if (getPtoRequestType.IsFailed) context.AddFailure("PtoRequestType", "API-ERROR.DESIGN.GET-VALUE-FAIL");
//                else
//                {
//                    if (!getPtoRequestType.Value.Select(c => c.Value).Contains(type))
//                    {
//                        context.AddFailure("PtoRequestType", "API-ERROR.DESIGN.ENUM-VALUE-INVALID");
//                    }
//                }
//            });
//        }
//        public static IRuleBuilderOptions<T, string> IsValidDocument<T>(this IRuleBuilder<T, string> ruleBuilder, ISearchQueryService service) where T : class
//        {
//            return (IRuleBuilderOptions<T, string>)ruleBuilder.CustomAsync(async (documentId, context, cancellationToken) =>
//            {
//                ArgumentNullException.ThrowIfNull(service);
//                var getDoc = await service.DocumentGetById(documentId);
//                if (getDoc.IsFailed) context.AddFailure("DocumentId", "API-ERROR.DESIGN.GET-VALUE-FAIL");
//            });
//        }

//        public static IRuleBuilderOptions<T, string> IsValidJournal<T>(this IRuleBuilder<T, string> ruleBuilder, IDSProcedureService<DSProcedure> service) where T : class
//        {
//            return (IRuleBuilderOptions<T, string>)ruleBuilder.CustomAsync(async (journalId, context, cancellationToken) =>
//            {
//                ArgumentNullException.ThrowIfNull(service);
//                var journalResult = await service.GetAsync<Journal>(x => x.Id == Guid.Parse(journalId));
//                if (journalResult.IsFailed) context.AddFailure("JournalId", "API-ERROR.DESIGN.GET-VALUE-FAIL");

//                if (journalResult.Value.Status != DEFJournalStatus.Values.Created) context.AddFailure("JournalId", "API-ERROR.DESIGN.JOURNAL-STATUS-ERROR");
//            });
//        }
//    }
//}
