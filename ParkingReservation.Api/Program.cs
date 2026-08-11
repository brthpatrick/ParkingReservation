using Microsoft.EntityFrameworkCore;
using ParkingReservation.Application.Interfaces;
using ParkingReservation.Application.Services;
using ParkingReservation.Infrastructure.Data;
using ParkingReservation.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ParkingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IParkingReservationRepository, ParkingReservationRepository>();
builder.Services.AddScoped<ReservationService>();

var app = builder.Build();

// Migráció futtatása és seed induláskor
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ParkingDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();