using BusinessLogicLayer.HttpClients;
using eCommerce.OrderMicroservice.BusinessLogicLayer;
using eCommerce.OrderMicroservice.DataAccessLayer;
using eCommerce.OrdersMicroservice.API.Middleware;
using FluentValidation.AspNetCore;
using Polly;

var builder = WebApplication.CreateBuilder(args);

//Add DAL and BLL services
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer(builder.Configuration);

builder.Services.AddControllers();

//FluentValidations
builder.Services.AddFluentValidationAutoValidation();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var usersHost = builder.Configuration["UsersMicroservice:Host"];
var usersPort = builder.Configuration["UsersMicroservice:Port"];
var productHost = builder.Configuration["ProductMicroservice:Host"];
var productPort = builder.Configuration["ProductMicroservice:Port"];

builder
    .Services.AddHttpClient<UsersMicroserviceClient>(client =>
    {
        client.BaseAddress = new Uri($"http://{usersHost}:{usersPort}");
    })
    .AddPolicyHandler(
        Policy
            .HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) => {
                    // var serviceProvider = builder.Services.BuildServiceProvider();
                    // var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                    // logger.LogWarning(
                    //     "Delaying for {delay} seconds, then making retry {retry}.",
                    //     timespan.TotalSeconds,
                    //     retryAttempt
                    // );
                }
            )
    );
builder.Services.AddHttpClient<ProductMicroserviceClient>(client =>
{
    client.BaseAddress = new Uri($"http://{productHost}:{productPort}");
});
var app = builder.Build();

app.UseExceptionHandlingMiddleware();
app.UseRouting();

//Cors
app.UseCors();

//Swagger
app.UseSwagger();
app.UseSwaggerUI();

//Auth
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

//Endpoints
app.MapControllers();

app.Run();
