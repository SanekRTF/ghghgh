using System.Collections.Generic;

namespace rocket_bot
{
    public class Channel<T> where T : class
    {
        private List<T> items = new(); // Переименование channelList на items

        public T this[int index]
        {
            get
            {
                lock (items) // Изменение с channelList на items
                {
                    if (index >= items.Count || index < 0)
                        return null;
                    return items[index];
                }
            }
            set
            {
                lock (items) // Изменение с channelList на items
                {
                    if (index == items.Count)
                        items.Add(value);
                    else
                    {
                        items[index] = value;
                        while (items.Count - 1 > index)
                            items.RemoveAt(items.Count - 1);
                    }
                }
            }
        }

        public T LastItem()
        {
            lock (items) // Изменение с channelList на items
            {
                if (items.Count == 0)
                    return null;
                return items[^1];
            }
        }

        public void AppendIfLastItemIsUnchanged(T newItem, T knownLastItem) // Переименование item на newItem
        {
            lock (items) // Изменение с channelList на items
            {
                if (items.Count == 0 || items[^1] == knownLastItem)
                    items.Add(newItem); // Изменение с item на newItem
            }
        }

        public int Count
        {
            get
            {
                lock (items) // Изменение с channelList на items
                {
                    return items.Count;
                }
            }
        }
    }
}