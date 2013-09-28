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
                where e.UserName == user.UserName
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
                incomingMessages.Add(user.UserName, new List<BallMessage>() { 
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
            Console.WriteLine(newMessage.User.UserName+" says :"+newMessage.Message+" at "+newMessage.Date);
            
            foreach (var user in this.ConectedUsers)
            {
                if (!newMessage.User.UserName.Equals(user.UserName) && newMessage.User.Key.Equals(user.Key))   //
                {
                    incomingMessages[user.UserName].Add(newMessage);                    
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<BallMessage> GetNewMessages(BallUser user)
        {
            List<BallMessage> myNewMessages = incomingMessages[user.UserName];
            incomingMessages[user.UserName] = new List<BallMessage>();

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
            this.ConectedUsers.RemoveAll(u => u.UserName == user.UserName);
        }
    }
}
