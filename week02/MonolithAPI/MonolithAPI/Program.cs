using Microsoft.EntityFrameworkCore;
using MonolithAPI;

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
// map enpoint for controller actions
app.MapControllers();

app.Run();
