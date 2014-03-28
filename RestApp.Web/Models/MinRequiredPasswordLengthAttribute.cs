using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Security;

namespace RestApp.Models
{
    public class MinRequiredPasswordLengthAttribute : StringLengthAttribute
    {
        public MinRequiredPasswordLengthAttribute()
            : base(maximumLength: Int32.MaxValue)
        {
            MinimumLength = Membership.MinRequiredPasswordLength;
        }
    }
}