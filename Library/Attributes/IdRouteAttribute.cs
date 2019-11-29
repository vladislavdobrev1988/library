using Microsoft.AspNetCore.Mvc;

namespace Library.Attributes
{
    public class IdRouteAttribute : RouteAttribute
    {
        public IdRouteAttribute() : base("{id}") { }
    }
}
