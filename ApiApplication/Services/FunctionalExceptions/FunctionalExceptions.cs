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
        
        public NotEnoughSeatsAvailableException() : base("Not enough seats are available for your reservation request")
        {
        }
        
        public NotEnoughSeatsAvailableException(string? message): base(message){}

    }
    
    public class SeatsReservationExpiredException : CustomException
    {
        public override string Code { get; set; } = "SeatsReservationExpired";

        public SeatsReservationExpiredException():base("The corresponding reservation has expired")
        {
        }
        
        public SeatsReservationExpiredException(string? message): base(message){}

    }
    
    public class TicketAlreadyPaidException : CustomException
    {
        public override string Code { get; set; } = "TicketAlreadyPaid";
        
        public TicketAlreadyPaidException():base("The corresponding ticket has already been paid")
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


        public ShowtimeCreationException():base("Failed to create showtime")
        {
            
        }
        
        public ShowtimeCreationException(string? message): base(message){}

    }
    
    public class MoviesServiceNotAvailableException : CustomException
    {
        public override string Code { get; set; } = "MoviesServiceNotAvailable";

        public MoviesServiceNotAvailableException():base("The movies service is not available for now")
        {
        }
        
        public MoviesServiceNotAvailableException(string? message): base(message){}

    }
    
}