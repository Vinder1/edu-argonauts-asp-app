using Argonauts.Web.SignalR;

namespace Argonauts.Web.Endpoints
{
    /// <summary>
    /// 
    /// </summary>
    public static class SignalREndpoints
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public static void MapGameHub(this IEndpointRouteBuilder app)
        {
            app.MapHub<GameHub>("/events");
        }
    }
}