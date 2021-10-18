namespace Kantaiko.Hosting.Hooks;

public interface IHookHandler<in THook> where THook : IAsyncHook
{
    void Handle(THook payload);
}