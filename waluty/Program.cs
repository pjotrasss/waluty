//variable for preparing app
using Microsoft.EntityFrameworkCore;
using waluty.Data;

//adding services to app
var builder = WebApplication.CreateBuilder(args);
//add controllers to app
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
//http client for API calls
builder.Services.AddHttpClient();
//context registrating for db
builder.Services.AddDbContext<CurrenciesDbContext>
(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("CurrenciesDb"))
);

//building app
var app = builder.Build();
app.MapControllers();

//starting app
app.Run();