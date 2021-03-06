﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataProviders.Concrete;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Infrastructure.Interfaces;
using Azimuth.Shared.Dto;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace Azimuth.Tests
{
    [TestFixture]
    class VkAccountProviderTests
    {

        // Fields for tests
        private string _userToJson;

        private const string AccessToken = "some user id";
        private const string UserId = "some access token";
        private const string Email = "besedadg@gmail.com";

        public string UserInfoUrl { get; set; }

        private IWebClient _webRequest;

        private VkUserData.VkResponse _vkUserData;

        [SetUp]
        public void SetUp()
        {
            // Query to VK api
            UserInfoUrl = String.Format(
                @"https://api.vk.com/method/users.get?user_id={0}&fields=screen_name,bdate,sex,city,country,photo_max_orig,timezone&v=5.23&access_token={1}",
                UserId,
                AccessToken);

            // Object that we will make Json
            _vkUserData = new VkUserData.VkResponse
            {
                Response = new List<VkUserData>
                {
                    new VkUserData
                    {
                        FirstName = "Beseda",
                        LastName = "Dmitrij",
                        ScreenName = "Beseda Dmitrij",
                        Sex = VkUserData.VkSex.Male,
                        Birthday = "12/1/1992",
                        City = new VkUserData.VkCity {Id = 1, Title = "Donetsk"},
                        Country = new VkUserData.VkCountry {Id = 1, Title = "Ukraine"},
                        Timezone = 2,
                        Photo = "photo.jpg",
                        Email = Email
                    }
                }
            };

            // Make Json
            _userToJson = JsonConvert.SerializeObject(_vkUserData);

            _webRequest = Substitute.For<IWebClient>();
            _webRequest.GetWebData(UserInfoUrl).Returns(Task.FromResult(_userToJson));
        }

        [Test]
        public async void Get_Vk_User_Data()
        {
            // Arrange
            var expectedUser = Mapper.Map(_vkUserData, new User());
            // Act
            var provider = new VkAccountProvider(_webRequest, new UserCredential
            {
                SocialNetworkId = UserId,
                AccessToken = AccessToken
            });
            var user = await provider.GetUserInfoAsync(Email);
            // Assert
            user.ToString().Should().Be(expectedUser.ToString());
        }
    }
}
