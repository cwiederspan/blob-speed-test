using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace MyWeb {

    public class Startup {

        public record Result {

            public string Location { get; internal set; }

            public string Filename { get; internal set; }

            public long Duration { get; internal set; }
        }

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            this.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {

            services.Configure<KestrelServerOptions>(o => {
                o.Limits.MaxRequestBodySize = int.MaxValue;     
            });

            services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => {

                endpoints.MapGet("/", async context => {
                    
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapPost("/upload", async context => {

                    var locations = this.Configuration.GetSection("Storage").GetChildren().Select(value => new {
                        Name = value["Location"],
                        Connection = value["Connection"]
                    }).ToList();

                    var results = new List<Result>();

                    foreach (var location in locations) {

                        foreach (var file in context.Request.Form.Files) {

                            var filename = $"{DateTime.UtcNow:yyyyMMddhhmmss}{file.FileName}";

                            var duration = await this.UploadBlobAsync(location.Connection, file, filename);

                            results.Add(new Result {
                                Filename = filename,
                                Location = location.Name,
                                Duration = duration
                            });
                        }
                    }

                    await context.Response.WriteAsJsonAsync(results);
                });
            });
        }

        private async Task<long> UploadBlobAsync(string connectionString, IFormFile file, string filename) {

            // Start the timer
            var stopwatch = Stopwatch.StartNew();

            // Upload the blob
            using (var stream = file.OpenReadStream()) {

                var blobClient = new BlobClient(connectionString, "speedtest", filename);
                var uploadResult = await blobClient.UploadAsync(stream);
            }

            // Stop the timer
            stopwatch.Stop();

            var duration = stopwatch.ElapsedMilliseconds;

            return duration;
        }
    }
}
