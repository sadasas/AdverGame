namespace AdverGame.Player
{
    public class ItemSerializable
    {
        public ItemContent Content;
        public int Stack { get; private set; } = 1;

        public ItemSerializable(ItemContent content)
        {
            Content = content;
        }
      
        public void UpdateStack(int stack)
        {
            Stack += stack;
        }

    }


}
