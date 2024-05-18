using System;

namespace ApiApplication.Services
{
    
    public class CustomException : Exception
    {
        public virtual string Code { get; set; }
        public CustomException(string? message): base(message)
        {
        }

        protected CustomException()
        {
        }
    }
    public class NotFoundException : CustomException
    {
        public override string Code { get; set; } = "ResourceNotFound";
        public NotFoundException(string? message) : base(message)
        {
        }

    }
    
    public class NotEnoughSeatsAvailableException : CustomException
    {
        public override string Code { get; set; } = "NotEnoughSeatsAvailable";
        public override string Message { get; } = "Not enough seats are available for your reservation request";

        
        public NotEnoughSeatsAvailableException() : base()
        {
        }
        
        public NotEnoughSeatsAvailableException(string? message): base(message){}

    }
    
    public class SeatsReservationExpiredException : CustomException
    {
        public override string Code { get; set; } = "SeatsReservationExpired";
        public override string Message { get; } = "The corresponding reservation has expired";

        public SeatsReservationExpiredException()
        {
        }
        
        public SeatsReservationExpiredException(string? message): base(message){}

    }
    
    public class TicketAlreadyPaidException : CustomException
    {
        public override string Code { get; set; } = "TicketAlreadyPaid";
        public override string Message { get; } = "The ticket corresponding ticket has already been paid ";


        public TicketAlreadyPaidException()
        {
            
        }
        
        public TicketAlreadyPaidException(string? message): base(message){}

    }
    
    public class AuditoriumNotAvailableException : CustomException
    {
        public override string Code { get; set; } = "AuditoriumNotAvailable";


        public AuditoriumNotAvailableException(): base("The auditorium is not available for showtime")
        {
        }
        
        public AuditoriumNotAvailableException(string? message): base(message){}

    }
    
    public class ShowtimeCreationException : CustomException
    {
        public override string Code { get; set; } = "ShowtimeCreationError";
        public override string Message { get; } = "Failed to create showtime";


        public ShowtimeCreationException()
        {
            
        }
        
        public ShowtimeCreationException(string? message): base(message){}

    }
    
}