using System.Collections.Generic;

namespace Microsoft.Azure.SignalR.Samples.ReliableChatRoom
{
    public interface IMessageHandler
    {
        /// <summary>
        /// Add a new messageStatus to the storage.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="message"></param>
        /// <returns>The sequenceId of the new messageStatus.</returns>
        int AddNewMessage(string sessionId, Message message);

        /// <summary>
        /// Update an existed messageStatus in the storage.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="sequenceId"></param>
        /// <param name="messageStatus"></param>
        void UpdateMessage(string sessionId, int sequenceId, string messageStatus);

        /// <summary>
        /// Selects the messages from startSequenceId to endSequenceId (both are included).
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="startSequenceId"></param>
        /// <param name="endSequenceId"></param>
        /// <returns>A list of messages sorted by the sequenceId</returns>
        List<Message> LoadHistoryMessage(string sessionId, int startSequenceId, int endSequenceId);
    }
}
