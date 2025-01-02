using Yail.Shared;

namespace Yail.Common;

public static class ExceptionHelper
{
    public static void ThrowIfNull(this ValueObj? value)
    {
        if (value == null)
        {
            throw new ArgumentNullException("Value was null.");
        }

    }
}