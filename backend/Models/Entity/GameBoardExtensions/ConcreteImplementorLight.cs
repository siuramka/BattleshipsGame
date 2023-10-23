using Shared;

namespace backend.Models.Entity.GameBoardExtensions
{
    public class ConcreteImplementorLight : ThemeImplementor
    {
        public override Color Background()
        {
            return Color.White;
        }

        public override string Text()
        {
            return "turn dark theme";
        }
    }
}
