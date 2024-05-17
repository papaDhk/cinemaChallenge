using System;

namespace ApiApplication.Services
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
            
        }
        
        public NotFoundException(string? message): base(message){}

    }
    
    public class NotEnoughSeatsAvailableException : Exception
    {
        public NotEnoughSeatsAvailableException()
        {
            
        }
        
        public NotEnoughSeatsAvailableException(string? message): base(message){}

    }
    
}