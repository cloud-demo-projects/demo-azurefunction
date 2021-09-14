
namespace requests
{
    public class TriggerRequest<T>
    {
        public T TriggerData { get; set; }
        public string TriggerReason { get; set; }
        public bool IsValidation { get; set; }

    }
}
