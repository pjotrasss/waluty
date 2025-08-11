//variable for preparing app
var builder = WebApplication.CreateBuilder(args);
//add services to app
builder.Services.AddControllers();

//building app
var app = builder.Build();
app.MapControllers();

//starting app
app.Run();