using System;

namespace ApiApplication.Services
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string? message): base(message){}

    }
    
    public class NotEnoughSeatsAvailableException : Exception
    {
        public NotEnoughSeatsAvailableException()
        {
            
        }
        
        public NotEnoughSeatsAvailableException(string? message): base(message){}

    }
    
    public class SeatsReservationExpiredException : Exception
    {
        public SeatsReservationExpiredException()
        {
            
        }
        
        public SeatsReservationExpiredException(string? message): base(message){}

    }
    
    public class TicketAlreadyPaidException : Exception
    {
        public TicketAlreadyPaidException()
        {
            
        }
        
        public TicketAlreadyPaidException(string? message): base(message){}

    }
    
}