using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text;
using WebApplicationAPI.Models;
using Microsoft.IdentityModel.Tokens;
using WebApplicationAPI.JwtServices;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// （暫時沒用到）避免Client端寫JS時，遇見問題「跨原始來源要求 (CORS)  / Access - Control - Allow - Origin」
string MyAllowSpecificOriginsCORS = "_myAllowSpecificOrigins";

#region  // （暫時沒用到） 避免Client端寫JS時，遇見問題「跨原始來源要求 (CORS)  / Access - Control - Allow - Origin」
// https://docs.microsoft.com/zh-tw/aspnet/core/security/cors?view=aspnetcore-5.0
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOriginsCORS,
                      builder =>
                      {
                          builder.WithOrigins("*");   // 全部開放
                                                                         //builder.WithOrigins("http://example.com", "http://www.contoso.com");  // 限定某些網站才能存取
                      });
});
#endregion

builder.Services.AddControllers();
//#region              // （第一個範例）  JWT (json web token) 才會用到這一段
//********************************************************************   
// 這裡需要新增 很多的命名空間，請使用「顯示可能的修正」讓系統自己加上。
var key = Encoding.UTF8.GetBytes(Settings.Secret);  // 位於 /JwtServices目錄下的Settings類別（搭配 System.Text命名空間）

builder.Services.AddAuthentication(z =>
{
    z.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    z.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;

    // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
    x.IncludeErrorDetails = true; // 預設值為 true

    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,

        // 底下三個設定屬性也可以寫在 appsettings.json檔案。https://www.cnblogs.com/nsky/p/10312101.html
        IssuerSigningKey = new SymmetricSecurityKey(key),
        // 或是寫成 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("您自己輸入的Secret Hash數值"))
        // (1) 用來雜湊 (Hash) 打亂的關鍵數值

        ValidateIssuer = false,        // (2) 是誰核發的？  (false 不驗證)
        ValidateAudience = false  // (3) 哪些客戶（Client）可以使用？  (false 不驗證)

        // === TokenValidationParameters的參數 (預設值) ====
        // https://docs.microsoft.com/zh-tw/dotnet/api/microsoft.identitymodel.tokens.tokenvalidationparameters?view=azure-dotnet
        // RequireSignedTokens = true,
        // SaveSigninToken = false,
        // ValidateActor = false,

        // 將下面兩個參數設置為false，就不會驗證 Issuer和 Audience，但是不建議這樣做。
        // ValidateAudience = true,     // 是誰核發的？ 
        // ValidateIssuer = true,            // 哪些客戶（Client）可以使用？ 
        // ValidateIssuerSigningKey = false,  // 如果token 中包含 key 才需要驗證，一般都只有簽章而已

        // 是否要求Token的Claims中必須包含Expires（過期時間）
        // RequireExpirationTime = true,

        // 允許的伺服器時間偏移量
        // ClockSkew = TimeSpan.FromSeconds(300),

        // 是否驗證 token有效期間，使用當前時間與 token的 Claims中的NotBefore和Expires對比
        // ValidateLifetime = true
    };
});

builder.Services.AddDbContext<ToDoListContext>(
        options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// JWT (json web token) 才會用到這一段。必須放在 app.UseAuthorization();之前，順序不能錯！
app.UseAuthentication();      // ** JWT 請自己動手加上這一段 **
//***************************************************************
app.UseAuthorization();       // 順序不能錯！

app.MapControllers();

app.Run();
