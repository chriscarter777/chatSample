using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections;
using System.Threading.Tasks;

namespace Bewander
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        private static Dictionary<string, string> userCnxnLookupTable = new Dictionary<string, string>();

        // method for sending a message to all clients
        public void sendMessage(string senderName, string message)
        {
            Clients.All.displayMessage(senderName, message);
        }

        // method for sending a message to one specific client
        public void sendMessage(string senderName, string message, string targetName)
        {
            if (userCnxnLookupTable.ContainsKey(targetName) == true)
            {
                //look up the target's ConnectionID 
                string targetConxnId = userCnxnLookupTable[targetName].ToString();
                //then send the message to the target, and echo it back to the sender
                Clients.Client(targetConxnId).displayMessage(senderName, message);
                Clients.Caller.displayMessage(senderName, message);
            }
            else
            {
                string error = "DELIVERY FAILED";
                message = targetName + " is not connected.";
                Clients.Caller.displayMessage(error, message);
            }
        }

        //upon connecting, add new userID and ConnectionID to the lookup table
        //so senderName and targetName can later be matched with their ConnectionIDs
        public void registerConxnId(string userName)
        {
            bool alreadyExists = false;
            if (userCnxnLookupTable.Count == 0)
            {
                //if no users previously connected, add this caller as the first user
                userCnxnLookupTable.Add(userName, Context.ConnectionId);
                //start the service to periodically broadcast list of current users
                sendListOfConnected();
            }
            else
            {
                foreach (string key in userCnxnLookupTable.Keys)
                {
                    if (key == userName)
                    {
                        userCnxnLookupTable[key] = Context.ConnectionId;
                        alreadyExists = true;
                        break;
                    }
                }
                if (!alreadyExists)
                {
                    userCnxnLookupTable.Add(userName, Context.ConnectionId);
                }
            }
        }

        //broadcast a list of current users to all clients; update every 10 seconds
        public void sendListOfConnected()
        {
            while (true)
            {
                //Build a list of current user names from the userCnxnLookupTable
                List<string> connectedUsers = new List<string>();
                foreach (string userName in userCnxnLookupTable.Keys)
                {
                    connectedUsers.Add(userName);
                }
                //send the list to clients
                Clients.All.displayListOfConnected(connectedUsers);
                //wait 10 seconds then repeat
                int wait = 10 * 1000;
                System.Threading.Thread.Sleep(wait);
            }
        }

        //upon detecting a disconnection, remove the disconnected user from the lookup table
        public override Task OnDisconnected(bool stopCalled)
        {
            foreach (KeyValuePair<string, string> user in userCnxnLookupTable)
            {
                if (user.Value == Context.ConnectionId)
                {
                    userCnxnLookupTable.Remove(user.Key);
                }
            }
            return base.OnDisconnected(stopCalled);
        }
    }
}