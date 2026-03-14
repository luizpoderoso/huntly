using FastEndpoints;

namespace Huntly.Api.Endpoints.Jobs;

public sealed class JobsGroup : Group
{
    public JobsGroup()
    {
        Configure("jobs", ep => { });
    }
}