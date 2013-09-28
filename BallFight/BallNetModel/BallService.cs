using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BallNetModel
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class BallService : IBallService
    {
        private BallEngine mainEngine = new BallEngine();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public BallUser ClientConnect(string userName, string appAddress, string key)
        {
            System.Console.WriteLine("ClientConnect({0},{1})", userName, key);
            return mainEngine.AddNewBallUser(new BallUser() { UserName = userName, Key = key, AppAddress = appAddress });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public List<BallMessage> GetNewMessages(BallUser user)
        {
            return mainEngine.GetNewMessages(user);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newMessage"></param>
        public void SendNewMessage(BallMessage newMessage)
        {
            mainEngine.AddNewMessage(newMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<BallUser> GetAllUsers()
        {
            return mainEngine.ConectedUsers;
        }

        public void RemoveUser(BallUser user)
        {
            mainEngine.RemoveUser(user);
            System.Console.WriteLine(user.AppAddress + " " + user.UserName + " Loged out");
        }


    }
}
