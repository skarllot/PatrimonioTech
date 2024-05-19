namespace PatrimonioTech.App.SelfApplication;

public interface ILocalPathProvider
{
    string AppData { get; }

    void Initialize();
}
