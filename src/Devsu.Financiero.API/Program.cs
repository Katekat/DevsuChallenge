using Devsu.Financiero.API.Features.Clientes;
using Devsu.Financiero.API.Infrastructure;

using MassTransit;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FinancieroDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FinancieroDb")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ClienteCreadoConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"]);
            h.Password(builder.Configuration["RabbitMq:Password"]);
        });

        // Reintenta automáticamente el procesamiento de cualquier mensaje que falle
        // si la BD momentáneamente no disponible), 3 veces con 5s de espera entre cada uno.
        cfg.UseMessageRetry(r => r.Interval(6, TimeSpan.FromSeconds(5)));
        cfg.ConfigureEndpoints(context);


        
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FinancieroDbContext>();
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
