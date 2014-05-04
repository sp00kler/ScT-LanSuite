using System;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using ChatJs.Net;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.AspNet.SignalR;
using UnityFramework;

namespace ScT_LanSuite
{
    /// <summary>
    /// Stub methods for obtaining the db user from the cookie.
    /// In a normal situation this would be done using the forms authentication cookie
    /// </summary>
    public class ChatHelper
    {
        public static string ROOM_ID_STUB = "sct-room";
        public static UnitOfWork uow = new UnitOfWork();


        /// <summary>
        /// Returns information about the user from cookie
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<ChatUser> GetChatUserAsync(HttpRequestBase request, string userName)
        {
            if (request.IsAuthenticated && !string.IsNullOrEmpty(userName))
        	{
                var user = await uow.userRepository.FindAsync(x => x.UserName == userName);
                var chatUser = new ChatUser();
                chatUser.Email = user.Email;
                chatUser.Name = user.UserName;
                chatUser.ProfilePictureUrl = GravatarHelper.GetGravatarUrl(GravatarHelper.GetGravatarHash(user.Email), GravatarHelper.Size.s32);
                chatUser.Id = user.Id;
                chatUser.RoomId = ROOM_ID_STUB;
                return chatUser;
        	}
            return null;
        }

    }
}