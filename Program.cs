using BookApi.Models;
using BookApi.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDB settings
builder.Services.Configure<IBooksDBSettings>(
    builder.Configuration.GetSection(nameof(BooksDBSettings)));

builder.Services.AddSingleton<IBooksDBSettings>(sp =>
    sp.GetRequiredService<IOptions<BooksDBSettings>>().Value);

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<IBooksDBSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// Register BookRepository
builder.Services.AddScoped<BookRepository>();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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