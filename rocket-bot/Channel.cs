using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace rocket_bot
{
    public class Channel<T> where T : class
    {
        private List<T> items = new List<T>();

        /// <summary>
        /// Возвращает элемент по индексу или null, если такого элемента нет.
        /// При присвоении удаляет все элементы после.
        /// Если индекс в точности равен размеру коллекции, работает как Append.
        /// </summary>
        public T this[int index]
        {
            get
            {
                lock (items)
                {
                    return index < items.Count ? items[index] : null;
                }
            }
            set
            {
                lock (items)
                {
                    if (index < items.Count - 1)
                    {
                        items.RemoveRange(index + 1, items.Count - index - 1);
                        items[index] = value;
                    }
                    else if (index == items.Count - 1)
                    {
                        items[index] = value;
                    }
                    else if (index == items.Count)
                    {
                        items.Add(value);
                    }
                }
            }
        }

        /// <summary>
        /// Возвращает последний элемент или null, если такого элемента нет
        /// </summary>
        public T LastItem()
        {
            lock (items)
            {
                return items.LastOrDefault();
            }
        }

        /// <summary>
        /// Добавляет item в конец только если lastItem является последним элементом
        /// </summary>
        public void AppendIfLastItemIsUnchanged(T item, T knownLastItem)
        {
            lock (items)
            {
                if (knownLastItem == LastItem())
                {
                    items.Add(item);
                }
            }
        }

        /// <summary>
        /// Возвращает количество элементов в коллекции
        /// </summary>
        public int Count => items.Count;
    }
}