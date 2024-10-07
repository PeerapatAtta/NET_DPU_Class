using Microsoft.EntityFrameworkCore;
using MonolithAPI;
using MonolithAPI.Helpers;
using MonolithAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------------- services section -----------------------
var services = builder.Services;
// add controllers
services.AddControllers();
// add database context
services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("sqlite"));
});
// add identity system and its db context
services.AddIdentity<UserModel, RoleModel>(options =>
{
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>();
// add cross-origin resource sharing
services.AddCors(options =>
{
    options.AddPolicy("MyCors", config =>
    {
        config
        .WithOrigins(builder.Configuration.GetSection("AllowedOrigins")
        .Get<string[]>()!)
        .AllowAnyMethod().AllowAnyHeader();
    });
});
// add token helper
services.AddScoped<TokenHelper>();
// configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// ---------------- app section -----------------------
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// redirect http request to https
app.UseHttpsRedirection();
// add CORS middleware
app.UseCors("MyCors");
// map enpoint for controller actions
app.MapControllers();

app.Run();