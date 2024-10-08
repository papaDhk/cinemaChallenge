﻿using ApiApplication.Database.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace ApiApplication.Database
{
    public class SampleData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<CinemaContext>();
            context.Database.EnsureCreated();
            

            context.Auditoriums.Add(new AuditoriumEntity
            {
                Showtimes = new List<ShowtimeEntity> 
                { 
                    new ShowtimeEntity
                    {
                        SessionDate = new DateTime(2023, 1, 1),
                        Movie = new MovieEntity
                        {
                            Title = "Inception",
                            ImdbId = "tt1375666",
                            ReleaseDate = new DateTime(2010, 01, 14),
                            Stars = "Leonardo DiCaprio, Joseph Gordon-Levitt, Ellen Page, Ken Watanabe"                            
                        },
                        AuditoriumId = 1,
                    } 
                },
                Seats = GenerateSeats(1, 28, 22)
            });

            context.Auditoriums.Add(new AuditoriumEntity
            {
                Seats = GenerateSeats(2, 21, 18)
            });

            context.Auditoriums.Add(new AuditoriumEntity
            {
                Seats = GenerateSeats(3, 15, 21)
            });

            context.SaveChanges();
        }

        public static List<SeatEntity> GenerateSeats(int auditoriumId, short rows, short seatsPerRow)
        {
            var seats = new List<SeatEntity>();
            for (short r = 1; r <= rows; r++)
                for (short s = 1; s <= seatsPerRow; s++)
                    seats.Add(new SeatEntity { AuditoriumId = auditoriumId, Row = r, SeatNumber = s });

            return seats;
        }
    }
}
