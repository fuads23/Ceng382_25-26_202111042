using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Helpers; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession();

builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

DataSeeder.SeedInitialData(app.Configuration);


app.UseAuthorization();
app.UseSession();


app.Use(async (context, next) =>
{
    if (context.Request.Path == "/" && context.Session.GetString("username") == null)
    {
        context.Response.Redirect("/Login");
        return;
    }
    await next();
});

app.MapRazorPages();
app.Run();