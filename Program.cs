using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Booking.Command.Models;
using Booking.Command.utils;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddCors( x => 
    x.AddDefaultPolicy(p =>
    p.AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();




app.MapPost("/book", async ([FromBody] BookingRequest request) =>
{
    var token = request.IdToken;
    var idTokenDetails = new JwtSecurityToken(token);

    var userId = idTokenDetails.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? "";
    var givenName = idTokenDetails.Claims.FirstOrDefault(x => x.Type == "given_name")?.Value ?? "";
    var familyName = idTokenDetails.Claims.FirstOrDefault(x => x.Type == "family_name")?.Value ?? "";
    var email = idTokenDetails.Claims.FirstOrDefault(x => x.Type == "email")?.Value ?? "";
    var phoneNumber = idTokenDetails.Claims.FirstOrDefault(x => x.Type == "phone_number")?.Value ?? "";

    var dto = new BookingDto
    {
        Id = Guid.NewGuid().ToString(),
        HotelId = request.HotelId,
        CheckinDate = request.CheckinDate,
        CheckoutDate = request.checkoutDate,
        UserId = userId,
        GivenName = givenName,
        FamilyName = familyName,
        Email = email,
        PhoneNumber = phoneNumber,

        Status = BookingStatus.Pending
    };

    using var dbClient = new AmazonDynamoDBClient();
    using var dbContext = new DynamoDBContext(dbClient);
    await dbContext.SaveAsync(dto);
});

app.MapGet("/health", () => new HttpResponseMessage(System.Net.HttpStatusCode.OK));

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
