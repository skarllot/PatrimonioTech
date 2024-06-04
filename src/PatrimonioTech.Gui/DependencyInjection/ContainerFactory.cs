using Microsoft.Extensions.DependencyInjection;

namespace PatrimonioTech.Gui.DependencyInjection;

public sealed class ContainerFactory<T> : IFactory<T>
{
    private readonly IServiceProvider _serviceProvider;

    public ContainerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        if (serviceProvider is IServiceProviderIsService isService && !isService.IsService(typeof(T)))
            throw new InvalidOperationException($"The type '{typeof(T)}' is not registered");
    }

    public T Create() =>
        (T?)_serviceProvider.GetService(typeof(T)) ??
        throw new InvalidOperationException($"A instance of type '{typeof(T)}' could not be created");
}
