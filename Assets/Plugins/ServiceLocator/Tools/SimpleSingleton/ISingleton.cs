namespace Plugins.ServiceLocator.Tools.SimpleSingleton
{
    public interface ISingleton<T>
    {
        public static T Instance;
    }
}