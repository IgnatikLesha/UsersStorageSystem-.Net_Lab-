namespace DAL.Infrastructure
{
    using System;
    using DAL.Infrastructure;

    [Serializable]
    public class ServiceComunicator
    {
        public event EventHandler<ActionEventArgs> Message;

        private string msg;

        public void Send(ActionEventArgs arg)
        {
            msg = arg.Message;
        }

        public string GetMessage()
        {
            if (msg == null)
            {
                return "No action";
            }

            return msg;
        }
    }
}