namespace PatrimonioTech.Gui.Desktop.DependencyInjection;

public interface IFactory<out T>
{
    T Create();
}

internal sealed class ContainerFactory<T>(IServiceProvider serviceProvider) : IFactory<T>
{
    public T Create() =>
        (T?)serviceProvider.GetService(typeof(T)) ??
        throw new InvalidOperationException($"A instance of type '{typeof(T)}' could not be created");
}
