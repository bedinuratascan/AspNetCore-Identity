using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace membershipSystem.CustomValidation
{
    public class CustomIdentityErrorDescriber:IdentityErrorDescriber
    {
        public override IdentityError InvalidUserName(string userName)
        {
            return new IdentityError
            {
                Code = "InvalidUserName",
                Description = $"Bu kullanıcı adı ({userName}) geçersizdir."
            };
        }
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = "DuplicateUserName",
                Description = $"Bu kullanıcı adı ({userName}) zaten kullanılmaktadır."
            };
        }
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = "DuplicateEmail",
                Description = $"Bu email adresi ({email}) zaten kullanılmaktadır."
            };
        }
        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = "PasswordTooShort",
                Description = $"Şifreniz en az {length} karakter olmalıdır."
            };
        }
    }
}
