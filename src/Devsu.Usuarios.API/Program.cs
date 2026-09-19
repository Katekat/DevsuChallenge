using Devsu.Usuarios.API.Infrastructure;
using Devsu.Usuarios.API.Features.Clientes.RegistrarCliente;

using MassTransit;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Persistencia local
builder.Services.AddDbContext<UsuariosDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UsuariosDb")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// Registro e infraestructura local de MassTransit
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration["RabbitMq:Host"];
        var user = builder.Configuration["RabbitMq:Username"];
        var pass = builder.Configuration["RabbitMq:Password"];

        cfg.Host(host, "/", h =>
        {
            h.Username(user);
            h.Password(pass);
        });

        cfg.UseMessageRetry(r => r.Interval(6, TimeSpan.FromSeconds(5)));

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UsuariosDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.UseGlobalExceptionHandler();

app.MapControllers();



app.Run();
