using Issue_972.common;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AdministrationContext>(opt =>
    opt.UseSqlServer($"SERVER=fileserver;Database=VMT Administration;Trusted_Connection=true"));

builder.Services.AddTransient<IRepository<Employee>, EmployeeRepository>();
builder.Services.AddTransient<IRepository<EmployeeTimeBlock>, EmployeeTimeBlockRepository>();
builder.Services.AddTransient<EmployeeTimeBlockRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthorization();

app.MapControllers();

app.Run();
