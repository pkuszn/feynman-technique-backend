using FeynmanTechniqueBackend.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args)
    .AddServices()
    .AddDatabases()
    .AddValidators()
    .AddConfiguration()
    .AddCors();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();