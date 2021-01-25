using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.API.Services.Common
{
    public enum HttpVerb
    {
        Get,
        Post
    }

    public interface IHttpCallService
    {
        Task<string> CallEndpoint(HttpVerb verb, Uri uri, HttpContent content = null);
    }

    public class HttpCallService : IHttpCallService
    {
        public async Task<string> CallEndpoint(HttpVerb verb, Uri uri, HttpContent content = null)
        {
            using var client = new HttpClient();

            var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = TimeSpan.FromMinutes(5);

            HttpResponseMessage response;

            switch (verb)
            {
                case HttpVerb.Get:
                    response = await client.GetAsync(uri);
                    break;

                case HttpVerb.Post:
                    response = await client.PostAsync(uri, content);
                    break;

                default:
                    throw new NotSupportedException("Http method not supported.");
            }

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var responseError = await response.Content.ReadAsStreamAsync();
                    var detail = await JsonSerializer.DeserializeAsync<ValidationProblemDetails>(responseError, jsonSerializerOptions);

                    if (detail.Errors.Count > 0)
                    {
                        throw new BadRequestException(detail.Title, detail.Errors, $"{detail.Detail}");
                    }

                    //errors are not in the validation problem details 
                    IDictionary<string, string[]> errors = new Dictionary<string, string[]>();

                    var errorsDictionary = detail.Extensions.ToDictionary(x => x.Key, v => v.Value.ToString()?.Replace("\r\n", string.Empty));

                    errors = errorsDictionary.ToDictionary(x => x.Key, v => new List<string> { v.Value.ToString().Replace("[", string.Empty).Replace("]", string.Empty).Replace("\"", string.Empty).Trim() }.ToArray());

                    throw new BadRequestException(detail.Title, errors, $"Please refer to the errors property for additional details.");
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    var responseError = await response.Content.ReadAsStreamAsync();
                    {
                        var detail = await JsonSerializer.DeserializeAsync<ProblemDetails>(responseError, jsonSerializerOptions);
                        throw new NotFoundHttpException(detail.Title, detail.Detail);
                    }
                }
                throw new Exception(response.ReasonPhrase);
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;
        }
    }
}
