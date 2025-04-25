using AutoMapper;
using Common.Application.Errors;
using Common.Application.Exceptions;
using Common.Application.Services.Logging;
using FluentResults;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Common.Application.Services.Helpers
{
    public class ResultHelper
    {
        private static ILogger<ResultHelper> _logger;
        private static ILogger<ResultHelper> Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = Logging.Logger.CreateLogger<ResultHelper>();
                }
                return _logger;
            }
        }

        /// <summary>
        /// Returns a fail result with a typed result error based on the type of the exception: BadRequestNotFoundException, BadRequestForbiddenException, BadRequestException, InternalServerException.
        /// If exception type cannot be mapped it returns a result with an InternalServerError
        /// Exception is added to the result error with CausedBy
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Result MapToResult(Exception e,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0)
        {
            var rslt = Result.Fail(MapToResultError(e));
            Logger.LogResult(rslt, level: LogLevel.Error, memberName: memberName, sourceFilePath: sourceFilePath, sourceLineNumber: sourceLineNumber);
            return rslt;
        }

        /// <summary>
        /// Returns a typed result error based on the type of the exception: BadRequestNotFoundError, BadRequestForbiddenError, BadRequestNotFoundException, InternalServerError (Default).
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Error MapToResultError(Exception e, Func<Exception, string> mainMessage = null)
        {
            var errorMessage = mainMessage is null ? $"Exception has been raised" : mainMessage.Invoke(e);

            if (e is BadRequestException)
            {
                return new BadRequestError(errorMessage).CausedBy(e);
            }
            else if (e is ConfigErrorException)
            {
                return new ConfigErrorError(errorMessage).CausedBy(e);
            }
            else if (e is ElasticOperationException)
            {
                return new ElasticOperationError(errorMessage).CausedBy(e);
            }
            else if (e is ForbiddenAccessException)
            {
                return new ForbiddenAccessError(errorMessage).CausedBy(e);
            }
            else if (e is InvalidStatusException)
            {
                return new InvalidStatusError(errorMessage).CausedBy(e);
            }
            else if (e is NotFoundException)
            {
                return new NotFoundError(errorMessage).CausedBy(e);
            }
            else if (e is ObjectAlreadyExistsException)
            {
                return new ObjectAlreadyExistsError(errorMessage).CausedBy(e);
            }
            else if (e is ServiceNotAvailableException)
            {
                return new ServiceNotAvailableError(errorMessage).CausedBy(e);
            }
            else if (e is ValidationException)
            {
                return new ValidationError(errorMessage).CausedBy(e);
            }
            else if (e is ArgumentException)
            {
                return new ArgumentNullError(errorMessage).CausedBy(e);
            }
            else if (e is ArgumentNullException)
            {
                return new ArgumentNullError(errorMessage).CausedBy(e);
            }
            else if (e is AutoMapperMappingException)
            {
                return new AutoMapperMappingError(errorMessage).CausedBy(e);
            }
            else if (e is InvalidOperationException)
            {
                return new InvalidOperationError(errorMessage).CausedBy(e);
            }
            else
            {
                return new InternalServerError(errorMessage).CausedBy(e);
            }

        }

    }
}
