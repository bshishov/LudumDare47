using System;

namespace Utils
{
    public static class EventBus
    {
        private static readonly TinyMessengerHub Hub = new TinyMessengerHub();
        
        public static TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction) where TMessage : class, ITinyMessage
        {
            return Hub.Subscribe(deliveryAction);
        }

        public static TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, ITinyMessageProxy proxy) where TMessage : class, ITinyMessage
        {
            return Hub.Subscribe(deliveryAction, proxy);
        }

        public static TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, bool useStrongReferences) where TMessage : class, ITinyMessage
        {
            return Hub.Subscribe(deliveryAction, useStrongReferences);
        }

        public static TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, bool useStrongReferences,
            ITinyMessageProxy proxy) where TMessage : class, ITinyMessage
        {
            return Hub.Subscribe(deliveryAction, useStrongReferences, proxy);
        }

        public static TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter) where TMessage : class, ITinyMessage
        {
            return Hub.Subscribe(deliveryAction, messageFilter);
        }

        public static TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter, ITinyMessageProxy proxy) where TMessage : class, ITinyMessage
        {
            return Hub.Subscribe(deliveryAction, messageFilter, proxy);
        }

        public static TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter, bool useStrongReferences) where TMessage : class, ITinyMessage
        {
            return Hub.Subscribe(deliveryAction, messageFilter, useStrongReferences);
        }

        public static TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter, bool useStrongReferences,
            ITinyMessageProxy proxy) where TMessage : class, ITinyMessage
        {
            return Hub.Subscribe(deliveryAction, messageFilter, useStrongReferences, proxy);
        }

        public static void Unsubscribe<TMessage>(TinyMessageSubscriptionToken subscriptionToken) where TMessage : class, ITinyMessage
        {
            Hub.Unsubscribe(subscriptionToken);
        }

        public static void Unsubscribe(TinyMessageSubscriptionToken subscriptionToken)
        {
            Hub.Unsubscribe(subscriptionToken);
        }

        public static void Publish<TMessage>(TMessage message) where TMessage : class, ITinyMessage
        {
            Hub.Publish(message);
        }

        public static void PublishAsync<TMessage>(TMessage message) where TMessage : class, ITinyMessage
        {
            Hub.PublishAsync(message);
        }

        public static void PublishAsync<TMessage>(TMessage message, AsyncCallback callback) where TMessage : class, ITinyMessage
        {
            Hub.PublishAsync(message, callback);
        }
    }
}