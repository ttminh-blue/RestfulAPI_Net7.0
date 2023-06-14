using MagicVilla_VillaAPI;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
    option.EnableSensitiveDataLogging();
});


builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddScoped<DbContext, AppDbContext>();
builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();

builder.Services.AddScoped<IRepository<Villa>, Repository<Villa>>();
builder.Services.AddScoped<IRepository<VillaNumber>, Repository<VillaNumber>>();

builder.Services.AddResponseCaching();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddSingleton<APIResponse>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
builder.Services.AddAutoMapper(typeof(MappingConfigure));

builder.Services.AddApiVersioning(options =>
{
	options.AssumeDefaultVersionWhenUnspecified = true;
	options.DefaultApiVersion = new ApiVersion(1, 0);
	options.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(options =>
{
	options.GroupNameFormat = "'v'VVV";
	options.SubstituteApiVersionInUrl = true;
});

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret"); 
builder.Services.AddAuthentication(x =>
{
	x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
	.AddJwtBearer(x => {
		x.RequireHttpsMetadata = false;
		x.SaveToken = true;
		x.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
			ValidateIssuer = false,
			ValidateAudience = false
		};
	}); ;

builder.Services.AddControllers(option =>
{
    option.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters() ; 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description =
			"JWT Authorization header using the Bearer scheme. \r\n\r\n " +
			"Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
			"Example: \"Bearer 12345abcdef\"",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Scheme = "Bearer"
	});
	options.AddSecurityRequirement(new OpenApiSecurityRequirement()
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							},
				Scheme = "oauth2",
				Name = "Bearer",
				In = ParameterLocation.Header
			},
			new List<string>()
		}
	});
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0",
        Title = "Magic Villa",
        Description = "API to manage Villa",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Tran Tuan Minh",
            Url = new Uri("https://www.facebook.com/ttminh.blue/")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2.0",
        Title = "Magic Villa Version 2",
        Description = "API to manage Villa",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Tran Tuan Minh",
            Url = new Uri("https://www.facebook.com/ttminh.blue/")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Magic_VillaV1");
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Magic_VillaV2");

    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
