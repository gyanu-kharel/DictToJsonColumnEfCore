using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using store.Models;
using store.ViewModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt
    => opt.UseNpgsql(builder.Configuration.GetConnectionString("default")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region Users endpoints

var users = app.MapGroup("users");

users.MapPost("/", async (CreateUserDto data, AppDbContext context) =>
{
    var user = new User()
    {
        Name = data.Name,
        Address = data.Address,
        CompanyId = data.CompanyId
    };

    await context.Users.AddAsync(user);
    await context.SaveChangesAsync();
    return Results.Ok(user);
});

users.MapGet("/", async (AppDbContext context) =>
{
    var users = await context.Users.Select(x => new
    {
        Id = x.Id,
        Name = x.Name,
        Address = x.Address,
        CompanyName = x.Company.Name
    }).ToListAsync();
    return Results.Ok(users);
});

#endregion

#region Companies endpoints

var companies = app.MapGroup("companies");

companies.MapPost("/", async (CreateCompanyDto data, AppDbContext context) =>
{
    var company = new Company()
    {
        Name = data.Name
    };

    await context.Companies.AddAsync(company);
    await context.SaveChangesAsync();
    return Results.Ok(company);
});

companies.MapGet("/", async (AppDbContext context) =>
{
    var companies = await context.Companies.Select(x => new
    {
        Id = x.Id,
        Name = x.Name,
        Users = x.Users.Select(y => new
        {
            Id = y.Id,
            Name = y.Name,
            Address = y.Address
        }).ToList()
    }).ToListAsync();
    return Results.Ok(companies);
});

companies.MapGet("/{id}/properties", async (int id, AppDbContext context) =>
{
    var dict = await context.CompanyProps
        .Where(x => x.CompanyId == id)
        .Select(y => y.Values)
        .Select(z => new
        {
            Key = z.Keys.FirstOrDefault(),
            Value = z.Values.FirstOrDefault()
        }).ToListAsync();

    return Results.Ok(dict);
});

companies.MapPost("/{id}/properties", async (int id, SetPropertyDto data, AppDbContext context) =>
{
    var companyProps = new List<CompanyProp>();
    foreach (var item in data.Properties)
    {
        companyProps.Add(new CompanyProp()
        {
            CompanyId = id,
            Values = new Dictionary<string, object>()
            {
                {item.Key, item.Value}
            }
        });
    }
    await context.CompanyProps.AddRangeAsync(companyProps);
    await context.SaveChangesAsync();
});

companies.MapGet("/{id}/properties/keys", async (int id, AppDbContext context) =>
{
    var keys = await context.CompanyProps
        .Where(x => x.CompanyId == id)
        .Select(y => y.Values)
        .Select(z => new
        {
            Key = z.Keys.FirstOrDefault()
        })
        .ToListAsync();
    
    return Results.Ok(keys);
});

companies.MapGet("/{id}/properties/keys/{key}", async (int id, string key,AppDbContext context) =>
{
    var dicts = await context.CompanyProps
        .Where(x => x.CompanyId == id)
        .Select(y => y.Values).ToListAsync();

    var value = dicts
        .Where(x => x.ContainsKey(key))
        .Select(y => new
        {
            Key = y.Keys.FirstOrDefault(),
            Value = y.Values.FirstOrDefault()
        }).FirstOrDefault();
    
    return Results.Ok(value);
});

#endregion
app.Run();
