namespace Task6
{

    class Node<T>
    {
        public T Value { get; set; }
        public Node<T>? Next { get; set; }
        public Node(T value)
        {
            Value = value;
            Next = null;
        }
    }

    internal class LockFreeStack<T>
    {
        private Node<T>? _head;

        public void Push(T item)
        {
            Node<T>? newNode = new Node<T>(item);
            Node<T>? currentHead;

            do
            {
                currentHead = _head;
                newNode.Next = currentHead;
            }
            while (Interlocked.CompareExchange(ref _head, newNode, currentHead) != currentHead);
        }

        public bool TryPop(out T? item)
        {
            Node<T>? currentHead;
            Node<T>? nextNode;

            do
            {
                currentHead = _head;
                if (currentHead == null)
                {
                    item = default;
                    return false;
                }
                nextNode = currentHead.Next;

            }
            while (Interlocked.CompareExchange(ref _head, nextNode, currentHead) != currentHead);

            item = currentHead.Value;
            return true;
        }

        public bool TryPeek(out T result)
        {
            Node<T>? currentHead = _head;

            if (currentHead == null)
            {
                result = default;
                return false;
            }

            result = currentHead.Value;
            return true;
        }

        public bool IsEmpty()
        {
            Node<T>? currentHead = _head;
            return currentHead == null;
        }

        public void Clear()
        {
            Interlocked.Exchange(ref _head, null);
        }
    }
}
