using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SingleSignOnAPI;
using SingleSignOnAPI.AppDbContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:7277",
                                        "http://localhost:80",
                                        "http://localhost:888",
                                        "http://hmmta-tpcap",
                                        "http://hmmt-app07",
                                        "http://hmmta-app05:90",
                                        "http://hmmta-app05:91",
                                        "http://hmmt-app03",
                                        "https://localhost:443",
                                        "https://hmmtweb01.hinothailand.com",
                                        "https://hinommt.com") // Replace with your client's origin
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());

});

builder.Services.AddDbContext<WorkFlowContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddDbContext<TSQLContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TSQLConnection"));
});

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ActiveDirectoryHelper>();

var app = builder.Build();

// Ensure CORS is configured before Authentication and Authorization
app.UseCors("AllowSpecificOrigin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
