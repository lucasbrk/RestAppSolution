using System.Collections.Generic;

namespace RestApp.Services.Events
{
    public interface ISubscriptionService
    {
        IList<IConsumer<T>> GetSubscriptions<T>();
    }
}
