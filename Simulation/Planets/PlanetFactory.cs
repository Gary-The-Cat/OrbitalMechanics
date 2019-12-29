using CarSimulation;
using SFML.System;
using System;

namespace Arkanoid_SFML.Planets
{
    public static class PlanetFactory
    {
        public static Planet GetPlanet()
        {
            var random = new Random();
            var planetType = PlanetType.Terrestrial;
            var radius = PlanetConfigurations.GetPlanetRadius(planetType);
            var mass = PlanetConfigurations.GetPlanetMass(planetType);
            var position = GetOnScreenPosition(radius);
            var texture = TextureFactory.GetPlanetTexture(planetType, radius);

            return new Planet(texture, position, radius, mass);
        }

        public static Vector2f GetOnScreenPosition(float radius)
        {
            var random = new Random();

            var xRange = (int)(Configuration.Width - (radius * 2));
            var yRange = (int)(Configuration.Height - (radius * 2));

            var x = radius + (int)(random.NextDouble() * xRange);
            var y = radius + (int)(random.NextDouble() * yRange);

            return new Vector2f(x, y);
        }
    }
}
