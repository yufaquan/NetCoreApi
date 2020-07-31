using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Authorization;
using Common;
using Common.Cache;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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
            services.AddControllers();

            //过滤器
            services.AddMvc(options =>
            {
                options.Filters.Add<ExceptionActionFilter>();
                options.Filters.Add<ManageVerifyAttribute>();
            });

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
                //c.IncludeXmlComments(xmlPath);

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

            //注册缓存
            services.AddSingleton<ICacheManager, RedisCacheManager>();

            #region 身份验证
            //身份验证
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            ////添加jwt验证：
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateLifetime = true,//是否验证失效时间
            //        ClockSkew = TimeSpan.FromMinutes(30),
            //        ValidateActor = false,
            //        ValidateTokenReplay = false,
            //        IgnoreTrailingSlashWhenValidatingAudience = false,


            //        ValidateAudience = false,//是否验证Audience
            //        //ValidAudience = Const.GetValidudience(),//Audience
            //        //这里采用动态验证的方式，在重新登陆时，刷新token，旧token就强制失效了
            //        //AudienceValidator = (m, n, z) =>
            //        //{
            //        //    return m != null && m.FirstOrDefault().Equals(Const.ValidAudience);
            //        //},
            //        ValidateIssuer = true,//是否验证Issuer
            //        ValidIssuer = Config.JWTInfo.Issuer,//Issuer，这两项和前面签发jwt的设置一致

            //        ValidateIssuerSigningKey = false,//是否验证SecurityKey
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Config.JWTInfo.SigningKey))//拿到SecurityKey
            //    };
            //    // ------------------------自定义分割线-------------------------

            //    options.SecurityTokenValidators.Clear();//清除默认的设置
            //    options.SecurityTokenValidators.Add(new MyTokenValidata(new RedisCacheManager()));//添加自己设定规则的验证方法
            //    options.Events = new JwtBearerEvents
            //    {
            //        //如果在请求处理期间引发异常，则调用。除非被抑制，否则将在此事件后重新引发异常
            //        OnAuthenticationFailed = context =>
            //        {
            //            //Token expired
            //            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            //            {
            //                context.Response.Headers.Add("Token-Expired", "true");
            //            }
            //            return Task.CompletedTask;
            //        }
            //        //此处为权限验证失败后触发的事件
            //        ,
            //        OnChallenge = context =>
            //       {
            //           //此处代码为终止.Net Core默认的返回类型和数据结果，这个很重要哦，必须
            //           context.HandleResponse();
            //           //自定义返回的数据类型
            //           context.Response.AuthFailed();
            //           return Task.FromResult(0);
            //       }
            //       //,OnForbidden = context =>
            //       //{
            //       //    return Task.CompletedTask;
            //       //}
            //       //,
            //       //OnMessageReceived = context =>
            //       //{
            //       //    return Task.CompletedTask;
            //       //}
            //       //,
            //       //OnTokenValidated = context =>
            //       //{
            //       //    return Task.CompletedTask;
            //       //}

            //    };
            //});

            #endregion

            //添加策略鉴权模式
            //只允许Default访问
            //services.AddAuthorization(o =>
            //{
            //    o.AddPolicy("Default", policy => policy.RequireClaim("DefaultType").Build());
            //});
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("Permission", policy => policy.Requirements.Add(new PolicyRequirement()));
            //})
            //授权
            services.AddAuthorization();

            //认证服务
            //services.AddSingleton<IAuthorizationHandler, PolicyHandler>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseMiddleware<TokenAuth>();

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
