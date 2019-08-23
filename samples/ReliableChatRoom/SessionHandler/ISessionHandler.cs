using System.Collections.Generic;

namespace Microsoft.Azure.SignalR.Samples.ReliableChatRoom
{
    public interface ISessionHandler
    {
        /// <summary>
        /// Creates a new session or loads the existed session.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="partnerName"></param>
        /// <returns>The session instance</returns>
        Session GetOrCreateSession(string userName, string partnerName);

        /// <summary>
        /// Gets all related sessions of one user.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>A list of sessions</returns>
        KeyValuePair<string, Session>[] GetAllSessions(string userName);
    }
}
