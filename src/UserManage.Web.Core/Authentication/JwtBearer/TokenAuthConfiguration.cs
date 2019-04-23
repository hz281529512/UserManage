﻿using System;
using Microsoft.IdentityModel.Tokens;

namespace UserManage.Authentication.JwtBearer
{
    public class TokenAuthConfiguration
    {
        public SymmetricSecurityKey SecurityKey { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public SigningCredentials SigningCredentials { get; set; }

        public TimeSpan Expiration { get; set; }

        #region ids4

        /// <summary>
        /// ids4 鉴权地址
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// ids4客户端id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// ids4客户端Secret
        /// </summary>
        public string Secret { get; set; }

        #endregion
    }
}
