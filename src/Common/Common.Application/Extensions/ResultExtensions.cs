using Common.Application.Errors;
using Common.Application.Exceptions;
using Common.Application.Services.Helpers;
using FluentResults;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace Common.Application.Extensions
{
    public static class ResultExtensions
    {
        /// <summary>
        /// Adds a new Error to Result
        /// </summary>
        /// <param name="result"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static ResultBase WithError(this ResultBase result, string errorMessage)
        {
            result.Errors.Add(new Error(errorMessage));
            return result;
        }

        /// <summary>
        /// Gives all of the Error messages in a Result
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string ToErrorMessages(this ResultBase result)
        {
            var errorMessages = new List<string>();
            foreach (var error in result.Errors)
            {
                errorMessages.Add(error.ToString());
            }
            return string.Join("\n", errorMessages);
        }

        /// <summary>
        /// Gives a short error message from a Result, intended to be seen by the front end user
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string ToShortErrorMessage(this ResultBase result)
        {
            var errorMessages = new List<string>();
            foreach (var error in result.Errors)
            {
                var messageSlot = error.Message;
                var reasonSlot = "";
                foreach (var errReason in error.Reasons)
                {
                    var exceptionProp = errReason.GetType().GetProperty("Exception");
                    if (exceptionProp != null)
                    {
                        var exception = exceptionProp.GetValue(errReason);
                        if (exception is Exception ex)
                        {
                            if (!ex.Message.Equals(messageSlot))
                            {
                                messageSlot += $": {ex.Message}";
                            }
                            if(ex.InnerException != null)
                            {
                                reasonSlot += (string.IsNullOrEmpty(reasonSlot))
                                    ? ex.InnerException.Message
                                    : ". " + ex.InnerException.Message;
                            }
                        }
                    }
                }

                var completeError = (string.IsNullOrEmpty(reasonSlot))
                    ?$"{messageSlot}."
                    :$"{messageSlot}. Reasons:{reasonSlot}.";

                errorMessages.Add(completeError);
            }
            return string.Join("\n", errorMessages);
        }

        /// <summary>
        /// Gives all of the Error messages in a ValidationResult
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string ToErrorMessages(this ValidationResult result)
        {
            var errorMessages = new List<string>();
            foreach (var error in result.Errors)
            {
                errorMessages.Add(error.ToString());
            }
            return string.Join("\n", errorMessages);
        }

        /// <summary>
        /// Adds a new BadRequestError to Result
        /// </summary>
        /// <param name="result"></param>
        /// <param name="key"></param>
        /// <param name="context"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static BadRequestError WithBadRequestError(this Result result, string key, object context = null, string property = null)
        {
            //TODO create BadRequestError ressource
            var error = new BadRequestError(key);
            result.WithError(error);
            return error;
        }

        /// <summary>
        /// Adds a new Adds a new BadRequestForbiddenError to Result to Result
        /// </summary>
        /// <param name="error"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static BadRequestError WithMetadataValidationProperty(this BadRequestError error, string property)
        {
            return error.WithMetadata(ResultMetadataKey.ValidationProperty, property) as BadRequestError;
        }

        public static int GetStatusCode(this ResultBase result)
        {
            var code = StatusCodes.Status500InternalServerError;

            if (result.HasException<BadRequestException>())
            {
                code = StatusCodes.Status400BadRequest;
            }
            else if (result.HasException<UnauthorizedAccessException>())
            {
                code = StatusCodes.Status401Unauthorized;
            }
            else if (result.HasException<ForbiddenAccessException>())
            {
                code = StatusCodes.Status403Forbidden;
            }
            else if (result.HasException<NotFoundException>())
            {
                code = StatusCodes.Status404NotFound;
            }
            else if (result.HasException<ValidationException>())
            {
                code = StatusCodes.Status422UnprocessableEntity;
            }
            return code;
        }

        public static void ValidateWithThrow(this ValidationResult[] results)
        {
            if (results.Select(x => x.ToResult()).All(x => x.IsSuccess)) return;
            ThrowExceptions(results);
        }

        public static void ThrowExceptions(ValidationResult[] results)
        {
            var exceptions = results.Select(x => x.ToResult()).Where(x => x.IsFailed).ToList();
            var codes = exceptions.Select(f => f.GetStatusCode()).ToList();
            if (codes.Contains(StatusCodes.Status403Forbidden))
            {
                var forRes = exceptions.FirstOrDefault(x => x.GetStatusCode() == StatusCodes.Status403Forbidden);
                throw new ForbiddenAccessException(forRes.ToErrorMessages());
            }
            else if (codes.Contains(StatusCodes.Status401Unauthorized))
            {
                var unRes = exceptions.FirstOrDefault(x => x.GetStatusCode() == StatusCodes.Status401Unauthorized);
                throw new UnauthorizedAccessException(unRes.ToErrorMessages());
            }
            else throw new Exceptions.ValidationException(results
                            .Where(r => r.Errors.Count != 0)
                            .SelectMany(r => r.Errors)
                            .ToList());
        }

        public static Result ToResult(this ValidationResult result)
        {
            if (result.IsValid) return Result.Ok();
            return ConvertToResult(result.Errors);
        }

        private static Result ConvertToResult(IList<ValidationFailure> errors)
        {
            Result result = null;
            foreach (var error in errors.Where(x => int.TryParse(x.ErrorCode, out _)))
            {
                var code = int.Parse(error.ErrorCode);
                Exception correspondingException = null;
                correspondingException = CreateCorrespondingException(code, error.ErrorMessage);
                var newResult = ResultHelper.MapToResult(correspondingException);
                if (result == null) result = newResult;
                else result = Result.Merge(result, newResult);
            }

            foreach (var error in errors.Where(x => !int.TryParse(x.ErrorCode, out _)))
            {
                var newResult = Result.Fail(error.ErrorMessage);
                if (result == null) result = newResult;
                else result = Result.Merge(result, newResult);
            }

            return result;
        }

        private static Exception CreateCorrespondingException(int code, string message)
        {
            return code switch
            {
                StatusCodes.Status400BadRequest => new BadRequestException(message),
                StatusCodes.Status401Unauthorized => new UnauthorizedAccessException(message),
                StatusCodes.Status403Forbidden => new ForbiddenAccessException(message),
                StatusCodes.Status404NotFound => new NotFoundException(message),
                StatusCodes.Status500InternalServerError => new Exception(message),
                _ => new Common.Application.Exceptions.ValidationException(message)
                {
                    Source = code.ToString()
                }
            };
        }
    }
}
