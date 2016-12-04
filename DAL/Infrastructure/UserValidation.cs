namespace DAL.Infrastructure
{
    using System;
    using Entities;
    using Interfaces;

    public class UserValidation:IValidator
    {

        public bool Validate(User user)
        {
            if (ReferenceEquals(user, null))
            {
                throw new ArgumentNullException();
            }
            if (string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.LastName))
            {
                return false;
            }
            return true;
        }
    }
}
