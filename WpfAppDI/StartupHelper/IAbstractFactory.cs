namespace WpfAppDI.StartupHelper;

public interface IAbstractFactory<T>
{
    T Create();
}