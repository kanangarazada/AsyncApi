using AsyncApi.Dtos;
using AsyncApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=RequestDB.db"));

var app = builder.Build();

app.UseHttpsRedirection();

//Start EndPoint
app.MapPost("api/v1/products", async (AppDbContext context, ListingRequest listingRequest) =>
{
    if (listingRequest is null) return Results.BadRequest();

    listingRequest.RequestStatus = "ACCEPT";
    listingRequest.EstimatedCompletionTime = "2023-02-06:14:00:00";

    await context.ListingRequests.AddAsync(listingRequest);
    await context.SaveChangesAsync();

    return Results.Accepted($"api/v1/productstatus/{listingRequest.RequestId}", listingRequest);
});


//Status endpoint
app.MapGet("api/v1/productstatus/{requestId}", (AppDbContext context, string requestId) =>
{
    var listingRequest = context.ListingRequests.FirstOrDefault(lr => lr.RequestId == requestId);

    if (listingRequest is null) return Results.NotFound();

    ListingStatus listingStatus = new ListingStatus
    {
        RequestStatus = listingRequest.RequestStatus,
        ResourceURL = String.Empty
    };

    if (listingRequest.RequestStatus!.ToUpper() == "COMPLETE")
    {
        listingStatus.ResourceURL = $"api/v1/products/{Guid.NewGuid().ToString()}";
        return Results.Redirect("https://localhost:7273/" + listingStatus.ResourceURL);
    }

    listingStatus.EstimatedCompletionTime = "2023-02-06:15:00:00";
    return Results.Ok(listingStatus);

});


//Final EndPoint
app.MapGet("api/v1/products/{requestId}", (string requestId) =>
{
    return Results.Ok("This is where you would pass back the final result");
});

app.Run();
