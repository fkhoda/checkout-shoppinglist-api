namespace ShoppingListService.Infrastructure.Actor.Monitoring
{
    using System;

    using Proto;

    public static class Monitoring
    {
        public static Func<Receive, Receive> ForReceiveMiddlewareUsing(IMonitoringProvider provider)
        {
            return next => async context =>
                {
                    await provider.IndexReceiveAsync(context);
                    await next(context);
                };
        }

        public static Func<Sender, Sender> ForSenderMiddlewareUsing(IMonitoringProvider provider)
        {
            return next => async (context, target, envelope) =>
                {
                    await provider.IndexSendAsync(context);
                    await next(context, target, envelope);
                };
        }
    }
}
