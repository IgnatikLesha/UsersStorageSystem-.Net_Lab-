namespace DAL.Infrastructure
{
    using System;
    using System.Net.Sockets;
    using System.Text;

    [Serializable]
    public class ActionEventArgs : EventArgs
    {
        private string msg;

        public string Message
        { 
            get
            {
                return msg;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                msg = value;

            } 
        }

    }
}