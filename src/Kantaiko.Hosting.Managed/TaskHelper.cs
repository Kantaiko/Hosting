namespace Kantaiko.Hosting.Managed;

internal static class TaskHelper
{
    public static Task CreateTaskFromCancellationToken(CancellationToken cancellationToken)
    {
        var taskCompletionSource = new TaskCompletionSource();
        cancellationToken.Register(taskCompletionSource.SetCanceled);

        return taskCompletionSource.Task;
    }

    public static Task<T> CreateTaskFromCancellationToken<T>(CancellationToken cancellationToken)
    {
        var taskCompletionSource = new TaskCompletionSource<T>();
        cancellationToken.Register(taskCompletionSource.SetCanceled);

        return taskCompletionSource.Task;
    }
}
