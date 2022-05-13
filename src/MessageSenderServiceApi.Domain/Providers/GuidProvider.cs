namespace MessageSenderServiceApi.Domain.Providers;

public interface IGuidProvider
{
    Guid CreateGuid();
}

public class GuidProvider : IGuidProvider
{
    public Guid CreateGuid()
    {
        return Guid.NewGuid();
    }
}