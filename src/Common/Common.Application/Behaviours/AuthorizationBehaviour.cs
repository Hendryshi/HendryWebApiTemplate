//using Common.Application.Exceptions;
//using Common.Application.Interfaces;
//using Common.Application.Services.Logging;
//using Common.Domain.Attributes.Security;
//using MediatR;
//using Microsoft.Extensions.Logging;
//using System.Reflection;

//namespace Common.Application.Behaviours
//{
//    public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
//    {
//        private readonly ICurrentUserService _currentUserService;
//        private readonly IIdentityService _identityService;
//        private readonly ILogger<TRequest> _logger;

//        public AuthorizationBehaviour(
//            ICurrentUserService currentUserService,
//            IIdentityService identityService,
//            ILogger<TRequest> logger)
//        {
//            _currentUserService = currentUserService;
//            _identityService = identityService;
//            _logger = logger;
//        }

//        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//        {
//            _logger.Debug($"Entering in method {System.Reflection.MethodBase.GetCurrentMethod().Name} of service {this.GetType().Name}");

//            var authorizeAttributes = request.GetType().GetCustomAttributes<OptonumAuthorizeAttribute>();

//            if (authorizeAttributes.Any())
//            {

//                // Check if user is authenticated
//                if (string.IsNullOrWhiteSpace(_currentUserService.UserId))
//                {
//                    _logger.ErrorSecurity("User is not authenticated");
//                    throw new UnauthorizedAccessException();
//                }

//                // Check if user is authorized to call the endpoint
//                var authorizedResult = await IsAuthorized(authorizeAttributes);
//                if (authorizedResult.HasValue)
//                {
//                    var authorized = authorizedResult.Value;
//                    if (!authorized)
//                    {
//                        _logger.ErrorSecurity("User is not authorized to call this endpoint.");
//                        throw new ForbiddenAccessException();
//                    }
//                }
//            }

//            // User is authorized / authorization not required
//            _logger.Debug($"Leaving method {System.Reflection.MethodBase.GetCurrentMethod().Name} of service {this.GetType().Name}");
//            return await next();
//        }

//        /// <summary>
//        /// Check if user is authorized to call the endpoint
//        /// <br />true: authorized
//        /// <br />false: not authorized
//        /// <br />null: not applicable because there is no authorization attribute for all methods used
//        /// </summary>
//        /// <param name="authorizeAttributes"></param>
//        /// <returns></returns>
//        private async Task<bool?> IsAuthorized(IEnumerable<OptonumAuthorizeAttribute> authorizeAttributes)
//        {
//            var authorizationMethods = new List<Func<IEnumerable<OptonumAuthorizeAttribute>, Task<bool?>>>
//            {
//                IsAuthorizedByRole,
//                IsAuthorizedByClaim,
//            };

//            bool foundFalse = false;
//            foreach (var authorizationMethod in authorizationMethods)
//            {
//                var authorized = await authorizationMethod(authorizeAttributes);
//                if (authorized.HasValue && authorized == true) return true;
//                if (authorized.HasValue && authorized == false) foundFalse = true;
//            }

//            if (foundFalse) return false;
//            else return null;
//        }

//        /// <summary>
//        /// Check if user is authorized to call the endpoint based on roles
//        /// <br />true: authorized
//        /// <br />false: not authorized
//        /// <br />null: not applicable because there is no authorization attribute of this type
//        /// </summary>
//        /// <param name="authorizeAttributes"></param>
//        /// <returns></returns>
//        private async Task<bool?> IsAuthorizedByRole(IEnumerable<OptonumAuthorizeAttribute> authorizeAttributes)
//        {
//            var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));
//            if (!authorizeAttributesWithRoles.Any()) return null;

//            foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
//            {
//                foreach (var role in roles)
//                {
//                    var isInRole = await _identityService.UserIsInRoleAsync(_currentUserService.UserId, role.Trim());
//                    if (isInRole) return true;
//                }
//            }
//            return false;
//        }

//        /// <summary>
//        /// Check if user is authorized to call the endpoint based on claims
//        /// <br />true: authorized
//        /// <br />false: not authorized
//        /// <br />null: not applicable because there is no authorization attribute of this type
//        /// </summary>
//        /// <param name="authorizeAttributes"></param>
//        /// <returns></returns>
//        private async Task<bool?> IsAuthorizedByClaim(IEnumerable<OptonumAuthorizeAttribute> authorizeAttributes)
//        {
//            var authorizeAttributesWithClaims = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.BackClaim));
//            if (!authorizeAttributesWithClaims.Any()) return null;

//            foreach (var claim in authorizeAttributesWithClaims.Select(a => a.BackClaim).Where(x => !string.IsNullOrWhiteSpace(x)))
//            {
//                var authorized = await _identityService.HasBackClaim(_currentUserService.UserId, claim);
//                if (authorized) return true;
//            }
//            return false;
//        }
//    }
//}
