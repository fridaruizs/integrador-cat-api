using integrador_cat_api.Services;
using Datadog.Trace;
using Datadog.Trace.Configuration;

var builder = WebApplication.CreateBuilder(args);


Tracer.Configure(new TracerSettings
{
    ServiceName = "integrador-cat-api",
    Environment = "production"
});


builder.Services.AddControllers();
builder.Services.AddHttpClient<ICatService, CatService>();
builder.Services.AddSingleton<ICatService, CatService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
