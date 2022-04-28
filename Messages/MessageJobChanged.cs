using JobViewer.Model;

namespace JobViewer.Messages
{
    internal class MessageJobChanged : IMessage
    {
        public Job NewJob { get; }

        public MessageJobChanged(Job job)
            => NewJob = job;
    }
}
