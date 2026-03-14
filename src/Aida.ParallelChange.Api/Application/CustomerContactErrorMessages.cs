namespace Aida.ParallelChange.Api.Application;

public static class CustomerContactErrorMessages
{
    public static string BuildAlreadyExistsDetail(int customerId)
    {
        return $"Customer contact '{customerId}' already exists.";
    }

    public static string BuildNotFoundDetail(int customerId)
    {
        return $"Customer contact '{customerId}' was not found.";
    }
}
