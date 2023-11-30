using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var connectionString = app.Configuration.GetConnectionString("DefaultConnection");
var secrets = new Secrets();
app.Configuration.GetSection("Secrets").Bind(secrets);
app.MapGet("/", () => new
{
    ConnectionString = connectionString,
    Secrets = secrets,
    ApiUrl = app.Configuration.GetValue<string>("ApiUrl")
});

app.Run();

public class Secrets
{
    public string JwtTokenSecret { get; set; }
    public string PrivateKey { get; set; }
    public string ApiKey { get; set; }
}