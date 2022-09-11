using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder
    .Services
    .Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
    .AddCors(opts => opts.AddDefaultPolicy(bld => bld.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()))
    .AddRouting()
    .AddControllers()
    ;

var app = builder.Build();

// Configure the HTTP request pipeline.

app.Use((ctx, next) => { ctx.Request.EnableBuffering(); return next(); });

app.UseRouting();

app.UseCors();

app.UseEndpoints(x =>
{
    x.MapControllers();
});

app
    .UseDefaultFiles(new DefaultFilesOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(app.Services.GetRequiredService<IHostEnvironment>().ContentRootPath, "Content")),
        RequestPath = "/ui",
    });

app
    .UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(app.Services.GetRequiredService<IHostEnvironment>().ContentRootPath, "Content")),
        RequestPath = "/ui",
    })
    .UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(app.Services.GetRequiredService<IHostEnvironment>().ContentRootPath, "Content")),
        RequestPath = "/ui/css",
    })
    .UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(app.Services.GetRequiredService<IHostEnvironment>().ContentRootPath, "Content")),
        RequestPath = "/ui/webfonts",
    })
    .UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(app.Services.GetRequiredService<IHostEnvironment>().ContentRootPath, "Content")),
        RequestPath = "/ui/woff2",
    })
    .UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(app.Services.GetRequiredService<IHostEnvironment>().ContentRootPath, "Content")),
        RequestPath = "/ui/woff",
    })
    ;

app.MapControllers();

app.Run();
