using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BallNetModel
{    
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IBallService
    {
        [OperationContract]
        BallUser ClientConnect(string userName,string appAddress,string key);

        [OperationContract]
        List<BallMessage> GetNewMessages(BallUser user);

        [OperationContract]
        void SendNewMessage(BallMessage newMessage);

        [OperationContract]
        List<BallUser> GetAllUsers();

        [OperationContract]
        void RemoveUser(BallUser user);        
    }
}
