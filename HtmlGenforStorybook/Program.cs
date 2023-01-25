using Bogus;
using HtmlGenforStorybook.Models;
using Razor.Templating.Core;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorTemplating();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

if (app.Environment.IsDevelopment())
{
    var htmlGenerator = new HtmlGenerator(app.Services.GetRequiredService<IRazorTemplateEngine>());
    htmlGenerator.SaveForStorybook("~/Views/Shared/Header.cshtml", new HeaderFaker());
    htmlGenerator.SaveForStorybook("~/Views/Shared/Footer.cshtml", new FooterFaker());
}

app.Run();


public class HtmlGenerator
{
    private readonly IRazorTemplateEngine _templateEngine;

    public HtmlGenerator(IRazorTemplateEngine templateEngine)
    {
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
    }

    public async Task SaveForStorybook<T>(string viewName, Faker<T> faker, Dictionary<string, object> viewData = null) where T : class
    {
        var shortViewName = viewName.Split("/").Last().Split(".").First();
        var jsonDirectoryInfo = $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\..\\app\\output\\json\\";
        var jsonFileName = $"{jsonDirectoryInfo}{shortViewName}.json";
        var htmlDirectoryInfo = $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\..\\app\\storybook\\src\\stories\\razor\\";
        var reactFileName = $"{htmlDirectoryInfo}{shortViewName}.jsx";
        var storyFileName = $"{htmlDirectoryInfo}{shortViewName}.stories.jsx";

        T model;

        if (File.Exists(jsonFileName))
        {
            var jsonString = await File.ReadAllTextAsync(jsonFileName);
            model = JsonSerializer.Deserialize<T>(jsonString) ?? throw new InvalidOperationException();
        }
        else
        {
            model = faker.Generate();
            var jsonString = JsonSerializer.Serialize(model);
            Directory.CreateDirectory(jsonDirectoryInfo);
            await File.WriteAllTextAsync(jsonFileName, jsonString);
        }

        var htmlString = await _templateEngine.RenderAsync(viewName, model, viewData);
        Directory.CreateDirectory(htmlDirectoryInfo);

        var componentString = "import React from 'react';\r\n" +
                              "import './site.css';\r\n\r\n" +
                              $"export const {shortViewName} = () => {{\r\n" +
                              $"  return (\r\n" +
                              $"    <>{htmlString}</>\r\n" +
                              "  );\r\n};";

        var storyString = "import React from 'react';\r\n" +
                          $"import {{{ shortViewName }}} from './{shortViewName}';\r\n" +
                          "export default {\r\n" +
                          $"  title: 'Razor/{shortViewName}',\r\n" +
                          $"  component: {shortViewName}\r\n}};\r\n" +
                          $"export const Default = {shortViewName}.bind();";

        await File.WriteAllTextAsync(reactFileName, componentString);
        await File.WriteAllTextAsync(storyFileName, storyString);
    }
}