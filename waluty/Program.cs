//necessary namespaces
using waluty.Data;
using waluty.Repositories;
using waluty.Services;

var builder = WebApplication.CreateBuilder(args);

//add controllers to app
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

//http client for API calls
builder.Services.AddHttpClient();

//connection Factory
var connString = builder.Configuration.GetConnectionString("CurrenciesDb")
    ?? throw new InvalidOperationException("Connection string 'CurrenciesDb' not found.");
builder.Services.AddSingleton(new DbConnectionFactory(connString));

//repositories
builder.Services.AddScoped<CurrencyRateRepository>();
builder.Services.AddScoped<ExchangeRateTableRepository>();

//services
builder.Services.AddScoped<DbCurrenciesService>();
builder.Services.AddScoped<ApiCurrenciesService>();

//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//building app
var app = builder.Build();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

//starting app
app.Run();