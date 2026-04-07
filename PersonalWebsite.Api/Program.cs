using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;
using PersonalWebsite.Api.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AdventureWorksContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AdventureWorks")));
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
builder.Services.AddScoped<IShipperService, ShipperService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AngularPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
