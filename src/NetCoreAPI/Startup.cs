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
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetCoreAPI.AuthHelp;
using Senparc.CO2NET;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.RegisterServices;

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

            services.AddCors(options =>
            {
                options.AddPolicy("all", builder =>
                {
                    builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(new[] { 
                        "http://localhost:9529"
                    });
                });
            });

            //过滤器
            services.AddMvc(options =>
            {
                options.Filters.Add<ExceptionActionFilter>();
                options.Filters.Add<ManageVerifyAttribute>();
            });

            #region Config
            //注册appsettings读取类
            services.AddSingleton(new Appsettings(Configuration));
            //读取相关配置
            var vtos = Appsettings.app<Dictionary<string, string>>(new string[] { "VisitToken", "Tos" });
            Common.Config.VisitTos = vtos.FirstOrDefault();
            Common.Config.IsOpenRedis= Appsettings.app(new string[] { "Config", "IsOpenRedis" });
            Common.Config.MysqlConnectionStrng = Appsettings.app(new string[] { "Config", "MysqlConnectionStrng" });
            Common.Config.RedisConnectionString = Appsettings.app(new string[] { "Config", "RedisConnectionString" });
            #endregion


            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Version = "v1",
                    Title = "YuFaquan API",
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
                Current.ServerPath = basePath;
                var xmlPath = Path.Combine(basePath, "NetCoreAPI.xml");
                var xmlPath2 =  Path.Combine(basePath, "Entity.xml");
                //c.IncludeXmlComments(xmlPath);

                //添加控制器层注释（true表示显示控制器注释）
                c.IncludeXmlComments(xmlPath2);
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
                //添加用户usertoken
                c.AddSecurityDefinition("UserToken", new OpenApiSecurityScheme
                {
                    Description = "用户信息授权(数据将在请求头中进行传输) 参数结构: \"UserToken:{token}\"",
                    Name = "UserToken",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "UT",
                    Scheme = "UserToken"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                   {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "UserToken"
                            }
                        },
                            new string[] { }
                        }
                });

            });
            #endregion

            #region 注册缓存
            if (Common.Config.IsOpenRedis.ToLower() == "true")
            {
                services.AddSingleton<ICacheManager, RedisCacheManager>();
            }
            else
            {
                services.AddSingleton<ICacheManager, MyMemoryCache>();
            } 
            #endregion

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

            #region 添加策略鉴权模式
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
            #endregion

            services.AddAuthorization();

            //认证服务
            //services.AddSingleton<IAuthorizationHandler, PolicyHandler>();

            //主要用于获取客户端ip
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //注册权限
            AuthorizationService.LoadAuthorize();

            #region 微信模块
            services.AddSenparcGlobalServices(Configuration)//Senparc.CO2NET 全局注册
                    .AddSenparcWeixinServices(Configuration);//Senparc.Weixin 注册

            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<SenparcSetting> senparcSetting, IOptions<SenparcWeixinSetting> senparcWeixinSetting)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(next => context =>
            {
                context.Request.EnableBuffering();
                return next(context);
            });
            //app.UseMiddleware<TokenAuth>();

            app.UseHttpsRedirection();

            app.UseRouting();

            //跨域
            app.UseCors("all");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.Map("/", context =>
                {
                    context.Response.Redirect("/swagger");
                    return Task.CompletedTask;
                });
            });

            #region 微信模块
            // 启动 CO2NET 全局注册，必须！
            IRegisterService register = RegisterService.Start(senparcSetting.Value)
                                                        .UseSenparcGlobal(false, null);
            //开始注册微信信息，必须！
            register.UseSenparcWeixin(senparcWeixinSetting.Value, senparcSetting.Value);


            //全局注册appid
            AccessTokenContainer.RegisterAsync(Senparc.Weixin.Config.SenparcWeixinSetting.WeixinAppId, Senparc.Weixin.Config.SenparcWeixinSetting.WeixinAppSecret);
            JsApiTicketContainer.RegisterAsync(Senparc.Weixin.Config.SenparcWeixinSetting.WeixinAppId, Senparc.Weixin.Config.SenparcWeixinSetting.WeixinAppSecret);
            #endregion

            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHellp V1");
                c.DocumentTitle = "Api帮助文档";
                
            });
            #endregion

        }
    }
}
