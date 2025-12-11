using BookCatalog.Application.Interfaces;
using BookCatalog.Infrastructure.Persistence;
using MediatR;
using System.Reflection;
using Dapper;

// Register Dapper type handler for decimal
SqlMapper.AddTypeHandler(new DecimalTypeHandler());

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("SqliteConnection");
builder.Services.AddSingleton<IDbConnectionFactory>(sp => new SqliteConnectionFactory(connectionString!));

builder.Services.AddTransient<DbInitializer>();

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IBookRepository).GetTypeInfo().Assembly));


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    initializer.Initialize();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.Run();

