using Reservation.Core.EFContext;
using Reservation.Core.Repositories.Base;
using Reservation.Core.Repositories.Interface;
using Reservation.Core.Uow;
using Reservation.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using Reservation.Core.Factory;
using Reservation.Services;

namespace Reservation_App.Extensions
{
	internal static class RegisterExtensions
	{
		internal static void AddDbContexts(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
		{
			var contextConnectionString = configuration.GetConnectionString("DefaultConnection");
			services.AddDbContextPool<DatabaseContext>(x => x.UseSqlServer(contextConnectionString, o =>
			{
				o.EnableRetryOnFailure(3);
			})
				.EnableSensitiveDataLogging(environment.IsDevelopment())
				.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
		}

		internal static void AddInjections(this IServiceCollection services)
		{
			services.AddScoped<IDatabaseContext, DatabaseContext>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
			//services.AddTransient(typeof(IPlayerServices), typeof(PlayersServices));
			//services.AddTransient(typeof(IPositionsServices), typeof(PositionsServices));
			services.AddScoped<IContextFactory, ContextFactory>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();
		}

		internal static void AddIdentity(this IServiceCollection services)
		{

			services.AddIdentity<ApplicationUser, IdentityRole>()
			.AddRoleManager<RoleManager<IdentityRole>>()
			.AddDefaultTokenProviders()
			.AddEntityFrameworkStores<DatabaseContext>();

			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<DatabaseContext>()
				.AddDefaultTokenProviders();

			services.Configure<IdentityOptions>(options =>
			{
				// Password settings
				options.Password.RequireDigit = false;
				options.Password.RequiredLength = 1;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireLowercase = false;
				options.Password.RequiredUniqueChars = 1;

				// Lockout settings
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.AllowedForNewUsers = true;

				// User settings
				options.User.RequireUniqueEmail = false;
			});
		}
	}

}
