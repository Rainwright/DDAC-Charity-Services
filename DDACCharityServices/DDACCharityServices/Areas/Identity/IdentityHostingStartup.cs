using System;
using DDACCharityServices.Areas.Identity.Data;
using DDACCharityServices.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(DDACCharityServices.Areas.Identity.IdentityHostingStartup))]
namespace DDACCharityServices.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<DDACCharityServicesContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("DDACCharityServicesContextConnection")));

                services.AddDefaultIdentity<DDACCharityServicesUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<DDACCharityServicesContext>();
            });
        }
    }
}