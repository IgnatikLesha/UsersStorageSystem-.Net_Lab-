namespace DAL.Interfaces
{
    using Entities;
    public interface IValidator
    {
       bool Validate(User user);
    }
}
