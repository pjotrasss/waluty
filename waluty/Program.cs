//variable for preparing app
var builder = WebApplication.CreateBuilder(args);
//add services to app
builder.Services.AddControllers();
//http client for API calls
builder.Services.AddHttpClient();

//building app
var app = builder.Build();
app.MapControllers();

//starting app
app.Run();