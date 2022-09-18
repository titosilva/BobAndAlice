namespace BobAndAlice.App.Models.User
{
    public enum LoginResults
    {
        UserNotFound = 1,
        WrongPassword,
        Success,
    }

    public class LoginResponse
    {
        public UserModel User { get; set; }
        public LoginResults Result { get; set; }
    }
}
