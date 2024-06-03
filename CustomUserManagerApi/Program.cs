using CustomUserManagerApi.Apllication_Layer.Services.AuthService;
using CustomUserManagerApi.Apllication_Layer.Services.Middleware;
using CustomUserManagerApi.Apllication_Layer.Services.PasswordHasher;
using CustomUserManagerApi.Apllication_Layer.Services.SignalR;
using CustomUserManagerApi.Apllication_Layer.Services.Token;
using CustomUserManagerApi.Infrastructure_Layer.DataManager;
using CustomUserManagerApi.Infrastructure_Layer.Repositories.ChatRep;
using CustomUserManagerApi.Infrastructure_Layer.Repositories.UserRep;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
namespace CustomUserManagerApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSignalR();

            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IUserRepo, UserRepo>();
            builder.Services.AddScoped<IChatRepo, ChatRepo>();
            builder.Services.AddScoped<IAuthDataService, AuthDataService>(); 
            builder.Services.AddScoped<ITokenService, TokenService>();

            builder.Services.AddDbContext<UserDbContext>(opt => opt.UseSqlServer(
                builder.Configuration.GetConnectionString("WholeApplicationDataBaseConStr")));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                TokenValidator tokValidator = new TokenValidator();
                opt.TokenValidationParameters = tokValidator.ValidationOfToken(builder.Configuration);
            });
            


            //24 6 10hod
            var app = builder.Build();

            app.MapHub<MyChatHub>("/MyChatHub");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection(); // for addroid testing

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseWhen(context =>
              !context.Request.Path.StartsWithSegments("/api/UserRegisterAndLogin/Login") &&
              !context.Request.Path.StartsWithSegments("/api/UserRegisterAndLogin/Register"),
              appBuilder =>
              {
                  appBuilder.UseMiddleware<CookieValidationMiddleware>();
           });

            app.MapControllers();

            app.Run();
        }
    }
}
