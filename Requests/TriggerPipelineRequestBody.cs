using models;

namespace requests
{
    public class TriggerPipelineRequestBody
    {
        public VNetEnvironment Environment { get; set; }
        public string Name { get; set; }

    }
}
