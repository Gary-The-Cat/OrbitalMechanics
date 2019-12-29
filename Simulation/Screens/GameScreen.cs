using Arkanoid_SFML.Planets;
using Arkanoid_SFML.Projectiles;
using CarSimulation;
using CarSimulation.ExtensionMethods;
using CarSimulation.Screens;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Arkanoid_SFML.Screens
{
    public class GameScreen : Screen
    {
        internal RenderTexture texture;

        internal int frame;

        readonly Vector2f TitleBarSize = new Vector2f(12, 57);

        List<Drawable> entities;

        public RectangleShape background;

        private bool isThrowing;

        private Vector2f? throwAnchor;

        private Vertex[] elastic;

        private Projectile projectile;

        private Goal goal;

        private List<Planet> planets;

        private List<Vector2f> projections;

        private const int planetCount = 3;

        public GameScreen(
            RenderWindow window,
            FloatRect configuration)
            : base(window, configuration)
        {
            frame = 0;

            window.KeyPressed += KeyPressed;
            window.MouseButtonPressed += MousePressed;
            window.MouseButtonReleased += MouseReleased;

            texture = new RenderTexture(Configuration.Width, Configuration.Height);
            background = new RectangleShape(new Vector2f(Configuration.Width, Configuration.Height));
            background.Texture = new Texture(new Image("Nightscape.png"));


            entities = new List<Drawable>();
            projectile = new Projectile(new CircleShape(30) { Origin = new Vector2f(30, 30) }, null);
            elastic = new Vertex[2];
            projections = new List<Vector2f>();

            PopulatePlanets();

            SetNewGoal();
            SetNewStart();
        }

        private void PopulatePlanets()
        {
            planets = new List<Planet>();

            for (int i = 0; i < planetCount; i++)
            {
                AddPlanet();
            }
        }

        private void AddPlanet()
        {
            var planet = PlanetFactory.GetPlanet();
            while(planets.Any(p => p.PlanetBody.Position.Magnitude(planet.PlanetBody.Position) < (p.Radius + planet.Radius + 40)))
            {
                planet.PlanetBody.Position = PlanetFactory.GetOnScreenPosition(planet.PlanetBody.Radius);
            }

            planets.Add(planet);
        }

        private void MouseReleased(object sender, MouseButtonEventArgs e)
        {
            isThrowing = false;
            LaunchProjectile();
        }

        private void LaunchProjectile()
        {
            var delta = throwAnchor - GetMousePosition();
            projectile.Velocity = delta.Value;
        }

        private void MousePressed(object sender, MouseButtonEventArgs e)
        {
            isThrowing = true;
            elastic[0] = new Vertex(throwAnchor.Value, Color.White);
            projectile.Velocity = null;
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if(e.Code == Keyboard.Key.N)
            {
                Task.Run(() =>
                {
                    PopulatePlanets();
                });
            }
        }

        /// <summary>
        /// Update - Here we add all our logic for updating components in this screen. 
        /// This includes checking for user input, updating the position of moving objects and more!
        /// </summary>
        /// <param name="deltaT">The amount of time that has passed since the last frame was drawn.</param>
        public override void Update(float deltaT)
        {
            if (isThrowing)
            {
                projectile.ProjectileBody.Position = GetMousePosition();
                elastic[1] = new Vertex(projectile.ProjectileBody.Position, Color.White);
                SetProjections();
            }

            if (projectile.Velocity != null)
            {
                projectile.Velocity += GetNewVelocity(projectile.ProjectileBody.Position, projectile.Mass, deltaT);
                projectile.ProjectileBody.Position += projectile.Velocity.Value * deltaT;
            }

            goal.Update(deltaT);

            CheckGoal();

            if (!CheckPlanetCollisions(projectile))
            {
                projectile.Velocity = new Vector2f(0, 0);
                projectile.ProjectileBody.Position = new Vector2f(-50, -50);
            }
        }

        private bool CheckPlanetCollisions(Projectile projectile)
        {
            foreach(var planet in planets)
            {
                var distance = planet.PlanetBody.Position.Magnitude(projectile.ProjectileBody.Position);

                if (distance < planet.Radius + projectile.ProjectileBody.Radius)
                {
                    return false;
                }
            }

            return true;
        }

        private void SetProjections()
        {
            projections.Clear();

            var projectileClone = new Projectile(
                new CircleShape(projectile.ProjectileBody.Radius)
                {
                    Position = projectile.ProjectileBody.Position,
                    Origin = new Vector2f(30, 30)
                },
                null)
            { Velocity = projectile.Velocity };


            var delta = throwAnchor - GetMousePosition();
            projectileClone.Velocity = delta.Value;

            for (int i = 0; i < 8; i++)
            {
                float time = 0;
                float timeStep = 1 / 60f;
                while (time < 0.4)
                {
                    var newVelocity = GetNewVelocity(projectileClone.ProjectileBody.Position, projectileClone.Mass, timeStep);
                    projectileClone.Velocity += newVelocity;
                    projectileClone.ProjectileBody.Position += projectileClone.Velocity.Value * timeStep;
                    time += timeStep;

                    if (!CheckPlanetCollisions(projectileClone))
                    {
                        return;
                    }
                }
                    
                projections.Add(projectileClone.ProjectileBody.Position);
            }
        }

        private void CheckGoal()
        {
            if (projectile.ProjectileBody.Position.Magnitude(goal.Position) < 30)
            {
                PopulatePlanets();

                SetNewGoal();
            }
        }

        private void SetNewGoal()
        {
            projectile.Velocity = null; 

            goal = new Goal(PlanetFactory.GetOnScreenPosition(30), 30);

            while (planets.Any(p => p.PlanetBody.Position.Magnitude(goal.Position) < (p.Radius + goal.Radius + 100)))
            {
                goal = new Goal(PlanetFactory.GetOnScreenPosition(30), 30);
            }

            SetNewStart();
        }

        private void SetNewStart()
        {
            projectile.ProjectileBody.Position = PlanetFactory.GetOnScreenPosition(projectile.ProjectileBody.Radius);

            while (planets.Any(p => p.PlanetBody.Position.Magnitude(goal.Position) < (p.Radius + goal.Radius + 100)) 
                || projectile.ProjectileBody.Position.Magnitude(goal.Position) < Configuration.Width / 2)
            {
                projectile.ProjectileBody.Position = PlanetFactory.GetOnScreenPosition(projectile.ProjectileBody.Radius);
            }

            throwAnchor = projectile.ProjectileBody.Position;
        }

        private Vector2f GetNewVelocity(Vector2f position, float mass, float deltaT)
        {
            var deltaV = new Vector2f();

            foreach(var planet in planets)
            {
                float force = (float)(0.0000000000667 * (planet.Mass * 100)) / planet.Position.Magnitude(position);

                var directionVector = (planet.Position - position).Normalize();
                deltaV += force * deltaT * directionVector * 30000000;
            }

            return deltaV;
        }

        /// <summary>
        /// Draw - Here we don't update any of the components, only draw them in their current state to the screen.
        /// </summary>
        /// <param name="deltaT">The amount of time that has passed since the last frame was drawn.</param>
        public override void Draw(float deltaT)
        {
            window.Clear(Color.Black);
            texture.Clear(Color.Black);
            texture.Draw(background);
            foreach (var planet in planets.ToList())
            {
                texture.Draw(planet.PlanetBody);
            }

            foreach (var entity in entities)
            {
                texture.Draw(entity);
            }

            goal.Draw(texture);

            if(projectile != null)
            {
                texture.Draw(projectile.ProjectileBody);
                if (isThrowing)
                {
                    var anchorCentre = new CircleShape(10)
                    {
                        Position = throwAnchor.Value,
                        Origin = new Vector2f(10, 10),
                        FillColor = Color.White
                    };

                    var outerSize = throwAnchor.Value.Magnitude(GetMousePosition()) / 4;
                    outerSize = outerSize == 0 ? 1 : outerSize;
                    var colour = new Color(255, (byte)(255 - outerSize), (byte)(255 - outerSize));
                    var anchorOuter = new CircleShape(outerSize)
                    {
                        Position = throwAnchor.Value,
                        Origin = new Vector2f(outerSize, outerSize),
                        FillColor = Color.Transparent,
                        OutlineThickness = outerSize / 20,
                        OutlineColor = colour
                    };


                    texture.Draw(anchorCentre);
                    texture.Draw(anchorOuter);
                }
            }

            if (isThrowing)
            {
                int position = 0;
                foreach(var projection in projections)
                {
                    texture.Draw(new CircleShape(10 - position) { Position = projection });
                    position++;
                }
            }

            frame++;
        }

        public Vector2f GetMousePosition()
        {
            var position = Mouse.GetPosition();

            var adjustedPosition = position - window.Position;

            return new Vector2f(adjustedPosition.X - TitleBarSize.X, adjustedPosition.Y - TitleBarSize.Y);
        }

        public void AddVisual(Drawable visual)
        {
            entities.Add(visual);
        }
    }
}