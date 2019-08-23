// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.Azure.SignalR.Samples.ReliableChatRoom
{
    public class ReliableChatRoom : Hub
    {
        private readonly IMessageHandler _messageHandler;

        private readonly ISessionHandler _sessionHandler;

        public ReliableChatRoom(IMessageHandler messageHandler, ISessionHandler sessionHandler)
        {
            _messageHandler = messageHandler;
            _sessionHandler = sessionHandler;
        }

        public override Task OnConnectedAsync()
        {
            var sender = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            //  Push the latest session information to the user.
            //  Currently push the whole list. Better pushing the timestamp to make incremental updates. 
            var userSessions = _sessionHandler.GetAllSessions(sender);
            Clients.Caller.SendAsync("updateSessions",userSessions);

            //  TODO: Push UnreadMessages to the Client (Better way is the client pull the messages later.)

            return base.OnConnectedAsync();
        }

        /// <summary>
        /// Broadcast a message to all clients. Won't store the message.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        public void BroadcastMessage(string name, string message)
        {
            Clients.All.SendAsync("broadcastMessage", name, message);
        }

        /// <summary>
        /// Create a new session with a specified user.
        /// </summary>
        /// <param name="receiver"></param>
        /// <returns>The sessionId.</returns>
        public string CreateNewSession(string receiver)
        {
            var sender = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = _sessionHandler.GetOrCreateSession(sender, receiver);

            return session.SessionId;
        }

        /// <summary>
        /// Send a message to the specified user.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="receiver"></param>
        /// <param name="messageContent"></param>
        /// <returns>The sequenceId of the message.</returns>
        public int SendUserMessage(string sessionId, string receiver, string messageContent)
        {
            var sender = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var message = new Message(sender, DateTime.Now, messageContent, "Sent");
            var sequenceId = _messageHandler.AddNewMessage(sessionId, message);

            Clients.User(receiver).SendAsync("displayUserMessage", sessionId, sequenceId, sender, messageContent);

            return sequenceId;
        }

        /// <summary>
        /// Send an ack to the message owner.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="sequenceId"></param>
        /// <param name="receiver"></param>
        /// <param name="messageStatus"></param>
        /// <returns>The status of the message.</returns>
        public string SendUserAck(string sessionId, int sequenceId, string receiver, string messageStatus)
        {
            _messageHandler.UpdateMessage(sessionId, sequenceId, messageStatus);

            Clients.User(receiver).SendAsync("displayAckMessage", sessionId, sequenceId, messageStatus);

            return messageStatus;
        }

        /// <summary>
        /// Load the unread/history messages of one session.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public List<Message> LoadMessages(string sessionId)
        {
            return _messageHandler.LoadHistoryMessage(sessionId, -1, -1);
        }
    }
}
