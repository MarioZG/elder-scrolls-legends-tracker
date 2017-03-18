using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Services
{
    /// <summary>
    /// Improved from answer of Dalstroem in stackOverflow!
    /// The question: http://stackoverflow.com/questions/23798425/wpf-mvvm-communication-between-view-model
    /// Dalstroem profile: http://stackoverflow.com/users/3683189/dalstroem
    /// My profile: http://stackoverflow.com/users/4871837/yeah69
    /// Improvement: I made the the MessageKey sensitive to the message type. Thus, several different message types with no context object can be registered to the same receiver object.
    /// </summary>
    public class Messenger : IMessenger
    {
        private static readonly object CreationLock = new object();

        private static readonly ConcurrentDictionary<MessengerKey, object> Dictionary =
            new ConcurrentDictionary<MessengerKey, object>();

        #region Default property

        private static IMessenger _instance;

        /// <summary>
        /// Gets the single instance of the Messenger.
        /// </summary>
        public static IMessenger Default
        {
            get
            {
                if (_instance == null)
                {
                    lock (CreationLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Messenger();
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the Messenger class.
        /// </summary>
        private Messenger() { }

        /// <summary>
        /// Registers a recipient for a type of message T. The action parameter will be executed
        /// when a corresponding message is sent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="recipient"></param>
        /// <param name="action"></param>
        public void Register<T>(object recipient, Action<T> action)
        {
            Register(recipient, action, null);
        }

        /// <summary>
        /// Registers a recipient for a type of message T and a matching context. The action parameter will be executed
        /// when a corresponding message is sent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="recipient"></param>
        /// <param name="action"></param>
        /// <param name="context"></param>
        public void Register<T>(object recipient, Action<T> action, object context)
        {
            var key = new MessengerKey(recipient, context, typeof(T));
            Dictionary.TryAdd(key, action);
        }

        /// <summary>
        /// Unregisters a messenger recipient completely. After this method is executed, the recipient will
        /// no longer receive any messages.
        /// </summary>
        /// <param name="recipient"></param>
        public void Unregister<T>(object recipient)
        {
            Unregister<T>(recipient, null);
        }

        /// <summary>
        /// Unregisters a messenger recipient with a matching context completely. After this method is executed, the recipient will
        /// no longer receive any messages.
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="context"></param>
        public void Unregister<T>(object recipient, object context)
        {
            object action;
            var key = new MessengerKey(recipient, context, typeof(T));
            Dictionary.TryRemove(key, out action);
        }

        /// <summary>
        /// Sends a message to registered recipients. The message will reach all recipients that are
        /// registered for this message type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public void Send<T>(T message)
        {
            Send(message, null);
        }

        /// <summary>
        /// Sends a message to registered recipients. The message will reach all recipients that are
        /// registered for this message type and matching context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="context"></param>
        public void Send<T>(T message, object context)
        {
            IEnumerable<KeyValuePair<MessengerKey, object>> result;

            if (context == null)
            {
                // Get all recipients where the context is null.
                result = from r in Dictionary where r.Key.Context == null select r;
            }
            else
            {
                // Get all recipients where the context is matching.
                result = from r in Dictionary where r.Key.Context != null && r.Key.Context.Equals(context) select r;
            }

            foreach (var action in result.Select(x => x.Value).OfType<Action<T>>())
            {
                // Send the message to all recipients.
                action(message);
            }
        }

        protected class MessengerKey : IEquatable<MessengerKey>
        {
            public object Recipient { get; }
            public object Context { get; }
            public Type Type { get; }

            /// <summary>
            /// Initializes a new instance of the MessengerKey class.
            /// </summary>
            /// <param name="recipient"></param>
            /// <param name="context"></param>
            /// <param name="type"></param>
            public MessengerKey(object recipient, object context, Type type)
            {
                Recipient = recipient;
                Context = context;
                Type = type;
            }

            #region Equality members

            /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
            /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
            /// <param name="other">An object to compare with this object.</param>
            public bool Equals(MessengerKey other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;

                return Equals(Recipient, other.Recipient) && Equals(Context, other.Context) && Equals(Type, other.Type);
            }

            /// <summary>Determines whether the specified object is equal to the current object.</summary>
            /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
            /// <param name="obj">The object to compare with the current object. </param>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;

                return Equals((MessengerKey)obj);
            }

            /// <summary>Serves as the default hash function. </summary>
            /// <returns>A hash code for the current object.</returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = Recipient?.GetHashCode() ?? 0;
                    hashCode = (hashCode * 397) ^ (Context?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 397) ^ (Type?.GetHashCode() ?? 0);
                    return hashCode;
                }
            }

            public static bool operator ==(MessengerKey left, MessengerKey right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(MessengerKey left, MessengerKey right)
            {
                return !Equals(left, right);
            }

            #endregion
        }
    }
}
