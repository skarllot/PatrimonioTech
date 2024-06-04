namespace PatrimonioTech.Gui.DependencyInjection;

public interface IFactory<out T>
{
    T Create();
}
