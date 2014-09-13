﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Azimuth.DataAccess.Entities;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Services.Concrete;
using Azimuth.Services.Interfaces;

namespace Azimuth.Controllers
{
    public class PublicPlaylistsController : Controller
    {
        private readonly IPlaylistLikesService _playlistLikesService;
        private readonly IUserService _userService;
        private readonly IPlaylistService _playlistService;
        private readonly IUserTracksService _userTracksService;
        public PublicPlaylistsController(IPlaylistLikesService playlistLikesService, IUserService userService, IPlaylistService playlistService, IUserTracksService userTracksService)
        {
            _playlistLikesService = playlistLikesService;
            _userService = userService;
            _playlistService = playlistService;
            _userTracksService = userTracksService;
        }

        //
        // GET: /PublicPlaylists/
        public async Task<ActionResult> Index(long? id)
        {
            if (AzimuthIdentity.Current == null)
            {
                return View();
            }
            //var user = id == null ? _userService.GetUserInfo(AzimuthIdentity.Current.UserCredential.Id) : _userService.GetUserInfo((long)id);
            var user = _userService.GetUserInfo(AzimuthIdentity.Current.UserCredential.Id);
            ViewBag.Id = id;
            return View(user);
        }

        [HttpPost]
        public PartialViewResult _ChangeLikeStatus(string playlistId, string isLiked, string isFavourited, string buttonType)
        {
            var playlist = Convert.ToInt32(playlistId);
            var like = Convert.ToBoolean(isLiked);
            var favourite = Convert.ToBoolean(isFavourited);
            if (buttonType == "like")
            {
                if (like)
                {
                    _playlistLikesService.RemoveCurrentUserAsLiker(playlist);
                }
                else
                {
                    _playlistLikesService.AddCurrentUserAsLiker(playlist);
                }

                ViewBag.isLiked = !like;
                ViewBag.isFavourited = favourite;
            }
            else
            {
                if (favourite)
                {
                    _playlistLikesService.RemoveCurrentUserAsFavorite(playlist);
                }
                else
                {
                    _playlistLikesService.AddCurrentUserAsFavorite(playlist);
                }

                ViewBag.isLiked = like;
                ViewBag.isFavourited = !favourite;
            }
            ViewBag.playlistId = playlistId;

            return PartialView();
        }

        public ActionResult NeedLogIn()
        {
            return RedirectToAction("Login", "Account");
        }

        public PartialViewResult _PublicPlaylists(ICollection<PlaylistLike> playlistFollowing, long? id)
        {
            var publicPlaylists = _playlistService.GetPublicPlaylistsSync(id);

            ViewData["playlistFollowing"] = playlistFollowing;

            return PartialView(publicPlaylists);
        }

        public PartialViewResult _PlaylistTracks(string playlistId, string playlistName, string isLiked, string isFavourited)
        {
            if (!String.IsNullOrEmpty(playlistId) || !String.IsNullOrWhiteSpace(playlistId))
            {
                var tracks = _userTracksService.GetTracksByPlaylistIdSync(Convert.ToInt32(playlistId));
                ViewBag.playlistId = playlistId;
                ViewBag.playlistName = playlistName;
                if (isLiked != "undefined")
                {
                    ViewBag.isLiked = Convert.ToBoolean(isLiked);
                }
                if (isFavourited != "undefined")
                {
                    ViewBag.isFavourited = Convert.ToBoolean(isFavourited);
                }
                return PartialView(tracks);
            }
            return null;
        }
	}
}