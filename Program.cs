using BookApi.Models;
using BookApi.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<BooksDBSettings>(
    builder.Configuration.GetSection(nameof(BooksDBSettings)));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<BooksDBSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IBooksDBSettings, BooksDBSettings>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.Run();


