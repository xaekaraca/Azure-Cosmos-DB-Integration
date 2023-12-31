using CosmosDbEntegrasyon.Api;
using EntityFrameworkCosmos.Data.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddBusinessServices();
builder.Services.AddCosmosDb<EntityFrameworkContext>(builder.Configuration);
//builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddMicrosoftCosmosDb(builder.Configuration);

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();