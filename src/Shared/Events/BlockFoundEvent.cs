namespace Cinder.Events
{
    public class BlockFoundEvent
    {
        public ulong Number { get; set; }

        public static BlockFoundEvent Create(ulong number)
        {
            return new BlockFoundEvent {Number = number};
        }
    }
}
