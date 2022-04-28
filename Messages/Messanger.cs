using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobViewer.Messages
{
    public class Messanger
    {
        Dictionary<Type, List<Action<IMessage>>> recievers = new Dictionary<Type, List<Action<IMessage>>>();   
        
        public void Send(IMessage message)
        {
            foreach (var actions in recievers.Where(x => x.Key == message.GetType()).Select(x => x.Value))
            {
                foreach (var action in actions)
                    try
                    {
                        action.Invoke(message);
                    }
                    catch (Exception ex)
                    { 
                    }
            }
        }

        public void Subscribe(Type type, Action<IMessage> action)
        {
            if (!recievers.ContainsKey(type))
                recievers[type] = new List<Action<IMessage>>();

            recievers[type].Add(action);
        }

        public void Unsubscribe(Type type, Action<IMessage> action)
        {
            if (!recievers.ContainsKey(type))
                return;

            recievers[type].Remove(action);
        }
    }
}
