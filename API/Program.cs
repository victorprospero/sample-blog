using Azure.Storage.Blobs;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var connectionString = app.Configuration.GetConnectionString("DefaultConnection");
var azureConnectionString = app.Configuration.GetValue<string>("AzureStorageConnectionString");


var secrets = new Secrets();

app.Configuration.GetSection("Secrets").Bind(secrets);

app.MapGet("/", () => new
{
    ConnectionString = connectionString,
    Secrets = secrets,
    ApiUrl = app.Configuration.GetValue<string>("ApiUrl")
});

app.MapPost("/", (Upload model) => new {
    ImageUrl = UploadBase64Image(model.Image, "user-images")
});

app.Run();

string UploadBase64Image(string base64Image, string container)
{
    var fileName = Guid.NewGuid().ToString() + ".jpg";
    var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(base64Image, "");
    byte[] imageBytes = Convert.FromBase64String(data);
    var blobClient = new BlobClient(azureConnectionString, container, fileName);
    using (var stream = new MemoryStream(imageBytes))
    {
        blobClient.Upload(stream);
    }
    return blobClient.Uri.AbsoluteUri;
}

public record Upload(string Image);

public class Secrets
{
    public string JwtTokenSecret { get; set; }
    public string PrivateKey { get; set; }
    public string ApiKey { get; set; }
}