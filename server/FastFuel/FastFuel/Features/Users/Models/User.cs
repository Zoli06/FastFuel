using FastFuel.Features.Common;
using FastFuel.Features.Restaurants.Models;
using FastFuel.Features.StationCategories.Models;
using System;
using System.Collections.Generic;
using FastFuel.Features.Themes.Models;
using FastFuel.Features.Orders.Models;

namespace FastFuel.Features.Users.Models
{
    public class User : IIdentifiable
    {
        public uint Id { get; init; }       
        public string Name { get; set; } = string.Empty;
        public List<Order> Orders { get; init; } = [];
        public Theme Theme { get; init; } = new Theme(); 
    }
}