using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NetCoreAPI.AuthHelp;

namespace NetCoreAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Version = "v1.1.0",
                    Title = "YuFaquan WebAPI",
                    Description = "desc",
                    TermsOfService = new Uri("https://yufaquan.cn"),
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact() { 
                        Email="13235601859@163.com",
                        Name="YuFaquan",
                        Url=new Uri("https://yufaquan.cn")
                    }
                });
                //添加读取注释服务
                dynamic type = new Program().GetType();
                string basePath = Path.GetDirectoryName(type.Assembly.Location);
                var xmlPath = Path.Combine(basePath, "NetCoreAPI.xml");
                c.IncludeXmlComments(xmlPath);

                //添加控制器层注释（true表示显示控制器注释）
                c.IncludeXmlComments(xmlPath, true);

                //添加header验证信息
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                   {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                            }
                        },
                            new string[] { }
                        }
                });

            });
            #endregion

            //注册Redis
            services.AddSingleton<IRedisCacheManager, RedisCacheManager>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("System", policy => policy.RequireClaim("SystemType").Build());
                options.AddPolicy("Client", policy => policy.RequireClaim("ClientType").Build());
                options.AddPolicy("Admin", policy => policy.RequireClaim("AdminType").Build());
            });


            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<TokenAuth>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHellp V1");
            });
            #endregion
        }
    }
}
