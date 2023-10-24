using Shared;

namespace backend.Models.Entity.GameBoardExtensions
{
    public class ThemeAbstraction
    {
        protected ThemeImplementor implementor;

        public ThemeAbstraction()
        {
            implementor = new ConcreteImplementorLight();
        }

        public ThemeImplementor Implementor
        {
            get { return implementor; }
            set { implementor = value; }
        }

        public virtual Color Background()
        {
            return implementor.Background();
        }
        public virtual string Text()
        {
            return implementor.Text();
        }
        public virtual Color TextColor()
        {
            return implementor.TextColor();
        }
        public virtual Color ButtonBackgroundColor()
        {
            return implementor.ButtonBackgroundColor();
        }
    }
}
