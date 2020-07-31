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

            //������
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
                //��Ӷ�ȡע�ͷ���
                dynamic type = new Program().GetType();
                string basePath = Path.GetDirectoryName(type.Assembly.Location);
                var xmlPath = Path.Combine(basePath, "NetCoreAPI.xml");
                //c.IncludeXmlComments(xmlPath);

                //��ӿ�������ע�ͣ�true��ʾ��ʾ������ע�ͣ�
                c.IncludeXmlComments(xmlPath, true);

                //���header��֤��Ϣ
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) �����ṹ: \"Authorization: Bearer {token}\"",
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

            //ע�Ỻ��
            services.AddSingleton<ICacheManager, RedisCacheManager>();

            #region �����֤
            //�����֤
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            ////���jwt��֤��
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
            //        ClockSkew = TimeSpan.FromMinutes(30),
            //        ValidateActor = false,
            //        ValidateTokenReplay = false,
            //        IgnoreTrailingSlashWhenValidatingAudience = false,


            //        ValidateAudience = false,//�Ƿ���֤Audience
            //        //ValidAudience = Const.GetValidudience(),//Audience
            //        //������ö�̬��֤�ķ�ʽ�������µ�½ʱ��ˢ��token����token��ǿ��ʧЧ��
            //        //AudienceValidator = (m, n, z) =>
            //        //{
            //        //    return m != null && m.FirstOrDefault().Equals(Const.ValidAudience);
            //        //},
            //        ValidateIssuer = true,//�Ƿ���֤Issuer
            //        ValidIssuer = Config.JWTInfo.Issuer,//Issuer���������ǰ��ǩ��jwt������һ��

            //        ValidateIssuerSigningKey = false,//�Ƿ���֤SecurityKey
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Config.JWTInfo.SigningKey))//�õ�SecurityKey
            //    };
            //    // ------------------------�Զ���ָ���-------------------------

            //    options.SecurityTokenValidators.Clear();//���Ĭ�ϵ�����
            //    options.SecurityTokenValidators.Add(new MyTokenValidata(new RedisCacheManager()));//����Լ��趨�������֤����
            //    options.Events = new JwtBearerEvents
            //    {
            //        //������������ڼ������쳣������á����Ǳ����ƣ������ڴ��¼������������쳣
            //        OnAuthenticationFailed = context =>
            //        {
            //            //Token expired
            //            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            //            {
            //                context.Response.Headers.Add("Token-Expired", "true");
            //            }
            //            return Task.CompletedTask;
            //        }
            //        //�˴�ΪȨ����֤ʧ�ܺ󴥷����¼�
            //        ,
            //        OnChallenge = context =>
            //       {
            //           //�˴�����Ϊ��ֹ.Net CoreĬ�ϵķ������ͺ����ݽ�����������ҪŶ������
            //           context.HandleResponse();
            //           //�Զ��巵�ص���������
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

            //��Ӳ��Լ�Ȩģʽ
            //ֻ����Default����
            //services.AddAuthorization(o =>
            //{
            //    o.AddPolicy("Default", policy => policy.RequireClaim("DefaultType").Build());
            //});
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("Permission", policy => policy.Requirements.Add(new PolicyRequirement()));
            //})
            //��Ȩ
            services.AddAuthorization();

            //��֤����
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
