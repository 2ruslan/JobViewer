using JobViewer.Model;

namespace JobViewer.Messages
{
    internal class MessageStepChanged : IMessage
    {
        public JobStep NewStep { get; }

        public MessageStepChanged(JobStep step)
            => NewStep = step;
    }
}
