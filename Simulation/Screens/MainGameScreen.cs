using SFML.Graphics;
using SFML.Window;

namespace Arkanoid_SFML.Screens
{
    public class MainGameScreen : GameScreen
    {
        public MainGameScreen(RenderWindow window, FloatRect configuration) : base(window, configuration)
        {
            window.MouseButtonPressed += MouseButtonPressed;
            window.MouseButtonPressed += MouseButtonReleased;
        }

        public override void Update(float deltaT)
        {
            base.Update(deltaT);
        }

        public override void Draw(float deltaT)
        {
            base.Draw(deltaT);

            texture.Display();

            var sprite = new Sprite(texture.Texture);

            window.Draw(sprite);
        }

        private void MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = GetMousePosition();
        }

        private void MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = GetMousePosition();
        }
    }
}
