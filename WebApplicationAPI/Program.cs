using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text;
using WebApplicationAPI.Models;
using Microsoft.IdentityModel.Tokens;
using WebApplicationAPI.JwtServices;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// �]�ȮɨS�Ψ�^�קKClient�ݼgJS�ɡA�J�����D�u���l�ӷ��n�D (CORS)  / Access - Control - Allow - Origin�v
string MyAllowSpecificOriginsCORS = "_myAllowSpecificOrigins";

#region  // �]�ȮɨS�Ψ�^ �קKClient�ݼgJS�ɡA�J�����D�u���l�ӷ��n�D (CORS)  / Access - Control - Allow - Origin�v
// https://docs.microsoft.com/zh-tw/aspnet/core/security/cors?view=aspnetcore-5.0
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOriginsCORS,
                      builder =>
                      {
                          builder.WithOrigins("*");   // �����}��
                                                                         //builder.WithOrigins("http://example.com", "http://www.contoso.com");  // ���w�Y�Ǻ����~��s��
                      });
});
#endregion

builder.Services.AddControllers();
//#region              // �]�Ĥ@�ӽd�ҡ^  JWT (json web token) �~�|�Ψ�o�@�q
//********************************************************************   
// �o�̻ݭn�s�W �ܦh���R�W�Ŷ��A�ШϥΡu��ܥi�઺�ץ��v���t�Φۤv�[�W�C
var key = Encoding.UTF8.GetBytes(Settings.Secret);  // ��� /JwtServices�ؿ��U��Settings���O�]�f�t System.Text�R�W�Ŷ��^

builder.Services.AddAuthentication(z =>
{
    z.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    z.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;

    // �����ҥ��ѮɡA�^�����Y�|�]�t WWW-Authenticate ���Y�A�o�̷|��ܥ��Ѫ��Բӿ��~��]
    x.IncludeErrorDetails = true; // �w�]�Ȭ� true

    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,

        // ���U�T�ӳ]�w�ݩʤ]�i�H�g�b appsettings.json�ɮסChttps://www.cnblogs.com/nsky/p/10312101.html
        IssuerSigningKey = new SymmetricSecurityKey(key),
        // �άO�g�� IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("�z�ۤv��J��Secret Hash�ƭ�"))
        // (1) �Ψ����� (Hash) ���ê�����ƭ�

        ValidateIssuer = false,        // (2) �O�ֵ֮o���H  (false ������)
        ValidateAudience = false  // (3) ���ǫȤ�]Client�^�i�H�ϥΡH  (false ������)

        // === TokenValidationParameters���Ѽ� (�w�]��) ====
        // https://docs.microsoft.com/zh-tw/dotnet/api/microsoft.identitymodel.tokens.tokenvalidationparameters?view=azure-dotnet
        // RequireSignedTokens = true,
        // SaveSigninToken = false,
        // ValidateActor = false,

        // �N�U����ӰѼƳ]�m��false�A�N���|���� Issuer�M Audience�A���O����ĳ�o�˰��C
        // ValidateAudience = true,     // �O�ֵ֮o���H 
        // ValidateIssuer = true,            // ���ǫȤ�]Client�^�i�H�ϥΡH 
        // ValidateIssuerSigningKey = false,  // �p�Gtoken ���]�t key �~�ݭn���ҡA�@�볣�u��ñ���Ӥw

        // �O�_�n�DToken��Claims�������]�tExpires�]�L���ɶ��^
        // RequireExpirationTime = true,

        // ���\�����A���ɶ������q
        // ClockSkew = TimeSpan.FromSeconds(300),

        // �O�_���� token���Ĵ����A�ϥη�e�ɶ��P token�� Claims����NotBefore�MExpires���
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

// JWT (json web token) �~�|�Ψ�o�@�q�C������b app.UseAuthorization();���e�A���Ǥ�����I
app.UseAuthentication();      // ** JWT �Цۤv�ʤ�[�W�o�@�q **
//***************************************************************
app.UseAuthorization();       // ���Ǥ�����I

app.MapControllers();

app.Run();
