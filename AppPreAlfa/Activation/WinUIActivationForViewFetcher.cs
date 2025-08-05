using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.UI.Xaml;
using ReactiveUI;

namespace AppPreAlfa.Activation;

public class WinUIActivationForViewFetcher : IActivationForViewFetcher
{
    public int GetAffinityForView(Type view)
    {
        return typeof(FrameworkElement).IsAssignableFrom(view) ? 10 : 0;
    }

    public IObservable<bool> GetActivationForView(IActivatableView view)
    {
        if (view is not FrameworkElement fe)
            return Observable.Return(false);

        var viewLoaded = Observable.FromEvent<RoutedEventHandler, bool>(
            handler => (s, e) => handler(true),
            handler => fe.Loaded += handler,
            handler => fe.Loaded -= handler);

        var viewUnloaded = Observable.FromEvent<RoutedEventHandler, bool>(
            handler => (s, e) => handler(false),
            handler => fe.Unloaded += handler,
            handler => fe.Unloaded -= handler);

        return viewLoaded
            .Merge(viewUnloaded)
            .Select(b => b)
            .StartWith(fe.IsLoaded)
            .DistinctUntilChanged();
    }
}
