﻿namespace FastFuel.Models;

public class Restaurant
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public required string Address { get; set; }
    public string? Phone { get; set; }
}