using project.Application.Interfaces;
using project.Application.ModelsDto;
using project.Application.ModelsDto.DomainModelsDto;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;
using project.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace project.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IEmailService _emailService;
        private readonly IUserTokenService _userTokenService;
        private readonly IUserRepository _userRepository;

        public AuthService(IEmailService emailService, IUserTokenService userTokenService, IUserRepository userRepository)
        {
            _emailService = emailService;
            _userTokenService = userTokenService;
            _userRepository = userRepository;
        }

        //public async Task<Result> ForgotPassword(ForgotPasswordModel model)
        //{
        //    try
        //    {
        //        var user = await _userRepository.FindByEmailAsync(model.Email);
        //        if (user == null || model.ClientUri == null)
        //        {
        //            return Result.Failure(new Error("","User not found"));
        //        }
        //        var token = await _userTokenService.GeneratePasswordTokenAsync(user);
        //        var encodedToken = WebUtility.UrlEncode(token);
        //        var param = new Dictionary<string, string> { { "token", encodedToken }, { "email", model.Email } };
        //        var resetLink = $"{model.ClientUri}/auth/reset-password" + $"?email={model.Email}&token={encodedToken}";
        //        var msgBody = $@"Xin chào {user.UserName} đây là link reset mật khẩu {resetLink}";
        //        var msg = new Message([user.Email!], "Reset password", msgBody);
        //        await _emailService.SendEmail(msg);
        //        return Result.Success();
        //    }
        //    catch(Exception ex)
        //    {
        //        return Result.Failure(new Error("", ex.Message));
        //    }
        //}

        //public async Task<Result<UserModel> Login(LoginModel model)
        //{
        //    try
        //    {
        //        var user = await _userRepository.FindByEmailAsync(model.Email);
        //        if (user == null)
        //        {
        //            return Result.Failure<UserModel>(new Error("", "User không tồn tại"));
        //        }
        //        var result = await _userRepository.(user, model.Password);
        //        if (!result)
        //        {
        //            return Result.Failure(new Error("", "Mật khẩu không chính xác"));
        //        }
        //        return Result.Success();
        //    }
        //    catch (Exception ex)
        //    {
        //        return Result.Failure(new Error("", ex.Message));
        //    }
        //}

        //public async Task<Result<UserModel>> Register(RegisterModel model)
        //{
        //    if (await EmailExists(model.Email))
        //    {
        //        return Result.Failure<UserModel>(new Error("", "Email đã tồn tại"));
        //    }
        //    var user = new User
        //    {
        //        Email = model.Email,
        //        UserName = model.UserName,
        //    };
        //    var result = await _userManager.CreateUserAsync(user, model.Password);
        //    if(result == null)
        //    {
        //        return Result.Failure<UserModel>(new Error("", "Đăng ký thất bại"));
        //    }
        //    var userApp = new UserApp(user.Id,user.UserName,user.Email,null,)

        //}

        //private async Task<bool> EmailExists(string email)
        //{
        //    var user = await _userValidation.FindEmailAsync(email);
        //    return user != null;
        //}

        //public Task<Result> ResetPassword(ResetPasswordModel model)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
