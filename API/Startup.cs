using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APIMINIMADIO;
using APIMINIMADIO.Dominio.Entidades;
using APIMINIMADIO.Dominio.Interfaces;
using APIMINIMADIO.Dominio.ModelViews;
using APIMINIMADIO.Dominio.Services;
using APIMINIMADIO.Dominio.DTO;
using APIMINIMADIO.Infra.DB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using APIMINIMADIO.Dominio.Enuns;



public class Startup
{
    public Startup(IConfiguration configuration)
    {
        
        Configuration = configuration;
        key = Configuration?.GetSection("Jwt")?.ToString() ?? "";
    }

    private string key = "";

    public IConfiguration Configuration { get; set;} = default!;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option => 
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });

        services.AddAuthorization();

        services.AddScoped<IAdministradorServices, AdministradorServicos>();
        services.AddScoped<IVeciulosServices, VeiculoServico>();


        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options => 
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT desta maneira:{Seu token}"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {   
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",

                        }
                        
                        
                    },
                    new string[]  {}
                }
            });
        });


        services.AddDbContext<DBcontexto>(options => 
        {
            options.UseMySql
            (
                Configuration.GetConnectionString("mysql"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("mysql"))
            );
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();
        

        

        app.UseEndpoints( endpoints =>
        {
            #region Home
            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
            #endregion

            #region Login
            string GerarTokenJwt(Administrador administrador)
            {
                if(string.IsNullOrEmpty(key)) return string.Empty;
                
                var securytiKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securytiKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim("Email", administrador.Email),
                    new Claim("Perfil", administrador.Perfil),
                    new Claim(ClaimTypes.Role, administrador.Perfil)
                };

                var token = new JwtSecurityToken
                (
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
                
            }


            endpoints.MapPost("/Administradores/login", ([FromBody]LoginDTO loginDTO, IAdministradorServices administradorServicos) =>
            {
                var adm = administradorServicos.Login(loginDTO);
                if(adm != null)
                {
                    string token = GerarTokenJwt(adm);
                    return Results.Ok(new AdmLogado
                    {
                        Email = adm.Email,
                        Perfil = adm.Perfil,
                        Token = token
                    });
                }
                else
                    return Results.Unauthorized();
            }).AllowAnonymous().WithTags("Administradores");

            endpoints.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServices administradorServicos) =>
            {
                var adms = new List<AdmView>();
                var administradores = administradorServicos.Todos(pagina);

                foreach(var adm in administradores)
                {
                    adms.Add(new AdmView 
                    {
                        Id = adm.Id,
                        Email = adm.Email,
                        Perfil = adm.Perfil
                    });
                }
                return Results.Ok(adms);
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm"}).WithTags("Administradores");

            endpoints.MapGet("/Administradores/{Id}", ([FromRoute]int Id, IAdministradorServices administradorServicos) =>
            {
                var adm = administradorServicos.BuscaPorId(Id);

                if(adm == null) return Results.NotFound();

                return Results.Ok(new AdmView 
                    {
                        Id = adm.Id,
                        Email = adm.Email,
                        Perfil = adm.Perfil
                    });
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm"}).WithTags("Administradores");

            endpoints.MapPost("/Administradores", ([FromBody]AdministradoresDTO administradoresDTO, IAdministradorServices administradorServicos) =>
            {   
                var validacao = new ErrosValidacao
                {
                    Mensagens = new List<string>()
                };

                if(string.IsNullOrEmpty(administradoresDTO.Email))
                {
                    validacao.Mensagens.Add("E-mail não pode ser vazio");
                }
                if(string.IsNullOrEmpty(administradoresDTO.Senha))
                {
                    validacao.Mensagens.Add("Senha não pode ser vazia");
                }
                if(administradoresDTO.Perfil == null)
                {
                    validacao.Mensagens.Add("Perfil não pode ser vazio");
                }

                if(validacao.Mensagens.Count > 0)
                {
                    return Results.BadRequest(validacao);
                }
                
                var adm = new Administrador
                {
                    Email = administradoresDTO.Email,
                    Senha = administradoresDTO.Senha,
                    Perfil = administradoresDTO.Perfil.ToString() ?? Perfil.Editor.ToString()

                };

                administradorServicos.Incluir(adm);

                return Results.Created($"/Administradores/{adm.Id}", new AdmView 
                    {
                        Id = adm.Id,
                        Email = adm.Email,
                        Perfil = adm.Perfil
                    });

            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm"}).WithTags("Administradores");

            #endregion

            #region Veiculo

            ErrosValidacao validaDto(VeiculoDTO veiculoDTO)
            {
                var validacao = new ErrosValidacao
                {
                    Mensagens = new List<string>()
                };

                if(string.IsNullOrEmpty(veiculoDTO.Nome))
                {
                    validacao.Mensagens.Add("O nome não pode ser vazio");
                }
                if(string.IsNullOrEmpty(veiculoDTO.Marca))
                {
                    validacao.Mensagens.Add("A marca não ficar em branco");
                } 
                if(veiculoDTO.Ano < 1889)
                {
                    validacao.Mensagens.Add("Veiculo de data inexistente");
                }
                return validacao;
            }


            endpoints.MapPost("/veiculos", ([FromBody]VeiculoDTO veiculoDTO, IVeciulosServices veciulosServices) =>
            {
                var validacao = validaDto(veiculoDTO);
                if(validacao.Mensagens.Count > 0)
                {
                    return Results.BadRequest(validacao);
                }

                var veiculo = new Veiculo
                {
                    Nome = veiculoDTO.Nome,
                    Marca = veiculoDTO.Marca,
                    Ano = veiculoDTO.Ano,

                };

                veciulosServices.Incluir(veiculo);

                return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm,Editor"}).WithTags("Veiculos");

            endpoints.MapGet("/veiculos", ([FromQuery]int? pagina, IVeciulosServices veciulosServices) =>
            {
                var veiculo = veciulosServices.Todos(pagina);

                return Results.Ok(veiculo);
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm,Editor"}).WithTags("Veiculos");

            endpoints.MapGet("/veiculos/{Id}", ([FromRoute]int Id, IVeciulosServices veciulosServices) =>
            {
                var veiculo = veciulosServices.BuscaPorId(Id);

                if(veiculo == null) return Results.NotFound();

                return Results.Ok(veiculo);
            }).RequireAuthorization().WithTags("Veiculos");

            endpoints.MapPut("/veiculos/{Id}", ([FromRoute]int Id, VeiculoDTO veiculoDTO, IVeciulosServices veciulosServices) =>
            {
                var veiculo = veciulosServices.BuscaPorId(Id);

                if(veiculo == null) return Results.NotFound();
                
                var validacao = validaDto(veiculoDTO);
                if(validacao.Mensagens.Count > 0) return Results.BadRequest(validacao);

                veiculo.Nome = veiculoDTO.Nome;
                veiculo.Marca = veiculoDTO.Marca;
                veiculo.Ano = veiculoDTO.Ano;

                veciulosServices.Atualizar(veiculo);

                return Results.Ok(veiculo);
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm"}).WithTags("Veiculos");

            endpoints.MapDelete("/veiculos/{Id}", ([FromRoute]int Id, IVeciulosServices veciulosServices) =>
            {
                var veiculo = veciulosServices.BuscaPorId(Id);

                if(veiculo == null) return Results.NotFound();

                veciulosServices.Apagar(veiculo);

                return Results.NoContent();
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm"}).WithTags("Veiculos");

            #endregion
        });
    }
}