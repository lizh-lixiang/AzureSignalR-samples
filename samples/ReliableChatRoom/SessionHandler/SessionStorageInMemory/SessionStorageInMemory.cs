using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.SignalR.Samples.ReliableChatRoom
{
    public class SessionStorageInMemory : ISessionHandler
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Session>> _sessionDictionary;

        public SessionStorageInMemory()
        {
            _sessionDictionary = new ConcurrentDictionary<string, ConcurrentDictionary<string, Session>>();
        }

        public Session GetOrCreateSession(string userName, string partnerName)
        {
            if (!_sessionDictionary.ContainsKey(userName))
            {
                _sessionDictionary.TryAdd(userName, new ConcurrentDictionary<string, Session>());
            }
            if (!_sessionDictionary.ContainsKey(partnerName))
            {
                _sessionDictionary.TryAdd(partnerName, new ConcurrentDictionary<string, Session>());
            }

            _sessionDictionary.TryGetValue(userName, out var userSessions);
            Debug.Assert(userSessions != null, nameof(userSessions) + " != null");

            if (userSessions.TryGetValue(partnerName, out var session)) return session;

            session = new Session(Guid.NewGuid().ToString());
            userSessions.TryAdd(partnerName, session);
            _sessionDictionary[partnerName].TryAdd(userName, session);

            return session;
        }

        public KeyValuePair<string, Session>[] GetAllSessions(string userName)
        {
            if (!_sessionDictionary.ContainsKey(userName))
            {
                _sessionDictionary.TryAdd(userName, new ConcurrentDictionary<string, Session>());
            }

            _sessionDictionary.TryGetValue(userName, out var userSessions);
            Debug.Assert(userSessions != null, nameof(userSessions) + " != null");

            var sortedSessions = new SortedDictionary<string, Session>(userSessions);
            return sortedSessions.ToArray();
            // return new List<Session>(userSessions.Values);
        }
    }
}
