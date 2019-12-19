using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Identity
{
    public class EmailConfirmationTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public EmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider, 
                                              IOptions<EmailConfirmationTokenProviderOptions> options, 
                                              ILogger<DataProtectorTokenProvider<TUser>> logger) 
            : base(dataProtectionProvider, options, logger)
        {
        }
    }
}
