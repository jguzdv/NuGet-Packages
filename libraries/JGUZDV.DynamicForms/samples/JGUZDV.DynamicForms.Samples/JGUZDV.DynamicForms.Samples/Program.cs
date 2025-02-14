using System.ComponentModel.DataAnnotations;

using JGUZDV.Blazor.Components;
using JGUZDV.DynamicForms;
using JGUZDV.DynamicForms.Model;
using JGUZDV.DynamicForms.Samples.Client.Model;
using JGUZDV.DynamicForms.Samples.Components;
using JGUZDV.DynamicForms.Samples.DataAccess;
using JGUZDV.DynamicForms.Serialization;
using JGUZDV.L10n;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddScoped<ValueProvider>();
builder.Services.AddToasts();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
    dbContext.Database.EnsureDeleted();
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

app.MapPost("api/documents/save", async (HttpRequest request, TestDbContext context) =>
{
    var form = await request.ReadFormAsync();

    async Task<(
            Dictionary<string, List<FileFieldType.FileType>> Files,
            List<(string FieldIdentifier, string Json)> Json
        )>
        FieldsFromRequest(IFormCollection form)
    {
        Dictionary<string, List<FileFieldType.FileType>> files = new();
        foreach (var group in form.Files
            .Where(x => x.Name.StartsWith(DynamicFormsConfiguration.FormFieldPrefix))
            .GroupBy(x => x.Name))
        {
            files[group.First().Name] = new List<FileFieldType.FileType>();
            foreach (var file in group)
            {
                var identifier = file.Name;
                var stream = file.OpenReadStream();
                var filename = file.FileName;
                var size = stream.Length;

                files[identifier].Add(new FileFieldType.FileType
                {
                    FileName = filename,
                    FileSize = size,
                    Stream = stream
                });
            }
        }

        List<(string identifier, string value)> jsons = new();
        foreach (var formField in form
            .Where(x => x.Key.StartsWith(DynamicFormsConfiguration.FormFieldPrefix)))
        {
            if (form.Files.Any(x => x.Name == formField.Key))
            {
                continue;
            }

            jsons.Add((formField.Key, formField.Value.ToString()));
        }

        return (files, jsons);
    }

    var (files, fields) = await FieldsFromRequest(form);

    var docDefId = form["DocumentDefinitionId"].First();
    var documentDefinition = await context.DocumentDefinitions
        .FirstOrDefaultAsync(x => x.Id.ToString() == docDefId);

    if (documentDefinition == null)
    {
        throw new InvalidOperationException("Document definition not found.");
    }

    var document = new Document();
    foreach (var formField in fields)
    {
        var def = documentDefinition.FieldDefinitions
            .FirstOrDefault(x => x.Identifier == formField.FieldIdentifier);

        if (def == null)
        {
            throw new InvalidOperationException($"Field definition not found for identifier {formField.FieldIdentifier}.");
        }

        var field = new Field(def);
        field.Value = def.Type!.ConvertToValue(formField.Json);

        var validationResults = field.Validate(new ValidationContext(field));
        if (validationResults.Any())
        {
            throw new ValidationException($"Validation failed for field {formField.FieldIdentifier}: {string.Join(", ", validationResults.Select(r => r.ErrorMessage))}");
        }

        document.Fields.Add(field);
    }

    foreach (var fileGroup in files)
    {
        var def = documentDefinition.FieldDefinitions
            .FirstOrDefault(x => x.Identifier == fileGroup.Key);

        if (def == null)
        {
            throw new InvalidOperationException($"Field definition not found for identifier {fileGroup.Key}.");
        }

        var field = new Field(def);
        field.Value = def.Type!.ConvertFromValue(fileGroup.Value);

        var validationResults = field.Validate(new ValidationContext(field));
        if (validationResults.Any())
        {
            throw new ValidationException($"Validation failed for field {fileGroup.Key}: {string.Join(", ", validationResults.Select(r => r.ErrorMessage))}");
        }

        document.Fields.Add(field);
    }

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
