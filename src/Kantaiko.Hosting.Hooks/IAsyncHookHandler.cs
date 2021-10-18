﻿namespace Kantaiko.Hosting.Hooks;

public interface IAsyncHookHandler<in THook> where THook : IAsyncHook
{
    Task HandleAsync(THook payload, CancellationToken cancellationToken);
}