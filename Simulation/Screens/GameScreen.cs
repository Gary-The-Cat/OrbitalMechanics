using CarSimulation;
using CarSimulation.Screens;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections.Generic;

namespace Arkanoid_SFML.Screens
{
    public class GameScreen : Screen
    {
        internal RenderTexture texture;

        internal int frame;

        readonly Vector2f TitleBarSize = new Vector2f(12, 57);

        List<Drawable> entities;

        public Vertex[] background;

        public GameScreen(
            RenderWindow window,
            FloatRect configuration)
            : base(window, configuration)
        {
            frame = 0;
            texture = new RenderTexture(Configuration.Width, Configuration.Height);

            entities = new List<Drawable>();
        }

        /// <summary>
        /// Update - Here we add all our logic for updating components in this screen. 
        /// This includes checking for user input, updating the position of moving objects and more!
        /// </summary>
        /// <param name="deltaT">The amount of time that has passed since the last frame was drawn.</param>
        public override void Update(float deltaT)
        {
            
        }

        /// <summary>
        /// Draw - Here we don't update any of the components, only draw them in their current state to the screen.
        /// </summary>
        /// <param name="deltaT">The amount of time that has passed since the last frame was drawn.</param>
        public override void Draw(float deltaT)
        {
            window.Clear();
            texture.Clear();

            texture.Draw(background, 0, 4, PrimitiveType.Quads);

            foreach (var entity in entities)
            {
                texture.Draw(entity);
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
