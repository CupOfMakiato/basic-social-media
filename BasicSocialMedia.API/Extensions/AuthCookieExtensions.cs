using BasicSocialMedia.Application.DTOs.Auth;

namespace BasicSocialMedia.API.Extensions
{
    public static class AuthCookieExtensions
    {
        public static void AppendJwtTokenCookies(
            this HttpResponse response,
            JwtTokenResult tokens,
            bool secure)
        {
            response.Cookies.Append(
                tokens.AccessTokenCookieName,
                tokens.AccessToken,
                CreateCookieOptions(tokens.AccessTokenExpiresAt, secure));

            response.Cookies.Append(
                tokens.RefreshTokenCookieName,
                tokens.RefreshToken,
                CreateCookieOptions(tokens.RefreshTokenExpiresAt, secure));
        }

        public static void DeleteJwtTokenCookies(
            this HttpResponse response,
            string accessTokenCookieName,
            string refreshTokenCookieName)
        {
            var options = new CookieOptions { Path = "/" };
            response.Cookies.Delete(accessTokenCookieName, options);
            response.Cookies.Delete(refreshTokenCookieName, options);
        }

        private static CookieOptions CreateCookieOptions(DateTime expiresAt, bool secure)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = secure,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = expiresAt
            };
        }
    }
}
