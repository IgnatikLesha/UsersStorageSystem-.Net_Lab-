namespace DAL.Infrastructure
{
    using System;

    [Serializable]
    public class IdIterator
    {
        public static bool IsPrime(int number)
        {
            if (number == 2)
            {
                return true;
            }
            if (number == 0 || number % 2 == 0)
            {
                return false;
            }
            for (int i = 3; i < number; i += 2)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }

            return true;
        }
        

        public static int GetNextId(int prevId)
        {
            if (prevId < 0)
            {
                throw new ArgumentException();
            }

            int nextId = prevId + 1;

            while (!IsPrime(nextId))
            {
                nextId++;
            }
            return nextId;
        }
    }
}
