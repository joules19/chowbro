using System;
using System.Security.Cryptography;

namespace Chowbro.Infrastructure.Helpers
{
    public static class OtpHelper
    {
        public static string GenerateOtp(int length = 6)
        {
            if (length < 4 || length > 10)
                throw new ArgumentOutOfRangeException(nameof(length), "OTP length must be between 4 and 10 characters.");

            const string digits = "0123456789";
            char[] otp = new char[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] data = new byte[length];
                rng.GetBytes(data);

                for (int i = 0; i < length; i++)
                {
                    otp[i] = digits[data[i] % digits.Length];
                }
            }

            return new string(otp);
        }
    }
}
