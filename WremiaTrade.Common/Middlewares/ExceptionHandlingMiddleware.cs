// namespace Papara.Common.Middlewares
// {
//     using Microsoft.AspNetCore.Http;
//     using Newtonsoft.Json;
//     using Newtonsoft.Json.Serialization;
//     using System;
//     using System.IO;
//     using System.Linq;
//     using System.Net;
//     using System.Data.Entity.Validation;
//     using System.Security.Claims;
//     using System.Threading.Tasks;
//     using Microsoft.EntityFrameworkCore;
//
//     using WremiaTrade.Common.ApiHelpers.ApiExceptionHandling;
//     using WremiaTrade.Logging.ElasticSearch;
//     using WremiaTrade.RestSharpClient.Exceptions;
//     using WremiaTrade.Services.Abstraction;
//     using WremiaTrade.Common.ApiHelpers;
//     
//     /// <summary>
//     /// Exception handler middleware
//     /// </summary>
//     public class ExceptionHandlingMiddleware
//     {
//         private string[] _ignoredLogFields = null!;
//         private string[] _maskedLogFields = null!;
//         private bool _ignoreAllFields = false;
//
//         private readonly RequestDelegate _next;
//         private readonly ILogService _logService;
//
//         public ExceptionHandlingMiddleware(
//             RequestDelegate next,
//             ILogService logService)
//         {
//             _next = next;
//             _logService = logService;
//         }
//
//         public async Task InvokeAsync(HttpContext context)
//         {
//             try
//             {
//                 await _next(context);
//             }
//             catch (Exception ex)
//             {
//                 await HandleExceptionAsync(context, ex);
//             }
//         }
//
//         private async Task HandleExceptionAsync(HttpContext context, Exception ex)
//         {
//             _logService.Info($"Exception Handling Middleware Catches the error. Exception is : {ex}");
//             // make sure enable request buffering for large requests
//             context.Request.EnableBuffering();
//
//             context.Items.TryGetValue("IgnoreFields", out object? ignoreFields);
//             context.Items.TryGetValue("MaskFields", out object? maskFields);
//
//             var request = context.Request;
//
//             if (ignoreFields != null)
//             {
//                 _ignoredLogFields = ignoreFields.ToString()?.Split(',')!;
//
//                 _ignoreAllFields = _ignoredLogFields.Any(p => p == "*");
//             }
//
//             if (maskFields != null)
//             {
//                 _maskedLogFields = maskFields.ToString()?.Split(',')!;
//             }
//
//             // If content possibly contains file (multipart) we are not copying body, so not reading/logging the request body.
//             var requestBodyReadEnabled = !request.HasFormContentType || !request.Form.Files.Any();
//
//             string requestBody = string.Empty;
//
//             string path = request.Path;
//
//             string queryString = request.QueryString.Value;
//
//             string requestMethod = request.Method;
//
//             string ipAddress = context.GetIpAddress();
//
//             if (string.IsNullOrEmpty(ipAddress))
//                 _logService.Error("Could not get client ip address");
//
//             if (requestBodyReadEnabled && !_ignoreAllFields && request?.Body != null)
//             {
//                 var cleanRequest = HttpContextExtension.GetRawBodyAsync(request).GetAwaiter().GetResult();
//
//                 if (!string.IsNullOrEmpty(requestBody))
//                 {
//                     requestBody = HttpContextExtension.GetCleanRequestBody(cleanRequest, _ignoredLogFields, _maskedLogFields);
//                     request.Body.Seek(0, SeekOrigin.Begin);
//
//                 }
//
//                 if (!string.IsNullOrWhiteSpace(request.ContentType))
//                 {
//                     _logService.Warn($"ApiExceptionHandler missing request properties: IpAdress: {ipAddress}, Path:{path}, QueryString:{queryString}, RequestMethod:{requestMethod}, context.Request.Content:{requestBody}");
//                 }
//             }
//
//             var headers = request.Headers.Where(x => !x.Key.Equals("Authorization") && !x.Key.Equals("Cookie")).ToArray();
//             string messsage = HandleExceptionMessage(ex);
//             string jsonHeaders = JsonConvert.SerializeObject(headers);
//
//             if (!context.User.Identity.IsAuthenticated)
//             {
//                 _logService.Error($"User not found request properties: Message : {messsage} Ex : {ex} Json :{jsonHeaders} Body : {requestBody} IpAdress: {ipAddress}, Path:{path}, QueryString:{queryString}, RequestMethod:{requestMethod}, context.Request.Content:{requestBody}");
//                 return;
//             }
//
//             string userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
//
//             if (ex is DbUpdateConcurrencyException)
//             {
//                 _logService.Warn(messsage, ex);
//             }
//             else if (ex is RestSharpException)
//             {
//                 var restException = ((RestSharpException)ex);
//
//                 _logService.Warn($"restRequestUrl:{restException.Message}", restException);
//             }
//             else
//             {
//                 var log = $"Message : {messsage} Ex : {ex} Json :{jsonHeaders} Body : {requestBody} Method : {requestMethod} Path : {path} QueryString : {queryString} UserId : {userId} IpAddress : {ipAddress}";
//                 _logService.Error(log);
//             }
//
//             context.Response.ContentType = "application/json";
//             context.Response.StatusCode = (int)HttpStatusCode.OK;
//             context.Response.ContentLength = requestBody.Length;
//
//             await context.Response.WriteAsync(PrepareResponseMessage());
//         }
//
//         private static string HandleExceptionMessage(Exception ex)
//         {
//             var result = "API Exception Occurred.";
//
//             try
//             {
//                 if (ex is DbEntityValidationException dbExp)
//                 {
//                     EntityValidationErrors validationErrors = new EntityValidationErrors();
//
//                     foreach (DbEntityValidationResult entityValidationResult in dbExp.EntityValidationErrors)
//                     {
//                         validationErrors.Errors.Add(new EntityValidationErrorWrapper
//                         {
//                             EntityName = entityValidationResult.Entry.Entity.GetType().Name,
//                             ValidationErrors = entityValidationResult.ValidationErrors.ToList()
//                         });
//                     }
//
//                     result = validationErrors.ToString();
//                 }
//
//                 return result;
//             }
//             catch
//             {
//                 return "API DbEntityValidationException Occurred.";
//             }
//         }
//
//         private static string PrepareResponseMessage()
//         {
//             var content = new ServiceResult(ServiceError.DefaultError);
//
//             var camelCaseSetting = new JsonSerializerSettings
//             {
//                 ContractResolver = new CamelCasePropertyNamesContractResolver()
//             };
//
//             string camelCaseJson = JsonConvert.SerializeObject(content, camelCaseSetting);
//
//             return camelCaseJson;
//         }
//     }
// }
