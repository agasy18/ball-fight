using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BallNetModel
{
    public class BallEngine
    {
        private List<BallUser> conectedUsers = new List<BallUser>();
        private Dictionary<string,List<BallMessage>> incomingMessages= new Dictionary<string,List<BallMessage>>();      
        public List<BallUser> ConectedUsers
        {
            get { return conectedUsers; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public BallUser AddNewBallUser(BallUser user)
        {           
            var exists =
                from BallUser e in this.ConectedUsers
                where e.AppAddress == user.AppAddress
                select e;

            if (exists.Count() == 0)
            {
                var partner =
                    from BallUser e in this.ConectedUsers
                    where e.Key == user.Key
                    select e;

                if (partner.Count() > 2)        //this is reserved key, please use another key
                    return null;
                else if (partner.Count() == 1)  //partner has been found
                { }
                else                            //first user with that key
                { }

                this.ConectedUsers.Add(user);
                incomingMessages.Add(user.AppAddress, new List<BallMessage>() { 
                    new BallMessage(){User=user, Message="Welcome to WPF simple Ball", Date=DateTime.Now}
                });

                Console.WriteLine("New user connected: " + user);
                return user;
            }
            else
                return null;           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newMessage"></param>
        public void AddNewMessage(BallMessage newMessage)
        {
            
            try
            {
                incomingMessages[ConectedUsers.First(user => newMessage.User.AppAddress != user.AppAddress && newMessage.User.Key == user.Key).AppAddress].Add(newMessage);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<BallMessage> GetNewMessages(BallUser user)
        {
            List<BallMessage> myNewMessages = incomingMessages[user.AppAddress];
            incomingMessages[user.AppAddress] = new List<BallMessage>();

            if (myNewMessages.Count > 0)
                return myNewMessages;
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public void RemoveUser(BallUser user)
        {
            this.ConectedUsers.RemoveAll(u => u.AppAddress == user.AppAddress);
        }
    }
}
