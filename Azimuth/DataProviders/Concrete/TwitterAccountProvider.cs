﻿using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure;

namespace Azimuth.DataProviders.Concrete
{
    public class TwitterAccountProvider: AccountProvider
    {
        private readonly string _accessToken;
        private readonly string _tokenExpiresIn;
        private readonly string _userId;
        private readonly string _accessTokenSecret;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;

        public TwitterAccountProvider(IWebClient webClient, UserCredential userCredential)
            :base(webClient)
        {
            this._accessToken = userCredential.AccessToken;
            this._userId = userCredential.SocialNetworkId;
            this._accessTokenSecret = userCredential.AccessTokenSecret;
            this._consumerKey = userCredential.ConsumerKey;
            this._consumerSecret = userCredential.ConsumerSecret;
        }
        public override async Task<User> GetUserInfoAsync(string email = "")
        {
            var user = await _webClient.GetWebData(_consumerKey, _consumerSecret, _accessToken, _accessTokenSecret);

            return (User) user;
        }
    }
}