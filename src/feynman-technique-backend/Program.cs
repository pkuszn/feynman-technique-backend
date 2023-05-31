using FeynmanTechniqueBackend.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args)
    .AddServices()
    .AddDatabases()
    .AddValidators();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
