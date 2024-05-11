var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

bool VerifyRFIDTag( string rfid ) {
    if ( rfid == "0A 48 3A 29" ) {
        return true;
    } else if ( rfid == "99 F7 5F B3" ) {
        return false;
    } else {
        return false;
    }
}

app.MapGet("/api/scan/{rfid}", (HttpContext httpContext, string rfid) => {
    var ipAddress = httpContext.Connection.RemoteIpAddress.ToString();
    Console.WriteLine($"Request from IP: {ipAddress}");

    Console.WriteLine($"GET /api/scan/{rfid}.");
    bool isValid = VerifyRFIDTag(rfid);
    if (isValid)
    {
        Console.WriteLine($"RFID tag {rfid} verified successfully.");
        return new { Status = "Success", Message = $"RFID tag {rfid} verified successfully." };
    }
    else
    {
        Console.WriteLine($"Failed to verify RFID tag {rfid}.");
        return new { Status = "Error", Message = $"Failed to verify RFID tag {rfid}." };
    }
});

app.Run();

// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast")
// .WithOpenApi();

// app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
