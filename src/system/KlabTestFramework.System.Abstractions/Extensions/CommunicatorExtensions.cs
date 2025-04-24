using System;
using System.Threading;
using System.Threading.Tasks;
using Klab.Toolkit.Results;
using KlabTestFramework.System.Abstractions.TypeInterfaces;

namespace KlabTestFramework.System.Abstractions.Extensions;

public static class CommunicatorExtensions
{
    public static async Task<Result<byte[]>> WriteReadByReadLength(this ICommunicator communicator, byte[] writeData, int responseLength, CancellationToken cancellationToken = default)
    {
        try
        {
            await communicator.Lock.LockAsync();
            return await communicator
                .WriteAsync(writeData, cancellationToken)
                .BindAsync(() => communicator.ReadAsync(responseLength, cancellationToken));

        }
        catch (Exception ex)
        {
            Error err = Error.FromException("", ErrorType.Error, ex);
            return Result.Failure<byte[]>(err);
        }
        finally
        {
            communicator.Lock.Release();
        }
    }

    public static async Task<Result<byte[]>> WriteReadByDelay(this ICommunicator communicator, byte[] writeData, int responseLength, TimeSpan delay, CancellationToken cancellationToken = default)
    {
        try
        {
            await communicator.Lock.LockAsync();
            return await communicator
                .WriteAsync(writeData, cancellationToken)
                .OnSuccessAsync(() => Task.Delay(delay, cancellationToken))
                .BindAsync(() => communicator.ReadAsync(responseLength, cancellationToken));
        }
        catch (Exception ex)
        {
            Error err = Error.FromException("", ErrorType.Error, ex);
            return Result.Failure<byte[]>(err);
        }
        finally
        {
            communicator.Lock.Release();
        }
    }
}
