namespace AbySalto.Junior.Exceptions
{
    public class OrderNotFoundException : Exception
    {
        public OrderNotFoundException(int id) : base($"Order with ID {id} was not found.")
        {
        }
    }
}
