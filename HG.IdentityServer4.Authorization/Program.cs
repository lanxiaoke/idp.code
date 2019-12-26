using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;
using System.Net;

namespace HG.IdentityServer4.Authorization
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //BuildWebHost(args).Run();

            //var config = new ConfigurationBuilder()
            //  .SetBasePath(Directory.GetCurrentDirectory())
            //  .AddJsonFile("appsettings.json", optional: true)
            //  .Build();

            //var host = WebHost.CreateDefaultBuilder(args)
            //    .UseStartup<Startup>()
            //    .UseConfiguration(config)
            //    .Build();

            //host.Run();




            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables()
            //.AddJsonFile("hosting.json", optional: true)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddCommandLine(args)
            //.AddJsonFile($"certificate.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
            .Build();

            var certificateSettings = config.GetSection("Certificates");
            string certificateFileName = certificateSettings.GetValue<string>("CerPath");
            string certificatePassword = certificateSettings.GetValue<string>("Password");

            var certificate = new X509Certificate2(certificateFileName, certificatePassword);

            var host = new WebHostBuilder()
                .UseKestrel(
                    options =>
                    {
                        options.AddServerHeader = false;
                        options.Listen(IPAddress.Any, 8090, listenOptions =>
                        {
                            listenOptions.UseHttps(certificate);
                        });
                    }
                )
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConsole(); // 加上这个
                })
                .UseConfiguration(config)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls("https://+:8090")
                .Build();

            host.Run();
        }



        //public static void Main(string[] args)
        //{
        //    var config = new ConfigurationBuilder()
        //        .AddCommandLine(args)
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddEnvironmentVariables()
        //        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //        .AddEnvironmentVariables("ASPNETCORE_").Build();

        //    var host = new WebHostBuilder()
        //        .UseConfiguration(config)
        //        .UseKestrel(x =>
        //        {
        //            var pfxFile = Path.Combine(Directory.GetCurrentDirectory(), "cas.clientservice.pfx");
        //            var certificate = new X509Certificate2(pfxFile, "HL2kwEIn");
        //            x.Listen(IPAddress.Any, 8090, listenOptions =>
        //            {
        //                listenOptions.UseHttps(certificate);
        //            });
        //        })
        //        .UseContentRoot(Directory.GetCurrentDirectory())
        //        .UseStartup<Startup>().Build();

        //    host.Run();
        //}

        //private static Action<KestrelServerOptions> ConfigHttps()
        //{
        //    return x =>
        //    {
        //        var pfxFile = Path.Combine(Directory.GetCurrentDirectory(), "*.pfx");                //password 填写申请的密钥
        //        var certificate = new X509Certificate2(pfxFile, "password");
        //        x.UseHttps(certificate);
        //    };
        //}


        //public static IWebHost BuildWebHost(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //        //.UseUrls("http://127.0.0.1:8090")
        //        .UseUrls("http://+:8090")
        //        .UseStartup<Startup>()
        //        .Build();
    }
}
