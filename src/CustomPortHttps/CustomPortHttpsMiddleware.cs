﻿using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CustomPortHttps
{
    public class CustomPortHttpsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly int _port;
        private const string SCHEME_HTTPS = "https";
        public CustomPortHttpsMiddleware(RequestDelegate next, int port)
        {
            if (!(port > 0 && port < 65535)) throw new ArgumentOutOfRangeException(nameof(port), "Port must be greater than 0 and less than 65535");
            _next = next;
            _port = port;
        }
        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.MovedPermanently)
            {
                var location = context.Response.GetTypedHeaders().Location;
                if (String.Equals(location.Scheme, SCHEME_HTTPS, StringComparison.OrdinalIgnoreCase))
                {
                    var builder = new UriBuilder(location);
                    builder.Port = _port;
                    context.Response.Headers["Location"] = builder.Uri.ToString();
                }

            }
        }
    }
    public static class CustomPortHttpsMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomPortHttps(this IApplicationBuilder app, int port)
        {
            return app.UseMiddleware<CustomPortHttpsMiddleware>(port);
        }
    }
}
