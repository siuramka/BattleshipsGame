using Shared;

namespace backend.Models.Entity.GameBoardExtensions
{
    public class ConcreteImplementorDark : ThemeImplementor
    {
        public override Color Background()
        {
            return Color.Black;
        }

        public override string Text()
        {
            return "turn light theme";
        }
    }
}
