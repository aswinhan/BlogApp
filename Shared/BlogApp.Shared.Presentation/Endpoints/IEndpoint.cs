namespace BlogApp.Shared.Presentation.Endpoints;

public interface IApiEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}