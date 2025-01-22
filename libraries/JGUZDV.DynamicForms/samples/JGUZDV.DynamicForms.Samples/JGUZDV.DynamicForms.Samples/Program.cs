using JGUZDV.Blazor.Components;
using JGUZDV.DynamicForms.Model;
using JGUZDV.DynamicForms.Samples.Client.Model;
using JGUZDV.DynamicForms.Samples.Components;
using JGUZDV.DynamicForms.Samples.DataAccess;
using JGUZDV.DynamicForms.Serialization;
using JGUZDV.L10n;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddLocalization();
builder.Services.AddSupportedCultures(["de", "en"]);

builder.Services.AddDbContext<TestDbContext>(options =>
{
    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ZDVDEV_DF_SAMPLE;Trusted_Connection=True");
});

builder.Services.AddHttpClient();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolver = new DefaultResolver();
    options.SerializerOptions.Converters.Add(new L10nStringJsonConverter());
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddScoped<ValueProvider>();
builder.Services.AddToasts();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
    //dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseRequestLocalization("de", "en");


app.MapPost("api/definitions/save", async (List<FieldDefinition> fieldDefinitions, TestDbContext context) =>
{
    var doc = new DocumentDefinition();
    doc.FieldDefinitions.AddRange(fieldDefinitions);

    context.DocumentDefinitions.Add(doc);
    await context.SaveChangesAsync();
});

app.MapGet("api/definitions", async (TestDbContext context) =>
{
    return await context.DocumentDefinitions.ToListAsync();
});

app.MapPost("api/documents/save", async (List<Field> fields, TestDbContext context) =>
{
    var document = new Document();
    document.Fields.AddRange(fields);
    Console.WriteLine(document);

    context.Documents.Add(document);
    await context.SaveChangesAsync();
});

app.MapGet("api/documents", async (TestDbContext context) =>
{
    return await context.Documents.ToListAsync();
});

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(JGUZDV.DynamicForms.Samples.Client._Imports).Assembly);

app.Run();
